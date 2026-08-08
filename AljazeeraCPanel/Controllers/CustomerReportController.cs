using AljazeeraCPanel.Context;
using AljazeeraCPanel.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml.html;
using Newtonsoft.Json.Linq;
using SIBCPanel.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using static iTextSharp.text.pdf.AcroFields;

namespace AljazeeraCPanel.Controllers
{

    //old
    public class CustomerReportController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /CustomerReport/
        public ActionResult CustomersReport()
        {
            
            


            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }


            Custreport model = new Custreport();
            String userbranch = Session["user_branch"].ToString();
            model.Branches = ds.PopulateBranchs();
            model.catgories = ds.GetGatgories();
            model.CustomerStatus = ds.PopulateCustStatus();


            Session["CustReport"] = model;

            return View(model);

        }

        [HttpPost]
        public ActionResult CustomersReport(Custreport model)
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
                string formatedFromDate = DateTime.Parse(model.fromdate).ToString();
                string formatedtodate = DateTime.Parse(model.todate).ToString();
                string[] words1 = formatedFromDate.Split(' ');
                formatedFromDate = words1[0];
                words1 = formatedtodate.Split(' ');
                formatedtodate = words1[0];
                model.Branches = ds.PopulateBranchs();
                model.catgories = ds.GetGatgories();
                model.CustomerStatus = ds.PopulateCustStatus();

                var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedCategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                var selectedStatus = model.CustomerStatus.Find(p => p.Value == model.StatusCode.ToString());

                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedCategory != null)
                {
                    selectedCategory.Selected = true;

                }
                if (selectedStatus != null)
                {
                    selectedStatus.Selected = true;

                }


                if (ModelState.IsValid)
                {
                    List<Custreport> accass = new List<Custreport>();

                    accass = ds.GetBranchUsersComplete(model.BranchCode, model.CategoryCode, model.StatusCode, formatedFromDate, formatedtodate);
                    if (accass.Count > 0)
                    {
                        if (model.BranchCode == "000")
                        {
                            Session["Branchname"] = "All Branches";
                        }
                        else
                        {
                            Session["Branchname"] = ds.getbranchnameenglish(model.BranchCode);
                        }

                        //if (model.BranchCode != "0")
                        //    //Session["Branchname"] = ds.getbranchnameenglish(model.BranchCode);
                        //else
                        //    Session["Branchname"] = "All Branches";
                        Session["BranchUsers"] = accass;
                        return RedirectToAction("ViewReport");
                    }


                    else
                    {
                        message = "No Customer Registered";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }
                }

                else
                {
                    message = "Please Contact us for Support";
                    ModelState.AddModelError("", "Something is missing" + message);
                    return View(model);
                }


            }
            catch (Exception e)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                return View(model);
            }
        }


        public ActionResult EsaliReport()
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["acresult"] != null)
            {
                ViewBag.SuccessMessage = Session["acresult"].ToString();
                Session["acresult"] = null;
            }
            if (Session["failresult"] != null)
            {
                ViewBag.ErrorMessage = Session["failresult"].ToString();
                Session["failresult"] = null;
            }
            CustomerRegBankinfo model = new CustomerRegBankinfo();
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();

            // model.catgories = ds.GetGatgories();

            //Session["regmodel"] = model;


            model.data = false;
            model.pay = false;

            if (Session["data"] != null)
            {
                model.data = Boolean.Parse(Session["data"].ToString());
                // Session["byaccount"] = null;
            }
            if (Session["pay"] != null)
            {
                model.pay = Boolean.Parse(Session["pay"].ToString());
                // Session["byaccount"] = null;
            }

            if (Session["byaccount"] != null)
            {
                model.byaccount  = Boolean.Parse( Session["byaccount"].ToString());
               // Session["byaccount"] = null;
            }
            //model.byaccount = false;


            if (Session["ModelInfo"] != null)
            {
                //CustomerRegBankinfo ob = new CustomerRegBankinfo();
                model = Session["ModelInfo"] as CustomerRegBankinfo;
            }
           // List<SelectListItem> catogery = new List<SelectListItem>();

            //catogery.Add(new SelectListItem { Text = "Cash", Value = "1" });
            //catogery.Add(new SelectListItem { Text = "Account", Value = "2" });
            if (Session["invoice"] != null)
            {
                model.invoice = Session["invoice"].ToString() as string;
            }
            if (Session["Branch"] != null)
            {
                model.Branch = Session["Branch"].ToString() as string;
            }
            if (Session["custnameen"] != null)
            {
                model.CustomerName = Session["custnameen"].ToString() as string;
            }
            if (Session["custAccount"] != null)
            {
                model.AvailableCustomerAccount = (List<SelectListItem>)Session["custAccount"];
            }
            if (Session["custphone"] != null)
            {
                model.AvailablePhoneNumbers = (List<SelectListItem>)Session["custphone"];
            }
            //if (Session["custid"] != null)
            //{
            //    model.Branch = Session["custid"].ToString() as string;
            //    model.CustomerName = Session["custname"].ToString() as string;
            //    model.AccountType = Session["AccountType"].ToString() as string;
            //    model.Currency = Session["Currency"].ToString() as string;
            //    model.BranchCode = Session["BranchCode"].ToString() as string;
            //    model.AccountNumber = Session["AccountNumber"].ToString() as string;
            //}
            //  model.catgories = catogery;
            //if (Session["catcode"]!= null)
            //{
            //    string cat = Session["catcode"].ToString();
            //    if (cat == "2")
            //    {

            // model.byaccount = true;
            // model.CategoryCode = "2";

            //    }
            //}
            //Session["cat"] = model.catgories;

            Session["regmodel"] = model;
            return View(model);

        }

        public ActionResult ResetAction(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["acresult"] != null)
            {
                ViewBag.SuccessMessage = Session["acresult"].ToString();
                Session["acresult"] = null;
            }
            string message;
            //Session.Clear();

            Session.Remove("custAccount");
            Session.Remove("custphone");
            Session.Remove("custnameen");

            string invoice = model.invoice;
            //model = ds.GetUserinfoDataLink(model.Branch);
            try
            {
                string apiresponse = Connecttocore.getCustomerInfoByRim(model.Branch, Session["accesstoken"].ToString());
                Session["invoice"] = model.invoice;
                Session["Branch"] = model.Branch;
                JObject customerInfo = new JObject();
                customerInfo = JObject.Parse(apiresponse);

                int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    model.byaccount = true;
                    Session["byaccount"] = model.byaccount;
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
                    List<SelectListItem> AvailableCustomerAccount = new List<SelectListItem>();
                    foreach (AccountDetails account in accountDetails)
                    {
                        AvailableCustomerAccount.Add(new SelectListItem
                        {
                            Text = account.Account_No + " - " + account.Currency_Code,
                            Value = account.Account_No
                        });
                    }
                    model.AvailableCustomerAccount = AvailableCustomerAccount;
                    //Session["accountDetails"] = accountDetails; //jarray
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


                    model.CustomerName = customerInfo.GetValue("Customer_Name_EN").ToString();
                    model.accountDetails = accountDetails;
                    //model.AvailableCustomerAccount = AvailableCustomerAccount;
                    model.AvailablePhoneNumbers = availablephonenumbers;

                    Session["custAccount"] = AvailableCustomerAccount; //selectlistitem
                    Session["custphone"] = availablephonenumbers; //selectlistitem

                    Session["custcat"] = model.CategoryCode;
                    Session["service"] = "3";
                    // logging activity
                    string usershorthand = model.AccountNumber;
                    //string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer inquery", usershorthand + " - " + custname, DateTime.Now.ToString());
                    return RedirectToAction("EsaliReport", model);
                }
                else
                {
                    Session["messagefalied"] = "Something is missing or error ";
                    TempData["fail"] = customerInfo.GetValue("Response_Message").ToString();
                    //return RedirectToAction("Registration");
                    message = customerInfo.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("Something is missing or error", message);
                    //return View(model);
                    Session["acresult"] = "Something is missing or error to load Customer Info";
                    return RedirectToAction("EsaliReport");
                }
            }

            //string response2 = Connecttocore.GetCustinfoCore(model.Branch, Session["accesstoken"].ToString());

            ////string response2 = "{\"Accounts_List\": [  {  \"IBAN\": \"SD2135010000053907\",          \"Account_Type_Code\": \"CUR\",          \"Account_No\": \"10000053907\",           \"Currency_Code\": \"SDG\",          \"Branch_Code\": \"017\"      }    ],   \"Response_Code\": 0,    \"Response_Message\": \"Successful\",   \"Email\": \" \",    \"Phones\": [        {            \"Phone_No\": \"249966303037\"       },       {           \"Phone_No\": \"249966303037\"       },       {          \"Phone_No\": \"249966303037\"       }  ],   \"Address\": \"حي الدوحة مربع 29\",    \"Customer_Name_EN\": \"Amel Ismail Osman Abdelwadoud\",   \"RIM\": \"79864\",   \"Customer_Name_AR\": \"امل اسماعيل عثمان عبد الودود\"}";


            //JObject jobj = new JObject();
            //jobj = JObject.Parse(response2);
            //dynamic result = jobj;

            //string responseCode = result.Response_Code;

            //if (responseCode == "0")
            //{
            //    string responseMessage = result.response_Message;
            //    string address = result.Address;
            //    Session["address"] = address;


            //    string email = result.Email;
            //    Session["email"] = email;
            //    string custnameen = result.Customer_Name_EN;
            //    Session["CustomerName"] = custnameen;
            //    string rim = result.RIM;
            //    Session["rim"] = rim;
            //    string custnamear = result.Customer_Name_AR;
            //    Session["customernameArabic"] = custnamear;
            //    string Address = result.Address;
            //    Session["Address"] = Address;
            //    //string bal = result.result;
            //    JToken resAccList = result.Accounts_List;
            //    //JObject jobj2 = new JObject();
            //    //jobj2 = JObject.FromObject(resAccList);
            //    JObject jobj2 = JObject.Parse(resAccList[0].ToString());
            //    //dynamic result2 = jobj2;


            //    string iban = jobj2.GetValue("IBAN").ToString();   //IBAN;
            //    Session["iban"] = iban;

            //    string acctype = jobj2.GetValue("Account_Type_Code").ToString();
            //    Session["AccountType"] = acctype;
            //    string accountno = jobj2.GetValue("Account_No").ToString();
            //    Session["AccountNumber"] = accountno;
            //    string curr = jobj2.GetValue("Currency_Code").ToString();
            //    Session["Currency"] = curr;
            //    string branch = jobj2.GetValue("Branch_Code").ToString();
            //    Session["Branch"] = branch;
            //    JToken phones = result.Phones;



            //    JObject jphone = JObject.Parse(phones[0].ToString());
            //    string phone = jphone.GetValue("Phone_No").ToString();
            //    Session["CustomerPhone"] = phone;


            //    String custID;
            //    String custphone;


            //    //if (responseCode == "0")
            //    //{
            //    custID = "N/A";
            //    //custname = acc[2].ToString();

            //    //custphone = acc[4].ToString();
            //    Session["custID"] = custID;
            //    //Session["custname"] = custname;
            //    //Session["custphone"] = custphone;
            //    Session["custcat"] = model.CategoryCode;
            //    //string usershorthand = "11" + model.BranchCode + model.AccountNumber;
            //    string usershorthand = accountno;
            //    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            //    //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Customer Inquery", usershorthand + " - " + custnameen, DateTime.Now.ToString());
            //    return RedirectToAction("Refreshuser");
            //}

            //else
            //{
            //    message = "Please check customer information something wrong ";
            //    ModelState.AddModelError("", message);
            //    return View(model);
            //}




            //String response;
            //String fullaccountnumber = "11" + model.BranchCode + model.AccountTypecode + model.AccountNumber + model.SUBNO + model.CurrencyCode + model.SUBGL;
            ////infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.CustomerID, model.CurrencyCode, model.CategoryCode, fullaccountnumber);
            ////infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullaccountnumber);

            ////response = infomodel.lblconfirm;

            //if (ModelState.IsValidField(model.AccountNumber))
            //{
            //    //String Accountnumber = "13" + model.BranchCode + model.AccountTypecode + model.CurrencyCode +
            //    //    model.AccountNumber;
            //    //String Accountnumber = "11" + model.BranchCode + model.AccountTypecode + model.AccountNumber + model.SUBNO + model.CurrencyCode + model.SUBGL;

            //    //String Accountnumber = restmodel.account;
            //    //List<resetpass> result = new List<resetpass>();
            //    //result = ds.resetpassword(Accountnumber, model.CustomerID);
            //    //foreach (var item in result)
            //    //{
            //    //    if (item.lblconfirm == "Successfully")
            //    //    {
            //    //        restmodel.name = item.name;
            //    //        restmodel.account = item.account;
            //    //        restmodel.branchname = item.branchname;
            //    //        restmodel.pass = item.pass;
            //    //        restmodel.user_log = item.user_log;
            //    //        restmodel.fullaccount = Accountnumber;
            //    //        Session["presetpassresult"] = restmodel;
            //    //        Session["CustLog"] = restmodel.user_log;
            //    //        return RedirectToAction("Print", "resetCustomer");
            //    //    }
            //    //    else
            //    //    {
            //    //        ModelState.AddModelError("", item.lblconfirm);
            //    //    }
            //    //model.byaccount = true;
            //    model.invoice = invoice;
            //    Session["invoice"] = model.invoice;
            //    Session["custname"] = model.CustomerName;
            //    Session["custid"] = model.CustomerID;
            //    Session["AccountType"] = model.AccountTypecode;
            //    Session["Currency"] = model.Currency;
            //    Session["BranchCode"] = model.BranchCode;
            //    Session["AccountNumber"] = model.AccountNumber;
            //    //}
            //    return RedirectToAction("EsaliReport", model);
            //}
            //else
            //{
            //    message = "This Customer Account Is Not Register";
            //    Session["reseterror"] = message;

            //    ModelState.AddModelError("", message);
            //    return RedirectToAction("EsaliReport", model);
            //}

            catch(Exception e)
            {
               
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing or error " + message);
                Session["failresult"] = "Something is missing or error " + message;
                 return RedirectToAction("EsaliReport", model);
            }
        }



        [HttpPost]
        public ActionResult EsaliReport(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["acresult"] != null)
            {
                ViewBag.SuccessMessage = Session["acresult"].ToString();
                Session["acresult"] = null;
            }
            string message = "";
            try
            {
                String userbranch = Session["user_branch"].ToString();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();

                if (Session["custAccount"] != null)
                {
                    model.AvailableCustomerAccount = (List<SelectListItem>)Session["custAccount"];
                }
                if (Session["custphone"] != null)
                {
                    model.AvailablePhoneNumbers = (List<SelectListItem>)Session["custphone"];
                }
                model.amount = Session["Amount"].ToString();

                    //model.catgories = ds.GetGatgories();

                    //List<SelectListItem> catogery = new List<SelectListItem>();

                    //catogery.Add(new SelectListItem { Text = "Cash", Value = "1" });
                    //catogery.Add(new SelectListItem { Text = "Account", Value = "2" });

                    //model.catgories = catogery; //(List<SelectListItem>)Session["cat"]; // catogery;
                    //                            //if (ModelState.IsValidField(model.invoice))
                    //Session["cat"] = model.catgories;

                    if (!String.IsNullOrEmpty(model.invoice))
                    Session["invoice"] = model.invoice;
                //if (model.CategoryCode.Equals("2"))
                //{

                   // Session["catcode"] = "2".ToString();
                    model.byaccount = true;
                Session["byaccount"] = model.byaccount;
                   // return RedirectToAction("EsaliReport", model);
                   // }
                   //{
                JObject obj = new JObject();

                obj.Add("Account_No", model.AccountNumber);
                obj.Add("Branch_Code", model.BranchCode);
                obj.Add("Currency_Code", model.Currency); //"SDG"
                obj.Add("Account_Type_Code", model.AccountType);

                //custinfo infomodel = new custinfo();
                
                //model.invoice, model.CategoryCode, Session["user_name"].ToString() , model.amount , obj   //model.invoice, model.CategoryCode, model.amount, model.Fees, Session["user_name"].ToString()
                string apiresponse = Connecttocore.PayInvoice(model.invoice, model.CategoryCode, Session["user_name"].ToString(), model.amount, obj, Session["accesstoken"].ToString());  //"{\"OtherReceiptNo\":\"12345\",\"amount\":\"150000\",\"SERVICESNAME\":\"Passport\",\"SERVICESID\":\"3\",\"UNITID\":\"1\",\"CENTERID\":1,\"InvoiceNo\":\"98765\",\"transactionID\":\"1234567\",\"responseCode\":\"0\",\"reference\":\"1234567\",\"UNITNAME\":\"Portsudan unit\",\"OPERATIONDATE\":\"12/25/20424\",\"receiptNo\":\"123456\",\"ServicesTotalAmount\":\"150000\",\"CENTERNAME\":\"Portsudan center\",\"InvoiceNumber\":\"12345690\",\"responseMessage\":\"Success\",\"CUSTOMERNAME\":\"lobna\"}";//{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
                //"{\"Bank_RRN\":\"00000250000006250419\",\"Tran_DateTime\":\"2026-04-12T16:27:25.7232459+02:00\",\"Additional_Reference\":\"\",\"Amount\":\"890760437.10\",\"Fees\":\"0\",\"Bill_Info\":[{\"Info_Value\":\"2603210107840425200036\",\"Info_Label\":\"Receipt Number\"},{\"Info_Value\":\"2603210107840425200036\",\"Info_Label\":\"Invoice Number\"},{\"Info_Label\":\"Biller Message\"}],\"Response_Code\":0,\"Response_Message\":\"Success\",\"Biller_ID\":\"2205\",\"Pay_Customer_Code\":\"2603210107840425200036\",\"Biller_Sub_ID\":\"\",\"App_RRN\":\"260412064651\",\"Currency_Code\":\"SDG\"}"
                //"{\"Response_Code\":-99,\"Response_Message\":\"System Error: null\",\"Tran_DateTime\":\"2026-03-26T14:42:45.4511832+02:00\",\"Biller_ID\":\"2205\",\"Pay_Customer_Code\":\"2026030107840425200044\",\"Additional_Reference\":\"\",\"Biller_Sub_ID\":\"\",\"Amount\":\"890760437.10\",\"App_RRN\":\"260326064624\",\"Fees\":\"0\",\"Currency_Code\":\"SDG\",\"Bill_Info\":[]}"

                //string apiresponse = "{\"Username\":\"National_Bank_Egypt\",\"InvoiceNumber\":\"2026030107840425200044\",\"InvoiceStatus\":\"Paid\",\"RRN\":\"00000250000006250419\"}";
                //"{\"Bank_RRN\":\"00000220000006136928\",\"Tran_DateTime\":\"2025-11-27T14:31:20.5389751+02:00\",\"Additional_Reference\":\"\",\"Amount\":\"81000.00\",\"Fees\":\"0\",\"Bill_Info\":[{\"Info_Value\":\"252100200071100015 | 252200200071100016\",\"Info_Label\":\"Receipt Number\"},{\"Info_Value\":\"202511000700200038\",\"Info_Label\":\"Invoice Number\"},{\"Info_Label\":\"Biller Message\"}],\"Response_Code\":0,\"Response_Message\":\"Success\",\"Biller_ID\":\"2205\",\"Pay_Customer_Code\":\"202511000700200038\",\"Biller_Sub_ID\":\"\",\"App_RRN\":\"251127064464\",\"Currency_Code\":\"SDG\"}";                                                                                                                                    // string apiresponse = "Sample Response:\r\n{\r\n    \"Response_Code\": 0,\r\n    \"Response_Message\": \"Success\",\r\n    \"Tran_DateTime\": \"281025021750\",\r\n    \"Biller_ID\": \"2205\",\r\n    \"Pay_Customer_Code\": \"202510000700200036\",\r\n    \"Additional_Reference\": \"000\",\r\n    \"Biller_Sub_ID\": \"000\",\r\n    \"Amount\": \"54000.00\",\r\n    \"App_RRN\": \"251028064348\",\r\n    \"Fees\": \"0\",\r\n    \"Currency_Code\": \"SDG\",\r\n    \"Bill_Info\": [\r\n        {\r\n            \"Info_Value\": \"فاتورة اختبارية البنك الاهلي المصري\",\r\n            \"Info_Label\": \"CustomerName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"unpaid\",\r\n            \"Info_Label\": \"InvoiceStatus\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"الإدارة العامة للسجل المدني\",\r\n            \"Info_Label\": \"MotherUnitName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني ولاية البحر الإحمر\",\r\n            \"Info_Label\": \"UNITNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني - بورتسودان\",\r\n            \"Info_Label\": \"CENTERNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"10/20/2025 8:39:15 AM\",\r\n            \"Info_Label\": \"OPERATIONDATE\"\r\n        }\r\n    ]\r\n}";                                                                                                                                         // Connecttocore.PayInvoice(model.invoice,model.CategoryCode ,model.amount,model.Fees, Session["user_name"].ToString());
                JObject customerInfo = new JObject();
                //{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
                customerInfo = JObject.Parse(apiresponse);
                //int responseCode = int.Parse(customerInfo.GetValue("responseCode").ToString());
                String response;
                //{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
                String fullnumber = model.AccountNumber;

                int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                // String response;
                //String fullnumber = model.AccountNumber;
                String CustomerName, InvoiceStatus, MotherUnitName, UNITNAME, CENTERNAME, OPERATIONDATE;

                if (responseCode == 0)
                {

                    string[] jsonStrings = new string[] { };
                    JArray array = new JArray();

                    //JObject objj = new JObject();
                    //JArray billInfoArray = (JArray)objj["Bill_Info"];

                    array = customerInfo.GetValue("Bill_Info") as JArray;
                    //foreach (JToken item in array)
                    //{
                    //    string Info_Label = item["Info_Label"]?.ToString();

                    //    //if (Info_Label.Equals("Invoice Number"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    invoke = item["Info_Value"]?.ToString();
                    //    //    model.CustomerName = CustomerName;


                    //    //}



                    //    //if (Info_Label.Equals("InvoiceStatus"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    InvoiceStatus = item["Info_Value"]?.ToString();
                    //    //    model.InvoiceStatus = InvoiceStatus;

                    //    //}

                    //    //if (Info_Label.Equals("MotherUnitName"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    MotherUnitName = item["Info_Value"]?.ToString();

                    //    //}

                    //    //if (Info_Label.Equals("UNITNAME"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    UNITNAME = item["Info_Value"]?.ToString();
                    //    //    model.UnitName = UNITNAME;
                    //    //}

                    //    //if (Info_Label.Equals("CENTERNAME"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    CENTERNAME = item["Info_Value"]?.ToString();
                    //    //    model.CenterName = CENTERNAME;
                    //    //}

                    //    //if (Info_Label.Equals("OPERATIONDATE"))
                    //    //{
                    //    //    //var token = JToken.Parse(item);
                    //    //    OPERATIONDATE = item["Info_Value"]?.ToString();
                    //    //    model.trandate = OPERATIONDATE;
                    //    //}


                    //    //string value = item["Info_Value"]?.ToString();
                    //    //values.Add(value);
                    //    //model.CenterName = item[0].In;
                    //    //model.CenterName = item.GetValue("CENTERNAME").ToString();

                    //    //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                    //    //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                    //    //model.InvoiceStatus = InvoiceStatus; //customerInfo.GetValue("InvoiceStatus").ToString();


                    //}


                    model.Fees = customerInfo.GetValue("Fees").ToString();
                    model.reference = customerInfo.GetValue("App_RRN").ToString();
                    model.respmsg = customerInfo.GetValue("Response_Message").ToString();
                    model.amount = customerInfo.GetValue("Amount").ToString();
                    model.invoice = customerInfo.GetValue("Pay_Customer_Code").ToString();
                    model.cb_rrn = customerInfo.GetValue("Bank_RRN").ToString();
                   // model.cb_rrn = customerInfo.GetValue("Bank_RRN").ToString();

                    //model.InvoiceStatus = InvoiceStatus;
                    //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                    //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                    /*  model.UnitName = customerInfo.GetValue("UNITNAME").ToString()*/
                    //;
                    //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                    //model.trandate = customerInfo.GetValue("Tran_DateTime").ToString();

                    //model.amount = customerInfo.GetValue("Amount").ToString();
                    //model.Fees = customerInfo.GetValue("Fees").ToString();
                    //model.respmsg = customerInfo.GetValue("Response_Message").ToString();


                    //JArray array= new JArray();
                    //array = customerInfo.GetValue("Bill_Info") as JArray;
                    //foreach(JObject item in array)
                    //{

                    //}


                    model.pay = true;
                    model.data = false;
                    Session["data"] = model.data;
                    Session["pay"] = model.pay;
                }
                else
                {
                    //model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
                    //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                    //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                    //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                    //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                    ////model.trandate = customerInfo.GetValue("CENTERNAME").ToString();
                    //model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
                    //model.Fees = customerInfo.GetValue("FeesAmount").ToString();



                    model.amount = customerInfo.GetValue("Amount").ToString();
                    model.Fees = customerInfo.GetValue("Fees").ToString();
                    model.invoice = customerInfo.GetValue("Pay_Customer_Code").ToString();
                    model.reference = customerInfo.GetValue("Reference_No").ToString();
                    model.cb_rrn = customerInfo.GetValue("CB_RRN").ToString();
                    //model.respmsg = customerInfo.GetValue("Response_Message").ToString();
                    model.respmsg = customerInfo.GetValue("Response_Message").ToString();//responseMessage
                    Session["acresult"] = model.respmsg;

                    //
                    //model.data = false;
                    //Session["data"] = model.data;
                   // model.respmsg = customerInfo.GetValue("Response_Message").ToString();
                    //Session["acresult"] = model.respmsg;

                }
                //model.pay = true;
               // ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), Session["branch_namee"].ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "customer Invoice Pay", model.CategoryCode + " - " + model.invoice, DateTime.Now.ToString());


                //if (responseCode == 0)
                //{


                //    model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
                //    model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                //    model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                //    model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                //    model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                //    model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                //    model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
                //    //model.Fees = customerInfo.GetValue("FeesAmount").ToString();
                //    model.tranid = customerInfo.GetValue("transactionID").ToString();
                //    model.reference = customerInfo.GetValue("reference").ToString();
                //    model.trandate = customerInfo.GetValue("OPERATIONDATE").ToString();
                //    model.respmsg = customerInfo.GetValue("responseMessage").ToString();

                //}
                //else
                //{

                //    //model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
                //    //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                //    //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                //    //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                //    //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                //    //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                //    //model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
                //    ////model.Fees = customerInfo.GetValue("FeesAmount").ToString();
                //    //model.tranid = customerInfo.GetValue("transactionID").ToString();
                //    //model.reference = customerInfo.GetValue("reference").ToString();
                //    //model.trandate = customerInfo.GetValue("OPERATIONDATE").ToString();
                //    model.respmsg = customerInfo.GetValue("responseMessage").ToString();
                //}
                //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullnumber);
                //response = infomodel.lblconfirm;
                //model.pay = true;

                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), Session["branch_namee"].ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "customer Invoice Pay", model.CategoryCode + " - " + model.invoice, DateTime.Now.ToString());

                //if (response.Equals("This Account is Already exist"))
                //{
                //    String act = model.AccountNumber;
                //    Session["Account"] = act;




                //}
                //else
                //{
                //    message = "Sorry this account Not Registered ";
                //    ModelState.AddModelError("", message);
                //    return View(model);
                //}
                //}

            }
            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                Session["acresult"] = "Something is missing Please Contact for Support";

            }
            return View(model);
            //return RedirectToAction("EsaliReport" , model);
        }

        //[HttpPost]
        //public ActionResult EsaliReport(CustomerRegBankinfo model)
        //{
        //    if (Session["user_name"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    if (Session["user_branch"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    string message = "";
        //    try
        //    {
        //        String userbranch = Session["user_branch"].ToString();
        //        model.Branches = ds.PopulateBranchs(userbranch);
        //        model.AccTypes = ds.PopulateAccountTypes();
        //        model.Currencies = ds.PopulateCurrencies();
        //        //model.catgories = ds.GetGatgories();

        //        List<SelectListItem> catogery = new List<SelectListItem>();

        //        catogery.Add(new SelectListItem { Text = "Cash", Value = "1" });
        //        catogery.Add(new SelectListItem { Text = "Account", Value = "2" });

        //        model.catgories = catogery; //(List<SelectListItem>)Session["cat"]; // catogery;
        //                                    //if (ModelState.IsValidField(model.invoice))
        //        Session["cat"] = model.catgories;
        //        if (!String.IsNullOrEmpty(model.invoice))
        //            Session["invoice"] = model.invoice;
        //        if (model.CategoryCode.Equals("2"))
        //        {

        //            Session["catcode"] = "2".ToString();
        //            model.byaccount = true;
        //            return RedirectToAction("EsaliReport", model);
        //        }
        //        //{


        //        //custinfo infomodel = new custinfo();

        //        string apiresponse = Connecttocore.PayInvoice(model.invoice, model.CategoryCode, model.amount, model.Fees, Session["user_name"].ToString());  //"{\"OtherReceiptNo\":\"12345\",\"amount\":\"150000\",\"SERVICESNAME\":\"Passport\",\"SERVICESID\":\"3\",\"UNITID\":\"1\",\"CENTERID\":1,\"InvoiceNo\":\"98765\",\"transactionID\":\"1234567\",\"responseCode\":\"0\",\"reference\":\"1234567\",\"UNITNAME\":\"Portsudan unit\",\"OPERATIONDATE\":\"12/25/20424\",\"receiptNo\":\"123456\",\"ServicesTotalAmount\":\"150000\",\"CENTERNAME\":\"Portsudan center\",\"InvoiceNumber\":\"12345690\",\"responseMessage\":\"Success\",\"CUSTOMERNAME\":\"lobna\"}";//{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
        //                                                                                                                                                      // string apiresponse = "Sample Response:\r\n{\r\n    \"Response_Code\": 0,\r\n    \"Response_Message\": \"Success\",\r\n    \"Tran_DateTime\": \"281025021750\",\r\n    \"Biller_ID\": \"2205\",\r\n    \"Pay_Customer_Code\": \"202510000700200036\",\r\n    \"Additional_Reference\": \"000\",\r\n    \"Biller_Sub_ID\": \"000\",\r\n    \"Amount\": \"54000.00\",\r\n    \"App_RRN\": \"251028064348\",\r\n    \"Fees\": \"0\",\r\n    \"Currency_Code\": \"SDG\",\r\n    \"Bill_Info\": [\r\n        {\r\n            \"Info_Value\": \"فاتورة اختبارية البنك الاهلي المصري\",\r\n            \"Info_Label\": \"CustomerName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"unpaid\",\r\n            \"Info_Label\": \"InvoiceStatus\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"الإدارة العامة للسجل المدني\",\r\n            \"Info_Label\": \"MotherUnitName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني ولاية البحر الإحمر\",\r\n            \"Info_Label\": \"UNITNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني - بورتسودان\",\r\n            \"Info_Label\": \"CENTERNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"10/20/2025 8:39:15 AM\",\r\n            \"Info_Label\": \"OPERATIONDATE\"\r\n        }\r\n    ]\r\n}";                                                                                                                                         // Connecttocore.PayInvoice(model.invoice,model.CategoryCode ,model.amount,model.Fees, Session["user_name"].ToString());
        //        JObject customerInfo = new JObject();
        //        //{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
        //        customerInfo = JObject.Parse(apiresponse);
        //        int responseCode = int.Parse(customerInfo.GetValue("responseCode").ToString());
        //        String response;
        //        //{"OtherReceiptNo":"12345","amount":"150000","SERVICESNAME":"Passport","SERVICESID":"3","UNITID":"1","CENTERID":1,"InvoiceNo":"98765","transactionID":"1234567","responseCode":"0","reference":"1234567","UNITNAME":"Portsudan unit","OPERATIONDATE":"12/25/20424","receiptNo":"123456","ServicesTotalAmount":"150000","CENTERNAME":"Portsudan center","InvoiceNumber":"12345690","responseMessage":"Success","CUSTOMERNAME":"lobna"}
        //        String fullnumber = model.AccountNumber;
        //        if (responseCode == 0)
        //        {


        //            model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
        //            model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
        //            model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
        //            model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
        //            model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
        //            model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
        //            model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
        //            //model.Fees = customerInfo.GetValue("FeesAmount").ToString();
        //            model.tranid = customerInfo.GetValue("transactionID").ToString();
        //            model.reference = customerInfo.GetValue("reference").ToString();
        //            model.trandate = customerInfo.GetValue("OPERATIONDATE").ToString();
        //            model.respmsg = customerInfo.GetValue("responseMessage").ToString();

        //        }
        //        else
        //        {

        //            //model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
        //            //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
        //            //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
        //            //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
        //            //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
        //            //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
        //            //model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
        //            ////model.Fees = customerInfo.GetValue("FeesAmount").ToString();
        //            //model.tranid = customerInfo.GetValue("transactionID").ToString();
        //            //model.reference = customerInfo.GetValue("reference").ToString();
        //            //model.trandate = customerInfo.GetValue("OPERATIONDATE").ToString();
        //            model.respmsg = customerInfo.GetValue("responseMessage").ToString();
        //        }
        //        //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullnumber);
        //        //response = infomodel.lblconfirm;
        //        model.pay = true;
        //        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), Session["branch_namee"].ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "customer Invoice Pay", model.CategoryCode + " - " + model.invoice, DateTime.Now.ToString());

        //        //if (response.Equals("This Account is Already exist"))
        //        //{
        //        //    String act = model.AccountNumber;
        //        //    Session["Account"] = act;




        //        //}
        //        //else
        //        //{
        //        //    message = "Sorry this account Not Registered ";
        //        //    ModelState.AddModelError("", message);
        //        //    return View(model);
        //        //}
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        message = "Please Contact for Support";
        //        ModelState.AddModelError("", "Something is missing" + message);

        //    }
        //    return View(model);
        //}

        [HttpPost]
        public ActionResult EsaliReportProcess(CustomerRegBankinfo model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["acresult"] != null)
            {
                ViewBag.SuccessMessage = Session["acresult"].ToString();
                Session["acresult"] = null;
            }
            string message = "";
            //if (model.CategoryCode.Equals("2"))
            //{
            // return RedirectToAction("EsaliReport", model);
            // }

            try
            {
                //if (Session["byaccount"] != null)
                //{
                    model.byaccount = true;
                     Session["byaccount"] = model.byaccount;
                //}
                String userbranch = Session["user_branch"].ToString();
                //model.Branches = ds.PopulateBranchs(userbranch);
                //model.AccTypes = ds.PopulateAccountTypes();
                //model.Currencies = ds.PopulateCurrencies();
                //model.catgories = ds.GetGatgories();

                //List<SelectListItem> catogery = new List<SelectListItem>();

                //catogery.Add(new SelectListItem { Text = "Passports", Value = "1" });
                //catogery.Add(new SelectListItem { Text = "Traffic", Value = "2" });

                //model.catgories = (List<SelectListItem>)Session["cat"]; // catogery;

                if (Session["accountDetails"] != null)
                {
                    //JArray Account_Info = new JArray();
                    //Account_Info = (JArray)Session["accountDetails"];
                    List<AccountDetails> accountDetails = new List<AccountDetails>();
                    accountDetails = (List<AccountDetails>)Session["accountDetails"];
                    foreach (var account in accountDetails)
                    {
                        if (model.selectedaccount.Equals(account.Account_No))

                        {
                            model.AccountNumber = account.Account_No;
                            model.AccountType = account.Account_Type_Code;
                            model.BranchCode = account.Branch_Code;
                            model.Currency = account.Currency_Code;








                        }

                    }
                }

                if (ModelState.IsValidField(model.invoice))
                {
                    // if(model.CategoryCode.Equals("2"))
                    JObject obj = new JObject();

                    obj.Add("Account_No", model.AccountNumber);
                    obj.Add("Branch_Code", model.BranchCode);
                    obj.Add("Currency_Code", model.Currency); //"SDG"
                    obj.Add("Account_Type_Code", model.AccountType);

                    //               "Account_No": "01222000242",             
                    //	"Branch_Code": "1",             
                    //	"Currency_Code": "SDG",           
                    //	"Account_Type_Code": "222"

                    //custinfo infomodel = new custinfo();

                    string apiresponse = Connecttocore.getInvoiceInfo(model.invoice, model.CategoryCode, Session["user_name"].ToString(), obj, Session["accesstoken"].ToString());   //"{\"InvoicePaymenTMethodType\":\"\",\"SERVICESNAME\":\"\",\"SERVICESID\":\"\",\"UNITID\":\"\",\"CENTERID\":\"\",\"InvoiceNo\":\"\",\"responseCode\":0,\"InvoiceSTATUS\":\"\",\"UNITNAME\":\"\",\"FeesAmount\":\"1500\",\"OPERATIONDATE\":\"\",\"ServicesTotalAmount\":\"\",\"CENTERNAME\":\"\",\"responseMessage\":\"\",\"CUSTOMERNAME\":\"\"}"; //Connecttocore.getInvoiceInfo(model.invoice, model.CategoryCode, Session["user_name"].ToString());
                    //string apiresponse = "{\r\n    \"Response_Code\": 0,\r\n    \"Response_Message\": \"Success\",\r\n    \"Tran_DateTime\": \"281025021750\",\r\n    \"Biller_ID\": \"2205\",\r\n    \"Pay_Customer_Code\": \"202510000700200036\",\r\n    \"Additional_Reference\": \"000\",\r\n    \"Biller_Sub_ID\": \"000\",\r\n    \"Amount\": \"54000.00\",\r\n    \"App_RRN\": \"251028064348\",\r\n    \"Fees\": \"0\",\r\n    \"Currency_Code\": \"SDG\",\r\n    \"Bill_Info\": [\r\n        {\r\n            \"Info_Value\": \"فاتورة اختبارية البنك الاهلي المصري\",\r\n            \"Info_Label\": \"CustomerName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"unpaid\",\r\n            \"Info_Label\": \"InvoiceStatus\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"الإدارة العامة للسجل المدني\",\r\n            \"Info_Label\": \"MotherUnitName\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني ولاية البحر الإحمر\",\r\n            \"Info_Label\": \"UNITNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"إدارة السجل المدني - بورتسودان\",\r\n            \"Info_Label\": \"CENTERNAME\"\r\n        },\r\n        {\r\n            \"Info_Value\": \"10/20/2025 8:39:15 AM\",\r\n            \"Info_Label\": \"OPERATIONDATE\"\r\n        }\r\n    ]\r\n}";                                                                                                                                         // Connecttocore.PayInvoice(model.invoice,model.CategoryCode ,model.amount,model.Fees, Session["user_name"].ToString());
                    //"{\"Response_Code\":0,\"Response_Message\":\"Success\",\"Tran_DateTime\":\"2026-03-26T14:22:11.463659+02:00\",\"Biller_ID\":\"2205\",\"Pay_Customer_Code\":\"2026030107840425200044\",\"Additional_Reference\":\"\",\"Biller_Sub_ID\":\"\",\"Amount\":\"890760437.10\",\"App_RRN\":\"260326064621\",\"Fees\":\"0\",\"Currency_Code\":\"SDG\",\"Bill_Info\":[{\"Info_Value\":\"البنك الاهلى المصرى\",\"Info_Label\":\"CustomerName\"},{\"Info_Value\":\"unpaid\",\"Info_Label\":\"InvoiceStatus\"},{\"Info_Value\":\"ديوان الضرائب\",\"Info_Label\":\"MotherUnitName\"},{\"Info_Value\":\"المراكز الضريبيه الموحده\",\"Info_Label\":\"UNITNAME\"},{\"Info_Value\":\"المركز الضريبى الموحد - الشركات الكبرى\",\"Info_Label\":\"CENTERNAME\"},{\"Info_Value\":\"3/16/2026 1:40:48 PM\",\"Info_Label\":\"OPERATIONDATE\"}]}"
                    JObject customerInfo = new JObject();
                    customerInfo = JObject.Parse(apiresponse);
                    // {"InvoicePaymenTMethodType":"","SERVICESNAME":"","SERVICESID":"","UNITID":"","CENTERID":"","InvoiceNo":"","responseCode":0,"InvoiceSTATUS":"","UNITNAME":"","FeesAmount":"1500","OPERATIONDATE":"","ServicesTotalAmount":"","CENTERNAME":"","responseMessage":"","CUSTOMERNAME":""}
                    int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
                    String response;
                    String fullnumber = model.AccountNumber;
                    String CustomerName, InvoiceStatus, MotherUnitName, UNITNAME, CENTERNAME, OPERATIONDATE;

                    if (responseCode == 0)
                    {
                        string[] jsonStrings = new string[] { };
                        JArray array = new JArray();
                        //JObject objj = new JObject();
                        //JArray billInfoArray = (JArray)objj["Bill_Info"];
                        array = customerInfo.GetValue("Bill_Info") as JArray;
                        foreach (JToken item in array)
                        {
                            string Info_Label = item["Info_Label"]?.ToString();
                            if (Info_Label.Equals("CustomerName"))
                            {
                                //var token = JToken.Parse(item);
                                CustomerName = item["Info_Value"]?.ToString();
                                model.CustomerName = CustomerName;


                            }

                            if (Info_Label.Equals("InvoiceStatus"))
                            {
                                //var token = JToken.Parse(item);
                                InvoiceStatus = item["Info_Value"]?.ToString();
                                model.InvoiceStatus = InvoiceStatus;

                            }

                            if (Info_Label.Equals("MotherUnitName"))
                            {
                                //var token = JToken.Parse(item);
                                MotherUnitName = item["Info_Value"]?.ToString();

                            }

                            if (Info_Label.Equals("UNITNAME"))
                            {
                                //var token = JToken.Parse(item);
                                UNITNAME = item["Info_Value"]?.ToString();
                                model.UnitName = UNITNAME;
                            }

                            if (Info_Label.Equals("CENTERNAME"))
                            {
                                //var token = JToken.Parse(item);
                                CENTERNAME = item["Info_Value"]?.ToString();
                                model.CenterName = CENTERNAME;
                            }

                            if (Info_Label.Equals("OPERATIONDATE"))
                            {
                                //var token = JToken.Parse(item);
                                OPERATIONDATE = item["Info_Value"]?.ToString();
                                model.trandate = OPERATIONDATE;
                            }


                            //string value = item["Info_Value"]?.ToString();
                            //values.Add(value);
                            //model.CenterName = item[0].In;
                            //model.CenterName = item.GetValue("CENTERNAME").ToString();

                            //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                            //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                            //model.InvoiceStatus = InvoiceStatus; //customerInfo.GetValue("InvoiceStatus").ToString();


                        }
                        model.amount = customerInfo.GetValue("Amount").ToString();
                        model.invoice = customerInfo.GetValue("Pay_Customer_Code").ToString();

                        Session["Amount"] = model.amount;
                        //model.InvoiceStatus = InvoiceStatus;
                        //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                        //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                        /*  model.UnitName = customerInfo.GetValue("UNITNAME").ToString()*/
                        ;
                        //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                        //model.trandate = customerInfo.GetValue("Tran_DateTime").ToString();

                        //model.amount = customerInfo.GetValue("Amount").ToString();
                        //model.Fees = customerInfo.GetValue("Fees").ToString();
                        //model.respmsg = customerInfo.GetValue("Response_Message").ToString();


                        //JArray array= new JArray();
                        //array = customerInfo.GetValue("Bill_Info") as JArray;
                        //foreach(JObject item in array)
                        //{

                        //}

                        model.data = true;

                    }
                    else
                    {

                        //"{\"Response_Code\":-200,\"Response_Message\":\"Unable to process the billing sevice\",\"Tran_DateTime\":\"2025-11-23T18:44:42.8366712+02:00\",\"Biller_ID\":\"2205\",\"Pay_Customer_Code\":\"202510000700200037\",\"Additional_Reference\":\"\",\"Biller_Sub_ID\":\"\",\"App_RRN\":\"251123064428\",\"Fees\":\"0\",\"Currency_Code\":\"SDG\",\"Bill_Info\":[]}"
                        //model.invoice = customerInfo.GetValue("InvoiceNo").ToString();
                        //model.CustomerName = customerInfo.GetValue("CUSTOMERNAME").ToString();
                        //model.ServiceName = customerInfo.GetValue("SERVICESNAME").ToString();
                        //model.UnitName = customerInfo.GetValue("UNITNAME").ToString();
                        //model.CenterName = customerInfo.GetValue("CENTERNAME").ToString();
                        ////model.trandate = customerInfo.GetValue("CENTERNAME").ToString();
                        //model.amount = customerInfo.GetValue("ServicesTotalAmount").ToString();
                        //model.Fees = customerInfo.GetValue("FeesAmount").ToString();
                        model.respmsg = customerInfo.GetValue("Response_Message").ToString();
                        Session["acresult"] = model.respmsg;

                    }
                    //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullnumber);
                    //response = infomodel.lblconfirm;
                    //model.data = true;
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), Session["branch_namee"].ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "customer essaliy Invoice inquiry", model.CategoryCode + " - " + model.invoice, DateTime.Now.ToString());

                    //if (response.Equals("This Account is Already exist"))
                    //{
                    //    String act = model.AccountNumber;
                    //    Session["Account"] = act;




                    //}
                    //else
                    //{
                    //    message = "Sorry this account Not Registered ";
                    //    ModelState.AddModelError("", message);
                    //    return View(model);
                    //}

                }
                else
                {
                    message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);

                }
            }//

            catch (Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                Session["acresult"] = "Something is missing Please Contact for Support";

            }
            Session["ModelInfo"] = model;
            //return View(model);
            return RedirectToAction("EsaliReport");
        }

        [HttpGet]
        public ActionResult getCustomerInformation()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();

            return View(model);
        }

        [HttpPost]
        public ActionResult getCustomerInformation(Custreport model)
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
            try { 
           
            //string apiresponse = "{\"Accounts_List\":[{\"IBAN\":\"SD4935010000000538\",\"Account_Type_Code\":\"CUR\",\"Account_No\":\"10000000538\",\"Currency_Code\":\"SDG\",\"Branch_Code\":\"017\"},{\"IBAN\":\"SD2735020000035708\",\"Account_Type_Code\":\"SAV\",\"Account_No\":\"20000035708\",\"Currency_Code\":\"USD\",\"Branch_Code\":\"004\"}],\"Response_Code\":0,\"Response_Message\":\"Successful\",\"Email\":\" \",\"Phones\":[{\"Phone_No\":\"249912328258\"},{\"Phone_No\":\"249912328258\"},{\"Phone_No\":\"249912328258\"}],\"Address\":\"الثوره الحاره 9 مربع525\",\"Customer_Name_EN\":\"Tarig Ahmed Abdelgalil Mohamed\",\"RIM\":\"39\",\"Customer_Name_AR\":\"طارق احمد عبدالجليل  محمد\"}";
            string apiresponse = Connecttocore.getcustomerinfousingphonenumber(model.phonenumber,model.rim,model.AccountNumber,model.username, Session["accesstoken"].ToString());
            JObject response = new JObject();
            response = JObject.Parse(apiresponse);

            int responseCode = int.Parse(response.GetValue("Response_Code").ToString());

            ////
            //JArray Account_Info = new JArray();
            //Account_Info = (JArray)response.GetValue("Accounts_List");
            //List<Custreport> accountDetails = new List<Custreport>();
            //foreach (JObject account in Account_Info)
            //{
            //    if (account.GetValue("IBAN") != null)
            //    {
            //        accountDetails.Add(new Custreport
            //        {
            //            Account_No = account.GetValue("Account_No").ToString(),
            //            Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
            //            Branch_Code = account.GetValue("Branch_Code").ToString(),
            //            Currency_Code = account.GetValue("Currency_Code").ToString(),
            //            IBAN = account.GetValue("IBAN").ToString()
            //        });
            //    }
            //    else
            //    {
            //        accountDetails.Add(new Custreport
            //        {
            //            Account_No = account.GetValue("Account_No").ToString(),
            //            Account_Type_Code = account.GetValue("Account_Type_Code").ToString(),
            //            Branch_Code = account.GetValue("Branch_Code").ToString(),
            //            Currency_Code = account.GetValue("Currency_Code").ToString()
            //        });
            //    }
            //}
            //model.info = accountDetails;
            //Session["accountDetails"] = accountDetails;

            //JArray phoneNumbers = (JArray)response.GetValue("Phones");
            //List<SelectListItem> availablephonenumbers = new List<SelectListItem>();
            //foreach (JObject phonenumber in phoneNumbers)
            //{
            //    availablephonenumbers.Add(new SelectListItem
            //    {
            //        Text = phonenumber.GetValue("Phone_No").ToString(),
            //        Value = phonenumber.GetValue("Phone_No").ToString()
            //    });
            //}

            //accountDetails.Add(new Custreport
            //{
            //    CustomerName = response.GetValue("Customer_Name_AR").ToString(),
                
            //    rim = response.GetValue("RIM").ToString(),
            //    address = response.GetValue("Address").ToString(),
            //    phonenumber = phoneNumbers[0].ToString()
            //});
            ////
            if (responseCode == 0)
            {
                JArray customersinresponse = (JArray)response.GetValue("Customers_List");
                List<custinfo> customers = new List<custinfo>();
                foreach(JObject customer in customersinresponse)
                {
                    customers.Add(new custinfo
                    {
                        type = customer.GetValue("Cust_Type").ToString(), //"Customer_Name_AR
                        user_name = customer.GetValue("Customer_Name_EN").ToString(),
                        name = customer.GetValue("Customer_Name_AR").ToString(),
                        creation_date = customer.GetValue("Creation_Date").ToString(),
                        status = customer.GetValue("Cust_Status").ToString(),
                        created_by = customer.GetValue("Created_By").ToString(),
                        user_id = customer.GetValue("User_ID").ToString(),
                        user_mobile = customer.GetValue("Phone_No").ToString(),
                        rim = customer.GetValue("RIM").ToString(),
                    });
                }

                Session["customerdetails"] = customers;
                return RedirectToAction("displaycustomerinfo");
            }
            else
            {
                message = response.GetValue("Response_Message").ToString();
                ModelState.AddModelError("", message);
            }
            }
            catch (Exception e)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                //return View(model);

            }
            return View(model);
        }

        public ActionResult displaycustomerinfo() {

            List<custinfo> customerinfo = (List<custinfo>)Session["customerdetails"];
            return View(customerinfo);
        }

        public ActionResult CustomersCountReport()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchs();
            model.catgories = ds.GetGatgories();
            model.CustomerStatus = ds.PopulateCustStatus();

            Session["CustReport"] = model;

            return View(model);
        }







        [HttpPost]
        public ActionResult CustomersCountReport(Custreport model)
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
                string formatedFromDate = "";
                string formatedtodate = "";
                String userbranch = Session["user_branch"].ToString();
                if (!string.IsNullOrEmpty(model.fromdate) || !string.IsNullOrEmpty(model.todate))
                {

                    formatedFromDate = DateTime.Parse(model.fromdate).ToString();
                    formatedtodate = DateTime.Parse(model.todate).ToString();
                    string[] words1 = formatedFromDate.Split(' ');
                    formatedFromDate = words1[0];
                    words1 = formatedtodate.Split(' ');
                    formatedtodate = words1[0];
                }
               
                model.Branches = ds.PopulateBranchs();
                model.catgories = ds.GetGatgories();
                model.CustomerStatus = ds.PopulateCustStatus();

               var selectedBranch = model.Branches.Find(p => p.Value == model.BranchCode.ToString());
                var selectedCategory = model.catgories.Find(p => p.Value == model.CategoryCode.ToString());
                var selectedStatus = model.CustomerStatus.Find(p => p.Value == model.StatusCode.ToString());

                if (selectedBranch != null)
                {
                    selectedBranch.Selected = true;

                }
                if (selectedCategory != null)
                {
                    selectedCategory.Selected = true;

                }
                if (selectedStatus != null)
                {
                    selectedStatus.Selected = true;

                }


                if (ModelState.IsValid)
                {
                    List<Custreport> accass = new List<Custreport>();

                    accass = ds.GetBranchUsers(model.BranchCode, model.CategoryCode, model.StatusCode, formatedFromDate, formatedtodate);
                    if (accass.Count > 0)
                    {
                        if (model.BranchCode == "000")
                        {
                            Session["Branchname"] = "All Branches";
                        }
                        else
                        {
                            Session["Branchname"] = ds.getbranchnameenglish(model.BranchCode);
                        }

                        //if (model.BranchCode != "0")
                        //    //Session["Branchname"] = ds.getbranchnameenglish(model.BranchCode);
                        //else
                        //    Session["Branchname"] = "All Branches";
                        Session["BranchUsersCount"] = accass;
                        return RedirectToAction("ViewCountReport");
                    }


                    else
                    {
                        message = "No Customer Registered";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }
                }
                else
                {
                    message = "Please Contact us for Support";
                    ModelState.AddModelError("", "Something is missing" + message);
                    return View(model);
                }

            }
            catch (Exception e)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult IssueDispute(CustomerTransferReportViewModel model)
        {
            CustomerTransferReportViewModel detailedtransaction = (CustomerTransferReportViewModel)Session["detailedtransaction"];
            detailedtransaction.TranID = Session["transactionid"].ToString();
            detailedtransaction.Comment = model.Comment;
            detailedtransaction.selected_dispute = model.selected_dispute;
            int disputeresult = ds.InsertDispute(detailedtransaction, Session["user_name"].ToString());
            if (disputeresult != -1)
            {
                TempData["disputecreated"] = "dispute created";
                detailedtransaction.dispute_id = disputeresult.ToString();
                int commentresult = ds.InsertComment(detailedtransaction, Session["user_name"].ToString());
            }

            return RedirectToAction("Disputes", "CustomerReport");
        }


        [HttpPost]
        public ActionResult UpdateDispute(CustomerTransferReportViewModel model)
        {
            string dispute_id = Session["disputetoedit"].ToString();
            model.dispute_id = dispute_id;
            model.Comment = model.Comment;
            model.selected_action = model.selected_action;
            model.TranStatus = ds.GetActionStatus(model.selected_action);
            int commentinsertresult = ds.InsertActionComment(model, Session["user_name"].ToString());
            if (commentinsertresult != -1)
            {
                ds.UpdateDispute(dispute_id, Session["user_name"].ToString());
                TempData["disputecreated"] = "Action logged.";
            }

            return RedirectToAction("Disputes", "CustomerReport");
        }


        [HttpGet]
        public ActionResult Disputes() //CustomerReportModel custInfo
        {


            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Dispute model = new Dispute();
            // return View(model);
            //List<Dispute> br = new List<Dispute>();



            List<SelectListItem> Branchlist = new List<SelectListItem>();
            List<SelectListItem> AccountTypelist = new List<SelectListItem>();


            string apirespone = Connecttocore.getBranchs(Session["accesstoken"].ToString());
            JObject response = new JObject();
            response = JObject.Parse(apirespone);
            int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
            if (responseCode == 0)
            {
                foreach (JObject br in response.GetValue("Branches_List")) if (br.HasValues)
                    {
                        // br.Add(new Dispute { BranchCode = role.GetValue("Branch_Code").ToString(), BranchName = role.GetValue("Branch_Name_EN").ToString() });
                        Branchlist.Add(new SelectListItem
                        {
                            Text = br.GetValue("Branch_Name_EN").ToString(),
                            Value = br.GetValue("Branch_Code").ToString()
                        });
                    }
            }



            string apiresponeAct = Connecttocore.getAccType(Session["accesstoken"].ToString());
            JObject responseAct = new JObject();
            responseAct = JObject.Parse(apiresponeAct);
            int responseCodeAct = int.Parse(responseAct.GetValue("Response_Code").ToString());
            if (responseCodeAct == 0)
            {
                foreach (JObject Act in responseAct.GetValue("Account_Types")) if (Act.HasValues)
                    {
                        // br.Add(new Dispute { BranchCode = role.GetValue("Branch_Code").ToString(), BranchName = role.GetValue("Branch_Name_EN").ToString() });
                        AccountTypelist.Add(new SelectListItem
                        {
                            Text = Act.GetValue("Account_Type_EN").ToString(),
                            Value = Act.GetValue("Account_Type_Code").ToString()
                        });
                    }
            }



            List<SelectListItem> Servicelist = new List<SelectListItem>();
            Servicelist.Add(new SelectListItem { Text = "Transfer To Card", Value = "2003" });
            Servicelist.Add(new SelectListItem { Text = "Zain Topup", Value = "2101" });
            Servicelist.Add(new SelectListItem { Text = "MTN Topup", Value = "2102" });
            Servicelist.Add(new SelectListItem { Text = "Sudani Topup", Value = "2103" });
            Servicelist.Add(new SelectListItem { Text = "NEC Topup", Value = "2104" });
            Servicelist.Add(new SelectListItem { Text = "MOHE SD", Value = "2105" });
            Servicelist.Add(new SelectListItem { Text = "MOHE ARAB", Value = "2106" });
            Servicelist.Add(new SelectListItem { Text = "Zain Bill Payment", Value = "2301" });
            Servicelist.Add(new SelectListItem { Text = "MTN Bill Payment", Value = "2302" });
            Servicelist.Add(new SelectListItem { Text = "Sudani Bill Payment", Value = "2303" });
            Servicelist.Add(new SelectListItem { Text = "E15 Bill Payment", Value = "2304" });
            Servicelist.Add(new SelectListItem { Text = "Customs Bill Payment", Value = "2305" });

            model.Branches = Branchlist;//ds.PopulateBranchs();
            model.service_names = Servicelist; //ds.PopulateServicess();
            model.account_type = AccountTypelist;//ds.PopulateAccountTypes();
        //    //List<SelectListItem> list = new List<SelectListItem>();
        //    //list.Add(new SelectListItem { Text = "All", Value = "All" });
        //    //list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
        //    //list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });

            //    //model.transactions_names = list;


            //    //string apirespone = Connecttocore.getDispute(Session["accesstoken"].ToString());

            //    String apirespone = "{\r\n    \"Response_Code\": 0,\r\n    \"Response_Message\": \"SUCCESSFUL\",\r\n    \"Trans_List\": [\r\n        {\r\n            \"Tran_ID\": \"854337\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD6235010000039589\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000039589\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:11:18.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"926519501\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001344845\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854488\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD6835010000040160\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000040160\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:30:17.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"998827294\",\r\n            \"Amount\": \"18250.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004349593\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854632\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD1935010000032797\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000032797\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:54:58.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"999207061\",\r\n            \"Amount\": \"13500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004543332\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854444\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD2135020000031451\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000031451\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:25:38.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"920818189\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000251467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"855237\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD6635010000051060\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000051060\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 11:42:24.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912365455\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004332584\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"855616\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-unable to connect to Raseedo\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3735020000033297\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033297\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 06:29:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123728596\",\r\n            \"Amount\": \"200.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000442841\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"856465\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD1835020000034512\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034512\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 10:25:03.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"962021425\",\r\n            \"Amount\": \"50.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006111742\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"857000\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD0935010000005429\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000005429\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 12:03:44.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912310625\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001123236\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852457\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 06:02:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854902\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6735010000010302\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000010302\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 10:35:30.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111151357\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000262389\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"857696\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6935010000034031\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000034031\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 02:27:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"120042662\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000256544\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854470\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD6835010000040160\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000040160\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:28:03.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"998827294\",\r\n            \"Amount\": \"18250.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004349593\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"859076\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6135010000046494\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000046494\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 06:06:35.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"918109104\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002438366\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860291\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alamarat Branch\",\r\n                \"IBAN\": \"SD8535020000032045\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032045\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:22:09.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122538414\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003430686\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860501\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6035010000004802\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004802\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:50:47.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"922530083\",\r\n            \"Amount\": \"7000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000453773\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860546\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD8135020000005151\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000005151\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:52:54.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"112152200\",\r\n            \"Amount\": \"15000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000579824\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"850111\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD0835020000006271\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000006271\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 02:35:54.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125996515\",\r\n            \"Amount\": \"6446.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000329373\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"851982\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD1935010000060539\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060539\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:17:57.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"119029427\",\r\n            \"Amount\": \"4480.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008279587\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852365\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:54:59.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852368\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:55:24.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852383\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:56:11.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852397\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:57:10.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852406\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:57:57.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852411\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:58:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852919\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"AL-Qadaref Branch\",\r\n                \"IBAN\": \"SD6135010000013514\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000013514\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 06:52:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125222080\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006588382\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"853235\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD4135010000046563\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000046563\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 07:34:01.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"923115881\",\r\n            \"Amount\": \"10100.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000201438\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"853344\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD2435010000032513\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000032513\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 07:41:36.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"916174198\",\r\n            \"Amount\": \"2500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003579491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860922\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD6235020000033817\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033817\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 10:55:07.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"996725897\",\r\n            \"Amount\": \"6000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003898907\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861798\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5835010000004935\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004935\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:04:07.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912336320\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005861491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861801\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5835010000004935\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004935\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:04:41.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912336320\",\r\n            \"Amount\": \"9000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005861491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861817\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935010000061122\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000061122\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:06:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123087444\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005860848\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"862071\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD7435010000034814\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000034814\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:42:13.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"115044367\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000210605\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052429937\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9135010000060601\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060601\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:07:46.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399615930\",\r\n            \"Tran_ID\": \"868457\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Pay_Customer_Code\": \"999011653\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003740\",\r\n            \"User_ID\": \"008039164\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052429942\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9135010000060601\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060601\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:10:09.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399615998\",\r\n            \"Tran_ID\": \"868473\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"999011653\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003742\",\r\n            \"User_ID\": \"008039164\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052430090\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:55:45.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399620563\",\r\n            \"Tran_ID\": \"868656\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123137954\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003747\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052430468\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD9035020000034186\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034186\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:25:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399624929\",\r\n            \"Tran_ID\": \"868945\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"113120694\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003758\",\r\n            \"User_ID\": \"005858746\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052439069\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8435010000011354\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000011354\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 11:48:11.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399664071\",\r\n            \"Tran_ID\": \"873119\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"111966698\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003839\",\r\n            \"User_ID\": \"002304286\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440069\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD3435010000027968\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000027968\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 12:10:52.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399668201\",\r\n            \"Tran_ID\": \"873584\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123990939\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003857\",\r\n            \"User_ID\": \"000831119\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440094\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alamarat Branch\",\r\n                \"IBAN\": \"SD0735010000044362\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000044362\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 12:11:18.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399668246\",\r\n            \"Tran_ID\": \"873588\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123675016\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003858\",\r\n            \"User_ID\": \"003146866\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440515\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5135010000052494\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000052494\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:20:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399669760\",\r\n            \"Tran_ID\": \"873783\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"126397399\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003864\",\r\n            \"User_ID\": \"006877821\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447148\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Soug Laybya\",\r\n                \"IBAN\": \"SD4535010000042611\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042611\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:52:39.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399701820\",\r\n            \"Tran_ID\": \"878172\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"121211514\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003994\",\r\n            \"User_ID\": \"001431545\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447157\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Soug Laybya\",\r\n                \"IBAN\": \"SD4535010000042611\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042611\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:53:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399701957\",\r\n            \"Tran_ID\": \"878195\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"121211514\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003995\",\r\n            \"User_ID\": \"001431545\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447680\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD7135020000000190\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000000190\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:57:04.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399712397\",\r\n            \"Tran_ID\": \"879105\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"122851992\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004017\",\r\n            \"User_ID\": \"007070360\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447686\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD7135020000000190\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000000190\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:57:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"4900.0\",\r\n            \"NS_RRN\": \"399712445\",\r\n            \"Tran_ID\": \"879111\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"122851992\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004018\",\r\n            \"User_ID\": \"007070360\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"863141\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD7735020000020937\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000020937\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:14:43.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"126346508\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003685860\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"865486\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 05:21:28.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111473288\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"865507\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 05:23:45.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111473288\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866213\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5335020000034270\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034270\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 07:18:08.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"911820754\",\r\n            \"Amount\": \"300.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004697881\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866392\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD6735020000032078\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032078\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 07:38:01.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"110235691\",\r\n            \"Amount\": \"8000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002650912\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866676\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD0535010000057299\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000057299\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:05:53.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"925029343\",\r\n            \"Amount\": \"180.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006521652\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867009\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"AlMawrada Branch\",\r\n                \"IBAN\": \"SD3735010000061282\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000061282\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:41:58.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"127970524\",\r\n            \"Amount\": \"200.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008274385\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867060\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD1035020000034753\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034753\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:50:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125586623\",\r\n            \"Amount\": \"170.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007323467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867071\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:51:20.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867076\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD1035020000034753\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034753\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:52:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125586623\",\r\n            \"Amount\": \"170.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007323467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867078\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:52:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867110\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:56:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867174\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:59:31.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867434\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:41:31.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867441\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:43:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867475\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:47:49.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867479\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:48:58.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867498\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:53:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867501\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:56:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867509\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:57:34.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867589\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD4735020000028655\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000028655\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:05:08.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"128141058\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001780825\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867694\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD8935010000060540\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060540\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:21:33.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"923774677\",\r\n            \"Amount\": \"40.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008289964\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867855\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD5835010000060031\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060031\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:30:19.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"124984488\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007081420\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"868407\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD4635020000022174\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000022174\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:58:40.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"129900755\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001974314\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052444732\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD5935020000034550\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034550\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 01:58:32.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"20.0\",\r\n            \"NS_RRN\": \"399685071\",\r\n            \"Tran_ID\": \"875873\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"999657414\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003929\",\r\n            \"User_ID\": \"005286215\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052446251\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0235020000033927\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033927\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:00:53.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"80.0\",\r\n            \"NS_RRN\": \"399693876\",\r\n            \"Tran_ID\": \"877354\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"910688295\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003961\",\r\n            \"User_ID\": \"001354675\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052446415\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7035010000043422\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043422\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:07:40.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399694726\",\r\n            \"Tran_ID\": \"877522\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123799232\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003967\",\r\n            \"User_ID\": \"005350384\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448596\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0735010000000518\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000000518\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:05:02.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"1000.0\",\r\n            \"NS_RRN\": \"399750928\",\r\n            \"Tran_ID\": \"881221\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Pay_Customer_Code\": \"914374971\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004090\",\r\n            \"User_ID\": \"004267375\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448718\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD9835010000042821\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042821\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:36:32.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"9000.0\",\r\n            \"NS_RRN\": \"399758058\",\r\n            \"Tran_ID\": \"881556\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"127416009\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004105\",\r\n            \"User_ID\": \"004499837\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448744\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:44:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399759747\",\r\n            \"Tran_ID\": \"881609\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004109\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448750\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:45:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399759944\",\r\n            \"Tran_ID\": \"881618\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004110\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448752\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:45:46.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"4000.0\",\r\n            \"NS_RRN\": \"399760075\",\r\n            \"Tran_ID\": \"881621\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004111\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448759\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:47:13.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"2000.0\",\r\n            \"NS_RRN\": \"399760371\",\r\n            \"Tran_ID\": \"881652\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004112\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448951\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:50:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399770929\",\r\n            \"Tran_ID\": \"882130\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004122\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448959\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD1835020000032378\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032378\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:53:00.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"3500.0\",\r\n            \"NS_RRN\": \"399771097\",\r\n            \"Tran_ID\": \"882152\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112216713\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004123\",\r\n            \"User_ID\": \"000366725\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448965\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD1835020000032378\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032378\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:54:01.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"500.0\",\r\n            \"NS_RRN\": \"399771231\",\r\n            \"Tran_ID\": \"882156\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112216713\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004124\",\r\n            \"User_ID\": \"000366725\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448987\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:00:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399772105\",\r\n            \"Tran_ID\": \"882214\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004128\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449016\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD7335010000060440\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060440\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:13:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"600.0\",\r\n            \"NS_RRN\": \"399773447\",\r\n            \"Tran_ID\": \"882269\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112770008\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004130\",\r\n            \"User_ID\": \"007661148\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449025\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD0935010000059555\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059555\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"2000.0\",\r\n            \"NS_RRN\": \"399773668\",\r\n            \"Tran_ID\": \"882289\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"123701535\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004133\",\r\n            \"User_ID\": \"007029562\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449027\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD8335020000030626\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000030626\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399773752\",\r\n            \"Tran_ID\": \"882292\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"129272811\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004134\",\r\n            \"User_ID\": \"003624431\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449029\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD0935010000059555\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059555\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"1000.0\",\r\n            \"NS_RRN\": \"399773754\",\r\n            \"Tran_ID\": \"882295\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"123701535\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004135\",\r\n            \"User_ID\": \"007029562\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449070\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:35:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"650.0\",\r\n            \"NS_RRN\": \"399775767\",\r\n            \"Tran_ID\": \"882415\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004139\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449080\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD9835020000025797\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000025797\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:40:44.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"1750.0\",\r\n            \"NS_RRN\": \"399776141\",\r\n            \"Tran_ID\": \"882433\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"124774948\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004140\",\r\n            \"User_ID\": \"006769593\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449177\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD9135020000033780\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033780\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:29:03.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399783266\",\r\n            \"Tran_ID\": \"882729\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"124081274\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004144\",\r\n            \"User_ID\": \"002809996\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449364\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 07:24:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399790033\",\r\n            \"Tran_ID\": \"883105\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004156\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448707\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD9835010000042821\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042821\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:35:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"9000.0\",\r\n            \"NS_RRN\": \"399757740\",\r\n            \"Tran_ID\": \"881545\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"127416009\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004103\",\r\n            \"User_ID\": \"004499837\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052450175\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD4935010000024012\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000024012\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 08:42:42.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399803708\",\r\n            \"Tran_ID\": \"883745\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"912633031\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004171\",\r\n            \"User_ID\": \"005889866\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052459351\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD4935010000026825\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000026825\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:35:08.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399863713\",\r\n            \"Tran_ID\": \"888368\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"110856400\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004269\",\r\n            \"User_ID\": \"004012763\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052459367\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD4935010000026825\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000026825\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:35:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399863881\",\r\n            \"Tran_ID\": \"888383\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"110856400\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004270\",\r\n            \"User_ID\": \"004012763\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052460351\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 01:06:47.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399872687\",\r\n            \"Tran_ID\": \"889155\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004288\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052460376\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 01:07:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399872910\",\r\n            \"Tran_ID\": \"889177\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004290\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448045\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0635010000059900\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059900\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:11:15.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"1400.0\",\r\n            \"NS_RRN\": \"399724577\",\r\n            \"Tran_ID\": \"879924\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"961366803\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004036\",\r\n            \"User_ID\": \"007568836\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448082\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD6135010000049598\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000049598\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:22:12.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"5800.0\",\r\n            \"NS_RRN\": \"399725926\",\r\n            \"Tran_ID\": \"880030\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Pay_Customer_Code\": \"999205305\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004040\",\r\n            \"User_ID\": \"007809851\"\r\n        }\r\n    ]\r\n}";
            //    //String test2 = Session["username"].ToString();
            //    JObject jobj = new JObject();
            //    jobj = JObject.Parse(apirespone);
            //    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
            //    dynamic result = jobj;
            //    //JObject jobj = new JObject();
            //    //jobj = JObject.Parse(response);


            //    //________________________
            //    JArray jobj2 = new JArray();
            //    jobj2 = JArray.FromObject(result.Trans_List);

            //    //int NoOfAcc = result.NoOfAct;
            //    //_____________________________________LUBNA________________________________________________________________________________-
            //    List<Dispute> disputes = new List<Dispute>();

            //    int Response_Code = result.GetValue("Response_Code");
            //    string ResponseMessage = result.GetValue("Response_Message").ToString();
            //    if (Response_Code == 0) { 
            //    //foreach (JObject Item in result)
            //    //{
            //        //string ResponseMessage = Item.GetValue("Response_Message").ToString();
            //        foreach (JObject i in jobj2)
            //        //for (int i = 0; i < jobj2.length; i++)
            //        {
            //            string Response_Message = i.GetValue("Response_Message").ToString();
            //            string Tran_DateTime = i.GetValue("Tran_DateTime").ToString();
            //            string Service_Name = i.GetValue("Service_Name").ToString();
            //            string Pay_Customer_Code = i.GetValue("Pay_Customer_Code").ToString();
            //            string Amount = i.GetValue("Amount").ToString();
            //            string Tran_Status = i.GetValue("Tran_Status").ToString();
            //            string User_ID = i.GetValue("User_ID").ToString();

            //            JObject Subjobj = new JObject();
            //            Subjobj = JObject.Parse(i.GetValue("Account_Info").ToString());
            //            dynamic Subresult = Subjobj;

            //            string Branch_Name = Subresult.GetValue("Branch_Name").ToString();
            //            string IBAN = Subresult.GetValue("IBAN").ToString();
            //            string Account_Type = Subresult.GetValue("Account_Type").ToString();
            //            string Account_No = Subresult.GetValue("Account_No").ToString();
            //            // string CUST_NO = Subresult.GetValue("Branch_Name").ToString();

            //            ///
            //            //string acctype = ds.getaccounttypename(ACT_TYPE);
            //            //string AccCode = ds.getaccounttypeCode(ACT_TYPE);
            //            ///
            //            disputes.Add(new Dispute()
            //            {
            //                Response_Message = Response_Message,
            //                Tran_DateTime = Tran_DateTime,
            //                Service_Name = Service_Name,
            //                Pay_Customer_Code = Pay_Customer_Code,
            //                Amount = Amount,
            //                STATUS = Tran_Status,
            //                User_ID = User_ID,
            //                Branch_Name = Branch_Name,
            //                IBAN = IBAN,
            //                Account_Type = Account_Type,
            //                Account_No = Account_No,


            //            });

            //        }
            //    }
            ////}

            List<Dispute> disputes = new List<Dispute>();
            Session["Dispute"] = disputes;
            //model = Disputes;
            // disputes = ds.GetAllDisputes();
            return View(model);

        }

        

        public ActionResult DisputeDetails(string dispute_id)
        {
            //getting dispute details data
            Dispute dispute_details = new Dispute();
            dispute_details = ds.GetDispute(dispute_id);
            //getting dispute comments
            dispute_details.Comments = ds.GetDisputeComments(dispute_id);
            Session["disputetoedit"] = dispute_id;

            //getting transaction data
            CustomerTransferReportViewModel transaction = ds.GetTransactionDetails(dispute_details.TRANSACTIONID);
            transaction.dispute_reasons = ds.GetDisputeReasons();
            transaction.dispute_actions = ds.GetDisputeActions();
            List<SelectListItem> dispute_action_list = new List<SelectListItem>();
            foreach (Dispute_Action_Model action in transaction.dispute_actions)
            {
                dispute_action_list.Add(new SelectListItem
                {
                    Value = action.id,
                    Text = action.action
                });
            }
            transaction.dispute_actions_list = dispute_action_list;

            //setting transaction model fields based on which transactions type it is
            dynamic requestdata = JObject.Parse(transaction.TranFullReq);
            dynamic responsedata = JObject.Parse(transaction.TranFullResp);
            if (requestdata.PAN != null)
            {
                //accounttocard
                transaction.TranToAccount = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.PAN = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                //transaction.CustomerName = requestdata.customerName;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words = word.Split(':');
                transaction.FT = words[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            else
            {
                //accounttoaccount
                transaction.TranFromAccount = requestdata.accountfrom;
                transaction.TranToAccount = requestdata.accountto;
                //transaction.Customername = requestdata.FromAccountName;
                //transaction.CustomerName = requestdata.recipientName;
                transaction.ResponseStatus = responsedata.status;
                if (responsedata.status != "")
                {
                    if (transaction.ResponseStatus.ToString() != "00")
                    {
                        string word = transaction.ResponseStatus;
                        string[] words = word.Split(':');
                        transaction.FT = words[1];
                    }
                }
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            Session["detailedtransaction"] = transaction;
            Session["dispute_details"] = dispute_details;
            return View(transaction);
        }

        [HttpGet]
        public ActionResult EPort()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult EPort(EPortReceipt receipt)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            EPortReceipt filledreceipt = ds.GetEPortReceipt(receipt.tran_payserviceid);
            if (filledreceipt.tran_bankode != null)
            {
                Session["eport_receipt"] = filledreceipt;
                return RedirectToAction("GetEPortReceipt", "CustomerReport");
            }
            else
            {
                string message = "No receipt found";
                ModelState.AddModelError("", message);
                return View(filledreceipt);
            }
        }

        public ActionResult GetEPortReceipt()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            EPortReceipt receipt = new EPortReceipt();
            receipt = (EPortReceipt)Session["eport_receipt"];
            return View(receipt);
        }

        [HttpPost]
        public ActionResult CustomersReportprocess(Custreport passedmodel)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            if (passedmodel.Branch != null)
            {
                model = new Custreport();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetCustomerReportData(passedmodel.Branch);
                model.catgories = ds.GetGatgories();
                model.CustomerStatus = ds.PopulateCustStatus(passedmodel.Branch);
                model.Branches = ds.PopulateBranchs(model.BranchCode, passedmodel.Branch);
                model.catgories = ds.GetGatgories();
                //model.catgories.RemoveAt(0);
                return View("CustomersReport", model);
            }
            else
            {
                String userbranch = Session["user_branch"].ToString();


                model.Branches = ds.PopulateBranchs(userbranch);
                model.catgories = ds.GetGatgories();
                model.CustomerStatus = ds.PopulateCustStatus();

                Session["CustReport"] = model;

                return View("CustomersReport", model);
            }
        }

        public ActionResult CustomersRegistrationReport(Custreport model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string adminbranch = Session["user_branch"].ToString();
            List<Custreport> customers = ds.getbranchcustomers(adminbranch);
            return View(customers);
        }

        public ActionResult ViewReport()
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            List<Custreport> accass = new List<Custreport>();
            accass = (List<Custreport>)Session["BranchUsers"];
            ViewBag.Total = accass.Count;
            ViewBag.Date = DateTime.Now.ToString("dd-MMM-yyyy");
            //  ViewBag.Username = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Time = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Branchname = Session["Branchname"].ToString();
            Session["totalcustomer"] = accass.Count;
            return View(accass);

        }

        public ActionResult ViewCountReport()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            List<Custreport> accass = new List<Custreport>();
            accass = (List<Custreport>)Session["BranchUsersCount"];
            int totaluser = ds.getuserscount();
            //    ViewBag.Total = accass.Count;
            ViewBag.Total = totaluser;
            ViewBag.Date = DateTime.Now.ToString("dd-MMM-yyyy");
            //  ViewBag.Username = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Time = DateTime.Now.ToString("HH:mm:ss");
            ViewBag.Branchname = Session["Branchname"].ToString();
            Session["totalcustomer"] = totaluser;
            return View(accass);
        }

        public ActionResult CreditAPIReport()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> transaction_names_list = new List<SelectListItem>();
            transaction_names_list.Add(new SelectListItem { Text = "All", Value = "All" });
            transaction_names_list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            transaction_names_list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });
            List<SelectListItem> transactions_statuses = new List<SelectListItem>();
            transactions_statuses.Add(new SelectListItem { Text = "All", Value = "All" });
            transactions_statuses.Add(new SelectListItem { Text = "Successful", Value = "Secussfully" });
            transactions_statuses.Add(new SelectListItem { Text = "Failed", Value = "Failed" });

            model.transactions_names = transaction_names_list;
            model.transactions_statuses = transactions_statuses;

            List<CustomerTransferReportViewModel> creditapitransactions = new List<CustomerTransferReportViewModel>();
            Session["creditapitransactions"] = creditapitransactions;
            return View(model);
        }


        public ActionResult DetailedTranReport()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> transaction_names_list = new List<SelectListItem>();
            transaction_names_list.Add(new SelectListItem { Text = "All", Value = "All" });
            transaction_names_list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            transaction_names_list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });
            List<SelectListItem> transactions_statuses = new List<SelectListItem>();
            transactions_statuses.Add(new SelectListItem { Text = "All", Value = "" });
            transactions_statuses.Add(new SelectListItem { Text = "Successful", Value = "S" });
            transactions_statuses.Add(new SelectListItem { Text = "Failed", Value = "F" });

            model.transactions_names = transaction_names_list;
            model.transactions_statuses = transactions_statuses;

            List<TranDetails> creditapitransactions = new List<TranDetails>();
            Session["creditapitransactions"] = creditapitransactions;
            return View(model);
        }


        public ActionResult UserRegReport()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> transaction_names_list = new List<SelectListItem>();
            transaction_names_list.Add(new SelectListItem { Text = "All", Value = "All" });
            transaction_names_list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            transaction_names_list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });
            List<SelectListItem> transactions_statuses = new List<SelectListItem>();
            transactions_statuses.Add(new SelectListItem { Text = "All", Value = "All" });
            transactions_statuses.Add(new SelectListItem { Text = "Successful", Value = "S" });
            transactions_statuses.Add(new SelectListItem { Text = "Failed", Value = "F" });

            model.transactions_names = transaction_names_list;
            model.transactions_statuses = transactions_statuses;

            List<CustomerTransferReportViewModel> creditapitransactions = new List<CustomerTransferReportViewModel>();
            Session["creditapitransactions"] = creditapitransactions;
            return View(model);
        }


        public ActionResult TransactionDetails(string TransactionID)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string[] words2 = TransactionID.Split('?');
            TransactionID = words2[0];
            Session["transactionid"] = TransactionID;

            //getting transaction details
            CustomerTransferReportViewModel transaction = ds.GetTransactionDetails(TransactionID);
            transaction.dispute_reasons = ds.GetDisputeReasons();
            transaction.dispute_actions = ds.GetDisputeActions();
            List<SelectListItem> dispute_action_list = new List<SelectListItem>();
            foreach (Dispute_Action_Model action in transaction.dispute_actions)
            {
                dispute_action_list.Add(new SelectListItem
                {
                    Value = action.id,
                    Text = action.action
                });
            }
            // this is dispute actions list retrival #2
            //transaction.dispute_actions_list = dispute_action_list;
            //setting transaction model fields based on which transactions type it is
            dynamic requestdata = JObject.Parse(transaction.TranFullReq);
            dynamic responsedata = JObject.Parse(transaction.TranFullResp);
            if (requestdata.PAN != null)
            {
                //accounttocard
                transaction.TranToAccount = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.PAN = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                //transaction.CustomerName = requestdata.customerName;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words = word.Split(':');
                transaction.FT = words[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            else
            {
                //accounttoaccount
                transaction.TranFromAccount = requestdata.accountfrom;
                transaction.TranToAccount = requestdata.accountto;
                //transaction.Customername = requestdata.FromAccountName;
                //transaction.CustomerName = requestdata.recipientName;
                transaction.ResponseStatus = responsedata.status;
                if (responsedata.status != "")
                {
                    if (transaction.ResponseStatus.ToString() != "00")
                    {
                        string word = transaction.ResponseStatus;
                        string[] words = word.Split(':');
                        transaction.FT = words[1];
                    }
                }
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            Session["detailedtransaction"] = transaction;
            return View(transaction);
        }

        [HttpGet]
        public JsonResult DateFilteredCreditAPIReport(string fromdate, string todate)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString().Substring(0, 10);
            string formatedtodate = DateTime.Parse(todate).ToString().Substring(0, 10);

            List<CustomerTransferReportViewModel> creditapitransactions = ds.DateFilteredGetCreditAPITransaction(formatedFromDate, formatedtodate);
            foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.PAN = requestdata.PAN;
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words2 = word.Split(':');
                transaction.FT = words2[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
                //transaction.CustomerName = requestdata.customerName;
            }

            JsonResult data = Json(new { data = creditapitransactions }, JsonRequestBehavior.AllowGet);
            return data;
        }

        public ActionResult TransactionsInfo()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> transaction_names_list = new List<SelectListItem>();
            transaction_names_list.Add(new SelectListItem { Text = "All", Value = "All" });
            transaction_names_list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            transaction_names_list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });
            List<SelectListItem> transactions_statuses = new List<SelectListItem>();
            transactions_statuses.Add(new SelectListItem { Text = "All", Value = "All" });
            transactions_statuses.Add(new SelectListItem { Text = "Successful", Value = "Secussfully" });
            transactions_statuses.Add(new SelectListItem { Text = "Failed", Value = "Failed" });

            model.transactions_names = transaction_names_list;
            model.transactions_statuses = transactions_statuses;

            List<CustomerTransferReportViewModel> transactionsinfo = new List<CustomerTransferReportViewModel>();
            Session["transactionsinfo"] = transactionsinfo;

            return View(model);
        }

        [HttpGet]
        public JsonResult FilteredTransactionsInfo(string fromdate, string todate, string branch, string status, string accountnumber, string toaccount)
        {
            string formatedFromDate = "", formatedtodate = "";

            formatedFromDate = DateTime.Parse(fromdate).ToString();
            formatedtodate = DateTime.Parse(todate).ToString();
            string[] words = formatedFromDate.Split(' ');
            formatedFromDate = words[0];
            words = formatedtodate.Split(' ');
            formatedtodate = words[0];

            List<CustomerTransferReportViewModel> filteredtransactions = new List<CustomerTransferReportViewModel>();
            filteredtransactions = ds.FilteredTransactionsInfo(formatedFromDate, formatedtodate, branch, status, accountnumber, toaccount);

            foreach (CustomerTransferReportViewModel transaction in filteredtransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                //dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                if (requestdata.PAN != null)
                {
                    transaction.TranToAccount = requestdata.PAN;
                    transaction.TranFromAccount = requestdata.Fromaccount;
                }
                else
                {
                    transaction.TranToAccount = requestdata.accountto;
                    transaction.TranFromAccount = requestdata.accountfrom;
                }
            }

            Session["transactionsinfo"] = filteredtransactions;
            JsonResult data = Json(new { data = filteredtransactions }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }





        [HttpGet]
        public JsonResult FilteredDispute(string fromdate, string todate, string branch, string service, string accountType)
        {
            string formatedFromDate = "", formatedtodate = "";

            formatedFromDate = DateTime.Parse(fromdate).ToString();
            formatedtodate = DateTime.Parse(todate).ToString();
            string[] words = formatedFromDate.Split(' ');
            formatedFromDate = words[0];


            string FromDate = Convert.ToDateTime(formatedFromDate).ToString("dd/MM/yyyy"); //returns 25/09/2011
            //DateTime date = DateTime.Parse(d, new CultureInfo("en-GB"));
            

            words = formatedtodate.Split(' ');
            formatedtodate = words[0];

            string ToDate = Convert.ToDateTime(formatedtodate).ToString("dd/MM/yyyy"); //returns 25/09/2011
            //DateTime date2 = DateTime.Parse(dd, new CultureInfo("en-GB"));

            List<CustomerTransferReportViewModel> filteredtransactions = new List<CustomerTransferReportViewModel>();
           

            ///////////////////////
            string apirespone = Connecttocore.getDispute(Session["accesstoken"].ToString() , FromDate, ToDate, branch,service,accountType);

            //String apirespone = "{\r\n    \"Response_Code\": 0,\r\n    \"Response_Message\": \"SUCCESSFUL\",\r\n    \"Trans_List\": [\r\n        {\r\n            \"Tran_ID\": \"854337\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD6235010000039589\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000039589\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:11:18.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"926519501\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001344845\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854488\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD6835010000040160\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000040160\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:30:17.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"998827294\",\r\n            \"Amount\": \"18250.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004349593\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854632\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD1935010000032797\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000032797\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:54:58.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"999207061\",\r\n            \"Amount\": \"13500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004543332\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854444\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD2135020000031451\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000031451\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:25:38.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"920818189\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000251467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"855237\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD6635010000051060\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000051060\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 11:42:24.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912365455\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004332584\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"855616\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-unable to connect to Raseedo\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3735020000033297\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033297\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 06:29:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123728596\",\r\n            \"Amount\": \"200.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000442841\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"856465\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD1835020000034512\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034512\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 10:25:03.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"962021425\",\r\n            \"Amount\": \"50.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006111742\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"857000\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD0935010000005429\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000005429\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 12:03:44.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912310625\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001123236\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852457\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 06:02:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854902\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6735010000010302\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000010302\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 10:35:30.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111151357\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000262389\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"857696\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6935010000034031\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000034031\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 02:27:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"120042662\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000256544\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"854470\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD6835010000040160\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000040160\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 09:28:03.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"998827294\",\r\n            \"Amount\": \"18250.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004349593\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"859076\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6135010000046494\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000046494\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 06:06:35.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"918109104\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002438366\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860291\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alamarat Branch\",\r\n                \"IBAN\": \"SD8535020000032045\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032045\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:22:09.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122538414\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003430686\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860501\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD6035010000004802\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004802\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:50:47.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"922530083\",\r\n            \"Amount\": \"7000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000453773\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860546\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD8135020000005151\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000005151\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 09:52:54.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"112152200\",\r\n            \"Amount\": \"15000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000579824\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"850111\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD0835020000006271\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000006271\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 02:35:54.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125996515\",\r\n            \"Amount\": \"6446.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000329373\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"851982\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD1935010000060539\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060539\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:17:57.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"119029427\",\r\n            \"Amount\": \"4480.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008279587\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852365\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:54:59.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852368\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:55:24.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852383\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:56:11.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852397\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:57:10.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852406\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:57:57.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852411\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD3335020000027452\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000027452\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 05:58:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"122952695\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005157913\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"852919\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"AL-Qadaref Branch\",\r\n                \"IBAN\": \"SD6135010000013514\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000013514\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 06:52:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125222080\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006588382\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"853235\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD4135010000046563\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000046563\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 07:34:01.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"923115881\",\r\n            \"Amount\": \"10100.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000201438\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"853344\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD2435010000032513\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000032513\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-05 07:41:36.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"916174198\",\r\n            \"Amount\": \"2500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003579491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"860922\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD6235020000033817\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033817\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-06 10:55:07.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"996725897\",\r\n            \"Amount\": \"6000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003898907\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861798\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5835010000004935\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004935\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:04:07.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912336320\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005861491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861801\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5835010000004935\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000004935\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:04:41.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"912336320\",\r\n            \"Amount\": \"9000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005861491\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"861817\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935010000061122\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000061122\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:06:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123087444\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005860848\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"862071\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD7435010000034814\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000034814\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:42:13.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"115044367\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"000210605\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052429937\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9135010000060601\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060601\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:07:46.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399615930\",\r\n            \"Tran_ID\": \"868457\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Pay_Customer_Code\": \"999011653\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003740\",\r\n            \"User_ID\": \"008039164\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052429942\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9135010000060601\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060601\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:10:09.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399615998\",\r\n            \"Tran_ID\": \"868473\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"999011653\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003742\",\r\n            \"User_ID\": \"008039164\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052430090\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 07:55:45.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399620563\",\r\n            \"Tran_ID\": \"868656\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123137954\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003747\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052430468\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD9035020000034186\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034186\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:25:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399624929\",\r\n            \"Tran_ID\": \"868945\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"113120694\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003758\",\r\n            \"User_ID\": \"005858746\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052439069\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8435010000011354\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000011354\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 11:48:11.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399664071\",\r\n            \"Tran_ID\": \"873119\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"111966698\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003839\",\r\n            \"User_ID\": \"002304286\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440069\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD3435010000027968\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000027968\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 12:10:52.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399668201\",\r\n            \"Tran_ID\": \"873584\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123990939\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003857\",\r\n            \"User_ID\": \"000831119\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440094\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alamarat Branch\",\r\n                \"IBAN\": \"SD0735010000044362\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000044362\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 12:11:18.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399668246\",\r\n            \"Tran_ID\": \"873588\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123675016\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003858\",\r\n            \"User_ID\": \"003146866\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052440515\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5135010000052494\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000052494\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:20:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399669760\",\r\n            \"Tran_ID\": \"873783\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"126397399\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003864\",\r\n            \"User_ID\": \"006877821\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447148\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Soug Laybya\",\r\n                \"IBAN\": \"SD4535010000042611\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042611\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:52:39.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399701820\",\r\n            \"Tran_ID\": \"878172\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"121211514\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003994\",\r\n            \"User_ID\": \"001431545\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447157\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Soug Laybya\",\r\n                \"IBAN\": \"SD4535010000042611\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042611\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:53:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399701957\",\r\n            \"Tran_ID\": \"878195\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"121211514\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003995\",\r\n            \"User_ID\": \"001431545\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447680\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD7135020000000190\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000000190\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:57:04.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399712397\",\r\n            \"Tran_ID\": \"879105\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"122851992\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004017\",\r\n            \"User_ID\": \"007070360\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052447686\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD7135020000000190\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000000190\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:57:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"4900.0\",\r\n            \"NS_RRN\": \"399712445\",\r\n            \"Tran_ID\": \"879111\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"122851992\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004018\",\r\n            \"User_ID\": \"007070360\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"863141\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD7735020000020937\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000020937\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:14:43.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"126346508\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"003685860\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"865486\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 05:21:28.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111473288\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"865507\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7635020000034729\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034729\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 05:23:45.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"111473288\",\r\n            \"Amount\": \"5000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006930311\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866213\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD5335020000034270\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034270\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 07:18:08.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Pay_Customer_Code\": \"911820754\",\r\n            \"Amount\": \"300.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"004697881\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866392\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD6735020000032078\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032078\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 07:38:01.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"110235691\",\r\n            \"Amount\": \"8000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002650912\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"866676\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD0535010000057299\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000057299\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:05:53.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"925029343\",\r\n            \"Amount\": \"180.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"006521652\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867009\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"AlMawrada Branch\",\r\n                \"IBAN\": \"SD3735010000061282\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000061282\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:41:58.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"127970524\",\r\n            \"Amount\": \"200.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008274385\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867060\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD1035020000034753\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034753\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:50:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125586623\",\r\n            \"Amount\": \"170.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007323467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867071\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:51:20.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867076\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD1035020000034753\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034753\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:52:17.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"125586623\",\r\n            \"Amount\": \"170.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007323467\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867078\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:52:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867110\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:56:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867174\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Eltaif Branch\",\r\n                \"IBAN\": \"SD9635010000043148\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043148\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 08:59:31.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"121349770\",\r\n            \"Amount\": \"2000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"002703224\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867434\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:41:31.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867441\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:43:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867475\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:47:49.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867479\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:48:58.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867498\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:53:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867501\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:56:33.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867509\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD8135010000003357\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000003357\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:57:34.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"123601353\",\r\n            \"Amount\": \"700.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"005369731\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867589\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD4735020000028655\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000028655\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:05:08.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"128141058\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001780825\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867694\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD8935010000060540\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060540\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 10:21:33.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Pay_Customer_Code\": \"923774677\",\r\n            \"Amount\": \"40.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"008289964\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"867855\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD5835010000060031\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060031\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-07 11:30:19.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"124984488\",\r\n            \"Amount\": \"500.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"007081420\"\r\n        },\r\n        {\r\n            \"Tran_ID\": \"868407\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Port Sudan Branch\",\r\n                \"IBAN\": \"SD4635020000022174\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000022174\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:58:40.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Pay_Customer_Code\": \"129900755\",\r\n            \"Amount\": \"10000.0\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"User_ID\": \"001974314\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052444732\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD5935020000034550\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000034550\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 01:58:32.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"20.0\",\r\n            \"NS_RRN\": \"399685071\",\r\n            \"Tran_ID\": \"875873\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"999657414\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003929\",\r\n            \"User_ID\": \"005286215\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052446251\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0235020000033927\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033927\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 04:00:53.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"80.0\",\r\n            \"NS_RRN\": \"399693876\",\r\n            \"Tran_ID\": \"877354\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"910688295\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003961\",\r\n            \"User_ID\": \"001354675\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052446415\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Khartoum Branch\",\r\n                \"IBAN\": \"SD7035010000043422\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000043422\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 03:07:40.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399694726\",\r\n            \"Tran_ID\": \"877522\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"123799232\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108003967\",\r\n            \"User_ID\": \"005350384\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448596\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0735010000000518\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000000518\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:05:02.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"1000.0\",\r\n            \"NS_RRN\": \"399750928\",\r\n            \"Tran_ID\": \"881221\",\r\n            \"Response_Code\": \"-999\",\r\n            \"Response_Message\": \"Failed-Connection time out occurred with biller\",\r\n            \"Pay_Customer_Code\": \"914374971\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004090\",\r\n            \"User_ID\": \"004267375\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448718\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD9835010000042821\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042821\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:36:32.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"9000.0\",\r\n            \"NS_RRN\": \"399758058\",\r\n            \"Tran_ID\": \"881556\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"127416009\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004105\",\r\n            \"User_ID\": \"004499837\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448744\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:44:25.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399759747\",\r\n            \"Tran_ID\": \"881609\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004109\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448750\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:45:21.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399759944\",\r\n            \"Tran_ID\": \"881618\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004110\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448752\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:45:46.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"4000.0\",\r\n            \"NS_RRN\": \"399760075\",\r\n            \"Tran_ID\": \"881621\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004111\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448759\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:47:13.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"2000.0\",\r\n            \"NS_RRN\": \"399760371\",\r\n            \"Tran_ID\": \"881652\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004112\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448951\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:50:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399770929\",\r\n            \"Tran_ID\": \"882130\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004122\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448959\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD1835020000032378\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032378\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:53:00.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"3500.0\",\r\n            \"NS_RRN\": \"399771097\",\r\n            \"Tran_ID\": \"882152\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112216713\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004123\",\r\n            \"User_ID\": \"000366725\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448965\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD1835020000032378\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000032378\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 09:54:01.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"500.0\",\r\n            \"NS_RRN\": \"399771231\",\r\n            \"Tran_ID\": \"882156\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112216713\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004124\",\r\n            \"User_ID\": \"000366725\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448987\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:00:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399772105\",\r\n            \"Tran_ID\": \"882214\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004128\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449016\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD7335010000060440\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000060440\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:13:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"600.0\",\r\n            \"NS_RRN\": \"399773447\",\r\n            \"Tran_ID\": \"882269\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"112770008\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004130\",\r\n            \"User_ID\": \"007661148\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449025\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD0935010000059555\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059555\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:05.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"2000.0\",\r\n            \"NS_RRN\": \"399773668\",\r\n            \"Tran_ID\": \"882289\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"123701535\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004133\",\r\n            \"User_ID\": \"007029562\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449027\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD8335020000030626\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000030626\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399773752\",\r\n            \"Tran_ID\": \"882292\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"129272811\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004134\",\r\n            \"User_ID\": \"003624431\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449029\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsagana Branch\",\r\n                \"IBAN\": \"SD0935010000059555\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059555\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:15:56.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"1000.0\",\r\n            \"NS_RRN\": \"399773754\",\r\n            \"Tran_ID\": \"882295\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"123701535\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004135\",\r\n            \"User_ID\": \"007029562\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449070\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:35:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"650.0\",\r\n            \"NS_RRN\": \"399775767\",\r\n            \"Tran_ID\": \"882415\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004139\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449080\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD9835020000025797\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000025797\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 10:40:44.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"1750.0\",\r\n            \"NS_RRN\": \"399776141\",\r\n            \"Tran_ID\": \"882433\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"124774948\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004140\",\r\n            \"User_ID\": \"006769593\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449177\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Bahari Branch\",\r\n                \"IBAN\": \"SD9135020000033780\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033780\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:29:03.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399783266\",\r\n            \"Tran_ID\": \"882729\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"124081274\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004144\",\r\n            \"User_ID\": \"002809996\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052449364\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Madani Branch\",\r\n                \"IBAN\": \"SD8935020000033816\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000033816\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 07:24:51.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"700.0\",\r\n            \"NS_RRN\": \"399790033\",\r\n            \"Tran_ID\": \"883105\",\r\n            \"Response_Code\": \"-106\",\r\n            \"Response_Message\": \"Failed-External System Error\",\r\n            \"Pay_Customer_Code\": \"129041353\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004156\",\r\n            \"User_ID\": \"003377435\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448707\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Alsuq Almahali\",\r\n                \"IBAN\": \"SD9835010000042821\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000042821\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 08:35:02.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"9000.0\",\r\n            \"NS_RRN\": \"399757740\",\r\n            \"Tran_ID\": \"881545\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"127416009\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004103\",\r\n            \"User_ID\": \"004499837\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052450175\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Omdurman Branch\",\r\n                \"IBAN\": \"SD4935010000024012\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000024012\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 08:42:42.0\",\r\n            \"Service_Name\": \"Zain Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399803708\",\r\n            \"Tran_ID\": \"883745\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"912633031\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004171\",\r\n            \"User_ID\": \"005889866\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052459351\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD4935010000026825\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000026825\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:35:08.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399863713\",\r\n            \"Tran_ID\": \"888368\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"110856400\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004269\",\r\n            \"User_ID\": \"004012763\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052459367\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Saad Geshra Branch\",\r\n                \"IBAN\": \"SD4935010000026825\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000026825\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 12:35:38.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"6000.0\",\r\n            \"NS_RRN\": \"399863881\",\r\n            \"Tran_ID\": \"888383\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"110856400\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004270\",\r\n            \"User_ID\": \"004012763\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052460351\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 01:06:47.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"10000.0\",\r\n            \"NS_RRN\": \"399872687\",\r\n            \"Tran_ID\": \"889155\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004288\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052460376\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Al-Kalakla Branch\",\r\n                \"IBAN\": \"SD9735020000007773\",\r\n                \"Account_Type\": \"SAVING\",\r\n                \"Account_No\": \"20000007773\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-09 01:07:26.0\",\r\n            \"Service_Name\": \"Sudani Topup\",\r\n            \"Amount\": \"5000.0\",\r\n            \"NS_RRN\": \"399872910\",\r\n            \"Tran_ID\": \"889177\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 4000.0 )\",\r\n            \"Pay_Customer_Code\": \"116101835\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230109004290\",\r\n            \"User_ID\": \"005839489\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448045\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Aljazeera Tower Branch\",\r\n                \"IBAN\": \"SD0635010000059900\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000059900\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:11:15.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"1400.0\",\r\n            \"NS_RRN\": \"399724577\",\r\n            \"Tran_ID\": \"879924\",\r\n            \"Response_Code\": \"-520\",\r\n            \"Response_Message\": \"Failed-pre-check operation has failed\",\r\n            \"Pay_Customer_Code\": \"961366803\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004036\",\r\n            \"User_ID\": \"007568836\"\r\n        },\r\n        {\r\n            \"Reference_No\": \"00001310000052448082\",\r\n            \"Account_Info\": {\r\n                \"Branch_Name\": \"Kassala Branch\",\r\n                \"IBAN\": \"SD6135010000049598\",\r\n                \"Account_Type\": \"CURRENT\",\r\n                \"Account_No\": \"10000049598\"\r\n            },\r\n            \"Tran_DateTime\": \"2023-01-08 06:22:12.0\",\r\n            \"Service_Name\": \"MTN Topup\",\r\n            \"Amount\": \"5800.0\",\r\n            \"NS_RRN\": \"399725926\",\r\n            \"Tran_ID\": \"880030\",\r\n            \"Response_Code\": \"-512\",\r\n            \"Response_Message\": \"Failed-TransactionPaidAmount is greater than maximum allowed bill amount ( maximumBillAmount : 5000.0 )\",\r\n            \"Pay_Customer_Code\": \"999205305\",\r\n            \"Tran_Status\": \"RR\",\r\n            \"App_RRN\": \"230108004040\",\r\n            \"User_ID\": \"007809851\"\r\n        }\r\n    ]\r\n}";
            //String test2 = Session["username"].ToString();
            JObject jobj = new JObject();
            jobj = JObject.Parse(apirespone);
            //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
            dynamic result = jobj;
            //JObject jobj = new JObject();
            //jobj = JObject.Parse(response);


            //________________________
            JArray jobj2 = new JArray();
            jobj2 = JArray.FromObject(result.Trans_List);

            //int NoOfAcc = result.NoOfAct;
            //_____________________________________LUBNA________________________________________________________________________________-
            List<Dispute> disputes = new List<Dispute>();

            int Response_Code = result.GetValue("Response_Code");
            string ResponseMessage = result.GetValue("Response_Message").ToString();
            if (Response_Code == 0)
            {
                //foreach (JObject Item in result)
                //{
                //string ResponseMessage = Item.GetValue("Response_Message").ToString();
                foreach (JObject i in jobj2)
                //for (int i = 0; i < jobj2.length; i++)
                {
                    string Response_Message = i.GetValue("Response_Message").ToString();
                    string Tran_DateTime = i.GetValue("Tran_DateTime").ToString();
                    string Service_Name = i.GetValue("Service_Name").ToString();
                    string Pay_Customer_Code = "";
                    if (Service_Name == "Transfer To Card" )
                    {
                         Pay_Customer_Code = i.GetValue("To_Card").ToString();
                    }
                    else {
                       Pay_Customer_Code = i.GetValue("Pay_Customer_Code").ToString();
                        }
                    string Amount = i.GetValue("Amount").ToString();
                    string Tran_Status = i.GetValue("Tran_Status").ToString();
                    string User_ID = i.GetValue("User_ID").ToString();

                    JObject Subjobj = new JObject();
                    Subjobj = JObject.Parse(i.GetValue("Account_Info").ToString());
                    dynamic Subresult = Subjobj;

                    string Branch_Name = Subresult.GetValue("Branch_Name").ToString();
                    string IBAN = Subresult.GetValue("IBAN").ToString();
                    string Account_Type = Subresult.GetValue("Account_Type").ToString();
                    string Account_No = Subresult.GetValue("Account_No").ToString();
                    // string CUST_NO = Subresult.GetValue("Branch_Name").ToString();

                    ///
                    //string acctype = ds.getaccounttypename(ACT_TYPE);
                    //string AccCode = ds.getaccounttypeCode(ACT_TYPE);
                    ///
                    disputes.Add(new Dispute()
                    {
                        Response_Message = Response_Message,
                        Tran_DateTime = Tran_DateTime,
                        Service_Name = Service_Name,
                        Pay_Customer_Code = Pay_Customer_Code,
                        Amount = Amount,
                        STATUS = Tran_Status,
                        User_ID = User_ID,
                        Branch_Name = Branch_Name,
                        IBAN = IBAN,
                        Account_Type = Account_Type,
                        Account_No = Account_No,


                    });

                }
            }
            //}
            Session["CustomerInfo"] = disputes;





            ////////////////

            //Session["transactionsinfo"] = filteredtransactions;
            JsonResult data = Json(new { data = disputes }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }




        //customer registration report pdf
        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("UsersRegistrationReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(8);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_PDF2(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF2(PdfPTable tableLayout)
        {
            float[] headers = { 10, 15, 17, 25, 13, 10, 10,15 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

            //List<userlist> userlist = ds.GetAllusers();
            string adminbranch = Session["user_branch"].ToString();
            List<Custreport> customers = ds.getbranchcustomers(adminbranch);

           // tableLayout.AddCell(new PdfPCell(new Phrase("Customers Registration Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });


            DateTime dTime = DateTime.Now;

            //paragraphs
            Paragraph Title = new Paragraph("National Bank of Egept ",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customers Registration Report" ,
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
     
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
           
            
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 8,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 8,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

         

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 8,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 8,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 8,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });



            ////Add header
            AddCellToHeader2(tableLayout, "CUstomerID");
            AddCellToHeader2(tableLayout, "CustomerFullName");
           
            AddCellToHeader2(tableLayout, "AccountNumber");
            AddCellToHeader2(tableLayout, "PhoneNumber");
            AddCellToHeader2(tableLayout, "Address");
            AddCellToHeader2(tableLayout, "Customer Email");
            AddCellToHeader2(tableLayout, "Customer Status");
            AddCellToHeader2(tableLayout, "CreatedBy");

            ////Add body




            foreach (var customer in customers)
            {
                AddCellToBody2(tableLayout, customer.CustomerID);
                AddCellToBody2(tableLayout, customer.customerfullname);
            
                AddCellToBody2(tableLayout, customer.AccountNumber);
                AddCellToBody2(tableLayout, customer.phonenumber);
                AddCellToBody2(tableLayout, customer.address);
                AddCellToBody2(tableLayout, customer.customeremail);
                AddCellToBody2(tableLayout, customer.CustStatus);
                AddCellToBody2(tableLayout, customer.createdby);
            }

            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeader2(PdfPTable tableLayout, string cellText)
        {
            
             tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(54, 54, 77) });
        }

        // Method to add single cell to the body
        private static void AddCellToBody2(PdfPTable tableLayout, string cellText)
        {
            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);

            const string regex_match_arabic_hebrew = @"[\u0600-\u06FF\u0590-\u05FF]+";
            if (Regex.IsMatch(cellText, regex_match_arabic_hebrew, RegexOptions.IgnoreCase))
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
            else
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,

                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }



            //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }

        // end of pdf preperation

        public FileResult SavePDF()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("CustomerReport For " + Session["Branchname"].ToString() + dTime.ToString("ddMMMyyyyHHmmss") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            /*PdfPTable tableLayout = new PdfPTable(5);*/
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

        public FileResult SavePDF2()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("CustomerCountReport For " + Session["Branchname"].ToString() + dTime.ToString("ddMMMyyyyHHmmss") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            /*PdfPTable tableLayout = new PdfPTable(5);*/
            PdfPTable tableLayout = new PdfPTable(3);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   

            doc.Add(Add_Content_To_PDF3(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF3(PdfPTable tableLayout)
        {



            PdfPTableHeader tableHeader = new PdfPTableHeader();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);
            float[] headers = { 60, 40, 40 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 95; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;

            //Add Title to the PDF file at the top  

            //List < Employee > UserLog = _context.UserLog.ToList < Employee > ();  
            List<Custreport> UserLog = new List<Custreport>();
            UserLog = (List<Custreport>)Session["BranchUsersCount"];

            DateTime dTime = DateTime.Now;

            //paragraphs
            Paragraph Title = new Paragraph("National Bank of Egept ",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customer Report For " + Session["Branchname"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title3 = new Paragraph("Total Customers Registered :" + Session["totalcustomer"].ToString(),
             new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("Total of Customers Registered : " + Session["totalcustomer"].ToString(),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title3))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 2,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 2,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            ////Add header 

            AddCellToHeader(tableLayout, "Customer Branch");
            AddCellToHeader(tableLayout, "Cusyomer Count");
            AddCellToHeader(tableLayout, "Customer Status");
            //AddCellToHeader(tableLayout, "Branch");

            ////Add body  

            foreach (var emp in UserLog)
            {

                AddCellToBody(tableLayout, emp.Branch.ToString());
                AddCellToBody(tableLayout, emp.Count.ToString());
                AddCellToBody(tableLayout, emp.CustStatus.ToString());
                //AddCellToBody(tableLayout, emp.Branch.ToString());

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 20,

                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT

            });
            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(54, 54, 77),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        public FileResult SavePDF4()
        {



            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("CustomerCountReport For " /*+ Session["totalbillercustomer"].ToString() */+ dTime.ToString("ddMMMyyyyHHmmss") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 6 columns  
            /*PdfPTable tableLayout = new PdfPTable(5);*/
            PdfPTable tableLayout = new PdfPTable(8);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloadss/" + strPDFFileName);

            PdfWriter.GetInstance(doc, workStream).CloseStream = false;

            doc.Open();

            //Add Content to PDF   

            doc.Add(Add_Content_To_PDF4(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF4(PdfPTable tableLayout)
        {



            PdfPTableHeader tableHeader = new PdfPTableHeader();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);
            float[] headers = { 25, 24, 45, 30, 30, 30, 25, 25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 95; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;

            //Add Title to the PDF file at the top  

            //List < Employee > UserLog = _context.UserLog.ToList < Employee > (); 
            //List<SelectListItem> billers = Session["bilers"] as List<SelectListItem>;
            List<CustomerTransferReportViewModel> req_res_data_biller = new List<CustomerTransferReportViewModel>();
            req_res_data_biller = (List<CustomerTransferReportViewModel>)Session["billersreport"];

            DateTime dTime = DateTime.Now;

            //paragraphs
            Paragraph Title = new Paragraph("NBE - Control Panel",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customer Report For " + "bilers".ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Empty = new Paragraph("Empty",
            new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            //Chunk c = new Chunk("Total of Customers Registered : " + Session["totalbillercustomer"].ToString(),
            //    new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            //Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 2,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 6,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(Empty))
            {
                Colspan = 8,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            //Add header 

            AddCellToHeader4(tableLayout, "Biller id");
            AddCellToHeader4(tableLayout, "Transaction date");
            AddCellToHeader4(tableLayout, "Biller name");
            AddCellToHeader4(tableLayout, "Customer name");
            AddCellToHeader4(tableLayout, "Voucher");
            AddCellToHeader4(tableLayout, "Bill amount");
            AddCellToHeader4(tableLayout, "Trace number");
            AddCellToHeader4(tableLayout, "Biller response");
            //AddCellToHeader(tableLayout, "Bank_response");


            ////Add body  

            foreach (var emp in req_res_data_biller)
            {

                AddCellToBody4(tableLayout, emp.bbl_id.ToString());
                AddCellToBody4(tableLayout, emp.bbl_trandate.ToString());
                AddCellToBody4(tableLayout, emp.bil_name.ToString());
                AddCellToBody4(tableLayout, emp.bbl_customername.ToString());
                AddCellToBody4(tableLayout, emp.bbl_billervoucher.ToString());
                AddCellToBody4(tableLayout, emp.bbl_billamount.ToString());
                AddCellToBody4(tableLayout, emp.bbl_sys_traceno.ToString());
                AddCellToBody4(tableLayout, emp.bbl_response.ToString());
                //AddCellToBody(tableLayout, emp.Bank_response.ToString());

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 20,

                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT

            });
            //tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            //{
            //    Colspan = 4,
            //    PaddingLeft = 60,
            //    Rowspan = 3,
            //    Border = 1,
            //    PaddingTop = 5,
            //    BackgroundColor = new BaseColor(67, 160, 106),
            //    PaddingBottom = 5,
            //    HorizontalAlignment = Element.ALIGN_CENTER
            //});

            return tableLayout;
        }

        //Add Content
        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {



            PdfPTableHeader tableHeader = new PdfPTableHeader();

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);
            float[] headers = { 40, 20, 40, 20 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 95; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;

            //Add Title to the PDF file at the top  

            //List < Employee > UserLog = _context.UserLog.ToList < Employee > ();  
            List<Custreport> UserLog = new List<Custreport>();
            UserLog = (List<Custreport>)Session["BranchUsers"];

            DateTime dTime = DateTime.Now;

            //paragraphs
            Paragraph Title = new Paragraph("National Bank of Egept",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customer Report For " + Session["Branchname"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("Total of Customers Registered : " + Session["totalcustomer"].ToString(),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 2,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 2,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            ////Add header 

            AddCellToHeader(tableLayout, "Name");
            AddCellToHeader(tableLayout, "Status");

            AddCellToHeader(tableLayout, "Branch");
            AddCellToHeader(tableLayout, "Account");

            ////Add body  

            foreach (var emp in UserLog)
            {

                AddCellToBody(tableLayout, emp.CustomerName.ToString());
                AddCellToBody(tableLayout, emp.CustStatus.ToString());

                AddCellToBody(tableLayout, emp.Branch.ToString());
                AddCellToBody(tableLayout, emp.AccountNumber.ToString());

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 20,

                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_LEFT

            });
            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        //Header Cells:
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(54, 54, 77))))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                Border = Rectangle.BOX,
                BorderWidth = 1,
                BorderWidthLeft = 0,
                BorderWidthRight = 0,
                BorderWidthTop = 0,

                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);

            const string regex_match_arabic_hebrew = @"[\u0600-\u06FF\u0590-\u05FF]+";
            if (Regex.IsMatch(cellText, regex_match_arabic_hebrew, RegexOptions.IgnoreCase))
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
            else
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,

                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
        }

        private static void AddCellToHeader4(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(67, 160, 106))))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                Border = Rectangle.BOX,
                BorderWidth = 1,
                BorderWidthLeft = 0,
                BorderWidthRight = 0,
                BorderWidthTop = 0,

                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody4(PdfPTable tableLayout, string cellText)
        {

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);

            const string regex_match_arabic_hebrew = @"[\u0600-\u06FF\u0590-\u05FF]+";
            if (Regex.IsMatch(cellText, regex_match_arabic_hebrew, RegexOptions.IgnoreCase))
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
            else
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,

                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
        }

        public ActionResult OverviewReport()
        {
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "All", Value = "All" });
            list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });

            model.transactions_names = list;

            List<CustomerTransferReportViewModel> accumulativereport = ds.TotalTransactionsAmountsPerBranch("000");
            Session["accumulativereport"] = accumulativereport;

            List<CustomerTransferReportViewModel> transactionperbranch = ds.GetTransactionPerBranch("All");
            Session["transactionperbranch"] = transactionperbranch;

            return View(model);
        }

        public ActionResult BillersReport()
        {
            ////List<CustomerTransferReportViewModel> billerstransactions = new List<CustomerTransferReportViewModel>();
            ////billerstransactions = ds.GetBillersReport();
            ////Session["billersreport"] = billerstransactions;
            //List<req_res_model> req_res_data = new List<req_res_model>();
            //dynamic requestdata = null, responsedata = null;
            //req_res_data = ds.getreq_res_log();
            //List<SelectListItem> billers = new List<SelectListItem>();
            //billers.Add(new SelectListItem
            //{
            //    Text = "All",
            //    Value = "All"
            //});
            //billers.Add(new SelectListItem
            //{
            //    Text = "NMSF",
            //    Value = "NMSF"
            //});
            //billers.Add(new SelectListItem
            //{
            //    Text = "EPORTS",
            //    Value = "EPORTS"
            //});
            //Session["bilers"] = billers;

            //List<CustomerTransferReportViewModel> billersreport = new List<CustomerTransferReportViewModel>();
            //foreach (req_res_model transaction in req_res_data)
            //{
            //    //depacking json request and response
            //    if (transaction.Request_Data != "" && transaction.Request_Data != null)
            //    {
            //        requestdata = JObject.Parse(transaction.Request_Data);
            //    }
            //    if (transaction.Response_Data != "" && transaction.Response_Data != null)
            //    {
            //        responsedata = JObject.Parse(transaction.Response_Data);
            //    }
            //    if (requestdata.PayOrgID != null && requestdata.PayCustomerCode != null)
            //    {
            //        string biller_id = "N/A", biller_name = "N/A", bill_voucher = "N/A", bill_amount = "N/A", customername = "N/A", bankresponse = "N/A", reversalstatus = "0", sys_traceno = "N/A", billerresponse = "N/A";
            //        if (requestdata.PayOrgID == "2")
            //        {
            //            biller_id = "8";
            //            biller_name = "NMSF";
            //        }
            //        if (responsedata != null && responsedata.PaymentVoucherNo != null)
            //        {
            //            bill_voucher = responsedata.PaymentVoucherNo;
            //        }
            //        if (responsedata != null && responsedata.RequiredAmount != null)
            //        {
            //            bill_amount = responsedata.RequiredAmount;
            //        }
            //        if (responsedata != null && responsedata.TranNo != null)
            //        {
            //            sys_traceno = responsedata.TranNo;
            //        }
            //        if (responsedata != null && responsedata.PayCustomerName != null)
            //        {
            //            customername = responsedata.PayCustomerName;
            //        }
            //        if (responsedata != null && responsedata.OrderStatus != null)
            //        {
            //            billerresponse = responsedata.OrderStatus;
            //        }

            //        billersreport.Add(new CustomerTransferReportViewModel
            //        {
            //            bbl_id = biller_id,
            //            bbl_trandate = transaction.RESPONSE_DATE,
            //            bil_name = biller_name,
            //            bbl_billervoucher = bill_voucher,
            //            bbl_billamount = bill_amount,
            //            bbl_sys_traceno = sys_traceno,
            //            bbl_reversalstatus = reversalstatus,
            //            bbl_customername = customername,
            //            bbl_response = billerresponse
            //        });
            //    }
            //}
            //Session["billersreport"] = billersreport;
            //return View();



            List<req_res_model> req_res_data = new List<req_res_model>();
            req_res_model model = new req_res_model();
            dynamic requestdata = null, responsedata = null;

            req_res_data = ds.getreq_res_log();
            Session["billersreport"] = req_res_data;
            List<SelectListItem> billers = new List<SelectListItem>();

            billers = ds.billers_statuses();


            //});
            Session["bilers"] = billers;

            return View();

        }

        public JsonResult FilteredBillersReport(string fromdate, string todate, string biller)
        {


            //string formatedFromDate = DateTime.Parse(fromdate).ToString();
            //string formatedtodate = DateTime.Parse(todate).ToString();
            //string[] readyfromdate = formatedFromDate.Split(' ');
            //string[] readytodate = formatedtodate.Split(' ');
            //List<req_res_model> req_res_data = new List<req_res_model>();
            //dynamic requestdata = null, responsedata = null;
            //req_res_data = ds.getfilteredreq_res_log(fromdate, todate, biller);
            //List<CustomerTransferReportViewModel> billersreport = new List<CustomerTransferReportViewModel>();

            //foreach (req_res_model transaction in req_res_data)
            //{
            //    //depacking json request and response
            //    if (transaction.Request_Data != null && transaction.Request_Data != "")
            //    {
            //        requestdata = JObject.Parse(transaction.Request_Data);
            //    }
            //    if (transaction.Response_Data != null && transaction.Response_Data != "")
            //    {
            //        responsedata = JObject.Parse(transaction.Response_Data);
            //    }
            //    if (requestdata != null && requestdata.PayOrgID != null && requestdata.PayCustomerCode != null)
            //    {
            //        string biller_id = "N/A", biller_name = "N/A", bill_voucher = "N/A", bill_amount = "N/A", customername = "N/A", bankresponse = "N/A", reversalstatus = "0", sys_traceno = "N/A", billerresponse = "N/A";
            //        if (requestdata.PayOrgID == "2")
            //        {
            //            biller_id = "8";
            //            biller_name = "NMSF";
            //        }
            //        if (responsedata != null && responsedata.PaymentVoucherNo != null)
            //        {
            //            bill_voucher = responsedata.PaymentVoucherNo;
            //        }
            //        if (responsedata != null && responsedata.RequiredAmount != null)
            //        {
            //            bill_amount = responsedata.RequiredAmount;
            //        }
            //        if (responsedata != null && responsedata.TranNo != null)
            //        {
            //            sys_traceno = responsedata.TranNo;
            //        }
            //        if (responsedata != null && responsedata.PayCustomerName != null)
            //        {
            //            customername = responsedata.PayCustomerName;
            //        }
            //        if (responsedata != null && responsedata.OrderStatus != null)
            //        {
            //            billerresponse = responsedata.OrderStatus;
            //        }

            //        billersreport.Add(new CustomerTransferReportViewModel
            //        {
            //            bbl_id = biller_id,
            //            bbl_trandate = transaction.RESPONSE_DATE,
            //            bil_name = biller_name,
            //            bbl_billervoucher = bill_voucher,
            //            bbl_billamount = bill_amount,
            //            bbl_sys_traceno = sys_traceno,
            //            bbl_reversalstatus = reversalstatus,
            //            bbl_customername = customername,
            //            bbl_response = billerresponse
            //        });
            //    }

            //}
            //Session["billersreport"] = billersreport;
            //JsonResult data = Json(new { data = billersreport }, JsonRequestBehavior.AllowGet);
            //data.MaxJsonLength = int.MaxValue;
            //return data;



            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            List<req_res_model> req_res_data = new List<req_res_model>();


            req_res_data = ds.getfilteredreq_res_log(fromdate, todate, biller);
            List<req_res_model> billersreport = new List<req_res_model>();

            billersreport = req_res_data;
            Session["billersreport"] = billersreport;
            JsonResult data = Json(new { data = billersreport }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }

        public ActionResult Accounttoaccountreport()
        {
            Custreport model = new Custreport();
            model.Branches = ds.PopulateBranchs();

            List<SelectListItem> transaction_names_list = new List<SelectListItem>();
            transaction_names_list.Add(new SelectListItem { Text = "All", Value = "All" });
            transaction_names_list.Add(new SelectListItem { Text = "AccountToCardTransfer", Value = "AccountToCardTransfer" });
            transaction_names_list.Add(new SelectListItem { Text = "To Bank Customer Transfer", Value = "To Bank Customer Transfer" });
            List<SelectListItem> transactions_statuses = new List<SelectListItem>();
            transactions_statuses.Add(new SelectListItem { Text = "All", Value = "All" });
            transactions_statuses.Add(new SelectListItem { Text = "Successful", Value = "Secussfully" });
            transactions_statuses.Add(new SelectListItem { Text = "Failed", Value = "Failed" });

            model.transactions_names = transaction_names_list;
            model.transactions_statuses = transactions_statuses;
           


            List<CustomerTransferReportViewModel> accounttranfertransactions = new List<CustomerTransferReportViewModel>();

            Session["accounttranfertransactions"] = accounttranfertransactions;

            return View(model);
        }

        public JsonResult FilterAccountToAccountReport(string branch_code, string status, string fromdate, string todate , int pageNumber)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            


            List<CustomerTransferReportViewModel> accounttranfertransactions = new List<CustomerTransferReportViewModel>();
            List<CustomerTransferReportViewModel> Printaccounttranfertransactions = new List<CustomerTransferReportViewModel>();
            accounttranfertransactions = ds.FilteredAccountToAccountTransactions(branch_code, status, readyfromdate[0], readytodate[0],pageNumber);
            Printaccounttranfertransactions = ds.FilteredAccountToAccountPrintTransactions(branch_code, status, readyfromdate[0], readytodate[0]);
            foreach (CustomerTransferReportViewModel transaction in accounttranfertransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                // transaction.TranReqAmount = requestdata.tranamount;
                transaction.TranFromAccount = requestdata.accountfrom;
                transaction.TranToAccount = requestdata.accountto;
                transaction.alsocustomername = requestdata.FromAccountName;
                transaction.CustomerName = requestdata.recipientName;
                transaction.ResponseStatus = responsedata.status;
                if (responsedata.status != "")
                {
                    if (transaction.ResponseStatus.ToString() != "00")
                    {
                        string word = transaction.ResponseStatus;
                        string[] words = word.Split(':');
                        transaction.FT = words[1];
                    }
                }
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }

            foreach (CustomerTransferReportViewModel transaction in Printaccounttranfertransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                //transaction.TranReqAmount = requestdata.tranamount;
                transaction.TranFromAccount = requestdata.accountfrom;
                transaction.TranToAccount = requestdata.accountto;
                transaction.alsocustomername = requestdata.FromAccountName;
                transaction.CustomerName = requestdata.recipientName;
                transaction.ResponseStatus = responsedata.status;
                if (responsedata.status != "")
                {
                    if (transaction.ResponseStatus.ToString() != "00")
                    {
                        string word = transaction.ResponseStatus;
                        string[] words = word.Split(':');
                        transaction.FT = words[1];
                    }
                }
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            Session["accounttranfertransactions"] = accounttranfertransactions;
            Session["Printaccounttranfertransactions"] = Printaccounttranfertransactions;
            JsonResult data = Json(new { data = accounttranfertransactions }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }


       

        public ActionResult CustomersByAdmin()
        {
            CustomerReportModel customer = new CustomerReportModel();
            customer.admins = ds.populateadmins();

            List<CustomerReportModel> customers = new List<CustomerReportModel>();
            customers = ds.GetCustomersByAdmin("All", "All", "All" , 0);
            Session["customersbyadmin"] = customers;

            return View(customer);
        }

        public JsonResult FilteredCustomersByAdmin(string admin, string fromdate, string todate , int PageNumber)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString().Substring(0, 9);
            string formatedtodate = DateTime.Parse(todate).ToString().Substring(0, 9);

            List<CustomerReportModel> customers = new List<CustomerReportModel>();
            List<CustomerReportModel> Printcustomers = new List<CustomerReportModel>();
            customers = ds.GetCustomersByAdmin(admin, formatedFromDate, formatedtodate ,PageNumber);
            Printcustomers = ds.PrintGetCustomersByAdmin(admin, formatedFromDate, formatedtodate);
            Session["customersbyadmin"] = customers;
            Session["Printcustomersbyadmin"] = Printcustomers; 
           JsonResult data = Json(new { data = customers }, JsonRequestBehavior.AllowGet);
            return data;
        }

       

        public JsonResult FilteredOverviewReport(string branch_code)
        {

            List<CustomerTransferReportViewModel> accumulativereport = ds.TotalTransactionsAmountsPerBranch(branch_code);
            Session["accumulativereport"] = accumulativereport;
            JsonResult data = Json(new { data = accumulativereport }, JsonRequestBehavior.AllowGet);
            return data;
        }

        public JsonResult FilteredDateOverviewReport(string branch_code, string fromdate, string todate)
        {

            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            List<CustomerTransferReportViewModel> accumulativereport = ds.TotalTransactionsAmountsPerBranch(branch_code, readyfromdate[0], readytodate[0]);
            Session["accumulativereport"] = accumulativereport;
            JsonResult data = Json(new { data = accumulativereport }, JsonRequestBehavior.AllowGet);
            return data;

        }

        public JsonResult FilterTransactionsPerBranches(string transaction_name)
        {


            List<CustomerTransferReportViewModel> accumulativereport = ds.GetTransactionPerBranch(transaction_name);


            Session["transactionperbranch"] = accumulativereport;
            JsonResult data = Json(new { data = accumulativereport }, JsonRequestBehavior.AllowGet);
            return data;
        }

        public JsonResult FilterTransactionsDatePerBranches(string transaction_name, string fromdate, string todate)
        {

            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            List<CustomerTransferReportViewModel> accumulativereport = ds.GetTransactionPerBranch(transaction_name, readyfromdate[0], readytodate[0]);


            Session["transactionperbranch"] = accumulativereport;
            JsonResult data = Json(new { data = accumulativereport }, JsonRequestBehavior.AllowGet);
            return data;
        }

        public JsonResult FilterCreditAPIReport(string branch_code, string status, string fromdate, string todate)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');

            List<CustomerTransferReportViewModel> creditapitransactions = ds.GetCreditAPITransaction(branch_code, status, readyfromdate[0], readytodate[0]);
            foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.PAN = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.CustomerName = requestdata.customerName;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words = word.Split(':');
                transaction.FT = words[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }
            Session["creditapitransactions"] = creditapitransactions;
            JsonResult data = Json(new { data = creditapitransactions }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }


        public JsonResult FilterUserRegReport(string branch_code, string status, string fromdate, string todate)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            //////
            ///

            ArrayList Linked_Acc_Counts = new ArrayList(); ;
            ArrayList Cust_Type = new ArrayList();
            ArrayList Branch_Name = new ArrayList();
            ArrayList Cust_Count = new ArrayList();
            string accessToken = Session["accesstoken"].ToString();

            string apiresponse = Connecttocore.userInfo(branch_code, fromdate, todate, accessToken);
            JObject customerInfo = new JObject();
            customerInfo = JObject.Parse(apiresponse);
            //Linked_Acc_Counts = customerInfo.GetValue("Accounts_List").ToString();
            //JObject customerInfo2 = new JObject();
            ////customerInfo2 = JObject.Parse(apiresponse.);

            int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
            if (responseCode == 0)
            {

                string AccList = customerInfo.GetValue("Accounts_List").ToString();
                JArray AccArray = JArray.Parse(AccList);

                List<TwoDObj> RegAccList = new List<TwoDObj>();

                foreach (JObject o in AccArray.Children<JObject>())
                {
                    //foreach (JProperty p in o.Properties())
                    //{
                    //    string name = p.Name;
                    //    string value = (string)p.Value;
                    //    Console.WriteLine(name + " -- " + value);
                    //}
                    RegAccList.Add(new TwoDObj(o.GetValue("Branch_Name").ToString(), o.GetValue("Customers_Count").ToString()));
                    //Cust_Count.Add(o.GetValue("Customers_Count").ToString());


                }
                //////Linked_Acc_Counts = customerInfo.GetValue("Linked_Acc_Counts").ToString();
                //////Cust_Type = customerInfo.GetValue("Cust_Type").ToString();
                //custAddress = customerInfo.GetValue("customerAddress").ToString();
                //customerbranchcode = customerInfo.GetValue("customerBranch").ToString();
                //customeraccounttypecode = customerInfo.GetValue("customerAccountType").ToString();
                //customerbranch = ds.getbranchnameenglish(customerbranchcode);
                //customeraccounttype = ds.getaccounttype(customeraccounttypecode);
                //Session["custID"] = custID;
                //////Session["Linked_Acc_Counts"] = Linked_Acc_Counts;
                //////Session["Cust_Type"] = Cust_Type;
                //Session["Linked_Acc_Counts"] = Linked_Acc_Counts;
                Session["Cust_Count"] = RegAccList;

                //List<CustomerTransferReportViewModel> creditapitransactions = new List<CustomerTransferReportViewModel>();
                //creditapitransactions.Add(new CustomerTransferReportViewModel
                //{


                //});


                //Session["branchcode"] = customerbranchcode;
                //Session["accounttypecode"] = customeraccounttypecode;
                //Session["branch"] = customerbranch;
                //Session["accounttype"] = customeraccounttype;

                //ViewBag.custname = custname;
                //ViewBag.custbranch = customerbranch;
                //ViewBag.custaccounttype = customeraccounttype;

                //return custname;
            }
            else
            {
                ModelState.AddModelError("", "Please Check Customer Information");
            }








            //List<CustomerTransferReportViewModel> creditapitransactions = ds.GetUserReg(branch_code, status, readyfromdate[0], readytodate[0]);
            //int i = 0;
            //foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            //{
            //    i++;

            //    dynamic requestdata = JObject.Parse(transaction.TranFullReq);
            //    dynamic responsedata = JObject.Parse(transaction.TranFullResp);
            //    transaction.TranReqAmount = requestdata.tranamount;
            //    transaction.PAN = requestdata.PAN;
            //    transaction.TranFromAccount = requestdata.Fromaccount;
            //transaction.num_count = Cust_Count[i];
            //    transaction.ResponseStatus = responsedata.responseStatus;
            //    transaction.RRN = responsedata.RRN;
            //    string word = responsedata.status;
            //    string[] words = word.Split(':');
            //    transaction.FT = words[1];
            //    string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
            //    transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            //}

            Session["creditapitransactions"] = Session["Cust_Count"];
            JsonResult data = Json(new { data = Session["creditapitransactions"] }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }



        public JsonResult FilterCustTranReport(string branch_code, string status, String UserID, string fromdate, string todate)
        {
            string formatedFromDate = DateTime.Parse(fromdate).ToString();
            string formatedtodate = DateTime.Parse(todate).ToString();
            string[] readyfromdate = formatedFromDate.Split(' ');
            string[] readytodate = formatedtodate.Split(' ');
            //////
            ///

            ArrayList Linked_Acc_Counts = new ArrayList(); ;
            ArrayList Cust_Type = new ArrayList();
            ArrayList Branch_Name = new ArrayList();
            ArrayList Cust_Count = new ArrayList();
            string accessToken = Session["accesstoken"].ToString();

            string apiresponse = Connecttocore.CustTranInfo(branch_code, status, UserID, fromdate, todate, accessToken);
            JObject customerInfo = new JObject();
            customerInfo = JObject.Parse(apiresponse);
            //Linked_Acc_Counts = customerInfo.GetValue("Accounts_List").ToString();
            //JObject customerInfo2 = new JObject();
            ////customerInfo2 = JObject.Parse(apiresponse.);

            int responseCode = int.Parse(customerInfo.GetValue("Response_Code").ToString());
            if (responseCode == 0)
            {

                string RegTransList = customerInfo.GetValue("Trans_List").ToString();
                JArray AccArray = JArray.Parse(RegTransList);

                List<TranDetails> RegTranList = new List<TranDetails>();

                foreach (JObject o in AccArray.Children<JObject>())
                {
                    //foreach (JProperty p in o.Properties())
                    //{
                    //    string name = p.Name;
                    //    string value = (string)p.Value;
                    //    Console.WriteLine(name + " -- " + value);
                    //}
                    RegTranList.Add(new TranDetails(o.GetValue("Tran_DateTime").ToString(), o.GetValue("User_ID").ToString(), o.GetValue("From_Account_Info").ToString(), o.GetValue("Branch_Name").ToString(), o.GetValue("To_Account_Info").ToString(), o.GetValue("Branch_Name").ToString(), o.GetValue("Amount").ToString(), o.GetValue("Reference_No").ToString(), o.GetValue("Tran_Status").ToString()));
                    //Cust_Count.Add(o.GetValue("Customers_Count").ToString());


                }
                //////Linked_Acc_Counts = customerInfo.GetValue("Linked_Acc_Counts").ToString();
                //////Cust_Type = customerInfo.GetValue("Cust_Type").ToString();
                //custAddress = customerInfo.GetValue("customerAddress").ToString();
                //customerbranchcode = customerInfo.GetValue("customerBranch").ToString();
                //customeraccounttypecode = customerInfo.GetValue("customerAccountType").ToString();
                //customerbranch = ds.getbranchnameenglish(customerbranchcode);
                //customeraccounttype = ds.getaccounttype(customeraccounttypecode);
                //Session["custID"] = custID;
                //////Session["Linked_Acc_Counts"] = Linked_Acc_Counts;
                //////Session["Cust_Type"] = Cust_Type;
                //Session["Linked_Acc_Counts"] = Linked_Acc_Counts;
                Session["Cust_Count"] = RegTranList;

                //List<CustomerTransferReportViewModel> creditapitransactions = new List<CustomerTransferReportViewModel>();
                //creditapitransactions.Add(new CustomerTransferReportViewModel
                //{


                //});


                //Session["branchcode"] = customerbranchcode;
                //Session["accounttypecode"] = customeraccounttypecode;
                //Session["branch"] = customerbranch;
                //Session["accounttype"] = customeraccounttype;

                //ViewBag.custname = custname;
                //ViewBag.custbranch = customerbranch;
                //ViewBag.custaccounttype = customeraccounttype;

                //return custname;
            }
            else
            {
                ModelState.AddModelError("", "Please Check Customer Information");
            }








            //List<CustomerTransferReportViewModel> creditapitransactions = ds.GetUserReg(branch_code, status, readyfromdate[0], readytodate[0]);
            //int i = 0;
            //foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            //{
            //    i++;

            //    dynamic requestdata = JObject.Parse(transaction.TranFullReq);
            //    dynamic responsedata = JObject.Parse(transaction.TranFullResp);
            //    transaction.TranReqAmount = requestdata.tranamount;
            //    transaction.PAN = requestdata.PAN;
            //    transaction.TranFromAccount = requestdata.Fromaccount;
            //transaction.num_count = Cust_Count[i];
            //    transaction.ResponseStatus = responsedata.responseStatus;
            //    transaction.RRN = responsedata.RRN;
            //    string word = responsedata.status;
            //    string[] words = word.Split(':');
            //    transaction.FT = words[1];
            //    string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
            //    transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            //}

            Session["creditapitransactions"] = Session["Cust_Count"];
            JsonResult data = Json(new { data = Session["creditapitransactions"] }, JsonRequestBehavior.AllowGet);
            data.MaxJsonLength = int.MaxValue;
            return data;
        }



        //public JsonResult FilterAccounttoaccountReport(string branch_code, string status)
        //{
        //    List<CustomerTransferReportViewModel> accounttranfertransactions = ds.GetAccountTransferTransactions(branch_code, status);
        //    foreach (CustomerTransferReportViewModel transaction in accounttranfertransactions)
        //    {
        //        dynamic requestdata = JObject.Parse(transaction.TranFullReq);
        //        dynamic responsedata = JObject.Parse(transaction.TranFullResp);
        //        //transaction.TranReqAmount = requestdata.tranamount;
        //        transaction.TranFromAccount = requestdata.accountfrom;
        //        transaction.TranToAccount = requestdata.accountto;
        //        transaction.Customername = requestdata.FromAccountName;
        //        transaction.CustomerName = requestdata.recipientName;
        //        transaction.ResponseStatus = responsedata.status;
        //        if (responsedata.status != "")
        //        {
        //            string word = transaction.ResponseStatus;
        //            string[] words = word.Split(':');
        //            transaction.FT = words[1];
        //        }
        //        string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
        //        transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
        //    }

        //    Session["accounttranfertransactions"] = accounttranfertransactions;
        //    JsonResult data = Json(new { data = accounttranfertransactions }, JsonRequestBehavior.AllowGet);
        //    return data;
        //}

        [HttpPost]
        public FileResult saveaccounttoaccountreport(Custreport model)
        {
            string transtatus = "";
            string branchname = "";
            Session["transtatus"] = transtatus;
            if (model.BranchCode != null)
            {
                branchname = ds.getbranchnameenglish(model.BranchCode);
                Session["bname"] = branchname;
            }
            else
            {
                branchname = "All";
                Session["bname"] = branchname;
            }
            if (model.CategoryCode != null)
            {
                transtatus = model.CategoryCode;
                Session["transtatus"] = transtatus;
            }
            else
            {
                transtatus = "All";
                Session["transtatus"] = transtatus;
            }

            Session["Branchname"] = branchname;
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Account To Account Report For " + branchname + " With Status " + transtatus + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(7);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_Account_To_Account_PDF(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_Account_To_Account_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 30, 15, 10, 10, 10, 15, 10 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;

            DateTime dTime = DateTime.Now;

            //paragraphs
            //paragraphs
            Paragraph Title = new Paragraph("NBE - ControL Panel",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Account To Account Report For " + Session["Branchname"].ToString() + " With Status " + Session["transtatus"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("National Bank of Egept - Account To Account Report",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 7,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 7,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 4,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 3,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });
             


            ////Add header
            AddCellToHeaderRefined(tableLayout, "From Account");
            AddCellToHeaderRefined(tableLayout, "To Account");
            AddCellToHeaderRefined(tableLayout, "Amount");
            AddCellToHeaderRefined(tableLayout, "Recipient Name");
            AddCellToHeaderRefined(tableLayout, "Status");
            AddCellToHeaderRefined(tableLayout, "FT");
            AddCellToHeaderRefined(tableLayout, "Date");

            List<CustomerTransferReportViewModel> creditapireport = new List<CustomerTransferReportViewModel>();
            creditapireport = (List<CustomerTransferReportViewModel>)Session["Printaccounttranfertransactions"];

            foreach (var report in creditapireport)
            {
               

                //AddCellToBodyRefined(tableLayout, report.CustomerName);
                AddCellToBodyRefined(tableLayout, report.alsocustomername); 
                AddCellToBodyRefined(tableLayout, report.TranToAccount);
                AddCellToBodyRefined(tableLayout, report.TranReqAmount);
                AddCellToBodyRefined(tableLayout, report.CustomerName);
                AddCellToBodyRefined(tableLayout, report.TranStatus);
                AddCellToBodyRefined(tableLayout, report.FT);
                AddCellToBodyRefined(tableLayout, report.TranDate);

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                //BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {  

                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        [HttpPost]
        public FileResult savecreditapireport(Custreport model)
        {
            string transtatus = "";
            string branchname = "";
            Session["transtatus"] = transtatus;
            if (model.BranchCode != null)
            {
                branchname = ds.getbranchnameenglish(model.BranchCode);
                Session["bname"] = branchname;
            }
            else
            {
                branchname = "All";
                Session["bname"] = branchname;
            }
            if (model.CategoryCode != null)
            {
                transtatus = model.CategoryCode;
                Session["transtatus"] = transtatus;
            }
            else
            {
                transtatus = "All";
                Session["transtatus"] = transtatus;
            }

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Account To Card Report For " + branchname + " With Status " + transtatus + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(7);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table
            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);
            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();
            //Add Content to PDF 
            doc.Add(Add_Content_To_Account_To_Card_PDF(tableLayout));
            // Closing the document
            doc.Close();
            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);
        }

        protected PdfPTable Add_Content_To_Account_To_Card_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 15, 15, 15, 10, 15, 20, 10 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;

            DateTime dTime = DateTime.Now;

            //paragraphs
            //paragraphs
            Paragraph Title = new Paragraph("NBE - Control Panel",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Account To Card Report For " + Session["bname"].ToString() + " With Status " + Session["transtatus"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("National Bank of Egept - Account To Card Report",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 7,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 7,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 4,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 3,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });


            ////Add header
            AddCellToHeaderRefined(tableLayout, "From Account");
            AddCellToHeaderRefined(tableLayout, "PAN");
            AddCellToHeaderRefined(tableLayout, "Amount");
            AddCellToHeaderRefined(tableLayout, "Status");
            AddCellToHeaderRefined(tableLayout, "RRN");
            AddCellToHeaderRefined(tableLayout, "FT");
            AddCellToHeaderRefined(tableLayout, "Date");

            List<CustomerTransferReportViewModel> creditapireport = new List<CustomerTransferReportViewModel>();
            creditapireport = (List<CustomerTransferReportViewModel>)Session["creditapitransactions"];

            foreach (var report in creditapireport)
            {
                AddCellToBodyRefined(tableLayout, report.TranFromAccount.ToString());
                AddCellToBodyRefined(tableLayout, report.PAN.ToString());
                AddCellToBodyRefined(tableLayout, report.TranReqAmount.ToString());
                AddCellToBodyRefined(tableLayout, report.ResponseStatus.ToString());
                AddCellToBodyRefined(tableLayout, report.RRN);
                AddCellToBodyRefined(tableLayout, report.FT.ToString());
                AddCellToBodyRefined(tableLayout, report.TranDate.ToString());

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                //BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 7,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        [HttpPost]
        public FileResult savecustomersbyadminsreport(CustomerReportModel model)
        {

         
            string branchname = "";

            if (branchname == "Admin")
            {
                branchname = "All Users";
                Session["Branchname"] = branchname;
            }

            if (model.BranchCode != null)
            {
                 branchname = model.BranchCode;
                
                Session["Branchname"] = branchname;
            }
            else
            {
                branchname = "All";
                Session["Branchname"] = branchname;
            }


            //
          //  string branchname = model.BranchCode;

           

            Session["Branchname"] = branchname;
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Registered Customers By " + branchname + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(8);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_Customer_By_Admins_PDF(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_Customer_By_Admins_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 20, 12, 10, 12, 10, 10, 16, 10 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;

            DateTime dTime = DateTime.Now;

            //paragraphs
            //paragraphs
            Paragraph Title = new Paragraph("FCB - NAS ALBAIT MOBILE",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Registered Customers By " + Session["Branchname"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("NAS ALBAIT MOBILE - Registered Customers By Users",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 8,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 8,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 4,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 4,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 8,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });


            ////Add header
            AddCellToHeaderRefined(tableLayout, "Customer Name");
            AddCellToHeaderRefined(tableLayout, "Customer Log");
            AddCellToHeaderRefined(tableLayout, "E-Mail");
            AddCellToHeaderRefined(tableLayout, "Mobile");
            AddCellToHeaderRefined(tableLayout, "Address");
            AddCellToHeaderRefined(tableLayout, "Status");
            AddCellToHeaderRefined(tableLayout, "Account Number");
            AddCellToHeaderRefined(tableLayout, "Create By");

            List<CustomerReportModel> Printcustomers = new List<CustomerReportModel>();
            Printcustomers = (List<CustomerReportModel>)Session["Printcustomersbyadmin"];

            foreach (var customer in Printcustomers)
            {

                AddCellToBodyRefined(tableLayout, customer.CustomerName.ToString());
                AddCellToBodyRefined(tableLayout, customer.CustomerLog.ToString());
                AddCellToBodyRefined(tableLayout, customer.Email.ToString());
                AddCellToBodyRefined(tableLayout, customer.mobile.ToString());
                AddCellToBodyRefined(tableLayout, customer.address.ToString());
                AddCellToBodyRefined(tableLayout, customer.CustStatus.ToString());
                AddCellToBodyRefined(tableLayout, customer.AccountNumber.ToString());
                AddCellToBodyRefined(tableLayout, customer.created_by.ToString());

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = 8,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                //BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 8,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        [HttpPost]
        public FileResult saveaccumulativereport(Custreport model)
        {

            string branchname = "";
            if (model.BranchCode != "000")
            {
                branchname = ds.getbranchnameenglish(model.BranchCode);
                Session["Branchname"] = branchname;
            }
            else
            {
                branchname = "All";
                Session["Branchname"] = branchname;
            }

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Accumulative Report For " + branchname + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(3);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_Accumulative_PDF(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_Accumulative_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 40, 30, 30 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;

            DateTime dTime = DateTime.Now;

            //paragraphs
            //paragraphs
            Paragraph Title = new Paragraph("FCB - NAS ALBAIT MOBILE",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Customer Report For " + Session["Branchname"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("NAS ALBAIT MOBILE - Accumulative Report",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 2,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 2,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });


            ////Add header
            AddCellToHeaderRefined(tableLayout, "Service");
            AddCellToHeaderRefined(tableLayout, "Transactions Count");
            AddCellToHeaderRefined(tableLayout, "Accumulitive Amount");
            //AddCellToHeader(tableLayout, "Role");
            //AddCellToHeader(tableLayout, "Status");


            List<CustomerTransferReportViewModel> accumulativereport = new List<CustomerTransferReportViewModel>();
            accumulativereport = (List<CustomerTransferReportViewModel>)Session["accumulativereport"];

            foreach (var report in accumulativereport)
            {

                AddCellToBodyRefined(tableLayout, report.TranResult.ToString());
                AddCellToBodyRefined(tableLayout, report.CurrencyCode);
                AddCellToBodyRefined(tableLayout, report.TranReqAmount);
                //AddCellToBody(tableLayout, user.rolename);
                //AddCellToBody(tableLayout, user.user_status);

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                //BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        [HttpPost]
        public FileResult savetransactionperbranchreport(Custreport model)
        {
            string service = "";
            if (model.BranchCode != "All")
            {
                service = model.BranchCode;
                Session["service"] = service;
            }
            else
            {
                service = "All";
                Session["service"] = service;
            }

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Transaction Per Branch Report For " + model.BranchCode + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(3);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_Transaction_Per_Branch_PDF(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_Transaction_Per_Branch_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 40, 30, 30 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;

            DateTime dTime = DateTime.Now;

            //paragraphs
            //paragraphs
            Paragraph Title = new Paragraph("FCB - NAS ALBAIT MOBILE",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Transaction Per Branch Report For " + Session["service"].ToString(),
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Chunk c = new Chunk("NAS ALBAIT MOBILE - Transaction Per Branch Report",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Title2))
            {
                Colspan = 5,
                PaddingLeft = 30,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            {
                Colspan = 2,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_LEFT
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            {
                Colspan = 2,
                PaddingLeft = 10,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 10,
                PaddingTop = 5,

                BackgroundColor = new BaseColor(67, 160, 106),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });


            ////Add header
            AddCellToHeaderRefined(tableLayout, "Branch Name");
            AddCellToHeaderRefined(tableLayout, "Transactions Count");
            AddCellToHeaderRefined(tableLayout, "Accumulative Amount");
            //AddCellToHeader(tableLayout, "Role");
            //AddCellToHeader(tableLayout, "Status");


            List<CustomerTransferReportViewModel> accumulativereport = new List<CustomerTransferReportViewModel>();
            accumulativereport = (List<CustomerTransferReportViewModel>)Session["transactionperbranch"];

            foreach (var report in accumulativereport)
            {

                AddCellToBodyRefined(tableLayout, report.TranResult.ToString());
                AddCellToBodyRefined(tableLayout, report.CurrencyCode);
                AddCellToBodyRefined(tableLayout, report.TranReqAmount);
                //AddCellToBody(tableLayout, user.rolename);
                //AddCellToBody(tableLayout, user.user_status);

            }

            tableLayout.AddCell(new PdfPCell(new Phrase(" "))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                //BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            tableLayout.AddCell(new PdfPCell(new Phrase(Total))
            {
                Colspan = 4,
                PaddingLeft = 60,
                Rowspan = 3,
                Border = 1,
                Top = 5,
                PaddingTop = 5,
                BackgroundColor = new BaseColor(67, 160, 106),
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeaderRefined(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(67, 184, 120) });
        }

        // Method to add single cell to the body
        private static void AddCellToBodyRefined(PdfPTable tableLayout, string cellText)
        {

            string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
            BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);

            const string regex_match_arabic_hebrew = @"[\u0600-\u06FF\u0590-\u05FF]+";
            if (Regex.IsMatch(cellText, regex_match_arabic_hebrew, RegexOptions.IgnoreCase))
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText,
                    new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK)))
                {
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 5,
                    Border = Rectangle.BOX,
                    BorderWidth = 1,
                    BorderWidthLeft = 0,
                    BorderWidthRight = 0,
                    BorderWidthTop = 0,
                    BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
                });
            }
            else
            {
                tableLayout.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });

            }
        }

    }
}