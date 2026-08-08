using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class EPortReceipt
    {
        public string tran_id { get; set; }
        public string tran_paycustomercode { get; set; }
        public string tran_payserviceid { get; set; }
        public string tran_bankode { get; set; }
        public string tran_amount { get; set; }
        public string tran_customername { get; set; }
        public string tran_eportresponse { get; set; }
        public string tran_plcno { get; set; }
        public string tran_curr { get; set; }
        public string tran_bankvoucher { get; set; }
        public string tran_service { get; set; }
    }
}