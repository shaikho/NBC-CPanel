using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AljazeeraCPanel
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // WAPT10: suppress the X-AspNetMvc-Version response header.
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // WAPT10 (Server Header Disclosure) — belt-and-suspenders header stripping.
        // Runs for EVERY response in the Integrated pipeline (dynamic pages, static
        // files, error responses), so it covers cases the Web.config settings and the
        // MVC/ASP.NET flags may miss, and works regardless of IIS version.
        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            try
            {
                var response = HttpContext.Current != null ? HttpContext.Current.Response : null;
                if (response == null) return;

                response.Headers.Remove("Server");
                response.Headers.Remove("X-Powered-By");
                response.Headers.Remove("X-AspNet-Version");
                response.Headers.Remove("X-AspNetMvc-Version");
            }
            catch
            {
                // Some hosting configurations disallow header edits here; the Web.config
                // settings remain the primary control, so never fail a request over this.
            }
        }
    }
}
