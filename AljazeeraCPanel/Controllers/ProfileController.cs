using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIBCPanel.Controllers
{
    public class ProfileController : Controller
    {
        //
        // GET: /Profile/
        DataSource ds = new DataSource();
        //
        // GET: /AddAcount/
        public ActionResult ProfileManagement()
        {
            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["userresult"] != null)
            {
                ViewBag.SuccessMessage = Session["userresult"].ToString();
                Session["userresult"] = null;
            }
            List<profilelist> profiles = ds.GetAllCustomerProfiles();
            // ViewBag.UserList = dataset.Tables[0];
            return View(profiles);
        }

        public ActionResult NewProfile()
        {
            if (Session["addprofileresult"] != null)
            {
                ViewBag.SuccessMessage = Session["addprofileresult"].ToString();
                Session["addprofileresult"] = null;

            }
            Session["profilelist"] = null;
            Session["menu_category"] = null;

            Profilemangement model = new Profilemangement();

            model.catgories = ds.GetGatgories();
            return View(model);

        }

        [HttpPost]
        public ActionResult NewProfile(Profilemangement model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            model.catgories = ds.GetGatgories();
            var selectedcategory = model.catgories.Find(p => p.Value == model.menu_category.ToString());
            if (selectedcategory != null)
            {
                selectedcategory.Selected = true;

            }
            if (model.menu_category != null)
            {
                Session["profilelist"] = true;
                Session["menu_category"] = model.menu_category;
                List<pageparameter> items = new List<pageparameter>();
                items = ds.PopulateCustomerProfilemangement(model.menu_category);

                model.pages = items;
                return View(model);
            }
            return View(model);

        }

        [HttpPost]
        public ActionResult Addprofile(Profilemangement model)
        {

            String result = "", res = "";
            String message;
            try
            {
                model.catgories = ds.GetGatgories();
                if (ModelState.IsValidField(model.profilename))
                {

                    List<pageparameter> lHob = new List<pageparameter>();
                    lHob = model.pages;
                    foreach (var item in lHob)
                    {
                        if (item.IsSelected == true)
                        {
                            result = ds.addnewprofile(model.profilename, item.menuid, item.menuparentid);
                            res += " " + item.menuname + " : " + result;
                            Session["addprofileresult"] = "Profile Creation  Complete Successfully";
                        }

                    }

                    return RedirectToAction("NewProfile");

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

        public ActionResult Delete(int roleid)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int userscount = ds.getprofileuserscount(roleid);
            if (userscount > 0)
            {
                Session["addprofileresult"] = "Profile Cannot be deleted while containing users";
                ViewBag.SuccessMessage = Session["addprofileresult"];
                return RedirectToAction("ProfileManagement");
            }
            else
            {
                int records = ds.deleteprofile(roleid);
                if (records > 0)
                {
                    Session["addprofileresult"] = "Profile deleted successfully";
                    return RedirectToAction("ProfileManagement");
                }
                else
                {
                    ModelState.AddModelError("", "Can Not Delete");
                    return View("ProfileManagement", "CPanelProfileManagement");
                }
            }
        }

        public ActionResult managelimits(string roleid)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            List<Transferlimit> services = new List<Transferlimit>();
            services = ds.GetServicesByRole(roleid);
            return View(services);
        }

        public ActionResult edit(string roleid)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Transferlimit service = ds.GetSingleServicesByRole(roleid);
            return View(service);
        }

        [HttpPost]
        public ActionResult edit(Transferlimit model)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Boolean status = ds.updatelimit(model);
            if (status)
            {
                //Transferlimit service = ds.GetSingleServicesByRole(roleid);
                TempData["success"] = "Customer profile : " + model.servicename + ", transaction limit is now up to " + model.amount_limit + " SDG,"+model.number_limit+" transaction per day and "+model.daily_limit+" SDG as a daily limit.";
            }
            else
            {
                TempData["fail"] = "Profile update failed";
            }
            return RedirectToAction("ProfileManagement");
        }
    }

}