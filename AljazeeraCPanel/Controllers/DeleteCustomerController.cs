using AljazeeraCPanel.Models;
using AljazeeraCPanel.Filters;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class DeleteCustomerController : Controller
    {
        DataSource ds = new DataSource();
        public ActionResult DeleteCustomer()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (TempData["success"] != null)
            {
                ViewBag.SuccessMessage = TempData["success"].ToString();
                TempData["success"] = null;
            }

            if (TempData["fail"] != null)
            {
                ViewBag.FailedMessage = TempData["fail"].ToString();
                TempData["fail"] = null;
            }
            String userbranch = Session["user_branch"].ToString();


            Customerinfopass model = new Customerinfopass();
            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCustomer(Customerinfopass model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            resetpass restmodel = new resetpass();
            string message = "";
            try
            {
                String userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();

                if (ModelState.IsValid)
                {
                    custinfo infomodel = new custinfo();
                    String response;
                    String fullaccountnumber = "35" + model.AccountNumber;
                    //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullaccountnumber);
                    infomodel = ds.getcustinfo(model.Branch,  model.AccountNumber);
                    response = infomodel.lblconfirm;

                    if (response.Equals("This Account is Already exist"))
                    {
                        string useridtodelete = ds.getuserid(infomodel.user_log);
                        int result = 0;
                        result = ds.deletecustomer(infomodel.user_log);
                        if(result == 1)
                        {
                            TempData["success"] = "Customer Deleted.";
                        }
                        else
                        {
                            TempData["fail"] = "Customer cannot be deleted.";
                        }
                        return View(model);
                    }
                    else
                    {
                        message = "This Customer Account Is Not Register";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                }
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCustomerprocess(Customerinfopass passedmodel)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Customerinfopass model = new Customerinfopass();
            if (passedmodel.Branch != null)
            {
                model = new Customerinfopass();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserinfoData(passedmodel.Branch);
                model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);

                model.catgories = ds.GetGatgories();
                return View("DeleteCustomer", model);
            }
            else
            {
                String userbranch = "";
                if (Session["addaccountresult"] != null)
                {
                    ViewBag.SuccessMessage = Session["addaccountresult"].ToString();
                    Session["addaccountresult"] = null;
                }

                userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                return View("DeleteCustomer", model);
            }

        }
    }
}