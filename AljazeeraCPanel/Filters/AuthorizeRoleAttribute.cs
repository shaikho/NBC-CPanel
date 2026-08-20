using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace AljazeeraCPanel.Filters
{
    /// <summary>
    /// Custom role-based authorization filter to enforce role-based access control (RBAC).
    /// Complies with WAPT03-01 requirement to create centralized [AuthorizeRole] filter.
    /// 
    /// This filter validates that the authenticated user's role matches one of the allowed roles
    /// before permitting access to the decorated controller/action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Allowed roles. Can be a comma-separated list (e.g., "Admin,Manager,Officer").
        /// Role IDs are compared as integers parsed from comma-separated values.
        /// </summary>
        public string AllowedRoles { get; set; }

        /// <summary>
        /// If AllowedRoles is left empty and this flag is true, only the controller-level role check applies.
        /// If false (default), an empty AllowedRoles at method level is ignored (inherits from class).
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// Constructor to allow role specification via attribute.
        /// </summary>
        /// <param name="allowedRoles">Comma-separated list of allowed role IDs (e.g., "1,2,3") or role names if supported</param>
        public AuthorizeRoleAttribute(string allowedRoles = "")
        {
            AllowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            // Get session from context
            var session = filterContext.HttpContext.Session;

            // Validate that session exists and user is authenticated
            if (!IsUserAuthenticated(session))
            {
                // Not authenticated; redirect to login
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login"
                    }));

                base.OnActionExecuting(filterContext);
                return;
            }

            // Skip role check if no roles are specified (AllowedRoles is empty or null)
            // This allows the attribute to be used without role restriction
            if (string.IsNullOrWhiteSpace(AllowedRoles))
            {
                // No specific roles required; session validation was sufficient
                base.OnActionExecuting(filterContext);
                return;
            }

            // Extract user's role from session
            string userRoleId = GetUserRoleId(session);

            // Validate user's role against allowed roles
            if (!IsUserInAllowedRole(userRoleId, AllowedRoles))
            {
                // User is authenticated but not in allowed role; return 403 Forbidden
                filterContext.Result = new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

                // Optionally: Redirect to unauthorized page
                // filterContext.Result = new RedirectToRouteResult(
                //     new RouteValueDictionary(new
                //     {
                //         controller = "Unauthorised",
                //         action = "Index"
                //     }));

                base.OnActionExecuting(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Validates that the user has an authenticated session.
        /// </summary>
        private bool IsUserAuthenticated(System.Web.HttpSessionStateBase session)
        {
            if (session == null)
                return false;

            // Check for primary auth flag
            if (session["cpanelLogin"] == null)
                return false;

            string cpanelLogin = session["cpanelLogin"].ToString();
            if (!cpanelLogin.Equals("true", StringComparison.OrdinalIgnoreCase) &&
                !cpanelLogin.Equals("changepass", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Verify critical session variables
            if (session["user_log"] == null || string.IsNullOrEmpty(session["user_log"].ToString()))
                return false;

            if (session["UserId"] == null || string.IsNullOrEmpty(session["UserId"].ToString()))
                return false;

            if (session["user_roleid"] == null || string.IsNullOrEmpty(session["user_roleid"].ToString()))
                return false;

            return true;
        }

        /// <summary>
        /// Extracts the user's role ID from session.
        /// </summary>
        private string GetUserRoleId(System.Web.HttpSessionStateBase session)
        {
            try
            {
                return session["user_roleid"]?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Validates that the user's role is in the list of allowed roles.
        /// Supports comma-separated role IDs.
        /// </summary>
        /// <param name="userRoleId">The role ID from the user's session</param>
        /// <param name="allowedRoles">Comma-separated list of allowed role IDs (e.g., "1,2,3")</param>
        /// <returns>True if user's role is in the allowed list; false otherwise</returns>
        private bool IsUserInAllowedRole(string userRoleId, string allowedRoles)
        {
            // Handle nullable/empty cases
            if (string.IsNullOrWhiteSpace(userRoleId) || string.IsNullOrWhiteSpace(allowedRoles))
                return false;

            try
            {
                // Split allowed roles and trim whitespace
                var allowedRoleArray = allowedRoles
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToArray();

                // Check if user's role is in the allowed list
                return allowedRoleArray.Contains(userRoleId, StringComparer.Ordinal);
            }
            catch
            {
                // Error parsing roles; deny access by default
                return false;
            }
        }
    }
}
