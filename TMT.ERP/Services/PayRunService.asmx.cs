using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using Newtonsoft.Json;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for WorkOrderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class PayRunService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //para:
        //payItemDetails: JSON format (PayItemID, Quantity, Rate)
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePayRunEmployee(int payRunEmployeeID, string payItemDetails, int status)
        {
            object result = "Success";
            //try
            //{
            //    var manager = new GenericManager<PayRunEmployeeDetail>();
            //    dynamic array = JsonConvert.DeserializeObject(payItemDetails);
            //    foreach (var item in array)
            //    {
            //        var payItemID = Int32.Parse(((string)item.PayItemID).Replace("{", "").Replace("}", ""));
            //        var payItemDetail = manager.Get().First(x => x.PayItemID == payItemID && x.PayRunEmployeeID == payRunEmployeeID);
            //        if (payItemDetail == null)
            //        {
            //            payItemDetail = new PayRunEmployeeDetail();
            //            payItemDetail.PayRunEmployeeID = payRunEmployeeID;
            //        }
            //        var quantity = Int32.Parse(((string)item.Quantity));
            //        var rate = double.Parse(((string)item.Rate));
            //        payItemDetail.PayItemID = payItemID;
            //        payItemDetail.Quantity = quantity;
            //        payItemDetail.Rate = rate;
            //        payItemDetail.TotalPay = quantity * rate;
            //    }

            //    manager.Save();
            //}
            //catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPayItemDetails(int payRunEmployeeID)
        {
            //var manager = new GenericManager<PayRunEmployeeDetail>();
            //IList<PayRunEmployeeDetail> payItemDetails = null;
            //payItemDetails = manager.Get(filter: x => x.PayRunEmployeeID == payRunEmployeeID);
            //if (payItemDetails != null)
            //{
            //    var result = serializer.Serialize(payItemDetails.Select(x => new { PayItemID = x.PayItemID, PayItemName = x.PayItem.Name, Quantity = x.Quantity, Rate = x.Rate, Amount = x.Amount }).OrderBy(x => x.PayItemID));
            //    return result;
            //}
            //else
            //{
            //    return serializer.Serialize("No items");
            //}
            return "success";
        }



    }
}
