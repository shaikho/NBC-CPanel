using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class Comment
    {
        [Display(Name = "ID")]
        public string ID { get; set; }
        [Display(Name = "Dispute ID")]
        public string Dispute_id { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Comment")]
        public string Comment_text { get; set; }
        [Display(Name = "Creator")]
        public string User_entry { get; set; }
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Display(Name = "Action")]
        public string Action { get; set; }
    }
}