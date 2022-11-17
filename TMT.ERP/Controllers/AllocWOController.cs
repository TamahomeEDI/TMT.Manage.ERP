using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using PagedList;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class AllocWOController : BaseController
    {
        User user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        public static string SortWorkOrderAsc;
        ErpEntities db = new ErpEntities();
        private static string SortTypeAsc;
        private static string SortProductAsc;
        //
        // GET: /AllocWO/
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        public ActionResult Edit(int? AlloWOID)
        {
            List<SelectListItem> workOrderItems = null;
            AllocWO alo = db.AllocWOes.Find(AlloWOID);
            if (alo != null)
            {
                WorkOrder wo = db.WorkOrders.Find(alo.WorkOrderID);
                ViewBag.WorkOrderID = alo.WorkOrderID;
                ViewBag.GoodID = wo.GoodID;
                if (alo.WorkCenterID != null)
                {
                    workOrderItems = new SelectList(db.WorkCenters.Where(st => st.CompanyID == user.CompanyID).ToList(), "ID", "Code", alo.WorkCenterID).ToList();
                }
                else
                {
                    workOrderItems = new SelectList(db.WorkCenters.Where(st => st.CompanyID == user.CompanyID).ToList(), "ID", "Code").ToList();
                }
                //List<SelectListItem> workOrderItems = workOrderList.ToList();
                workOrderItems.Insert(0, (new SelectListItem { Text = @Resources.Resource.Allocate_Edit_Choose_WorkCenter, Value = "", Selected = true }));
                ViewData["workOrderList"] = workOrderItems;
            }
            return View(alo);
        }

        private void populateDropDownList(int? AlloWOID)
        {
            var StockList = new SelectList(db.Stocks, "ID", "Code");
            List<SelectListItem> stockItems = StockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = @Resources.Resource.Allocate_Edit_Choose_WorkCenter, Value = "-1", Selected = true }));
            ViewData["StockList"] = stockItems;

            var workOrderList = new SelectList(db.WorkCenters.Where(st => st.CompanyID == user.CompanyID).ToList(), "ID", "Code");
            List<SelectListItem> workOrderItems = workOrderList.ToList();
            workOrderItems.Insert(0, (new SelectListItem { Text = @Resources.Resource.Allocate_Edit_Choose_WorkCenter, Value = "", Selected = true }));           
            ViewData["workOrderList"] = workOrderItems;
        }


        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            //Get stockoutcard export partical 
            var stockOutList = db.StockOutCards.Where(st => st.Type == 1 && (st.Status == 1 || st.Status == 2)).ToList();
            List<WorkOrder> workOrdersLs = new List<WorkOrder>();
            List<WOStockOutObj> woStockOutLs = new List<WOStockOutObj>();

            var allocList = db.AllocWOes.Where(al => al.Status == 0).OrderByDescending(al => al.ID);
            foreach (var item in allocList)
            {
                    WorkOrder wo = db.WorkOrders.Find(item.WorkOrderID);
                    WOStockOutObj obj = new WOStockOutObj();
                    obj.AlloWOID = item.ID;
                    obj.StockOutCardID = item.StockOutCardID ?? 0;
                    obj.WorkOrderID = wo.ID;
                    obj.WorkOrderNo = wo.Code;
                    if (item.StockOutCard != null)
                    {
                        obj.StockOutCardCode = item.StockOutCard.Code;
                    }
                    switch (wo.ManufacturingType)
                    {
                        case 1:
                            obj.Type = "None Continue";
                                break;
                        case 2:
                                obj.Type = "Continue";
                                break;
                        default:
                                break;

                    }
                    obj.ProductOrder = wo.Good.Code;
                    obj.Description = wo.Description;
                    obj.Qty = db.StockOutCardsDetails.Where(st => st.StockOutCardID == item.StockOutCardID).Sum(qty => qty.Quantity);
                    obj.Status = "Hasn't been allocate.";
                    woStockOutLs.Add(obj);
            }
           
            if (!String.IsNullOrEmpty(searchString))
            {
                woStockOutLs = SearchWorkOrder(woStockOutLs, searchString).ToList();
            }

            if (SortWorkOrderAsc == "asc")
            {
                woStockOutLs = woStockOutLs.OrderBy(x => x.WorkOrderNo).ToList();
                SortWorkOrderAsc = null;
            }
            if (SortWorkOrderAsc == "desc")
            {
                woStockOutLs = woStockOutLs.OrderByDescending(x => x.WorkOrderNo).ToList();
                SortWorkOrderAsc = null;
            }

            if (SortTypeAsc == "asc")
            {
                woStockOutLs = woStockOutLs.OrderBy(x => x.Type).ToList();
                SortTypeAsc = null;
            }

            if (SortTypeAsc == "desc")
            {
                woStockOutLs = woStockOutLs.OrderByDescending(x => x.Type).ToList();
                SortTypeAsc = null;
            }
            if (SortProductAsc == "asc")
            {
                woStockOutLs = woStockOutLs.OrderBy(x => x.Type).ToList();
                SortProductAsc = null;
            }
            if (SortProductAsc == "desc")
            {
                woStockOutLs = woStockOutLs.OrderByDescending(x => x.Type).ToList();
                SortProductAsc = null;
            }
            
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(woStockOutLs.ToPagedList(pageNumber, pageSize));
        }

        private List<WOStockOutObj> SearchWorkOrder(List<WOStockOutObj> woStockOutLs, string searchString)
        {
            var workOrder = new List<WOStockOutObj>();

            List<WOStockOutObj> workOrderCode = woStockOutLs.Where(wo => wo.WorkOrderNo.Contains(searchString)).ToList();

            var workOrderProduct = woStockOutLs.Where(wo => wo.ProductOrder.Contains(searchString));
           workOrder = workOrderCode.Union(workOrderProduct).ToList();

           return workOrder;
        }

        public string SetWorkOrderAsc(string value)
        {
            SortWorkOrderAsc = value;
            return SortWorkOrderAsc;
        }

        public string SetTypeAsc(string value)
        {
            SortTypeAsc = value;
            return SortTypeAsc;
        }

        public string SetProductAsc(string value)
        {
            SortProductAsc = value;
            return SortProductAsc;
        }
        public string SetProductAsc1(string value)
        {
            SortProductAsc = value;
            return SortProductAsc;
        }
    }
}
