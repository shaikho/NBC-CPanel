using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Controllers
{
    public class UnauthorisedController : Controller
    {
        //
        // GET: /Unauthorised/
        public ActionResult Index()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Session.Abandon();
            return View();
        }
	}
}