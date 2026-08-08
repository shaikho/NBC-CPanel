using FCBCPanel.Models;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FCBCPanel.Controllers
{
    public class CurrenciesController : Controller
    {
        DataSource ds = new DataSource();
        public ActionResult Currencies()
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

            List<CurrencyModel> currencies = ds.GetAllCurrencies();
            return View(currencies);
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
        public ActionResult Add(CurrencyModel model)
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
                if (ModelState.IsValid)
                {
                    model.currency_code = model.currency_code;
                    int _records = ds.Insertcurrency(model.currency_code, model.currency_name, model.currency_summary, model.currency_status);
                    if (_records > 0)
                    {
                        TempData["Success"] = model.currency_name + "Currency added successfuly";
                        return RedirectToAction("Currencies");
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

            CurrencyModel model;
            model = ds.getcurrencydetails(id);

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CurrencyModel updatemodel, string id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            updatemodel.currency_code = id;
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                int _records = ds.updatecurrency(updatemodel);
                if (_records > 0)
                {
                    TempData["Success"] = updatemodel.currency_name + "Currency has been updated successfuly";
                    return RedirectToAction("Currencies");
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
            int records = ds.deletecurrency(id);
            if (records > 0)
            {
                TempData["Success"] = "currency deleted successfuly";
                return RedirectToAction("Currencies");
            }
            else
            {
                ModelState.AddModelError("", "Can Not Delete");
                return View("Currencies");
            }
            // return View("Index");
        }
    }
}