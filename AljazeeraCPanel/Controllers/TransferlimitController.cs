using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Controllers
{
    //public class TransferlimitController : Controller
    //{
    //    // GET: Transferlimit
    //    DataSource ds = new DataSource();
    //    public ActionResult Index()
    //    {
    //        if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
    //        {
    //            return RedirectToAction("Login", "Login");
    //        }

    //        if (Session["userresult"] != null)
    //        {
    //            ViewBag.SuccessMessage = Session["userresult"].ToString();
    //            Session["userresult"] = null;
    //        }

    //        //List<Transferlimit> branchs = ds.GetAllBranchs();
    //        return View(branchs);
    //    }
    //}
}