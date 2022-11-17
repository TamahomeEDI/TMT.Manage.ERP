using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Web.SessionState;
using System.Globalization;

namespace TMT.ERP.Commons
{
    public class SessionManager
    {

        protected HttpSessionState session;

        public SessionManager(HttpSessionState httpSessionState)
        {
            session = httpSessionState;
        }

        public static int CurrentCulture
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name == "zh-CN")
                    return 2;
                else if (Thread.CurrentThread.CurrentUICulture.Name == "de-DE")
                    return 1;
                else if (Thread.CurrentThread.CurrentUICulture.Name == "en-US")
                    return 0;
                else
                    return 0;
            }
            set
            {
                //
                // Set the thread's CurrentUICulture.
                //
                if (value == 0)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                else if (value == 1)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
                else if (value == 2)
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");
                else
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
                //
                // Set the thread's CurrentCulture the same as CurrentUICulture.
                //
                Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
        }
    }
}