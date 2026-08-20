
using AljazeeraCPanel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ModelState = System.Web.Mvc.ModelState;


namespace AljazeeraCPanel.Context
{

    public class Connecttocore
    {


        //Base Url
        public static string BASE_URL = "";
        public static string BASE_URLshare = "";
        public static string BASE_URLbank = "";
        public static string BASE_URLcorp = "";
        public static string configip = null, configport = null, configpath = null;
        public static string configipshare = null, configportshare = null, configpathshare = null;
        public static string configipbank = null, configportbank = null, configpathbank = null;
        public static string configipcorp = null, configportcorp = null, configpathcorp = null;



        public static void getconfig()
        {


            try
            {


                //using (StreamReader sr = new StreamReader("D:\\Projects\\AZ\\NBE\\NBE\\AljazeeraCPanel\\Configuration\\NBEconfiguration.txt"))
                 using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanel\\Configuration\\NBEconfiguration.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        configip = line;
                        //configport = sr.ReadLine();
                        configpath = sr.ReadLine();
                        BASE_URL = configip + "/" + configpath;
                    }
                }
            }

            catch (Exception e)
            {
                String s = e.Message;
            }
        }


        public static void getconfigshare()
        {
            try
            {

                using (StreamReader sr = new StreamReader("C:\\Users\\smah\\Desktop\\last nbe\\NBE2\\AljazeeraCPanel\\Configuration\\NBEconfigurationshare.txt"))

                //using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanelTest\\Configuration\\JSBconfigurationshare.txt"))
                // using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanel\\Configuration\\NBEconfigurationshare.txt"))
                {


                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        configipshare = line;
                        //configport = sr.ReadLine();
                        configpathshare = sr.ReadLine();
                        BASE_URLshare = configipshare + "/" + configpathshare;
                    }
                }
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
        }
        public static void getconfigbank()
        {
            try
            {

                using (StreamReader sr = new StreamReader("C:\\Users\\smah\\Desktop\\last nbe\\NBE2\\AljazeeraCPanel\\Configuration\\NBEconfigurationbank.txt"))

                //using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanelTest\\Configuration\\JSBconfigurationshare.txt"))
                // using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanel\\Configuration\\NBEconfigurationbank.txt"))
                {


                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        configipbank = line;
                        //configport = sr.ReadLine();
                        configpathbank = sr.ReadLine();
                        BASE_URLbank = configipbank + "/" + configpathbank;
                    }
                }
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
        }

        public static void getconfigcorp()
        {
            try
            {

                // using (StreamReader sr = new StreamReader("C:\\Users\\itsadmin\\Desktop\\NBE2\\NBE2\\AljazeeraCPanel\\Configuration\\NBEconfigurationcorp.txt"))

                // using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanelTest\\Configuration\\JSBconfigurationcorp.txt"))
                using (StreamReader sr = new StreamReader("C:\\inetpub\\wwwroot\\CPanel\\Configuration\\NBEconfigurationcorp.txt"))


                {


                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        configipcorp = line;
                        //configport = sr.ReadLine();
                        configpathcorp = sr.ReadLine();
                        BASE_URLcorp = configipcorp + "/" + configpathcorp;
                    }
                }
            }
            catch (Exception e)
            {
                String s = e.Message;
            }
        }

        public string sendotp2(string sms, string phonenumber)
        {
            getconfigshare();
            // String UserID, String PhoneNo, String OTP
            Uri requestUri = new Uri(BASE_URL + "/SendOTP");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            //dynamicJson.UserID = userid;//"130042010593883".ToString();
            // dynamicJson.OTP = sms;//"10";
            dynamicJson.SMS = sms;  //"Your Account temporery password is : ";//"10";
            dynamicJson.Phone = phonenumber;//"10";
            dynamicJson.flag = "Internetbanking";//"10";
            dynamicJson.lang = 1;
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return responJsonText;
            }
        }

        public string sendotp(string userid, string sms, string phonenumber)
        {
            getconfig();

            Uri requestUri = new Uri(BASE_URL + "/SendSMS");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            dynamicJson.userid = userid;//"130042010593883".ToString();
            dynamicJson.SMS = sms;//"10";
            dynamicJson.PhoneNumber = phonenumber;//"10";
            dynamicJson.flag = "Internetbanking";//"10";
            dynamicJson.lang = 1;
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return responJsonText;
            }
        }
        public static string getInvoiceInfo(string invoice, string type, string userid, JObject Account_Info, string accessToken)
        {

            getconfigbank();
            //Uri requestUri = new Uri(BASE_URL + "/GetCustInfoByID");
            Uri requestUri = new Uri(BASE_URLbank + "/billing"); //GetInvoiceInfo
            //string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //string[] splittedtoken = accessToken.Split(' ');
            //accessToken = splittedtoken[1];
            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Pay_Customer_Code = invoice;
            dynamicJson.PayType = type;
            dynamicJson.Account_Info = Account_Info;
            dynamicJson.Biller_ID = "2205";
            dynamicJson.TellerID = userid;
            dynamicJson.Pay_Flag = "0";
            //dynamicJson.Amount = amount;
            dynamicJson.Tran_DateTime = DateTime.Now;
            dynamicJson.lang = "0";

            ///
            /// {
            //           "Biller_ID": "2205", 
            //"Biller_Sub_ID": "000",                //Optional => depends on biller
            //"Pay_Customer_Code": "202510000700200036",         
            //"Additional_Reference": "000",			//Optional => depends on biller
            //"Account_Info": {                       //Used on payment and top up only                 
            //               "Account_No": "01222000242",             
            //	"Branch_Code": "1",             
            //	"Currency_Code": "SDG",           
            //	"Account_Type_Code": "222"

            //   },
            //"Amount": "54000",
            //"Pay_Flag": "0",					//on inquiries Pay_flag = 0, on 										payment and topups pay_flag = 1
            //"Tran_DateTime": "281025021750",             
            //"Lang": "0"
            //}
            ////

            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json,
                        Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }

        }


        public static string SensSMS(string sms, string phone, string accessToken)
        {

            getconfigshare();
            // String UserID, String PhoneNo, String OTP
            Uri requestUri = new Uri(BASE_URLshare + "/sendSMS");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            //dynamicJson.UserID = userid;//"130042010593883".ToString();
            // dynamicJson.OTP = sms;//"10";
            dynamicJson.SMS = sms;  //"Your Account temporery password is : ";//"10";
            dynamicJson.Phone = phone;//"10";
            dynamicJson.flag = "Internetbanking";//"10";
            dynamicJson.lang = 1;
            dynamicJson.uuid = Guid.NewGuid();



            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json,
                        Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }

        }

        //string invoice, string type, string amount, string fees, string userid
        public static string PayInvoice(string invoice, string type, string userid, string amount, JObject Account_Info, string accessToken)
        {

            getconfigbank();
            //Uri requestUri = new Uri(BASE_URL + "/GetCustInfoByID");
            Uri requestUri = new Uri(BASE_URLbank + "/billing"); //PayInvoice
            //string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            //string[] splittedtoken = accessToken.Split(' ');
            //accessToken = splittedtoken[1];
            //dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.PayCustomerCode = invoice;
            //dynamicJson.ServiceID = type;
            //dynamicJson.RequiredAmount = amount;
            //dynamicJson.UserID = userid;
            ////dynamicJson.FeesAmount = fees;
            //dynamicJson.lang = "1";
            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Pay_Customer_Code = invoice;
            dynamicJson.PayType = type;
            dynamicJson.Account_Info = Account_Info;
            dynamicJson.Biller_ID = "2205";
            dynamicJson.TellerID = userid;
            dynamicJson.Pay_Flag = "1";
            dynamicJson.Amount = amount;
            dynamicJson.Tran_DateTime = DateTime.Now;
            dynamicJson.lang = "0";
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json,
                        Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }

        }

        public string sendpredefinedsms(string userid, string account, string messagecode, string phonenumber)
        {
            getconfig();

            Uri requestUri = new Uri(BASE_URL + "/PredefinedSMS");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            dynamicJson.userid = userid;//"130042010593883".ToString();
            dynamicJson.Account = account;
            dynamicJson.Code = messagecode;
            dynamicJson.PhoneNumber = phonenumber;//"10";
            dynamicJson.flag = "Internetbanking";//"10";
            dynamicJson.lang = 1;
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                return responJsonText;
            }
        }





        //public class OTPService
        //    {
        public async Task SendOTPAsync(string phoneNo, string otp)
        {
            string responseData = null;
            string output = null;

            try
            {
                // Build request data (not actually used in GET, but kept for parity)
                string data = "{\n" +
                              $"    \"Phone_No\" :\"{phoneNo}\",\n" +
                              $"    \"Message\":\"{otp}\"\n" +
                              "}";

                string url = $"https://bulk.gawali.net/sms_services/bulk/sms.php?username=nbe_admin&password=Nbek@get1**&to={phoneNo}&text={otp}";
                Console.WriteLine("URL : " + url);

                using (HttpClient client = new HttpClient())
                {
                    // Send GET request
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Failed : HTTP error code : " + (int)response.StatusCode);
                    }

                    Console.WriteLine((int)response.StatusCode);

                    using (StreamReader reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
                    {
                        while ((output = reader.ReadLine()) != null)
                        {
                            responseData = output;
                            Console.WriteLine(output);
                        }
                    }

                    string code = ((int)response.StatusCode).ToString();

                    if (code != "200")
                        Console.WriteLine(phoneNo, otp, "F");
                    else
                        Console.WriteLine(phoneNo, otp, "S");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception: " + e.Message);
                Console.WriteLine(phoneNo, otp, "UK"); // Unknown
            }
        }




        public async Task<string> sendotpbyURL(string userid, string sms, string phonenumber)
        {

            // "https://bulk.gawali.net/sms_services/bulk/sms.php?username=nbe_admin&password=Nbek@get1**&to=" + phonenumber + "& text=" + sms;
            // URL to execute
            string url = "https://bulk.gawali.net/sms_services/bulk/sms.php?username=nbe_admin&password=Nbek@get1**&to=" + phonenumber + "&text=" + sms;
            // string url = "https://www.airtel.sd/bulksms/webbal.aspx?user=jabank&pwd=835402";

            var responJsonText = "";

            // Create HttpClient instance
            using (HttpClient client = new HttpClient())
            {

                try
                {
                    // Send GET request
                    HttpResponseMessage response = await client.GetAsync(url);
                    // Ensure successful response
                    response.EnsureSuccessStatusCode();
                    var statusCode = response.StatusCode;
                    // Read response content
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    string responseBody = await response.Content.ReadAsStringAsync();


                    Console.WriteLine(responseBody);
                    responJsonText = responseBody;


                    return responJsonText;
                    // Print response to console

                }
                catch (HttpRequestException e)
                {

                    return $"Request error: {e.Message}";

                }

            }
        }


        public static string GetCustinfo(string account)
        {
            getconfig();
            //Uri requestUri = new Uri(BASE_URL + "/GetCustInfoByID");
            Uri requestUri = new Uri(BASE_URL + "/GetCustinfo");
            dynamic dynamicJson = new ExpandoObject();


            //dynamicJson.CustID = cif;//"1300420105s93883".ToString();
            dynamicJson.account = account;
            dynamicJson.Authentication = "Card";
            dynamicJson.ChannelID = "InternetBanking";
            dynamicJson.lang = "1";
            dynamicJson.uuid = Guid.NewGuid();
            //JArray ob = new JArray();


            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {


                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;
            }
        }

        public static string GetCustinfoCore(string account, string accessToken)
        {
            getconfig();
            //Uri requestUri = new Uri(BASE_URL + "/GetCustInfoByID");
            //Uri requestUri = new Uri(BASE_URL + "/GetCustinfo");
            Uri requestUri = new Uri(BASE_URL + "/cpGetCustInfoCore");
            dynamic dynamicJson = new ExpandoObject();

            List<JObject> Account_info = new List<JObject>();
            JObject accountt = new JObject();
            accountt.Add("Account_No", account);
            Account_info.Add(accountt);
            dynamicJson.Account_Info = Account_info;
            //dynamicJson.CustID = cif;//"1300420105s93883".ToString();
            //dynamicJson.Account_No = account;
            //dynamicJson.Authentication = "Card";
            //dynamicJson.ChannelID = "InternetBanking";
            //dynamicJson.lang = "1";
            dynamicJson.Cust_Info_Type = "2";
            dynamicJson.uuid = Guid.NewGuid();
            //JArray ob = new JArray();


            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;
            }
        }
        public static string cpLogin(string username, string password)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpLogin");
            DateTime datetimenow = DateTime.Now;

            dynamic dynamicJson = new ExpandoObject();

            dynamicJson.username = username;
            dynamicJson.password = password;
            dynamicJson.ChannelID = "InternetBanking";
            dynamicJson.lang = 1;
            dynamicJson.Tran_DateTime = datetimenow;

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;
            }
        }

        public static string getCustomerInfo(string account_number, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpGetCustInfoCore");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            List<JObject> Account_info = new List<JObject>();
            JObject account = new JObject();
            account.Add("Account_No", account_number);

            Account_info.Add(account);


            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Cust_Info_Type = 1;
            //dynamicJson.Account_Info = Account_info;
            dynamicJson.RIM = account_number;
            dynamicJson.Tran_DateTime = datetimenow;
            dynamicJson.Lang = 1;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {



                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }


                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getCustomerInfoByRim(string RIM, string accessToken)
        {


            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpGetCustInfoCore");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];


            //List<JObject> Account_info = new List<JObject>();
            //JObject account = new JObject();
            //account.Add("Account_No", account_number);
            //account.Add("Account_Type_Code", Account_Type_Code);
            //account.Add("Currency_Code", Currency_Code);
            //account.Add("AccounBranch_Codet_No", Branch_Code);
            //Account_info.Add(account);

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Cust_Info_Type = 1;
            ////dynamicJson.Account_Info = Account_info;
            dynamicJson.RIM = RIM;
            dynamicJson.Type = 2;
            //dynamicJson.Tran_DateTime = datetimenow;
            //dynamicJson.Lang = 1;


            //dynamicJson.Account_No = account_number;
            dynamicJson.Authentication = "Card";
            dynamicJson.ChannelID = "InternetBanking";
            dynamicJson.lang = "1";
            //dynamicJson.Cust_Info_Type = "2";

            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {

                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }


        public static string getCustomerInfoByRimforCorp(string RIM, string Cat, String[] channel, string accessToken)
        {
            getconfigcorp();
            //BASE_URL = "http://172.23.2.72:8080/JSB_OMNI_Ph2/omniServices/corpRoutes";
            Uri requestUri = new Uri(BASE_URLcorp + "/getCorpInfoCore");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            //List<JObject> Account_info = new List<JObject>();
            //JObject account = new JObject();
            //account.Add("Account_No", account_number);
            //account.Add("Account_Type_Code", Account_Type_Code);
            //account.Add("Currency_Code", Currency_Code);
            //account.Add("AccounBranch_Codet_No", Branch_Code);
            //Account_info.Add(account);

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Cust_Info_Type = 1;
            ////dynamicJson.Account_Info = Account_info;
            dynamicJson.RIM = RIM;
            //dynamicJson.Tran_DateTime = datetimenow;
            //dynamicJson.Lang = 1;


            //dynamicJson.Account_No = account_number;
            dynamicJson.Authentication = "Card";
            dynamicJson.ChannelID = "InternetBanking";
            dynamicJson.lang = "1";
            //dynamicJson.Cust_Info_Type = "2";

            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {

                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }


        public static string userInfo(string Branch_Code, string fromdate, string todate, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/getCustRegRprt");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            List<JObject> Account_info = new List<JObject>();
            JObject account = new JObject();
            account.Add("Branch_Code", Branch_Code);
            account.Add("From_Date", fromdate);
            account.Add("To_Date", todate);

            Account_info.Add(account);

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Cust_Info_Type = 2;
            dynamicJson.Account_Info = Account_info;
            //dynamicJson.RIM = RIM;
            dynamicJson.Tran_DateTime = datetimenow;
            dynamicJson.Lang = 1;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }


        public static string NID_Info(string nid, string accessToken)
        {
            getconfigshare();
            //BASE_URL = "http://172.23.2.72:8080/JSB_OMNI_Ph2/omniServices/corpRoutes";
            Uri requestUri = new Uri(BASE_URLshare + "/GetCRSData");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            dynamic dynamicJson = new ExpandoObject();

            ////dynamicJson.Account_Info = Account_info;
            dynamicJson.NID = nid;
            //dynamicJson.Tran_DateTime = datetimenow;
            //dynamicJson.Lang = 1;


            //dynamicJson.Account_No = account_number;

            //dynamicJson.lang = "1";
            //dynamicJson.Cust_Info_Type = "2";

            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {

                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                    /////
                    //respon.Content.ReadAsStringAsync();
                    //ImageResponse imageResponse = JsonConvert.DeserializeObject<ImageResponse>(responJsonText);
                    //byte[] imageBytes = Convert.FromBase64String(imageResponse.ImageBase64);
                    //var imageResponser = JsonConvert.DeserializeObject<ImageResponse>(responJsonText); // Pass the base64 string to the view ViewBag.ImageBase64 = imageResponse.ImageBase64;
                    //responJsonText = responJsonText + imageResponse.ImageBase64; 
                    //////
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string CustTranInfo(string Branch_Code, string Status, string UserID, string fromdate, string todate, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/getTransDetlRprt");
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            List<JObject> Account_info = new List<JObject>();
            JObject account = new JObject();
            account.Add("Branch_Code", Branch_Code);
            account.Add("Tran_Status", Status);
            account.Add("User_ID", UserID);
            account.Add("From_Date", fromdate);
            account.Add("To_Date", todate);

            Account_info.Add(account);

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Cust_Info_Type = 2;
            dynamicJson.Account_Info = Account_info;
            //dynamicJson.RIM = RIM;
            dynamicJson.Tran_DateTime = datetimenow;
            dynamicJson.Lang = 1;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }





        public static string restCustomerPassword(string userid, string accessToken)
        {

            getconfig();

            Uri requestUri = new Uri(BASE_URL + "/cpResetCustPWD");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.User_ID = userid;
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string restCustomerPasswordDevice(string userid, string accessToken)
        {
            getconfig();

            Uri requestUri = new Uri(BASE_URL + "/requestOTP");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.User_ID = userid;
            dynamicJson.Lang = 0;
            dynamicJson.userid = userid;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string AddAccountOTP(string userid, string accessToken)
        {
            getconfigshare();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Uri requestUri = new Uri(BASE_URLshare + "/getRegOTP");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.User_ID = userid;
            dynamicJson.Lang = 0;
            dynamicJson.userid = userid;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getUnauthorizedUsers(string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/getUnauthCust");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getCustomersCounts(string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/getCustCounts");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];
            //accessToken = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIwMDAxNjc3NTIiLCJpYXQiOjE2NzI3NjU0MDQsImlzcyI6IjEiLCJVc2VySUQiOiIwMDAxNjc3NTIiLCJEZXZpY2VLZXkiOiIwMmJmZjU4OWY5MzI0ODEwIiwiZXhwIjoxNjcyNzc1NDA0fQ.n01g7iJBW3jCcGLY15DEgFxJrbT0Z-xP0an93gICa1k";
            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getDispute(string accessToken, string fromdate, string todate, string branch, string service, string accountType)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/getDisputeTrans");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];
            //accessToken = "Bearer eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIwMDAxNjc3NTIiLCJpYXQiOjE2NzI3NjU0MDQsImlzcyI6IjEiLCJVc2VySUQiOiIwMDAxNjc3NTIiLCJEZXZpY2VLZXkiOiIwMmJmZjU4OWY5MzI0ODEwIiwiZXhwIjoxNjcyNzc1NDA0fQ.n01g7iJBW3jCcGLY15DEgFxJrbT0Z-xP0an93gICa1k";
            string datetimenow = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            dynamic dynamicJson = new ExpandoObject();
            //dynamicJson.Tran_DateTime = datetimenow;
            dynamicJson.From_date = fromdate;
            dynamicJson.To_Date = todate;
            dynamicJson.Branch_Code = branch;
            dynamicJson.Service_Code = service;
            dynamicJson.Account_Type_Code = accountType;
            //dynamicJson.Lang = 1;
            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            //string  json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }
        public static string isAZAlive(string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/isAliveAZ");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string isEBSAlive(string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/isAliveEBS");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getRoles(string accessToken)
        {
            getconfig();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri requestUri = new Uri(BASE_URL + "/getRoles");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getBranchs(string accessToken)
        {
            getconfigshare();
            //string BASE_URL = "https://mob.jsjbank.com:8383/JSB_OMNI_Ph2/omniServices/sharedRoutes";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri requestUri = new Uri(BASE_URLshare + "/getBranches");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }


        public static string getAccType(string accessToken)
        {
            getconfigshare();
            //string BASE_URL = "https://mob.jsjbank.com:8383/JSB_OMNI_Ph2/omniServices/sharedRoutes";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri requestUri = new Uri(BASE_URLshare + "/getAccTypes");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string activateCustomer(string userid, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpActivateCust");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.User_ID = userid;
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }


        public static string deactivateCustomer(string userid, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpSetCustStatus");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.User_ID = userid;
            dynamicJson.Cust_Status = "DA";
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string authroizeCustomer(string userid, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpSetCustStatus");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.User_ID = userid;
            dynamicJson.Cust_Status = "A";
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string rejectCustomer(string userid, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpSetCustStatus");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.User_ID = userid;
            dynamicJson.Cust_Status = "R";
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getcustomerinfousingphonenumber(string phonenumber, string rim, string accountnumber, string username, string accessToken)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/cpGetRegCustInfo"); //  // cpGetRegCustInfo  //cpGetCustInfoCore
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1]; ;

            dynamic dynamicJson = new ExpandoObject();
            if (rim != null)
            {
                dynamicJson.RIM = rim;
                dynamicJson.Cust_Info_Type = 1;
                dynamicJson.uuid = Guid.NewGuid();

            }
            if (phonenumber != null)
            {
                dynamicJson.Phone_No = phonenumber;
                dynamicJson.Cust_Info_Type = 3;
                dynamicJson.uuid = Guid.NewGuid();
            }

            if (username != null)
            {
                dynamicJson.User_ID = username;
                dynamicJson.Cust_Info_Type = 4;
                dynamicJson.uuid = Guid.NewGuid();
            }

            if (accountnumber != null)
            {

                //List<JObject> Account_info = new List<JObject>();
                //JObject accountt = new JObject();
                //accountt.Add("Account_No", accountnumber);
                //Account_info.Add(accountt);
                //dynamicJson.Account_Info = Account_info;


                dynamicJson.Account_No = accountnumber;

                dynamicJson.Cust_Info_Type = 2;
                dynamicJson.uuid = Guid.NewGuid();

            }

            //dynamicJson.Phone_No = phonenumber;
            //dynamicJson.Rim = rim;
            //dynamicJson.Account_No = accountnumber;
            //dynamicJson.UserName = username;
            //dynamicJson.Cust_Info_Type = 3;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }


                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string registerCustomer(string email, string phonenumber, string address, string customernamear, string customernameen, string rim, string account_no, string cat, String[] channel, string accessToken)
        {
            getconfig();

            Uri requestUri = new Uri(BASE_URL + "/cpCreateCustomer");
            string[] splittedtoken = accessToken.Split(' ');
            accessToken = splittedtoken[1];

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.Email = email;
            dynamicJson.Phone_No = phonenumber;
            dynamicJson.Address = address;
            dynamicJson.Customer_Name_EN = customernameen;
            dynamicJson.Customer_Name_AR = customernamear;
            dynamicJson.RIM = rim;

            dynamicJson.Account_No = account_no;

            dynamicJson.CategoryCode = cat;
            dynamicJson.SelectedChannelsID = channel;
            dynamicJson.Lang = 0;
            string json = json = JsonConvert.SerializeObject(dynamicJson);

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    objClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    HttpResponseMessage respon = objClient.PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string getSDECBalance()
        {
            getconfig();
            BASE_URL = "http://192.168.40.20:8080/IBMiddleware/webresources/IBWebservices";

            Uri requestUri = new Uri(BASE_URL + "/getSDECBalance");

            var responJsonText = "";

            using (var objClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage respon = objClient.GetAsync(requestUri).Result;
                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error " + e.Message;
                }
                return responJsonText;
            }
        }

        public static string GetHeartBeat()
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/HeartBeat");
            dynamic dynamicJson = new ExpandoObject();

            //dynamicJson.Authentication = "Card";
            //dynamicJson.ChannelID = "InternetBanking";
            //dynamicJson.lang = "1";
            //dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;

            }

        }

        public static string GetCustinfobycif(string cif)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/GetCustInfoByID");
            dynamic dynamicJson = new ExpandoObject();

            dynamicJson.CustID = cif;//"130042010593883".ToString();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            dynamicJson.lang = "1";
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {
                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;

            }

        }

        public static string GetCustaccounts(string accountNo)
        {
            getconfig();
            Uri requestUri = new Uri(BASE_URL + "/GetCustinfoByID");

            dynamic dynamicJson = new ExpandoObject();
            dynamicJson.account = accountNo;//"130042010593883".ToString();
            dynamicJson.Authentication = "Card";
            dynamicJson.Channel = "InternetBanking";
            dynamicJson.lang = "1";
            dynamicJson.uuid = Guid.NewGuid();

            string json = "";
            json = JsonConvert.SerializeObject(dynamicJson);
            var responJsonText = "";
            JObject JResp = new JObject();

            using (var objClient = new HttpClient())
            {
                try
                {

                    HttpResponseMessage respon = objClient
                        .PostAsync(requestUri, new StringContent(json, Encoding.UTF8, "application/json")).Result;

                    if (respon.IsSuccessStatusCode)
                    {
                        responJsonText = respon.Content.ReadAsStringAsync().Result;
                    }
                }
                catch (Exception e)
                {
                    responJsonText = "Error";
                }

                return responJsonText;

            }

        }

    }
}