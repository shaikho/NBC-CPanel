using iTextSharp.text;
using iTextSharp.text.pdf;
using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace Cpanel.Controllers
{
    [AuthorizeSession]
    public class resetCustomerController : Controller
    {
        DataSource ds = new DataSource();
        Connecttocore core = new Connecttocore();
        //
        // GET: /resetCustomer/
        public ActionResult ResetCust()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String userbranch = Session["user_branch"].ToString();

            if (TempData["successful"] != null)
            {
                ViewBag.SuccessMessage = TempData["successful"].ToString();
                TempData["successful"] = null;
            }
            //if (Session["userresult"] != null)
            //{
            //    ViewBag.SuccessMessage = Session["userresult"].ToString();
            //    Session["userresult"] = null;
            //}

            if (Session["userresultF"] != null)
            {
                ViewBag.failMessage = Session["userresultF"].ToString();
                Session["userresultF"] = null;
            }

            Customerinfopass model = new Customerinfopass();
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);
        }

        public ActionResult ResetCustDevice()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String userbranch = Session["user_branch"].ToString();


            Customerinfopass model = new Customerinfopass();
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);
        }

        public ActionResult AddAccOTP()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String userbranch = Session["user_branch"].ToString();


            Customerinfopass model = new Customerinfopass();
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetCust(Customerinfopass model)
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


                //if (ModelState.IsValid)
                //{
                //custinfo infomodel = new custinfo();
                //String fullaccountnumber = "35" + model.AccountNumber;
                //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode,fullaccountnumber);
                //response = infomodel.lblconfirm;

                //if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("A"))
                //{
                //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                //    model.AccountNumber;
                //String Accountnumber = "35" + model.AccountNumber;

                //String Accountnumber = restmodel.account;
                //List<resetpass> result = new List<resetpass>();
                //result = ds.resetpassword(Accountnumber);

                string apiresponse = Connecttocore.restCustomerPassword(model.Branch, Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                   string  result2 = ds.resetpassword(model.Branch);
                    if(!result2.Equals("0"))
                    {
                        model.pass = result2;
                        //Session["pass"] = model.pass;
                    }
                    Session["pass"] = model.pass;
                    return RedirectToAction("Print", "resetCustomer");
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
                return View(model);
                //    foreach (var item in result)
                //{
                //    if (item.lblconfirm == "Successfully")
                //    {
                //        restmodel.name = item.name;
                //        restmodel.account = item.account;
                //        //restmodel.branchname = item.branchname;
                //        restmodel.pass = item.pass;
                //        Session["presetpassresult"] = restmodel;
                //        return RedirectToAction("Print", "resetCustomer");
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", item.lblconfirm);
                //    }
                //}


                    //return View(model);
                    //}
                    //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("B"))
                    //{
                    //    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                    //    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;

                    //    //String Accountnumber = restmodel.account;
                    //    List<resetpass> result = new List<resetpass>();
                    //    result = ds.resetpassword(Accountnumber);

                    //    foreach (var item in result)
                    //    {
                    //        if (item.lblconfirm == "Successfully")
                    //        {
                    //            restmodel.name = item.name;
                    //            restmodel.account = item.account;
                    //            restmodel.branchname = item.branchname;
                    //            restmodel.pass = item.pass;
                    //            Session["presetpassresult"] = restmodel;
                    //            return RedirectToAction("Print", "resetCustomer");
                    //        }
                    //        else
                    //        {
                    //            ModelState.AddModelError("", item.lblconfirm);
                    //        }
                    //    }


                    //    return View(model);
                    //}

                    //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("P"))
                    //{

                    //    message = "This Customer Account Is Not Authorized";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);
                    //}

                    //else if (response.Equals("This Account is not activated yet") && infomodel.status.ToString().Equals("U"))
                    //{

                    //    message = "This Customer Account Is Not Authorized";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);
                    //}

                    //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("R"))
                    //{
                    //    message = "This Customer Account Is Rejected";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);

                    //}


                    //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("D"))
                    //{
                    //    message = "This Customer Account Is Deleted or Deactivated";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);


                    //}
                    //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("S"))
                    //{
                    //    message = "This Customer Account Is Stoped";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);


                    //}
                    //else
                    //{
                    //    message = "This Customer Account Is Not Register";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);


                    //}

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
                //if ("".Equals("Un Autherize"))
                //{
                //    // ModelState.AddModelError("", "Can Not Reset Password");
                //    // return View("Users");
                //     message = "Customer is not authorized. Password reset cannot be performed";
                //    Session["userresultF"] = message;
                //    return RedirectToAction("ResetCust", "resetCustomer");
                //    // return View("Users");
                //}

                if (ds.UpdatecustomerSts(model.Branch, "RR"))
                {
                    TempData["successful"] = "Reset Customer Password request was successful";
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), model.AccountNumber, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request Reset customer Password", model.CustomerName + " - " + model.Branch, DateTime.Now.ToString());
                }
                else
                {
                    TempData["successful"] = "Something has gone wrong, please try again.";
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
                return RedirectToAction("ResetCust", model);
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
            return RedirectToAction("ResetCust", model);
            //return View(model);
        }


        [HttpPost]
        public ActionResult ResetCustDevice(Customerinfopass model)
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


                //if (ModelState.IsValid)
                //{
                //custinfo infomodel = new custinfo();
                //String fullaccountnumber = "35" + model.AccountNumber;
                //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode,fullaccountnumber);
                //response = infomodel.lblconfirm;

                //if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("A"))
                //{
                //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                //    model.AccountNumber;
                //String Accountnumber = "35" + model.AccountNumber;

                //String Accountnumber = restmodel.account;
                //List<resetpass> result = new List<resetpass>();
                //result = ds.resetpassword(Accountnumber);


                string apiresponse = Connecttocore.restCustomerPasswordDevice(model.Branch, Session["accesstoken"].ToString());
                
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                    string result2 = ds.resetpassword(model.Branch);
                    if (!result2.Equals("0"))
                    {
                        model.pass = result2;
                    }
                    Session["pass"]= model.pass;

                    return RedirectToAction("Print", "resetCustomer");
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }

                return View(model);

                //    foreach (var item in result)
                //{
                //    if (item.lblconfirm == "Successfully")
                //    {
                //        restmodel.name = item.name;
                //        restmodel.account = item.account;
                //        //restmodel.branchname = item.branchname;
                //        restmodel.pass = item.pass;
                //        Session["presetpassresult"] = restmodel;
                //        return RedirectToAction("Print", "resetCustomer");
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", item.lblconfirm);
                //    }
                //}


                //return View(model);
                //}
                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("B"))
                //{
                //    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                //    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;

                //    //String Accountnumber = restmodel.account;
                //    List<resetpass> result = new List<resetpass>();
                //    result = ds.resetpassword(Accountnumber);

                //    foreach (var item in result)
                //    {
                //        if (item.lblconfirm == "Successfully")
                //        {
                //            restmodel.name = item.name;
                //            restmodel.account = item.account;
                //            restmodel.branchname = item.branchname;
                //            restmodel.pass = item.pass;
                //            Session["presetpassresult"] = restmodel;
                //            return RedirectToAction("Print", "resetCustomer");
                //        }
                //        else
                //        {
                //            ModelState.AddModelError("", item.lblconfirm);
                //        }
                //    }


                //    return View(model);
                //}

                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("P"))
                //{

                //    message = "This Customer Account Is Not Authorized";
                //    ModelState.AddModelError("", message);
                //    return View(model);
                //}

                //else if (response.Equals("This Account is not activated yet") && infomodel.status.ToString().Equals("U"))
                //{

                //    message = "This Customer Account Is Not Authorized";
                //    ModelState.AddModelError("", message);
                //    return View(model);
                //}

                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("R"))
                //{
                //    message = "This Customer Account Is Rejected";
                //    ModelState.AddModelError("", message);
                //    return View(model);

                //}


                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("D"))
                //{
                //    message = "This Customer Account Is Deleted or Deactivated";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}
                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("S"))
                //{
                //    message = "This Customer Account Is Stoped";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}
                //else
                //{
                //    message = "This Customer Account Is Not Register";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}

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
        public ActionResult AddAccOTP(Customerinfopass model)
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


                //if (ModelState.IsValid)
                //{
                //custinfo infomodel = new custinfo();
                //String fullaccountnumber = "35" + model.AccountNumber;
                //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode,fullaccountnumber);
                //response = infomodel.lblconfirm;

                //if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("A"))
                //{
                //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
                //    model.AccountNumber;
                //String Accountnumber = "35" + model.AccountNumber;

                //String Accountnumber = restmodel.account;
                //List<resetpass> result = new List<resetpass>();
                //result = ds.resetpassword(Accountnumber);

                string apiresponse = Connecttocore.AddAccountOTP(model.Branch, Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                     ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                    string result2 = ds.resetpassword(model.Branch);
                    if (!result2.Equals("0"))
                    {
                        model.pass = result2;
                    }
                    Session["pass"] = model.pass;

                    return RedirectToAction("Print", "resetCustomer");
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
                return View(model);
                //    foreach (var item in result)
                //{
                //    if (item.lblconfirm == "Successfully")
                //    {
                //        restmodel.name = item.name;
                //        restmodel.account = item.account;
                //        //restmodel.branchname = item.branchname;
                //        restmodel.pass = item.pass;
                //        Session["presetpassresult"] = restmodel;
                //        return RedirectToAction("Print", "resetCustomer");
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("", item.lblconfirm);
                //    }
                //}


                //return View(model);
                //}
                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("B"))
                //{
                //    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                //    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;

                //    //String Accountnumber = restmodel.account;
                //    List<resetpass> result = new List<resetpass>();
                //    result = ds.resetpassword(Accountnumber);

                //    foreach (var item in result)
                //    {
                //        if (item.lblconfirm == "Successfully")
                //        {
                //            restmodel.name = item.name;
                //            restmodel.account = item.account;
                //            restmodel.branchname = item.branchname;
                //            restmodel.pass = item.pass;
                //            Session["presetpassresult"] = restmodel;
                //            return RedirectToAction("Print", "resetCustomer");
                //        }
                //        else
                //        {
                //            ModelState.AddModelError("", item.lblconfirm);
                //        }
                //    }


                //    return View(model);
                //}

                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("P"))
                //{

                //    message = "This Customer Account Is Not Authorized";
                //    ModelState.AddModelError("", message);
                //    return View(model);
                //}

                //else if (response.Equals("This Account is not activated yet") && infomodel.status.ToString().Equals("U"))
                //{

                //    message = "This Customer Account Is Not Authorized";
                //    ModelState.AddModelError("", message);
                //    return View(model);
                //}

                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("R"))
                //{
                //    message = "This Customer Account Is Rejected";
                //    ModelState.AddModelError("", message);
                //    return View(model);

                //}


                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("D"))
                //{
                //    message = "This Customer Account Is Deleted or Deactivated";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}
                //else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("S"))
                //{
                //    message = "This Customer Account Is Stoped";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}
                //else
                //{
                //    message = "This Customer Account Is Not Register";
                //    ModelState.AddModelError("", message);
                //    return View(model);


                //}

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
        public ActionResult ResetCustprocess(Customerinfopass passedmodel)
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
                string message = "";
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserinfoData(passedmodel.Branch);
                if (model.status.ToString().Equals("U"))
                {
                    message = "This Customer Account is Autherized, please Activate it";
                    ModelState.AddModelError("", message);
                    return View("ResetCust", model);
                }
                if (model.status.ToString().Equals("UA"))
                {
                    message = "This Customer Account is Un Autherized, please Autherized and Activate it";
                    ModelState.AddModelError("", message);

                
                   

                    return View("ResetCust", model);
                }
              
                else if(model.status.ToString().Equals("B"))
                {
                    message = "This Customer Account is Blocked , please Activate it ";
                    ModelState.AddModelError("", message);
                    return View("ResetCust", model);
                }

                else if (model.status.ToString().Equals("D"))
                {
                    message = "This Customer Account is DeActivated or Deleted, please Activate it";
                    ModelState.AddModelError("", message);
                    return View("ResetCust", model);
                }

                //model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                //model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                //model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);
                Session["presetpassresult"] = model;
                //model.catgories = ds.GetGatgories();
                return View("ResetCust", model);
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
                return View("ResetCust", model);
            }

        }

        [HttpPost]
        public ActionResult ResetCustprocessDevice(Customerinfopass passedmodel)
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

                //model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                //model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                //model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);
                Session["presetpassresult"] = model;
                //model.catgories = ds.GetGatgories();
                return View("ResetCustDevice", model);

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
                return View("ResetCustDevice", model);
            }

        }

        [HttpPost]
        public ActionResult AddAccOTPProcess(Customerinfopass passedmodel)
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

                //model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                //model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                //model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);
                Session["presetpassresult"] = model;
                //model.catgories = ds.GetGatgories();
                return View("AddAccOTP", model);
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
                return View("AddAccOTP", model);
            }

        }


        public ActionResult smspassword(string password, string account)
        {
            custinfo customerinformations = ds.getcustinfo( "", account);
            //string msg = "Your Account temporery password is : "+password;
            string msg = password;
            //Clipboard.SetDataObject(msg, true);
            //string msg = "تم إعادة تعين كلمه المرور الخاص بك. ويمكنك الدخول عن طريق كلمة السر : " + password + " .";
            //string response = core.sendpredefinedsms(customerinformations.user_id, password, "3", customerinformations.user_mobile);
            var response = core.sendotpbyURL(customerinformations.user_id, msg, customerinformations.user_mobile);

            //JObject jobj = new JObject();
            //jobj = JObject.Parse(response);
            //dynamic result = jobj;

            //var errorCode = result.errorcode;
            //var errormsg = result.errormsg;
            var Status = 1;  //result.status;

            if (Status == 1)
            {
                string custname = customerinformations.user_name;
                string customeraccount = account;
                string usershorSthand = account;  //"23" + customeraccount.Substring(3, 3) + customeraccount.Substring(13);
                string adminbranch = Session["branch_namee"].ToString();   //ds.getbranchnameenglish(Session["user_branch"].ToString());
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reset password sent to customer vis sms", usershorSthand + " - " + custname, DateTime.Now.ToString());

                TempData["Success"] = true;
                ViewBag.ResponseStat = "Successful";
                ViewBag.ResponseMSG = "Password sent to customer via sms successfully";
                ViewBag.SuccessMessage = "Password sent to customer via SMS.";
                TempData["successful"] = "Password sent to customer via sms successfully";
                return RedirectToAction("ResetCust");
            }
            else
            {
                TempData["Success"] = true;
                ViewBag.ResponseStat = "Not Successful";
                ViewBag.ResponseMSG = "Faild to send password sms, please try again.";
                ViewBag.SuccessMessage = "Message was not sent to customer, Please try again.";
                TempData["failed"] = "Failed to send password sms, please try again.";
                return RedirectToAction("ResetCust");
            }
        }

        public ActionResult Print()
       {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //resetpass model = new resetpass();
            Customerinfopass model = new Customerinfopass();

            model = (Customerinfopass)Session["presetpassresult"];
            model.pass = Session["pass"].ToString();
            //model = (Customerinfopass)Session["presetpassresult2"];
            return View(model);
        }

        public FileResult SavePDF()
        {
            //List < Employee > employees = _context.employees.ToList < Employee > ();  
            Customerinfopass model = new Customerinfopass();

            model = (Customerinfopass)Session["presetpassresult2"];

            string custname = model.CustomerName;
            string customeraccount = model.AccountNumber;
            string pass = model.pass;
            string usershorSthand = "35" + customeraccount.Substring(13);
            string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reset password printed to customer", usershorSthand + " - " + custname, DateTime.Now.ToString());


            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("Customerpassword - " + model.AccountNumber.ToString() + " - " + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(4);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   

            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 25, 25, 25, 25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  




            tableLayout.AddCell(new PdfPCell(new Phrase("JSB  CPanel", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header 

            AddCellToHeader(tableLayout, "Customer Account");
            //AddCellToHeader(tableLayout, "Customer Branch");
            AddCellToHeader(tableLayout, "Customer Name");
            AddCellToHeader(tableLayout, "Customer Password");
            AddCellToHeader(tableLayout, "Date");

            ////Add body  

            Customerinfopass model = new Customerinfopass();

            model = (Customerinfopass)Session["presetpassresult2"];


            AddCellToBody(tableLayout, model.AccountNumber.ToString());
            //AddCellToBody(tableLayout, model.branchname.ToString());
            AddCellToBody(tableLayout, model.CustomerName.ToString());
            AddCellToBody(tableLayout, model.pass.ToString());
            AddCellToBody(tableLayout, DateTime.Now.ToString());



            return tableLayout;
        }

        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 128, 128)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

    }
}