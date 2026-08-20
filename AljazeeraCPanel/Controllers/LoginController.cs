using AljazeeraCPanel.Context;
using AljazeeraCPanel.Models;
using AljazeeraCPanel.Security;
using AljazeeraCPanel.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using System.Web.SessionState;

namespace AljazeeraCPanel.Controllers
{
    public class LoginController : Controller
    {
        DataSource ds = new DataSource();

        private void RegenerateSessionId()
        {
            SessionIDManager manager = new SessionIDManager();
            string newSessionId = manager.CreateSessionID(System.Web.HttpContext.Current);
            bool redirected;
            bool isAdded;
            manager.SaveSessionID(System.Web.HttpContext.Current, newSessionId, out redirected, out isAdded);
        }

        public ActionResult Login()
        {
            Session.Clear();
            Session.Abandon();
            Session["cpanelLogin"] = "false";
            return View();
        }

        [HttpPost] 
        public ActionResult Login(Loginmodel model)
        {

            Loginmodelresult result = new Loginmodelresult();
            try
            {
                // WAPT02-02: Check password policy BEFORE database authentication
                // This prevents weak credentials from being used even if they exist in the system
                var (isPolicyValid, policyError) = PasswordPolicyValidator.ValidatePassword(model.Password);
                if (!isPolicyValid)
                {
                    ModelState.AddModelError("", "Invalid password: " + policyError);
                    return View(model);
                }

                string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                string logincallresponse = "";
                string accesstoken = "";

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }

                // WAPT07: rate-limit login attempts per IP + username (5 failures / 15 min).
                string rlKey = "login:" + ipAddress + ":" + (model.Username ?? "").ToLowerInvariant();
                if (RateLimiter.IsBlocked(rlKey, 5))
                {
                    ModelState.AddModelError("", "Too many failed login attempts. Please try again in a few minutes.");
                    return View(model);
                }

                // We do not want to use any existing identity information

                //Connecttocore.getconfig();
                //Uri requestUri = new Uri(Connecttocore.BASE_URL + "/cpLogin");
                //string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                //dynamic dynamicJson = new ExpandoObject();

                //dynamicJson.User_ID = model.Username;
                //dynamicJson.Password = model.Password;
                //dynamicJson.ChannelID = 3;
                //dynamicJson.Device_Key = "02bff589f9324810";

                //string json = "";
                //json = JsonConvert.SerializeObject(dynamicJson);
                //var responJsonText = "";
                //JObject JResp = new JObject();
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //using (var objClient = new HttpClient())
                //{
                //    try
                //    {


                //        HttpResponseMessage respon = objClient
                //            .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;


                //        if (respon.IsSuccessStatusCode)
                //        {
                //            logincallresponse = respon.Content.ReadAsStringAsync().Result;
                //            accesstoken = respon.Headers.GetValues("Authorization").FirstOrDefault();
                //        }


                //    }

                //    catch (Exception e)
                //    { 

                //        logincallresponse = "Error";
                //    }


                //}




                //JObject response = new JObject();
                //response = JObject.Parse(logincallresponse);


                //if (int.Parse(response.GetValue("Response_Code").ToString()) == 0)
                //{


                    result = ds.checkuserlogin(model.Username, model.Password, ipAddress);

                    if (result.lblconfirm.Equals("home"))
                    {
                        RateLimiter.Reset(rlKey); // WAPT07: clear the counter on success
                        Session.Clear();
                        Session.Abandon();
                        RegenerateSessionId();

                        Session["cpanelLogin"] = "true";
                        Session["accesstoken"] = accesstoken;
                        Session["username"] = model.Username;
                        Session["user_log"] = model.Username;
                        Session["UserId"] = result.UserId;
                        Session["user_name"] = result.user_name;
                        Session["user_branch"] = result.user_branch;
                        Session["user_roleid"] = result.user_roleid;
                        Session["user_status"] = result.status;

                        string br = result.user_branch;
                        String brr = ds.GetBranchName(br);
                        Session["branch_namee"] = brr;
                        //model = ds.GetOnlineOfflineUsers(result.user_branch);
                        //Session["onlineofflineusers"] = list;
                        return RedirectToAction("Index", "Home");

                    }

                    else
                if (result.lblconfirm.Equals("change_pass"))
                    {
                        Session.Clear();
                        Session.Abandon();
                        RegenerateSessionId();

                        RateLimiter.Reset(rlKey); // WAPT07: clear the counter on success
                        Session["cpanelLogin"] = "changepass";
                        Session["accesstoken"] = accesstoken;
                        Session["user_log"] = model.Username;
                        Session["UserId"] = result.UserId;
                        Session["user_name"] = result.user_name;
                        Session["user_branch"] = result.user_branch;
                        Session["user_roleid"] = result.user_roleid;
                        List<int> list = ds.GetOnlineOfflineUsers(result.user_branch);
                        Session["onlineofflineusers"] = list;
                        return RedirectToAction("Changepassword");
                    }
                    else
                    {
                        // WAPT07: record a failed attempt against the IP+username window.
                        RateLimiter.RegisterAttempt(rlKey, 15);
                        ModelState.AddModelError("", result.lblconfirm);
                        return View(model);
                    }

                    /////






                    //Session["user_status"] = result.status;



                   // return RedirectToAction("Index", "Home");


                //}
                //else
                //{
                //    ModelState.AddModelError("", response.GetValue("Response_Message").ToString());
                //    return View(model);
                //}
            }
            catch (Exception e)
            {
            }
            return View();
        }

        public ActionResult Changepassword()
        {
            Session["cpanelLogin"] = "changepass";
            changepassword model = new changepassword();
            model.OldPassword = null;
            model.newPassword = null;
            model.confrimPassword = null;
            return View();
        }

        [HttpPost]
        public ActionResult Changepassword(changepassword model)
        {
            if (((model.OldPassword == null)
            || ((model.newPassword == null)
            || (model.confrimPassword == null))))
            {
                ModelState.AddModelError("", "Please Check Your Information ");
                return View();
            }


            if ((model.newPassword != model.confrimPassword))
            {

                ModelState.AddModelError("", "Please Check Your New Password ");
                model.OldPassword = null;
                model.newPassword = null;
                model.confrimPassword = null;
                return View();


            }

            // WAPT02-02: Validate new password against policy to prevent weak credentials after change
            var (isPolicyValid, policyError) = PasswordPolicyValidator.ValidatePassword(model.newPassword);
            if (!isPolicyValid)
            {
                ModelState.AddModelError("", "New password is invalid: " + policyError);
                model.OldPassword = null;
                model.newPassword = null;
                model.confrimPassword = null;
                return View();
            }

            String username = Session["user_log"].ToString();
            String result = ds.changepass(username, model.OldPassword, model.newPassword);
            if (result.Equals("Your Password was Changed Successfully"))
            {
                Session["cpanelLogin"] = "true";
                Session["Homemessage"] = result;
                //return RedirectToAction("Registration", "CustomerRegistration");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Session["cpanelLogin"] = "changepass";
                ModelState.AddModelError("", result);
                return View();
            }

        }

    }
}