using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class Limit
    {
        [Required]
        [Display(Name = "Maximum number of transactions per day")]
        public int Transaction_per_day { get; set; }

        [Required]
        [Display(Name = "Maximum Transaction amount")]
        public double Transaction_amount { get; set; }

        [Required]
        [Display(Name = "Maximum daily limit")]
        public double Transactions_accumulation { get; set; }

        [Required]
        [Display(Name = "Service Name")]
        public string service_name { get; set; }
  
        [Display(Name = "Fees")]
        public int Fees { get; set; }
     
        [Display(Name = "Tax")]
        public int Tax { get; set; }

        [Display(Name = "Flag")]
        public int flag { get; set; }
       
        [Display(Name = "Service Id")]
        public int serviceid { get; set; }
    }
}