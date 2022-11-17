using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using CommonLib;
using Newtonsoft.Json;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Collections.Generic;
using TMT.ERP.Services;

namespace TMT.ERP.Controllers
{
    public class PlanningController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        //
        // GET: /Planning/

        public ActionResult Index(int? index = 0)
        {
            //var listPending = new List<ProductivePlanDetail>();
            //var listCompleted = new List<ProductivePlanDetail>();
            //if (index == 0)
            //{
            //    listPending = _db.ProductivePlanDetails.Where(s => s.Status < 2).ToList();
            //    listCompleted = _db.ProductivePlanDetails.Where(s => s.Status == 2).ToList();
            //    ViewBag.Completed = listCompleted;
            //}
            //else
            //{
            //    var machine = _db.Machines.Where(m => m.WorkCenterID == index);
            //    foreach (var item in machine)
            //    {
            //        var listPlanTemp = _db.ProductivePlanDetails.Where(p => p.MachineID == item.ID && p.Status < 2);
            //        listCompleted = _db.ProductivePlanDetails.Where(p => p.MachineID == item.ID && p.Status == 2).ToList();
            //        listPending.AddRange(listPlanTemp);
            //    }
            //    ViewBag.Completed = listCompleted;

            //}
            //ViewBag.WorkCenterID = new SelectList(_db.WorkCenters, "ID", "Code");
            //ViewBag.MpsNo = new Func<int, string>(GetMpsNo);
            //ViewBag.MachineCode = new Func<int, string>(GetMachineNo);
            //ViewBag.WorkOrderNo = new Func<int, string>(GetWorkOrderNo);
            return View("Index");
        }
        /// <summary>
        /// Get Code MPS No When create Productiveplan
        /// </summary>
        /// <returns></returns>
        public string GetCode()
        {
            var item = _db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 4);
            var code = "MPS - 00001";
            if (item != null)
            {
                code = item.InvoicePrefix + item.NextNumber;
            }
            return code;
        }
        /// <summary>
        /// Form Create plan and plan details
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePlan()
        {
            ViewBag.PlanCode = GetCode();//Code of ProductivePlanList
            ViewBag.WorkCenterID = new SelectList(_db.WorkCenters, "ID", "Code");//Dropdown for Workcenter
            ViewBag.MachineID = new SelectList(_db.Machines, "ID", "Code");//Dropdown for machine
            ViewBag.WorkOrderID = new SelectList(_db.WorkOrders, "ID", "Code");//Dropdown for workorder

            //Get List ProductivePlanList
            //List<ProductivePlanDetail> listPlanDetailsPending =
            //    _db.ProductivePlanDetails.Where(p => p.Status < 2).ToList();
            //List<ProductivePlanDetail> listPlanDetailsComplete =
            //    _db.ProductivePlanDetails.Where(p => p.Status == 2).ToList();

            //ViewBag.listPending = listPlanDetailsPending;
            //ViewBag.listCompelete = listPlanDetailsComplete;
            return View();
        }
        /// <summary>
        /// Add New Line 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult AddNewLine(int iIndex, int workcenterId)
        {
            ViewBag.MachineID = new SelectList(_db.Machines.Where(m => m.WorkCenterID == workcenterId), "ID", "Code");
            ViewBag.WorkOrderID = new SelectList(_db.WorkOrders, "ID", "Code");
            ViewBag.Index = iIndex;
            return PartialView("_AddNewLine");
        }
        /// <summary>
        /// Get All Machine by workCenterId
        /// </summary>
        /// <param name="workcenterId"></param>
        /// <returns></returns>
        public PartialViewResult GetMachineByWorkCenter(int workcenterId)
        {
            ViewBag.MachineID = new SelectList(_db.Machines.Where(m => m.WorkCenterID == workcenterId), "ID", "Code");
            return PartialView("_GetMachineByWorkCenter");
        }
        /// <summary>
        /// Call Ajax Function Create plan and plan details
        /// </summary>
        /// <param name="mpsNo"></param>
        /// <param name="createDate"></param>
        /// <param name="workcenterId"></param>
        /// <param name="productivePlanDetail"></param>
        /// <returns></returns>
        public ActionResult SaveProductivePlanLists(string mpsNo, string createDate, int workcenterId, string productivePlanDetail)
        {
            //Add new object ProductivePlanList
            //var objPlan = new ProductivePlanList
            //{
            //    MPSNo = mpsNo,
            //    CreatedDate = createDate != null ? Convert.ToDateTime(createDate) : DateTime.Now,
            //    WorkCenterID = workcenterId,
            //    CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
            //    ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID,
            //    Description = "Add New ProductivePlan" + "  " + mpsNo
            //};
            //_db.ProductivePlanLists.Add(objPlan);
            _db.SaveChanges();
            //var i = objPlan.ID;
            //Add new object ProductivePlanDetail
            dynamic array = JsonConvert.DeserializeObject(productivePlanDetail);
            int idd = 0;
            foreach (var item in array)
            {
                //var oProductivePlanDetail = new ProductivePlanDetail
                //{
                //    MPSNoID = objPlan.ID,
                //    MachineID = item.machineId,
                //    WorkOrderID = item.workorderId,
                //    EarliestStartDate = item.earliestStartDate,
                //    LatestStartDate = item.latestStartDate,
                //    SetupDateTime = item.setupDateTime,
                //    ProcessTime = item.processTime,
                //    ScheduleStartTime = item.scheduleStartTime,
                //    ScheduleEndTime = item.scheduleEndTime,
                //    Status = item.status
                //};
                //_db.ProductivePlanDetails.Add(oProductivePlanDetail);
                //_db.SaveChanges();
                //AddNewMonitor(objPlan.ID, oProductivePlanDetail.MachineID, oProductivePlanDetail.WorkOrderID,
                //              objPlan.WorkCenterID);
                //Add new ProductiveMonitorDetail
            }


            //Set MPS-No identity 1
            var itemSaleSetting = _db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 4);
            if (itemSaleSetting != null)
            {
                var nextNumber = Int32.Parse(itemSaleSetting.NextNumber) + 1;
                var strNext = nextNumber.ToString();
                while (strNext.Length < 5)
                    strNext = "0" + strNext;

                itemSaleSetting.NextNumber = strNext;
                _db.Entry(itemSaleSetting).State = EntityState.Modified;
            }
            _db.SaveChanges();
            return RedirectToAction("CreatePlan");
        }
        /// <summary>
        /// Add New ProductiveMonitorDetail
        /// </summary>
        /// <param name="mpsId"></param>
        /// <param name="machineId"></param>
        /// <param name="workOrderId"></param>
        /// <param name="workcenterId"></param>
        /// <returns></returns>
        public string AddNewMonitor(int mpsId, int machineId, int workOrderId, int workcenterId)
        {
            string result = "success";
            try
            {
                //var objProductPlanDetail = _db.ProductivePlanDetails.Find(mpsId);

                //int j = _db.ProductiveMonitorDetails.OrderByDescending(p => p.ID).FirstOrDefault().ID;
                //IList<WorkOrderDetail> workOrderDetails =
                //    _db.WorkOrderDetails.Where(w => w.WorkOrderID == workOrderId).ToList();

                //foreach (var i in workOrderDetails)
                //{
                //    j++;
                //    var objTemp = new ProductiveMonitorDetail
                //    {
                //        ID = j,
                //        MPSID = mpsId,
                //        MachineID = machineId,
                //        WorkOrderID = workOrderId,
                //        WorkCenterID = workcenterId,
                //        GoodID = i.GoodID,
                //        BeginDate = DateTime.Now,
                //        EndDate = DateTime.Now,
                //        OrderQuantity = i.OrderQuantity
                //    };
                //    _db.ProductiveMonitorDetails.Add(objTemp);
                //}
                _db.SaveChanges();

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        /// <summary>
        /// Get Mps No
        /// </summary>
        /// <param name="mpsNoId"></param>
        /// <returns></returns>
        public string GetMpsNo(int mpsNoId)
        {
            //var obj = _db.ProductivePlanLists.Find(mpsNoId);
            //return obj.MPSNo;
            return "";
        }
        /// <summary>
        /// Get Machine Code
        /// </summary>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public string GetMachineNo(int machineId)
        {
            var obj = _db.Machines.Find(machineId);
            return obj.Code;
        }
        /// <summary>
        /// Get WorkOrder No
        /// </summary>
        /// <param name="workOrderId"></param>
        /// <returns></returns>
        public string GetWorkOrderNo(int workOrderId)
        {
            var obj = _db.WorkOrders.Find(workOrderId);
            return obj.Code;
        }
        /// <summary>
        /// Display ProductionMonitorDetail
        /// </summary>
        /// <param name="mpsId"></param>
        /// <param name="workorderId"></param>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public ActionResult ProductionMonitor(int mpsId, int workorderId, int machineId)
        {
            //Get list productMonitor
            //var productionMonitor =
            //    _db.ProductiveMonitorDetails.Where(p => p.MPSID == mpsId && p.WorkOrderID == workorderId && p.MachineID == machineId);
            ////Mps, workcenter,workorder,machine,create date
            //ViewBag.MpsNo = _db.ProductivePlanLists.Find(mpsId).MPSNo;//MPS No
            //ViewBag.WorkCenterNo = _db.WorkCenters.Find(_db.ProductivePlanLists.Find(mpsId).WorkCenterID).Code;
            //ViewBag.WorkOrderNo = _db.WorkOrders.Find(workorderId).Code;
            //ViewBag.WorkOrderID = workorderId;
            //ViewBag.MachineNo = _db.Machines.Find(machineId).Name;
            //ViewBag.MachineID = machineId;
            //ViewBag.GoodName = new Func<int, string>(GetGoodName);
            //ViewBag.Somnhat = new Func<int, int, int, DateTime>(GetEarliestStartDate);
            //ViewBag.Muonnhat = new Func<int, int, int, DateTime>(GetLatestStartDate);
            //
            return View("Index");
        }
        /// <summary>
        /// Return Good Name in ProductMonitor Detail
        /// </summary>
        /// <param name="goodId"></param>
        /// <returns></returns>
        public string GetGoodName(int goodId)
        {
            var objGood = _db.Goods.Find(goodId);
            return objGood.Name;
        }
        /// <summary>
        /// Return GetEarliestStartDate in ProducttivePlanDetail
        /// </summary>
        /// <param name="mpsId"></param>
        /// <param name="workorderId"></param>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public DateTime GetEarliestStartDate(int mpsId, int workorderId, int machineId)
        {
            var date0 = DateTime.Now;
            //var objTemp =
            //    _db.ProductivePlanDetails.FirstOrDefault(p => p.MPSNoID == mpsId && p.WorkOrderID == workorderId && p.MachineID == machineId);
            //if (objTemp != null && objTemp.EarliestStartDate != null)
            //    date0 = Convert.ToDateTime(objTemp.EarliestStartDate);
            return date0;
        }
        /// <summary>
        /// Return GetEarliestStartDate in ProducttivePlanDetail
        /// </summary>
        /// <param name="mpsId"></param>
        /// <param name="workorderId"></param>
        /// <param name="machineId"></param>
        /// <returns></returns>
        public DateTime GetLatestStartDate(int mpsId, int workorderId, int machineId)
        {
            var date0 = DateTime.Now;
            //var objTemp =
            //    _db.ProductivePlanDetails.FirstOrDefault(p => p.MPSNoID == mpsId && p.WorkOrderID == workorderId && p.MachineID == machineId);
            //if (objTemp != null && objTemp.LatestStartDate != null)
            //    date0 = Convert.ToDateTime(objTemp.LatestStartDate);
            return date0;
        }

        public ActionResult WorkOrderDetails(int workOrderId, int machineId)
        {
            //Get obj WorkOrder Parent
            //var workOrder = _db.WorkOrderDetails.FirstOrDefault(w => w.WorkOrderID == workOrderId && w.ParentGoodID == null);
            //ViewBag.WorkOrder = workOrder;
            ////Get list WorkOrder Child
            //var workOrderDetail = _db.WorkOrderDetails.Where(w => w.WorkOrderID == workOrderId && w.ParentGoodID != null);
            ////Get element
            //ViewBag.GoodName = new Func<int, string>(GetGoodName);
            //ViewBag.StockName = _db.Stocks.Find(_db.WorkOrders.Find(workOrderId).StockID).Name;
            //ViewBag.WorkOrderCode = _db.WorkOrders.Find(workOrderId).Code;
            //ViewBag.MachineName = _db.Machines.Find(machineId).Name;
            //ViewBag.ScheduleStartTime = "";
            //ViewBag.ScheduleEndTime = "";
            return View("Index");
        }
    }
}
