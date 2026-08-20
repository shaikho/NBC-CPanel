using FCBCPanel.Models;
using AljazeeraCPanel.Models;
using AljazeeraCPanel.Filters;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FCBCPanel.Controllers
{
    [AuthorizeSession]
    public class AccountTypesController : Controller
    {
        DataSource ds = new DataSource();
        public ActionResult AccountTypes()
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

            List<AccountTypeModel> accounttypes = ds.GetAllAccountTypes();
            return View(accounttypes);
        }

        public ActionResult Authorize()
        {
            if ((Session["cpanelLogin"] == null) || !Session["cpanelLogin"].ToString().Equals("true"))
            {
                return RedirectToAction("Login", "Login");
            }

            if (Session["userresult"] != null)
            {
                ViewBag.SuccessMessage = Session["userresult"].ToString();
                Session["useerresult"] = null;
            }

            List<AccountTypeModel> accounttypes = ds.GetAllPendingAccountTypes();
            return View(accounttypes);
        }

        public ActionResult Details(string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            AccountTypeModel account_type = ds.getaccounttypedetails(id);
            return View(account_type);
        }

        public ActionResult AuthorizeAccountType(string account_type_code)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Boolean status = ds.AuthorizeAccountType(account_type_code);
            if (status)
            {
                TempData["success"] = "Account Type Authorized.";
            }
            else
            {
                TempData["fail"] = "Account type authroization failed.";
            }
            return RedirectToAction("Authorize");
        }

        public ActionResult RejectAccountType(string account_type_code)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Boolean status = ds.RejectAccountType(account_type_code);
            if (status)
            {
                TempData["rejectsuccess"] = "Account Type Rejected.";
            }
            else
            {
                TempData["rejectfail"] = "Account type rejection failed.";
            }
            return RedirectToAction("Authorize");
        }


        public ActionResult Add()
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
        public ActionResult Add(AccountTypeModel model)
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
                ModelState.Clear();
                model.account_type_status = "P";
                if (ModelState.IsValid)
                {
                    model.account_type_code = model.account_type_code;
                    int _records = ds.Insertaccounttype(model.account_type_code, model.account_type, model.account_type_arabic, model.account_type_no, model.account_type_status);
                    if (_records > 0)
                    {
                        TempData["Success"] = model.account_type + " Account Type added successfuly";
                        return RedirectToAction("AccountTypes");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Can Not Insert");
                    }
                }
                else
                {
                    string message = "All Fields are required ";
                    ModelState.AddModelError("", "Something is missing" + message);
                }
            }
            catch (Exception e)
            {
                string message = "All Fields are required ";
                ModelState.AddModelError("", "Something is missing" + message);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            AccountTypeModel model;
            model = ds.getaccounttypedetails(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(AccountTypeModel updatemodel, string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            updatemodel.account_type_code = id;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                int _records = ds.UpdateAccountType(updatemodel);
                if (_records > 0)
                {
                    TempData["Success"] = updatemodel.account_type + "Account Type updated successfuly";
                    return RedirectToAction("AccountTypes");
                }
                else
                {
                    ModelState.AddModelError("", "Can Not Update");
                }
            }
            else
            {
                ModelState.AddModelError("", "All Information Required");
            }
            return View(updatemodel);
        }

        public ActionResult Delete(string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            int records = ds.deleteAccountType(id);
            if (records > 0)
            {
                TempData["Success"] = "Account Type deleted successfuly";
                return RedirectToAction("AccountTypes");
            }
            else
            {
                ModelState.AddModelError("", "Can Not Delete");
                return View("AccountTypes");
            }
            // return View("Index");
        }
    }
}