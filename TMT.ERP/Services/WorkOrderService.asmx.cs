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
    public class WorkOrderService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveWorkOrder(int workOrderID, string workOrderCode, string createDate, int createUserID, int? approvalUserID, int stockID, int parentGoodID, int parentQuantity, string description, string workOrderDetail, int manufacturingType, string customerJob, int status, int companyID)
        {
            string result = "OK";
            var manager = new GenericManager<WorkOrder>();
            try
            {
                result = CheckItemInWorkOrder(workOrderDetail);
                if (result.Contains("OK"))
                {
                    WorkOrder oWorkOrder = null;
                    if (workOrderID > 0)
                    {
                        oWorkOrder = manager.FindById(workOrderID);
                        oWorkOrder.WorkOrderDetails.ToList().ForEach(x => manager.Delete(x));
                    }
                    else
                    {
                        if (manager.Get().Where(x => x.Code == workOrderCode).ToList().Count > 0)
                        {
                            result = "ERROR: " + Resources.Resource.Common_WOUnique;
                            return serializer.Serialize(result);
                        }

                        oWorkOrder = new WorkOrder();
                        manager.Add(oWorkOrder);
                        oWorkOrder.ImplementQuantity = 0;
                        oWorkOrder.RemainQuantity = parentQuantity;
                        oWorkOrder.CreatedUserID = createUserID;
                        oWorkOrder.CompanyID = companyID;
                    }
                    oWorkOrder.Status = status;
                    oWorkOrder.Code = workOrderCode;
                    oWorkOrder.CreatedDate = DateTime.Parse(createDate);
                    oWorkOrder.ApprovalUserID = approvalUserID;
                    oWorkOrder.ManufacturingType = manufacturingType;
                    oWorkOrder.CustomerJob = customerJob;
                    oWorkOrder.StockID = stockID;
                    oWorkOrder.GoodID = parentGoodID;
                    oWorkOrder.OrderQuantity = parentQuantity;
                    oWorkOrder.Description = description;

                    dynamic array = JsonConvert.DeserializeObject(workOrderDetail);

                    foreach (var item in array)
                    {
                        WorkOrderDetail oWorkOrderDetail = null;
                        var goodID = Int32.Parse(((string)item.SubGoodId).Replace("{", "").Replace("}", ""));
                        //recheck
                        //if (workOrderID > 0)
                        //{
                        //    oWorkOrderDetail = oWorkOrder.WorkOrderDetails.FirstOrDefault(x => x.WorkOrderID == workOrderID & x.GoodID == goodID);
                        //    if (oWorkOrder.WorkOrderDetails.Count > 0)
                        //        oWorkOrder.WorkOrderDetails.Clear();
                        //}
                        //if (oWorkOrderDetail == null)
                        //{
                        //    oWorkOrderDetail = new WorkOrderDetail();
                        //    oWorkOrder.WorkOrderDetails.Add(oWorkOrderDetail);
                        //}

                        oWorkOrderDetail = new WorkOrderDetail();
                        oWorkOrder.WorkOrderDetails.Add(oWorkOrderDetail);
                        oWorkOrderDetail.WorkOrderID = oWorkOrder.ID;
                        oWorkOrderDetail.GoodID = goodID;
                        oWorkOrderDetail.OrderQuantity = item.SubQuantity;
                        oWorkOrderDetail.ImplementQuantity = 0;
                        oWorkOrderDetail.RemainQuantity = item.SubQuantity;
                    }
                    manager.Save();
                    if (workOrderID == 0)
                        TMT.ERP.Commons.Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.WorkOrder), Constant.CODE_LENGTH, "");
                    result = oWorkOrder.ID.ToString();
                }
            }
            catch (Exception ex) { result = "ERROR: " + ex.Message; }
            return serializer.Serialize(result);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string UpdateStatus(string workOrders, int status)
        {
            object result = "Success";
            try
            {
                var manager = new GenericManager<WorkOrder>();
                dynamic items = JsonConvert.DeserializeObject(workOrders);
                foreach (var item in items)
                {
                    var itemID = Int32.Parse(((string)item.WorkOrderID).Replace("{", "").Replace("}", ""));
                    WorkOrder workOrder = manager.FindById(itemID);
                    if (workOrder != null)
                    {
                        workOrder.Status = status;
                    }
                }
                manager.Save();
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        //[WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CheckItemInWorkOrder(string workOrderDetail)
        {
            string result = "";
            try
            {
                dynamic array = JsonConvert.DeserializeObject(workOrderDetail);
                bool checkQuantity = true;
                foreach (var item in array)
                {
                    var goodID = Int32.Parse(((string)item.SubGoodId).Replace("{", "").Replace("}", ""));
                    var orderQuantity = item.SubQuantity;
                    var goodInActual = new GenericManager<ActualGoodInStock>().Get().Where(x => x.GoodID == goodID);
                    var remainQuantity = goodInActual.Sum(x => x.RemainQuantity);
                    if (Convert.ToInt32(((string)orderQuantity).Replace("{", "").Replace("}", "")) > Convert.ToInt32(remainQuantity))
                    {
                        checkQuantity = false;
                        result += goodInActual.FirstOrDefault().Good.Code + "(" + orderQuantity.ToString() + "/" + remainQuantity.ToString() + ")" + ",";
                    }
                }

                if (checkQuantity)
                {
                    result = "OK";
                }
                else
                {
                    result = "ERROR: " + Resources.Resource.WO_Err_ItemNotEnough + " " + result.Substring(0, result.Length - 1);
                }
            }
            catch (Exception ex)
            {
                result = "ERROR: " + ex.ToString();
            }

            //return serializer.Serialize(result);
            return result;
        }

    }
}
