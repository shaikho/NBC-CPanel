using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class req_res_model
    {
        [Display(Name = "ID")]
        public string ID { get; set; }
        [Display(Name = "Request Data")]
        public string Request_Data { get; set; }
        [Display(Name = "Response Data")]
        public string Response_Data { get; set; }
        [Display(Name = "Connection Response")]
        public string CONNECTION_RESPONSE { get; set; }
        [Display(Name = "Request Date")]
        public string REQUEST_DATE { get; set; }
        [Display(Name = "REsponse Date")]
        public string RESPONSE_DATE { get; set; }

        public string TRAN_Data { get; set; }
        public string Biller_ID { get; set; }
        public string Biller_Name { get; set; }
        public string BILLER_VOUCHER { get; set; }
        public string BILL_AMOUNT { get; set; }
        public string BBL_BILLERRESPONSE { get; set; }
        public string BBL_BNKREFRENCE { get; set; }
        public string BBL_SYS_TRACENO { get; set; }
        public String Biller { get; set; }

        public List<string> Billers { get; set; }

    }
}