using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.BusinessLogic.Utils
{
    public class ScheduleTask
    {
        public static void ExecRepeating(int repeatType)
        {
            int CODE_LENGTH = 5;
            var currentDateTime = DateTime.Today;
            var manager = new GenericManager<TMT.ERP.DataAccess.Model.Repeating>();
            var tasks = manager.Get().Where(x => x.RepeatType == repeatType && x.DateRun.Date >=currentDateTime);
            if (repeatType == 0) //sale repeating
            {
                var saleManager = new GenericManager<SaleInvoice>();

                foreach (var item in tasks)
                {
                    try
                    {
                        SaleInvoice objSaleInvoice = new SaleInvoice();
                        objSaleInvoice.Code = Util.GetCode(repeatType.ToString());
                        objSaleInvoice.CreatedDate = item.DateRun;
                        objSaleInvoice.DueDate = GetDueDate(item.DateRun, item.DueDateNum, item.DueDateTypeID);
                        objSaleInvoice.CreatedUserID = item.SaleInvoice.CreatedUserID;
                        objSaleInvoice.Status = item.Status;
                        objSaleInvoice.ContactName = item.SaleInvoice.ContactName;
                        objSaleInvoice.ContactID = item.SaleInvoice.ContactID;
                        objSaleInvoice.Reference = item.SaleInvoice.Reference;
                        objSaleInvoice.CurrencyID = item.SaleInvoice.CurrencyID;
                        objSaleInvoice.VAT = item.SaleInvoice.VAT;
                        objSaleInvoice.TaxAmountType = item.SaleInvoice.TaxAmountType;
                        objSaleInvoice.SubTotal = item.SaleInvoice.SubTotal;
                        objSaleInvoice.TotalTax = item.SaleInvoice.TotalTax;
                        objSaleInvoice.TotalMoney = item.SaleInvoice.TotalMoney;
                        objSaleInvoice.Description = item.SaleInvoice.Description;
                        objSaleInvoice.Type = item.SaleInvoice.Type;
                        objSaleInvoice.UseVAT = item.SaleInvoice.UseVAT;
                        objSaleInvoice.StockID = item.SaleInvoice.StockID;
                        objSaleInvoice.ApprovalUserID = item.SaleInvoice.ApprovalUserID;
                        objSaleInvoice.CompanyID = item.SaleInvoice.User.CompanyID;

                        saleManager.Add(objSaleInvoice);
                        foreach (var subItem in item.SaleInvoice.SaleInvoiceDetails)
                        {
                            var objSaleDetail = new SaleInvoiceDetail
                            {
                                GoodID = subItem.GoodID,
                                UOMID = subItem.UOMID,
                                Quantity = subItem.Quantity,
                                Price = subItem.Price,
                                TaxRateID = subItem.TaxRateID,
                                Discount = subItem.Discount,
                                Subtotal = subItem.Subtotal,
                                AccountID = subItem.AccountID,
                                TotalMoney = subItem.TotalMoney,
                                Description = subItem.Description
                            };
                            objSaleInvoice.SaleInvoiceDetails.Add(objSaleDetail);
                        }
                        //update NextCode in Setting
                        Util.UpdateNextNumber(repeatType.ToString(), CODE_LENGTH, "");

                        //update next repeating
                        item.DateRun = GetNextRepeatingDate(item.DateRun, item.NumDayRepeat, item.RepeatTime);
                    }
                    catch { }
                }
                saleManager.Save();
            }
            else //purchase repeating
            {
                var purchaseManager = new GenericManager<Purchase>();

                foreach (var item in tasks)
                {
                    try
                    {
                        Purchase objPurchase = new Purchase
                        {
                            Code = Util.GetCode(repeatType.ToString()),
                            CreatedDate = item.DateRun,
                            DueDate = GetDueDate(item.DateRun, item.DueDateNum, item.DueDateTypeID),
                            CreatedUserID = item.Purchase.CreatedUserID,
                            //Status = item.Status==null?0:item.Status,
                            ContactName = item.Purchase.ContactName,
                            ContactID = item.Purchase.ContactID,
                            Reference = item.Purchase.Reference,
                            CurrencyID = item.Purchase.CurrencyID,
                            VAT = item.Purchase.VAT,
                            Discount = item.Purchase.Discount,
                            SubTotal = item.Purchase.SubTotal,
                            Tax = item.Purchase.Tax,
                            TotalMoney = item.Purchase.TotalMoney,
                            Type = item.Purchase.Type,
                            UseVAT = item.Purchase.UseVAT,
                            StockID = item.Purchase.StockID,
                            ApprovalUserID = item.Purchase.ApprovalUserID,
                            CompanyID = item.Purchase.User.CompanyID
                        };

                        purchaseManager.Add(objPurchase);
                        foreach (var subItem in item.Purchase.PurchaseDetails)
                        {
                            var objPurchaseDetail = new PurchaseDetail
                            {
                                GoodID = subItem.GoodID,
                                UOMID = subItem.UOMID,
                                Quantity = subItem.Quantity,
                                Price = subItem.Price,
                                TaxRateID = subItem.TaxRateID,
                                Discount = subItem.Discount,
                                AccountID = subItem.AccountID,
                                TotalMoney = subItem.TotalMoney,
                                Description = subItem.Description
                            };
                            objPurchase.PurchaseDetails.Add(objPurchaseDetail);
                        }
                        //update NextCode in Setting
                        Util.UpdateNextNumber(repeatType.ToString(), CODE_LENGTH, "");

                        //update next repeating
                        item.DateRun = GetNextRepeatingDate(item.DateRun, item.NumDayRepeat, item.RepeatTime);
                    }
                    catch { }
                }
                purchaseManager.Save();                
            
            }
            try
            {
                manager.Save();
            }
            catch { }
        }

        //dueDateType=1-of the following month; 2-day(s) after the invoice date; 3-of the current month

        public static DateTime GetDueDate(DateTime invoiceDate, int dayNum, int dueDateType)
        {
            DateTime dueDate = invoiceDate;
            switch (dueDateType)
            {
                case 1:
                    {
                        var nextMonth = invoiceDate.AddMonths(1);
                        dueDate = new DateTime(nextMonth.Year, nextMonth.Month, dayNum);
                        break;
                    }
                case 2:
                    {
                        dueDate = invoiceDate.AddDays(dayNum);
                        break;
                    }
                case 3:
                    {
                        dueDate = new DateTime(invoiceDate.Year, invoiceDate.Month, dayNum);
                        break;
                    }
            }
            return dueDate;
        }

        //repeatTime=0-Week;1-Month
        public static DateTime GetNextRepeatingDate(DateTime invoiceDate, int NumDayRepeat, int repeatTime)
        {
            DateTime dateRun = invoiceDate;
            if(repeatTime==0)
                dateRun = invoiceDate.AddDays(NumDayRepeat*7);
            else
                dateRun = invoiceDate.AddMonths(NumDayRepeat);
            return dateRun;
        }

    }
}
