using FCBCPanel.Models;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FCBCPanel.Controllers
{
    [AuthorizeSession]
    public class BranchsController : Controller
    {
        DataSource ds = new DataSource();
        // GET: Branchs
        public ActionResult Branchs()
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

            List<BranchModel> branchs = ds.GetAllBranchs();
            return View(branchs);
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

            List<BranchModel> branchs = ds.GetAllPendingBranchs();
            return View(branchs);
        }

        public ActionResult Details(string branch_code)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            BranchModel branch = ds.getbranchdetails(branch_code);
            return View(branch);
        }

        public ActionResult AuthorizeBranch(string branch_code)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Boolean status = ds.AuthorizeBranch(branch_code);
            if (status)
            {
                TempData["success"] = "Branch Authorized.";
            }
            else
            {
                TempData["fail"] = "Branch authroization failed.";
            }
            return RedirectToAction("Authorize");
        }

        public ActionResult RejectBranch(string branch_code)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Boolean status = ds.RejectBranch(branch_code);
            if (status)
            {
                TempData["rejectsuccess"] = "Branch Rejected.";
            }
            else
            {
                TempData["rejectfail"] = "Branch rejection failed.";
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
        public ActionResult Add(BranchModel model)
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
                model.branch_status = "P";
                if (ModelState.IsValid)
                {
                    model.branch_code_no = model.branch_code;
                    int _records = ds.Insertbranch(model.branch_code, model.branch_name, model.branch_name_arabic, model.branch_status,model.branch_code_no);
                    if (_records > 0)
                    {
                        TempData["Success"] = model.branch_name + "branch added successfuly";
                        return RedirectToAction("Branchs");
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
            BranchModel model;
            model = ds.getbranch(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(BranchModel updatemodel, string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            updatemodel.branch_code = id;
            updatemodel.branch_code_no = id;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                int _records = ds.Updatebranch(updatemodel);
                if (_records > 0)
                {
                    TempData["Success"] = updatemodel.branch_name + "branch updated successfuly";
                    return RedirectToAction("Branchs");
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
            int records = ds.deletebranch(id);
            if (records > 0)
            {
                TempData["Success"] = "Branch deleted successfuly";
                return RedirectToAction("Branchs");
            }
            else
            {
                ModelState.AddModelError("", "Can Not Delete");
                return View("Branchs");
            }
            // return View("Index");
        }

        public ActionResult managelimits()
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
    }
}