using iTextSharp.text;
using iTextSharp.text.pdf;
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

namespace AljazeeraCPanel.Controllers
{
    public class ActionsLogController : Controller
    {
        DataSource data = new DataSource();
        // GET: ActionsLog
        public ActionResult ActionsLog()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            List<ActionsLogViewModel> actions = new List<ActionsLogViewModel>();
            
            actions = data.getactionslog();

            return View(actions);
        }

        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created 
            string strPDFFileName = string.Format("UsersActionReport" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns
            PdfPTable tableLayout = new PdfPTable(7);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table

            //file will created in this path
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF 
            doc.Add(Add_Content_To_PDF2(tableLayout));

            // Closing the document
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF2(PdfPTable tableLayout)
        {

            float[] headers = { 12, 15, 15, 20, 25, 20, 20 };  //Header Widths
            tableLayout.SetWidths(headers);        //Set the pdf headers
            tableLayout.WidthPercentage = 95;       //Set the PDF File witdh percentage
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top
            //prepering pdf dataset
            List<ActionsLogViewModel> actions = new List<ActionsLogViewModel>();
            actions = data.getactionslog();

            DateTime datetimenow = DateTime.Now;
            tableLayout.AddCell(new PdfPCell(new Phrase("User Actions Report", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });
            tableLayout.AddCell(new PdfPCell(new Phrase(datetimenow.ToString(), new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) { Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER });


            ////Add header
            //AddCellToHeader2(tableLayout, "To Account Number");
            //AddCellToHeader2(tableLayout, "Amount");
            //AddCellToHeader2(tableLayout, "From File");
            //AddCellToHeader2(tableLayout, "Salary Customer");
            //AddCellToHeader2(tableLayout, "Account Number");
            //AddCellToHeader2(tableLayout, "Salary Refrence");

            AddCellToHeader2(tableLayout, "User id");
            AddCellToHeader2(tableLayout, "User name");
            AddCellToHeader2(tableLayout, "User Branch");
            AddCellToHeader2(tableLayout, "User Role");
            AddCellToHeader2(tableLayout, "Action");
            AddCellToHeader2(tableLayout, "Affected User");
            AddCellToHeader2(tableLayout, "Date - Time");

            ////Add body




            foreach (var action in actions)
            {
                AddCellToBody2(tableLayout, action.user_id);
                AddCellToBody2(tableLayout, action.user_name);
                AddCellToBody2(tableLayout, action.user_branch);
                AddCellToBody2(tableLayout, action.user_role);
                AddCellToBody2(tableLayout, action.action);
                AddCellToBody2(tableLayout, action.action_on_user);
                AddCellToBody2(tableLayout, action.timedate);
            }

            return tableLayout;
        }

        // Method to add single cell to the Header
        private static void AddCellToHeader2(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.WHITE))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 128, 128) });
        }

        // Method to add single cell to the body
        private static void AddCellToBody2(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK))) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255) });
        }

        // end of pdf preperation
    }
}