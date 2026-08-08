using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace AljazeeraCPanel.Models
{
    public class ChqRequest
    {
        public int request_id { get; set; }
        [Display(Name = "Customer Account")]
        public string accountmap { get; set; }
        [Display(Name = "Customer Name")]
        public string name { get; set; }

        [Display(Name = "Book Size")]
        public string booksize { get; set; }
        [Display(Name = "Request Date")]
        public string date { get; set; }
        [Display(Name = "Request Status")]
        public string status { get; set; }

        [Display(Name = "User ID")]
        public string userid { get; set; }

        [Display(Name = "Request Date")]
        public string reqdate { get; set; }

        [Display(Name = "Request Status")]
        public string reqsts { get; set; }

        [Display(Name = "Account")]
        public string act { get; set; }

        [Display(Name = "Branch Name")]
        public string branchname { get; set; }

        [Display(Name = "Branch Code")]
        public string branchcode { get; set; }

    }
}