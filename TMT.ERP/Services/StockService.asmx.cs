using System;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.Commons;
using TMT.ERP.Controllers;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using Newtonsoft.Json;
using CommonLib;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for StockService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class StockService : System.Web.Services.WebService
    {
        List<KeyValuePair<int, int>> listKeyVal = new List<KeyValuePair<int, int>>();
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        //For Stock out for Sale Invoice
        Dictionary<int, int> dictionary = new Dictionary<int, int>();
        Dictionary<int, bool> dicGood = new Dictionary<int, bool>();
        int? realAmount = 0;
        int? totalAmount = 0;
        string shipmentNo = "";
        //End 

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //Update StockIn
        public string SavePurchaseStockIn(int? id, string code, int createdUserId, int stockId, string createdDate, int purchaseId, string sender, float? tax, float? totalMoney, int status, string stockInCardsDetails)
        {
            object result = 0;
            try
            {
                var db = new ErpEntities();
                StockInCard oStockInCard = null;
                if (id.GetValueOrDefault() > 0)
                {
                    oStockInCard = db.StockInCards.Find(id);
                    var stockInCardDetail = db.StockInCardsDetails.Where(s => s.StockInCardID == oStockInCard.ID);
                    db.Entry(stockInCardDetail).State = System.Data.EntityState.Deleted;
                }
                else
                {
                    oStockInCard = new StockInCard();
                    db.StockInCards.Add(oStockInCard);
                }
                //Add new StockInCard (Purchase)
                oStockInCard.Code = code;
                oStockInCard.CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;
                oStockInCard.ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID;
                oStockInCard.StockID = stockId;
                oStockInCard.CreatedDate = Convert.ToDateTime(createdDate);
                oStockInCard.SourceID = purchaseId;
                //Type:0-Purchase;1-GoodProcess;2-Movement
                oStockInCard.Type = 0;
                oStockInCard.Sender = sender;
                oStockInCard.Tax = tax;
                oStockInCard.TotalMoney = totalMoney;
                //Status:0 - Created, 1 - Approval, 2 - Completed
                oStockInCard.Status = status;
                oStockInCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                db.SaveChanges();

                //Add new StockInCardDetails

                dynamic array = JsonConvert.DeserializeObject(stockInCardsDetails);
                foreach (var item in array)
                {
                    StockInCardsDetail oStockInCardsDetail = new StockInCardsDetail();
                    oStockInCardsDetail.StockInCardID = oStockInCard.ID;

                    oStockInCardsDetail.GoodID = item.goodId;
                    oStockInCardsDetail.UOMID = item.uomId;
                    oStockInCardsDetail.Quantity = item.quantity;
                    oStockInCardsDetail.DateIn = item.dateIn;

                    db.StockInCardsDetails.Add(oStockInCardsDetail);
                }
                db.SaveChanges();
                result = oStockInCard.ID;
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
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return _serializer.Serialize(result);
        }
        public string GetCodeStockIn()
        {
            ErpEntities db = new ErpEntities();
            var item = db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 2);
            var code = "SH_00001";
            if (item != null)
            {
                code = item.InvoicePrefix + item.NextNumber;
                var nextNumber = Int32.Parse(item.NextNumber) + 1;
                var strNext = nextNumber.ToString();
                while (strNext.Length < 5)
                    strNext = "0" + strNext;

                item.NextNumber = strNext;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return code;
        }
        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePurchaseStockInAppvored(string[] purchaseID, int userId)
        {
            string result = "";
            bool isOutGoingShipment = false;
            string shipmentNo = "";
            try
            {
                var db = new ErpEntities();
                foreach (var item in purchaseID)
                {
                    #region Add StockIncard
                    Purchase purchase = db.Purchases.Find(Convert.ToInt32(item));
                    if (purchase.Type != 1)
                    {
                        isOutGoingShipment = true;
                        var oStockInCard = new StockInCard();

                        oStockInCard.Code = GetCodeStockIn();
                        oStockInCard.CreatedUserID = userId;
                        oStockInCard.ApprovalUserID = userId;
                        oStockInCard.StockID = purchase.StockID;
                        oStockInCard.CreatedDate = DateTime.Now;
                        oStockInCard.SourceID = purchase.ID;
                        oStockInCard.Type = 0;
                        oStockInCard.Sender = purchase.Reference ?? "@@@@";
                        oStockInCard.Tax = purchase.Tax;
                        oStockInCard.TotalMoney = purchase.TotalMoney;
                        oStockInCard.Status = 0;
                        oStockInCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                        //Type:0-Purchase -------------Status:0 - Draft                    
                        db.StockInCards.Add(oStockInCard);
                        db.SaveChanges();
                        shipmentNo += oStockInCard.Code + ",";
                       #endregion
                        #region Add StockIncardDetail
                        int itemTemp = Convert.ToInt32(item);
                        var purchaseDetailList = db.PurchaseDetails.Where(p => p.PurchaseID == itemTemp);
                        foreach (var value in purchaseDetailList)
                        {
                            var stockInCardsDetail = new StockInCardsDetail
                                                         {
                                                             StockInCardID = oStockInCard.ID,
                                                             GoodID = value.GoodID,
                                                             UOMID = value.UOMID,
                                                             QuantityReq = value.Quantity,
                                                             DateIn = DateTime.Now,
                                                             Discount = value.Discount,
                                                             Price = value.Price,
                                                             TotalMoney = value.TotalMoney
                                                         };

                            db.StockInCardsDetails.Add(stockInCardsDetail);
                        }
                        #endregion
                        #region AccountTrans
                        Utility.CreateAccountTransaction(1, Int32.Parse(item), null, null);
                        #endregion
                    }
              }
                db.SaveChanges();
                if (isOutGoingShipment && purchaseID.Length > 1)
                {
                    result = "Shipment No " + shipmentNo.Substring(0, shipmentNo.Length - 1) + " have been created in Incomingshipment Screen.";
                }
                else if(isOutGoingShipment && purchaseID.Length == 1)
                {
                    result = "A Shipment No " + shipmentNo.Substring(0, shipmentNo.Length - 1) + " have been created in Incomingshipment Screen.";
                }
                else 
                    return "";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #region 08-10-2013
        public string GetCodeStockOut()
        {
            ErpEntities db = new ErpEntities();
            var item = db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 3);
            var code = "SH_OUT-00001";
            if (item != null)
            {
                code = item.InvoicePrefix + item.NextNumber;
                var nextNumber = Int32.Parse(item.NextNumber) + 1;
                var strNext = nextNumber.ToString();
                while (strNext.Length < 5)
                    strNext = "0" + strNext;

                item.NextNumber = strNext;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return code;
        }
        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveSaleInvoiceStockOutAppvored(string[] saleInvoiceId, int userId)
        {
            string result = "";
            string shipmentNo = "";
            try
            {
                //StockInCard NOT NULL : Code,CreatedUserID,StockID,CreatedDate,Sender,Status
                ErpEntities db = new ErpEntities();
                foreach (var item in saleInvoiceId)
                {
                    #region Add StockOutCard
                    SaleInvoice saleInvoice = db.SaleInvoices.Find(Convert.ToInt32(item));
                    StockOutCard oStockOutCard = new StockOutCard();
                    //Add new StockInCard (Purchase)
                    oStockOutCard.Code = GetCodeStockOut();
                    oStockOutCard.CreatedUserID = userId;
                    oStockOutCard.ApprovalUserID = userId;
                    oStockOutCard.StockID = Convert.ToInt32(saleInvoice.StockID);
                    oStockOutCard.CreatedDate = saleInvoice.CreatedDate != null ? saleInvoice.CreatedDate : DateTime.Now;
                    oStockOutCard.TargetID = saleInvoice.ID;
                    //Type:0-Purchase;1-GoodProcess;2-Movement
                    oStockOutCard.Type = 0;
                    oStockOutCard.Receiver = saleInvoice.Description ?? "No Content";
                    //oStockOutCard.Tax = saleInvoice.TaxAmountType;
                    //oStockOutCard.TotalMoney = saleInvoice.TotalMoney;
                    //Status:0 - Draft ; Status:1 - Done
                    oStockOutCard.Status = 0;
                    oStockOutCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                    db.StockOutCards.Add(oStockOutCard);
                    db.SaveChanges();
                    shipmentNo += oStockOutCard.Code + ",";
                    #endregion
                    #region Add StockIncardDetail

                    int itemTemp = Convert.ToInt32(item);
                    var saleInvoiceList = db.SaleInvoiceDetails.Where(s => s.SaleInvoiceID == itemTemp);
                    foreach (var value in saleInvoiceList)
                    {
                        StockOutCardsDetail stockOutCardsDetail = new StockOutCardsDetail();
                        stockOutCardsDetail.StockOutCardID = oStockOutCard.ID;
                        stockOutCardsDetail.GoodID = value.GoodID;
                        stockOutCardsDetail.UOMID = value.UOMID;
                        stockOutCardsDetail.Quantity = Convert.ToInt32(value.Quantity);
                        stockOutCardsDetail.DateOut = DateTime.Now;
                        stockOutCardsDetail.Discount = value.Discount;
                        stockOutCardsDetail.Price = value.Price;
                        stockOutCardsDetail.TotalMoney = value.TotalMoney;

                        db.StockOutCardsDetails.Add(stockOutCardsDetail);
                        //db.SaveChanges();
                    }

                    #endregion

                }
                db.SaveChanges();
                if (saleInvoiceId.Length > 1)
                {
                    result = "Shipment No " + shipmentNo.Substring(0, shipmentNo.Length - 1) + " have been created in Incomingshipment Screen.";
                }
                else
                {
                    result = "A Shipment No " + shipmentNo.Substring(0, shipmentNo.Length - 1) + " have been created in Incomingshipment Screen.";
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
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion
        #region 14-10-2013
        [WebMethod]
        public string ReturnWorkOrder(int workOrderId)
        {
            string result = "success";
            try
            {
                var db = new ErpEntities();
                WorkOrder workOrder = db.WorkOrders.Find(workOrderId);
                var workOrderDetail = db.WorkOrderDetails.Where(w => w.WorkOrderID == workOrderId);
                foreach (var item in workOrderDetail)
                {
                    if (item.RemainQuantity > 0)
                    {
                        SaveAfterReturn(item);
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        [WebMethod(EnableSession = true)]
        public string SaveAfterReturn(WorkOrderDetail workOrderDetail)
        {
            string result = "success";
            var db = new ErpEntities();
            var workOrder = db.WorkOrders.Find(workOrderDetail.WorkOrderID);
            try
            {
                var oStockInCard = new StockInCard
                                       {
                                           Code = GetCodeStockIn(),
                                           CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
                                           ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID,
                                           StockID = Convert.ToInt32(workOrder.StockID),
                                           CreatedDate = DateTime.Now,
                                           SourceID = workOrder.ID,
                                           Type = 2,
                                           Sender = AppContext.RequestVariables.CurrentUser.UserName,
                                           Status = 0,
                                           CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID
                                       };
                db.StockInCards.Add(oStockInCard);
                db.SaveChanges();
                var oStockInCardsDetail = new StockInCardsDetail
                                              {
                                                  StockInCardID = oStockInCard.ID,
                                                  GoodID = Convert.ToInt32(workOrderDetail.GoodID),
                                                  UOMID = db.UOMs.Find(db.Goods.Find(workOrderDetail.GoodID).UOMID).ID,
                                                  QuantityReq = workOrderDetail.RemainQuantity,
                                                  DateIn = DateTime.Now,
                                                  Price = db.Goods.Find(workOrderDetail.GoodID).InputPrice,
                                                  TotalMoney = workOrderDetail.RemainQuantity *
                                                               db.Goods.Find(workOrderDetail.GoodID).InputPrice
                                              };
                db.StockInCardsDetails.Add(oStockInCardsDetail);
                db.SaveChanges();

                workOrder.ImplementQuantity = workOrder.OrderQuantity;
                workOrder.RemainQuantity = 0;
                db.Entry(workOrder).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion
        [WebMethod]
        public string IsSave(ProductionMonitor obj)
        {
            //var db = new ErpEntities();
            //var worder = db.WorkOrders.Find(obj.WorkOrderID);
            //float sum = 0;
            //var worders = db.WorkOrderDetails.Where(w => w.WorkOrderID == worder.ID);
            //foreach (var objW in worders)
            //{
            //    var good = db.Goods.Find(objW.GoodID);
            //    var imp = objW.ImplementQuantity;
            //    var price = good.InputPrice * imp;
            //    sum += float.Parse(price.ToString());
            //}
            //var giathanh = sum / worder.ImplementQuantity;
            //var priceFinal = worder.ImplementQuantity * giathanh;

            string result = "success";
            //try
            //{
            //    var oStockInCard = new StockInCard
            //    {
            //        Code = GetCodeStockIn(),
            //        CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
            //        ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID,
            //        StockID = Convert.ToInt32(worder.StockID),
            //        CreatedDate = DateTime.Now,
            //        SourceID = worder.ID,
            //        Type = 1,
            //        Sender = AppContext.RequestVariables.CurrentUser.UserName,
            //        Status = 0,
            //        TotalMoney = priceFinal,
            //        Tax = 0

            //    };
            //    db.StockInCards.Add(oStockInCard);
            //    db.SaveChanges();
            //    var stockInCardsDetail = new StockInCardsDetail
            //    {
            //        StockInCardID = oStockInCard.ID,
            //        GoodID = Convert.ToInt32(worder.GoodID),
            //        UOMID = db.UOMs.Find(db.Goods.Find(worder.GoodID).UOMID).ID,
            //        QuantityReq = worder.ImplementQuantity,//Số sản phẩm đã làm được
            //        DateIn = DateTime.Now,
            //        Price = giathanh,
            //        TotalMoney = priceFinal//Số sản phẩm làm được * giá thành làm ra sản phẩm
            //    };
            //    db.StockInCardsDetails.Add(stockInCardsDetail);
            //    db.SaveChanges();
            //}
            //catch (Exception ex)
            //{
            //    result = ex.Message;
            //}
            return result;
        }

        [WebMethod(EnableSession = true)]
        public string
            SaveStockOutWhenCreateProduction(int workOrderId)
        {
            string result = "success";

            var db = new ErpEntities();
            var user = AppContext.RequestVariables.CurrentUser;

            var workOrder = db.WorkOrders.Find(workOrderId);
            try
            {
                var oStockOutCard = new StockOutCard
                                        {
                                            Code = GetCodeStockOut(),
                                            CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
                                            ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID,
                                            StockID = Convert.ToInt32(workOrder.StockID),
                                            CreatedDate = DateTime.Now,
                                            TargetID = workOrder.ID,
                                            Type = 4,
                                            Status = 0,
                                            CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID
                                        };
                db.StockOutCards.Add(oStockOutCard);
                db.SaveChanges();
                var workOrderDetails = db.WorkOrderDetails.Where(w => w.WorkOrderID == workOrderId);
                foreach (var w in workOrderDetails)
                {
                    var stockOutCardsDetail = new StockOutCardsDetail
                                                  {
                                                      StockOutCardID = oStockOutCard.ID,
                                                      GoodID = w.GoodID,
                                                      UOMID = db.UOMs.Find(db.Goods.Find(w.GoodID).UOMID).ID,
                                                      Quantity = Convert.ToInt32(w.RemainQuantity),
                                                      DateOut = DateTime.Now,
                                                  };

                    db.StockOutCardsDetails.Add(stockOutCardsDetail);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

       
        [WebMethod(EnableSession = true)]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateInvoiceStockOut(string[] saleInvoiceId, int userId)
        {
            string result = string.Empty;
            try
            {                
                var manager = new GenericManager<StockOutCard>();
                var managerSale = new GenericManager<SaleInvoice>();
                var managerStock = new GenericManager<Stock>();
                foreach (var x in saleInvoiceId)
                {
                    int oSaleID = Convert.ToInt32(x);
                    var oSaleInvoice = managerSale.FindById(oSaleID);
                    var oStockOutCard = new StockOutCard();
                    oStockOutCard.Code = GetCodeStockOut();
                    oStockOutCard.CreatedUserID = userId;
                    oStockOutCard.ApprovalUserID = userId;
                    var oStock = managerStock.Get().FirstOrDefault();
                    if (oStock != null)
                        oStockOutCard.StockID = oStock.ID;//  oSaleInvoice.StockID;
                    oStockOutCard.CreatedDate = DateTime.Now;
                    oStockOutCard.TargetID = oSaleInvoice.ID;
                    oStockOutCard.Type = 0;
                    oStockOutCard.Receiver = oSaleInvoice.Description ?? "None";
                    oStockOutCard.Status = 0;
                    oStockOutCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                    manager.Add(oStockOutCard);
                    if (saleInvoiceId.Count() > 1)
                    {
                        result += oStockOutCard.Code + "&";
                    }
                    else
                    {
                        result = oStockOutCard.Code;
                    }

                    var ooSaleInvoiceDetails = oSaleInvoice.SaleInvoiceDetails.ToList();
                    foreach (var item in ooSaleInvoiceDetails)
                    {
                        var oStockOutCardsDetail = new StockOutCardsDetail();
                        oStockOutCardsDetail.StockOutCardID = oStockOutCard.ID;
                        oStockOutCardsDetail.GoodID = item.GoodID;
                        oStockOutCardsDetail.UOMID = item.Good.UOMID;
                        oStockOutCardsDetail.Quantity = Convert.ToInt32(item.Quantity);
                        oStockOutCardsDetail.DateOut = DateTime.Now;
                        oStockOutCardsDetail.Price = item.Good.OutputPrice;
                        oStockOutCardsDetail.TotalMoney = item.Quantity * item.Good.OutputPrice;
                        oStockOutCard.StockOutCardsDetails.Add(oStockOutCardsDetail);
                    }
                    manager.Save();
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
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        #region Create out going shipment from many stock
        public string CreateInvoiceStockOutSale(string[] SaleInvoiceID, int userId)
        {
            string result = "";
            int realAmountInStock = 0;
            try
            {
                //StockInCard NOT NULL : Code,CreatedUserID,StockID,CreatedDate,Sender,Status
                ErpEntities db = new ErpEntities();
                foreach (var item in SaleInvoiceID)
                {

                    SaleInvoice sale = db.SaleInvoices.Find(Convert.ToInt32(item));
                    //List<WorkOrderDetail> lstWODetails = workOrder.WorkOrderDetails.ToList();
                    if (sale != null)
                    {
                        result = StockOutSale(sale.SaleInvoiceDetails.ToList(), Int32.Parse(item), userId);
                    }
                }
                db.SaveChanges();
                if (result == null)
                {
                    //temp
                    result = Resources.Resource.Common_Error_CreateOutShipment;
                }
                else if (result.Equals("1"))
                {
                    int shipmentLength = shipmentNo.Trim().Length;
                    if (shipmentLength > 0)
                        shipmentNo = shipmentNo.Trim().Substring(0, shipmentLength - 1);
                    //result = dictionary.Count + " Shipment No( " + shipmentNo + ") have been created in Outcomming Screen with amount is " + realAmount + "/" + totalAmount + " items";
                    realAmountInStock = GetRealAmountInStock(dicGood);
                    result = " Shipment No( " + shipmentNo + ") have been created in Outcomming Screen with amount is " + realAmount + "/" + realAmountInStock + " items";
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
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        private string StockOutSale(List<SaleInvoiceDetail> lstSODetails, int SaleInvID, int userID)
        {
            string result = "";
            ErpEntities db = new ErpEntities();
            List<int> stockIDls = new List<int>();
            foreach (var item in lstSODetails)
            {

                int? GoodID = item.GoodID;
                int? Quantity = item.Quantity;
                if (dicGood.ContainsKey(GoodID ?? 0))
                {
                    bool isOut = dicGood[GoodID ?? 0];
                    if (isOut)
                    {
                        break;
                    }
                }
                var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.GoodID == GoodID).OrderBy(ac => ac.DateIn).ToList();
                if (goodOrderByDateIn != null && goodOrderByDateIn.Count > 0)
                    result = StockOutByGoodID(ref stockIDls, goodOrderByDateIn, Quantity, SaleInvID, userID);
            }
            return result;
        }

        private string StockOutByGoodID(ref List<int> stockIDls, List<ActualGoodInStock> goodOrderByDateIn, int? quantityForExport, int SaleInvID, int userId)
        {
            object result = null;
           
            int StockID = 0;
            int GoodID = 0;

            int? totalAmountOut = goodOrderByDateIn.Sum(good => good.RemainQuantity);
            int? realAmountOut = 0;
            bool isOut = false;
            try
            {
                foreach (var item in goodOrderByDateIn)
                {
                    StockID = item.StockID ?? 0;
                    GoodID = item.GoodID ?? 0;
                    int quantityRemain = item.RemainQuantity ?? 0;
                    if (StockID != 0 && !stockIDls.Contains(StockID))
                    {
                        stockIDls.Add(StockID);
                        if (quantityForExport >= quantityRemain)
                        {
                            SaveInStockOut(SaleInvID, userId, item.ID, quantityRemain, StockID);
                            realAmountOut += quantityRemain;
                            quantityForExport = (quantityForExport ?? 0) - quantityRemain;

                        }
                        else
                        {
                            realAmountOut += quantityForExport;
                            SaveInStockOut(SaleInvID, userId, item.ID, quantityForExport, StockID);
                            break;
                        }
                    }
                    else if (stockIDls.Contains(StockID))
                    {
                        if (dictionary.ContainsKey(StockID))
                        {
                            int StockOutCardID = dictionary[StockID];
                            if (quantityForExport >= quantityRemain)
                            {
                                AddDetailsForStockOutCard(StockOutCardID, item.ID, quantityRemain);
                                realAmountOut += quantityRemain;
                                quantityForExport = (quantityForExport ?? 0) - quantityRemain;

                            }
                            else
                            {
                                realAmountOut += quantityForExport;
                                AddDetailsForStockOutCard(StockOutCardID, item.ID, quantityForExport);
                                break;
                            }
                        }
                    }
                }
                realAmount += realAmountOut;
                totalAmount += totalAmountOut;
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            if (realAmountOut == totalAmountOut)
            {
                isOut = true;
            }

            if (GoodID > 0 && !dicGood.ContainsKey(GoodID))
            {
                dicGood.Add(GoodID, isOut);
                result = 1;
            }
            return _serializer.Serialize(result);
        }

        private void AddDetailsForStockOutCard(int StockOutCardID, int? actualGoodInStockID, int? quantityForExport)
        {
            ErpEntities db = new ErpEntities();
           // Good oGood = db.Goods.Find(GoodID);
            ActualGoodInStock oActualGood = db.ActualGoodInStocks.Find(actualGoodInStockID);
            StockOutCardsDetail stockOutCardsDetail = new StockOutCardsDetail();
            stockOutCardsDetail.StockOutCardID = StockOutCardID;
            stockOutCardsDetail.GoodID = oActualGood.GoodID ?? 0;
            stockOutCardsDetail.UOMID = oActualGood.Good.UOMID;
            stockOutCardsDetail.Quantity = quantityForExport ?? 0;
            stockOutCardsDetail.DateOut = DateTime.Now;
            //stockOutCardsDetail.Discount = oGood.Dis
            stockOutCardsDetail.Price = oActualGood.InputPrice;
            stockOutCardsDetail.TotalMoney = quantityForExport * oActualGood.InputPrice;
            db.StockOutCardsDetails.Add(stockOutCardsDetail);
            db.SaveChanges();

        }


        [WebMethod(EnableSession = true)]
        private void SaveInStockOut(int SaleInvID, int userId, int? actualGoodInStockID, int? Quantity, int StockID)
        {
            ErpEntities db = new ErpEntities();
            SaleInvoice saleInvoice = db.SaleInvoices.Find(Convert.ToInt32(SaleInvID));
           // Good oGood = db.Goods.Find(GoodID);
            ActualGoodInStock oActualGood = db.ActualGoodInStocks.Find(actualGoodInStockID);
            #region StockOutCard
            StockOutCard oStockOutCard = new StockOutCard();
            //Add new StockInCard (Purchase)
            oStockOutCard.Code = GetCodeStockOut();
            oStockOutCard.CreatedUserID = userId;
            oStockOutCard.ApprovalUserID = userId;
            oStockOutCard.StockID = StockID;
            oStockOutCard.CreatedDate = saleInvoice.CreatedDate != null ? saleInvoice.CreatedDate : DateTime.Now;
            oStockOutCard.TargetID = saleInvoice.ID;
            //Type:0-Purchase;1-GoodProcess;2-Movement
            oStockOutCard.Type = 0;
            oStockOutCard.Receiver = saleInvoice.Description ?? "No Content";
            //oStockOutCard.Tax = saleInvoice.TaxAmountType;
            //oStockOutCard.TotalMoney = saleInvoice.TotalMoney;
            //Status:0 - Draft ; Status:1 - Done
            oStockOutCard.Status = 0;
            oStockOutCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
            db.StockOutCards.Add(oStockOutCard);
            db.SaveChanges();
            shipmentNo += oStockOutCard.Code + ",";
            #endregion StockOutCard
            #region Add StockIncardDetail


            StockOutCardsDetail stockOutCardsDetail = new StockOutCardsDetail();
            stockOutCardsDetail.StockOutCardID = oStockOutCard.ID;
            stockOutCardsDetail.GoodID = oActualGood.GoodID ?? 0;
            stockOutCardsDetail.UOMID = oActualGood.Good.UOMID;
            stockOutCardsDetail.Quantity = Convert.ToInt32(Quantity);
            stockOutCardsDetail.DateOut = DateTime.Now;
            stockOutCardsDetail.Price = oActualGood.InputPrice;
            stockOutCardsDetail.TotalMoney = Quantity * oActualGood.InputPrice;
            db.StockOutCardsDetails.Add(stockOutCardsDetail);
            db.SaveChanges();

            dictionary.Add(StockID, oStockOutCard.ID);
            // listKeyVal.Add(new KeyValuePair<int, int>(StockID, oStockOutCard.ID));                              
            #endregion
        }
        #endregion
        public int GetRealAmountInStock(Dictionary<int, bool> dicGood)
        {
            ErpEntities db = new ErpEntities();
            int? amount = 0;
            foreach (var item in dicGood.Keys)
            {
                amount += db.ActualGoodInStocks.Where(ac => ac.GoodID == (int)item).Sum(am => am.RemainQuantity);
            }
            return amount ?? 0;
        }
    }
}


