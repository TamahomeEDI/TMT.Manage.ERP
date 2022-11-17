using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using CommonLib;
using Newtonsoft.Json;
using PagedList;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Collections.Generic;
using TMT.ERP.Models.Lookups;
using TMT.ERP.Services;


namespace TMT.ERP.Controllers
{
    public class ProductionMonitorsController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        private static int _pageSize = Constant.DefaultPageSize;

        private object Alloc()
        {
            var manager = new GenericManager<AllocWO>();
            var allocs = manager.Get().OrderBy(x => x.ID).Skip(manager.Get().Count).Select(x => new { Text = x.Code, Value = x.ID }).Distinct().ToList();
            return allocs;
        }

        #region View,Edit and List
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            var ooProduct = _db.ProductionMonitors.ToList();
            ViewBag.Product = ooProduct;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var objList = _db.ProductionMonitors.Where(x => x.Status == 0).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                objList = objList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            ViewBag.WorkOrderNo = new Func<int, string>(GetWorkOrderNo);
            ViewBag.Item = new Func<int, string>(GetItem);
            ViewBag.PageSize = _pageSize;
            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Process(string currentFilter, string searchString, int? page)
        {
            var ooProduct = _db.ProductionMonitors.ToList();
            ViewBag.Product = ooProduct;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var objList = _db.ProductionMonitors.Where(x => x.Status == 1).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                objList = objList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            ViewBag.WorkOrderNo = new Func<int, string>(GetWorkOrderNo);
            ViewBag.Item = new Func<int, string>(GetItem);
            ViewBag.PageSize = _pageSize;
            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Completed(string currentFilter, string searchString, int? page)
        {
            var ooProduct = _db.ProductionMonitors.ToList();
            ViewBag.Product = ooProduct;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var objList = _db.ProductionMonitors.Where(x => x.Status == 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                objList = objList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            ViewBag.WorkOrderNo = new Func<int, string>(GetWorkOrderNo);
            ViewBag.Item = new Func<int, string>(GetItem);
            ViewBag.PageSize = _pageSize;
            int pageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            ViewBag.Code = Utility.GetCode(CodeType.WorkProcess.GetDescription());
            var user = AppContext.RequestVariables.CurrentUser;
            ViewBag.WorkCenterID =
                _db.WorkCenters.Where(x => x.CompanyID == user.CompanyID).OrderByDescending(x => x.ID).ToList();
            return View();
        }
        public string SaveProductMonitors(string code, int workCenterId, int machineId, int allocId, string scheduleStartTime, string scheduleEndTime, string realStartTime, string endStartTime, string processTime, int status)
        {
            string result = "success";
            try
            {
                var objAlloc = _db.AllocWOes.Find(allocId);
                var objMachine = _db.Machines.Find(machineId);
                var objTemp = new ProductionMonitor();
                objTemp.Code = code;
                objTemp.WorkCenterID = workCenterId;
                objTemp.MachineID = machineId;
                objTemp.AllocWOID = allocId;
                objTemp.ScheduleStartTime = !string.IsNullOrEmpty(scheduleStartTime) ? DateTime.Parse(scheduleStartTime) : DateTime.Now;
                objTemp.ScheduleEndTime = !string.IsNullOrEmpty(scheduleEndTime) ? DateTime.Parse(scheduleEndTime) : DateTime.Now;
                objTemp.RealStartTime = !string.IsNullOrEmpty(realStartTime) ? DateTime.Parse(realStartTime) : DateTime.Now;
                objTemp.EndStartTime = !string.IsNullOrEmpty(endStartTime) ? DateTime.Parse(endStartTime) : DateTime.Now;
                objTemp.ProcessTime = !string.IsNullOrEmpty(processTime) ? Convert.ToInt32(processTime) : 0;
                objTemp.Status = status;
                objTemp.GoodID = objAlloc.GoodID;
                objTemp.OrderQuantity = objAlloc.WorkOrder.OrderQuantity;
                objTemp.ImplementQuantity = objAlloc.WorkOrder.ImplementQuantity;
                objTemp.RemainQuantity = objAlloc.WorkOrder.RemainQuantity;
                objTemp.ImplementDate = DateTime.Now;
                objTemp.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                _db.ProductionMonitors.Add(objTemp);
                Utility.UpdateNextNumber(CodeType.WorkProcess.GetDescription(), Constant.CODE_LENGTH, "");
                if (status == 1)
                {
                    objMachine.Status = 2;
                    _db.Entry(objMachine).State = EntityState.Modified;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string EditProductMonitors(int id, int workCenterId, int allocId, int machineId, string scheduleStartTime, string scheduleEndTime, int status)
        {
            string result = "success";
            var obj = _db.ProductionMonitors.Find(id);
            try
            {
                obj.WorkCenterID = workCenterId;
                obj.MachineID = machineId;
                obj.AllocWOID = allocId;
                obj.ScheduleStartTime = !string.IsNullOrEmpty(scheduleStartTime) ? DateTime.Parse(scheduleStartTime) : DateTime.Now;
                obj.ScheduleEndTime = !string.IsNullOrEmpty(scheduleEndTime) ? DateTime.Parse(scheduleEndTime) : DateTime.Now;
                obj.Status = status;
                if (status == 1)
                {
                    obj.RealStartTime = DateTime.Now;
                    obj.EndStartTime = DateTime.Now;
                    obj.ImplementDate = DateTime.Now;
                    var oMachine = _db.Machines.Find(machineId);
                    oMachine.Status = 2;
                    _db.Entry(obj).State = EntityState.Modified;
                }
                _db.Entry(obj).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public ActionResult Edit(int id = 0)
        {
            var objProduct = _db.ProductionMonitors.Find(id);
            if (objProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkCenterID = new SelectList(_db.WorkCenters, "ID", "Code");
            ViewBag.MachineID = _db.Machines.Where(x => x.WorkCenterID == objProduct.WorkCenterID && x.Status == 3).OrderByDescending(x => x.ID).ToList();
            ViewBag.AllocID = _db.AllocWOes.Where(x => x.WorkCenterID == objProduct.WorkCenterID && x.Status == 1).OrderByDescending(x => x.ID).ToList();
            return View(objProduct);
        }
        public ActionResult ViewCompleted(int id)
        {
            var objProduct = _db.ProductionMonitors.Find(id);
            ViewBag.WorkOrderNo = GetWorkOrderNo(objProduct.AllocWOID);
            ViewBag.GetItem = GetItem(Convert.ToInt32(objProduct.GoodID));
            ViewBag.QuanlityOrder = GetQuanlityOrder(objProduct.AllocWOID);
            ViewBag.Complete = _db.AllocWOItemDetails.Where(x => x.AllocWOID == objProduct.AllocWO.ID).ToList();
            int i = _db.AllocWOItemDetails.Count(x => x.AllocWOID == objProduct.AllocWO.ID);
            ViewBag.Good = new Func<int, Good>(GetGoods);
            return View(objProduct);
        }
        public ActionResult ViewProcess(int id)
        {
            var obj = _db.ProductionMonitors.Find(id);
            ViewBag.WorkCenterID = new SelectList(_db.WorkCenters, "ID", "Code");
            ViewBag.MachineID = new SelectList(_db.Machines.Where(x => x.WorkCenterID == obj.WorkCenterID), "ID", "Code");
            ViewBag.AllocID = new SelectList(_db.AllocWOes.Where(x => x.WorkCenterID == obj.WorkCenterID), "ID", "Code");
            ViewBag.Item = GetItem(Convert.ToInt32(obj.GoodID));

            var allocs = _db.AllocWOItemDetails.Where(x => x.AllocWOID == obj.AllocWOID).ToList();
            ViewBag.ListAllocate = allocs;
            ViewBag.Good = new Func<int, Good>(GetGoods);
            return View(obj);
        }
        public string Delete(int id)
        {
            string result = "success";
            try
            {
                var objProduct = _db.ProductionMonitors.Find(id);
                _db.ProductionMonitors.Remove(objProduct);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion

        public string ApprovedSave(int id, string actualEndDate, string actualProduct, string listAlloc)
        {
            string result = "success";
            try
            {
                var obj = _db.ProductionMonitors.Find(id);
                UpdateProductionMonitor(0, obj, actualEndDate, actualProduct);
                UpdateAllocate(listAlloc);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string ApprovedSubmit(int id, string actualEndDate, string actualProduct, string listAlloc)
        {
            string result = "success";
            try
            {
                var obj = _db.ProductionMonitors.Find(id);
                CreateStock(0, obj, actualProduct, listAlloc);
                UpdateAllocate(listAlloc);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string ApprovedCompleted(int id, string actualEndDate, string actualProduct, string listAlloc)
        {
            string result = "success";
            try
            {
                var obj = _db.ProductionMonitors.Find(id);
                UpdateProductionMonitor(2, obj, actualEndDate, actualProduct);
                CreateStock(1, obj, actualProduct, listAlloc);
                UpdateAllocate(listAlloc);
                ReturnMaterial(id);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #region Get Reference && Implement variable
        private object GetMachineList(int wId)
        {
            var manager = new GenericManager<Machine>();
            var machines = manager.Get().Where(m => m.WorkCenterID == wId && m.Status != 0).Select(x => new { Text = x.Code, Value = x.ID }).Distinct().ToList();
            return machines;
        }


        public PartialViewResult GetMachine(int wId)
        {
            //Lấy tất cả machine thuộc workcenter và trạng thái Available(Status==3)            
            ViewBag.MachineID = _db.Machines.Where(m => m.WorkCenterID == wId && m.Status == 3).OrderByDescending(x => x.ID).ToList();
            return PartialView("_GetMachine");
        }
        public PartialViewResult GetAlloCate(int wId)
        {
            //Đổi sang lấy tất cả các allocate có Status=1 nghĩa là complete allocate để sản xuất(Status==1)
            var ooProduct = _db.ProductionMonitors.ToList();
            var ooAlocate = _db.AllocWOes.Where(x => x.WorkCenterID == wId && x.Status == 1).ToList();

            foreach (var oProduct in ooProduct)
            {
                ooAlocate.Remove(oProduct.AllocWO);
            }


            ViewBag.AllocID = ooAlocate.ToList();
            return PartialView("_GetAlloCate");
        }
        public PartialViewResult GetGood(int allocId)
        {
            var good = _db.AllocWOes.Find(allocId).Good;
            var goodItem = string.Empty;
            if (good != null)
            {
                goodItem = good.Name ?? good.Code;
            }

            ViewBag.GoodName = goodItem;
            ViewBag.Item = new Func<int, string>(GetItem);
            return PartialView("_GetGood", _db.AllocWOItemDetails.Where(x => x.AllocWOID == allocId).ToList());
        }
        public string GetItem(int goodId)
        {
            string result;
            try
            {
                result = _db.Goods.Find(goodId).Code;
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }
        public int SetPageSize(string pageSize)
        {
            _pageSize = Convert.ToInt32(pageSize);
            return _pageSize;
        }
        public string GetWorkOrderNo(int allocId)
        {
            return _db.AllocWOes.Find(allocId).WorkOrder.Code;
        }
        public double GetProcessTime(string startDate, string endDate)
        {
            var x = Convert.ToDateTime(endDate);
            var y = Convert.ToDateTime(startDate);
            var z = x.Subtract(y);
            return z.TotalMinutes;
        }
        public Good GetGoods(int goodId)
        {
            return _db.Goods.Find(goodId);
        }
        public string GetQuanlityOrder(int allocId)
        {
            var objAlloc = _db.AllocWOes.Find(allocId);
            return _db.WorkOrders.Find(objAlloc.WorkOrderID).OrderQuantity.ToString();
        }
        #endregion
        #region Process Planing
        public void UpdateProductionMonitor(int status, ProductionMonitor obj, string actualEndDate, string actualProduct)
        {
            if (status == 0)
            {
                if (obj.ImplementQuantity != null)
                {
                    obj.ImplementQuantity = obj.ImplementQuantity + Convert.ToInt32(actualProduct);
                }
                else
                {
                    obj.ImplementQuantity = Convert.ToInt32(actualProduct);
                }
            }
            if (status == 1)
            {

            }
            if (status == 2)
            {
                if (obj.ImplementQuantity != null)
                {
                    obj.ImplementQuantity = obj.ImplementQuantity + Convert.ToInt32(actualProduct);
                }
                else
                {
                    obj.ImplementQuantity = Convert.ToInt32(actualProduct);
                }
                obj.EndStartTime = !string.IsNullOrEmpty(actualEndDate) ? DateTime.Parse(actualEndDate) : obj.EndStartTime;
                var processTimes = DateTime.Parse(actualEndDate).Subtract(Convert.ToDateTime(obj.RealStartTime));
                obj.ProcessTime = Convert.ToInt32(processTimes.TotalMinutes);
                var objMachine = _db.Machines.Find(obj.MachineID);
                objMachine.Status = 3;
                obj.Status = 2;
                _db.Entry(objMachine).State = EntityState.Modified;
            }

            _db.Entry(obj).State = EntityState.Modified;
            _db.SaveChanges();
        }
        public void UpdateAllocate(string listAlloc)
        {
            var obj = new AllocWOItemDetail();

            dynamic array = JsonConvert.DeserializeObject(listAlloc);
            foreach (var x in array)
            {
                var Id = Convert.ToInt32(SafeConvert.ToStringOrEmpty(x.id.Value));
                var used = Convert.ToInt32(SafeConvert.ToStringOrEmpty(x.used.Value));

                var objTemp = _db.AllocWOItemDetails.Find(Id);
                if (objTemp.ImplementQuantity == null)
                {
                    objTemp.ImplementQuantity = 0 + used;
                }
                else
                {
                    objTemp.ImplementQuantity = objTemp.ImplementQuantity + used;
                }

                objTemp.RemainQuantity = objTemp.AllocQty - objTemp.ImplementQuantity;
                _db.Entry(objTemp).State = EntityState.Modified;
            }
            _db.SaveChanges();
        }
        public bool CreateStock(int status, ProductionMonitor obj, string actualProduct, string listAlloc)
        {
            bool result = true;
            try
            {
                var ostock = new StockInCard();
                ostock.Code = Utility.GetCode(TMT.ERP.Models.Lookups.CodeType.IncomingShipment.GetDescription());
                ostock.CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;
                ostock.StockID = obj.AllocWO.ToStockID;
                ostock.CreatedDate = DateTime.Now;
                ostock.SourceID = obj.ID;
                ostock.Type = 1;
                ostock.Sender = AppContext.RequestVariables.CurrentUser.UserName;
                ostock.Status = 0;
                _db.StockInCards.Add(ostock);
                _db.SaveChanges();
                Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment), Constant.CODE_LENGTH, "");
                var stockIn = new StockInCardsDetail();
                stockIn.StockInCardID = ostock.ID;
                stockIn.GoodID = obj.AllocWO.WorkOrder.Good.ID;
                stockIn.UOMID = obj.AllocWO.WorkOrder.Good.UOM.ID;
                if (status == 0)
                {
                    stockIn.QuantityReq = !string.IsNullOrEmpty(actualProduct) ? (int)Math.Round(Convert.ToDouble(actualProduct)) : 0;
                    float price = 0;
                    dynamic array = JsonConvert.DeserializeObject(listAlloc);
                    foreach (var x in array)
                    {
                        var Id = Convert.ToInt32(SafeConvert.ToStringOrEmpty(x.id.Value));
                        var used = Convert.ToInt32(SafeConvert.ToStringOrEmpty(x.used.Value));

                        var objTemp = _db.AllocWOItemDetails.Find(Id);
                        price += objTemp.Price * used;
                    }
                    stockIn.Price = price / Convert.ToInt32(actualProduct);
                    stockIn.TotalMoney = Convert.ToInt32(actualProduct) * (price / Convert.ToInt32(actualProduct));
                }
                if (status == 1)
                {
                    stockIn.QuantityReq = obj.ImplementQuantity;
                    double price = 0;
                    var listAllocates = _db.AllocWOItemDetails.Where(x => x.AllocWOID == obj.AllocWO.ID).ToList();
                    foreach (var x in listAllocates)
                    {
                        price += x.Price * x.AllocQty;
                    }
                    stockIn.Price = price / obj.ImplementQuantity;
                    stockIn.TotalMoney = obj.ImplementQuantity * (price / obj.ImplementQuantity);
                }
                stockIn.DateIn = DateTime.Now;
                _db.StockInCardsDetails.Add(stockIn);
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        public string ReturnMaterial(int id)
        {
            string result = "success";
            try
            {
                var obj = _db.ProductionMonitors.Find(id);
                var items = _db.AllocWOItemDetails.Where(x => x.AllocWOID == obj.AllocWOID);
                var manager = new GenericManager<StockInCard>();
                var managerStockInCardDetail = new GenericManager<StockInCardsDetail>();
                var managerAllocateDetail = new GenericManager<AllocWOItemDetail>();

                var ostock = new StockInCard();
                ostock.Code = Utility.GetCode(TMT.ERP.Models.Lookups.CodeType.IncomingShipment.GetDescription());
                ostock.CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;
                ostock.StockID = obj.AllocWO.ToStockID;
                ostock.CreatedDate = DateTime.Now;
                ostock.SourceID = obj.ID;
                ostock.Type = 2;
                ostock.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                ostock.Sender = AppContext.RequestVariables.CurrentUser.UserName;
                ostock.Status = 0;
                manager.Add(ostock);
                foreach (var x in items)
                {
                    if (x.RemainQuantity > 0)
                    {
                        var stockIn = new StockInCardsDetail();
                        var good = _db.Goods.Find(x.GoodID);
                        stockIn.StockInCardID = ostock.ID;
                        stockIn.GoodID = good.ID;
                        stockIn.UOMID = good.UOM.ID;
                        stockIn.Quantity = x.AllocQty;
                        stockIn.QuantityReq = x.RemainQuantity;
                        stockIn.DateIn = DateTime.Now;
                        stockIn.Price = good.InputPrice ?? 0;
                        managerStockInCardDetail.Add(stockIn);

                        Utility.UpdateNextNumber(CodeType.IncomingShipment.GetDescription(),Constant.CODE_LENGTH, "");
                        AllocWOItemDetail objAllocateDetail = managerAllocateDetail.FindById(x.ID);
                        objAllocateDetail.RemainQuantity = 0;
                        managerAllocateDetail.Update(objAllocateDetail);
                    }
                }

                manager.Save();
                managerStockInCardDetail.Save();
                managerAllocateDetail.Save();
                var objectTemp = _db.ProductionMonitors.Find(id);
                objectTemp.Status = 2;
                _db.Entry(objectTemp).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion
    }
}
