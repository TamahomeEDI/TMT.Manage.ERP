using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonLib;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using System.Configuration;


namespace TMT.ERP.Commons
{
    public class AppContext
    {
        public class SessionVariables
        {
            public class SessionKey
            {
                public const string CURRENT_USER_ID = "CURRENT_USER_ID";
                public const string TEMPORARY_ID = "TEMPORARY_ID";
                public const string SESSION_ID = "SESSION_ID";
                public const string CURRENT_COMPANY_ID = "CURRENT_COMPANY_ID";
            }

            public static System.Web.SessionState.HttpSessionState Session
            {
                get { return HttpContext.Current.Session; }
            }

            public static int? CurrentUserID
            {
                get { return SafeConvert.ToNullable<int>(Session[SessionKey.CURRENT_USER_ID], true); }                
                //get { return SafeConvert.ToNullable<int>("1", true); } 
                set { Session[SessionKey.CURRENT_USER_ID] = value; }
            }

            public static int? CurrentCompanyID
            {
                get { return SafeConvert.ToNullable<int>(Session[SessionKey.CURRENT_COMPANY_ID], true); }
                set { Session[SessionKey.CURRENT_COMPANY_ID] = value; }
            }

            public static int TemporaryID
            {
                get
                {
                    if (Session[SessionKey.TEMPORARY_ID] == null)
                        Session[SessionKey.TEMPORARY_ID] = int.MinValue;

                    return SafeConvert.To<int>(Session[SessionKey.TEMPORARY_ID]);
                }
                set { Session[SessionKey.TEMPORARY_ID] = value; }
            }

            public static string SessionID
            {
                get
                {
                    if (Session[SessionKey.SESSION_ID] == null)
                        Session[SessionKey.SESSION_ID] = int.MinValue.ToString();

                    return Session[SessionKey.SESSION_ID].ToString();
                }
                set { Session[SessionKey.SESSION_ID] = value; }
            }

            public static void Clear()
            {
                Session.Clear();
            }
        }



        /// <summary>
        /// Data commonly used each request, should not be editted manually
        /// </summary>
        public class RequestVariables
        {
            private class RequestKey
            {
                public const string CURRENT_USER = "CURRENT_USER";
                public const string CURRENT_COMPANY = "CURRENT_COMPANY";
            }

            private static IDictionary Request
            {
                get { return HttpContext.Current.Items; }
            }

            public static Company CurrentCompany
            {
                get
                {
                    if (Request[RequestKey.CURRENT_COMPANY] == null)
                    {
                        if (SessionVariables.CurrentCompanyID.HasValue)
                        {
                            var currentCompany = new GenericManager<User>().Find(x => x.ID == SessionVariables.CurrentCompanyID.Value);
                            Request[RequestKey.CURRENT_COMPANY] = currentCompany;
                        }
                    }

                    return Request[RequestKey.CURRENT_COMPANY] as Company;
                }
                private set
                {
                    Request[RequestKey.CURRENT_COMPANY] = value;
                    if (value == null)
                        SessionVariables.CurrentCompanyID = null;
                    else
                        SessionVariables.CurrentCompanyID = value.ID;
                }
            }


            public static User CurrentUser
            {
                get
                {
                    if (Request[RequestKey.CURRENT_USER] == null)
                    {
                       
                        if (SessionVariables.CurrentUserID.HasValue)
                        {
                            User currentUser = null;
                            currentUser = new GenericManager<User>().Find(x => x.ID == SessionVariables.CurrentUserID.Value);

                            if (currentUser != null)
                            {
                                SessionVariables.CurrentCompanyID = currentUser.CompanyID;
                            }

                            Request[RequestKey.CURRENT_USER] = currentUser;
                        }
                    }

                    return Request[RequestKey.CURRENT_USER] as User;
                }
                set
                {
                    Request[RequestKey.CURRENT_USER] = value;
                    if (value != null)
                    {
                        try
                        {
                            SessionVariables.CurrentUserID = value.ID;
                        }
                        catch (Exception ex)
                        {
                            //logger.Error("Cannot release file lock at LOGIN", ex);
                        }
                    }
                    else
                    {
                        SessionVariables.CurrentUserID = null;
                    }
                }
            }

            public static void Clear()
            {
                Request.Clear();
            }

        }

    }
}