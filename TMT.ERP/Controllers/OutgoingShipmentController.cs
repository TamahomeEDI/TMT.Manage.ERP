using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Collections.Generic;
using TMT.ERP.Services;
using System.Collections;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using TMT.ERP.Models.Lookups;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class OutgoingShipmentController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private static int _pageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        private static string _sortDateAsc;

        #region View
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
        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.Stocks = _db.StockOutCards.ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var outList = _db.StockOutCards.Where(x => x.Status < 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                outList = outList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            if (_sortDateAsc == "asc")
            {
                outList = outList.OrderBy(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            if (_sortDateAsc == "desc")
            {
                outList = outList.OrderByDescending(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            ViewBag.SourceNo = new Func<int, int, string>(GetSourcNo);
            int pageSize = _pageSize;
            ViewBag.PageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(outList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult History(string currentFilter, string searchString, int? page)
        {
            ViewBag.Stocks = _db.StockOutCards.ToList();
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var outList = _db.StockOutCards.Where(x => x.Status == 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                outList = outList.Where(x => x.Code.Contains(searchString)).ToList();
            }
            if (_sortDateAsc == "asc")
            {
                outList = outList.OrderBy(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            if (_sortDateAsc == "desc")
            {
                outList = outList.OrderByDescending(x => x.CreatedDate).ToList();
                _sortDateAsc = string.Empty;
            }
            ViewBag.SourceNo = new Func<int, int, string>(GetSourcNo);
            int pageSize = _pageSize;
            ViewBag.PageSize = _pageSize;
            int pageNumber = (page ?? 1);
            return View(outList.ToPagedList(pageNumber, pageSize));
        }
        public string GetSourcNo(int targetId, int type)
        {
            //0: Sale 1: WorkOrder 2 : StockMovement            
            string value;
            if (type == 0)
            {
                try
                {
                    if (_db.SaleInvoices.Find(targetId).Code != null)
                    {
                        value = _db.SaleInvoices.Find(targetId).Code;
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }

            }
            else if (type == 1)
            {
                try
                {
                    if (_db.WorkOrders.Find(targetId).Code != null)
                    {
                        value = _db.WorkOrders.Find(targetId).Code;
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }

            }
            else if (type == 2)
            {
                try
                {
                    if (_db.StockMovements.Find(targetId).Code != null)
                    {
                        value = _db.StockMovements.Find(targetId).Code ?? string.Empty;
                    }
                    else
                    {
                        value = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    value = string.Empty;
                }
            }
            else
            {
                value = string.Empty;
            }
            return value;
        }
        #endregion

        #region Form StockOut Invoice, WorkOrder,StockMovement
        public ActionResult OutForSaleInvoice(int stockOutCardId, int type)
        {
            ViewBag.Enable = type;
            ViewBag.StockCardID = stockOutCardId;
            var oStockOutCard = _db.StockOutCards.Find(stockOutCardId);
            var oSaleInvoice = _db.SaleInvoices.Find(oStockOutCard.TargetID);
            ViewBag.objStockOut = oStockOutCard;
            ViewBag.objSale = oSaleInvoice;
            var ooStockOutCard = _db.StockOutCardsDetails.Where(x => x.StockOutCardID == stockOutCardId).ToList();
            return View(ooStockOutCard);

        }
        public ActionResult OutForWorkOrder(int stockoutCardId, int? type)
        {
            ViewBag.EnableDone = type;
            ViewBag.ID = stockoutCardId;
            var oStockOutCard = _db.StockOutCards.Find(stockoutCardId);
            var oWorkOrder = _db.WorkOrders.Find(_db.StockOutCards.Find(stockoutCardId).TargetID);
            ViewBag.StockOutCard = oStockOutCard;
            ViewBag.oWorkOrder = oWorkOrder;
            var ooStocks = _db.StockOutCardsDetails.Where(s => s.StockOutCardID == stockoutCardId).ToList();
            return View(ooStocks);
        }
        public ActionResult OutForStockMovement(int stockOutCardId, int type)
        {
            ViewBag.Enable = type;
            var oStockOutCard = _db.StockOutCards.Find(stockOutCardId);
            var oStockMovement = _db.StockMovements.Find(oStockOutCard.TargetID);
            ViewBag.StockOutCard = oStockOutCard;
            ViewBag.StockMovement = oStockMovement;
            var ooStocks = _db.StockOutCardsDetails.Where(s => s.StockOutCardID == stockOutCardId).ToList();
            return View(ooStocks);
        }
        #endregion

        #region Do StockOut
        public string UpdateStatus(string[] quatity, string[] listid, int stockOutCardId)
        {
            string result = "success";
            try
            {
                //array goods for stock out
                var arrOutStockDetails = new List<object>();
                //End 
                for (int i = 0; i < listid.Length; i++)
                {
                    int temp = Convert.ToInt32(listid[i]);
                    var stOutCardDetail = _db.StockOutCardsDetails.Find(temp);
                    if (stOutCardDetail.QuantityRef != 0 && stOutCardDetail.QuantityRef != null)
                    {
                        stOutCardDetail.QuantityRef = stOutCardDetail.QuantityRef + Convert.ToInt32(quatity[i]);
                    }
                    else if (stOutCardDetail.QuantityRef == null)
                    {
                        stOutCardDetail.QuantityRef = 0 + Convert.ToInt32(quatity[i]);
                    }
                    else
                    {
                        stOutCardDetail.QuantityRef = Convert.ToInt32(quatity[i]);
                    }

                    // Add to array for stock out
                    object outStockGoodsDetails = new
                    {
                        goodId = stOutCardDetail.GoodID,
                        qty = quatity[i],
                        Price = stOutCardDetail.Price,
                        uomid = stOutCardDetail.UOMID
                    };
                    arrOutStockDetails.Add(outStockGoodsDetails);
                    //End Add to array for stock out

                    _db.Entry(stOutCardDetail).State = EntityState.Modified;
                }
                _db.SaveChanges();

                //OutStock process
                string status = DoStockOut(stockOutCardId, arrOutStockDetails);
                //End
                var stOutCard = _db.StockOutCardsDetails.Where(s => s.StockOutCardID == stockOutCardId);
                int qtyRequet = Convert.ToInt32(stOutCard.Sum(s => s.QuantityRef));
                int qty = Convert.ToInt32(stOutCard.Sum(s => s.Quantity));
                var stockOutCard = _db.StockOutCards.Find(stockOutCardId);
                if (qtyRequet == qty && status.Equals("1"))
                {
                    stockOutCard.Status = 2;
                    stockOutCard.ApprovalDate = DateTime.Now;
                    if (stockOutCard.Type == 1)
                    {
                        var oWorkOrder = _db.WorkOrders.Find(Convert.ToInt32(stockOutCard.TargetID));

                    }
                    _db.Entry(stockOutCard).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    stockOutCard.Status = 1;
                    _db.Entry(stockOutCard).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                // Create Allocate base on StockOutCard workorder
                if (stockOutCard.Type == 1)
                {
                    CreateAllocOnStockOutFromWO(stockOutCardId);
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        private void CreateAllocOnStockOutFromWO(int stockOutCardId)
        {
            StockOutCard stOut = _db.StockOutCards.Find(stockOutCardId);
            if (stOut != null) {
                var oWorkOrder = _db.WorkOrders.Find(Convert.ToInt32(stOut.TargetID));
                AllocWO oAlloc = new AllocWO();
                string code = TMT.ERP.Commons.Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.AllocateWorkOrder));
                oAlloc.Code = code;
                oAlloc.WorkOrderID = oWorkOrder.ID;          
                oAlloc.ToStockID = stOut.StockID;
                oAlloc.GoodID = oWorkOrder.GoodID??0;
                oAlloc.StockOutCardID = stockOutCardId;
                //  createdEmployeeID = Commons.AppContext.RequestVariables.CurrentUser.ID;  
                oAlloc.Status = 0;
                _db.AllocWOes.Add(oAlloc);
                _db.SaveChanges();
                Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.AllocateWorkOrder), Constant.CODE_LENGTH, "");
                foreach(var item in stOut.StockOutCardsDetails) {
                    AllocWOItemDetail oAlloWODetail = new AllocWOItemDetail();
                    oAlloWODetail.AllocWOID = oAlloc.ID;                    
                    oAlloWODetail.GoodID = item.GoodID;
                    oAlloWODetail.ImplementQuantity = 0;
                    oAlloWODetail.AllocQty = item.Quantity;
                    oAlloWODetail.BalanceQuantity = item.Quantity;
                    oAlloWODetail.RemainQuantity = item.Quantity;
                    oAlloWODetail.Price = item.Price ??0;
                    oAlloWODetail.UOMID = item.UOMID;
                    _db.AllocWOItemDetails.Add(oAlloWODetail);
                }
                _db.SaveChanges();
            }

            
        }
        private string DoStockOut(int stockOutCardId, List<object> arrOutStockDetails)
        {
            StockOutCard oStockOut = _db.StockOutCards.Find(stockOutCardId);
            var service = new StockMovementService();
            var status = service.outStock(oStockOut, _serializer.Serialize(arrOutStockDetails));
            return status;
        }
        #endregion
    }
}
