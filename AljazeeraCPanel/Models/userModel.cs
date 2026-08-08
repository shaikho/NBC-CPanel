using AljazeeraCPanel.Context;
using SIBCPanel.Context;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{


    public class userInsertModel
    {
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> Branches { get; set; }
        
        
        [Required]
        [Display(Name = "First Name:")]
        public string name { get; set; }
        [Required]
        [Display(Name = "Login Name:")]
        public string user_name { get; set; }
        
        [Display(Name = "Role Name:")]
        public string rolename { get; set; }
        [Display(Name = "User PhoneNo:")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed")]
        public string phone { get; set; }

        public string Password { get; set; }
        [Display(Name = "User Email:")]
        public string email { get; set; }
        [Display(Name = "Branch Name:")]

        public string branch_name { get; set; }
        [Required]
        public string roleid { get; set; }
        [Required]
        public string BranchCode { get; set; }

        public int roleidcreated { get; set; }


    }

    public class userlist {
        public int user_id { get; set; }

        public string phone { get; set; }


        public string pass { get; set; }
        [Display(Name =  "Name")]
        public string name { get; set; }

        [Display(Name = "Login Name")]
        public string user_log { get; set; }

        [Display(Name = "User Mobile")]
        public string user_mobile { get; set; }

        [Display(Name = "User Name")]
        public string user_name { get; set; }
        [Display(Name = "Role")]
        public string rolename { get; set; }
           
        [Display(Name = "Branch")]

        public string user_branch { get; set; }

        [Display(Name = "User Status")]

        public string user_status { get; set; }
        public string user_status_code { get; set; }

    }

    public class userUpdateModel
    {
       
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> Roles { get; set; }
        DataSource ds = new DataSource();
           public int user_id { get; set; }
        [Required]
        [Display(Name = "First Name:")]
        public string name { get; set; }
         [Required]
        [Display(Name = "Login Name:")]
        public string user_name { get; set; }
          
         [Display(Name = "Role Name:")]
        public string rolename {get;set;}

        [Display(Name = "User PhoneNo:")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed")]
        public string phone { get; set; }

        public string Password { get; set; }
        [Display(Name = "User Email:")]
        public string email { get; set; }

        [Display(Name = "Branch Name:")]
       
        public string branch_name {get;set;}
         [Required]
         public string roleid { get; set; }
         [Required]
         public string BranchCode { get; set; }
    }
}