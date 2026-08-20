using AljazeeraCPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using Newtonsoft.Json.Linq;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class DeActiveAccountController : Controller
    {
        //
        // GET: /DeActiveAccount/

        DataSource ds = new DataSource();
        Connecttocore core = new Connecttocore();
        //
        // GET: /ActiveAccount/
        public ActionResult DeActiveCustomer()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["deresult"] != null)
            {
                ViewBag.SuccessMessage = Session["deresult"].ToString();
                Session["deresult"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            //String userbranch = Session["user_branch"].ToString();


            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);

        }

        // WAPT05: anti-forgery on this state-changing POST.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateCustomerSts(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string message = "";

            try
            {
                // WAPT04/06: a deactivation request may only be raised on a customer whose
                // REAL current status is Active. Re-derived from the DB so a tampered
                // "status" field cannot bypass the approval workflow.
                string realCode = ds.getCustomerStatusCode(model.Branch) ?? "";
                if (!realCode.Equals("A", StringComparison.OrdinalIgnoreCase))
                {
                    Session["deresult"] = "Deactivation request not allowed: customer is not in an active state.";
                    return RedirectToAction("DeActiveCustomer") /* WAPT09: was RedirectToAction(..., model) — do not serialize model into the redirect URL */;
                }

                if (ds.UpdatecustomerSts(model.Branch, "RDA"))
                {
                    Session["deresult"] = "Customer information De-Activation request was successful";
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), model.AccountNumber, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request DeActivated customer", model.CustomerName + " - " + model.Branch, DateTime.Now.ToString());
                }
                else
                {
                    Session["deresult"] = "Something has gone wrong, please try again.";
                }

                //string apiresponse = Connecttocore.activateCustomer(model.Branch, Session["accesstoken"].ToString());
                //JObject response = new JObject();
                //response = JObject.Parse(apiresponse);

                //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                //if (responseCode == 0)
                //{
                //    ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                //}
                //else
                //{
                //    message = response.GetValue("Response_Message").ToString();
                //    ModelState.AddModelError("", message);
                //}
                return RedirectToAction("DeActiveCustomer") /* WAPT09: was RedirectToAction(..., model) — do not serialize model into the redirect URL */;
                //return View(model);

                //String userbranch = Session["user_branch"].ToString();
                //model.Branches = ds.PopulateBranchs(userbranch);
                //model.AccTypes = ds.PopulateAccountTypes();
                //model.Currencies = ds.PopulateCurrencies();
                //model.catgories = ds.GetGatgories();
                //model.catgories.RemoveAt(0);
                //var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                //var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                //var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                //var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                //if (selectedBranch != null)
                //{
                //    selectedBranch.Selected = true;

                //}
                //if (selectedAccType != null)
                //{
                //    selectedAccType.Selected = true;

                //}
                //if (selectedCurrency != null)
                //{
                //    selectedCurrency.Selected = true;

                //}
                //if (selectedcategory != null)
                //{
                //    selectedcategory.Selected = true;

                //}

                //if ( ModelState.IsValidField(model.AccountNumber))

                //{
                //    custinfo infomodel = new custinfo();

                //    String response;
                //    String fullnumber =  model.AccountNumber;

                //    infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullnumber);
                //    response = infomodel.lblconfirm;
                //    if (response.Equals("This Account is Already exist"))
                //    {
                //        //String act = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                //        String act = model.AccountNumber;
                //        Session["Account"] = act;
                //        if (infomodel.status.ToString().Equals("D") || infomodel.status.ToString().Equals("U") || infomodel.status.ToString().Equals("B"))
                //        {
                //            int result = ds.updatecustomerusingact(act, "A");
                //            if (result != -1)
                //            {
                //                string custname = ds.getcustomerfullname(act);
                //                string customeraccount = custname;
                //                string usershorSthand = "35" +  act.Substring(2, 11).ToString();
                //                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                //                custinfo customerinformations = ds.getcustinfo("", "", "", "", "", model.AccountNumber);
                //                string response2 = core.sendpredefinedsms(customerinformations.user_id, act.Substring(2, 11).ToString() ,"5", customerinformations.user_mobile);
                //                //string response2 = core.sendotp(customerinformations.user_id, "Your account : " + model.AccountNumber + " has been successfully activated as you requested.", customerinformations.user_mobile);
                //                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Activated customer", usershorSthand + " - " + custname, DateTime.Now.ToString());

                //                String s = act.Substring(2, 11).ToString();

                //                Session["acresult"] = "The Customer Account " + s + " Activated Successfully";
                //                //return RedirectToAction("Index", "Home", new { area = "" });
                //                return RedirectToAction("ActiveCustomer");
                //            }
                //        }
                //        else if (infomodel.status.ToString().Equals("P"))
                //        {

                //            message = "This Customer Account Is Not Authorized";
                //            ModelState.AddModelError("", message);
                //            return View(model);
                //        }

                //        else if (infomodel.status.ToString().Equals("R"))
                //        {
                //            message = "This Customer Account Is Rejected";
                //            ModelState.AddModelError("", message);
                //            return View(model);

                //        }

                //        else if (infomodel.status.ToString().Equals("A"))
                //        {
                //            message = "This Customer Account Is  activated already";
                //            ModelState.AddModelError("", message);
                //            return View(model);
                //        }


                //    }
                //    else
                //    {
                //        message = "Sorry this account Not Registered ";
                //        ModelState.AddModelError("", message);
                //        return View(model);
                //    }
                //}
                //else
                //{
                //    message = "All Fields are required ";
                //    ModelState.AddModelError("", "Something is missing" + message);

                //}


            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);

            }
            return RedirectToAction("DeActiveCustomer") /* WAPT09: was RedirectToAction(..., model) — do not serialize model into the redirect URL */;
            //return View(model);
        }

        [HttpPost]
        public ActionResult DeActiveCustomer(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string message = "";
            try
            {
                String userbranch = Session["user_branch"].ToString();

                string apiresponse = Connecttocore.deactivateCustomer(model.Branch, Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ViewBag.SuccessMessage = "DeActivate " + response.GetValue("Response_Message").ToString();
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
                return View(model);


                //model.Branches = ds.PopulateBranchs(userbranch);
                //model.AccTypes = ds.PopulateAccountTypes();
                //model.Currencies = ds.PopulateCurrencies();
                //model.catgories = ds.GetGatgories();
                //model.catgories.RemoveAt(0);
                //var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                //var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                //var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                //var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());

                //if (selectedBranch != null)
                //{
                //    selectedBranch.Selected = true;

                //}
                //if (selectedAccType != null)
                //{
                //    selectedAccType.Selected = true;

                //}
                //if (selectedCurrency != null)
                //{
                //    selectedCurrency.Selected = true;

                //}
                //if (selectedcategory != null)
                //{
                //    selectedcategory.Selected = true;

                //}


                //if (ModelState.IsValidField(model.AccountNumber))
                //{
                //    custinfo infomodel = new custinfo();

                //    String response;
                //    String fullnumber = model.AccountNumber;
                //    infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode,fullnumber);
                //    response = infomodel.lblconfirm;
                //    if (response.Equals("This Account is Already exist"))
                //    {
                //        //String act = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                //        String act = model.AccountNumber;
                //        Session["Account"] = act;
                //        if (infomodel.status.ToString().Equals("A"))
                //        {
                //            int result = ds.updatecustomerusingact(act, "D");
                //            if (result != -1)
                //            {
                //                string custname = ds.getcustomerfullname(act);
                //                string customeraccount = custname;
                //                string usershorSthand = "35" + act.Substring(2, 11).ToString();
                //                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                //                custinfo customerinformations = ds.getcustinfo("", "", "", "", "", model.AccountNumber);
                //                string response2 = core.sendpredefinedsms(customerinformations.user_id,  act.Substring(2, 11).ToString() ,"6", customerinformations.user_mobile);
                //                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Deactivated customer", usershorSthand + " - " + custname, DateTime.Now.ToString());
                //                String s = act.Substring(2,11).ToString();


                //                Session["deresult"] = "The Customer Account " + s + " DeActivated Successfully";
                //                //return RedirectToAction("Index", "Home", new { area = "" });
                //                return RedirectToAction("DeActiveCustomer");
                //            }
                //        }
                //        else if (infomodel.status.ToString().Equals("P"))
                //        {

                //            message = "This Customer Account Is Not Authorized";
                //            ModelState.AddModelError("", message);
                //            return View(model);
                //        }

                //        else if (infomodel.status.ToString().Equals("R"))
                //        {
                //            message = "This Customer Account Is Rejected";
                //            ModelState.AddModelError("", message);
                //            return View(model);

                //        }

                //        else if (infomodel.status.ToString().Equals("D"))
                //        {
                //            message = "This Customer Account Is Deleted or Deactivated";
                //            ModelState.AddModelError("", message);
                //            return View(model);


                //        }
                //        else if (infomodel.status.ToString().Equals("S"))
                //        {
                //            message = "This Customer Account Is Stoped";
                //            ModelState.AddModelError("", message);
                //            return View(model);




                //        }


                //    }
                //    else
                //    {
                //        message = "Sorry this account Not Registered ";
                //        ModelState.AddModelError("", message);
                //        return View(model);
                //    }
                //}
                //else
                //{
                //    message = "All Fields are required ";
                //    ModelState.AddModelError("", "Something is missing" + message);

                //}
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);

            }
            return View(model);
        }
       

        [HttpPost]
        public ActionResult DeActiveCustomerprocess(CustomerRegBankinfo passedmodel)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            if (passedmodel.Branch != null)
            {
                model = new CustomerRegBankinfo();
                string message = "";
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserRegistrationData(passedmodel.Branch);

                if (model.status.ToString().Equals("DE"))
                {
                    message = "This Customer is already Deactivated";
                    ModelState.AddModelError("", message);
                    return View("DeActiveCustomer", model);
                }

                //if (model.status.ToString().Equals("U"))
                //{
                //    message = "This Customer Account is Autherized, please Activate it";
                //    ModelState.AddModelError("", message);
                //    return View("DeActiveCustomer", model);
                //}
                if (model.status.ToString().Equals("UA"))
                {
                    message = "This Customer Account is Un Autherized, please Autherized it";
                    ModelState.AddModelError("", message);




                    return View("DeActiveCustomer", model);
                }

                //else if (model.status.ToString().Equals("B"))
                //{
                //    message = "This Customer Account is Blocked , please Activate it ";
                //    ModelState.AddModelError("", message);
                //    return View("DeActiveCustomer", model);
                //}

                //else if (model.status.ToString().Equals("A"))
                //{
                //    message = "This Customer Account is DeActivated , please Activate it";
                //    ModelState.AddModelError("", message);
                //    return View("ActiveCustomer", model);
                //}

                //else if (model.status.ToString().Equals("D"))
                //{
                //    message = "This Customer Account is DeActivated , please Activate it";
                //    ModelState.AddModelError("", message);
                //    return View("ActiveCustomer", model);
                //}

                //model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                //model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                //model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);

                //model.catgories = ds.GetGatgories();
                //model.Channels = ds.Channels();
                return View("DeActiveCustomer", model);
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
                return View("DeActiveCustomer", model);
            }
            

        }

    }
}
