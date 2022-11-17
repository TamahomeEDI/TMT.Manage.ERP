using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Manage the internationalization before to invokes the action in the current controller context.
        /// </summary>
        
   
        protected override void ExecuteCore()
        {
            var currentUser = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
            var userName = currentUser == null ? "" : currentUser.UserName;

            if (currentUser == null)
            {
                Response.Redirect(TMT.ERP.Commons.Constant.SiteMap.LOGIN_PAGE, true);
                return;
            }

            int culture = 0;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {
                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            
            SessionManager.CurrentCulture = culture;
           
            //
            // Invokes the action in the current controller context.
            //

            try
            {
                base.ExecuteCore();
            }
            catch (Exception e)
            {
                //var a = e.ToString();
                throw new HttpAntiForgeryException("", e); 
            }
            
            //base.ExecuteCore();
        }

        protected override bool DisableAsyncSupport
        {
            get { return true; }
        }

    }
}
