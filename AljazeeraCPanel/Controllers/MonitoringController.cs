using cpanel.Models;
using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class MonitoringController : Controller
    {
        DataSource ds = new DataSource();
        Connecttocore connect = new Connecttocore();
        // GET: Monitoring
        public ActionResult Monitoring()
        {
            List<CustomerTransferReportViewModel> creditapitransactions = ds.GetCurrentCreditAPITransaction();
            foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.PAN = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.CustomerName = requestdata.customerName;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words = word.Split(':');
                transaction.FT = words[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }

            Session["creditapitransactions"] = creditapitransactions;

            string response = Connecttocore.GetHeartBeat();
            JObject jobj = new JObject();
            jobj = JObject.Parse(response);
            dynamic result = jobj;
            List<biller> connections = new List<biller>();
            connections.Add(new biller
            {
                name = result.B1.Name,
                connectivity = result.B1.status,
                last_cnnection = result.B1.Date
            });
            connections.Add(new biller
            {
                name = result.B2.Name,
                connectivity = result.B2.status,
                last_cnnection = result.B2.Date
            });
            connections.Add(new biller
            {
                name = result.B3.Name,
                connectivity = result.B3.status,
                last_cnnection = result.B3.Date
            });
            connections.Add(new biller
            {
                name = result.B4.Name,
                connectivity = result.B4.status,
                last_cnnection = result.B4.Date
            });

            //List<biller> connections = new List<biller>();
            //connections.Add(new biller
            //{
            //    name = "CoreBank",
            //    connectivity = "Connected",
            //    last_cnnection = "-"
            //});
            //connections.Add(new biller
            //{
            //    name = "NMSF",
            //    connectivity = "Connected",
            //    last_cnnection = "-"
            //});
            //connections.Add(new biller
            //{
            //    name = "EPORT",
            //    connectivity = "Not Connected",
            //    last_cnnection = "-"
            //});
            //connections.Add(new biller
            //{
            //    name = "EBS",
            //    connectivity = "Connected",
            //    last_cnnection = "-"
            //});
            ViewData["connections"] = connections;
            return View();
        }

        public JsonResult AJAXMonitoring()
        {
            List<CustomerTransferReportViewModel> creditapitransactions = ds.GetCurrentCreditAPITransaction();
            foreach (CustomerTransferReportViewModel transaction in creditapitransactions)
            {
                dynamic requestdata = JObject.Parse(transaction.TranFullReq);
                dynamic responsedata = JObject.Parse(transaction.TranFullResp);
                transaction.TranReqAmount = requestdata.tranamount;
                transaction.PAN = requestdata.PAN;
                transaction.TranFromAccount = requestdata.Fromaccount;
                transaction.CustomerName = requestdata.customerName;
                transaction.ResponseStatus = responsedata.responseStatus;
                transaction.RRN = responsedata.RRN;
                string word = responsedata.status;
                string[] words = word.Split(':');
                transaction.FT = words[1];
                string amorpm = transaction.TranDate.Substring(transaction.TranDate.Length - 2);
                transaction.TranDate = transaction.TranDate.Substring(0, 15) + " " + amorpm;
            }

            Session["creditapitransactions"] = creditapitransactions;

            JsonResult data = Json(new { data = creditapitransactions }, JsonRequestBehavior.AllowGet);

            return data;
        }

        public ActionResult Update()
        {
            List<LatestTransactions> transactions = new List<LatestTransactions>();
            transactions = ds.getAllTransactions();
            ViewData["transactions"] = transactions;
            return PartialView(transactions);
        }
    }
}