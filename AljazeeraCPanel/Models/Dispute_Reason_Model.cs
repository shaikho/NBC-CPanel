using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class Dispute_Reason_Model
    {
        [Display(Name = "Reason ID")]
        public string id { get; set; }
        [Display(Name = "Reason")]
        public string reason { get; set; }
        [Display(Name = "Arabic Reason")]
        public string reason_arabic { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Reason Code")]
        public string reason_code { get; set; }
        [Display(Name = "Reason Status")]
        public string reason_status { get; set; }
    }
}