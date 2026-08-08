using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AljazeeraCPanel.Controllers
{
    public class CustomerRegistrationController : Controller
    {
        DataSource ds = new DataSource();

        //
        // GET: /CustomerRegistration/
        public ActionResult Registration()
        {

            //System.Int64 timeout = System.Web.HttpContext.Current.Session.Timeout;

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
            //model.Branches = ds.PopulateBranchs(userbranch);
            //model.AccTypes = ds.PopulateAccountTypes();
            //model.Currencies = ds.PopulateCurrencies();

  
            model.catgories = ds.GetGatgories();
            model.Channels = ds.Channels();
            Session["regmodel"] = model;
            return View(model);

        }


        [HttpPost]
        public ActionResult Registration(CustomerRegBankinfo model)
        {

            
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ///////////////////
            string message = "";

            if (string.IsNullOrEmpty(model.AccountNumber))
            {

                ModelState.AddModelError("", "Customer ID is required.");
                //message = "This Customer Account is Autherized, please Activate it";
                //ModelState.AddModelError("", message);
                //Session["userresultF"] = message;
                return View("Registration", model);


            }
         

            ModelState.Clear();
            try
            {

                String userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();


                model.catgories = ds.GetGatgories();
                model.Channels = ds.Channels();
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


                    ////this update in version 2//////


                    if (!string.IsNullOrEmpty(model.CategoryCode))
                    {
                        if (model.CategoryCode == "1")
                        {
                            model.AccountNumber = tempnumber;
                            custNo = ds.getCustNoFromRim(model.AccountNumber);
                            databaseresponse =  ds.custregcheck(custNo, model.AccountNumber); //"This Account is available";
                        }


                        else
                        {
                            string v = "";
                            string c = "";
                            model.AccountNumber = tempnumber;
                            info = ds.getCustNoFromRimCorp(model.AccountNumber, model.CategoryCode);
                            if (info.Count > 0) {
                                 v = info[0].AccountNumberAdded;
                                 c = info[0].CustomerID;
                            }
                            databaseresponse =   ds.custregcheckforlinkreg(v, model.AccountNumber, c); //"This Account is available"; 
                        }


                    }

                    //model.AccountNumber = tempnumber;
                    //string custNo = ds.getCustNoFromRim(model.AccountNumber);
                    //String databaseresponse = ds.custregcheck(custNo, model.AccountNumber);

                    if (databaseresponse.Equals("This Account is available"))
                    {
                        string rim =  model.AccountNumber;
                        Session["Account"] = rim;
                        Session["branchcode"] = model.BranchCode;
                        Session["shortaccount"] = shortaccount;
                        string customerbranchcode = "N/A";
                        string customeraccounttypecode = "N/A";
                        string customerbranch = "N/A";
                        string customeraccounttype = "N/A";
                        String custname = "N/A";



                        //if(model.CategoryCode = "1")
                        //{

                        //}

                        string apiresponse = "N/A";

                        Session["channel"] = model.SelectedChannelsID;
                        Session["cat"] = model.CategoryCode;

                        //var selectedcategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());


                        if (!string.IsNullOrEmpty(model.CategoryCode))
                        {
                          
                            
                            if (model.CategoryCode == "1")
                            {
                                //string apiresponse = Connecttocore.getCustomerInfo(rim, "10000000052", "CUR", "SDG", "005", Session["accesstoken"].ToString());

                                apiresponse = Connecttocore.getCustomerInfoByRim(model.AccountNumber, Session["accesstoken"].ToString());

                            }
                            else
                            {
                                apiresponse = Connecttocore.getCustomerInfoByRimforCorp(model.AccountNumber,model.CategoryCode , model.SelectedChannelsID , Session["accesstoken"].ToString());
                            }
                        }
                        JObject customerInfo = new JObject();
                        customerInfo = JObject.Parse(apiresponse);

                        int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                        if(responseCode == 0)
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
                            Session["messagefalied"] = "Something is missing or error ";
                            TempData["fail"] = customerInfo.GetValue("Response_Message").ToString();
                            //return RedirectToAction("Registration");
                            message = customerInfo.GetValue("Response_Message").ToString();
                            ModelState.AddModelError("Something is missing or error", message);
                            //return View(model);
                            return RedirectToAction("Registration");
                        }
                    }

                    else
                    {
                        message = databaseresponse;  //"This Account is Already exist ";
                        ModelState.AddModelError("",  message);

                    }

                }
            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", " Something is missing or Wrong" + message);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Registrationprocess(CustomerRegBankinfo model)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            return View("Registration", model);

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

            //model.channel =  Session["channel"].ToString(); 
              model.cat = Session["cat"].ToString() ; 
            
            foreach (AccountDetails account in model.accountDetails)
            {
                model.AvailableCustomerAccount.Add(new SelectListItem
                {
                    Text = account.Account_No + " - " + account.Currency_Code,
                    Value = account.Account_No
                });
            }

            model.Profiles = ds.PopulateProfiles();

            var Selectedprofile = model.Profiles.Find(p => p.Text == model.cat.ToString());
            if (Selectedprofile != null)
            {
                Selectedprofile.Selected = true;

            }



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

            
            //model.Profiles = ds.PopulateProfiles();

            //var Selectedprofile = model.Profiles.Find(p => p.Text == model.selectedprofile.ToString());
            //if (Selectedprofile != null)
            //{
            //    Selectedprofile.Selected = true;

            //}

            model.CustomerID = Session["custID"].ToString();
            model.CustomerNameEN = Session["custnameen"].ToString();
            model.CustomerNameArabic = Session["custnamear"].ToString();
            model.CustomerAddress = Session["custAddress"].ToString();
            model.Email = Session["custemail"].ToString();
            model.RIM = Session["rim"].ToString();

            String[] ch = (String[])Session["channel"];
            model.cat = Session["cat"].ToString();

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


            String username =   model.RIM;
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

            if (model.selectedphonenumber.Substring(0).Equals("0"))
            {
                model.selectedphonenumber = "249" + model.selectedphonenumber.Substring(1);
            }

            string userfullaccount = model.selectedaccount;//Session["fullaccountnumber"].ToString();
            string apiresponse = Connecttocore.registerCustomer(model.Email,model.selectedphonenumber,model.CustomerAddress,model.CustomerNameArabic, model.CustomerNameEN, model.RIM,model.selectedaccount, model.cat, ch, Session["accesstoken"].ToString()   );

            JObject response = new JObject();
            response = JObject.Parse(apiresponse);

            int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
              
            if (responseCode == 0)
            {

                string userlog = ds.checkuser(Account_No);
                if (model.cat != "1")
                {
                    userlog = ds.checkuserforcorp(Account_No, model.cat);
                }
                Boolean resp = true;
                if (model.cat == "1")
                {
                    resp = ds.checkaccountuser(Account_Type_Code, Account_No , userlog);
                }
                else
                {
                    resp = ds.checkaccountuserforcorp(Account_Type_Code, Account_No, userlog);
                }
                //Boolean resp = ds.checkaccountuser(Account_Type_Code, Account_No);
                if (resp.Equals(false))
                {
                    string res = ds.addnewacountforFrist(userlog, Account_No, Account_Type_Code, Branch_Code, Currency_Code, IBAN , true);
                    
                }
                Session["message"] = "Customer Registered Successfully";
                TempData["success"] = "Customer Registered Successfully";
                return RedirectToAction("Registration");
            }

            else
            {
                //string userlog = ds.checkuser(Account_No);
                //Boolean resp = ds.checkaccountuser(Account_Type_Code, Account_No);
                //if (resp.Equals(false))
                //{
                //    string res = ds.addnewacountforFrist(userlog, Account_No, Account_Type_Code, Branch_Code, Currency_Code, IBAN);

                //}
                
                Session["messagefalied"] = response.GetValue("Response_Message").ToString();
                TempData["fail"] = response.GetValue("Response_Message").ToString();
                return RedirectToAction("Registration");
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


        public ActionResult Registrationpersonalinfo()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
           
          
            CustomerRegpersonalinfo model = new CustomerRegpersonalinfo();
            //model.Profiles = ds.PopulateProfiles();
            //model.phonenumber = Session["custphone"].ToString();
            //model.UserName = Session["username"].ToString();

            //model.Address = Session["custAddress"].ToString();
            model.data = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult Registrationpersonalinfo(CustomerRegpersonalinfo model)
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
            string apiresponse = "N/A";
            model.data = false;
            try
            {
                //if (ModelState.IsValid)
                //{
                    apiresponse = Connecttocore.NID_Info(model.NID, Session["accesstoken"].ToString());

                //var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(apiresponse);
                //ViewBag.ImageBase64 = imageResponse.ImageBase64;
                //ImageResponse imageResponsee = JsonConvert.DeserializeObject<ImageResponse>(apiresponse);
                //byte[] imageBytes = Convert.FromBase64String(imageResponse.ImageBase64);
                //var imageResponser = JsonConvert.DeserializeObject<ImageResponse>(apiresponse); // Pass the base64 string to the view ViewBag.ImageBase64 = imageResponse.ImageBase64;

                //////here updated //////////////
                ///


                JObject customerInfo = new JObject();
                customerInfo = JObject.Parse(apiresponse);

                //var imageResponse = JsonConvert.DeserializeObject<ImageResponse>(apiresponse);
                //ViewBag.ImageBase64 = imageResponse.ImageBase64;

                var imageResponse = customerInfo["PHOTOGRAPH"].Value<String>();
                byte[] imageBytes = Convert.FromBase64String(imageResponse);
                //var imageResponser = JsonConvert.DeserializeObject<ImageResponse>(apiresponse);

                ViewBag.ImageBase64 = imageResponse;

                model.First_Names = customerInfo["FIRST_NAMES"].Value<String>();
                model.Last_Name = customerInfo["LAST_NAME"].Value<String>();
                model.Identity_Number  = customerInfo["IDENTITY_NUMBER"].Value<String>();
                model.Father_Name = customerInfo["FATHER_NAME"].Value<String>();
                model.Grand_Father_Name = customerInfo["GRAND_FATHER_NAME"].Value<String>();
                model.Gre_Gra_Father_Name = customerInfo["GRE_GRA_FATHER_NAME"].Value<String>();
                model.Mot_Father_Name = customerInfo["MOT_FATHER_NAME"].Value<String>();
                model.Mot_Gra_Father_Name = customerInfo["MOT_GRA_FATHER_NAME"].Value<String>();
                model.Mot_Gre_Gra_Father_Name = customerInfo["MOT_GRE_GRA_FATHER_NAME"].Value<String>();
                model.Mother_Name = customerInfo["MOTHER_NAME"].Value<String>();
                model.Name = customerInfo["NAME"].Value<String>();
                model.Address = customerInfo["ADDRESS"].Value<String>();
                model.Gender = customerInfo["GENDER"].Value<String>();
                model.Birth_date = customerInfo["BIRTH_DATE"].Value<String>();
                model.data = true;
                
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), Session["branch_namee"].ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer information inquiry", model.First_Names + " - " + model.Identity_Number, DateTime.Now.ToString());

                //model.Birth_date = customerInfo["PHOTOGRAPH"].Value<String>();

                //var Last_Name = apiresponse;

                //int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                //if (responseCode == 0)
                //{
                //    model.Name = customerInfo.GetValue("customerName").ToString();

                //    if (!String.IsNullOrEmpty(customerInfo.GetValue("Customer_Name_AR").ToString()))
                //    {
                //        Session["custnamear"] = customerInfo.GetValue("Customer_Name_AR").ToString();
                //    }
                //}



                //}
                //ViewBag.ImageBase64 = apiresponse.imageResponse.ImageBase64;
                //

                //else
                //{
                //    message = "All Fields are required ";
                //    ModelState.AddModelError("", "Something is missing" + message);
                //    return View(model);
                //}
                return View(model);
            }
            catch (Exception e)
            {
                message = "Something is missing or Error  ";
                ModelState.AddModelError("", " please try again" + message);
                return View(model);
            }

            //if (ModelState.IsValid)
            //{
            //    String username = model.UserName;
            //    String email = model.Email;
            //    String address = model.Address;
            //    String account = Session["Account"].ToString();
            //    String customerprofile = "1"; //model.profileCode.ToString();
            //    String CustomerID = Session["custID"].ToString();
            //    String CustomerName = "N/A";
            //    if (Session["custnamearabic"] != null)
            //    {
            //        CustomerName = Session["custnamearabic"].ToString();
            //    }
            //    else if (Session["custname"] != null)
            //    {
            //        CustomerName = Session["custname"].ToString();
            //    }
            //    String CustomerPhone = model.phonenumber;//Session["custphone"].ToString();
            //    String customercatgory = Session["custcat"].ToString();
            //    String CUSTOMERSERVICE = Session["service"].ToString();
            //    string adminusername = Session["user_log"].ToString();
            //    string userfullaccount = Session["fullaccountnumber"].ToString();

            //    if (CustomerPhone == "0")
            //    {
            //        message = "Cannot register customer without phone number, Please update customer phone number in core bank first.";
            //        Session["message"] = message;
            //        ViewBag.SuccessMessage = message;
            //        return View(model);
            //    }

            //    int response = ds.custreg(CustomerID, CustomerName, account, userfullaccount, username, address, CustomerPhone, email, customerprofile, customercatgory, CUSTOMERSERVICE, adminusername);

            //    if (response.Equals(1))
            //    {
            //        string usershorthand = Session["custID"].ToString();
            //        string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            //        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer registration", usershorthand + " - " + CustomerName, DateTime.Now.ToString());
            //        message = "Customer Registration Completed Successfully";
            //        Session["message"] = message;
            //        ModelState.AddModelError("", message);
            //        return RedirectToAction("Registration");
            //    }
            //    else if (response.Equals(2))
            //    {
            //        message = "This customer is registered already.";
            //        Session["message"] = message;
            //        ViewBag.SuccessMessage = message;
            //        return View(model);
            //    }
            //    else
            //    {
            //        message = "This customer is registered already.";
            //        ModelState.AddModelError("", message);
            //        return View(model);
            //    }
            //}
            //else
            //{
            //    message = "All Fields are required ";
            //    ModelState.AddModelError("", "Something is missing" + message);
            //    return View(model);
            //}

        }
    }
}