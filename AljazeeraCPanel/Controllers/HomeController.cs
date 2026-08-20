using FCBCPanel.Models;
using Newtonsoft.Json.Linq;
using AljazeeraCPanel.Context;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using AljazeeraCPanel.Repository;
using SIBCPanel.Context;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class HomeController : Controller
    {
        DataSource ds = new DataSource();
        LoginLogic obj = new LoginLogic();
        LoginLogic userBL = new LoginLogic();

        public ActionResult Index()
        {
            if (Session["Homemessage"] != null)
            {
                ViewBag.SuccessMessage = Session["Homemessage"].ToString();
                Session["Homemessage"] = null;
            }

            string branchcode = Session["user_branch"].ToString();
            string adminname = Session["user_name"].ToString();
            ViewData["adminname"] = adminname;

            DashboardData dashboard = new DashboardData();

            Boolean isazalive = false;
            Boolean isebsalive = false;
              
            string apirespone = Connecttocore.getCustomersCounts(Session["accesstoken"].ToString());
            JObject response = new JObject();
            response = JObject.Parse(apirespone);

            int responseCode = int.Parse(response.GetValue("Response_Code").ToString());
            if (responseCode == 0)
            {
                dashboard.AccountCustomersCount = response.GetValue("Acc_Customers_Count").ToString();
                dashboard.LinkedAccountCount = response.GetValue("Linked_Acc_Counts").ToString();
                dashboard.CardCustomersCount = response.GetValue("Card_Customers_Count").ToString();
                dashboard.TotalCustomersCount = response.GetValue("Total_Customers_Count").ToString();
            }


            //apirespone = Connecttocore.isAZAlive(Session["accesstoken"].ToString());
            //response = JObject.Parse(apirespone);
            responseCode = 0; //int.Parse(response.GetValue("Response_Code").ToString());
            if(responseCode == 0)
            {
                isazalive = true;
            }

            //apirespone = Connecttocore.isEBSAlive(Session["accesstoken"].ToString());
            //response =  JObject.Parse(apirespone);
            responseCode = 0;//int.Parse(response.GetValue("Response_Code").ToString());
            if (responseCode == 0)
            {
                isebsalive = true;
            }

            Session["dashboarddata"] = dashboard;
            Session["isazalive"] = isazalive;
            Session["isebsalive"] = isebsalive;


            //List<int> list = ds.GetOnlineOfflineUsers(branchcode);
            //string online = list[0].ToString(); ViewBag.Online = online;
            //string offline = list[4].ToString(); ViewBag.Offline = offline;

            // getting SDEC Balance
            //double sdecbalancecontainer = 0.0;
            //string sdecbalance = "N/A";

            //try
            //{
            //    JObject response = JObject.Parse(Connecttocore.getSDECBalance());
            //    if (response.ContainsKey("responseCode"))
            //    {
            //        if (int.Parse(response.GetValue("responseCode").ToString()) == 0)
            //        {
            //            sdecbalancecontainer = double.Parse(response.GetValue("Balance").ToString());
            //            sdecbalance = sdecbalancecontainer.ToString("C2").Substring(1, sdecbalancecontainer.ToString().Length + 1);
            //        }
            //    }
            //}
            //catch (Exception e)
            //{

            //}

            //Session["SDECBalance"] = sdecbalance;

            // getting all transactions log

            //List<Charter> usersperbranchscount = ds.getUsersBranchsCount();
            //Session["usersperbranchscount"] = usersperbranchscount;
            //List<Charter> usersstatuses = ds.getAllStatuses();
            //Session["usersstatuses"] = usersstatuses;
            //List<Charter> branchstransactionscount = ds.getBranchsTransactionsCount();
            //Session["branchstransactionscount"] = branchstransactionscount;

            return View();
        }



        public ActionResult TransactionsStatuses()
        {

            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            string branchcode = Session["user_branch"].ToString();
            List<TransactionStatusesModel> transactionsstatuses = new List<TransactionStatusesModel>();
            transactionsstatuses = ds.GetTransactionStatusesDetails(branchcode);
            return View(transactionsstatuses);

        }

        public ActionResult NumberOfAccounts()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string user_id = Session["UserID"].ToString();

            string count = ds.GetAccountsCount(user_id);

            ViewBag.AccCount = count;


            return PartialView();
        }


        public ActionResult NumberOfTransfers()
        {
            if (Session["user_name"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            if (Session["user_branch"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            string user_id = Session["UserID"].ToString();

            string count = ds.GetTransferCount(user_id);

            ViewBag.TranCount = count;


            return PartialView();
        }

        public virtual ActionResult TopMenu()
        {
            int myRole = obj.userRole;
            if (myRole == 1)
            {
                //Roles.AddUserToRole("user", "Admin");
            }
            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewBag.username = Session["username"].ToString();

            IEnumerable<Menu> Menu = null;

            if (Session["_Menu"] != null)
            {
                Menu = (IEnumerable<Menu>)Session["_Menu"];
            }
            else
            {
                //return RedirectToAction("Login", "Login");
                string user_id = Session["UserID"].ToString();
                string user_role = Session["user_roleid"].ToString();






                //List<Menu> menuList = new List<Menu>();
                //menuList.Add(new Menu { 
                //    MID = 1,
                //    MenuName = "Customer Management",
                //    MenuURL = "#",
                //    MenuParentID = 0
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 2,
                //    MenuName = "Register NewCustomer",
                //    MenuURL = "CustomerRegistration/Registration",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 3,
                //    MenuName = "Reset Customer Password",
                //    MenuURL = "resetCustomer/ResetCust",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 4,
                //    MenuName = "Activate Customer",
                //    MenuURL = "ActiveAccount/ActiveCustomer",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 5,
                //    MenuName = "DeActivate Customer",
                //    MenuURL = "DeActiveAccount/DeActiveCustomer",
                //    MenuParentID = 1
                //});


                //menuList.Add(new Menu
                //{
                //    MID = 18,
                //    MenuName = "Refresh Customer",
                //    MenuURL = "CustomerRefresh/CustomerRefresh",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 6,
                //    MenuName = "Get Customer Information",
                //    MenuURL = "CustomerReport/getCustomerInformation",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 7,
                //    MenuName = "Customers Authorization",
                //    MenuURL = "CustomerAuthorization/CustomerAuthorization",
                //    MenuParentID = 1
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 9,
                //    MenuName = "Users",
                //    MenuURL = "#",
                //    MenuParentID = 0
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 10,
                //    MenuName = "Users Management",
                //    MenuURL = "User/Users",
                //    MenuParentID = 9
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 10,
                //    MenuName = "Configuration",
                //    MenuURL = "User/Users",
                //    MenuParentID = 0
                //});
                //menuList.Add(new Menu
                //{
                //    MID = 11,
                //    MenuName = "CPanle Profiles Management",
                //    MenuURL = "CPanelProfileManagement/ProfileManagement",
                //    MenuParentID = 10
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 12,
                //    MenuName = "Report",
                //    MenuURL = "#",
                //    MenuParentID = 0
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 13,
                //    MenuName = "Pending Disputes",
                //    MenuURL = "CustomerReport/Disputes",
                //    MenuParentID = 12
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 14,
                //    MenuName = "Customers Count Report",
                //    MenuURL = "CustomerReport/CustomersCountReport",
                //    MenuParentID = 12
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 15,
                //    MenuName = "Customer Registration Report",
                //    MenuURL = "CustomerReport/CustomersRegistrationReport",
                //    MenuParentID = 12
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 16,
                //    MenuName = "Cheque Book Request Report",
                //    MenuURL = "ChqRequest/View",
                //    MenuParentID = 12
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 17,
                //    MenuName = "Atm Card Request Report",
                //    MenuURL = "CardRequest/CardRequest",
                //    MenuParentID = 12
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 18,
                //    MenuName = "Customer Device Password",
                //    MenuURL = "resetCustomer/ResetCustDevice",
                //    MenuParentID = 1
                //});

                //menuList.Add(new Menu
                //{
                //    MID = 19,
                //    MenuName = "New Account Password",
                //    MenuURL = "resetCustomer/AddAccOTP",
                //    MenuParentID = 1
                //});

          
                Menu = MenuData.GetMenus(user_id, user_role);// pass employee id here
                Session["_Menu"] = Menu;

                //Menu = MenuData.GetMenus(user_id, user_role);// pass employee id here
                //Menu = menuList;
                Session["_Menu"] = Menu;
            }
            return PartialView(Menu);
        }

        public ActionResult Logout()
        {
            Session["cpanelLogin"] = "0";
            Session["cpanel_Menu"] = null;
            Session["UserID"] = null;
            Session["user_roleid"] = null;
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Login", "Login");
        }

        public ActionResult test2()
        {
            if (Session["Homemessage"] != null)
            {
                ViewBag.SuccessMessage = Session["Homemessage"].ToString();
                Session["Homemessage"] = null;
            }
            return View();
        }
        public ActionResult Test()
        {
            ViewBag.Message = "Your Test page.";

            return View();
        }
    }

}
