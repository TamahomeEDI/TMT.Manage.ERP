using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;

using System.Collections.Generic;
using TMT.ERP.Services;
using System.Web.Script.Serialization;


namespace TMT.ERP.Controllers
{
    public class IncomingShipmentController : BaseController
    {
        #region variable
        private readonly ErpEntities _db = new ErpEntities();
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private static int _pageSize = Constant.DefaultPageSize;
        private static string _sortDateAsc;
        public string SetDateAsc(string value)
        {
            _sortDateAsc = value;
            return _sortDateAsc;
        }
        public int SetPageSize(string pageSize)
        {
            _pageSize = Convert.ToInt32(pageSize);
            return _pageSize;
        }
        #endregion

        #region View
        public ActionResult Index(string currentFilter, string searchString, int? page)
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
            var objList = _db.StockInCards.Where(x => x.Status < 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                objList = objList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            if (_sortDateAsc == "asc")
            {
                objList = objList.OrderBy(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            if (_sortDateAsc == "desc")
            {
                objList = objList.OrderByDescending(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            ViewBag.SourceNo = new Func<int, int, string>(GetSourceNo);
            ViewBag.Stocks = _db.StockInCards.ToList();
            int pageSize = _pageSize;
            ViewBag.PageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult History(string currentFilter, string searchString, int? page)
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
            var objList = _db.StockInCards.Where(x => x.Status == 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                objList = objList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            if (_sortDateAsc == "asc")
            {
                objList = objList.OrderBy(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            if (_sortDateAsc == "desc")
            {
                objList = objList.OrderByDescending(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            ViewBag.SourceNo = new Func<int, int, string>(GetSourceNo);
            ViewBag.Stocks = _db.StockInCards.ToList();
            int pageSize = _pageSize;
            ViewBag.PageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        #endregion
        #region IncomingShipment (Purchase,WorkOrder,Return WorkOrder)
        public ActionResult PurchaseOrder(int id, int isDone)
        {
            ViewBag.ID = id;
            ViewBag.Done = isDone;
            var stock = _db.StockInCards.Find(id);
            var oPurchare = _db.Purchases.Find(stock.SourceID);
            ViewBag.Purchase = oPurchare;
            ViewBag.StockID = _db.Stocks.ToList();
            return View(stock);
        }
        public ActionResult WorkOrder(int id, int isDone)
        {
            ViewBag.ID = id;
            ViewBag.Done = isDone;
            var stock = _db.StockInCards.Find(id);
            ViewBag.StockID = new SelectList(_db.Stocks, "ID", "Name");
            ViewBag.Product = _db.ProductionMonitors.Find(stock.SourceID);
            return View(stock);
        }
        public ActionResult ReturnWorkOrder(int id, int isDone)
        {
            var stock = _db.StockInCards.Find(id);
            ViewBag.Done = isDone;
            ViewBag.StockID = new SelectList(_db.Stocks, "ID", "Name");
            ViewBag.Product = _db.ProductionMonitors.Find(stock.SourceID);
            ViewBag.Good = new Func<int, Good>(GetGood);
            return View(stock);
        }
        #endregion

        #region ViewForPurchase,ViewForWorkOrder and ViewForReturn
        public ActionResult ViewForPurcharse(int stockInCardId)
        {
            var stockCard = _db.StockInCards.Find(stockInCardId);
            var purchase = _db.Purchases.FirstOrDefault(p => p.ID == stockCard.SourceID);
            ViewBag.ShipmentNo = stockCard.Code;
            ViewBag.From = stockCard.Sender;
            ViewBag.PONo = purchase.Code;
            ViewBag.VAT = purchase.UseVAT;
            ViewBag.PurchaseDate = purchase.CreatedDate;
            ViewBag.CurrencyID = new SelectList(_db.Currencies, "ID", "Name", purchase.CurrencyID);
            ViewBag.ID = stockInCardId;

            var stockIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId).Include(g => g.Good).Include(g => g.UOM).Include(g => g.StockInCard);
            return PartialView("_ViewForPurcharse", stockIncard);
        }
        public ActionResult ViewForStockMovement(int stockInCardId)
        {
            var stockCard = _db.StockInCards.Find(stockInCardId);
            var stockMovement = _db.StockMovements.FirstOrDefault(p => p.ID == stockCard.SourceID);
            ViewBag.ShipmentNo = stockCard.Code;
            ViewBag.From = stockCard.Sender;
            ViewBag.SMNo = stockMovement.Code;
            ViewBag.StockMovementDate = stockMovement.CreatedDate;
            ViewBag.ID = stockInCardId;

            var stockIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId).Include(g => g.Good).Include(g => g.UOM).Include(g => g.StockInCard);
            return PartialView("_ViewForStockMovement", stockIncard);
        }
        public ActionResult ViewForWorkOrder(int stockInCardId)
        {
            var stockIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId).Include(g => g.Good).Include(g => g.UOM).Include(g => g.StockInCard);
            var objStockInCard = _db.StockInCards.Find(stockInCardId);
            //  var objProductMonitors = _db.ProductionMonitors.FirstOrDefault(p => p.WorkOrderID == objStockInCard.SourceID);
            #region
            /* ViewBag.ShipmentNo = objStockInCard.Code;
            if (objProductMonitors != null)
            {
                ViewBag.MPSNo = objProductMonitors.Code;
                ViewBag.WorkCenterNo = _db.WorkCenters.Find(objProductMonitors.WorkCenterID).Code;
                ViewBag.WorkOrderNo = _db.WorkOrders.Find(objProductMonitors.WorkOrderID).Code;
                ViewBag.MachineNo = _db.Machines.Find(objProductMonitors.MachineID).Code;
            } */
            ViewBag.CreatedBy = _db.Users.Find(objStockInCard.CreatedUserID).UserName ?? AppContext.RequestVariables.CurrentUser.UserName;
            ViewBag.StockID = new SelectList(_db.Stocks.ToList(), "ID", "Code",
                                             _db.Stocks.Find(objStockInCard.StockID).ID);
            ViewBag.CreaDate = objStockInCard.CreatedDate;
            ViewBag.ReceivedDate = objStockInCard.ReceiveDate ?? DateTime.Now;
            #endregion
            return PartialView("_ViewForWorkOrder", stockIncard);
        }
        public ActionResult ViewForReturn(int stockInCardId)
        {
            var stockIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId).Include(g => g.Good).Include(g => g.UOM).Include(g => g.StockInCard);
            var objStockInCard = _db.StockInCards.Find(stockInCardId);
            // var objProductMonitors = _db.ProductionMonitors.FirstOrDefault(p => p.WorkOrderID == objStockInCard.SourceID);

            ViewBag.ShipmentNo = objStockInCard.Code;
            /* if (objProductMonitors != null)
             {
                 ViewBag.MPSNo = objProductMonitors.Code;
                 ViewBag.WorkCenterNo = _db.WorkCenters.Find(objProductMonitors.WorkCenterID).Code;
                 ViewBag.WorkOrderNo = _db.WorkOrders.Find(objProductMonitors.WorkOrderID).Code;
                 ViewBag.MachineNo = _db.Machines.Find(objProductMonitors.MachineID).Code;
             } */
            ViewBag.CreatedBy = _db.Users.Find(objStockInCard.CreatedUserID).UserName ?? AppContext.RequestVariables.CurrentUser.UserName;
            ViewBag.StockID = new SelectList(_db.Stocks.ToList(), "ID", "Code",
                                             _db.Stocks.Find(objStockInCard.StockID).ID);
            ViewBag.CreaDate = objStockInCard.CreatedDate;
            ViewBag.ReceivedDate = objStockInCard.ReceiveDate ?? DateTime.Now;
            return PartialView("_ViewForReturn", stockIncard);
        }
        #endregion

        public string UpdateStatus(string[] quatity, string[] listid, int stockInCardId, int stockId)
        {
            string result = "success";
            try
            {
                //array goods for stock out
                object InStockGoodsDetails = null;
                List<object> arrInStockDetails = new List<object>();
                //End 

                for (int i = 0; i < listid.Length; i++)
                {
                    int temp = Convert.ToInt32(listid[i]);
                    var stInCardDetail = _db.StockInCardsDetails.Find(temp);
                    //Type = 0(StockInCard For Purchase) update StockInCardDetails
                    if (stInCardDetail.StockInCard.Type == 0)
                    {
                        if (stInCardDetail.Quantity != 0 && stInCardDetail.Quantity != null)
                        {
                            stInCardDetail.Quantity = stInCardDetail.Quantity + Convert.ToInt32(quatity[i]);
                        }
                        else if (stInCardDetail.Quantity == null)
                        {
                            stInCardDetail.Quantity = 0 + Convert.ToInt32(quatity[i]);
                        }
                        else
                        {
                            stInCardDetail.Quantity = Convert.ToInt32(quatity[i]);
                        }
                    }

                    // Add to array for stock out
                    InStockGoodsDetails = new
                    {
                        goodId = stInCardDetail.GoodID,
                        qty = quatity[i],
                        price = stInCardDetail.Price,
                        uomid = stInCardDetail.UOMID
                    };
                    arrInStockDetails.Add(InStockGoodsDetails);
                    //End Add to array for stock out

                    _db.Entry(stInCardDetail).State = EntityState.Modified;
                }
                _db.SaveChanges();
                //Instock 
                string status = DoStockIn(stockInCardId, arrInStockDetails, stockId);
                //End

                var stIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId);
                int qtyRequet = Convert.ToInt32(stIncard.Sum(s => s.QuantityReq));
                int qty = Convert.ToInt32(stIncard.Sum(s => s.Quantity));
                var stockInCard = _db.StockInCards.Find(stockInCardId);
                if (stockInCard.Type == 0)
                {
                    if (qtyRequet >= qty && status.Equals("1"))
                    {
                        stockInCard.Status = 2;
                        stockInCard.ReceiveDate = DateTime.Now;
                        stockInCard.StockID = stockId;
                        stockInCard.ApprovalDate = DateTime.Now;
                        stockInCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                        _db.Entry(stockInCard).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        stockInCard.Status = 1;
                        stockInCard.ReceiveDate = DateTime.Now;
                        stockInCard.StockID = stockId;
                        _db.Entry(stockInCard).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                else if ((stockInCard.Type == 1 || stockInCard.Type == 2) && status.Equals("1"))
                {
                    stockInCard.Status = 2;
                    stockInCard.ReceiveDate = DateTime.Now;
                    stockInCard.StockID = stockId;
                    stockInCard.ApprovalDate = DateTime.Now;
                    stockInCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                    _db.Entry(stockInCard).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        private string DoStockIn(int stockInCardId, List<object> arrInStockDetails, int stockId)
        {
            StockInCard objStockIn = _db.StockInCards.Find(stockInCardId);
            StockMovementService service = new StockMovementService();
            string status = service.InStock(objStockIn, _serializer.Serialize(arrInStockDetails), stockId);
            return status;
        }
        public ActionResult ShipmentDetails(int stockInCardId, int? enable = 1)
        {
            ViewBag.Enable = enable == 1 ? "Enable" : "Disable";
            ViewBag.ID = stockInCardId;
            //Type value set for hidden field . (0 -1 -2 ): Load partialView pucharse , workorder,return ...                                    
            ViewBag.Type = _db.StockInCards.Find(stockInCardId).Type;
            var stockIncard = _db.StockInCardsDetails.Where(s => s.StockInCardID == stockInCardId).ToList();
            return View(stockIncard);
        }
        #region Lookup For StockInCardDetails
        public int GetDiscount(int goodId)
        {
            int discount = 0;
            var purchaseDetails = _db.PurchaseDetails.FirstOrDefault(s => s.GoodID == goodId);
            if (purchaseDetails != null)
            {
                if (purchaseDetails.Discount != null)
                {
                    discount = Convert.ToInt32(purchaseDetails.Discount);
                }
            }
            return discount;
        }
        public double GetSubTotal(int goodId)
        {
            double subtotal = 0;
            var purchaseDetails = _db.PurchaseDetails.FirstOrDefault(s => s.GoodID == goodId);
            if (purchaseDetails != null)
            {
                subtotal = Convert.ToDouble(purchaseDetails.TotalMoney);
            }
            return subtotal;
        }
        public string GetSourceNoForPurchase(int sourceId)
        {
            //sourceId=purcharseId
            string result = "None";
            var oPucharse = _db.Purchases.Find(sourceId);
            if (oPucharse != null)
            {
                result = oPucharse.Code;
            }
            return result;
        }
        public string GetSourceNoForWorkOrder(int sourceId)
        {
            //sourceId=productMonitorId
            string result = "None";
            var oProduct = _db.ProductionMonitors.Find(sourceId);
            if (oProduct != null)
            {
                result = oProduct.AllocWO.WorkOrder.Code;
            }
            return result;
        }
        public string GetSourceNoForStockMovement(int sourceId)
        {
            //sourceId=workorderId
            var result = "None";
            var oStockMove = _db.StockMovements.Find(sourceId);
            if (oStockMove != null)
            {
                result = oStockMove.Code;
            }
            return result;
        }
        #endregion
        public Good GetGood(int goodId)
        {
            return _db.Goods.Find(goodId);
        }


        #region 26 - 12 -2013
        public string GetSourceNo(int sourceId, int type)
        {
            //type : 0 - Purchase 1: WorkOrder
            string value = string.Empty;
            if (type == 0)
            {
                try
                {
                    value = _db.Purchases.Find(sourceId) != null ? _db.Purchases.Find(sourceId).Code : string.Empty;
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }
            }
            if (type == 1)
            {
                try
                {
                    value = _db.ProductionMonitors.Find(sourceId) != null ? _db.ProductionMonitors.Find(sourceId).AllocWO.WorkOrder.Code : string.Empty;
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }

            }
            if (type == 2)
            {
                try
                {
                    value = _db.ProductionMonitors.Find(sourceId) != null ? _db.ProductionMonitors.Find(sourceId).AllocWO.WorkOrder.Code : string.Empty;
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }
            }
            return value;
        }
        #endregion
    }
}
