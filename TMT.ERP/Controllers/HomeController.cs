using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class HomeController : BaseController  //Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ChangeCurrentCulture(int id)
        {
            //
            // Change the current culture for this user.
            //
            SessionManager.CurrentCulture = id;
            //
            // Cache the new current culture into the user HTTP session. 
            //
            Session["CurrentCulture"] = id;
            //
            // Redirect to the same page from where the request was made! 
            //
            return Redirect(Request.UrlReferrer.ToString());
        }
        public int SetTabActive(int tabSelect)
        {
            int result = 0;
            switch (tabSelect)
            {
                case 0:
                    result = 0;
                    break;
                case 1:
                    result = 1;
                    break;
                case 2:
                    result = 2;
                    break;
                case 3:
                    result = 3;
                    break;
                case 4:
                    result = 4;
                    break;
                case 5:
                    result = 5;
                    break;
                case 6:
                    result = 6;
                    break;
                case 7:
                    result = 7;
                    break;
                case 8:
                    result = 8;
                    break;
                default:
                    result = 0;
                    break;
            }
            Session["Tab"] = result;
            return result;
        }
    }
}
