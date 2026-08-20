using System.Web;
using System.Web.Mvc;
using AljazeeraCPanel.Filters;

namespace AljazeeraCPanel
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            // Add centralized session/auth guard for WAPT02-01 compliance
            filters.Add(new AuthorizeSessionAttribute());
            // WAPT03-02: server-side role/menu authorization (Broken Access Control, A01:2025).
            // Registered globally so every controller is covered; runs after the session
            // guard. Behaviour (LogOnly vs Enforce) is controlled by the app setting
            // "Authorization.EnforcementMode" in Web.config.
            filters.Add(new AuthorizePermissionAttribute());
        }
    }
}
