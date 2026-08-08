using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using System.Web.Mvc;
namespace AljazeeraCPanel.Models
{
    public class CustomerRegpersonalinfo
    {
        public List<SelectListItem> Profiles { get; set; }
        [Required(ErrorMessage = "Please Enter User Name")]
        [Display(Name = "User name")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter User Address")]
        [Display(Name = "User Address")]
        public string Address { get; set; }
        [RegularExpression(".+@.+\\..+", ErrorMessage = "Please Enter Correct Email Address")]
        [Display(Name = "User Email")]
        public string Email { get; set; }
        [Display(Name = "User Profile")]
        public string Profile { get; set; }
        //[Required]
        public String profileCode { get; set; }
        [Required(ErrorMessage = "Phone number required")]
        [Display(Name = "User Phone number")]
        public string phonenumber { get; set; }

        [Required(ErrorMessage = "National number required")]
        [Display(Name = "National number")]
        public string NID { get; set; }
        [Display(Name = "First Name")]
        public string First_Names { get; set; }
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Father Name")]
        public string Father_Name { get; set; }
        [Display(Name = "Grand Father Name")]
        public string Grand_Father_Name { get; set; }
        [Display(Name = "Gre Gra Father Name")]
        public string Gre_Gra_Father_Name { get; set; }

        [Display(Name = "Mother Name")]
        public string Mother_Name { get; set; }

        [Display(Name = "MOT Father Name")]
        public string Mot_Father_Name { get; set; }

        [Display(Name = "MOT GRA Father Name")]
        public string Mot_Gra_Father_Name { get; set; }

        [Display(Name = "MOT GRE GRA Father Name")]
        public string Mot_Gre_Gra_Father_Name { get; set; }
        [Display(Name = "Birth Date")]
        public string Birth_date { get; set; }
        [Display(Name = "Identity Number")]
        public string Identity_Number { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Full Customer Name")]
        public string Full_Customer { get; set; }
        [Display(Name = "Full Mother Name")]
        public string Full_Mother { get; set; }
        [Display(Name = "Full Father Name")]
        public string Full_Father { get; set; }

        [Display(Name = "Photograph")]
        public Image Photograph { get; set; }
        public Boolean data { get; set; }

    }

    public class ImageResponse {
        public string ImageBase64 { get; set; } 
    }
}