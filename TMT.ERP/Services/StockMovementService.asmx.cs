using CommonLib;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
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
using System.Data;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for StockMovementService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class StockMovementService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        ErpEntities db = new ErpEntities();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveStockMovement(int? stockMovementID, string code, string createDate,
                                        int stockID, int createdEmployeeID, int stockIDDes, float? totalMoney, int status, int type, string stockMovementDetail)
        {
            object result = 0;
            try
            {

                StockMovement oStockMov = null;
                if (stockMovementID.GetValueOrDefault() > 0)
                {
                    oStockMov = db.StockMovements.Find(stockMovementID);
                    var oStockMovDetails = db.StockMovementDetails.Where(details => details.StockMovementID == stockMovementID);
                    foreach (var item in oStockMovDetails)
                    {
                        db.Entry(item).State = System.Data.EntityState.Deleted;
                    }
                }
                else
                {
                    oStockMov = new StockMovement();
                    db.StockMovements.Add(oStockMov);
                }

                oStockMov.Code = code;
                oStockMov.CreatedDate = DateTime.Now;
                if (createDate != null && createDate != "")
                {
                    oStockMov.CreatedDate = DateTime.Parse(createDate);
                }

                oStockMov.FromStockID = stockID;
                oStockMov.ToStockID = stockIDDes;
                oStockMov.CreatedUserID = createdEmployeeID;
                oStockMov.Status = status;
                //oStockMov.FromAccountID = accountIDFrom;
                //oStockMov.ToAccountID = accountIDTo;
                db.SaveChanges();
                //Utility.UpdateNextNumber(CodeType.TranferStock.ToString(), Constant.CODE_LENGTH, "");
                Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.TranferStock), Constant.CODE_LENGTH, "");
                dynamic array = JsonConvert.DeserializeObject(stockMovementDetail);
                foreach (var item in array)
                {
                    StockMovementDetail oStockMove_Details = new StockMovementDetail();
                    oStockMove_Details.StockMovementID = oStockMov.ID;
                    oStockMove_Details.GoodID = Int32.Parse(((string)item.goodId).Replace("{", "").Replace("}", ""));
                    oStockMove_Details.DateIn = DateTime.Now;
                    oStockMove_Details.UOMID = item.umo;
                    oStockMove_Details.Description = item.description;
                    oStockMove_Details.Quantity = Int32.Parse(((string)item.qty).Replace("{", "").Replace("}", ""));
                    db.StockMovementDetails.Add(oStockMove_Details);
                }

                db.SaveChanges();
                result = oStockMov.ID;
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


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string outStock(StockOutCard objStockOut, string stockMovementDetail)
        {
            object result = "success";
            try
            {
                int type = objStockOut.Type;
                int targetID = objStockOut.TargetID ?? 0;
                switch (type)
                {
                    case 0:
                        //Sale Invoice
                        SaleInvoice objSale = db.SaleInvoices.Find(targetID);
                        StockOutFromSale(objSale, stockMovementDetail);
                        break;
                    case 1:
                        WorkOrder oWorkOrder = db.WorkOrders.Find(targetID);
                        StockOutFromWorkOrder(oWorkOrder, stockMovementDetail);
                        break;
                    case 2:
                        //Stock Movement
                        StockMovement objStockMov = db.StockMovements.Find(targetID);
                        StockOutFromStockMovement(objStockMov, stockMovementDetail);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            result = 1;
            return serializer.Serialize(result);

        }

        private void StockOutFromWorkOrder(WorkOrder oWorkOrder, string stockMovementDetail)
        {
            dynamic array = JsonConvert.DeserializeObject(stockMovementDetail);
            foreach (var item in array)
            {
                int? goodId = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                int? quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));

                var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.GoodID == goodId).OrderBy(ac => ac.DateIn).ToList();
                UpdateQtyAfterExpForSale(goodOrderByDateIn, quantity);
            }
        }


        private void StockOutFromSale(SaleInvoice objSale, string stockMovementDetail)
        {
            dynamic MovementDetailsArr = JsonConvert.DeserializeObject(stockMovementDetail);
            foreach (var item in MovementDetailsArr)
            {
                int? GoodID = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                int? Quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));
                // int? price = SafeConvert.ToNullable<int>(((string)item.price).Replace("{", "").Replace("}", ""));
                //int? uomid = SafeConvert.ToNullable<int>(((string)item.uomid).Replace("{", "").Replace("}", ""));
                //var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.StockID == objSale.StockID && ac.GoodID == GoodID).OrderBy(ac => ac.DateIn).ToList();

                //update 24-12-2013 : Không cần where theo kho khi xuất hàng . Vì có thể xuất trên nhiều kho.
                var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.GoodID == GoodID).OrderBy(ac => ac.DateIn).ToList();
                UpdateQtyAfterExpForSale(goodOrderByDateIn, Quantity);
            }

        }

        private void UpdateQtyAfterExpForSale(List<ActualGoodInStock> goodOrderByDateIn, int? quantityForExport)
        {

            foreach (var item in goodOrderByDateIn)
            {
                int quantityRemain = item.RemainQuantity ?? 0;
                ActualGoodInStock obj = db.ActualGoodInStocks.Find(item.ID);
                if (quantityRemain == 0)
                {
                    deleteFromStock(obj);
                }
                else if (quantityForExport >= quantityRemain)
                {
                    obj.OutputQuantity = (obj.OutputQuantity ?? 0) + (obj.RemainQuantity ?? 0);
                    obj.RemainQuantity = 0;
                    obj.UpdatedDate = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    quantityForExport = (quantityForExport ?? 0) - quantityRemain;

                }
                else
                {
                    obj.OutputQuantity = (obj.OutputQuantity ?? 0) + quantityForExport;
                    obj.RemainQuantity = quantityRemain - quantityForExport;
                    obj.UpdatedDate = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    break;
                }
            }
        }

        private void StockOutFromStockMovement(StockMovement objStockMov, string stockMovementDetail)
        {
            dynamic MovementDetailsArr = JsonConvert.DeserializeObject(stockMovementDetail);
            foreach (var item in MovementDetailsArr)
            {
                int? GoodID = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                int? Quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));
                var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.StockID == objStockMov.FromStockID && ac.GoodID == GoodID).OrderBy(ac => ac.DateIn).ToList();
                UpdateQtyAfterExpForStockMovement(objStockMov.ID, goodOrderByDateIn, Quantity);
            }
        }

        /*  private void BalanceAccountTrans(int status, int stockIDFrom, int accountIDFrom, string stockMovementDetail)
          {
              dynamic MovementDetailsArr = JsonConvert.DeserializeObject(stockMovementDetail);
              ErpEntities db = new ErpEntities();
              foreach (var item in MovementDetailsArr)
              {
                  int? GoodID = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                  int? Quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));
                  var goodOrderByDateIn = db.ActualGoodInStocks.Where(ac => ac.StockID == stockIDFrom && ac.GoodID == GoodID).OrderByDescending(ac => ac.DateIn).FirstOrDefault();
                  AccountTran acTrans = new AccountTran();
                  acTrans.CreatedDate = DateTime.Now;
                  acTrans.StockID = stockIDFrom;
                  acTrans.AccountID = accountIDFrom;
                  acTrans.GoodID = GoodID ?? 0;
                  acTrans.Quantity = Quantity;
                  acTrans.Price = goodOrderByDateIn.InputPrice;
                  if (status == 0)
                  {
                      acTrans.Credit += acTrans.Price;
                  }
                  else
                  {
                      acTrans.Debit += acTrans.Price;
                  }
              }
          } */

        private void UpdateQtyAfterExpForStockMovement(int StockMoveID, List<ActualGoodInStock> goodOrderByDateIn, int? quantityForExport)
        {

            /* ActualGoodInStock oActual = goodOrderByDateIn.FirstOrDefault();

             int quantityRemain = oActual.RemainQuantity ?? 0;
             if (quantityForExport <= quantityRemain)
             {
                 ActualGoodInStock obj = db.ActualGoodInStocks.Find(oActual.ID);
                 obj.RemainQuantity = quantityRemain - (quantityForExport ?? 0);
                 obj.UpdatedDate = DateTime.Now;

                 obj.OutputQuantity = (obj.OutputQuantity ?? 0)  + (quantityForExport ?? 0);
                 db.Entry(obj).State = EntityState.Modified;
                 db.SaveChanges();
                 //save in Temp
                 saveInTemp(StockMoveID, quantityForExport, obj);
             }
             else
             { */
            CountShortageGoodsForDateIn(StockMoveID, goodOrderByDateIn, quantityForExport);
            //}
        }

        private void CountShortageGoodsForDateIn(int StockMoveID, List<ActualGoodInStock> goodOrderByDateIn, int? quantityForExport)
        {

            foreach (var item in goodOrderByDateIn)
            {
                int quantityRemain = item.RemainQuantity ?? 0;
                ActualGoodInStock obj = db.ActualGoodInStocks.Find(item.ID);
                if (quantityRemain == 0)
                {
                    deleteFromStock(obj);
                }
                else if (quantityForExport >= quantityRemain)
                {
                    obj.OutputQuantity = (obj.OutputQuantity ?? 0) + (obj.RemainQuantity ?? 0);
                    obj.RemainQuantity = 0;
                    obj.UpdatedDate = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    quantityForExport = (quantityForExport ?? 0) - quantityRemain;
                    //save in Temp
                    saveInTemp(StockMoveID, quantityForExport, obj);
                }
                else
                {
                    obj.OutputQuantity = (obj.OutputQuantity ?? 0) + quantityForExport;
                    obj.RemainQuantity = quantityRemain - quantityForExport;
                    obj.UpdatedDate = DateTime.Now;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    //save in Temp
                    saveInTemp(StockMoveID, quantityForExport, obj);
                    break;
                }
            }
        }

        private void deleteFromStock(ActualGoodInStock obj)
        {
            db.Entry(obj).State = EntityState.Deleted;
            db.SaveChanges();
        }

        public void saveInTemp(int StockMovementID, int? quantityForExport, ActualGoodInStock obj)
        {
            StockMovement objSM = db.StockMovements.Find(StockMovementID);
            StockMovementTemp objTemp = db.StockMovementTemps.Where(temp => temp.StockMovementID == StockMovementID && temp.DateIn == obj.DateIn && temp.GoodID == obj.GoodID).FirstOrDefault();
            if (objTemp != null)
            {
                objTemp.Quantity = (objTemp.Quantity ?? 0) + quantityForExport;
                db.Entry(objTemp).State = EntityState.Modified;
            }
            else
            {
                StockMovementTemp temp = new StockMovementTemp();
                temp.StockMovementID = StockMovementID;
                temp.GoodID = obj.GoodID;
                temp.DateIn = obj.DateIn;
                temp.Quantity = quantityForExport;
                temp.InputPrice = obj.InputPrice;
                temp.UOMID = obj.UOMID;
                db.StockMovementTemps.Add(temp);
            }
            db.SaveChanges();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string InStock(StockInCard objStockIn, string stockMovementDetail, int? stockId)
        {
            object result = "success";
            try
            {
                int type = objStockIn.Type ?? 0;
                int targetID = objStockIn.SourceID ?? 0;
                switch (type)
                {
                    case 0:
                        //Purchase
                        Purchase objPur = db.Purchases.Find(targetID);
                        //StockInFromPurchase(Convert.ToInt32(objPur.StockID), stockMovementDetail);
                        //stockFinalID : Mục đích lấy ra Stock ở phiếu nhập kho đã chọn thay vì stock theo phiếu Purchase (hoặc tránh trường hợp không chọn kho ở phiếu Purchase)
                        var stockFinalID = 0;
                        if (stockId == 0)
                        {
                            stockFinalID = Convert.ToInt32(objPur.StockID);
                        }
                        else
                        {
                            stockFinalID = Convert.ToInt32(stockId);
                        }
                        StockInFromPurchase(stockFinalID, stockMovementDetail);
                        break;
                    case 3:
                        //Stock Movement
                        StockMovement objStockMov = db.StockMovements.Find(targetID);
                        StockInFromStockMovement(objStockMov.ID, objStockMov.ToStockID, stockMovementDetail);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            result = 1;
            return serializer.Serialize(result);
        }

        private void StockInFromStockMovement(int StockMovementID, int stockIDDes, string stockMovementDetail)
        {
            dynamic MovementDetailsArr = JsonConvert.DeserializeObject(stockMovementDetail);
            foreach (var item in MovementDetailsArr)
            {
                int? GoodID = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                int? Quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));
                var goodsExportedToTemp = db.StockMovementTemps.Where(st => st.StockMovementID == StockMovementID && st.GoodID == GoodID).OrderBy(ac => ac.DateIn).ToList();

                foreach (var good in goodsExportedToTemp)
                {
                    var qtyRemainByDateAsc = good.Quantity;
                    if (qtyRemainByDateAsc == 0)
                    {
                        deleteFromTemp(good);
                    }
                    else if (Quantity > 0 && Quantity >= qtyRemainByDateAsc)
                    {
                        DoImportStock(stockIDDes, qtyRemainByDateAsc, good);
                        Quantity = Quantity - (qtyRemainByDateAsc ?? 0);

                        //Do Delete in Temp
                        db.Entry(good).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                    else
                    {
                        DoImportStock(stockIDDes, Quantity, good);
                    }
                }
            }
        }

        private void deleteFromTemp(StockMovementTemp good)
        {
            db.Entry(good).State = EntityState.Deleted;
            db.SaveChanges();
        }


        private void DoImportStock(int StockIDDes, int? qtyImport, StockMovementTemp good)
        {
            var stockDesGoodByDateIn = db.ActualGoodInStocks.Where(ac => ac.StockID == StockIDDes && ac.GoodID == good.GoodID && ac.DateIn == good.DateIn).FirstOrDefault();
            if (stockDesGoodByDateIn != null)
            {
                stockDesGoodByDateIn.UpdatedDate = DateTime.Now;
                stockDesGoodByDateIn.InputQuantity = (stockDesGoodByDateIn.InputQuantity ?? 0) + (qtyImport ?? 0);
                stockDesGoodByDateIn.RemainQuantity = (stockDesGoodByDateIn.RemainQuantity ?? 0) + (qtyImport ?? 0);
                db.Entry(stockDesGoodByDateIn).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                ActualGoodInStock oActual = new ActualGoodInStock();
                oActual.StockID = StockIDDes;
                oActual.GoodID = good.GoodID;
                oActual.DateIn = good.DateIn;
                oActual.UpdatedDate = DateTime.Now;
                oActual.UOMID = good.UOMID;
                oActual.InputQuantity = (oActual.InputQuantity ?? 0) + (qtyImport ?? 0);
                oActual.RemainQuantity = (oActual.RemainQuantity ?? 0) + (qtyImport ?? 0);
                oActual.InputPrice = good.InputPrice;
                db.ActualGoodInStocks.Add(oActual);
                db.SaveChanges();
            }
        }

        private void StockInFromPurchase(int stockIDFrom, string stockMovementDetail)
        {
            dynamic MovementDetailsArr = JsonConvert.DeserializeObject(stockMovementDetail);

            foreach (var item in MovementDetailsArr)
            {
                int? GoodID = SafeConvert.ToNullable<int>(((string)item.goodId).Replace("{", "").Replace("}", ""));
                int? Quantity = SafeConvert.ToNullable<int>(((string)item.qty).Replace("{", "").Replace("}", ""));
                int? price = SafeConvert.ToNullable<int>(((string)item.price).Replace("{", "").Replace("}", ""));
                int? uomid = SafeConvert.ToNullable<int>(((string)item.uomid).Replace("{", "").Replace("}", ""));

                //var stockInGoodFromPurchase = db.ActualGoodInStocks.Where(ac => ac.StockID == stockIDFrom && ac.GoodID == GoodID && ac.DateIn == DateTime.Today).FirstOrDefault();
                //if (stockInGoodFromPurchase != null)
                //{
                //    stockInGoodFromPurchase.UpdatedDate = DateTime.Now;
                //    stockInGoodFromPurchase.InputQuantity = (stockInGoodFromPurchase.InputQuantity ?? 0) + (Quantity ?? 0);
                //    stockInGoodFromPurchase.RemainQuantity = (stockInGoodFromPurchase.RemainQuantity ?? 0) + (Quantity ?? 0);
                //    db.Entry(stockInGoodFromPurchase).State = EntityState.Modified;
                //}
                //else
                //{
                ActualGoodInStock oActual = new ActualGoodInStock();
                oActual.StockID = stockIDFrom;
                oActual.GoodID = GoodID;
                oActual.DateIn = DateTime.Now;
                oActual.UOMID = uomid;
                oActual.InputQuantity = (oActual.InputQuantity ?? 0) + (Quantity ?? 0);
                oActual.RemainQuantity = (oActual.RemainQuantity ?? 0) + (Quantity ?? 0);
                oActual.InputPrice = price;
                db.ActualGoodInStocks.Add(oActual);
                //}
                db.SaveChanges();
            }
        }
    }
}




