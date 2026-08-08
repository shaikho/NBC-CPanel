using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AljazeeraCPanel.Models
{
 
    public class CustomerRegBankinfo
    {
        public List<CustomerRegBankinfo> Accounts_List { get; internal set; }
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> AccTypes { get; set; }
        public List<SelectListItem> Currencies { get; set; }
        public List<SelectListItem> catgories { get; set; }
        [Display(Name = "Arabic Customer Name")]
        public string customernameArabic { get; set; }
        [Display(Name = "Customer Phone Number")]
        public string customerphonenumber { get; set; }
        public List<channel> Channels { get; set; }
        [Display(Name = "Account Details")]
        public List<AccountDetails> accountDetails { get; set; }

        public string selectedphonenumber { get; set; }
        public string selectedaccount { get; set; }
        public List<SelectListItem> AvailablePhoneNumbers { get; set; }
        public List<SelectListItem> AvailableCustomerAccount { get; set; } = new List<SelectListItem>();
        //[Required]
        //[Range(0, 10)]
        [Required(ErrorMessage = "User Id is required.")]
        //[RegularExpression(@"^\d{1,10}$", ErrorMessage = "User Id must be numeric and up to 10 digits.")]
        [Display(Name = "Customer Branch")]

        public  string Branch { get; set; }

        [Display(Name = "Response Message")]
        public string respmsg { get; set; }

        [Display(Name = "Invoice Number")]
        public string invoice { get; set; }

        [Display(Name = "Service Name")]
        public String ServiceName { get; set; }
        [Display(Name = "Response Code")]
        public int responseCode { get; set; }

        [Display(Name = "Invoice Status")]
        public String InvoiceStatus { get; set; }
        [Display(Name = "Unit Name")]
        public String UnitName { get; set; }
        [Display(Name = "Center Name")]
        public String CenterName { get; set; }
        [Display(Name = "Tran Date")]
        public String trandate { get; set; }

        [Display(Name = "Amount")]
        public String amount { get; set; }

        //[Display(Name = "Amount")]
        //public decimal Amount { get; set; }

        [Display(Name = "Fees")]
        public String Fees { get; set; }


        [Display(Name = "CB RRN")]
        public String cb_rrn { get; set; }
        [Display(Name = "Tran ID")]
        public String tranid { get; set; }
        [Display(Name = "Reference")]
        public String reference { get; set; }

        public Boolean data { get; set; }
        public Boolean byaccount { get; set; }
        public Boolean pay { get; set; }

        //[Required]
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }
        //[Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }

        [Display(Name = "User Status")]
        public string status { get; set; }

        //[Required]
        [Display(Name = "Account Number")]

        [RegularExpression(@"^\d{1,8}$", ErrorMessage = "Numbers only, max 8 digits")]
        
        public string AccountNumber { get; set; }

        //[Required]
        [Display(Name = "Add Account")]

        public string AccountNumberAdded { get; set; }
        [Display(Name = "Customer Card")]
        public String CustomerCard { get; set; }

        //[Required]
        public String BranchCode { get; set; }

        //[Required]
        public String AccountTypecode { get; set; }

        //[Required]
        public String CurrencyCode { get; set; }


        public bool Cust_Info_Type { get; set; }

        [RegularExpression("[^0-9]", ErrorMessage = "Must be numeric")]
        [MaxLength (2)]
        [Display(Name ="SUBNO")]
        public String SUBNO { get; set; }

        //[RegularExpression(@"^\d+$", ErrorMessage = "Only numbers are allowed")]
        [RegularExpression("[^0-9]", ErrorMessage = "Must be numeric")]
        [MaxLength(3)]
        [Display(Name ="SUBGL")]
        public String SUBGL { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }

        [Display(Name = "Customer RIM")]
        [RegularExpression("[^0-9]", ErrorMessage = "Only numbers are allowed")]
        public string rim { get; set; }
        [Display(Name = "Customer IBAN")]
        public string iban { get; set; }

        public String CustomerID { get; set; }
        //[Required]
        public String CategoryCode { get; set; }
         [Display(Name="Category")]
        public String category { get; set; }
        [Display(Name = "Type")]
        public String type { get; set; }
        public String[] SelectedChannelsID { get; set; }
         [Display(Name = "Channel")]
         public String Channel { get; set; }
         public List<channel> SelectedChannels { get; set; }
        public string placeholder { get; set; }
        [Display(Name = "CIF")]
        public string cif { get; set; }
        [Display(Name = "Customer Accounts")]
        public List<SelectListItem> CustomerAccounts { get; set; }
        [Display(Name = "Customer Address")]
        public string address { get; set; }
        [Display(Name = "Customer Account")]
        public string CustomerAccount { get; set; }
     
    }
    public class CustomerRegBankinfo2
    {
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Customer Name En")]
        public string CustomerNameEN{ get; set; }
        [Display(Name = "Customer Name Arabic")]
        public string CustomerNameArabic { get; set; }

        [Display(Name = "Customer Phone")]
        public string CustomerPhone { get; set; }
        [Display(Name = "Customer ID")]
        public String CustomerID { get; set; }
        [Display(Name = "Customer Address")]
        public String CustomerAddress { get; set; }
        [Display(Name = "Customer Account Number")]
        public String CustomerAccount { get; set; }
        [Display(Name = "Phone Numbers")]
        public List<SelectListItem> AvailablePhoneNumbers { get; set; }
        public List<SelectListItem> AvailableCustomerAccount { get; set; } = new List<SelectListItem>();
        [Display(Name = "Account Details")]
        public List<AccountDetails> accountDetails { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "RIM")]
        public string RIM { get; set; }
        public string selectedphonenumber { get; set; }
        public string selectedaccount { get; set; }
        public List<SelectListItem> Profiles { get; set; }
        public string selectedprofile { get; set; }

        public string channel { get; set; }

        public string cat { get; set; }

    }

   public class addaccount
   {
        [Display(Name = "Customer Branch")]
        public  string Branch { get; set; }
        //[Required]
        [Display(Name = "Account Currency")]
        public string Currency { get; set; }
        //[Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
        [Required]
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }
       [Display(Name = "Account Number")]
        public string AccountNumbercomplete { get; set; }
       
        public int AccountID { get; set; }
        public bool IsSelected { get; set; }
   }
   public class accountsresult
   {

       public List<addaccount> accountSelected { get; set; }
       public int[] acctIds { get; set; }
   }

   public class account
   {
       [Required]
       [Display(Name = "Customer Account")]
       public string Account { get; set; }
   }


   public class pendingacts
   {
       [Display(Name = "Customer ID")]
       public string USER_ID { get; set; }
       [Display(Name = "Customer Name")]
       public string USER_NAME { get; set; }
       [Display(Name = "Customer Account")]
       public string DEF_ACC { get; set; }
       [Display(Name = "Customer New Account")]
       public string ACC_NO { get; set; }
       public string ACC_NO1 { get; set; }
   }
   public class actAuthorizationinfo
   {
       [Display(Name = "Customer Branch")]
       public string Branch { get; set; }
       //[Required]
       [Display(Name = "Account Currency")]
       public string Currency { get; set; }
       //[Required]
       [Display(Name = "Customer Account Type")]
       public string AccountType { get; set; }
       [Display(Name = "Customer Name")]
       public String Customername { get; set; }
       [Display(Name = "Customer Account")]
       public String Customeraccount { get; set; }
       [Display(Name = "Customer ID")]
       public String CustomerID { get; set; }
       
       [Display(Name = "Reject Reason")]
       public string RejectReason { get; set; }
       public string userid { get; set; }
       public string authsts { get; set; }
       public string rjtsts { get; set; }
       public string completeact { get; set; }
   }
 
 
}