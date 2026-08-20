using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using System.Data;
using AljazeeraCPanel.Context;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using SIBCPanel.Context;
using System.Security.Cryptography;
using AljazeeraCPanel.Security;
using Newtonsoft.Json.Linq;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class UserController : Controller
    {
        DataSource ds = new DataSource();
        Connecttocore core = new Connecttocore();

        // WAPT06: raw status codes that mean "a request is already pending" — no new
        // maker action may be started while the account sits in one of these.
        private static readonly System.Collections.Generic.HashSet<string> PendingCodes =
            new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase)
            { "RA", "RDA", "RD", "RED", "RRP" };

        /// <summary>
        /// WAPT04/06: reject reverts the pending request server-side, derived from the
        /// account's REAL status code — never from a client-supplied value, and never a
        /// delete. A request to delete (RD) reverts to Active so the account is kept.
        /// </summary>
        private static string RevertStatusFor(string realCode)
        {
            switch ((realCode ?? "").ToUpperInvariant())
            {
                // Preserves the application's existing revert mapping (per sign-off),
                // with the one WAPT04 correction: RD no longer maps to 'DE' (delete).
                case "UA": return "UA";   // brand-new, unauthorized: stays pending review
                case "RA": return "A";
                case "RDA": return "D";
                case "RD": return "A";    // was 'DE' (delete) — now keeps the account (WAPT04)
                case "RRP": return "A";
                case "RED": return "A";
                default: return null;     // not a pending/rejectable state
            }
        }

        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("UserReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


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

            float[] headers = { 25, 24, 45, 30,30 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

            DateTime dTime = DateTime.Now;
            //string UserName = Session["name"].ToString();
            //string AccNo = Session["AccNo"].ToString();
            //string fromDate = Session["fromDate"].ToString();
            //string toDate = Session["toDate"].ToString();
            //string AccountNumber = AccNo.Substring(13);
            //string AccountType = data.getaccounttype(AccNo.ToString().Substring(5, 5));
            //string BranchName = data.getbranchnameenglish(AccNo.ToString().Substring(2, 3));
            //string currency = data.GetCurrencyName(AccNo.Substring(10, 3));
            //String oDate = DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy");

            //paragraphs
            Paragraph Title = new Paragraph("NBE - Control Panel ",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK));
            Paragraph Title2 = new Paragraph("Customer Report For All Users",
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK));
            //Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
            //    new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            //Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
            //    new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            /*Paragraph From = new Paragraph("Statement of Account From  : " + DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy") + " To " + DateTime.ParseExact(toDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/
            Chunk c = new Chunk("Total of Customers Registered : total",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);

            /*Paragraph AccountNo = new Paragraph("Account No : " + AccountNumber,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /* Paragraph Currency = new Paragraph("Currency : " + currency,
                 new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /*Paragraph customerName = new Paragraph("User Name:" + UserName,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

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
                BackgroundColor = new BaseColor(255,255,255),
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
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            //tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            //{
            //    Colspan = 1,
            //    //PaddingRight = 10,
            //    Border = 0,
            //    PaddingBottom = 10,
            //    BackgroundColor = new BaseColor(67, 160, 106),
            //    HorizontalAlignment = Element.ALIGN_LEFT
            //});

            //tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            //{
            //    Colspan = 2,
            //    PaddingLeft = 10,
            //    Rowspan = 1,
            //    Border = 0,
            //    PaddingBottom = 10,
            //    PaddingTop = 5,

            //    BackgroundColor = new BaseColor(67, 160, 106),
            //    HorizontalAlignment = Element.ALIGN_RIGHT
            //});


            //tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            //{
            //    Colspan = 4,
            //    PaddingLeft = 60,
            //    Rowspan = 1,
            //    Border = 0,
            //    PaddingBottom = 15,
            //    PaddingTop = 15,
            //    HorizontalAlignment = Element.ALIGN_LEFT
            //});

            int roleid =  int.Parse(Session["user_roleid"].ToString());


            List<userlist> userlist = ds.GetAllusers(roleid);


            //tableLayout.AddCell(new PdfPCell(new Phrase("Users Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });


            ////Add header
            AddCellToHeader(tableLayout, "UserID");
            AddCellToHeader(tableLayout, "Name");
            AddCellToHeader(tableLayout, "Branch");
            AddCellToHeader(tableLayout, "Role");
            AddCellToHeader(tableLayout, "Status");


            ////Add body




            foreach (var user in userlist)
            {

                AddCellToBody(tableLayout, user.user_id.ToString());
                AddCellToBody(tableLayout, user.name);
                AddCellToBody(tableLayout, user.user_branch);
                AddCellToBody(tableLayout, user.rolename);
                AddCellToBody(tableLayout, user.user_status);

            }

            return tableLayout;
        }

        public FileResult PrintUsersLog()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("Users Log Report" + dTime.ToString("yyyyMMdd")+".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(6);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_CustomersLog_PDF(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_CustomersLog_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 20, 20, 20, 20, 10,10 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

            DateTime dTime = DateTime.Now;
            //string UserName = Session["name"].ToString();
            //string AccNo = Session["AccNo"].ToString();
            //string fromDate = Session["fromDate"].ToString();
            //string toDate = Session["toDate"].ToString();
            //string AccountNumber = AccNo.Substring(13);
            //string AccountType = data.getaccounttype(AccNo.ToString().Substring(5, 5));
            //string BranchName = data.getbranchnameenglish(AccNo.ToString().Substring(2, 3));
            //string currency = data.GetCurrencyName(AccNo.Substring(10, 3));
            //String oDate = DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy");

            //paragraphs
            Paragraph Title = new Paragraph("NBE - CPanel",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK));
            Paragraph Title2 = new Paragraph("Customers Log Report",
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK));
            //Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
            //    new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            //Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
            //    new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));

            /*Paragraph From = new Paragraph("Statement of Account From  : " + DateTime.ParseExact(fromDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy") + " To " + DateTime.ParseExact(toDate.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None).ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/
            Chunk c = new Chunk("Total of Customers Registered : " + Session["customerslogcount"].ToString(),
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            Paragraph Total = new Paragraph(c);

            /*Paragraph AccountNo = new Paragraph("Account No : " + AccountNumber,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /* Paragraph Currency = new Paragraph("Currency : " + currency,
                 new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

            /*Paragraph customerName = new Paragraph("User Name:" + UserName,
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));*/

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
                BackgroundColor = new BaseColor(255, 255, 255),
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
                BackgroundColor = new BaseColor(255, 255, 255),
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            //tableLayout.AddCell(new PdfPCell(new Phrase(Date))
            //{
            //    Colspan = 1,
            //    //PaddingRight = 10,
            //    Border = 0,
            //    PaddingBottom = 10,
            //    BackgroundColor = new BaseColor(67, 160, 106),
            //    HorizontalAlignment = Element.ALIGN_LEFT
            //});

            //tableLayout.AddCell(new PdfPCell(new Phrase(Time))
            //{
            //    Colspan = 2,
            //    PaddingLeft = 10,
            //    Rowspan = 1,
            //    Border = 0,
            //    PaddingBottom = 10,
            //    PaddingTop = 5,

            //    BackgroundColor = new BaseColor(67, 160, 106),
            //    HorizontalAlignment = Element.ALIGN_RIGHT
            //});


            //tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            //{
            //    Colspan = 4,
            //    PaddingLeft = 60,
            //    Rowspan = 1,
            //    Border = 0,
            //    PaddingBottom = 15,
            //    PaddingTop = 15,
            //    HorizontalAlignment = Element.ALIGN_LEFT
            //});




            List<UsersMangementViewModel> users = (List<UsersMangementViewModel>)Session["CustomerLog"];


            //tableLayout.AddCell(new PdfPCell(new Phrase("Users Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });


            ////Add header
            AddCellToHeader(tableLayout, "Username");
            AddCellToHeader(tableLayout, "Login Time");
            AddCellToHeader(tableLayout, "IpAddress");
            AddCellToHeader(tableLayout, "Status");
            AddCellToHeader(tableLayout, "Category");
            AddCellToHeader(tableLayout, "User ID");

            ////Add body




            foreach (UsersMangementViewModel user in users)
            {
                AddCellToBody(tableLayout, user.Username.ToString());
                AddCellToBody(tableLayout, user.LoginTime);
                AddCellToBody(tableLayout, user.IpAddress);
                AddCellToBody(tableLayout, user.UserStatus);
                AddCellToBody(tableLayout, user.Category);
                AddCellToBody(tableLayout, user.UserID);
            }

            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128,128,128) });
        }

        // Method to add single cell to the body
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }

        public ActionResult Users()
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

            if (Session["userresultF"] != null)
            {
                ViewBag.failMessage = Session["userresultF"].ToString();
                Session["userresultF"] = null;
            }
            int roleid = int.Parse(Session["user_roleid"].ToString());
            List<userlist> users = ds.GetAllusers(roleid);
            // ViewBag.UserList = dataset.Tables[0];
            return View(users);
        }

        public ActionResult PenddingUsers()
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

            int roleid = int.Parse(Session["user_roleid"].ToString());

            List<userlist> users = ds.GetAllPenddingusers(roleid);

            // ViewBag.UserList = dataset.Tables[0];
            return View(users);
        }

        //[HttpGet]
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
            userInsertModel model = new userInsertModel();
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchsForAdmins();
            model.Roles = ds.PopulatecpanelProfiles(userbranch);



            return View(model);

        }
        protected string Encrypt(string clearText)
        {
            //string EncryptionKey = "IBAZ2TWTQS77898";
            //byte[] cleFCBytes = Encoding.Unicode.GetBytes(clearText);
            //using (Aes encryptor = Aes.Create())
            //{
            //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            //    encryptor.Key = pdb.GetBytes(32);
            //    encryptor.IV = pdb.GetBytes(16);
            //    using (MemoryStream ms = new MemoryStream())
            //    {
            //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            //        {
            //            cs.Write(cleFCBytes, 0, cleFCBytes.Length);
            //            cs.Close();
            //        }
            //        clearText = Convert.ToBase64String(ms.ToArray());
            //    }
            //}
            CryptLib _crypt = new CryptLib();

            String key = "b16920894899c7780b5fc7161560a412";//CryptLib.SHA256("my secret key", 32); //32 bytes = 256 bit

            String iv = "e77886746a9b416d";
            //String iv = CryptLib.GenerateRandomIV(16); //16 bytes = 128 bits
            //string key = CryptLib.getHashSha256("my secret key", 31); //32 bytes = 256 bits
            String cypherText = _crypt.encrypt(clearText, key, iv);

            //Console.WriteLine("Plain text =" + _crypt.decrypt(cypherText, key, iv));
            return cypherText;
        }

        [HttpPost]
        public ActionResult Add(AljazeeraCPanel.Models.userInsertModel insertmodel)
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
                    if (ds.checkadminusernameavailability(insertmodel.user_name))
                    {
                        int roleid = int.Parse(Session["user_roleid"].ToString());

                        //string p = CreatePassword(8);

                        //enc_pwd = Encrypt(re);
                        //enc_pwd2 = enc_pwd;

                        insertmodel.roleidcreated = roleid;
                        //string p = MD5Hash(insertmodel.Password);




                        //insertmodel.Password = p;
                        //string p = ds.CreatePassword(8);

                        //string enc_pwd = Encrypt(p);
                        insertmodel.Password = "";

                        int _records = ds.insert(insertmodel);

                        if (_records > 0)
                        {

                            //custinfo customerinformations = ds.getcustinfo("", account);
                            //string msg = "Your User Name is: " + insertmodel.user_name + " and Password is:"+ p;

                            //string apiresponse = Connecttocore.sendotp(insertmodel.user_name ,"your pass is :", insertmodel.phone, Session["accesstoken"].ToString());
                            //JObject response = new JObject();
                            //response = JObject.Parse(apiresponse);

                            //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                            //if (responseCode == 0)
                            //{
                            //}  var response = core.SendOTPAsync(customerinformations.user_id, msg, customerinformations.user_mobile);
                            //try
                            //{

                            //    var response = Connecttocore.SensSMS(msg, insertmodel.phone, Session["accesstoken"].ToString());
                            //}
                            //catch (Exception e)
                            //{
                            //    ModelState.AddModelError("", " Something is Error ,Can Not Send Msg ");

                            //}

                            String message = "User Added successfully";
                            Session["userresult"] = message;
                            //TempData["success"] = "User Added successfully";
                            return RedirectToAction("Users", "User");

                        }

                        else
                        {
                            ModelState.AddModelError("", "Can Not Insert");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Username already taken");
                    }
                    
                }
            }
            catch (Exception e)
            {
                string message = "All Fields are required ";
                ModelState.AddModelError("", "Something is missing" + message);
            }
            userInsertModel model = new userInsertModel();
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchsForAdmins();
            model.Roles = ds.PopulatecpanelProfiles();
            return View(model);
        }


     
        // WAPT05: POST + anti-forgery. WAPT06: the pending action to approve is taken
        // from the DB's real status, never from a client-supplied value (prevents the
        // "modify status to bypass the checker approval stage" workflow bypass).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Autherize(int id)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: authoritative status from DB. Only pending/unauthorized states are approvable.
            string status = ds.getUserStatusCode(id) ?? "";
            if (!status.Equals("UA", StringComparison.OrdinalIgnoreCase) && !PendingCodes.Contains(status))
            {
                Session["userresultF"] = "This user has no pending request to authorize.";
                return RedirectToAction("PenddingUsers", "User");
            }

            //if(status.Equals("RA"))
            String message ="";
            String sts = "";
            if (status.Equals("UA"))
            {
                string p = ds.CreatePassword(8);

                // WAPT11: store a one-way hash; the plaintext temp password is only
                // sent to the user by SMS below, never persisted.
                string enc_pwd = PasswordHasher.Hash(p);
                //insertmodel.Password = enc_pwd;/
                List<userlist> info = new List<userlist>();
                info = ds.getMoreinfo(id);
                string msg = "Your User Name is: " + info[0].user_log + " and Password is:" + p;
                //update pwd

                int res = ds.Updateinfo(info[0].user_log, enc_pwd);

                if (res > 0)
                {

                    //string apiresponse = Connecttocore.sendotp(insertmodel.user_name ,"your pass is :", insertmodel.phone, Session["accesstoken"].ToString());
                    //JObject response = new JObject();
                    //response = JObject.Parse(apiresponse);

                    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                    //if (responseCode == 0)
                    //{
                    //}  var response = core.SendOTPAsync(customerinformations.user_id, msg, customerinformations.user_mobile);

                    try
                    {
                        string phone = info[0].user_mobile;

                        if (phone.Length >= 10 && phone.Substring(0, 2).Equals("09"))
                        {
                            phone = "249" + phone.Substring(1);
                        }

                        var response = Connecttocore.SensSMS(msg, phone, Session["accesstoken"].ToString());

                    }

                    catch (Exception e)
                    {
                        ModelState.AddModelError("", " Something is Error ,Can Not Send Msg ");

                    }

                    sts = "A";////
                }
                else
                {
                    ModelState.AddModelError("", " Something is Error ,Please try again  ");
                }


                   // sts = "A";
            }
            if (status.Equals("A"))
                sts = "A";
            if (status.Equals("RDA"))
                sts = "D";
            if (status.Equals("RD"))
                sts = "DE";
            if (status.Equals("RRP"))
            {

                string p = ds.CreatePassword(8);

                // WAPT11: store a one-way hash; plaintext temp password is SMS'd only.
                string enc_pwd = PasswordHasher.Hash(p);
                ////insertmodel.Password = enc_pwd;

                ////int _records = ds.insert(id);

                ////string enc_pwd = Encrypt(p);
                ///

                int recordss = ds.resetpassworduserA(id, enc_pwd);



                //string msg = "Your Password is Reset: " + " and new  Password is:" + enc_pwd;

                //string apiresponse = Connecttocore.sendotp(insertmodel.user_name ,"your pass is :", insertmodel.phone, Session["accesstoken"].ToString());
                //JObject response = new JObject();
                //response = JObject.Parse(apiresponse);

                //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                //if (responseCode == 0)
                //{
                //}  var response = core.SendOTPAsync(customerinformations.user_id, msg, customerinformations.user_mobile);

                if (recordss > 0)
                {
                    //List<userlist> list = new List<userlist>();

                    try
                    {
                        sts = "A";
                        string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                        List<userlist> info = new List<userlist>();
                        info = ds.getMoreinfo(id);

                        ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Reset Password", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());

                        string phone  = ds.getinfo(id);

                        if (phone.Length >= 10 && phone.Substring(0,2).Equals("09"))
                        {
                            phone = "249" + phone.Substring(1);
                        }

                        string msg = "Your Password is Reset: " + " and new  Password is:" + p;

                        var response = Connecttocore.SensSMS(msg, phone, Session["accesstoken"].ToString());
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", " Something is Error ,Can Not Send Msg ");

                    }
                }
            }
            // sts = "A";
            if (status.Equals("RED"))
                sts = "A";
            //{

            //string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            //List<userlist> info = new List<userlist>();
            //info = ds.getMoreinfo(id);

            //ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Edit info", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
            ////int records = ds.UpdateuserSTS(id, sts);
            //}
            int records = ds.UpdateuserSTS(id, sts);
            if (records > 0)
            {
                if (status.Equals("UA"))
                {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Autherized", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "User Autherized Successfully";
                }
                if (status.Equals("RA")) {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Activated", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "User Activated Successfully";
                     }
                if (status.Equals("RDA"))
                {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Deactivated", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "User DeActivated Successfully";
                }
                if (status.Equals("RD"))
                {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Deleted", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "User Deleted Successfully";
                }
                if (status.Equals("RRP"))
                {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Reset Password", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "Reset Password to user Successfully";
                }
                if (status.Equals("RED"))
                {
                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User updated Info", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    message = "Update User Info  Successfully";
                }
                //String message = "User Autherized Successfully";
                Session["userresult"] = message;
                return RedirectToAction("PenddingUsers", "User");
            }
            else
            {
                ModelState.AddModelError("", "Failed to Autherize");
                return View("PenddingUsers");
            }

            //model.Branches = ds.PopulateBranchsForAdmins();
            //model.Roles = ds.PopulatecpanelProfiles();
            //return View(model);
        }

        // WAPT05: state-changing action requires POST + anti-forgery token.
        // WAPT04/06: status is taken from the DB, not from the client; reject reverts
        // the pending request (never deletes).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: derive the real status from the DB, ignore any client-supplied value.
            string realCode = ds.getUserStatusCode(id);
            string sts = RevertStatusFor(realCode);
            if (sts == null)
            {
                Session["userresultF"] = "This user has no pending request to reject.";
                return RedirectToAction("PenddingUsers", "User");
            }

            string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
            List<userlist> info = new List<userlist>();
            info = ds.getMoreinfo(id);

            ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "User Rejected", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());

            int records = ds.UpdateuserSTS(id, sts);
            if (records > 0)
            {
                Session["userresult"] = "User Request Rejected Successfully";
                return RedirectToAction("PenddingUsers", "User");
            }
            else
            {
                Session["userresultF"] = "Failed to reject the request";
                return RedirectToAction("PenddingUsers", "User");
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: only currently Active accounts may be edited — checked against the DB.
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "A", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only active users can be edited.";
                return RedirectToAction("Users", "User");
            }

            userUpdateModel model;
            model = ds.getuserdata(id);
            String userbranch = Session["user_branch"].ToString();


            model.Branches = ds.PopulateBranchsForAdmins();
            model.Roles = ds.PopulatecpanelProfiles();



            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AljazeeraCPanel.Models.userUpdateModel updatemodel, int id)
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: only currently Active accounts may be edited (server-side check).
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "A", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only active users can be edited.";
                return RedirectToAction("Users", "User");
            }
            // WAPT06: bind the target id from the route, not from a hidden/posted field the
            // client could swap. Login name and first name are immutable after creation
            // and are ignored by ds.Update (see DataSource.Update).
            updatemodel.user_id = id;

            updatemodel.Roles = ds.PopulatecpanelProfiles();
            String userbranch = Session["user_branch"].ToString();


            //updatemodel.Branches = ds.PopulateBranchs(userbranch);
            updatemodel.Branches = ds.PopulateBranchs();
            var selectedBranch = updatemodel.Branches.Find(p => p.Value == updatemodel.BranchCode.ToString());
            if (selectedBranch != null)
            {
                selectedBranch.Selected = true;

            }
            var selectedRole = updatemodel.Roles.Find(p => p.Value == updatemodel.roleid.ToString());
            if (selectedRole != null)
            {
                selectedRole.Selected = true;

            }
            if (ModelState.IsValid)
            {
                 
                int _records = ds.Update(updatemodel);
                if (_records > 0)
                {
                    String message = "User Update info request is done";
                    Session["userresult"] = message;
                    //return RedirectToAction("Users", "User");

                    string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                    List<userlist> info = new List<userlist>();
                    info = ds.getMoreinfo(id);

                    ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request to Update ", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                    return RedirectToAction("Users", "User");
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

        // WAPT05: POST + anti-forgery. WAPT06: gate on the real DB status, not client input.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: a delete request may only be raised on a currently Active account.
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "A", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only active users can be deleted.";
                return RedirectToAction("Users", "User");
            }

            int records = ds.deleteuser(id);
            if (records > 0)
            {
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                List<userlist> info = new List<userlist>();
                info = ds.getMoreinfo(id);

                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request To Delete", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                return RedirectToAction("Users", "User");
            }
            else
            {
                ModelState.AddModelError("", "Can Not Delete");
                return View("Users");
            }
            // return View("Index");
        }


        //public ActionResult AuthDelete(int id)
        //{
        //    if (Session["user_name"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    if (Session["user_branch"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    int records = ds.Authdeleteuser(id);
        //    if (records > 0)
        //    {
        //        String message = "User Deleted Successfully";
        //        Session["userresult"] = message;
        //        return RedirectToAction("Users", "User");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Can Not Delete");
        //        return View("Users");
        //    }
        //    // return View("Index");
        //}

        // WAPT05: POST + anti-forgery. WAPT06: gate on the real DB status.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reset(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: password reset may only be requested on a currently Active account.
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "A", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only active users can have a password reset.";
                return RedirectToAction("Users", "User");
            }

            // WAPT07: throttle admin password-reset requests per target user (feeds an SMS):
            // max 3 / 10 minutes.
            string rlKey = "userreset:" + id;
            if (RateLimiter.IsBlocked(rlKey, 3))
            {
                Session["userresultF"] = "Too many reset requests for this user. Please try again later.";
                return RedirectToAction("Users", "User");
            }
            RateLimiter.RegisterAttempt(rlKey, 10);
            //string p = ds.CreatePassword(8);

            //string enc_pwd = Encrypt(p);
            //insertmodel.Password = enc_pwd;

            //int _records = ds.insert(insertmodel);

            //if (_records > 0)
            //{

            //    //custinfo customerinformations = ds.getcustinfo("", account);
            //    string msg = "Your User Name is: " + insertmodel.user_name + " and Password is:" + p;

            //    //string apiresponse = Connecttocore.sendotp(insertmodel.user_name ,"your pass is :", insertmodel.phone, Session["accesstoken"].ToString());
            //    //JObject response = new JObject();
            //    //response = JObject.Parse(apiresponse);

            //    //int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
            //    //if (responseCode == 0)
            //    //{
            //    //}  var response = core.SendOTPAsync(customerinformations.user_id, msg, customerinformations.user_mobile);
            //    try
            //    {

            //        var response = Connecttocore.SensSMS(msg, insertmodel.phone, Session["accesstoken"].ToString());
            //    }
            //    catch (Exception e)
            //    {
            //        ModelState.AddModelError("", " Something is Error ,Can Not Send Msg ");

            //    }

            //string p = ds.CreatePassword(8);

            //string enc_pwd = Encrypt(p);
            ////insertmodel.Password = enc_pwd;

            ////int _records = ds.insert(id);

            ////string enc_pwd = Encrypt(p);
            int records = ds.resetpassworduser(id );
            if (records > 0)
            {
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                List<userlist> info = new List<userlist>();
                info = ds.getMoreinfo(id);

                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request To Reset", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                //string msg = "Your Password is Reset: " + " and new  Password is:" + enc_pwd;

                ////string apiresponse = Connecttocore.sendotp(insertmodel.user_name ,"your pass is :", insertmodel.phone, Session["accesstoken"].ToString());
                ////JObject response = new JObject();
                ////response = JObject.Parse(apiresponse);

                ////int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
                ////if (responseCode == 0)
                ////{
                ////}  var response = core.SendOTPAsync(customerinformations.user_id, msg, customerinformations.user_mobile);
                //try
                //{
                //string phone = ds.getinfo(id);

                //    var response = Connecttocore.SensSMS(msg, phone, Session["accesstoken"].ToString());
                //}
                //catch (Exception e)
                //{
                //    ModelState.AddModelError("", " Something is Error ,Can Not Send Msg ");

                //}

                String message = "User Rest Password request is done";
                Session["userresult"] = message;
                return RedirectToAction("Users", "User");
            }
            else
            {
                ModelState.AddModelError("", "Can Not Reset Password");
                return View("Users");
            }
            // return View("Index");
        }

        //public ActionResult AuthReset(int id)
        //{
        //    if (Session["user_name"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    if (Session["user_branch"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    int records = ds.Authresetpassworduser(id);
        //    if (records > 0)
        //    {
        //        String message = "User Reset Password  Successfully";
        //        Session["userresult"] = message;
        //        return RedirectToAction("Users", "User");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Can Not Reset Password");
        //        return View("Users");
        //    }
        //    // return View("Index");
        //}


        // WAPT05: POST + anti-forgery. WAPT06: gate on the real DB status.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deactivate(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: deactivation may only be requested on a currently Active account.
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "A", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only active users can be deactivated.";
                return RedirectToAction("Users", "User");
            }

            int records = ds.deactive(id);
            if (records > 0)
            {
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                List<userlist> info = new List<userlist>();
                info = ds.getMoreinfo(id);

                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request to Deactivate", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                String message = "User DeActivation Request is Done";
                Session["userresult"] = message;
                return RedirectToAction("Users", "User");
            }
            else
            {
                ModelState.AddModelError("", "Can not deactivate user");
                return View("Users");
            }
            // return View("Index");
        }

        // WAPT05: POST + anti-forgery. WAPT06: gate on the real DB status.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // WAPT06: activation may only be requested on a currently Deactivated account.
            string realCode = ds.getUserStatusCode(id);
            if (!string.Equals(realCode, "D", StringComparison.OrdinalIgnoreCase))
            {
                Session["userresultF"] = "Action not allowed: only deactivated users can be activated.";
                return RedirectToAction("Users", "User");
            }

            int records = ds.Active(id);
            if (records > 0)
            {
                string adminbranch = ds.getbranchnameenglish(Session["user_branch"].ToString());
                List<userlist> info = new List<userlist>();
                info = ds.getMoreinfo(id);

                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Request To Activate", info[0].user_log + " - " + info[0].user_name, DateTime.Now.ToString());
                String message = "User Activation Request is Done";
                Session["userresult"] = message;
                return RedirectToAction("Users", "User");
            }
            else
            {
                ModelState.AddModelError("", "Can not Activate user");
                return View("Users");
            }
            // return View("Index");
        }

        //public ActionResult AuthDeactivate(int id)
        //{
        //    if (Session["user_name"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    if (Session["user_branch"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    int records = ds.Authdeactive(id);
        //    if (records > 0)
        //    {
        //        String message = "User Deactiveted Successfully";
        //        Session["userresult"] = message;
        //        return RedirectToAction("Users", "User");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Can not deactivate user");
        //        return View("Users");
        //    }
        //    // return View("Index");
        //}

        //public ActionResult AuthActivate(int id)
        //{
        //    if (Session["user_name"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    if (Session["user_branch"] == null)
        //    {
        //        return RedirectToAction("Login", "Login");
        //    }
        //    int records = ds.AuthActive(id);
        //    if (records > 0)
        //    {
        //        String message = "User Activeted Successfully";
        //        Session["userresult"] = message;
        //        return RedirectToAction("Users", "User");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError("", "Can not Activate user");
        //        return View("Users");
        //    }
        //    // return View("Index");
        //}


        public ActionResult smspassword(string password, string account)
        {
            custinfo customerinformations = ds.getcustinfo("", account);
            string msg = "Your Account temporery password is : "+password;
            //string msg = password;
            //Clipboard.SetDataObject(msg, true);
            //string msg = "تم إعادة تعين كلمه المرور الخاص بك. ويمكنك الدخول عن طريق كلمة السر : " + password + " .";
            //string response = core.sendpredefinedsms(customerinformations.user_id, password, "3", customerinformations.user_mobile);
            var response = core.SendOTPAsync( msg, customerinformations.user_mobile);

            //JObject jobj = new JObject();
            //jobj = JObject.Parse(response);
            //dynamic result = jobj;

            //var errorCode = result.errorcode;
            //var errormsg = result.errormsg;
            var Status = 1;  //result.status;

            if (Status == 1)
            {
                string custname = customerinformations.user_name;
                string customeraccount = account;
                string usershorSthand = account;  //"23" + customeraccount.Substring(3, 3) + customeraccount.Substring(13);
                string adminbranch = Session["branch_namee"].ToString();   //ds.getbranchnameenglish(Session["user_branch"].ToString());
                ds.insertadminslog(Session["UserId"].ToString(), Session["user_name"].ToString(), adminbranch, Session["user_roleid"].ToString(), Session["user_status"].ToString(), "Reset password sent to customer vis sms", usershorSthand + " - " + custname, DateTime.Now.ToString());

                TempData["Success"] = true;
                ViewBag.ResponseStat = "Successful";
                ViewBag.ResponseMSG = "Password sent to customer via sms successfully";
                ViewBag.SuccessMessage = "Password sent to customer via SMS.";
                TempData["successful"] = "Password sent to customer via sms successfully";
                return RedirectToAction("ResetCust");
            }
            else
            {
                TempData["Success"] = true;
                ViewBag.ResponseStat = "Not Successful";
                ViewBag.ResponseMSG = "Faild to send password sms, please try again.";
                ViewBag.SuccessMessage = "Message was not sent to customer, Please try again.";
                TempData["failed"] = "Failed to send password sms, please try again.";
                return RedirectToAction("ResetCust");
            }
        }
    }


}
