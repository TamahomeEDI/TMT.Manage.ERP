using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for AccountService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
   [System.Web.Script.Services.ScriptService]
    public class AccountService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]

        public string GetTaxRateByAccountID(int AccountID)
        {
            object result = "Success";
            //var goodManage = new GoodManage();
            var manager = new GenericManager<Account>();
            try
            {

                var account = manager.FindById(AccountID);
                if (account != null)
                {
                    result = new
                    {
                        TaxRate = account.TaxRateID,
                        TaxRateValue = account.TaxRate.Rate
                    };
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }
    }
}
