using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{
    public class CustomerTransferReportViewModel
    {
        [Display(Name = "Transaction ID")]
        public string TranID { get; set; }
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> AccTypes { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public List<SelectListItem> catgories { get; set; }
        /*public List<channel> Channels { get; set; }*/

        public string SUBNO { get; set; }
        public string SUBGL { get; set; }
        [Display(Name = "Customer Branch")]
        public string Branch { get; set; }
        //[Required]
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }
        //[Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        //[Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
       
        [Display(Name = "User mobile")]
        public string UserMobile { get; set; }
        [Display(Name = "Transaction Name")]
        public string TranName { get; set; }

        [Display(Name = "Amount")]
        public string TranAmount { get; set; }

        
        public string Account_Info { get; set; }
        //[Required]
        [Display(Name = "Branch Code")]
        public String BranchCode { get; set; }

        //[Required]
        [Display(Name = "Account type code")]
        public String AccountTypecode { get; set; }

        //[Required]
        [Display(Name = "Currency code")]
        public String CurrencyCode { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        public string alsocustomername { get; set; }
        [Required]
        [Display(Name = "Operator Comment")]
        public string Comment { get; set; }

        public String CustomerID { get; set; }
        [Display(Name = "Transaction Date")]
        public String TranDate { get; set; }
        [Display(Name = "Transaction full request")]
        public String TranFullReq { get; set; }
        [Display(Name = "User ID")]
        public String UserId { get; set; }
        [Display(Name = "Username")]
        public String User_log { get; set; }
        [Display(Name = "Transaction full response")]
        public String TranFullResp { get; set; }


        [Display(Name = "Transaction Result")]
        public String TranResult { get; set; }

        [Display(Name = "From Account")]
        public String TranFromAccount{ get; set; }

        [Display(Name = "To Account")]
        public String TranToAccount { get; set; }

        [Display(Name = "Amount")]
        public String TranReqAmount { get; set; }
        public String TranReqAmountnew { get; set; }

        [Display(Name = "Transaction Status")]
        public String TranStatus { get; set; }

        [Display(Name = "From Date")]
        public String FromDate { get; set; }
        [Display(Name = "To Date")]
        public String ToDate { get; set; }
        [Display(Name = "PAN")]
        public String PAN { get; set; }
        [Display(Name = "Response Status")]
        public String ResponseStatus { get; set; }
        [Display(Name = "RRN")]
        public String RRN { get; set; } = "N/A";
        [Display(Name = "FT")]
        public String FT { get; set; } = "N/A";
        [Display(Name = "Narriation")]
        public String Narriation { get; set; } = "N/A";

        [Display(Name = "Biller id")]
        public String bbl_id { get; set; }
        [Display(Name = "Transaction date")]
        public String bbl_trandate { get; set; }
        [Display(Name = "Biller name")]
        public String bil_name { get; set; }
        [Display(Name = "Voucher")]
        public String bbl_billervoucher { get; set; }
        [Display(Name = "Bill amount")]
        public String bbl_billamount { get; set; }
        [Display(Name = "Bank response")]
        public String bbl_bnkresponse { get; set; }
        [Display(Name = "Reversal status")]
        public String bbl_reversalstatus { get; set; }
        [Display(Name = "Trace number")]
        public String bbl_sys_traceno { get; set; }
        [Display(Name = "Customer name")]
        public String bbl_customername { get; set; }
        [Display(Name = "Biller response")]
        public String bbl_response { get; set; }
        public string dispute_id { get; set; }
        public string USER_ENTRY { get; set; } = "N/A";
        public string REASON_CODE { get; set; } = "0";
        public string ACTION_CODE { get; set; } = "0";
        [Display(Name = "Dispute reasons")]
        public List<SelectListItem> dispute_reasons { get; set; }
        public string selected_dispute { get; set; }
        public List<Dispute_Action_Model> dispute_actions { get; set; }
        [Required]
        public string selected_action { get; set; }
        public List<SelectListItem> dispute_actions_list { get; set; }


        public string count { get; set; }
        public string user_type { get; set; }
        public string num_count { get; set; }
    }
}