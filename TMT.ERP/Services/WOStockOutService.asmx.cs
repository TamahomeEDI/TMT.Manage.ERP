using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for WOStockOutService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class WOStockOutService : System.Web.Services.WebService
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

        #region Stock out from many stock for workorder
        [WebMethod]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CreateInvoiceStockOutWO(string[] workOrderId, int userId)
        {
            string result = "";
            int realAmountInStock = 0;
            try
            {
                //StockInCard NOT NULL : Code,CreatedUserID,StockID,CreatedDate,Sender,Status
                ErpEntities db = new ErpEntities();
                foreach (var item in workOrderId)
                {

                    WorkOrder workOrder = db.WorkOrders.Find(Convert.ToInt32(item));
                    //List<WorkOrderDetail> lstWODetails = workOrder.WorkOrderDetails.ToList();
                    if (workOrder != null)
                    {
                        result = StockOutWO(workOrder.WorkOrderDetails.ToList(), Int32.Parse(item), userId);
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
                    result = " Shipment No( " + shipmentNo + ") have been created in Outgoing Shipment Screen with amount is " + realAmount + "/" + realAmountInStock + " items";
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

        private string StockOutWO(List<WorkOrderDetail> lstWODetails, int workOrderId, int userID)
        {
            List<int> stockIDls = new List<int>();
            string result = "";
            ErpEntities db = new ErpEntities();
            foreach (var item in lstWODetails)
            {
                int? GoodID = item.GoodID;
                int? Quantity = item.OrderQuantity;
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
                    result = StockOutByGoodID(ref stockIDls, goodOrderByDateIn, Quantity, workOrderId, userID);
            }
            return result;
        }

        private string StockOutByGoodID(ref List<int> stockIDls, List<ActualGoodInStock> goodOrderByDateIn, int? quantityForExport, int workOrderId, int userId)
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
                            SaveInStockOut(workOrderId, userId, item.ID, quantityRemain, StockID);
                            realAmountOut += quantityRemain;
                            quantityForExport = (quantityForExport ?? 0) - quantityRemain;

                        }
                        else
                        {
                            realAmountOut += quantityForExport;
                            SaveInStockOut(workOrderId, userId, item.ID, quantityForExport, StockID);
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
            //Good oGood = db.Goods.Find(GoodID);
            ActualGoodInStock oActualGood = db.ActualGoodInStocks.Find(actualGoodInStockID);
            StockOutCardsDetail stockOutCardsDetail = new StockOutCardsDetail();
            stockOutCardsDetail.StockOutCardID = StockOutCardID;
            stockOutCardsDetail.GoodID = oActualGood.GoodID ?? 0;
            stockOutCardsDetail.UOMID = oActualGood.Good.UOMID;
            stockOutCardsDetail.Quantity = quantityForExport ?? 0;
            stockOutCardsDetail.RemainQuantity = quantityForExport ?? 0;
            stockOutCardsDetail.DateOut = DateTime.Now;
            //stockOutCardsDetail.Discount = oGood.Dis
            stockOutCardsDetail.Price = oActualGood.InputPrice;
            stockOutCardsDetail.TotalMoney = quantityForExport * oActualGood.InputPrice;
            db.StockOutCardsDetails.Add(stockOutCardsDetail);
            db.SaveChanges();

        }



        private void SaveInStockOut(int workOrderId, int userId, int? actualGoodInStockID, int? Quantity, int StockID)
        {
            ErpEntities db = new ErpEntities();
            WorkOrder workOrder = db.WorkOrders.Find(Convert.ToInt32(workOrderId));
           // Good oGood = db.Goods.Find(GoodID);
            ActualGoodInStock oActualGood = db.ActualGoodInStocks.Find(actualGoodInStockID);
            #region StockOutCard
            StockOutCard oStockOutCard = new StockOutCard();
            //Add new StockInCard (Purchase)
            oStockOutCard.Code = GetCodeStockOut();
            oStockOutCard.CreatedUserID = userId;
            oStockOutCard.ApprovalUserID = userId;
            oStockOutCard.StockID = StockID;
            oStockOutCard.CreatedDate = workOrder.CreatedDate != null ? workOrder.CreatedDate : DateTime.Now;
            oStockOutCard.TargetID = workOrder.ID;
            //Type:0-Purchase;1-GoodProcess;2-Movement
            oStockOutCard.Type = 1;
            oStockOutCard.Receiver = workOrder.Description ?? "No Content";
            //oStockOutCard.Tax = saleInvoice.TaxAmountType;
            //oStockOutCard.TotalMoney = saleInvoice.TotalMoney;
            //Status:0 - Draft ; Status:1 - Done
            oStockOutCard.Status = 0;
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
            stockOutCardsDetail.RemainQuantity = Convert.ToInt32(Quantity);
            stockOutCardsDetail.DateOut = DateTime.Now;
            stockOutCardsDetail.Price = oActualGood.InputPrice;
            stockOutCardsDetail.TotalMoney = Quantity * oActualGood.InputPrice;
            db.StockOutCardsDetails.Add(stockOutCardsDetail);
            db.SaveChanges();

            dictionary.Add(StockID, oStockOutCard.ID);
            // listKeyVal.Add(new KeyValuePair<int, int>(StockID, oStockOutCard.ID));                              
            #endregion
        }

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

        public int GetRealAmountInStock(Dictionary<int, bool> dicGood)
        {
            ErpEntities db = new ErpEntities();
            int? amount = 0;
           foreach(var item in dicGood.Keys)
            {
                amount += db.ActualGoodInStocks.Where(ac => ac.GoodID == (int)item).Sum(am => am.RemainQuantity);
            }
           return amount ?? 0;
        }
        #endregion

    }
}
