using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class Transferlimit
    {
        [Display(Name = "id")]
        public string tran_id { get; set; }
        [Display(Name = "Service Name")]
        public string servicename { get; set; }
        [Display(Name = "Amount Limit")]
        public string amount_limit { get; set; }
        [Display(Name = "Daily Limit")]
        public string daily_limit { get; set; }
        [Display(Name = "Number of transactions")]
        public string number_limit { get; set; }
    }
}