using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading;
using System.Configuration;
using TMT.ERP.Commons;
using System.Data.Entity;
using System.Web.SessionState;

namespace TMT.ERP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {

        //Add Session webapi
        public override void Init()
        {
            this.PostAuthenticateRequest += MvcApplication_PostAuthenticateRequest;
            base.Init();
        }

        void MvcApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                SessionStateBehavior.Required);
        }
        //End 
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            //llam comment here
            //AuthConfig.RegisterAuth();
        }

        protected void Application_BeginRequest()
        {
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern = Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
        }

        protected void Application_EndRequest()
        {
            //dispose context
            var contextPerRequestKey = ConfigurationManager.AppSettings["HttpContextDbContextKey"];

            DisposeContext(contextPerRequestKey);

            //clear request variables
            AppContext.RequestVariables.Clear();
        }

        private void DisposeContext(string contextKey)
        {
            if (!string.IsNullOrEmpty(contextKey))
            {
                if (HttpContext.Current != null)
                {
                    var context = HttpContext.Current.Items[contextKey] as DbContext;
                    if (context != null)
                    {
                        context.Configuration.LazyLoadingEnabled = false;
                        context.Dispose();
                    }
                }
            }
        }

        protected void Application_End()
        {
            //  Code that runs on application shutdown
        }

        protected void Application_Error()
        {
            // Code that runs when an unhandled error occurs
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

    }
}