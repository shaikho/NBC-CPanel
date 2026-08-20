using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AljazeeraCPanel.Filters
{
    /// <summary>
    /// Custom authorization filter to enforce authenticated session state.
    /// Centralizes session validation and prevents manual scattered checks.
    /// Complies with WAPT02-01 session fixation and auth hardening requirements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// List of controller names that bypass authentication (e.g., Login).
        /// </summary>
        private static readonly string[] PublicControllers = { "Login" };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];

            // Allow public access to Login controller
            if (Array.Exists(PublicControllers, element => element.Equals(controllerName, StringComparison.OrdinalIgnoreCase)))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Validate authenticated session state
            if (!IsValidSession(filterContext.HttpContext.Session))
            {
                // Redirect to login on invalid session
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Login"
                    }));

                base.OnActionExecuting(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// Validates that session contains required authenticated state.
        /// </summary>
        private bool IsValidSession(System.Web.HttpSessionStateBase session)
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

            // Verify required session variables are present
            if (session["user_log"] == null || string.IsNullOrEmpty(session["user_log"].ToString()))
                return false;

            if (session["UserId"] == null || string.IsNullOrEmpty(session["UserId"].ToString()))
                return false;

            if (session["user_name"] == null || string.IsNullOrEmpty(session["user_name"].ToString()))
                return false;

            if (session["user_branch"] == null || string.IsNullOrEmpty(session["user_branch"].ToString()))
                return false;

            if (session["user_roleid"] == null || string.IsNullOrEmpty(session["user_roleid"].ToString()))
                return false;

            return true;
        }
    }
}
