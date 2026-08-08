using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCBCPanel.Models
{
    public class AccountTypeModel
    {
        [Required]
        [Display(Name = "Account Type Code")]
        public string account_type_code { get; set; }
        [Required]
        [Display(Name = "Account Type")]
        public string account_type { get; set; }
        [Required]
        [Display(Name = "Account Type Status")]
        public string account_type_status { get; set; }
        [Display(Name = "Account Type No")]
        public string account_type_no { get; set; }
        [Required]
        [Display(Name = "Account Type Arabic")]
        public string account_type_arabic { get; set; }
    }
}