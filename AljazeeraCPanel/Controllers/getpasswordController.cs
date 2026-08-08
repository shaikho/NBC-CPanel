//using hashmakersol.pdfmaker;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using hashmakersol.pdfmaker;

namespace Cpanel.Controllers
{
    public class getpasswordController : Controller
    {
        DataSource ds = new DataSource();
        Connecttocore core = new Connecttocore();
        //
        // GET: /getpassword/
        public ActionResult GetPassword()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            //return View();
            Customerinfopass model = new Customerinfopass();
            String userbranch = Session["user_branch"].ToString();
            model.Branches = ds.PopulateBranchs(userbranch);
            model.AccTypes = ds.PopulateAccountTypes();
            model.Currencies = ds.PopulateCurrencies();
            model.catgories = ds.GetGatgories();

            Session["regmodel"] = model;
            return View(model);

        }

        [HttpPost]
        public ActionResult GetPassword(Customerinfopass model)
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


                GETpassword model1 = new GETpassword();
                model.Branches = ds.PopulateBranchs(userbranch);
                model.AccTypes = ds.PopulateAccountTypes();
                model.Currencies = ds.PopulateCurrencies();
                model.catgories = ds.GetGatgories();

                if (ModelState.IsValid)
                {
                    custinfo infomodel = new custinfo();

                    String response;

                    String fullaccount = "35" + model.AccountNumber;
                    Session["getpasswordfullaccount"] = fullaccount;
                    infomodel = ds.getcustinfo(model.BranchCode,  fullaccount);
                    response = infomodel.lblconfirm;
                    if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("U"))
                    {
                        String Accountnumber = "35" + model.AccountNumber;
                        //String Accountnumber = model1.account;
                        List<GETpassword> result = new List<GETpassword>();
                        result = ds.getpassword(Accountnumber);
                        foreach (var item in result)
                        {
                            if (item.lblconfirm == "Successfully")
                            {
                                //ds.updatecustomer(infomodel.user_id.ToString(), "A");
                                //ds.updateAccount(infomodel.user_id.ToString(), Accountnumber.ToString(), "A");

                                model1.name = item.name;
                                model1.account = item.account;
                                model1.branchname = item.branchname;
                                model1.pass = item.pass;
                                model1.fullaccount = fullaccount;
                                Session["pgetpassresult"] = model1;
                                return RedirectToAction("Print", "getpassword");
                                //return RedirectToAction("Print","getpassword");
                            }
                            else
                            {
                                ModelState.AddModelError("", item.lblconfirm);
                            }
                        }
                    }

                    else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("P"))
                    {

                        message = "This Customer Account Is Not Authorized";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }

                    else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("R"))
                    {
                        message = "This Customer Account Is Rejected";
                        ModelState.AddModelError("", message);
                        return View(model);

                    }

                    else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("A"))
                    {
                        message = "This Customer Account Is  activated already";
                        ModelState.AddModelError("", message);
                        return View(model);
                    }
                    else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("D"))
                    {
                        message = "This Customer Account Is Deleted or Deactivated";
                        ModelState.AddModelError("", message);
                        return View(model);


                    }
                    else if (response.Equals("This Account is Already exist") && infomodel.status.ToString().Equals("S"))
                    {
                        message = "This Customer Account Is Stoped";
                        ModelState.AddModelError("", message);
                        return View(model);


                    }
                    else
                    {
                        message = "This Customer Account Is Not Register";
                        ModelState.AddModelError("", message);
                        return View(model);


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

        [HttpPost]
        public ActionResult GetPasswordprocess(Customerinfopass passedmodel)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            Customerinfopass model = new Customerinfopass();
            if (passedmodel.placeholder != null)
            {
                model = new Customerinfopass();
                String userbranch = Session["user_branch"].ToString();
                model = ds.GetUserinfoData(passedmodel.placeholder);
                model.Branches = ds.PopulateBranchs(model.BranchCode);
                model.AccTypes = ds.PopulateAccountTypes(passedmodel.placeholder);
                model.Currencies = ds.PopulateCurrencies(model.CurrencyCode);

                model.catgories = ds.GetGatgories();
                return View("GetPassword", model);
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
                return View("GetPassword", model);
            }
        }

        public ActionResult smspassword(string password, string account)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            custinfo customerinformations = ds.getcustinfo( "", account);
            string msg = "Your Account temporery password is : " + password;
            //string msg = "تم إنشاء حسابك ويمكنك الدخول عن طريق كلمة السر : " + password + " .";
           // string response2 = core.sendpredefinedsms(customerinformations.user_id,"","1", customerinformations.user_mobile);
            //string apiresponse = Connecttocore.activateCustomer(customerinformations.user_id, Session["accesstoken"].ToString()); // customerinformations.user_mobile , msg
            //string apiresponse = Connecttocore.sendotpbyURL(customerinformations.user_id, msg , customerinformations.user_mobile); // customerinformations.user_mobile , msg
            var response = core.sendotpbyURL(customerinformations.user_id, msg, customerinformations.user_mobile);


            // sendotpbyURL
            //JObject jobj = new JObject();
            //jobj = JObject.Parse(response);
            //dynamic result = jobj;

            //var errorCode = result.errorcode;
            //var errormsg = result.errormsg;
            //var Status = result.status;


            //JObject response = new JObject();
            //response = JObject.Parse(apiresponse);

            //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());

            //if (responseCode == 0)
            //    {
            //        ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
            //    }
            //    else
            //    {
            //        message = response.GetValue("Response_Message").ToString();
            //        ModelState.AddModelError("", message);
            //    }
            var Status = 1;
            if (Status == 1) //
            {
               // ViewBag.SuccessMessage = "Activate " + response.GetValue("Response_Message").ToString();
                string customeraccount = Session["getpasswordfullaccount"].ToString();
                string userid = ds.getCustIDFromAcc(customeraccount);
                ds.updatecustomer(userid, "A");
                ds.updateAccount(userid, customeraccount, "A");
                string custname = Session["customername"].ToString();
                string usershorSthand = "23" + customeraccount.Substring(2, 3) + customeraccount.Substring(13);
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Password sent to customer via sms", usershorSthand + " - " + custname, DateTime.Now.ToString());

                TempData["Success"] = true;
                ViewBag.ResponseStat = "Successful";
                ViewBag.ResponseMSG = "Password sent to customer via sms successfully";
                ViewBag.SuccessMessage = "Password sent to customer via SMS.";
                TempData["successful"] = "Password sent to customer via sms successfully";
                return RedirectToAction("GetPassword");
            }
            else
            {
                //message = response.GetValue("Response_Message").ToString();
                //ModelState.AddModelError("", message);

                TempData["Success"] = true;
                ViewBag.ResponseStat = "Not Successful";
                ViewBag.ResponseMSG = "Failed to send password sms, please try again.";
                ViewBag.SuccessMessage = "Password has not been sent yet.";
                TempData["failed"] = "Failed to send password sms, please try again.";
                return RedirectToAction("GetPassword");
            }
        }

        public ActionResult Print()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            GETpassword model = new GETpassword();

            model = (GETpassword)Session["pgetpassresult"];
            Session["customername"] = model.name;
            Session["customeraccount"] = model.account;
            return View(model);
        }

        public ActionResult DownloadPdf()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            List<GETpassword> model = new List<GETpassword>();
            model = (List<GETpassword>)Session["pgetpassresult"];
            return new PdfResult(model, "Print");

        }

        public FileResult SavePDF()
        {
            string customeraccount = Session["getpasswordfullaccount"].ToString();
            string userid = ds.getCustIDFromAcc(customeraccount);
            ds.updatecustomer(userid, "A");
            ds.updateAccount(userid, customeraccount, "A");
            string custname = Session["customername"].ToString();
            //string customeraccount = Session["getpasswordfullaccount"].ToString();
            string usershorSthand = "35" + customeraccount.Substring(2, 3) + customeraccount.Substring(13);
            string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Password printed to customer", usershorSthand + " - " + custname, DateTime.Now.ToString());

            //List < Employee > employees = _context.employees.ToList < Employee > ();  
            GETpassword model = new GETpassword();

            model = (GETpassword)Session["pgetpassresult"];

            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("Customerpassword - " + model.account.ToString() + " - " + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
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

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 25, 25, 25, 25 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  




            tableLayout.AddCell(new PdfPCell(new Phrase("FCB Internet Banking", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0))))
            {
                Colspan = 12,
                Border = 0,
                PaddingBottom = 5,
                HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header 

            AddCellToHeader(tableLayout, "Customer Account");
            AddCellToHeader(tableLayout, "Customer Name");
            AddCellToHeader(tableLayout, "Customer Password");
            AddCellToHeader(tableLayout, "Date");

            ////Add body  

            GETpassword model = new GETpassword();

            model = (GETpassword)Session["pgetpassresult"];


            AddCellToBody(tableLayout, model.account.ToString());
            //AddCellToBody(tableLayout, model.branchname.ToString());
            AddCellToBody(tableLayout, model.name.ToString());
            AddCellToBody(tableLayout, model.pass.ToString());
            AddCellToBody(tableLayout, DateTime.Now.ToString());



            return tableLayout;
        }


        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(128, 128, 128)
            });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Padding = 5,
                BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
    }
}