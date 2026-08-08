using AljazeeraCPanel.Context;
using AljazeeraCPanel.Models;
using Newtonsoft.Json.Linq;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Services.Description;

namespace AljazeeraCPanel.Controllers
{
    public class CustomerAuthorizationController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /CustomerAuth/
        public ActionResult CustomerAuthorization()
        {
            //Response.AddHeader("Refresh", "5");
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["result"] != null)
            {
                ViewBag.SuccessMessage = Session["result"].ToString();
                Session["result"] = null;
            }

            if (Session["fail"] != null)
            {
                TempData["fail"] = Session["fail"].ToString();
                Session["fail"] = null;
            }

            //Session["bracode"] = "000";
            String branchcode = Session["user_branch"].ToString();

            List<CustomerAuthorization> customers = new List<CustomerAuthorization>();
            return View(customers);
        }

        public ActionResult CustomerAuthorizationrefresh()
        {
            if (Session["user_name"] == null)
            {
                RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                RedirectToAction("Login", "Login");
            }
            if (Session["result"] != null)
            {
                ViewBag.SuccessMessage = Session["result"].ToString();
                Session["result"] = null;
            }

            //Session["bracode"] = "000";
            String branchcode = Session["user_branch"].ToString();

            List<CustomerAuthorization> customer = new List<CustomerAuthorization>();
            customer = ds.PendingCustomer(branchcode);

            //var jsonresponse = new JavaScriptSerializer().Serialize(customer);
            //dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonresponse);

            JavaScriptSerializer js = new JavaScriptSerializer();
            var response = js.Serialize(customer);
            return Json(response);
        }

        public ActionResult CustomersTable()
       {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            String branchcode = Session["user_branch"].ToString();

            List<CustomerAuthorization> customers = new List<CustomerAuthorization>();
            try
            {
                string apirespone = Connecttocore.getUnauthorizedUsers(Session["accesstoken"].ToString());
                 JObject response = new JObject();
                response = JObject.Parse(apirespone);
                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    JArray unauthorizedcustomers = (JArray)response.GetValue("Customers_List");
                    foreach (JObject customer in unauthorizedcustomers)
                    {
                        customers.Add(new CustomerAuthorization
                        {
                            CustomerID = customer.GetValue("User_ID").ToString(),
                            Customername = customer.GetValue("Customer_Name_EN").ToString(),
                            RIM = customer.GetValue("RIM").ToString(),
                            type = customer.GetValue("Cust_Type").ToString(),
                            phonennumber = customer.GetValue("Phone_No").ToString(),
                            createdby = customer.GetValue("Created_By").ToString(),
                            stscode = customer.GetValue("User_Status").ToString(),
                            custsts = customer.GetValue("Cust_Status").ToString()
                        });
                    }
                }


               // return PartialView(customers);
            }
          
   catch (Exception ex)
            {
                //message = "Please Contact for Support";
                //Session["FailedMessage"] = message;
                ModelState.AddModelError("", "Something is missing Please Contact for Support" );
                //return RedirectToAction("Login", "Login");

            }
            return PartialView(customers);

        }

        public ActionResult Details(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            CustomerAuthorizationinfo model = new CustomerAuthorizationinfo();
            Session["user"] = id;
            List<CustomerAuthorizationinfo> customer = new List<CustomerAuthorizationinfo>();
            customer = ds.CustomerAuthorizationinfo(id.ToString());
            Session["customer"] = customer;
            foreach (var item in customer)
            {
                model.Branch = item.Branch;
                model.AccountType = item.AccountType;
                model.Customername = item.Customername;
                Session["customername"] = item.Customername;
                model.Currency = item.Currency;
                model.Customeraccount = item.Customeraccount;
                model.UserName = item.UserName;
                model.Address = item.Address;
                model.CustomerPhone = item.CustomerPhone;
                model.Email = item.Email;
                model.Profile = item.Profile;
                model.userid = item.userid;

            }
            model.authsts = "true";
            model.rjtsts = "false";
            Session["model"] = model;
            return View(model);
        }

        public ActionResult Authorize(string id , string sts)
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
            if (sts.Equals("UA"))
            {
                string apiresponse = Connecttocore.authroizeCustomer(id.ToString(), Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " Authorized customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());
                    Session["result"] = "Authroized" +  response.GetValue("Response_Message").ToString();
                    ViewBag.SuccessMessage = "Authroized" + response.GetValue("Response_Message").ToString();
                }
                else
                {
                    Session["fail"] = response.GetValue("Response_Message").ToString();
                    TempData["fail"] = response.GetValue("Response_Message").ToString();
                }
               // return RedirectToAction("CustomerAuthorization");
            }
            if (sts.Equals("RA"))
            {
                string apiresponse = Connecttocore.activateCustomer(id.ToString(), Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    Session["result"] = "Activate " + response.GetValue("Response_Message").ToString(); //response.GetValue("Response_Message").ToString();
                    ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
            }
            if (sts.Equals("RDA"))
            {
                string apiresponse = Connecttocore.deactivateCustomer(id.ToString(), Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " De-Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    Session["result"] = "DeActivate " + response.GetValue("Response_Message").ToString();
                    ViewBag.SuccessMessage = "DeActivate " + response.GetValue("Response_Message").ToString();
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
            }
            if (sts.Equals("RR"))
            {
                string apiresponse = Connecttocore.restCustomerPassword(id.ToString(), Session["accesstoken"].ToString());
                JObject response = new JObject();
                response = JObject.Parse(apiresponse);

                int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                if (responseCode == 0)
                {
                    ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                    string result2 = ds.resetpassword(id.ToString());
                    if (!result2.Equals("0"))
                    {
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " Reset Customer Password", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                        Session["result"] = "Reset Password " + response.GetValue("Response_Message").ToString();
                        ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                        // model.pass = result2;
                        //Session["pass"] = model.pass;
                    }
                   // Session["pass"] = model.pass;
                   // return RedirectToAction("Print", "resetCustomer");
                }
                else
                {
                    message = response.GetValue("Response_Message").ToString();
                    ModelState.AddModelError("", message);
                }
            }
            return RedirectToAction("CustomerAuthorization");
        }
        public ActionResult Reject(string id, string sts)  //String status
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


            try
            {
                custinfo infomodel = new custinfo();
                infomodel = ds.getcustinfobyid(id);


                if (sts.Equals("Un Authorized"))
                {
                    //string apiresponse = Connecttocore.authroizeCustomer(id.ToString(), Session["accesstoken"].ToString());
                    //JObject response = new JObject();
                    //response = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                    //if (responseCode == 0)
                    //{
                    //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " Authorized customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());
                    //    Session["result"] = "Authroized" + response.GetValue("Response_Message").ToString();
                    //    ViewBag.SuccessMessage = "Authroized" + response.GetValue("Response_Message").ToString();
                    //}
                    //else
                    //{
                    //    Session["fail"] = response.GetValue("Response_Message").ToString();
                    //    TempData["fail"] = response.GetValue("Response_Message").ToString();
                    //}

                    if (ModelState.IsValid)
                    {
                        //custinfo infomodel = new custinfo();
                        String responseinfo;
                        //String fullaccountnumber = "35" + model.AccountNumber;
                        //infomodel = ds.getcustinfo(model.BranchCode, model.AccountTypecode, model.AccountNumber, model.CurrencyCode, model.CategoryCode, fullaccountnumber);
                        //infomodel = ds.getcustinfobyid(id);
                        responseinfo = infomodel.lblconfirm;

                        if (responseinfo.Equals("This Account is Already exist"))
                        {
                            string useridtodelete = ds.getuserid(infomodel.user_log);
                            int result = 0;
                            result = ds.deletecustomer(infomodel.user_log);
                            if (result == 1)
                            {
                                Session["result"] = "Customer Deleted Successfuly";
                                TempData["success"] = "Customer Deleted.";
                            }
                            else
                            {
                                Session["fail"] = "Customer cannot be deleted.";
                                TempData["fail"] = "Customer cannot be deleted.";
                            }
                            //return View(model);
                        }
                        else
                        {
                            message = "This Customer Account Is Not Register";
                            ModelState.AddModelError("", message);
                            //return View(model);
                        }
                    }
                    else
                    {
                        message = "All Fields are required ";
                        ModelState.AddModelError("", "Something is missing" + message);
                    }



                    // return RedirectToAction("CustomerAuthorization");
                }

                if (sts.Equals("Request to Activate"))
                {

                    //UpdatecustomerSts

                     if (ds.UpdatecustomerSts(infomodel.user_log, "A"))
                    {
                        Session["result"] = " Reject Customer Activated Successfuly";
                        //Session["acresult"] = "Customer information activation request was successful";
                        //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), model.AccountNumber, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request Activated customer", model.CustomerName + " - " + model.Branch, DateTime.Now.ToString());
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    }
                    else
                    {
                        Session["acresult"] = "Something has gone wrong, please try again.";
                    }
                    //string apiresponse = Connecttocore.activateCustomer(id.ToString(), Session["accesstoken"].ToString());
                    //JObject response = new JObject();
                    //response = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                    //if (responseCode == 0)
                    //{
                    //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    //    Session["result"] = "Activate " + response.GetValue("Response_Message").ToString(); //response.GetValue("Response_Message").ToString();
                    //    ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                    //}
                    //else
                    //{
                    //    message = response.GetValue("Response_Message").ToString();
                    //    ModelState.AddModelError("", message);
                    //}
                }

                if (sts.Equals("Request to DeActivate"))
                {

                    //UpdatecustomerSts

                    if (ds.UpdatecustomerSts(infomodel.user_log, "DA"))
                    {
                        Session["result"] = " Reject Customer DeActivated Successfuly";
                        //Session["acresult"] = "Customer information Deactivation request was successful";
                        //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), model.AccountNumber, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request Activated customer", model.CustomerName + " - " + model.Branch, DateTime.Now.ToString());
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject DeActivated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    }
                    else
                    {
                        Session["acresult"] = "Something has gone wrong, please try again.";
                    }
                    //string apiresponse = Connecttocore.activateCustomer(id.ToString(), Session["accesstoken"].ToString());
                    //JObject response = new JObject();
                    //response = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                    //if (responseCode == 0)
                    //{
                    //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    //    Session["result"] = "Activate " + response.GetValue("Response_Message").ToString(); //response.GetValue("Response_Message").ToString();
                    //    ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                    //}
                    //else
                    //{
                    //    message = response.GetValue("Response_Message").ToString();
                    //    ModelState.AddModelError("", message);
                    //}
                }

                if (sts.Equals("Request to Reset Password"))
                {

                    //UpdatecustomerSts

                    if (ds.UpdatecustomerSts(infomodel.user_log, "A"))
                    {
                        Session["result"] = " Reject Customer Reset Password Successfuly";
                        //Session["acresult"] = "Customer information Deactivation request was successful";
                        //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), model.AccountNumber, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request Activated customer", model.CustomerName + " - " + model.Branch, DateTime.Now.ToString());
                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject Reset Password ", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    }
                    else
                    {
                        Session["acresult"] = "Something has gone wrong, please try again.";
                    }
                    //string apiresponse = Connecttocore.activateCustomer(id.ToString(), Session["accesstoken"].ToString());
                    //JObject response = new JObject();
                    //response = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                    //if (responseCode == 0)
                    //{
                    //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reject Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                    //    Session["result"] = "Activate " + response.GetValue("Response_Message").ToString(); //response.GetValue("Response_Message").ToString();
                    //    ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                    //}
                    //else
                    //{
                    //    message = response.GetValue("Response_Message").ToString();
                    //    ModelState.AddModelError("", message);
                    //}
                }




                //if (sts.Equals("RDA"))
                //{
                //    string apiresponse = Connecttocore.deactivateCustomer(id.ToString(), Session["accesstoken"].ToString());
                //    JObject response = new JObject();
                //    response = JObject.Parse(apiresponse);

                //    int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                //    if (responseCode == 0)
                //    {
                //        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " De-Activated customer", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                //        Session["result"] = "DeActivate " + response.GetValue("Response_Message").ToString();
                //        ViewBag.SuccessMessage = "DeActivate " + response.GetValue("Response_Message").ToString();
                //    }
                //    else
                //    {
                //        message = response.GetValue("Response_Message").ToString();
                //        ModelState.AddModelError("", message);
                //    }
                //}
                //if (sts.Equals("RR"))
                //{
                //    string apiresponse = Connecttocore.restCustomerPassword(id.ToString(), Session["accesstoken"].ToString());
                //    JObject response = new JObject();
                //    response = JObject.Parse(apiresponse);

                //    int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                //    if (responseCode == 0)
                //    {
                //        ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                //        string result2 = ds.resetpassword(id.ToString());
                //        if (!result2.Equals("0"))
                //        {
                //            ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), " Reset Customer Password", id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                //            Session["result"] = "Reset Password " + response.GetValue("Response_Message").ToString();
                //            ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                //            // model.pass = result2;
                //            //Session["pass"] = model.pass;
                //        }
                //        // Session["pass"] = model.pass;
                //        // return RedirectToAction("Print", "resetCustomer");
                //    }
                //    else
                //    {
                //        message = response.GetValue("Response_Message").ToString();
                //        ModelState.AddModelError("", message);
                //    }
                //}






                //string apiresponse = Connecttocore.rejectCustomer(id.ToString(), Session["accesstoken"].ToString());
                //JObject response = new JObject();
                //response = JObject.Parse(apiresponse);

                //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                //    //string  responseCode = response.GetValue("Response_Message").ToString();
                //    if (responseCode == 0)
                //{


                //    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), id.ToString(), Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request Rejected  for status " + sts, id.ToString() + " - " + id.ToString(), DateTime.Now.ToString());

                //    Session["result"] = "Request Rejected " +  response.GetValue("Response_Message").ToString();
                //    ViewBag.SuccessMessage = response.GetValue("Response_Message").ToString();
                //}
                //else
                //{
                //    Session["fail"] = response.GetValue("Response_Message").ToString();
                //    TempData["fail"] = response.GetValue("Response_Message").ToString();
                //}
                //return RedirectToAction("CustomerAuthorization");




            }
            catch(Exception ex)
            {
                message = "Please Contact for Support";
                ModelState.AddModelError("", "Something is missing" + message);

                //return RedirectToAction("CustomerAuthorization");
            }
            return RedirectToAction("CustomerAuthorization");
        }
    }
}