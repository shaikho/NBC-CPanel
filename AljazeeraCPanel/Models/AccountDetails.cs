using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class AccountDetails
    {
        public string Account_No { get; set; }
        public string Account_Type_Code { get; set; }
        public string Currency_Code { get; set; }
        public string Branch_Code { get; set; }
        public string IBAN { get; set; }
        public string user_mobile { get; set; }
        public string customer_en { get; set; }
        public string customer_ar { get; set; }

        public string address { get; set; }
        public string rim { get; set; }
        public JArray phones { get; set; }
        public string phone_no { get; set; }
    }
}