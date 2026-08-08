using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCBCPanel.Models
{
    public class BranchModel
    {
        [Required]
        [Display(Name = "Branch Code")]
        public string branch_code { get; set; }
        [Required]
        [Display(Name = "Branch Name")]
        public string branch_name { get; set; }
        [Required]
        [Display(Name = "Branch Status")]
        public string branch_status { get; set; }
        [Display(Name = "Branch Code No")]
        public string branch_code_no { get; set; }
        [Required]
        [Display(Name = "Branch Name Arabic")]
        public string branch_name_arabic { get; set; }
        [Display(Name = "Branch Database Link")]
        public string branch_db_link { get; set; }
    }
}