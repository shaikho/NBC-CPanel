using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AljazeeraCPanel.Models
{
    public class LatestTransactions
    {
        public int TranId { get; set; }
        public string TranName { get; set; }
        public string TranStatus { get; set; }

        public string TranResult { get; set; }
        public string tranreq { get; set; }
        public string trandate { get; set; }
        public string tranamount {get;set;}

    }
}