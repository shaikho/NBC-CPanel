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
        }
    }
}
