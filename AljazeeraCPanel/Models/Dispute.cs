using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{
    public class Dispute
    {
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> service_names { get; set; }
        public List<SelectListItem> account_type { get; set; }
        public String BranchCode { get; set; }
        public String BranchName{ get; set; }
        public String ServiceCode { get; set; }
        public String AccountType { get; set; }

        [Display(Name = "ID")]
        public string id { get; set; }
        [Display(Name = "Account from")]
        public string ACCOUNTFROM { get; set; }
        [Display(Name = "Account to")]
        public string ACCOUNTTO { get; set; }
        [Display(Name = "Beneficiary")]
        public string beneficiary { get; set; }
        [Display(Name = "Service Name")]
        public string serviceName { get; set; }
        [Display(Name = "Amount")]
        public string AMOUNT { get; set; }
        [Display(Name = "Date/Time")]
        public string DATETIME { get; set; }
        [Display(Name = "Status")]
        public string STATUS { get; set; }

        [Display(Name = "Fees")]
        public string fees { get; set; }
        [Display(Name = "Username")]
        public string USER_LOG { get; set; }
        [Display(Name = "Reason Code")]
        public string REASON_CODE { get; set; }
        [Display(Name = "Reason")]
        public string REASON { get; set; }
        [Display(Name = "User Entry")]
        public string USER_ENTRY { get; set; }
        [Display(Name = "Authorizor")]
        public string AUTHORIZOR { get; set; }
        [Display(Name = "FT")]
        public string FT { get; set; }
        [Display(Name = "RRN")]
        public string RRN { get; set; }
        [Display(Name = "Narriation")]
        public string NARRIATION { get; set; }
        [Display(Name = "Transaction ID")]
        public string TRANSACTIONID { get; set; }
        [Display(Name = "Comments")]
        public List<Comment> Comments { get; set; }

        [Display(Name = "APP_RRN")]
        public string app_rrn { get; set; }
        [Display(Name = "CORE_RRN")]
        public string core_rrn { get; set; }



        /////
        ///
        [Display(Name = "Response Message")]
        public string Response_Message { get; set; }
        [Display(Name = "Tran DateTime")]
        public string Tran_DateTime { get; set; }
        [Display(Name = "Service Name")]
        public string Service_Name { get; set; }
        [Display(Name = "Pay Customer Code")]
        public string Pay_Customer_Code { get; set; }
        [Display(Name = "Amount")]
        public string Amount { get; set; }
        [Display(Name = "User ID")]
        public string User_ID { get; set; }
        [Display(Name = "Branch Name")]
        public string Branch_Name { get; set; }
        [Display(Name = "IBAN")]
        public string IBAN { get; set; }
        [Display(Name = "Account Type")]
        public string Account_Type { get; set; }
        [Display(Name = "Account No")]
        public string Account_No { get; set; }


    }
}