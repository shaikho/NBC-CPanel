using System.Collections.Generic;
using System.Web.Mvc;
using AljazeeraCPanel.Filters;
using AljazeeraCPanel.Models;
using AljazeeraCPanel.Repository;

namespace AljazeeraCPanel.Controllers
{
    [AuthorizeSession]
    public class AccountController : Controller
    {
        public virtual PartialViewResult Menu()
        {
            IEnumerable<Menu> Menu = null;
            if (Session["cpanel_Menu"] != null)
            {
                Menu = (IEnumerable<Menu>)Session["cpanel_Menu"];
            }
            else
            {
                Menu = MenuData.GetMenus(Session["UserId"].ToString(), Session["user_roleid"].ToString());// pass employee id here
                Session["cpanel_Menu"] = Menu;
            }
            return PartialView(Menu);
        }

    }
}