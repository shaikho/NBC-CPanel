using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FCBCPanel.Models
{
    public class CurrencyModel
    {
        [Required]
        [Display(Name = "Currency Code")]
        public string currency_code { get; set; }
        [Required]
        [Display(Name = "Currency Name")]
        public string currency_name { get; set; }
        [Required]
        [Display(Name = "Currency Summary")]
        public string currency_summary { get; set; }
        [Required]
        [Display(Name = "Currency Status")]
        public string currency_status { get; set; }
    }
}