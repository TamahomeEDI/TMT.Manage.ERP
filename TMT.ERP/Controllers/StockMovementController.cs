using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Data;
using TMT.ERP.Services;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;
using PagedList;

namespace TMT.ERP.Controllers
{
    public class StockMovementController : BaseController
    {
        //
        // GET: /StockMovement/
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        private ErpEntities db = new ErpEntities();
        private List<SelectListItem> PopulateGoodsDropDownList(object selectedGoods = null)
        {
            //var goodsQuery = from d in db.Goods
            //                 orderby d.Name
            //                 select d;
            var list = new SelectList(new List<Good>(), "ID", "Code", selectedGoods);
            List<SelectListItem> goodItems = list.ToList();
            goodItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));

            ViewData["GoodList"] = goodItems;



            return goodItems;
        }

        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        public ActionResult Index(int? tabNum)
        {
            ViewBag.TabNum = tabNum;
            var stockMovements = new GenericManager<StockMovement>().Get().AsQueryable().OrderByDescending(s => s.ID).ToList();
            // var stockMovements = new GenericManager<StockMovement>().Get().AsQueryable().ToList();
            ViewBag.CountDraft = stockMovements.Count(x => x.Status == 0);
            ViewBag.CountHistory = stockMovements.Count(x => x.Status == 1);
            return View(stockMovements);
        }

        public ActionResult GetStockMovement(int? page, int? type)
        {

            var stockMovements = new GenericManager<StockMovement>().Get().Where(x => x.Status == type).OrderByDescending(s => s.ID).AsQueryable();
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.TabNum = type;
            switch (type)
            {
                case 0:
                    return View("_Draft", stockMovements.ToPagedList(pageNumber, pageSize));
                case 1:
                    return View("_History", stockMovements.ToPagedList(pageNumber, pageSize));
                default:
                    break;
            }
            return View();
        }

        public ViewResult Draft(string searchString, int? page, int? type)
        {
            var stockMovements = new GenericManager<StockMovement>().Get().AsQueryable().OrderByDescending(s => s.ID).ToList();

            ViewBag.CountDraft = stockMovements.Count(x => x.Status == 0);
            ViewBag.CountHistory = stockMovements.Count(x => x.Status == 1);
            stockMovements = stockMovements.Where(x => x.Status == 0).OrderByDescending(s => s.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                stockMovements = stockMovements.Where(x => x.Code.Contains(searchString)).ToList();
            }

            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(stockMovements.ToPagedList(pageNumber, pageSize));
        }

        public ViewResult History(string searchString, int? page, int? type)
        {
            var stockMovements = new GenericManager<StockMovement>().Get().AsQueryable().OrderByDescending(s => s.ID).ToList();
            ViewBag.CountDraft = stockMovements.Count(x => x.Status == 0);
            ViewBag.CountHistory = stockMovements.Count(x => x.Status == 1);
            stockMovements = stockMovements.Where(x => x.Status == 1).OrderByDescending(s => s.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                stockMovements = stockMovements.Where(x => x.Code.Contains(searchString)).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(stockMovements.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Code");
            PopulateGoodsDropDownList();
            // ViewBag.ddlStockID = new SelectList(db.Stocks, "ID", "Code", "");

            var StockList = new SelectList(db.Stocks, "ID", "Code");
            List<SelectListItem> stockItems = StockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["StockList"] = stockItems;

            ViewBag.ddlStockIDDes = new SelectList(db.Stocks, "ID", "Code");
            return View();
        }

        public ActionResult Edit(int? id)
        {
            var stockMovements = new GenericManager<StockMovement>().FindById(id);
            ViewBag.ddlStockID = new SelectList(db.Stocks, "ID", "Code", stockMovements.FromStockID);
            ViewBag.ddlStockIDDes = new SelectList(db.Stocks, "ID", "Code", stockMovements.ToStockID);
            List<SelectListItem> lstGood = getGoodsByStockID(stockMovements.FromStockID);
            ViewBag.GoodID = lstGood;
            ViewBag.AccountIDFrom = new SelectList(db.Accounts, "ID", "Code", stockMovements.FromAccountID);
            ViewBag.AccountIDTo = new SelectList(db.Accounts, "ID", "Code", stockMovements.ToAccountID);
            return View(stockMovements);
        }
        [HttpPost]
        public ActionResult Create(StockMovement stock, FormCollection formCollection)
        {
            ViewBag.ddlStockID = new SelectList(db.Stocks, "ID", "Code");
            ViewBag.ddlStockIDDes = new SelectList(db.Stocks, "ID", "Code");
            PopulateGoodsDropDownList();
            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Code");
            return View();
        }

        public ActionResult GetGoodByStockID(int? StockID)
        {
            var lstActualGoodInStock = db.ActualGoodInStocks.Where(ac => ac.StockID == StockID).ToList();
            List<SelectListItem> lstGood = getGoodsByStockID(StockID);
            ViewBag.Good = new SelectList(lstGood, "ID", "Code");
            // return Json(lstStock, JsonRequestBehavior.AllowGet);
            return PartialView("_tblStockMovement", lstActualGoodInStock);
        }

        public List<SelectListItem> getGoodsByStockID(int? StockID)
        {
            List<Good> lstGoods = new List<Good>();
            List<int> lsID = new List<int>();
            var lstActualGoodInStock = db.ActualGoodInStocks.Where(ac => ac.StockID == StockID).ToList();
            foreach (var item in lstActualGoodInStock)
            {
                if (!lsID.Contains(item.GoodID ?? 0))
                {
                    Good oGood = db.Goods.Find(item.GoodID);
                    lstGoods.Add(new Good { ID = oGood.ID, Code = oGood.Code });
                    lsID.Add(item.GoodID ?? 0);
                }
            }

            var list = new SelectList(lstGoods, "ID", "Code");
            List<SelectListItem> goodItems = list.ToList();
            //  goodItems.Insert(0, (new SelectListItem { Text = "+ Add new inventory...", Value = "0" }));
            goodItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            ViewData["GoodList"] = goodItems;
            return goodItems;
        }

        [HttpPost]
        public string Approved(string[] stockMovementID, int? status)
        {
            string result = "success";
            try
            {
                //  dynamic array = JsonConvert.DeserializeObject(purchaseID);
                foreach (string item in stockMovementID)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        StockMovement oStockMove = db.StockMovements.Find(Int64.Parse(item));
                        oStockMove.Status = status ?? 0;
                        //oPurchase.Code = GetBillNoPurchase();
                        db.Entry(oStockMove).State = EntityState.Modified;
                    }
                }
                //db.SaveChanges();
                //Create Incoming and Outcoming shipment          
                Utility.CreateOutgomingShipment(Int32.Parse(CommonLib.EnumHelper.GetDescription(OutType.StockMovement)), 1, stockMovementID);
                Utility.CreateIncomingShipment(Int32.Parse(CommonLib.EnumHelper.GetDescription(InType.StockMovement)), 1, stockMovementID);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }


        [HttpPost]
        public string DelStockMovement(string[] stockMovementID)
        {

            string result = "success";
            try
            {
                for (int i = 0; i < stockMovementID.Length; i++)
                {

                    if (!String.IsNullOrEmpty(stockMovementID[i]))
                    {
                        int stockMoveID = Int32.Parse(stockMovementID[i]);
                        var lstStockMoveDetails = db.StockMovementDetails.Where(p => p.StockMovementID == stockMoveID).ToList();
                        foreach (StockMovementDetail oDetail in lstStockMoveDetails)
                        {
                            db.StockMovementDetails.Remove(oDetail);
                        }
                        StockMovement oStockMovement = db.StockMovements.Find(Int64.Parse(stockMovementID[i]));
                        db.StockMovements.Remove(oStockMovement);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
