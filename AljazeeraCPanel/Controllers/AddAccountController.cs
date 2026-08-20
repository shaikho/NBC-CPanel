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
using System.Web.Services.Protocols;
using System.Web.Services.Description;

namespace SFBSPancel.Controllers
{
    [AuthorizeSession]
    public class AddAccountController : Controller
    {
        DataSource ds = new DataSource();
        Connecttocore core = new Connecttocore();
        //
        // GET: /AddAcount/
        public ActionResult Add()
        {
            //if (Session["user_name"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            //if (Session["user_branch"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            //String userbranch = "";
            //if (Session["addaccountresult"] != null)
            //{
            //    ViewBag.SuccessMessage = Session["addaccountresult"].ToString();
            //    Session["addaccountresult"] = null;
            //}
            //CustomerRegBankinfo model = new CustomerRegBankinfo();
            //if (Session["user_branch"].ToString() != null)
            //{
            //    userbranch = Session["user_branch"].ToString();
            //}
            //else
            //{
            //    RedirectToAction("Index", "Home");
            //}

            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();
            //model.catgories = ds.GetGatgories();
            //return View(model);
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["message"] != null)
            {
                ViewBag.SuccessMessage = Session["message"].ToString();
                Session["message"] = null;
            }
            if (Session["messagefalied"] != null)
            {
                ViewBag.failed = Session["messagefalied"].ToString();
                Session["messagefalied"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            String userbranch = Session["user_branch"].ToString();
            model.catgories = ds.GetGatgories();
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();

            //model.catgories = ds.GetGatgories();
            //model.Channels = ds.Channels();
            Session["regmodel"] = model;
            return View(model);

        }

        [HttpPost]
        public ActionResult Add(CustomerRegBankinfo model)
        {
            //if (Session["user_name"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            //if (Session["user_branch"] == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            //ModelState.Clear();
            //String message;
            //try
            //{
            //    String userbranch = Session["user_branch"].ToString();

            //    //model.Branches = ds.PopulateBranchs(userbranch);
            //    //model.AccTypes = ds.PopulateAccountTypes();
            //    //model.Currencies = ds.PopulateCurrencies();
            //    //model.catgories = ds.GetGatgories();

            //    if (ModelState.IsValid)
            //    {

            //        String Accountnumber = model.AccountNumber;
            //        String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
            //        if (response.Equals("This Account is Already exist"))
            //        {
            //            Session["modelCategoryCode"] = model.CategoryCode;
            //            return RedirectToAction(actionName: "newCustomerAccount", routeValues: new { Account = Accountnumber  , Userlog = model.placeholder });
            //        }
            //        else
            //        {
            //            ModelState.AddModelError("", "Please Check Customer Information");
            //            TempData["Success"] = true;
            //            ViewBag.ResponseStat = "Successfully Sent";
            //            ViewBag.ResponseMSG = "Thank you for Contacting us";
            //        }
            //    }
            //    else
            //    {
            //        message = "All Fields are required ";
            //        ModelState.AddModelError("", "Something is missing" + message);
            //        TempData["Success"] = true;
            //        ViewBag.ResponseStat = "Successfully Sent";
            //        ViewBag.ResponseMSG = "Thank you for Contacting us";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    message = "Please Contact for Support";
            //    ModelState.AddModelError("", "Something is missing" + message);
            //}


            //return View(model);


            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string message = "";
            ModelState.Clear();
            try
            {
                String userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                if (ModelState.IsValid)
                {

                    // verifying if account already exists
                    string tempnumber = model.AccountNumber;
                    string shortaccount = model.AccountNumber;
                    Session["username"] = model.AccountNumber;
                    //while (tempnumber.Length != 11)
                    //{
                    //    tempnumber = "0" + tempnumber;
                    //}

                    List<CustomerRegBankinfo> info = new List<CustomerRegBankinfo>();
                    string custNo = "";
                    String databaseresponse = "";

                    if (!string.IsNullOrEmpty(model.CategoryCode))
                    {
                        if (model.CategoryCode == "1")
                        {
                            model.AccountNumber = tempnumber;
                            custNo = ds.getCustNoFromRim(model.AccountNumber);
                            databaseresponse = ds.custregcheckperaddlink(custNo, model.AccountNumber);
                        }

                        else
                        {
                            model.AccountNumber = tempnumber;
                            info = ds.getCustNoFromRimCorp(model.AccountNumber , model.CategoryCode );
                            string v = info[0].AccountNumberAdded;
                            string c = info[0].CustomerID;
                            databaseresponse = ds.custregcheckforlink(v, model.AccountNumber , c);
                        }


                    }


                    //model.AccountNumber = tempnumber;
                    //string custNo = ds.getCustNoFromRim(model.AccountNumber);
                    //String databaseresponse = ds.custregcheckforlink(custNo, model.AccountNumber);

                    if (databaseresponse.Equals("This Account is available"))
                    {
                        string rim = model.AccountNumber;
                        Session["Account"] = rim;
                        Session["branchcode"] = model.BranchCode;
                        Session["shortaccount"] = shortaccount;
                        string customerbranchcode = "N/A";
                        string customeraccounttypecode = "N/A";
                        string customerbranch = "N/A";
                        string customeraccounttype = "N/A";
                        String custname = "N/A";


                        string apiresponse = "N/A";

                        if (!string.IsNullOrEmpty(model.CategoryCode))
                        {


                            if (model.CategoryCode == "1")
                            {
                                //string apiresponse = Connecttocore.getCustomerInfo(rim, "10000000052", "CUR", "SDG", "005", Session["accesstoken"].ToString());

                                apiresponse = Connecttocore.getCustomerInfoByRim(model.AccountNumber, Session["accesstoken"].ToString());

                            }
                            else
                            {
                                apiresponse = Connecttocore.getCustomerInfoByRimforCorp(model.AccountNumber, model.CategoryCode, model.SelectedChannelsID, Session["accesstoken"].ToString());
                            }
                        }

                        //string apiresponse = Connecttocore.getCustomerInfo(rim, "10000000052", "CUR", "SDG", "005", Session["accesstoken"].ToString());

                        //string apiresponse = Connecttocore.getCustomerInfoByRim(model.AccountNumber, Session["accesstoken"].ToString());

                        JObject customerInfo = new JObject();
                        customerInfo = JObject.Parse(apiresponse);

                        int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                        if (responseCode == 0)
                        {
                            //custname = customerInfo.GetValue("customerName").ToString();
                            Session["custID"] = "";
                            if (!String.IsNullOrEmpty(customerInfo.GetValue("Customer_Name_AR").ToString()))
                            {
                                Session["custnamear"] = customerInfo.GetValue("Customer_Name_AR").ToString();
                            }

                            if (!String.IsNullOrEmpty(customerInfo.GetValue("Customer_Name_EN").ToString()))
                            {
                                Session["custnameen"] = customerInfo.GetValue("Customer_Name_EN").ToString();
                            }
                            Session["custAddress"] = customerInfo.GetValue("Address").ToString();
                            Session["custemail"] = customerInfo.GetValue("Email");
                            Session["rim"] = customerInfo.GetValue("RIM").ToString();
                            JArray Account_Info = new JArray();
                            Account_Info = (JArray)customerInfo.GetValue("Accounts_List");
                            List<AccountDetails> accountDetails = new List<AccountDetails>();
                            foreach (JObject account in Account_Info)
                            {
                                if (account.GetValue("IBAN") != null)
                                {
                                    accountDetails.Add(new AccountDetails
                                    {
                                        Account_No = account.GetValue("Account_No").ToString(),
                                        Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
                                        Branch_Code = account.GetValue("Branch_Code").ToString(),
                                        Currency_Code = account.GetValue("Currency_Code").ToString(),
                                        IBAN = account.GetValue("IBAN").ToString()
                                    });
                                }
                                else
                                {
                                    accountDetails.Add(new AccountDetails
                                    {
                                        Account_No = account.GetValue("Account_No").ToString(),
                                        Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
                                        Branch_Code = account.GetValue("Branch_Code").ToString(),
                                        Currency_Code = account.GetValue("Currency_Code").ToString()
                                    });
                                }
                            }


                            Session["accountDetails"] = accountDetails; //jarray
                            JArray phoneNumbers = (JArray)customerInfo.GetValue("Phones");
                            List<SelectListItem> availablephonenumbers = new List<SelectListItem>();
                            foreach (JObject phonenumber in phoneNumbers)
                            {
                                availablephonenumbers.Add(new SelectListItem
                                {
                                    Text = phonenumber.GetValue("Phone_No").ToString(),
                                    Value = phonenumber.GetValue("Phone_No").ToString()
                                });
                            }

                            Session["custphone"] = availablephonenumbers; //selectlistitem
                            //
                            //customerbranchcode = customerInfo.GetValue("customerBranch").ToString();
                            //customeraccounttypecode = customerInfo.GetValue("customerAccountType").ToString();
                            //customerbranch = ds.getbranchnameenglish(customerbranchcode);
                            //customeraccounttype = ds.getaccounttype(customeraccounttypecode);




                            //Session["customerbranch"] = customerbranch;
                            //Session["customeraccounttype"] = customeraccounttype;
                            //Session["fullaccountnumber"] = "35" + customerbranchcode + customeraccounttypecode + "001" + model.AccountNumber;
                            Session["custcat"] = model.CategoryCode;
                            Session["service"] = "3";
                            // logging activity
                            string usershorthand = model.AccountNumber;
                            //string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                            //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer inquery", usershorthand + " - " + custname, DateTime.Now.ToString());
                            return RedirectToAction("custinfo");
                        }
                        else
                        {
                            message = customerInfo.GetValue("Response_Message").ToString();
                            ModelState.AddModelError("", message);
                            return View(model);
                        }
                    } 
                    else
                    {
                        message = databaseresponse;
                        ModelState.AddModelError("", message);

                    }
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
        public ActionResult Addprocess(CustomerRegBankinfo passedmodel)
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
            if (passedmodel.placeholder != null)
            {

                model = new CustomerRegBankinfo();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserRegistrationData(passedmodel.placeholder);

                model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.placeholder);
                model.AccTypes = ds.PopulateAccountTypes(model.AccountType);
                model.Currencies = ds.PopulateCurrencies();

                model.catgories = ds.GetGatgories();
                //model.Channels = ds.Channels();

                return View("Add", model);
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



        public ActionResult AddAction(CustomerRegBankinfo model)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var rim = model.rim;
            var cat = model.CategoryCode;
            string message = "";
            model = ds.GetUserRegistrationDatalink(model.rim , cat);

            if (model.status.ToString().Equals("UA"))
            {
                message = "This Customer Account is Un Autherized, please Autherized it";
                ModelState.AddModelError("", message);




                return View("Add", model);
            }
            if (model.status.ToString().Equals("DE"))
            {
                message = "This Customer Account is Deactivated, please activate it";
                ModelState.AddModelError("", message);




                return View("Add", model);
            }

            if (model.AccountNumber == null)
            {
                Session["adderror"] = "This customer is not registered";
            }


            model.Branches = ds.PopulateBranchslink( model.AccountNumber);
            model.AccTypes = ds.PopulateAccountTypeslink(model.AccountNumber);
            model.Currencies = ds.PopulateCurrencies();


            //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
            String Accountnumber = "11" + model.BranchCode + model.AccountTypecode + model.AccountNumber + model.SUBNO + model.CurrencyCode + model.SUBGL;
            //String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
            String response = ds.custregcheck2(model.AccountNumber, cat);

            if (response.Equals("This Account is Already exist"))
            {
                Session["modelCategoryCode"] = model.CategoryCode;
                return RedirectToAction(actionName: "newCustomerAccount", routeValues: new { Account = model.AccountNumber, Userlog  = model.CustomerID , Rim = rim }); //Accountnumber
            }
            else
            {


                ModelState.AddModelError("", "Please Check Customer Information");
                TempData["Success"] = true;
                ViewBag.ResponseStat = "Successfully Sent";
                ViewBag.ResponseMSG = "Thank you for Contacting us";


            }
            return RedirectToAction("Add");
        }

        public ActionResult newCustomerAccount(String Account , String Userlog , String Rim) // String Userlog
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

            //String userlog = Session["userlog"].ToString();
            Session["Accountold"] = Account;
            String CategoryCode = "1";

            custinfo infomodel = ds.getcustinfo(Userlog, Account);

            String name = infomodel.user_name;
            String username = infomodel.user_log;
            //string sourcebranch = ds.getbranchnameenglish(infomodel.def_account.Substring(2, 3));
            //string sourceaccounttype = ds.getaccounttype(infomodel.def_account.Substring(5, 5));
            ViewBag.name = name;
           // Session["sourcebranch"] = sourcebranch;
            //Session["sourceaccounttype"] = sourceaccounttype;
            ViewBag.username = username;

            Session["Accountoldname"] = "";
            Session["Accountoldname"] = name;
            Session["Accountoldusername"] = "";
            Session["Accountoldusername"] = username;
            Session["modelCategoryCode"] = CategoryCode;
            Session["custID"] = Userlog;

            Session["Rim"] = Rim;
            model.rim = Rim;


            CustomerRegBankinfo model1 = checknewCustomerAccount(model);

      
            String userbranch = Session["user_branch"].ToString();
            if (name != "")
            {
                model = ds.GetUserRegistrationData(username);

                model.Branches = ds.PopulateBranchslink(Account);
                model.AccTypes = ds.PopulateAccountTypeslink(username);
                model.Currencies = ds.PopulateCurrencies();
            }
            else
            {
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
            }
            return RedirectToAction("custinfo");
            //return View(model);

        }

        [HttpPost]
        public ActionResult newCustomerAccount(CustomerRegBankinfo model, string command)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String CategoryCode = Session["modelCategoryCode"].ToString();
            model.CategoryCode = CategoryCode;

            ViewBag.name = Session["Accountoldname"].ToString();

            ViewBag.username = Session["Accountoldusername"].ToString();
            if (command == "Check")
            {
               /* String name*/
                CustomerRegBankinfo model1 = checknewCustomerAccount(model);
                List<SelectListItem> list = new List<SelectListItem>();
                list = model1.CustomerAccounts;
                for (int i = 0; i < list.Count; i++)
                {
                    if (model1.CustomerName != "No Customer Found")
                    {
                        // do stuff  
                        ViewBag.msg = model1.CustomerName;
                    }

                    else
                        ModelState.AddModelError("", model1.CustomerName);
                }
                return View(model1);
            }
            else
                if (command == "Add")
            {


                String message;
                //  account model;
                try
                {
                    String userbranch = Session["user_branch"].ToString();


                    //model.Branches = ds.PopulateBranchs(userbranch);
                    //model.AccTypes = ds.PopulateAccountTypes();
                    //model.Currencies = ds.PopulateCurrencies();

                    //var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                    //var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                    //var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
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


                    if (ModelState.IsValid)
                    {
                        //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                        //model.AccountNumber = model.AccountNumber.Substring(2);
                        String Accountnumber = "35" + Session["branchcode"].ToString() + Session["accounttypecode"].ToString() + "001" + model.AccountNumber;

                        String result2 = ds.addnewacount(Session["Accountold"].ToString(), Accountnumber, CategoryCode);
                        custinfo customerinformations = ds.getcustinfo( "", Session["Accountold"].ToString());
                        //string response = core.sendpredefinedsms(customerinformations.user_id, model.AccountNumber,"5", customerinformations.user_mobile);
                        String res = " " + Accountnumber.Substring(13) + " : " + result2;

                        string custname = Session["Accountoldname"].ToString();//ds.getcustomerfullname(Accountnumber);
                        //string usershorthand = "11" + model.BranchCode + model.AccountNumber;
                        string usershorthand = model.AccountNumber;
                        string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Account link request", usershorthand + " - " + custname, DateTime.Now.ToString());

                        Session["addaccountresult"] = res;
                        return RedirectToAction("Add");
                        // return RedirectToAction(actionName: "newCustomerAccount", routeValues: new { Account = Accountnumber });

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
            else
            {
                ModelState.AddModelError("", "Please check one of buttons");
                return View(model);
            }
        }


        [HttpPost]

        public CustomerRegBankinfo checknewCustomerAccount(CustomerRegBankinfo model)   //CustomerRegBankinfo
        {
            String message;
            //  account model;
            try
            {

                String userbranch = Session["user_branch"].ToString();
                String CategoryCode = Session["modelCategoryCode"].ToString();
                String custname;
                String custID;
                String custphone;
                string custAddress;

                string customerbranchcode = "N/A";
                string customeraccounttypecode = "N/A";
                string customerbranch = "N/A";
                string customeraccounttype = "N/A";
                model.CategoryCode = CategoryCode;
                if (ModelState.IsValidField("AccountNumber"))
                {

                    string accesstoken = Session["accesstoken"].ToString();
                    //String Accountnumber = "35" + model.AccountNumber;
                    string apiresponse = Connecttocore.getCustomerInfo(model.rim, accesstoken);
                    JObject customerInfo = new JObject();
                    customerInfo = JObject.Parse(apiresponse);
                    List<SelectListItem> accounts = new List<SelectListItem>();
                    int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                    if (responseCode == 0)
                    {
                        //custname = customerInfo.GetValue("customerName").ToString();
                        Session["custID"] = "";
                        if (!String.IsNullOrEmpty(customerInfo.GetValue("Customer_Name_AR").ToString()))
                        {
                            Session["custnamear"] = customerInfo.GetValue("Customer_Name_AR").ToString();
                        }

                        if (!String.IsNullOrEmpty(customerInfo.GetValue("Customer_Name_EN").ToString()))
                        {
                            Session["custnameen"] = customerInfo.GetValue("Customer_Name_EN").ToString();
                        }
                        Session["custAddress"] = customerInfo.GetValue("Address").ToString();
                        Session["custemail"] = customerInfo.GetValue("Email");
                        Session["rim"] = customerInfo.GetValue("RIM").ToString();
                        JArray Account_Info = new JArray();
                        Account_Info = (JArray)customerInfo.GetValue("Accounts_List");
                        List<AccountDetails> accountDetails = new List<AccountDetails>();
                        foreach (JObject account in Account_Info)
                        {
                            if (account.GetValue("IBAN") != null)
                            {
                                accountDetails.Add(new AccountDetails
                                {
                                    Account_No = account.GetValue("Account_No").ToString(),
                                    Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
                                    Branch_Code = account.GetValue("Branch_Code").ToString(),
                                    Currency_Code = account.GetValue("Currency_Code").ToString(),
                                    IBAN = account.GetValue("IBAN").ToString()
                                });
                             
                                //model.CustomerAccounts = new List<SelectListItem>();
                                //string customeraccounts = result.CustomerAccounts;
                               // string[] accounts = customeraccounts.Split('-');
                             
                               



                            }
                            else
                            {
                                accountDetails.Add(new AccountDetails
                                {
                                    Account_No = account.GetValue("Account_No").ToString(),
                                    Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
                                    Branch_Code = account.GetValue("Branch_Code").ToString(),
                                    Currency_Code = account.GetValue("Currency_Code").ToString()
                                });
                            }
                            //for (int i = 0; i < accountDetails.Count; i++)
                            //{
                            //    string AccountNumber1 = account.GetValue("Account_No").ToString();
                            //    string AccountType = account.GetValue("Account_Type_Code").ToString();
                            //    string Currency = account.GetValue("Currency_Code").ToString();
                            //    string BranchName = ds.getbranchnameenglish(account.GetValue("Branch_Code").ToString());
                            //    //Currency = ds.GetCurrencyName(accounts[i].ToString().Substring(10, 3));
                            //    //List<SelectListItem> accounts = new List<SelectListItem>();

                            //    model.CustomerAccounts.Add(new SelectListItem
                            //    {
                            //        Text = BranchName + " - " + AccountType + " - " + Currency + " - " + AccountNumber1,
                            //        Value =   AccountNumber1 //accounts[i],
                            //    });

                            //}
                           
                        }


                        Session["accountDetails"] = accountDetails; //jarray
                        JArray phoneNumbers = (JArray)customerInfo.GetValue("Phones");
                        List<SelectListItem> availablephonenumbers = new List<SelectListItem>();
                        foreach (JObject phonenumber in phoneNumbers)
                        {
                            availablephonenumbers.Add(new SelectListItem
                            {
                                Text = phonenumber.GetValue("Phone_No").ToString(),
                                Value = phonenumber.GetValue("Phone_No").ToString()
                            });
                        }

                        Session["custphone"] = availablephonenumbers;

                         custname = Session["custnameen"].ToString();
                        //Session["customerbranch"] = customerbranch;
                        //Session["customeraccounttype"] = customeraccounttype;
                        //Session["fullaccountnumber"] = "35" + customerbranchcode + customeraccounttypecode + "001" + model.AccountNumber;
                        Session["custcat"] = "1";
                        Session["service"] = "3";
                        // logging activity
                        string usershorthand = model.AccountNumber;
                        //string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                        //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer inquery", usershorthand + " - " + custname, DateTime.Now.ToString());
                       // return RedirectToAction("custinfo");
                        // return model; //custname;
                         return model;
                    }

                    else
                    {
                        message = customerInfo.GetValue("Response_Message").ToString();
                        ModelState.AddModelError("", message);
                       // return View(model);
                    }
                    //JObject customerInfo = new JObject();
                    //customerInfo = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(customerInfo.GetValue("responseCode").ToString());
                    //if (responseCode == 200)
                    //{
                    //    custID = "";
                    //    custname = customerInfo.GetValue("customerName").ToString();
                    //    custphone = customerInfo.GetValue("customerMobile").ToString();
                    //    custAddress = customerInfo.GetValue("customerAddress").ToString();
                    //    customerbranchcode = customerInfo.GetValue("customerBranch").ToString();
                    //    customeraccounttypecode = customerInfo.GetValue("customerAccountType").ToString();
                    //    customerbranch = ds.getbranchnameenglish(customerbranchcode);
                    //    customeraccounttype = ds.getaccounttype(customeraccounttypecode);
                    //    Session["custID"] = custID;
                    //    Session["custname"] = custname;
                    //    Session["custphone"] = custphone;
                    //    Session["branchcode"] = customerbranchcode;
                    //    Session["accounttypecode"] = customeraccounttypecode;
                    //    Session["branch"] = customerbranch;
                    //    Session["accounttype"] = customeraccounttype;

                    //    ViewBag.custname = custname;
                    //    ViewBag.custbranch = customerbranch;
                    //    ViewBag.custaccounttype = customeraccounttype;

                    //    return custname;
                    //}
                    //else
                    //{
                    //    ModelState.AddModelError("", "Please Check Customer Information");
                    //}
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                    model.CustomerName = "No Customer Found";
                   // return model; //"No Customer Found";
                   return model;
                }
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                model.CustomerName = "No Customer Found";
                //return model; //"No Customer Found";
                return model;
            }

            //return model; //"No Customer Found";
            return model;
        }

        public ActionResult Delete()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String userbranch = "";
            if (Session["deleleaccountresult"] != null)
            {
                ViewBag.SuccessMessage = Session["deleleaccountresult"].ToString();
                Session["deleleaccountresult"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            if (Session["user_branch"].ToString() != null)
            {
                userbranch = Session["user_branch"].ToString();
            }
            else
            {
                RedirectToAction("Index", "Home");
            }

            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();
            return View(model);

        }

        [HttpPost]
        public ActionResult Delete(CustomerRegBankinfo model)
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
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                //model.catgories.RemoveAt(0);
                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedAccType != null)
                {
                    selectedAccType.Selected = true;

                }
                if (selectedCurrency != null)
                {
                    selectedCurrency.Selected = true;

                }
                if (selectedcategory != null)
                {
                    selectedcategory.Selected = true;

                }

                if (ModelState.IsValid)
                {
                    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + "001" + model.AccountNumber;
                    Session["accounttodeleteaccountfrom"] = Accountnumber;
                    String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
                    if (response.Equals("This Account is Already exist"))
                    {
                        Session["modelCategoryCode"] = model.CategoryCode;
                        return RedirectToAction(actionName: "deleteCustomerAccount", routeValues: new { Account = Accountnumber });
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
        public ActionResult Deleteprocess(CustomerRegBankinfo passedmodel)
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
            if (passedmodel.CustomerID != null)
            {
                model = new CustomerRegBankinfo();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserRegistrationData(passedmodel.CustomerID);

                model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.CustomerID);
                model.AccTypes = ds.PopulateAccountTypes(passedmodel.CustomerID);
                model.Currencies = ds.PopulateCurrencies();

                model.catgories = ds.GetGatgories();
                model.Channels = ds.Channels();
                return View("Delete", model);
            }
            else
            {
                String userbranch = "";
                if (Session["deleleaccountresult"] != null)
                {
                    ViewBag.SuccessMessage = Session["deleleaccountresult"].ToString();
                    Session["deleleaccountresult"] = null;
                }

                userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                return View("Delete", model);
            }
        }

        public ActionResult DeleteAction(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            model = ds.GetUserRegistrationData(model.CustomerID);
            if (model.AccountNumber == null)
            {
                Session["deleteerror"] = "This customer is not registered.";
            }
            model.Branches = ds.PopulateBranchs(model.BranchCode, model.CustomerID);
            model.AccTypes = ds.PopulateAccountTypes(model.CustomerID);
            model.Currencies = ds.PopulateCurrencies();
            //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
            String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + "001" + model.AccountNumber;
            Session["accounttodeleteaccountfrom"] = Accountnumber;
            String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
            if (response.Equals("This Account is Already exist"))
            {
                Session["modelCategoryCode"] = model.CategoryCode;
                return RedirectToAction(actionName: "deleteCustomerAccount", routeValues: new { Account = Accountnumber });
            }
            else
            {
                ModelState.AddModelError("", "Please Check Customer Information");
                TempData["Success"] = true;
                ViewBag.ResponseStat = "Successfully Sent";
                ViewBag.ResponseMSG = "Thank you for Contacting us";
            }
            return RedirectToAction("Delete");
        }

        public ActionResult deleteCustomerAccount(String Account)
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

            Session["Accountold"] = "";
            Session["Accountold"] = Account;
            String CategoryCode = Session["modelCategoryCode"].ToString();

            custinfo infomodel = ds.getcustinfo( CategoryCode, Account);
            String name = infomodel.user_name;
            String username = infomodel.user_log;
            ViewBag.name = name;
            ViewBag.username = username;
            Session["Accountoldname"] = "";
            Session["Accountoldname"] = name;
            Session["Accountoldusername"] = "";
            Session["Accountoldusername"] = username;
            Session["modelCategoryCode"] = CategoryCode;


            String userbranch = Session["user_branch"].ToString();
            if (name != "")
            {
                model = ds.GetUserRegistrationData(username);

                model.Branches = ds.PopulateBranchs(model.BranchCode, username);
                model.AccTypes = ds.PopulateAccountTypes(username);
                model.Currencies = ds.PopulateCurrencies();
            }

            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();


            return View(model);

        }

        [HttpPost]
        public ActionResult deleteCustomerAccount(CustomerRegBankinfo model, string command)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String CategoryCode = Session["modelCategoryCode"].ToString();
            model.CategoryCode = CategoryCode;

            ViewBag.name = Session["Accountoldname"].ToString();

            ViewBag.username = Session["Accountoldusername"].ToString();
            if (command == "Check")
            {
                String name = checkdeleteCustomerAccount(model);
                if (name != "No Customer Found")
                {
                    // do stuff  
                    ViewBag.msg = name;
                    return View(model);
                }
                else
                    ModelState.AddModelError("", name);
                return View(model);
            }
            else
                if (command == "Delete")
            {


                String message;
                //  account model;
                try
                {
                    String userbranch = Session["user_branch"].ToString();


                    model.Branches = ds.PopulateBranchs(userbranch);
                    model.AccTypes = ds.PopulateAccountTypes();
                    model.Currencies = ds.PopulateCurrencies();

                    var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                    var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                    var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                    if (selectedBranch != null)
                    {
                        selectedBranch.Selected = true;

                    }
                    if (selectedAccType != null)
                    {
                        selectedAccType.Selected = true;

                    }
                    if (selectedCurrency != null)
                    {
                        selectedCurrency.Selected = true;

                    }


                    if (ModelState.IsValidField("BranchCode") && ModelState.IsValidField("AccountTypecode") && ModelState.IsValidField("CurrencyCode") && ModelState.IsValidField("AccountNumber"))
                    {
                        //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                        String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + "001" + model.AccountNumber;

                        String result2 = ds.deleteaccount(Session["Accountold"].ToString(), Accountnumber, CategoryCode);
                       // custinfo customerinformations = ds.getcustinfo("", "", "", "", "", Session["Accountold"].ToString());
                       // string response = core.sendpredefinedsms(customerinformations.user_id, model.AccountNumber, "7", customerinformations.user_mobile);

                        String res = " " + Accountnumber.Substring(11, 7) + " : " + result2;

                        string custname = Session["Accountoldname"].ToString();//ds.getcustomerfullname(Accountnumber);
                        //string usershorthand = "11" + model.BranchCode + model.AccountNumber;
                        string usershorthand = model.AccountNumber;
                        string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Account link request", usershorthand + " - " + custname, DateTime.Now.ToString());

                        Session["deleleaccountresult"] = res;
                        return RedirectToAction("Delete");
                        // return RedirectToAction(actionName: "newCustomerAccount", routeValues: new { Account = Accountnumber });

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
            else
            {
                ModelState.AddModelError("", "Please check one of buttons");
                return View(model);
            }
        }


        [HttpPost]

        public String checkdeleteCustomerAccount(CustomerRegBankinfo model)
        {

            String message;
            //  account model;
            try
            {
                String userbranch = Session["user_branch"].ToString();


                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();

                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                //     var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedAccType != null)
                {
                    selectedAccType.Selected = true;

                }
                if (selectedCurrency != null)
                {
                    selectedCurrency.Selected = true;

                }
                //if (selectedcategory != null)
                //{
                //    selectedcategory.Selected = true;

                //}

                String CategoryCode = Session["modelCategoryCode"].ToString();
                model.CategoryCode = CategoryCode;
                if (ModelState.IsValidField("BranchCode") && ModelState.IsValidField("AccountTypecode") && ModelState.IsValidField("CurrencyCode") && ModelState.IsValidField("AccountNumber"))
                {

                    while (model.AccountNumber.ToString().Length != 7)
                    {
                        model.AccountNumber = "0" + model.AccountNumber;
                    }
                    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + "001" + model.AccountNumber;

                    string userid = ds.getCustIDFromAcc(Session["accounttodeleteaccountfrom"].ToString());
                    Boolean doseitbelongsto = ds.checkaccountbelongstouser(userid, Accountnumber);
                    if (doseitbelongsto)
                    {
                        String response = Connecttocore.GetCustinfo(Accountnumber);
                        JObject jobj = new JObject();
                        jobj = JObject.Parse(response);
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
                        String custAddress;

                        if (acc.Length >= 10)
                        {
                            custID = acc[1].ToString();
                            custname = acc[3].ToString();
                            custphone = acc[5].ToString();
                            custAddress = acc[7].ToString();

                            Session["custID"] = custID;
                            Session["custname"] = custname;
                            Session["custphone"] = custphone;

                            ViewBag.custname = custname;
                            return custname;
                            // return RedirectToAction(actionName: "newCustomerAccount", routeValues: new { Account = Accountnumber });
                        }
                        else
                        {
                            ModelState.AddModelError("", "Please Check Customer Information");
                        }
                    }
                    else
                    {
                        message = " this account : '" + model.AccountNumber + "' is not added to this specific users";
                        ModelState.AddModelError("", "Something is wrong" + message);
                        return "Account not added to this specific users";
                    }
                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                    return "No Customer Found";
                }
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                return "No Customer Found";
            }

            return "No Customer Found";
        }

        public ActionResult CustomerAccounts(String Account)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            try
            {
                string response = Connecttocore.GetCustaccounts(Account);
                //"{'1':{'ACT_C_NAME':'¿¿¿¿ ¿¿¿¿¿ ¿¿¿¿ ¿¿¿¿¿¿¿¿','CURRENCY_C_CODE':'001','CUST_I_NO':'467','MOBILE_C_NO':'0','BRANCH_C_CODE':'004','ACT_C_TYPE':'20102'},'tranDateTime':'180118082930','NoOfAct':2,'2':{'ACT_C_NAME':'¿¿¿¿ ¿¿¿¿¿ ¿¿¿¿ ¿¿¿¿¿¿¿¿','CURRENCY_C_CODE':'001','CUST_I_NO':'82','MOBILE_C_NO':'0','BRANCH_C_CODE':'004','ACT_C_TYPE':'20105'},'uuid':'d0088690-368a-4737-a9e0-5f330add73c1','errormsg':'Successfully','errorcode':'1'}";//Connecttocore.GetCustaccounts(Account);
                JObject jobj = new JObject();
                jobj = JObject.Parse(response);
                dynamic result = jobj;
                List<addaccount> items = new List<addaccount>();
                string errormsg = result.errormsg;
                string errorcode = result.errorcode;
                string NoOfAct = result.NoOfAct;
                if (errorcode.Equals("1") && !NoOfAct.Equals("0"))
                {
                    int index = Convert.ToInt32(NoOfAct);
                    for (int i = 1; i <= index; i++)
                    {

                        try
                        {

                            JToken singlerow = result[i.ToString()];
                            JObject newObj = new JObject();
                            dynamic singleObj = singlerow;
                            String Branchname = singleObj.BRANCH_C_CODE;
                            String AccountTypename = singleObj.ACT_C_TYPE;// ds.getaccounttype(singleObj.ACT_C_TYPE);
                            String Currencyname = singleObj.CURRENCY_C_CODE;// ds.getcurrencyname(singleObj.CURRENCY_C_CODE);
                            String accno = singleObj.CUST_I_NO;
                            if (!Account.Substring(13).Equals(accno))
                            {
                                items.Add(new addaccount
                                {
                                    AccountID = i + 1,
                                    AccountNumber = singleObj.CUST_I_NO,
                                    AccountNumbercomplete = "13" + singleObj.BRANCH_C_CODE + singleObj.ACT_C_TYPE + singleObj.CURRENCY_C_CODE + singleObj.CUST_I_NO,
                                    Branch = ds.getbranchnameenglish(Branchname),
                                    AccountType = ds.getaccounttype(AccountTypename),
                                    Currency = ds.getcurrencyname(Currencyname),
                                    IsSelected = false,
                                });
                            }
                            Session["Accountold"] = Account;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "System Error");

                        }



                    }

                    accountsresult accountsresult = new accountsresult();
                    accountsresult.accountSelected = items;
                    return View(accountsresult);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "System Error");

            }
            return View();
        }
        [HttpPost]
        public ActionResult CustomerAccounts(accountsresult model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String result = "", res = "";
            List<addaccount> lHob = new List<addaccount>();
            lHob = model.accountSelected;
            foreach (var item in lHob)
            {
                if (item.IsSelected == true)
                {
                    result = ds.addnewacount(Session["Accountold"].ToString(), item.AccountNumbercomplete, Session["modelCategoryCode"].ToString());
                    res += " " + item.AccountNumbercomplete + " : " + result;

                }
                Session["addaccountresult"] = res;
            }
            return RedirectToAction("Add");
        }


        public ActionResult Authorizer()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["authresult"] != null)
            {
                ViewBag.SuccessMessage = Session["authresult"].ToString();
                Session["authresult"] = null;
            }
            String branchcode = Session["user_branch"].ToString();
            List<pendingacts> customer = new List<pendingacts>();
            customer = ds.Pendingacounts(branchcode);
            return View(customer);
        }

        public ActionResult AuthorizeAll()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["authresult"] != null)
            {
                ViewBag.SuccessMessage = Session["authresult"].ToString();
                Session["authresult"] = null;
            }
            String branchcode = Session["user_branch"].ToString();
            Session["allpending"] = true;
            List<pendingacts> customer = new List<pendingacts>();
            customer = ds.AllPendingAccounts();
            return View(customer);
        }

        public ActionResult Details(int id, String act)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            actAuthorizationinfo model = new actAuthorizationinfo();
            Session["CustomerBranchCode"] = act.Substring(2, 3);
            List<actAuthorizationinfo> customer = new List<actAuthorizationinfo>();
            customer = ds.newactAuthorizationinfo(id.ToString(), act);

            Session["customer"] = customer;
            foreach (var item in customer)
            {
                model.Branch = item.Branch;
                model.AccountType = item.AccountType;
                model.Customername = item.Customername;
                model.Currency = item.Currency;
                model.Customeraccount = item.Customeraccount;
                model.completeact = item.completeact;
                model.userid = item.userid;

            }
            model.authsts = "true";
            model.rjtsts = "false";
            Session["model"] = model;
            return View(model);
        }

        public ActionResult Authorize(int id, String act)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int response = ds.updateAccount(id.ToString(), act, "A");
            if (response != -1)
            {
                actAuthorizationinfo model = new actAuthorizationinfo();
                model = (actAuthorizationinfo)Session["model"];
                string custname = model.Customername;

                //sending customer predefined sms
                //getting customer information
                custinfo customerinformations = ds.getcustinfobyid(id.ToString());
                string response2 = core.sendpredefinedsms(customerinformations.user_id, act.Substring(13), "4", customerinformations.user_mobile);
                //
                //string usershorthand = "11" + Session["CustomerBranchCode"].ToString() + model.Customeraccount;
                string usershorthand = model.Customeraccount;
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Authorized account link", usershorthand + " - " + custname, DateTime.Now.ToString());

                Session["authresult"] = "Account Authorization Completed Successfuly";
                TempData["authorizedsuccess"] = "Account Authorization Completed Successfully.";
                //return RedirectToAction("Index", "Home", new { area = "" });
                if (Session["allpending"] != null)
                {
                    return RedirectToAction("AuthorizeAll");
                }
                else
                {
                    return RedirectToAction("Authorizer");
                }
            }
            else
            {
                TempData["authorizedfail"] = "Account Authorization failed.";
                return RedirectToAction(actionName: "Details", routeValues: new { id = id, act = act });

                //return RedirectToAction("Details", id, act);
            }
        }
        public ActionResult Reject(int id, String act)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int response = ds.updateAccount(id.ToString(), act, "R");
            if (response != -1)
            {
                actAuthorizationinfo model = new actAuthorizationinfo();
                model = (actAuthorizationinfo)Session["model"];
                string custname = model.Customername;
                //string usershorthand = "11" + Session["CustomerBranchCode"].ToString() + model.Customeraccount;
                string usershorthand = model.Customeraccount;
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Rejected account link", usershorthand + " - " + custname, DateTime.Now.ToString());

                Session["authresult"] = "Reject Completed Successfully";
                TempData["rejected"] = "Rejection Completed Successfully";
                // return RedirectToAction("Index", "Home", new { area = "" });
                return RedirectToAction("Authorizer");
            }
            else
            {
                return RedirectToAction(actionName: "Details", routeValues: new { id = id, act = act });

                //return RedirectToAction("Details", id, act);
            }
        }

        public ActionResult custinfo()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            CustomerRegBankinfo2 model = new CustomerRegBankinfo2();
            model.CustomerID = Session["custID"].ToString();
            model.CustomerNameEN = Session["custnameen"].ToString();
            model.CustomerNameArabic = Session["custnamear"].ToString();
            model.AvailablePhoneNumbers = (List<SelectListItem>)Session["custphone"];
            model.CustomerAddress = Session["custAddress"].ToString();
            model.Email = Session["custemail"].ToString();
            model.RIM = Session["rim"].ToString();

            model.accountDetails = (List<AccountDetails>)Session["accountDetails"];

            foreach (AccountDetails account in model.accountDetails)
            {
                model.AvailableCustomerAccount.Add(new SelectListItem
                {
                    Text = account.Account_No + " - " + account.Currency_Code,
                    Value = account.Account_No
                });
            }

            model.Profiles = ds.PopulateProfiles();
            model.cat = Session["custcat"].ToString();
            
            return View(model);
        }
        [HttpPost]
        public ActionResult custinfo(CustomerRegBankinfo2 model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }


            model.Profiles = ds.PopulateProfiles();

            model.CustomerID = Session["custID"].ToString();
            model.CustomerNameEN = Session["custnameen"].ToString();
            model.CustomerNameArabic = Session["custnamear"].ToString();
            model.CustomerAddress = Session["custAddress"].ToString();
            model.Email = Session["custemail"].ToString();
            model.RIM = Session["rim"].ToString();
            List<AccountDetails> accountDetails = new List<AccountDetails>();
            //Session["accountDetails"] = accountDetails;
            accountDetails = (List<AccountDetails>)Session["accountDetails"];
            foreach (var item in accountDetails)
            {
                if (item.IBAN != null)
                {
                    if (model.selectedaccount == item.Account_No)
                    {


                        //accountDetails.Add(new AccountDetails
                        //{
                        //    Account_No = item.Account_No.ToString(),
                        //    Account_Type_Code = item.Account_Type_Code.ToString(),
                        //    Branch_Code = item.Branch_Code.ToString(),
                        //    Currency_Code = item.Currency_Code.ToString(),
                        //    IBAN = item.IBAN.ToString()
                        //});


                        Session["Account_No"] = item.Account_No.ToString();
                        Session["Account_Type_Code"] = item.Account_Type_Code.ToString();
                        Session["Branch_Code"] = item.Branch_Code.ToString();
                        Session["Currency_Code"] = item.Currency_Code.ToString();
                        Session["IBAN"] = item.IBAN.ToString();
                    }
                }


            }



            String Account_No = Session["Account_No"].ToString();
            String Account_Type_Code = Session["Account_Type_Code"].ToString();
            String Branch_Code = Session["Branch_Code"].ToString();
            String Currency_Code = Session["Currency_Code"].ToString();
            String IBAN = Session["IBAN"].ToString();


            String username = model.RIM;
            String email = model.Email;
            String address = model.CustomerAddress;
            //String account = model.selectedaccount;
            String customerprofile = model.selectedprofile;
            String CustomerID = Session["custID"].ToString();
            String CustomerName = model.CustomerName;
            String CustomerPhone = model.selectedphonenumber;//Session["custphone"].ToString();
            String customercatgory = Session["custcat"].ToString();
            String CUSTOMERSERVICE = Session["service"].ToString();
            string adminusername = Session["user_log"].ToString();
            string userfullaccount = model.selectedaccount;//Session["fullaccountnumber"].ToString();
                                                           //string apiresponse = Connecttocore.registerCustomer(model.Email, model.selectedphonenumber, model.CustomerAddress, model.CustomerNameArabic, model.CustomerNameEN, model.RIM, model.selectedaccount, Session["accesstoken"].ToString());

            //JObject response = new JObject();
            //response = JObject.Parse(apiresponse);

            //int responseCode = 0;//int.Parse(response.GetValue("Response_Code").ToString());
            string userlog = ds.checkuserbyrim(model.RIM);
            if (model.cat != "1")
            {
                 userlog = ds.checkuserforcorpbyrim(model.RIM, model.cat);
            }
            Boolean resp = true;
            if (!string.IsNullOrEmpty(userlog))
            {
                if (model.cat == "1")
                {
                     resp = ds.checkaccountuser(Account_Type_Code, Account_No,userlog);
             
                }
                else
                {
                     resp = ds.checkaccountuserforcorp(Account_Type_Code, Account_No , userlog);
                  
                }
                //string userlog = ds.checkuser(Account_No);

             //   Boolean resp = ds.checkaccountuser(Account_Type_Code, Account_No);
                if (resp.Equals(false))
                {
                    string res = ds.addnewacountforFrist(userlog, Account_No, Account_Type_Code, Branch_Code, Currency_Code, IBAN , false);


                    Session["message"] = "Customer Added Account Successfully";
                    TempData["success"] = "Customer Added Account Successfully";
                }
                else
                {
                    Session["message"] = " This Customer Account is already linked ";
                    TempData["success"] = "This Customer Account is already linked";
                }
           
                return RedirectToAction("Add");
            }

            else
            {
                //string userlog = ds.checkuser(Account_No);
                //Boolean resp = ds.checkaccountuser(Account_Type_Code, Account_No);
                //if (resp.Equals(false))
                //{
                //    string res = ds.addnewacountforFrist(userlog, Account_No, Account_Type_Code, Branch_Code, Currency_Code, IBAN);

                //}

                Session["messagefalied"] = "Customer Added Account Failed";
                TempData["fail"] = "Customer Added Account Failed";
                return RedirectToAction("Add");
            }

            //int response = ds.custreg(CustomerID, CustomerName, account, userfullaccount, username, address, CustomerPhone, email, customerprofile, customercatgory, CUSTOMERSERVICE, adminusername);

            //if (response.Equals(1))
            //{
            //    string usershorthand = Session["custID"].ToString();
            //    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer registration", usershorthand + " - " + CustomerName, DateTime.Now.ToString());
            //    message = "Customer Registration Completed Successfully";
            //    Session["message"] = message;
            //    // ModelState.AddModelError("", message );
            //    return RedirectToAction("Registration");
            //}
            //else if (response.Equals(2))
            //{
            //    message = "This customer is registered already.";
            //    Session["message"] = message;
            //    ViewBag.SuccessMessage = message;
            //    return View(model);
            //}
            //else
            //{
            //    message = "This customer is registered already.";
            //    ModelState.AddModelError("", message);
            //    return View(model);
            //}
        }

        public ActionResult GetACC()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["addaccountresult"] != null)
            {
                ViewBag.SuccessMessage = Session["addaccountresult"].ToString();
                Session["addaccountresult"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();

            //model.catgories.RemoveAt(0);
            return View(model);

        }
        [HttpPost]
        public ActionResult GetACC(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            String message;
            //  account model;
            try
            {
                String userbranch = Session["user_branch"].ToString();


                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();
                //model.catgories.RemoveAt(0);
                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedAccType = model.AccTypes.Find(p => p.Value == model.AccountTypecode.ToString());
                var selectedCurrency = model.Currencies.Find(p => p.Value == model.CurrencyCode.ToString());
                var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedAccType != null)
                {
                    selectedAccType.Selected = true;

                }
                if (selectedCurrency != null)
                {
                    selectedCurrency.Selected = true;

                }
                if (selectedcategory != null)
                {
                    selectedcategory.Selected = true;

                }

                if (ModelState.IsValid)
                {
                    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode + model.AccountNumber;
                    String Accountnumber = "35" + model.BranchCode + model.AccountTypecode + "001" + model.AccountNumber;
                    String response = ds.custregcheck2(Accountnumber, model.CategoryCode);
                    if (response.Equals("This Account is Already exist"))
                    {
                        Session["modelCategoryCode"] = model.CategoryCode;
                        return RedirectToAction(actionName: "CustomerAccounts", routeValues: new { Account = Accountnumber });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Please Check Customer Information");
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


    }
}