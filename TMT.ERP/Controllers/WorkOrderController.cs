using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models.Lookups;
using PagedList;
using CommonLib;
using System.ComponentModel;
using System.Web.UI.DataVisualization.Charting;

using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class WorkOrderController : BaseController
    {
        //
        // GET: /WorkOrder/
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        private readonly GenericManager<WorkOrder> manager = new GenericManager<WorkOrder>();

        private SelectList CreateManufacturingTypes(int? selectedValue)
        {
            SelectList manufacturingTypes = new SelectList(new[]{
            new SelectListItem {Value = "1", Text = "None Continue"},
            new SelectListItem {Value = "2", Text = "Continue"}
            }, "Value", "Text", selectedValue.ToString());
            return manufacturingTypes;
        }

        private object CreateGoodList()
        {
            var productTypeList = Constant.ProductTypeList;
            var productTypeArr = productTypeList.Split(',').Select(n => int.Parse(n)).ToList();
            var manager = new GenericManager<ActualGoodInStock>();
            object goodItems = null;
            if (productTypeArr.Count>0)
                goodItems = manager.Get().Where(x => productTypeArr.Contains(x.Good.ProductTypeID)).Select(x => new { Text = x.Good.Code, Value = x.GoodID }).Distinct().ToList();
            else
                goodItems = manager.Get().Select(x => new { Text = x.Good.Code, Value = x.GoodID }).Distinct().ToList();
            return goodItems;
        }

        private List<SelectListItem> InitGoods()
        {
            var goodList = new SelectList(new GenericManager<Good>().Get(), "ID", "Code");
            List<SelectListItem> goodItems = goodList.ToList();
            return goodItems;
        }

        private List<SelectListItem> CreateBomList()
        {
            var bomManager = new GenericManager<BOM>();
            var itemList = new SelectList(bomManager.Get().Where(x => x.Active == true), "ID", "Name");
            List<SelectListItem> items = itemList.ToList();
            return items;
        }

        public IList<SelectListItem> CreateManufacturingList()
        {
            //IList<SelectListItem> list = Enum.GetValues(typeof(ManufacturingType)).Cast<ManufacturingType>().Select(x => new SelectListItem()                
            //{
            //    Text = CommonLib.EnumHelper.GetDescription(x),
            //    Value = ((int)x).ToString()
            //}).ToList();
            return null;
        }

        public ActionResult Index()
        {
            //return View();
            var manager = new GenericManager<WorkOrder>();
            ViewBag.Draft = manager.Get().Where(x => x.Status == 0).Count();
            ViewBag.Done = manager.Get().Where(x => x.Status == 1).Count();
            return View(manager.Get().Where(x => x.Status == 0).AsQueryable());
        }

        public ActionResult WorkOrderList()
        {
            var manager = new GenericManager<WorkOrder>();
            ViewBag.Draft = manager.Get().Where(x =>x.Status==0).Count();
            ViewBag.Done = manager.Get().Where(x => x.Status == 1).Count();
            return View(manager.Get().Where(x => x.Status == 0).AsQueryable());
        }

        public ActionResult Create()
        {
            var user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
            ViewBag.NextCode = TMT.ERP.Commons.Utility.GetCode(CommonLib.EnumHelper.GetDescription(CodeType.WorkOrder));
            var stockItems = new GenericManager<Stock>().Get().Where(x=>x.CurrentCompanyID==user.CompanyID);
            ViewBag.StockID = stockItems;
            ViewData["ManufacturingType"] = CreateManufacturingTypes(1);
            ViewBag.GoodID = CreateGoodList();
            ViewBag.BOMID = CreateBomList();
            return View();
        }

        public PartialViewResult AddNewLine()
        {
            ViewBag.GoodID = InitGoods();
            return PartialView("_AddNewLine");
        }

        public ActionResult Edit(int id = 0)
        {
            var productTypeList = Constant.ProductTypeList;
            var productTypeArr = productTypeList.Split(',').Select(n => int.Parse(n)).ToList();
            var goodItems = new GenericManager<ActualGoodInStock>().Get().Where(x => productTypeArr.Contains(x.Good.ProductTypeID)).Select(x => new { Text = x.Good.Code, Value = x.GoodID }).Distinct().ToList();


            var manager = new GenericManager<WorkOrder>();
            WorkOrder workOrder = manager.Get().Where(x => x.ID == id).FirstOrDefault();
            if (workOrder == null)
            {
                return HttpNotFound();
            }
            int bomID = 0;
            var goodID = workOrder.GoodID;
            var bomManeger = new GenericManager<BOM>();
            var bomItem = bomManeger.Find(x => x.GoodID == goodID);
            if (bomItem != null)
                bomID = bomItem.ID;
            ViewData["ManufacturingType"] = CreateManufacturingTypes(workOrder.ManufacturingType);
            //ViewData["GoodItems"] = new SelectList(new GenericManager<Good>().Get().Where(x => x.ProductTypeID == 1 || x.ProductTypeID == 2).AsQueryable(), "ID", "Name", workOrder.GoodID);
            //ViewData["GoodItems"] = new SelectList(new GenericManager<ActualGoodInStock>().Get().Where(x => productTypeArr.Contains(x.Good.ProductTypeID)), "ID", "Name", workOrder.GoodID);
            //ViewData["GoodItems"] = new SelectList(CreateGoodList(), "Value", "Text", workOrder.GoodID);
            ViewData["GoodItems"] = new SelectList(goodItems, "Value", "Text", workOrder.GoodID);
            ViewData["Stock"] = new SelectList(new GenericManager<Stock>().Get().AsQueryable(), "ID", "Name", workOrder.StockID);
            ViewData["BOMID"] = new SelectList(bomManeger.Get().Where(x=>x.Active==true).AsQueryable(), "ID", "Name", bomID);
            ViewBag.GoodID = InitGoods();
            ViewBag.SubGoodID = CreateGoodList();
            return View(workOrder);
        }

        public ActionResult GetWorkOrderDone()
        {
            return PartialView("_GetWorkOrderDone", GetWorkOrderByStatus(1));
        }

        private IQueryable<WorkOrder> GetWorkOrderByStatus(int status)
        {
            var manager = new GenericManager<WorkOrder>();
            return manager.Get().Where(x => x.Status == status).AsQueryable();
            //var compID=3;
            ////var totalUser = new GenericManager<User>().Get().Select() .Where(x => x.ID == compID).ToList().Sum(x => x.ID);
            //var totalUser = new GenericManager<User>().Get().Select(x => { var expens = x.ExpenseClaims; return new {expens.Sum(y=>y.User.ID)}; }).Where(x => x.ID == compID).ToList().Sum(x => x.ID);
        }

        public ActionResult ReOrder(int id = 0)
        {
            var manager = new GenericManager<WorkOrder>();
            WorkOrder workOrder = manager.Get().Where(x => x.ID == id).FirstOrDefault();
            if (workOrder == null)
            {
                return HttpNotFound();
            }

            ViewBag.NextCode = TMT.ERP.Commons.Utility.GetCode(CommonLib.EnumHelper.GetDescription(CodeType.WorkOrder));
            ViewData["GoodItems"] = new SelectList(new GenericManager<Good>().Get().Where(x => x.ProductTypeID == 1 || x.ProductTypeID == 2).AsQueryable(), "ID", "Name", workOrder.GoodID);
            ViewData["Stock"] = new SelectList(new GenericManager<Good>().Get().AsQueryable(), "ID", "Name", workOrder.StockID);
            return View(workOrder);
        }


        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public ViewResult All(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;            
            ViewBag.PageSize = PageSize;
            int pageNumber = 1;
            var workOrders = SearchWorkOrder(currentFilter, searchString, page, null, out pageNumber);
            return View(workOrders.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Pending(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.PageSize = PageSize;
            int pageNumber = 1;
            int status = 0;
            var workOrders = SearchWorkOrder(currentFilter, searchString, page, status, out pageNumber);
            return View(workOrders.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult AwaitingApproval(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.PageSize = PageSize;            
            int pageNumber = 1;
            int status = 1;
            var workOrders = SearchWorkOrder(currentFilter, searchString, page, status, out pageNumber);
            return View(workOrders.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult AwaitingOutgoingShipment(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.PageSize = PageSize;
            int pageNumber = 1;
            int status = 2;
            var workOrders = SearchWorkOrder(currentFilter, searchString, page, status, out pageNumber);
            return View(workOrders.ToPagedList(pageNumber, PageSize));
        }

        public ActionResult Completed(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.PageSize = PageSize;
            int pageNumber = 1;
            int status = 3;
            var workOrders = SearchWorkOrder(currentFilter, searchString, page, status, out pageNumber);
            return View(workOrders.ToPagedList(pageNumber, PageSize));
        }

        public IList<WorkOrder> SearchWorkOrder(string currentFilter, string searchString, int? page, int? status, out int pageNumber)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            IList<WorkOrder> workOrders = null;
            if(status!=null)
                workOrders = manager.Get().Where(i => i.Status == status).OrderByDescending(x => x.ID).ToList();
            else
                workOrders = manager.Get().Where(i => i.Status != -1).OrderByDescending(x => x.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                workOrders = SearchWorkOrder(workOrders, searchString).ToList();
            }
            pageNumber = (page ?? 1);
            return workOrders;
        }


        public IList<WorkOrder> SearchWorkOrder(IList<WorkOrder> workOrder, string sStr)
        {
            return workOrder.Where(x => x.Code.Contains(sStr)).ToList();
        }

        public void GetValue(DateTime fromDate, DateTime toDate)
        {
            var manager = new GenericManager<Company>();
            //var items = manager.Get().Where(filter: x => x.SaleInvoices.FirstOrDefault);
        }

    }
}
