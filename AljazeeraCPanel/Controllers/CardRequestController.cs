using iTextSharp.text;
using iTextSharp.text.pdf;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class CardRequestController : Controller
    {
        DataSource ds = new DataSource();

        // GET: CardRequest
        public ActionResult CardRequest()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["chqmessage"] != null)
            {
                ViewBag.SuccessMessage = Session["chqmessage"].ToString();
                Session["chqmessage"] = null;
            }
            List<ChqRequest> cards = new List<ChqRequest>();
            cards = ds.Cardrequest(Session["user_branch"].ToString());
            return View(cards);
        }

        public ActionResult Accept(int id)
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            String message = "", sts = "A";
            int response = ds.updatecardsts(id, sts);

            if (!response.Equals(-1))
            {


                message = "Request Accpet Successfully";
                Session["chqmessage"] = message;

            }
            else
            {
                message = "Sorry You Cannot process now, please try later  ";
                Session["chqmessage"] = message;

            }
            return RedirectToAction("CardRequest");

        }

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

            String message = "", sts = "R";
            int response = ds.updatecardsts(id, sts);
            if (!response.Equals(-1))
            {


                message = "Request Reject Successfully";
                Session["chqmessage"] = message;

            }
            else
            {
                message = "Sorry You Cannot process now, please try later  ";
                Session["chqmessage"] = message;

            }
            return RedirectToAction("CardRequest");
        }

   

        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("AtmCardReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
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

            float[] headers = { 50, 24, 45, 35, 30, 30 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 100;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top

            string adminbranch = Session["user_branch"].ToString();

            List<ChqRequest> cheques = ds.Cardrequest(Session["user_branch"].ToString()); // ds.ChqrequestReport(adminbranch);

            string branchname = ds.getbranchnameenglish(adminbranch);
            //tableLayout.AddCell(new PdfPCell(new Phrase(branchname, new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });
            //tableLayout.AddCell(new PdfPCell(new Phrase("Cheques Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });

            DateTime dTime = DateTime.Now;

            //paragraphs
            Paragraph Title = new Paragraph("JS Bank",
                new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Title2 = new Paragraph("Atm Card Request Report For " + branchname,
               new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Date = new Paragraph("Date: " + dTime.ToString("dd-MMM-yyyy"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            Paragraph Time = new Paragraph("TIME:" + dTime.ToString("HH:mm:ss"),
                new Font(Font.FontFamily.HELVETICA, 5, 1, iTextSharp.text.BaseColor.WHITE));
            //Chunk c = new Chunk("Total of Customers Registered : " + Session["totalcustomer"].ToString(),
            //    new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE));

            // Paragraph Total = new Paragraph(c);
            //Adding Cells
            Paragraph empty = new Paragraph("\n\n",
                new Font(Font.FontFamily.HELVETICA, 8, 1, new BaseColor(0, 0, 0)));
            //Adding Cells
            tableLayout.AddCell(new PdfPCell(new Phrase(Title))
            {
                Colspan = 6,
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
                Colspan = 6,
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
                Colspan = 3,
                PaddingRight = 10,
                Border = 0,
                PaddingBottom = 10,
                BackgroundColor = new BaseColor(54, 54, 77),
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

                BackgroundColor = new BaseColor(54, 54, 77),
                HorizontalAlignment = Element.ALIGN_RIGHT
            });


            tableLayout.AddCell(new PdfPCell(new Phrase(empty))
            {
                Colspan = 6,
                PaddingLeft = 60,
                Rowspan = 1,
                Border = 0,
                PaddingBottom = 15,
                PaddingTop = 15,
                HorizontalAlignment = Element.ALIGN_LEFT
            });





            //string branchname = ds.getbranchnameenglish(adminbranch);
            //tableLayout.AddCell(new PdfPCell(new Phrase(branchname, new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });
            //tableLayout.AddCell(new PdfPCell(new Phrase("Cheques Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });


            ////Add header
            //AddCellToHeader(tableLayout, "ID");

            AddCellToHeader(tableLayout, "Request Date");
            AddCellToHeader(tableLayout, "User ID");
            AddCellToHeader(tableLayout, "Account");
            AddCellToHeader(tableLayout, "Branch Name");
            AddCellToHeader(tableLayout, "Book Size");
            AddCellToHeader(tableLayout, "Request Status");
            //AddCellToHeader(tableLayout, "Account");
            ////Add body
            foreach (var cheque in cheques)
            {
                //AddCellToBody(tableLayout, cheque.request_id;
                AddCellToBody(tableLayout, cheque.reqdate);
                AddCellToBody(tableLayout, cheque.userid);
                AddCellToBody(tableLayout, cheque.act);
                AddCellToBody(tableLayout, cheque.branchname);
                AddCellToBody(tableLayout, cheque.booksize);
                AddCellToBody(tableLayout, cheque.reqsts);
            }
            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(54, 54, 77) });
        }

        // Method to add single cell to the body
        //private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        //{
        //    string fontpath = Environment.GetEnvironmentVariable("SystemRoot") + "\\fonts\\times.ttf";
        //    BaseFont basefont = BaseFont.CreateFont(fontpath, BaseFont.IDENTITY_H, true);
        //    tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(basefont, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        //}

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



            //tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }
    }
}