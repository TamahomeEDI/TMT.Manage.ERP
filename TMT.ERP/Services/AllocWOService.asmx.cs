using CommonLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for AllocWOService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class AllocWOService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        ErpEntities db = new ErpEntities();
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveAllocWO(int? allowWOID, int? workCenterID, string description, int status, string workOrderDetail)
        {
            object result = 0;
           // int totalAllocate = 0;
            try
            {
               
                AllocWO oAlloc = null;
                if (allowWOID.GetValueOrDefault() > 0)
                {
                    oAlloc = db.AllocWOes.Find(allowWOID);
                    oAlloc.WorkCenterID = workCenterID ?? 0;
                    oAlloc.Description = description;
                    oAlloc.Status = status;
                    dynamic array = JsonConvert.DeserializeObject(workOrderDetail);
                    foreach (var item in array)
                    {
                        int AllocID = SafeConvert.ToNullable<int>(((string)item.AlloWODetailsID).Replace("{", "").Replace("}", "")) ?? 0;
                        AllocWOItemDetail oAlloWODetail = db.AllocWOItemDetails.Find(AllocID);                       
                        int allocQty = SafeConvert.ToNullable<int>(((string)item.allocQty).Replace("{", "").Replace("}", "")) ?? 0;
                        oAlloWODetail.BalanceQuantity = SafeConvert.ToNullable<int>(((string)item.remainQty).Replace("{", "").Replace("}", "")) ?? 0;
                        oAlloWODetail.RemainQuantity = oAlloWODetail.RemainQuantity ?? 0 + allocQty;
                        db.Entry(oAlloWODetail).State = System.Data.EntityState.Modified;
                    }
                    db.SaveChanges();
                    SetStockOutAlocateFinished(oAlloc.ID);
                    setWorkOrderStatus(oAlloc.WorkOrderID);
                    result = oAlloc.ID;
                }


            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        private void SetStockOutAlocateFinished(int allocateID)
        {
            AllocWO alloc = db.AllocWOes.Find(allocateID);          
            int? balanceQty = alloc.AllocWOItemDetails.Where(all => all.AllocWOID == alloc.ID).Sum(all => all.BalanceQuantity);         
            if (balanceQty == 0)
            {
                //StockOut allocate all 
                //stoOut.Status = 3;
                alloc.Status = 1;
                db.Entry(alloc).State = System.Data.EntityState.Modified;
                db.SaveChanges();
            }
        }
        #region set stockoutcard allocated
        private void setStockOutCardStatus(int? stockOutCardID, int workOrderID)
        {
            WorkOrder wo = db.WorkOrders.Find(workOrderID);
            StockOutCard stout = db.StockOutCards.Find(stockOutCardID);
            int totalAllocQty = 0;
            int totalQtyReq = 0;
            var lstAlloc = db.AllocWOes.Where(al => al.WorkOrderID == workOrderID).ToList();
            foreach (var item in lstAlloc)
            {
                totalAllocQty += item.AllocWOItemDetails.Sum(al => al.AllocQty);
            }
            var lstQtyReq = db.StockOutCards.Where(st => st.TargetID == workOrderID && st.Type == 1 && (st.Status == 1 || st.Status == 2)).ToList();
            foreach (var item in lstQtyReq)
            {
                totalQtyReq += item.StockOutCardsDetails.Sum(al => al.Quantity);
            }
            if (totalAllocQty == totalQtyReq)
            {
                wo.Status = 5;
                db.SaveChanges();
            }
            
        }
        #endregion

        #region allocalte all Stockout cards set workorder compleate
        public void setWorkOrderStatus(int? workOrderID)
        {
            WorkOrder wo = db.WorkOrders.Find(workOrderID);
            int totalAllocQty = 0;
            int totalQtyReq = 0;
            var lstAlloc = db.AllocWOes.Where(al => al.WorkOrderID == workOrderID).ToList();
            foreach (var item in lstAlloc)
            {
                totalAllocQty += item.AllocWOItemDetails.Sum(al => al.AllocQty);
            }
            var lstQtyReq = db.StockOutCards.Where(st => st.TargetID == workOrderID && st.Type == 1 && (st.Status == 1 || st.Status == 2)).ToList();
            foreach (var item in lstQtyReq)
            {
                totalQtyReq += item.StockOutCardsDetails.Sum(al => al.Quantity);
            }
            if (totalAllocQty == totalQtyReq)
            {
                wo.Status = 5;
                db.SaveChanges();
            }
            
        }
        #endregion
    }
}
