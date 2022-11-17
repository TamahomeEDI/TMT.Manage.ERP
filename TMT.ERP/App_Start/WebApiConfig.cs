using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using TMT.ERP.Commons;

namespace TMT.ERP
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
            name: "logout",
            routeTemplate: "api/{controller}/{action}/{sessionId}",
            defaults: new { sessionId = RouteParameter.Optional, action = "Get" }
            );

            config.Routes.MapHttpRoute(
                      name: "login",
                      routeTemplate: "api/{controller}/{action}/{deviceId}/{userName}/{passWord}",
                      defaults: new { deviceId = RouteParameter.Optional, userName = RouteParameter.Optional, passWord = RouteParameter.Optional, action = "Get" }
                  );
            config.Routes.MapHttpRoute(
                      name: "getCompany",
                      routeTemplate: "api/{controller}/{action}/{sessionId}/{fromDate}/{toDate}",
                      defaults: new { sessioId = RouteParameter.Optional, fromDate = RouteParameter.Optional, toDate = RouteParameter.Optional }
                  );
            config.Routes.MapHttpRoute(
             name: "getTotalSale",
            routeTemplate: "api/{controller}/{action}/{sessionId}/{companyId}/{fromDate}/{toDate}",
            defaults: new { sessioId = RouteParameter.Optional, companyId = RouteParameter.Optional, fromDate = RouteParameter.Optional, toDate = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
             name: "getPersons",
            routeTemplate: "api/{controller}/{action}/{sessionId}/{fromDate}/{toDate}",
            defaults: new { sessionId = RouteParameter.Optional, fromDate = RouteParameter.Optional, toDate = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
             name: "getClients",
            routeTemplate: "api/{controller}/{action}/{sessionId}/{fromDate}/{toDate}",
            defaults: new { sessionId = RouteParameter.Optional, fromDate = RouteParameter.Optional, toDate = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
             name: "getListInvoice",
            routeTemplate: "api/{controller}/{action}/{sessionId}/{category}",
            defaults: new { sessionId = RouteParameter.Optional, category = RouteParameter.Optional}
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );





            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}