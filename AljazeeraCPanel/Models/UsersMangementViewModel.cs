using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class UsersMangementViewModel
    {
        [Required(ErrorMessage = "Enter Username Please")]
        public String Username { get; set; }
        public String SuccessfulLogin { get; set; }
        public String FailedLogin { get; set; }

        public String IpAddress { get; set; }
        public String LoginTime { get; set; }
        public String UserPass { get; set; }
        public String UserLogin { get; set; }
        public String LoginStatus { get; set; }

        public String UserID { get; set; }
        public String UserStatus { get; set; }
        public String Category { get; set; }

        [Display(Name = "From Date")]
        public string fromdate { get; set; }
        [Display(Name = "To Date")]
        public string todate { get; set; }
        public string accountcuslog { get; set; }
    }
}