using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AljazeeraCPanel.Repository;

namespace AljazeeraCPanel.Filters
{
    /// <summary>
    /// WAPT03-02 — Server-side Broken Access Control enforcement (A01:2025).
    ///
    /// The application previously enforced role scope only in the UI (the sidebar was
    /// filtered from JSB_ROLE_MENU_MAPPING, but requests were never re-checked, so any
    /// authenticated user could reach another role's pages by typing the URL). This
    /// filter re-checks every request against that SAME mapping table on the server.
    ///
    /// Model:
    ///   * "Role-scoped" controllers = every controller that appears in JSB_MENU_MASTER.
    ///     These are guarded: a role may reach one only if the mapping grants it.
    ///   * Any other controller (dashboard, own-profile, config sub-pages, error pages)
    ///     is a supporting page governed by session auth only (see AlwaysAllow + the
    ///     "not in menu model" fall-through).
    ///   * A permitted controller's sub-actions (e.g. User/Reject reached from
    ///     User/Users) are allowed by controller membership, since the menu maps only
    ///     landing actions.
    ///
    /// Rollout: controlled by the app setting "Authorization.EnforcementMode".
    ///   * "LogOnly" (default) — denials are recorded to Trace but the request proceeds.
    ///     Use during UAT to confirm the mapping is complete with zero user impact.
    ///   * "Enforce" — denials return HTTP 403 Forbidden.
    /// Flip one web.config setting to switch; no recompile required.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizePermissionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Controllers always reachable by any authenticated user, regardless of the
        /// menu mapping (framework/support pages, not part of the role-scoped menu).
        /// </summary>
        private static readonly HashSet<string> AlwaysAllow = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Login",          // public (also handled by AuthorizeSession)
            "Home",           // dashboard shell + menu build
            "Account",        // personalized menu partial
            "Profile",        // own profile
            "Changepass",     // own password change (own session state logic)
            "BarChart",       // dashboard widgets
            "Unauthorised",   // access-denied page
            "Error",          // error page
            "Empty",
            "Default1"
        };

        /// <summary>
        /// Sensitive controllers that are NOT part of the menu model but perform
        /// high-impact operations (retrieve customer passwords, hard-delete customers,
        /// system monitoring). Because they are not menu-mapped they would otherwise
        /// fall through to "any authenticated user"; instead they are restricted to the
        /// Admin role. Adjust the role list here if these need to be delegated.
        /// </summary>
        private static readonly HashSet<string> AdminOnlyControllers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "getpassword",     // retrieves / SMS customer passwords
            "DeleteCustomer",  // hard-deletes customers
            "Monitoring"       // system monitoring
        };

        /// <summary>Role IDs permitted to reach the AdminOnlyControllers set.</summary>
        private static readonly HashSet<string> AdminRoleIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "1"                // Admin
        };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var session = filterContext.HttpContext != null ? filterContext.HttpContext.Session : null;

            // 1) Must be authenticated. (AuthorizeSession normally runs first; this is defense in depth.)
            if (!IsAuthenticated(session))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Login", action = "Login" }));
                base.OnActionExecuting(filterContext);
                return;
            }

            // 2) Support pages: session auth is sufficient.
            if (controllerName != null && AlwaysAllow.Contains(controllerName))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // 2b) Sensitive non-menu controllers: Admin role only.
            if (controllerName != null && AdminOnlyControllers.Contains(controllerName))
            {
                string roleId0 = session["user_roleid"] != null ? session["user_roleid"].ToString() : "";
                if (AdminRoleIds.Contains(roleId0.Trim()))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                string user0 = session["user_log"] != null ? session["user_log"].ToString() : "?";
                if (IsEnforcing())
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "[WAPT03-02][ENFORCE] Denied (admin-only) user='{0}' role='{1}' -> {2}/{3}",
                        user0, roleId0, controllerName, actionName);
                    filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                    base.OnActionExecuting(filterContext);
                    return;
                }
                System.Diagnostics.Trace.TraceWarning(
                    "[WAPT03-02][LOGONLY] Would deny (admin-only) user='{0}' role='{1}' -> {2}/{3}",
                    user0, roleId0, controllerName, actionName);
                base.OnActionExecuting(filterContext);
                return;
            }

            HashSet<string> menuControllers;
            HashSet<string> permittedControllers;
            HashSet<string> permittedUrls;
            try
            {
                menuControllers = PermissionData.GetAllMenuControllers();
                permittedControllers = GetSessionPermittedControllers(session);
                permittedUrls = GetSessionPermittedUrls(session);
            }
            catch (Exception ex)
            {
                // Fail-open on data errors so an auth-store outage can't lock everyone out,
                // but make it loud. (WAPT: never silently swallow.)
                System.Diagnostics.Trace.TraceError(
                    "[WAPT03-02] Permission load failed for {0}/{1}: {2}",
                    controllerName, actionName, ex);
                base.OnActionExecuting(filterContext);
                return;
            }

            // 3) Controller not part of the role-scoped menu model → session-only page.
            if (controllerName == null || !menuControllers.Contains(controllerName))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // 4) Role-scoped controller: allow on exact action mapping or controller membership.
            string key = PermissionData.BuildKey(controllerName, actionName);
            bool allowed =
                (key != null && permittedUrls.Contains(key)) ||
                permittedControllers.Contains(controllerName);

            if (allowed)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // 5) Denied.
            string roleId = session["user_roleid"] != null ? session["user_roleid"].ToString() : "?";
            string user = session["user_log"] != null ? session["user_log"].ToString() : "?";

            if (IsEnforcing())
            {
                System.Diagnostics.Trace.TraceWarning(
                    "[WAPT03-02][ENFORCE] Denied user='{0}' role='{1}' -> {2}/{3}",
                    user, roleId, controllerName, actionName);
                filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
                base.OnActionExecuting(filterContext);
                return;
            }

            // LogOnly: record what WOULD be blocked, then let it through.
            System.Diagnostics.Trace.TraceWarning(
                "[WAPT03-02][LOGONLY] Would deny user='{0}' role='{1}' -> {2}/{3}",
                user, roleId, controllerName, actionName);
            base.OnActionExecuting(filterContext);
        }

        private static bool IsEnforcing()
        {
            string mode = ConfigurationManager.AppSettings["Authorization.EnforcementMode"];
            return !string.IsNullOrWhiteSpace(mode) &&
                   mode.Trim().Equals("Enforce", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAuthenticated(HttpSessionStateBase session)
        {
            if (session == null || session["cpanelLogin"] == null)
                return false;
            string flag = session["cpanelLogin"].ToString();
            if (!flag.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                !flag.Equals("changepass", StringComparison.OrdinalIgnoreCase))
                return false;
            return session["user_roleid"] != null &&
                   !string.IsNullOrEmpty(session["user_roleid"].ToString());
        }

        // Per-session cache so the mapping is read from the DB once per login, not per request.
        private static HashSet<string> GetSessionPermittedControllers(HttpSessionStateBase session)
        {
            var cached = session["perm_controllers"] as HashSet<string>;
            if (cached != null)
                return cached;
            string roleId = session["user_roleid"].ToString();
            var set = PermissionData.GetPermittedControllers(roleId);
            session["perm_controllers"] = set;
            return set;
        }

        private static HashSet<string> GetSessionPermittedUrls(HttpSessionStateBase session)
        {
            var cached = session["perm_urls"] as HashSet<string>;
            if (cached != null)
                return cached;
            string roleId = session["user_roleid"].ToString();
            var set = PermissionData.GetPermittedUrls(roleId);
            session["perm_urls"] = set;
            return set;
        }
    }
}
