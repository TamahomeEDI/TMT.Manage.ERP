using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using Newtonsoft.Json;
using TMT.ERP.Models.Lookups;
using TMT.ERP.Commons;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.SqlClient;

namespace TMT.ERP.Commons
{
    //Create by NgDang
    public static class Utility
    {
        private static ErpEntities db = new ErpEntities();
        private readonly static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string GetFullCode(int currentValue, int lengthOfCode)
        {
            var nextNumber = currentValue + 1;
            var nextStr = nextNumber.ToString();
            while (nextStr.Length < lengthOfCode)
                nextStr = "0" + nextStr;
            return nextStr;
        }

        //type: 0-Sale;1-Purchase;2-Stock in;3-Stock out;4- Planning;5-WorkOrder
        public static string GetCode(string type)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().FirstOrDefault(x => x.Type == Int32.Parse(type));
            if (item != null)
                return item.InvoicePrefix + item.NextNumber;
            else
                return "";
        }

        public static bool ExistCode(string code, string type)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().Where(x => x.Type == Int32.Parse(type) && x.InvoicePrefix == code).FirstOrDefault();
            if (item != null)
                return true;
            else
                return false;
        }

        public static void UpdateNextNumber(string type, int lengOfCode, string defaultPrefix)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().FirstOrDefault(x => x.Type == Int32.Parse(type));
            int currentValue = 0;
            if (item == null)
            {
                item = new SaleInvoiceSetting();
                if (!string.IsNullOrEmpty(defaultPrefix))
                    item.InvoicePrefix = defaultPrefix;
                manager.Add(item);
            }
            else
            {
                currentValue = Int32.Parse(item.NextNumber.ToString());
            }

            item.NextNumber = GetFullCode(currentValue, lengOfCode);
            manager.Save();
        }

        /// <summary>
        /// Check if two dates are same
        /// </summary>
        /// <typeparam name="TElement">Type</typeparam>
        /// <param name="valueSelector">date field</param>
        /// <param name="value">date compared</param>
        /// <returns>bool</returns>
        public static Expression<Func<TElement, bool>> IsSameDate<TElement>(Expression<Func<TElement, DateTime>> valueSelector, DateTime value)
        {
            ParameterExpression p = valueSelector.Parameters.Single();

            var antes = Expression.GreaterThanOrEqual(valueSelector.Body, Expression.Constant(value.Date, typeof(DateTime)));

            var despues = Expression.LessThan(valueSelector.Body, Expression.Constant(value.AddDays(1).Date, typeof(DateTime)));

            Expression body = Expression.And(antes, despues);

            return Expression.Lambda<Func<TElement, bool>>(body, p);
        }
        //inType=0-Purchase;1-WokOrder;2-Return;3-StockMovement
        public static string CreateIncomingShipment(int inType, int createdUserID, string[] sourceID)
        {
            try
            {
                var manager = new GenericManager<StockInCard>();
                string Code = "";
                switch (inType)
                {
                    #region update from Purchase
                    case 0:
                        foreach (var item in sourceID)
                        {
                            var purchase = new GenericManager<Purchase>().FindById(Int32.Parse(item));
                            if (purchase != null)
                            {
                                Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment));
                                var stockInCardItem = new StockInCard
                                {
                                    Code = Code,
                                    CreatedUserID = createdUserID,
                                    StockID = Convert.ToInt32(purchase.StockID),
                                    CreatedDate = DateTime.Now,
                                    SourceID = Int32.Parse(item),
                                    Type = inType,
                                    Sender = purchase.User.FullName,
                                    Status = 0,
                                };
                                manager.Add(stockInCardItem);

                                foreach (var itemDetail in purchase.PurchaseDetails)
                                {
                                    var stockInCardsDetail = new StockInCardsDetail
                                    {
                                        StockInCardID = stockInCardItem.ID,
                                        GoodID = itemDetail.GoodID,
                                        UOMID = itemDetail.UOMID,
                                        Quantity = itemDetail.Quantity,
                                        DateIn = DateTime.Now,
                                        Price = itemDetail.Price,
                                        Discount = itemDetail.Discount,
                                        TotalMoney = itemDetail.TotalMoney
                                    };

                                    stockInCardItem.StockInCardsDetails.Add(stockInCardsDetail);
                                }
                                UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment), Constant.CODE_LENGTH, "");
                            }
                        }
                        break;
                    #endregion

                    #region update from WorkOrder
                    case 1:
                        {
                            foreach (var item in sourceID)
                            {
                                var workOrder = new GenericManager<WorkOrder>().FindById(Int32.Parse(item));
                                if (workOrder != null)
                                {
                                    Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment));
                                    var stockInCardItem = new StockInCard
                                    {
                                        Code = Code,
                                        CreatedUserID = createdUserID,
                                        StockID = Convert.ToInt32(workOrder.StockID),
                                        CreatedDate = DateTime.Now,
                                        SourceID = Int32.Parse(item),
                                        Type = inType,
                                        Sender = workOrder.User.FullName,
                                        Status = 0,
                                    };
                                    manager.Add(stockInCardItem);

                                    foreach (var itemDetail in workOrder.WorkOrderDetails)
                                    {
                                        var stockInCardsDetail = new StockInCardsDetail
                                        {
                                            StockInCardID = stockInCardItem.ID,
                                            GoodID = itemDetail.GoodID,
                                            UOMID = itemDetail.Good.UOMID,
                                            Quantity = itemDetail.OrderQuantity,
                                            DateIn = DateTime.Now,
                                            Price = itemDetail.Good.InputPrice,
                                            Discount = 0,
                                            TotalMoney = itemDetail.OrderQuantity * itemDetail.Good.InputPrice
                                        };

                                        stockInCardItem.StockInCardsDetails.Add(stockInCardsDetail);
                                    }
                                    UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment), Constant.CODE_LENGTH, "");
                                }
                            }
                            break;
                        }
                    #endregion

                    #region update from Return
                    case 2:
                        {
                            foreach (var item in sourceID)
                            {
                                var workOrder = new GenericManager<WorkOrder>().FindById(Int32.Parse(item));
                                if (workOrder != null)
                                {
                                    Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment));
                                    var stockInCardItem = new StockInCard
                                    {
                                        Code = Code,
                                        CreatedUserID = createdUserID,
                                        StockID = Convert.ToInt32(workOrder.StockID),
                                        CreatedDate = DateTime.Now,
                                        SourceID = Int32.Parse(item),
                                        Type = inType,
                                        Sender = workOrder.User.FullName,
                                        Status = 0,
                                    };
                                    manager.Add(stockInCardItem);

                                    foreach (var itemDetail in workOrder.WorkOrderDetails)
                                    {
                                        var stockInCardsDetail = new StockInCardsDetail
                                        {
                                            StockInCardID = stockInCardItem.ID,
                                            GoodID = itemDetail.GoodID,
                                            UOMID = itemDetail.Good.UOMID,
                                            Quantity = itemDetail.RemainQuantity,
                                            DateIn = DateTime.Now,
                                            Price = itemDetail.Good.InputPrice,
                                            Discount = 0,
                                            TotalMoney = itemDetail.RemainQuantity * itemDetail.Good.InputPrice
                                        };

                                        stockInCardItem.StockInCardsDetails.Add(stockInCardsDetail);
                                    }
                                    UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment), Constant.CODE_LENGTH, "");
                                }
                            }

                            break;
                        }
                    #endregion update from Purchase

                    #region update from Movement
                    //recheck input price
                    case 3:
                        {
                            foreach (var item in sourceID)
                            {
                                var tranferStock = new GenericManager<StockMovement>().FindById(Int32.Parse(item));
                                if (tranferStock != null)
                                {
                                    Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment));
                                    var stockInCardItem = new StockInCard
                                    {
                                        Code = Code,
                                        CreatedUserID = createdUserID,
                                        StockID = tranferStock.FromStockID,
                                        CreatedDate = DateTime.Now,
                                        SourceID = Int32.Parse(item),
                                        Type = inType,
                                        Sender = tranferStock.User.FullName,
                                        Status = 0,
                                    };
                                    manager.Add(stockInCardItem);

                                    foreach (var itemDetail in tranferStock.StockMovementDetails)
                                    {
                                        var stockInCardsDetail = new StockInCardsDetail
                                        {
                                            StockInCardID = stockInCardItem.ID,
                                            GoodID = itemDetail.GoodID,
                                            UOMID = itemDetail.Good.UOMID,
                                            Quantity = 0,
                                            QuantityReq = itemDetail.Quantity,
                                            DateIn = DateTime.Now,
                                            Price = itemDetail.Good.InputPrice,
                                            Discount = 0
                                            //TotalMoney = itemDetail.RemainQuantity * itemDetail.Good.InputPrice
                                        };

                                        stockInCardItem.StockInCardsDetails.Add(stockInCardsDetail);
                                    }
                                    UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.IncomingShipment), Constant.CODE_LENGTH, "");
                                }
                            }

                            break;
                        }
                    #endregion update from Movement
                }

                manager.Save();
                return "Success";
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.ToString();
            }
        }

        //outType: 0-SaleInvoice;1-GoodProcess;2-StockMovement
        public static string CreateOutgomingShipment(int outType, int createdUserID, string[] targetID)
        {
            try
            {
                var manager = new GenericManager<StockOutCard>();
                string Code = "";
                switch (outType)
                {
                    #region update from Purchase
                    case 0:
                        foreach (var item in targetID)
                        {
                            var saleInvoice = new GenericManager<SaleInvoice>().FindById(Int32.Parse(item));
                            if (saleInvoice != null)
                            {
                                Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment));
                                var stockOutCard = new StockOutCard
                                {
                                    Code = Code,
                                    CreatedUserID = createdUserID,
                                    StockID = Convert.ToInt32(saleInvoice.StockID),
                                    CreatedDate = DateTime.Now,
                                    TargetID = Int32.Parse(item),
                                    Type = outType,
                                    Receiver = saleInvoice.User.FullName,
                                    Tax = saleInvoice.TotalTax,
                                    TotalMoney = saleInvoice.TotalMoney,
                                    Status = 0,
                                };
                                manager.Add(stockOutCard);

                                foreach (var itemDetail in saleInvoice.SaleInvoiceDetails)
                                {
                                    var stockOutCardsDetail = new StockOutCardsDetail
                                    {
                                        StockOutCardID = stockOutCard.ID,
                                        GoodID = itemDetail.GoodID,
                                        UOMID = itemDetail.UOMID,
                                        Quantity = itemDetail.Quantity,
                                        DateOut = DateTime.Now,
                                        Price = itemDetail.Price,
                                        Discount = itemDetail.Discount,
                                        TotalMoney = itemDetail.TotalMoney
                                    };

                                    stockOutCard.StockOutCardsDetails.Add(stockOutCardsDetail);
                                }
                                UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment), Constant.CODE_LENGTH, "");
                            }
                        }
                        break;
                    #endregion

                    #region update from WorkOrder
                    case 1:
                        foreach (var item in targetID)
                        {
                            var workOrder = new GenericManager<WorkOrder>().FindById(Int32.Parse(item));
                            if (workOrder != null)
                            {
                                Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment));
                                var stockOutCard = new StockOutCard
                                {
                                    Code = Code,
                                    CreatedUserID = createdUserID,
                                    StockID = Convert.ToInt32(workOrder.StockID),
                                    CreatedDate = DateTime.Now,
                                    TargetID = Int32.Parse(item),
                                    Type = outType,
                                    Receiver = workOrder.User.FullName,
                                    Status = 0,
                                };
                                manager.Add(stockOutCard);

                                foreach (var itemDetail in workOrder.WorkOrderDetails)
                                {
                                    var stockOutCardsDetail = new StockOutCardsDetail
                                    {
                                        StockOutCardID = stockOutCard.ID,
                                        GoodID = itemDetail.GoodID,
                                        UOMID = itemDetail.Good.UOMID,
                                        Quantity = itemDetail.OrderQuantity,
                                        DateOut = DateTime.Now,
                                        Price = itemDetail.Good.DomesticPrice,
                                        Discount = 0,
                                        TotalMoney = itemDetail.OrderQuantity * itemDetail.Good.DomesticPrice
                                    };

                                    stockOutCard.StockOutCardsDetails.Add(stockOutCardsDetail);
                                }
                                UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment), Constant.CODE_LENGTH, "");
                            }
                        }
                        break;
                    #endregion

                    #region Update to TranferStock
                    case 2:
                        foreach (var item in targetID)
                        {
                            var stockMovement = new GenericManager<StockMovement>().FindById(Int32.Parse(item));
                            if (stockMovement != null)
                            {
                                Code = GetCode(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment));
                                var stockOutCard = new StockOutCard
                                {
                                    Code = Code,
                                    CreatedUserID = createdUserID,
                                    StockID = stockMovement.ToStockID,
                                    CreatedDate = DateTime.Now,
                                    TargetID = Int32.Parse(item),
                                    Type = outType,
                                    Receiver = stockMovement.User.FullName,
                                    Status = 0,
                                };
                                manager.Add(stockOutCard);

                                foreach (var itemDetail in stockMovement.StockMovementDetails)
                                {
                                    var stockOutCardsDetail = new StockOutCardsDetail
                                    {
                                        StockOutCardID = stockOutCard.ID,
                                        GoodID = itemDetail.GoodID,
                                        UOMID = itemDetail.UOMID,
                                        Quantity = itemDetail.Quantity,
                                        DateOut = DateTime.Now,
                                        Price = itemDetail.Good.DomesticPrice,
                                        Discount = 0,
                                        TotalMoney = itemDetail.Quantity * itemDetail.Good.OutputPrice
                                    };
                                    stockOutCard.StockOutCardsDetails.Add(stockOutCardsDetail);
                                }
                                UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.OutgomingShipment), Constant.CODE_LENGTH, "");
                            }
                        }
                        break;
                    #endregion Update to TranferStock
                }

                manager.Save();
                return "Success";
            }
            catch (Exception ex)
            {
                return "ERROR:" + ex.ToString();
            }
        }

        #region Account Transaction

        //typeAccTrans : 1.Sale Order, 0.Purchase Order 
        //transTypeID : ID of Sale Invoice or Purchase ...
        //type 0: bank account , 1: normal account
        public static string CreateAccountTransaction(int typeAccTrans, int transTypeID, Payment payment, int? type)
        {
            string result = "";
            switch (typeAccTrans)
            {
                case 1:
                    result = CreatePOAccountTrans(typeAccTrans, transTypeID, payment, type);
                    break;
                case 0:
                    result = CreateSOAccountTrans(typeAccTrans, transTypeID, payment, type);
                    break;
                default:
                    break;
            }
            return result;
        }


        private static string CreateSOAccountTrans(int typeAccTrans, int transTypeID, Payment payment, int? accountType)
        {
            object result = null;
            try
            {
                var manager = new GenericManager<AccountFeature>();
                var managerSO = new GenericManager<SaleInvoice>();
                var item = manager.Get().FirstOrDefault(ac => ac.Type == typeAccTrans);
                int? saleInAcctID = item.AccountID;
                item = manager.Get().FirstOrDefault(ac => ac.Type == 2);
                int? saleTaxAccID = item.AccountID;
                SaleInvoice oSale = managerSO.FindById(transTypeID);
                var detailsSaleInv = oSale.SaleInvoiceDetails;
                int? type = oSale.Type;
                //type is Credit Note
                switch (type)
                {
                    case 1:
                        //Create Transaction for Account Sale Order
                        saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleInAcctID, DateTime.Today, oSale.Reference, oSale.TotalMoney, null, null, null, null);

                        //Create Transaction for Account Sale Tax
                        saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleTaxAccID, DateTime.Today, oSale.Reference, oSale.TotalTax, null, null, null, null);

                        foreach (SaleInvoiceDetail saleItem in detailsSaleInv)
                        {
                            if (oSale.UseVAT == 1)
                            {
                                double? totalMoneyWithTax = saleItem.TotalMoney - (saleItem.TotalMoney * saleItem.TaxRate.Rate / 100);
                                saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, totalMoneyWithTax, null, null, null, null);
                            }
                            else
                            {
                                saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, saleItem.TotalMoney, null, null, null, null);
                            }
                        }

                        break;
                    case 0:
                        if (payment == null)
                        {
                            // Insert SaleInvoice into table AccountTrans
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleInAcctID, DateTime.Today, oSale.Reference, oSale.TotalMoney, null, null, null, null);
                            UpdateDebitCreditAccount(saleInAcctID, oSale.TotalMoney, 0);
                            //Create Transaction for Account Sale Tax
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleTaxAccID, DateTime.Today, oSale.Reference, oSale.TotalTax, null, null, null, null);
                            UpdateDebitCreditAccount(saleInAcctID, 0, oSale.TotalTax);
                            foreach (SaleInvoiceDetail saleItem in detailsSaleInv)
                            {

                                if (oSale.UseVAT == 1)
                                {
                                    //double? totalMoneyWithTax = saleItem.TotalMoney - (saleItem.TotalMoney * saleItem.TaxRate.Rate / 100);
                                    var totalMoneyWithTax = saleItem.TotalMoney ?? ((saleItem.Quantity * saleItem.Price) - ((saleItem.Discount * saleItem.Price) / 100));
                                    saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, totalMoneyWithTax, null, null, null, null);
                                    UpdateDebitCreditAccount(saleInAcctID, 0, totalMoneyWithTax);
                                }
                                else
                                {
                                    saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, saleItem.TotalMoney, null, null, null, null);
                                    UpdateDebitCreditAccount(saleInAcctID, 0, saleItem.TotalMoney);
                                }
                            }
                        }
                        // Payment Transaction
                        else
                        {
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? 
                                Resources.Resource.Transaction_Name_Payment : Resources.Resource.Transaction_Name_Payment + " " + oSale.Contact.ContactName, payment.PayToAccount, DateTime.Today, null, payment.TotalMoney, null, null, null, accountType);
                        }
                        break;
                }


            }
            catch (Exception e)
            {
                result = 0;
            }
            result = 1;
            return serializer.Serialize(result);
        }


        private static string CreatePOAccountTrans(int typeAccTrans, int transTypeID, Payment payment, int? AccountType)
        {
            object result = null;
            try
            {
                var manager = new GenericManager<AccountFeature>();
                var managerPO = new GenericManager<Purchase>();
                var item = manager.Get().FirstOrDefault(ac => ac.Type == typeAccTrans);
                int? purchaseAccID = item.AccountID;
                item = manager.Get().FirstOrDefault(ac => ac.Type == 2);
                int? SaleTaxAccID = item.AccountID;
                Purchase oPur = managerPO.FindById(transTypeID);
                var detailsPurchase = oPur.PurchaseDetails;
                int? type = oPur.Type;
                //type is Credit Note
                switch (type)
                {
                    case 1:
                        //Create Transaction for Account Purchase Order
                        saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purchaseAccID, DateTime.Today, oPur.Reference, oPur.TotalMoney, null, null, null, null);
                        //Create Transaction for Account Sale Tax
                        if(oPur.Tax != 0)
                            saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, SaleTaxAccID, DateTime.Today, oPur.Reference, null, oPur.Tax, null, null, null);
                        foreach (PurchaseDetail purItem in detailsPurchase)
                        {
                            if (oPur.UseVAT == 1)
                            {
                                double? totalMoneyWithTax = purItem.TotalMoney - (purItem.TotalMoney * purItem.TaxRate.Rate / 100);
                                saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purItem.AccountID, DateTime.Today, null, null, totalMoneyWithTax, null, null, null);

                            }
                            else
                            {
                                saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purItem.AccountID, DateTime.Today, null, null, purItem.TotalMoney, null, null, null);
                            }
                        }
                        break;
                    case 0:
                        if (payment == null)
                        {
                            //Create Transaction for Account Purchase Order
                            saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purchaseAccID, DateTime.Today, oPur.Reference, null, oPur.TotalMoney, null, null, null);
                            UpdateDebitCreditAccount(purchaseAccID, 0, oPur.TotalMoney);
                            //Create Transaction for Account Sale Tax
                            if(oPur.Tax != 0)
                                saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, SaleTaxAccID, DateTime.Today, oPur.Reference, oPur.Tax, null, null, null, null);

                            UpdateDebitCreditAccount(purchaseAccID, 0, oPur.Tax);
                            //Create Transaction for items 
                            foreach (PurchaseDetail purItem in detailsPurchase)
                            {
                                if (oPur.UseVAT == 1)
                                {
                                    double? totalMoneyWithTax = purItem.TotalMoney - (purItem.TotalMoney * purItem.TaxRate.Rate / 100);
                                    saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purItem.AccountID, DateTime.Today, null, totalMoneyWithTax, null, null, null, null);
                                    UpdateDebitCreditAccount(purchaseAccID, totalMoneyWithTax, 0);
                                }
                                else
                                {
                                    saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? "" : oPur.Contact.ContactName, purItem.AccountID, DateTime.Today, null, purItem.TotalMoney, null, null, null, null);
                                    UpdateDebitCreditAccount(purchaseAccID, purItem.TotalMoney, 0);
                                }
                            }
                        }
                        else
                        {
                            saveInAccountTran(oPur.ID, 1, oPur.Contact == null ? Resources.Resource.Transaction_Name_Payment : Resources.Resource.Transaction_Name_Payment + " " + oPur.Contact.ContactName,
                                payment.PayToAccount, DateTime.Today, payment.Reference, payment.TotalMoney, null, null, null, AccountType);
                        }
                        break;

                    default:
                        break;
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
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", error.Message);

                }
            }
            catch (Exception e)
            {
                result = 0;
            }
            result = 1;
            return serializer.Serialize(result);

        }
        /*
                /// <summary>
                /// 
                /// </summary>
                /// <param name="SaleAccID">AccountID from Purchae or Sale Invoice</param>
                /// <param name="SaleTaxAccID">Account Tax</param>
                /// <param name="typeSale"> Purchase:0, Sale Invoice : 1</param>
                /// <param name="oPur">Purchase Object</param>
                /// <param name="detailsPurchase"></param>
                /// 
                private void saveAllTrans(int? SaleAccID, int SaleTaxAccID, int typeSale, int SaleID)
                {
                    var managerPO = new GenericManager<Purchase>();
                    Purchase oPur = managerPO.FindById(SaleID);
                    var detailsPurchase = oPur.PurchaseDetails;
                    //Create Transaction for Account Purchase Order
                    saveInAccountTran(SaleAccID, DateTime.Today, oPur.Reference, oPur.TotalMoney, null, null, null, null);
                    //Create Transaction for Account Sale Tax
                    saveInAccountTran(SaleTaxAccID, DateTime.Today, oPur.Reference, oPur.Tax, null, null, null, null);
                    //Create Transaction for items 
                    foreach (PurchaseDetail purItem in detailsPurchase)
                    {
                        if (oPur.UseVAT == 1)
                        {
                            double? totalMoneyWithTax = purItem.TotalMoney - (purItem.TotalMoney * purItem.TaxRate.Rate / 100);
                            saveInAccountTran(purItem.AccountID, DateTime.Today, null, totalMoneyWithTax, null, null, null, null);
                        }
                        else
                        {
                            saveInAccountTran(purItem.AccountID, DateTime.Today, null, purItem.TotalMoney, null, null, null, null);
                        }
                    }

                } */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountID"></param>
        /// <param name="date"></param>
        /// <param name="reference"></param>
        /// <param name="spent"></param>
        /// <param name="receive"></param>
        /// <param name="amountAre"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        private static void saveInAccountTran(int? sourceID, int? sourceType, string transactionName, int? accountID, DateTime? date, string reference, double? spent, double? receive, int? amountAre, int? status, int? type)
        {
            AccountTran objAccTran = new AccountTran();
            if (type == 0)
            {
                objAccTran.BankAccountID = accountID;
            }
            else
            {
                objAccTran.AccountID = accountID;
            }

            objAccTran.Date = date;            
            objAccTran.SourceID = sourceID;
            objAccTran.SourceType = sourceType;
            objAccTran.TransactionName = transactionName;
            objAccTran.Reference = reference;
            objAccTran.Spent = spent;
            objAccTran.Received = receive;
            objAccTran.AmountAre = amountAre;
            objAccTran.Status = status;
            objAccTran.AccountType = type;
            //include Tax
            objAccTran.AmountAre = amountAre;
            db.AccountTrans.Add(objAccTran);
            db.SaveChanges();

        }


        private static void UpdateDebitCreditAccount(int? accountID, double? debit, double? credit)
        {
            try
            {
                var manager = new GenericManager<Account>();
                var oAccount = manager.FindById(accountID);
                if (oAccount != null)
                {
                    if (oAccount.TotalCredit == 0 || oAccount.TotalCredit == null)
                    {
                        oAccount.TotalCredit = 0 + credit;
                    }
                    else
                    {
                        oAccount.TotalCredit = oAccount.TotalCredit + credit;
                    }

                    if (oAccount.TotalDebit == 0 || oAccount.TotalDebit == null)
                    {
                        oAccount.TotalDebit = 0 + debit;
                    }
                    else
                    {
                        oAccount.TotalDebit = oAccount.TotalDebit + debit;
                    }
                }
                manager.Update(oAccount);
                manager.Save();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static string PayRunAccountTransactiom(int typeAccTrans, int transTypeID, Payment payment)
        {
            object result = null;
            try
            {
                var manager = new GenericManager<AccountFeature>();
                var managerSO = new GenericManager<SaleInvoice>();
                var item = manager.Get().FirstOrDefault(ac => ac.Type == typeAccTrans);
                int? SaleInAcctID = item.AccountID;
                item = manager.Get().FirstOrDefault(ac => ac.Type == 2);
                int? SaleTaxAccID = item.AccountID;
                SaleInvoice oSale = managerSO.FindById(transTypeID);
                var detailsSaleInv = oSale.SaleInvoiceDetails;
                int? type = oSale.Type;
                //type is Credit Note
                switch (type)
                {
                    case 1:


                        //Create Transaction for Account Sale Order
                        saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, SaleInAcctID, DateTime.Today, oSale.Reference, oSale.TotalMoney, null, null, null, null);

                        //Create Transaction for Account Sale Tax
                        saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, SaleTaxAccID, DateTime.Today, oSale.Reference, oSale.TotalTax, null, null, null, null);

                        foreach (SaleInvoiceDetail saleItem in detailsSaleInv)
                        {
                            if (oSale.UseVAT == 1)
                            {
                                double? totalMoneyWithTax = saleItem.TotalMoney - (saleItem.TotalMoney * saleItem.TaxRate.Rate / 100);
                                saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, totalMoneyWithTax, null, null, null, null);
                            }
                            else
                            {
                                saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, saleItem.TotalMoney, null, null, null, null);
                            }
                        }

                        break;
                    case 0:
                        if (payment == null)
                        {
                            // Insert SaleInvoice into table AccountTrans
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, SaleInAcctID, DateTime.Today, oSale.Reference, null, oSale.TotalMoney, null, null, null);

                            //Create Transaction for Account Sale Tax
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, SaleTaxAccID, DateTime.Today, oSale.Reference, null, oSale.TotalTax, null, null, null);

                            foreach (SaleInvoiceDetail saleItem in detailsSaleInv)
                            {

                                if (oSale.UseVAT == 1)
                                {
                                    double? totalMoneyWithTax = saleItem.TotalMoney - (saleItem.TotalMoney * saleItem.TaxRate.Rate / 100);
                                    saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, null, totalMoneyWithTax, null, null, null);
                                }
                                else
                                {
                                    saveInAccountTran(oSale.ID, 0, oSale.Contact == null ? "" : oSale.Contact.ContactName, saleItem.AccountID, DateTime.Today, null, null, saleItem.TotalMoney, null, null, null);
                                }
                            }
                        }
                        // Payment Transaction
                        else
                        {
                            saveInAccountTran(oSale.ID, 0, oSale.Contact == null ?
                                Resources.Resource.Transaction_Name_Payment : Resources.Resource.Transaction_Name_Payment + " " + oSale.Contact.ContactName, payment.PayToAccount, DateTime.Today, null, null, payment.TotalMoney, null, null, null);
                        }
                        break;
                    default:
                        break;
                }


            }
            catch (Exception e)
            {
                result = 0;
            }
            result = 1;
            return serializer.Serialize(result);
        }
        #endregion Account Transaction

        #region Transaction when approved Sale
        public static string TransactionApprovedSaleOrder(int saleInvoiceId)
        {
            string result = "success";
            var manager = new GenericManager<SaleInvoice>();
            var managerAccountFeature = new GenericManager<AccountFeature>();
            var managerAccountTran = new GenericManager<AccountTran>();
            var managerBank = new GenericManager<BankAccount>();
            var managerTaxRate = new GenericManager<TaxRate>();
            var managerAccount = new GenericManager<Account>();
            var oReceived = managerAccountFeature.Get(x => x.ID == 1).First().Account;
            var oTax = managerAccountFeature.Get(x => x.ID == 3).First().Account;
            try
            {
                var oSaleInvoice = manager.FindById(saleInvoiceId);
                var ooSaleInvoiceDetails = oSaleInvoice.SaleInvoiceDetails.ToList();
                //Include Taxes
                if (oSaleInvoice.UseVAT == 1)
                {
                    //Item
                    foreach (var item in ooSaleInvoiceDetails)
                    {
                        var oTaxRate = managerTaxRate.FindById(item.TaxRateID);
                        var amountItem = item.Quantity * (item.Price / (1 + (oTaxRate.Rate / 100)));
                        var oAccountTran = new AccountTran();
                        oAccountTran.AccountID = managerAccount.FindById(item.AccountID).ID;
                        oAccountTran.AccountType = managerAccount.FindById(item.AccountID).AccountTypeID;
                        oAccountTran.Description = "Update Account Tran Item ----- Include Taxes";
                        oAccountTran.Date = DateTime.Now;
                        oAccountTran.Spent = amountItem;
                        managerAccountTran.Add(oAccountTran);
                    }
                    //Received 
                    var oAccountReceived = new AccountTran();
                    oAccountReceived.AccountID = oReceived.ID;
                    oAccountReceived.AccountType = oReceived.AccountTypeID;
                    oAccountReceived.Description = "Update Account Tran Received ----- Include Taxes";
                    oAccountReceived.Date = DateTime.Now;
                    oAccountReceived.Spent = oSaleInvoice.TotalMoney ?? (oSaleInvoice.SubTotal ?? 0);
                    managerAccountTran.Add(oAccountReceived);
                    //Tax
                    var oAccountTaxRate = new AccountTran();
                    oAccountTaxRate.AccountID = oTax.ID;
                    oAccountTaxRate.AccountType = oTax.AccountTypeID;
                    oAccountTaxRate.Description = "Update Account Tran Tax ----- Include Taxes";
                    oAccountTaxRate.Date = DateTime.Now;
                    oAccountTaxRate.Spent = oSaleInvoice.TotalTax;
                    managerAccountTran.Add(oAccountTaxRate);

                    managerAccountTran.Save();

                }
                //Exclude Taxes
                if (oSaleInvoice.UseVAT == 0 || oSaleInvoice.UseVAT == null)
                {
                    double totalAmountItem = 0;
                    double totalAmountTax = 0;
                    //Item
                    foreach (var item in ooSaleInvoiceDetails)
                    {
                        var amountItem = item.TotalMoney ?? ((item.Quantity * item.Price) - ((item.Discount * item.Price) / 100));
                        totalAmountItem += Convert.ToDouble(amountItem);
                        var oAccountTran = new AccountTran();
                        oAccountTran.AccountID = item.Account.ID;
                        oAccountTran.AccountType = item.Account.AccountTypeID;
                        oAccountTran.Description = "Update Account Tran Item ----- Exclude Taxes";
                        oAccountTran.Date = DateTime.Now;
                        oAccountTran.Spent = amountItem;
                        managerAccountTran.Add(oAccountTran);

                        if (item.TaxRateID != null)
                        {
                            var oTaxRate = item.TaxRate;
                            totalAmountTax += Convert.ToDouble((amountItem * oTaxRate.Rate) / 100);
                        }
                    }

                    //var oBank = managerBank.Get().First();
                    //Received
                    var oAccountReceived = new AccountTran();
                    oAccountReceived.AccountID = oReceived.ID;
                    oAccountReceived.AccountType = oReceived.AccountTypeID;
                    oAccountReceived.Description = "Update Account Tran Received ----- Exclude Taxes";
                    oAccountReceived.Date = DateTime.Now;
                    oAccountReceived.Spent = totalAmountItem + totalAmountTax;
                    managerAccountTran.Add(oAccountReceived);

                    //TaxRate
                    var oAccountTaxRate = new AccountTran();
                    oAccountTaxRate.AccountID = oTax.ID;
                    oAccountTaxRate.AccountType = oTax.AccountTypeID;
                    oAccountTaxRate.Description = "Update Account Tran Tax ----- Exclude Taxes";
                    oAccountTaxRate.Date = DateTime.Now;
                    oAccountTaxRate.Spent = totalAmountTax;
                    managerAccountTran.Add(oAccountTaxRate);
                    //Save
                    managerAccountTran.Save();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion
        //public static string DateFormat(DateTime date)
        //{
        //    var result = date.GetDateTimeFormats()[6];
        //    return result;
        //}
        public static string DateFormat(DateTime date)
        {
            var result = date.ToString("dd-MMM-yyyy");
            return result;
        }
    }
}