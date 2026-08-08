using AljazeeraCPanel.Models;
using AljazeeraCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIBCPanel.Context;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json;
using System.Net;

namespace AljazeeraCPanel.Controllers
{
    public class LoginController : Controller
    {
        DataSource ds = new DataSource();
        //
        // GET: /Login/


        public ActionResult Login()
        {
            Session["cpanelLogin"] = "false";
            return View();
        }


        [HttpPost] 
        public ActionResult Login(Loginmodel model)
        {

            Loginmodelresult result = new Loginmodelresult();
            try
            {

                string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                string logincallresponse = "";
                string accesstoken = "";

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];
                }

                // We do not want to use any existing identity information

                Connecttocore.getconfig();
                Uri requestUri = new Uri(Connecttocore.BASE_URL + "/cpLogin");
                string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");


                dynamic dynamicJson = new ExpandoObject();

                dynamicJson.User_ID = model.Username;
                dynamicJson.Password = model.Password;
                dynamicJson.ChannelID = 3;
                dynamicJson.Device_Key = "02bff589f9324810";

                string json = "";
                json = JsonConvert.SerializeObject(dynamicJson);
                var responJsonText = "";
                JObject JResp = new JObject();
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var objClient = new HttpClient())
                {
                    try
                    {
                        

                        HttpResponseMessage respon = objClient
                            .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;


                        if (respon.IsSuccessStatusCode)
                        {
                            logincallresponse = respon.Content.ReadAsStringAsync().Result;
                            accesstoken = respon.Headers.GetValues("Authorization").FirstOrDefault();
                        }


                    }

                    catch (Exception e)
                    { 

                        logincallresponse = "Error";
                    }


                }




                JObject response = new JObject();
                response = JObject.Parse(logincallresponse);


                if (int.Parse(response.GetValue("Response_Code").ToString()) == 0)
                {


                    Session["cpanelLogin"] = "true";
                    Session["accesstoken"] = accesstoken;
                    Session["username"] = response.GetValue("User_Name").ToString();
                    Session["user_log"] = model.Username;
                    Session["UserId"] = "3";
                    Session["user_name"] = response.GetValue("User_Name").ToString();
                    Session["user_branch"] = response.GetValue("Branch_Code").ToString();
                    Session["user_roleid"] = response.GetValue("Role").ToString(); //"2";






                    ////
                    ///
                    result = ds.checkuserlogin(model.Username, model.Password, ipAddress);

                    if (result.lblconfirm.Equals("home"))
                    {
                        Session["cpanelLogin"] = "true";
                        string br = Session["user_branch"].ToString();
                        String brr = ds.GetBranchName(br);
                        Session["branch_namee"] = brr;
                        Session["username"] = model.Username;//added
                        Session["user_log"] = model.Username;
                        Session["UserId"] = result.UserId;
                        Session["user_name"] = result.user_name;
                        Session["user_branch"] = result.user_branch;
                        Session["user_roleid"] = result.user_roleid;
                        Session["user_status"] = result.status;
                        //model = ds.GetOnlineOfflineUsers(result.user_branch);
                        //Session["onlineofflineusers"] = list;
                        return RedirectToAction("Index", "Home");

                    }

                    else
                if (result.lblconfirm.Equals("change_pass"))
                    {
                        Session["cpanelLogin"] = "changepass";
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
                        ModelState.AddModelError("", result.lblconfirm);
                        return View(model);
                    }

                    /////






                    //Session["user_status"] = result.status;



                   // return RedirectToAction("Index", "Home");


                }
                else
                {
                    ModelState.AddModelError("", response.GetValue("Response_Message").ToString());
                    return View(model);
                }
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