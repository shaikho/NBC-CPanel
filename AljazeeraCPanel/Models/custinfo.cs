using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{
    public class custinfo
    {
        public List<SelectListItem> Profiles { get; set; }


        [Display(Name = "User Catogery")]
        public string role_id { get; set; }

        public string profileCode { get; set; }
        public string user_id { get; set; }
        [Display(Name = "Customer Name")]
        public string user_name { get; set; }
        [Display(Name = "Customer Username")]
        public string user_log { get; set; }
        public string user_pwd { get; set; }
        [Display(Name = "Customer Email")]
        public string user_email { get; set; }
        [Display(Name = "Customer Mobile")]
        public string user_mobile { get; set; }
        [Display(Name = "Customer Address")]
        public string user_adrs { get; set; }
        [Display(Name = "Customer Profile")]
        public string name { get; set; }
        [Display(Name = "Customer Status")]
        public string status { get; set; }
        [Display(Name = "Customer Type")]
        public string type { get; set; }
        [Display(Name = "Creation Date")]
        public string creation_date { get; set; }
        [Display(Name = "Created By")]
        public string created_by { get; set; }

        [Display(Name = "RIM")]
        public string rim { get; set; }
        public string lblconfirm { get; set; }
        public string catgory { get; set; }
        public List<channel> Channels { get; set; }
        public String[] SelectedChannelsID { get; set; }
        [Display(Name = "Channel")]
        public int Channel { get; set; }
        public List<channel> SelectedChannels { get; set; }
        public String def_account { get; set; }

    }
}