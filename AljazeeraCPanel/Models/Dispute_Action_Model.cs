using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class Dispute_Action_Model
    {
        [Display(Name = "Action ID")]
        public string id { get; set; }
        [Display(Name = "Action")]
        public string action { get; set; }
        [Display(Name = "Arabic Action")]
        public string action_arabic { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Action Code")]
        public string action_code { get; set; }
        [Display(Name = "Action Status")]
        public string action_status { get; set; }
    }
}