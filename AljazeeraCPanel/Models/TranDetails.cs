using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{
    public class TranDetails
    {
        public TranDetails (string p1, string p2, string p3, string p4, string p5, string p6, string p7, string p8, string p9)
        {
            // TODO: Complete member initialization
            this.SysDateTime = p1;
            this.UserID = p2;
            this.FromAcc = p3;
            this.FromBranch = p4;
            this.ToAcc = p5;
            this.ToBranch = p6;
            this.Amount = p7;
            this.RRN = p8;
            this.Status = p9;
        }


        public List<SelectListItem> Branches { get; set; }
        /*public List<SelectListItem> AccTypes { get; set; }*/
        /*public List<SelectListItem> Currencies { get; set; }*/
        public List<SelectListItem> catgories { get; set; }

        public List<SelectListItem> transactions_statuses { get; set; }

        public string SysDateTime { get; set; }
    public string UserID { get; set; }
    public string FromAcc { get; set; }
    public string FromBranch { get; set; }
    public string ToAcc { get; set; }
    public string ToBranch { get; set; }
    public string Amount { get; set; }
        public string RRN { get; set; }
        public string Status { get; set; }


        public String BranchCode { get; set; }

        public String CategoryCode { get; set; }

    }
}