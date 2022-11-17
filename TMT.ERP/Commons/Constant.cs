using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonLib;
using System.Configuration;

namespace TMT.ERP.Commons
{
    public class Constant
    {
        public static int CODE_LENGTH = 5;
        public static int CURRENT_STOCK_ID = 2; 
        public static string UPLOAD_DOCUMENT_FOLDER
        {
            get { return HttpContext.Current.Server.MapPath("~/Uploads"); }
        }

        public static int TEMPORARY_ID
        {
            get
            {
                var tempID = AppContext.SessionVariables.TemporaryID;
                tempID++;
                AppContext.SessionVariables.TemporaryID = tempID;
                return tempID; 
            }
        }

        public static int DefaultPageSize
        {
            get
            {
                try
                {
                    return SafeConvert.To<int>(ConfigurationManager.AppSettings["DefaultPageSize"]);
                }
                catch { return 10; }
            }
        }

        public static string ProductTypeList
        {
            get
            {
                try
                {
                    return ConfigurationManager.AppSettings["ProductTypeList"].ToString();
                }
                catch { return ""; }
            }
        }


        public class SiteMap
        {
            public const string HOME_PAGE = "~/Home/index";
            public const string LOGIN_PAGE = "~/Login/index";
        }

        public static int DefaultCurrency
        {
            get
            {
                try
                {
                    return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultCurrency"].ToString());
                }
                catch { return 0; }
            }
        }


    }
}