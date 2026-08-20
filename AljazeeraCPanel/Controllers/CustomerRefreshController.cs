using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class CustomerRefreshController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: CustomerRefresh



        public ActionResult CustomerRefresh()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String userbranch = "";
            //if (Session["refreshaccountresult"] != null)
            //{
            //    ViewBag.SuccessMessage = Session["refreshaccountresult"].ToString();
            //    Session["refreshaccountresult"] = null;
            //}
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            if (Session["user_branch"].ToString() != null)
            {
                userbranch = Session["user_branch"].ToString();
            }
            else
            {
                RedirectToAction("Index", "Home");
            }

         
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();
            return View(model);
        }

        [HttpPost]
        public ActionResult CustomerRefresh(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ModelState.Clear();
            String message;
            //  account model;
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

                if (ModelState.IsValid)
                {
                    //while (model.AccountNumber.Length < 11)
                    //{
                    //    model.AccountNumber = "0" + model.AccountNumber;
                    //}
                    String Accountnumber = model.AccountNumber;
                    String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
                    if (response.Equals("This Account is Already exist"))
                    {
                        //String act = model.AccountNumber;
                        //act = "1101601560001634680001000";
                        Session["Account"] = Accountnumber;
                        Session["branchcode"] = model.BranchCode;
                        //Session["shortaccount"] = shortaccount;
                        //CustomerRegBankinfo accounts_Listt = new CustomerRegBankinfo();
                        ////accounts_List.Account_No = primaryAccountNo;
                        //List <CustomerRegBankinfo> Account_Info = new List<CustomerRegBankinfo>();
                        ////accounts_Listt.Accounts_List = new List<CustomerRegBankinfo>();
                        //string A
                        //Account_Info.Add(new CustomerRegBankinfo




                        //    );
                        //accounts_Listt.Accounts_List.Add(Accountnumber);

                         

                        string response2 = Connecttocore.GetCustinfoCore(Accountnumber , Session["accesstoken"].ToString());

                        //string response2 = "{\"Accounts_List\": [  {  \"IBAN\": \"SD2135010000053907\",          \"Account_Type_Code\": \"CUR\",          \"Account_No\": \"10000053907\",           \"Currency_Code\": \"SDG\",          \"Branch_Code\": \"017\"      }    ],   \"Response_Code\": 0,    \"Response_Message\": \"Successful\",   \"Email\": \" \",    \"Phones\": [        {            \"Phone_No\": \"249966303037\"       },       {           \"Phone_No\": \"249966303037\"       },       {          \"Phone_No\": \"249966303037\"       }  ],   \"Address\": \"حي الدوحة مربع 29\",    \"Customer_Name_EN\": \"Amel Ismail Osman Abdelwadoud\",   \"RIM\": \"79864\",   \"Customer_Name_AR\": \"امل اسماعيل عثمان عبد الودود\"}";

                            JObject jobj = new JObject();
                        jobj = JObject.Parse(response2);
                        dynamic result = jobj;

                        string responseCode = result.Response_Code;

                        if (responseCode == "0")
                        {
                        string responseMessage = result.response_Message;
                        string address = result.Address;
                          Session["address"] = address;
                      

                            string email = result.Email;
                            Session["email"] = email;
                            string custnameen = result.Customer_Name_EN;
                            Session["CustomerName"]  = custnameen;
                        string rim = result.RIM;
                            Session["rim"] = rim;
                        string custnamear = result.Customer_Name_AR;
                            Session["customernameArabic"] = custnamear;
                            string Address = result.Address;
                            Session["Address"] = Address;
                            //string bal = result.result;
                            JToken resAccList = result.Accounts_List;
                            //JObject jobj2 = new JObject();
                            //jobj2 = JObject.FromObject(resAccList);
                            JObject jobj2 = JObject.Parse(resAccList[0].ToString());
                            //dynamic result2 = jobj2;


                            string iban = jobj2.GetValue("IBAN").ToString();   //IBAN;
                            Session["iban"] = iban;

                            string acctype = jobj2.GetValue("Account_Type_Code").ToString();
                            Session["AccountType"] = acctype;
                            string accountno = jobj2.GetValue("Account_No").ToString();
                            Session["AccountNumber"] = accountno;
                            string curr = jobj2.GetValue("Currency_Code").ToString();
                            Session["Currency"] = curr;
                            string branch = jobj2.GetValue("Branch_Code").ToString();
                            Session["Branch"] = branch;
                            JToken phones = result.Phones;



                            JObject jphone =  JObject.Parse( phones[0].ToString());
                            string phone = jphone.GetValue("Phone_No").ToString();
                            Session["CustomerPhone"] = phone;


                        String custID;
                        String custphone;


                        //if (responseCode == "0")
                        //{
                            custID = "N/A";
                            //custname = acc[2].ToString();

                            //custphone = acc[4].ToString();
                            Session["custID"] = custID;
                            //Session["custname"] = custname;
                            //Session["custphone"] = custphone;
                            Session["custcat"] = model.CategoryCode;
                            //string usershorthand = "11" + model.BranchCode + model.AccountNumber;
                            string usershorthand = accountno;
                            string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                            //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer Inquery", usershorthand + " - " + custnameen, DateTime.Now.ToString());
                            return RedirectToAction("Refreshuser");
                        }

                        else
                        {
                            message = "Please check customer information something wrong ";
                            ModelState.AddModelError("", message);
                            return View(model);
                        }
                        // 
                        //}
                        //else
                        //{
                        //    message = "Sorry You Cannot register to this account because  ";
                        //    ModelState.AddModelError("", message + response);
                        //    return View(model);
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Check Customer Information");
                        TempData["Success"] = true;
                        ViewBag.ResponseStat = "Successfully Sent";
                        ViewBag.ResponseMSG = "Thank you for Contacting us";
                    }
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                    TempData["Success"] = true;
                    ViewBag.ResponseStat = "Successfully Sent";
                    ViewBag.ResponseMSG = "Thank you for Contacting us";
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
        public ActionResult GetCustomerRefreshPage(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string message;
            if (model.Branch != null)
            {
                model = new CustomerRegBankinfo();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserRegistrationData(model.Branch);



                if (model.status.ToString().Equals("UA"))
                {
                    message = "This Customer Account is Un Autherized, please Autherized it";
                    ModelState.AddModelError("", message);




                    return View("CustomerRefresh", model);
                }
                if (model.status.ToString().Equals("DE"))
                {
                    message = "This Customer Account is Deactivated, please activate it";
                    ModelState.AddModelError("", message);




                    return View("CustomerRefresh", model);
                }

                model.Branches = ds.PopulateBranchs(model.BranchCode, model.Branch);
                model.AccTypes = ds.PopulateAccountTypes(model.Branch);
                model.Currencies = ds.PopulateCurrencies();

                model.catgories = ds.GetGatgories();
                model.Channels = ds.Channels();
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
            }
            try
            {
                String userbranch = Session["user_branch"].ToString();
                //model.catgories.RemoveAt(0);
                if (ModelState.IsValid)
                {
                    while (model.AccountNumber.Length < 7)
                    {
                        model.AccountNumber = "0" + model.AccountNumber;
                    }
                    String Accountnumber = "23" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                    String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
                    if (response.Equals("This Account is Already exist"))
                    {
                        String act = "23" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                        //act = "1101601560001634680001000";
                        Session["Account"] = act;
                        Session["branchcode"] = model.BranchCode;
                        //Session["shortaccount"] = shortaccount;
                        string response2 = Connecttocore.GetCustinfo(act);
                        JObject jobj = new JObject();
                        jobj = JObject.Parse(response2);
                        dynamic result = jobj;

                        string responseStatus = result.responseStatus;
                        string responseMessage = result.responseMessage;
                        string bal = result.result;
                        string[] separators = { ",", ":" };
                        string value = bal;
                        string[] acc = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        String custname;
                        String custID;
                        String custphone;

                        if (acc.Length >= 10)
                        {
                            custID = acc[1].ToString();
                            custname = acc[3].ToString();

                            custphone = acc[5].ToString();
                            Session["custID"] = custID;
                            Session["custname"] = custname;
                            Session["custphone"] = custphone;
                            Session["custcat"] = model.CategoryCode;
                            //string usershorthand = "11" + model.BranchCode + model.AccountNumber;
                            string usershorthand = model.AccountNumber;
                            string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                            ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer Inquery", usershorthand + " - " + custname, DateTime.Now.ToString());
                            return RedirectToAction("Refreshuser");
                        }

                        else
                        {
                            message = "Please check customer information something wrong ";
                            ModelState.AddModelError("", message);
                            return View(model);
                        }
                        // 
                        //}
                        //else
                        //{
                        //    message = "Sorry You Cannot register to this account because  ";
                        //    ModelState.AddModelError("", message + response);
                        //    return View(model);
                        //}
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Check Customer Information");
                        TempData["Success"] = true;
                        ViewBag.ResponseStat = "Successfully Sent";
                        ViewBag.ResponseMSG = "Thank you for Contacting us";
                    }
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                    TempData["Success"] = true;
                    ViewBag.ResponseStat = "Successfully Sent";
                    ViewBag.ResponseMSG = "Thank you for Contacting us";
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
        public ActionResult CustomerRefreshprocess(CustomerRegBankinfo passedmodel)
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
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserRegistrationData(passedmodel.Branch);

                //model.CustomerName = 
                //model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                //model.AccTypes = ds.PopulateAccountTypes(passedmodel.Branch);
                //model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);

                //model.catgories = ds.GetGatgories();
                //model.Channels = ds.Channels();
                return View("CustomerRefresh", model);
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
                return View("Add", model);
            }

        }

        public ActionResult Refreshuser()
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            //if (Session["refreshaccountresult"] != null)
            //{
            //    ViewBag.msg = Session["refreshaccountresult"].ToString();
            //    Session["refreshaccountresult"] = null;
            //}

            if (Session["refreshaccountresult"] != null)
            {
                ViewBag.SuccessMessage = Session["refreshaccountresult"].ToString();
                Session["refreshaccountresult"] = null;
            }

            string brname = ds.GetBranchName(Session["Branch"].ToString());
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            //model.CustomerID = Session["custID"].ToString();
            model.CustomerName = Session["CustomerName"].ToString();
            model.address = Session["Address"].ToString();
            model.CustomerPhone = Session["CustomerPhone"].ToString();

            //Session["CustomerName"] = custnameen;
            model.rim =  Session["rim"].ToString();
            model.customernameArabic = Session["customernameArabic"].ToString();
            model.iban = Session["iban"].ToString();
            model.AccountType = Session["AccountType"].ToString();
           model.AccountNumber = Session["AccountNumber"].ToString();
            model.Branch = brname;// Session["Branch"].ToString();
            model.email = Session["email"].ToString();
            return View(model);

        }


        public ActionResult executeupdate(string email , string address, string customerphonenumber)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string accounttorefresh = Session["AccountNumber"].ToString();
            string userid = ds.getCustIDFromAcc(accounttorefresh);
            //string email = Session["email"].ToString();
            if(ds.refreshcustomer(int.Parse(userid), email, address, customerphonenumber))
            {
                Session["refreshaccountresult"] = "Customer data updated accordingly";
            }
            else
            {
                Session["refreshaccountresult"] = "Something has gone wrong, please try again.";
            }
            return RedirectToAction("CustomerRefresh", "Refreshuser");
        }
    }
}