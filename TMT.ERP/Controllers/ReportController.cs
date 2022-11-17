using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class ReportController : BaseController
    {
        public ActionResult Profit_Loss()
        {
            var obj = new Profit_Loss();

            var company = new GenericManager<TMT.ERP.DataAccess.Model.Company>().Get();
            var userCurrentCompany = new GenericManager<TMT.ERP.DataAccess.Model.User>().Get().Where(x => x.ID == TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID);
            var currCompany = (from c in company
                               join u in userCurrentCompany on c.ID equals u.CompanyID
                               select c).FirstOrDefault();

            //Resources.Resource
            obj = obj.SetPropertyAll(SERVER_MAPPATH, "Profit_LossTemp.xlsx", FORMATNUMBEREXPORT_1, "Profit Loss", "Profit & Loss", "Income", "Total Income",
                "Gross Profit", "Less Operating Expenses", "Total Operating Expenses", "Net Profit", DateTime.Now, FORMATDATETIME_1, "For the month ended ",
                currCompany.DisplayName, "Profit Loss_", FORMATDATETIME_3, FORMATDATETIME_2, "YTD");
                       
            //income
            obj.Income = GetObjectForIncome_LOE(5);
            //Less Operating Expenses 
            obj.OperatingExpenses = GetObjectForIncome_LOE(4);
            SetValueResult(obj);

            Obj = obj;
            return View(obj);
        }

        public ActionResult Inventory_Items_Summary(string dtFrom, string dtTo)
        {
            var obj = new Inventory_Items_Summary();

            DateTime sDateTime = string.IsNullOrEmpty(dtFrom) ? GetFirstDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtFrom).Date;
            DateTime eDateTime = string.IsNullOrEmpty(dtTo) ? GetLastDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtTo).Date;

            var company = new GenericManager<TMT.ERP.DataAccess.Model.Company>().Get();
            var userCurrentCompany = new GenericManager<TMT.ERP.DataAccess.Model.User>().Get().Where(x => x.ID == TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID);
            var currCompany = (from c in company
                               join u in userCurrentCompany on c.ID equals u.CompanyID
                               select c).FirstOrDefault();

            //Resources.Resource
            obj = obj.SetPropertyAll(SERVER_MAPPATH, "Inventory_Items_SummaryTemp.xlsx", FORMATNUMBEREXPORT_1, "Inventory Items Summary", "Inventory Items Summary",
                "Item", "Purchases Price", "Qty", "Total", "Avg", "WO Price", "Qty", "Total", "Avg", "MPS Price", "Qty", "Total", "Avg", "Sales Price", "Qty",
                "Total", "Avg", "Net Qty", "Net Total", currCompany.DisplayName, sDateTime, eDateTime, FORMATDATETIME_1, "to", "Total", "Inventory Items Summary_",
                FORMATDATETIME_3, FORMATNUMBEREXPORT_2);

            GetObjectForInventory_Item_Summary(obj, sDateTime, eDateTime, currCompany.ID);

            Obj = obj;

            return View(obj);
        }

        public ActionResult CashSummary_Report(string s_dtReport)
        {
            var obj = new Cash_Summary();

            DateTime dtReport = string.IsNullOrEmpty(s_dtReport) ? GetFirstDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(s_dtReport).Date;
            
            var company = new GenericManager<TMT.ERP.DataAccess.Model.Company>().Get();
            var userCurrentCompany = new GenericManager<TMT.ERP.DataAccess.Model.User>().Get().Where(x => x.ID == TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID);
            var currCompany = (from c in company
                               join u in userCurrentCompany on c.ID equals u.CompanyID
                               select c).FirstOrDefault();

            //Resources.Resource
            obj = obj.SetPropertyAll(SERVER_MAPPATH, "Cash_SummaryTemp.xlsx", FORMATNUMBEREXPORT_1, "Cash Summary", "Cash Summary", currCompany.DisplayName, DateTime.Now, FORMATDATETIME_1, "For the month ended ", "Cash Summary_",
                FORMATDATETIME_3, FORMATNUMBEREXPORT_2, "Excluding Tax", "Income", "Total Income", "Less Operating Expenses", "Total Operating Expenses",
                "Operating Surplus (Deficit)", "Plus Movements in Equity", "Total Movements in Equity", "Tax Movements", "Net Tax Movements", "Net Cash Movement",
                "Summary", "Opening Balance", "Plus Net Cash Movement", "Closing Balance", FORMATNUMBEREXPORT_3, dtReport, FORMATDATETIME_4,
                "YTD Actual", "Income %");

            obj.Income = GetObjectForIncome(dtReport);

            obj.OperatingExpenses = GetObjectForOperatingExpense(dtReport, GetNumber(obj.Set_Income.Total_Month_01.Value), GetNumber(obj.Set_Income.Total_YTD.Value));

            obj.TaxMovements = GetObjectForTaxMovements(dtReport, GetNumber(obj.Set_Income.Total_Month_01.Value), GetNumber(obj.Set_Income.Total_YTD.Value));

            SetValueResult(obj);

            Obj = obj;

            return View(obj);
        }
        
        public ActionResult GetMonth_CashSummary()
        {
            List<string> oIE = new List<string>();
            for (int i = 0; i >= -12; i--)
            {
                oIE.Add(DateTime.Now.AddMonths(i).ToString("MMMM yyyy"));
            }

            ViewBag.MonthID = new SelectList(oIE);

            return View();
        }

        public ActionResult Customer_Invoice_Report(string dtFrom, string dtTo)
        {
            var obj = new CustomerInvoice_SupplierInvoice_Report();

            DateTime sDateTime = string.IsNullOrEmpty(dtFrom) ? GetFirstDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtFrom).Date;
            DateTime eDateTime = string.IsNullOrEmpty(dtTo) ? GetLastDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtTo).Date;

            var company = new GenericManager<TMT.ERP.DataAccess.Model.Company>().Get();
            var userCurrentCompany = new GenericManager<TMT.ERP.DataAccess.Model.User>().Get().Where(x => x.ID == TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID);
            var currCompany = (from c in company
                               join u in userCurrentCompany on c.ID equals u.CompanyID
                               select c).FirstOrDefault();

            //Resources.Resource
            //tính lại chố việt nam đồng
            var sReport = "Customer Invoice Report (Vietnamese Dong)";
            //tính lại chố việt nam đồng
            obj = obj.SetPropertyAll(SERVER_MAPPATH, "Customer_InvoiceTemp.xlsx", "Customer Invoice", sReport,
                currCompany.DisplayName, sDateTime, eDateTime, FORMATDATETIME_5, "From", "to", "Report Total",
                sReport + "_", FORMATDATETIME_3, 1, "Invoice Number", "Reference", "Type", "To", "Date", "Due Date", "Paid Date",
                "Invoice Total", "Paid", "Due" , "Status", FORMATNUMBEREXPORT_1);
            obj.s_Type = new List<string>() { "INV", "CR" };
            obj.s_Status = new List<string>() { "Draft", "Awaiting Approval", "Approval", "Paid", "Repeating", "Rejected", "Done" };
            obj.CustomerInvoice_SupplierInvoices = GetCustomerInvoice(sDateTime, eDateTime);

            var total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Invoice_Total);
            obj.s_Total_Invoice_Total = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Paid);
            obj.s_Total_Paid = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Due);
            obj.s_Total_Due = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            Obj = obj;
            return View(obj);
        }

        public ActionResult Supplier_Invoice_Report(string dtFrom, string dtTo)
        {
            var obj = new CustomerInvoice_SupplierInvoice_Report();

            DateTime sDateTime = string.IsNullOrEmpty(dtFrom) ? GetFirstDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtFrom).Date;
            DateTime eDateTime = string.IsNullOrEmpty(dtTo) ? GetLastDayOfMonth(DateTime.Now.Date) : Convert.ToDateTime(dtTo).Date;

            var company = new GenericManager<TMT.ERP.DataAccess.Model.Company>().Get();
            var userCurrentCompany = new GenericManager<TMT.ERP.DataAccess.Model.User>().Get().Where(x => x.ID == TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID);
            var currCompany = (from c in company
                               join u in userCurrentCompany on c.ID equals u.CompanyID
                               select c).FirstOrDefault();

            //Resources.Resource
            //tính lại chố việt nam đồng
            var sReport = "Supplier Invoice Report (Vietnamese Dong)";
            //tính lại chố việt nam đồng

            obj = obj.SetPropertyAll(SERVER_MAPPATH, "Supplier_InvoiceTemp.xlsx", "Supplier Invoice", sReport,
                currCompany.DisplayName, sDateTime, eDateTime, FORMATDATETIME_5, "From", "to", "Report Total",
                sReport + "_", FORMATDATETIME_3, 2, "Reference", "Type", "From", "Date", "Due Date", "Paid Date",
                "Invoice Total", "Paid", "Due", "Status", FORMATNUMBEREXPORT_1);
            
            obj.s_Type = new List<string>() { "INV", "CR" };
            obj.s_Status = new List<string>() { "Draft", "Awaiting Approval", "Approval", "Paid", "Repeating", "Rejected", "Done" };
            obj.CustomerInvoice_SupplierInvoices = GetSupplierInvoice(sDateTime, eDateTime);

            var total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Invoice_Total);
            obj.s_Total_Invoice_Total = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Paid);
            obj.s_Total_Paid = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            total = obj.CustomerInvoice_SupplierInvoices.Sum(x => x.Due);
            obj.s_Total_Due = SetNegativeNumber(total.Value.ToString(FORMATNUMBER_1));

            Obj = obj;

            return View(obj);
        }

        #region Private constant Function
        private readonly string SERVER_MAPPATH = System.Web.HttpContext.Current.Server.MapPath("~/Report/Temp/");
        private readonly string FORMATDATETIME_1 = "d MMMM yyyy";
        private readonly string FORMATDATETIME_2 = "MMM-yy";
        private readonly string FORMATDATETIME_3 = "yyyy-MM-dd HH:mm:ss:ffff tt";
        private readonly string FORMATDATETIME_4 = "MMM yyyy";
        private readonly string FORMATDATETIME_5 = "dd MMMM yyyy";
        private readonly string FORMATDATETIME_6 = "dd MMM yyyy";
        private readonly string FORMATNUMBER_1 = "#,0.00";
        private readonly string FORMATNUMBER_2 = "#,0.0";
        private readonly string FORMATNUMBEREXPORT_1 = "\"đ\"#,##0.00;-\"đ\"#,##0.00";
        private readonly string FORMATNUMBEREXPORT_2 = "#,##0.0_ ;-#,##0.0";
        private readonly string FORMATNUMBEREXPORT_3 = "0.0###%;-0.0###%";
        private static object Obj;       
        #endregion Private constant Function

        #region Private Function
        #region Private Sub Function
        private string SetNegativeNumber(string sInput)
        {
            return sInput = (sInput.IndexOf("-") > -1) ? sInput.Replace("-", "(") + ")" : sInput;
        }
        
        private DateTime GetFirstDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).Date;
        }

        private DateTime GetLastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month)).Date;
        }
                
        private DateTime GetLastDayOfYear(DateTime dt)
        {
            return new DateTime(dt.Year, 12, DateTime.DaysInMonth(dt.Year, 12)).Date;
        }
        
        #endregion Private Sub Function

        #region Private Profit_Loss
        /// <summary>
        /// iAccountType: 4 Expenses
        /// iAccountType: 5 Revenue
        /// </summary>
        /// <param name="iAccountType"></param>
        /// <returns></returns>
        private List<Income_LOE> GetObjectForIncome_LOE(int iAccountType)
        {
            List<Income_LOE> objCol = null;

            var account = new GenericManager<TMT.ERP.DataAccess.Model.Account>().Get();
            var accounttype = new GenericManager<TMT.ERP.DataAccess.Model.AccountType>().Get();
            var accountTrans = new GenericManager<TMT.ERP.DataAccess.Model.AccountTran>().Get().Where(at => at.Date.Value.Year == DateTime.Now.Year);
            if (account != null && accountTrans != null && accounttype != null)
            {
                objCol = new List<Income_LOE>();
                var value = (from a in account
                             join ap in accounttype on a.AccountTypeID equals ap.ID
                             join at in accountTrans on a.ID equals at.AccountID
                             select new { a, ap, at });
                value = value.Where(vl => vl.ap.Type == iAccountType);
                var val = value.GroupBy(vl => vl.at.AccountID);
                if (val != null && val.Count() > 0)
                {

                    for (int i = 0; i < val.Count(); i++)
                    {
                        Income_LOE oObj = new Income_LOE();
                        oObj.AccountID = val.ElementAt(i).Select(x => x.a.ID).FirstOrDefault();
                        oObj.AccountType = val.ElementAt(i).Select(x => x.ap.Type.Value).FirstOrDefault();
                        oObj.AccountName = val.ElementAt(i).Select(x => x.a.Name).FirstOrDefault();
                        oObj.TotalAccountTran_1 = (from item in val.ElementAt(i)
                                                   where item.at.Date > GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                                                   && item.at.Date <= GetLastDayOfMonth(DateTime.Now.AddMonths(0))
                                                   select item).Sum(x => (iAccountType == 4) ? x.at.Received : x.at.Spent);
                        oObj.s_TotalAccountTran_1 = SetNegativeNumber(oObj.TotalAccountTran_1.Value.ToString(FORMATNUMBER_1));
                        oObj.TotalAccountTran_2 = (from item in val.ElementAt(i)
                                                   where item.at.Date > GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                                                   && item.at.Date <= GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                                                   select item).Sum(x => (iAccountType == 4) ? x.at.Received : x.at.Spent);
                        oObj.s_TotalAccountTran_2 = SetNegativeNumber(oObj.TotalAccountTran_2.Value.ToString(FORMATNUMBER_1));
                        oObj.TotalAccountTran_3 = (from item in val.ElementAt(i)
                                                   where item.at.Date > GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                                                   && item.at.Date <= GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                                                   select item).Sum(x => (iAccountType == 4) ? x.at.Received : x.at.Spent);
                        oObj.s_TotalAccountTran_3 = SetNegativeNumber(oObj.TotalAccountTran_3.Value.ToString(FORMATNUMBER_1));
                        oObj.TotalAccountTran_4 = (from item in val.ElementAt(i)
                                                   where item.at.Date > GetLastDayOfMonth(DateTime.Now.AddMonths(-4))
                                                   && item.at.Date <= GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                                                   select item).Sum(x => (iAccountType == 4) ? x.at.Received : x.at.Spent);
                        oObj.s_TotalAccountTran_4 = SetNegativeNumber(oObj.TotalAccountTran_4.Value.ToString(FORMATNUMBER_1));
                        oObj.TotalAccountTran_YTD = (from item in val.ElementAt(i)
                                                     where item.at.Date > GetLastDayOfYear(DateTime.Now.AddYears(-1))
                                                     && item.at.Date <= GetLastDayOfYear(DateTime.Now.AddYears(0))
                                                     select item).Sum(x => (iAccountType == 4) ? x.at.Received : x.at.Spent);
                        oObj.s_TotalAccountTran_YTD = SetNegativeNumber(oObj.TotalAccountTran_YTD.Value.ToString(FORMATNUMBER_1));
                        objCol.Add(oObj);
                        oObj = null;
                    }


                }
            }
            return objCol;
        }

        /// <summary>
        /// Set value display
        /// </summary>
        /// <param name="pl"></param>
        private void SetValueResult(Profit_Loss pl)
        {
            var TotalInCome_1 = pl.Income.Sum(x => x.TotalAccountTran_1);
            pl.s_TotalIncome_1 = SetNegativeNumber(TotalInCome_1.Value.ToString(FORMATNUMBER_1));
            var TotalOperatingExpenses_1 = pl.OperatingExpenses.Sum(x => x.TotalAccountTran_1);
            pl.s_TotalOperatingExpenses_1 = SetNegativeNumber(TotalOperatingExpenses_1.Value.ToString(FORMATNUMBER_1));
            var NetProfit_1 = TotalInCome_1 - TotalOperatingExpenses_1;
            pl.s_NetProfit_1 = SetNegativeNumber(NetProfit_1.Value.ToString(FORMATNUMBER_1));

            var TotalInCome_2 = pl.Income.Sum(x => x.TotalAccountTran_2);
            pl.s_TotalIncome_2 = SetNegativeNumber(TotalInCome_2.Value.ToString(FORMATNUMBER_1));
            var TotalOperatingExpenses_2 = pl.OperatingExpenses.Sum(x => x.TotalAccountTran_2);
            pl.s_TotalOperatingExpenses_2 = SetNegativeNumber(TotalOperatingExpenses_2.Value.ToString(FORMATNUMBER_1));
            var NetProfit_2 = TotalInCome_2 - TotalOperatingExpenses_2;
            pl.s_NetProfit_2 = SetNegativeNumber(NetProfit_2.Value.ToString(FORMATNUMBER_1));

            var TotalInCome_3 = pl.Income.Sum(x => x.TotalAccountTran_3);
            pl.s_TotalIncome_3 = SetNegativeNumber(TotalInCome_3.Value.ToString(FORMATNUMBER_1));
            var TotalOperatingExpenses_3 = pl.OperatingExpenses.Sum(x => x.TotalAccountTran_3);
            pl.s_TotalOperatingExpenses_3 = SetNegativeNumber(TotalOperatingExpenses_3.Value.ToString(FORMATNUMBER_1));
            var NetProfit_3 = TotalInCome_3 - TotalOperatingExpenses_3;
            pl.s_NetProfit_3 = SetNegativeNumber(NetProfit_3.Value.ToString(FORMATNUMBER_1));

            var TotalInCome_4 = pl.Income.Sum(x => x.TotalAccountTran_4);
            pl.s_TotalIncome_4 = SetNegativeNumber(TotalInCome_4.Value.ToString(FORMATNUMBER_1));
            var TotalOperatingExpenses_4 = pl.OperatingExpenses.Sum(x => x.TotalAccountTran_4);
            pl.s_TotalOperatingExpenses_4 = SetNegativeNumber(TotalOperatingExpenses_4.Value.ToString(FORMATNUMBER_1));
            var NetProfit_4 = TotalInCome_4 - TotalOperatingExpenses_4;
            pl.s_NetProfit_4 = SetNegativeNumber(NetProfit_4.Value.ToString(FORMATNUMBER_1));

            var TotalInCome_YTD = pl.Income.Sum(x => x.TotalAccountTran_YTD);
            pl.s_TotalIncome_YTD = SetNegativeNumber(TotalInCome_YTD.Value.ToString(FORMATNUMBER_1));
            var TotalOperatingExpenses_YTD = pl.OperatingExpenses.Sum(x => x.TotalAccountTran_YTD);
            pl.s_TotalOperatingExpenses_YTD = SetNegativeNumber(TotalOperatingExpenses_YTD.Value.ToString(FORMATNUMBER_1));
            var NetProfit_YTD = TotalInCome_YTD - TotalOperatingExpenses_YTD;
            pl.s_NetProfit_YTD = SetNegativeNumber(NetProfit_YTD.Value.ToString(FORMATNUMBER_1));
        }
        #endregion Private Profit_Loss

        #region Private Inventory_Item_Summary

        #region Private Sub Inventory_Item_Summary

        #region Private Purchase
        #region Private Calculate Purchase
        /// <summary>
        /// Sell - iStatus: 2, iType: 0;
        /// Return - iStatus: 2, iType: 1;
        /// </summary>        
        private int Sell_Return_ItemQty_Purchases(int iStatus, int iType, int GoodId, DateTime dtFrom, DateTime dtTo)
        {
            return new GenericManager<TMT.ERP.DataAccess.Model.PurchaseDetail>().Get().Where(psd => psd.GoodID == GoodId && psd.Purchase.Status == iStatus
                && psd.Purchase.Type == iType && psd.Purchase.ApprovalDate >= dtFrom.Date && psd.Purchase.ApprovalDate <= dtTo.Date).Sum(p=>p.Quantity);
        }

        /// <summary>
        /// Sell - iStatus: 2, iType: 0;
        /// Return - iStatus: 2, iType: 1;
        /// </summary> 
        private double? Sell_Return_ItemPrice_Purchases(int iStatus, int iType, int GoodId, DateTime dtFrom, DateTime dtTo)
        {
            var obj = new GenericManager<TMT.ERP.DataAccess.Model.PurchaseDetail>().Get().Where(psd => psd.GoodID == GoodId && psd.Purchase.Status == iStatus
                && psd.Purchase.Type == iType && psd.Purchase.ApprovalDate >= dtFrom.Date && psd.Purchase.ApprovalDate <= dtTo.Date);

            return obj.Sum(p => p.Quantity) * obj.Sum(p => p.Price);
        }

        /// <summary>
        /// Get Qty Price for Purchases
        /// </summary> 
        private void GetQty_Price_Purchases(SummaryForInventoryItem obj, int GoodId, double? dValue, DateTime dtFrom, DateTime dtTo)
        {
            obj.Value.Value = dValue;
            obj.Value.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

            var iQty = Sell_Return_ItemQty_Purchases(2, 0, GoodId, dtFrom, dtTo);
            iQty -= Sell_Return_ItemQty_Purchases(2, 1, GoodId, dtFrom, dtTo);
            obj.Qty = iQty;
            obj.s_Qty = SetNegativeNumber(obj.Qty.Value.ToString(FORMATNUMBER_2));

            var dPrice = Sell_Return_ItemPrice_Purchases(2, 0, GoodId, dtFrom, dtTo);
            dPrice -= Sell_Return_ItemPrice_Purchases(2, 1, GoodId, dtFrom, dtTo);
            obj.Total.Value = dPrice;
            obj.Total.Name = SetNegativeNumber(dPrice.Value.ToString(FORMATNUMBER_1));

            obj.Avg.Value = iQty != 0 ? dPrice / iQty : 0;
            obj.Avg.Name = SetNegativeNumber(obj.Avg.Value.Value.ToString(FORMATNUMBER_1));            
        }
        #endregion Private Calculate Purchase

        /// <summary>
        /// Get value Purchase for Inventory Item Summary
        /// </summary>
        private List<Inventory_Item_Summary> GetPurchase(DateTime dtFrom, DateTime dtTo)
        {
            List<Inventory_Item_Summary> objFin = new List<Inventory_Item_Summary>();

            var good = new GenericManager<TMT.ERP.DataAccess.Model.PurchaseDetail>().Get()
                .Where(p => p.Purchase.ApprovalDate >= dtFrom.Date && p.Purchase.ApprovalDate <= dtTo.Date && p.Purchase.Status == 2)
                .Select(x => new { ID = x.GoodID, x.Good.Name, x.Good.InputPrice }).GroupBy(gr => gr.Name);


            //var goodManager = new GenericManager<TMT.ERP.DataAccess.Model.Good>();
            //var good = goodManager.Get().Select(x =>
            //{
            //    var purchaseDetails = x.PurchaseDetails
            //        .Where(y => y.Purchase.ApprovalDate >= dtFrom.Date && y.Purchase.ApprovalDate <= dtTo.Date && y.Purchase.Status == 2).ToList();
            //    return new { x.ID, x.Name, x.InputPrice };
            //}).GroupBy(gr => gr.Name);

            if (good != null)
            {
                objFin = new List<Inventory_Item_Summary>();
                for (int i = 0; i < good.Count(); i++)
                {                    
                    if (good.ElementAt(i).Select(x => x.Name).FirstOrDefault() != null)
                    {
                        var obj = new Inventory_Item_Summary();
                        var objCol = new SummaryForInventoryItem();
                        obj.Item = good.ElementAt(i).Select(x => x.Name).FirstOrDefault();
                        int goodId = good.ElementAt(i).Select(x => x.ID).FirstOrDefault();
                        var dValue = good.ElementAt(i).Select(x => x.InputPrice).FirstOrDefault();
                        GetQty_Price_Purchases(objCol, goodId, dValue, dtFrom, dtTo);
                        obj.Purchases_Price = objCol;
                        objFin.Add(obj);
                        obj = null;
                        objCol = null;
                    }                   
                }
            }
            return objFin;
        }
        #endregion Private Purchase

        #region Private Outgoing
        #region Private Calculate Outgoing
        /// <summary>
        /// Get Qty Price for Outgoing
        /// </summary> 
        private void GetQty_Price_Outgoing(SummaryForInventoryItem obj, int GoodId, double? dValue, DateTime dtFrom, DateTime dtTo)
        {
            obj.Value.Value = dValue;
            obj.Value.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

            var socd = new GenericManager<TMT.ERP.DataAccess.Model.StockOutCardsDetail>().Get().Where(s => s.GoodID == GoodId && s.StockOutCard.ApprovalDate >= dtFrom.Date && s.StockOutCard.ApprovalDate <= dtTo.Date);
            var iQty = socd.Sum(x => x.Quantity - x.RemainQuantity);
            obj.Qty = iQty;
            obj.s_Qty = SetNegativeNumber(obj.Qty.Value.ToString(FORMATNUMBER_2));

            var dTotal = socd.Sum(x => (x.Quantity - x.RemainQuantity) * x.Price);
            obj.Total.Value = dTotal;
            obj.Total.Name = SetNegativeNumber(dTotal.Value.ToString(FORMATNUMBER_1));

            obj.Avg.Value = iQty != 0 ? dTotal / iQty : 0;
            obj.Avg.Name = SetNegativeNumber(obj.Avg.Value.Value.ToString(FORMATNUMBER_1));            
        }
        #endregion Private Calculate Outgoing

        /// <summary>
        /// Get value Work Order Stock Out Card for Inventory Item Summary
        /// </summary>
        private void GetOutgoing(List<Inventory_Item_Summary> objFin, DateTime dtFrom, DateTime dtTo)
        {
            var good = new GenericManager<TMT.ERP.DataAccess.Model.StockOutCardsDetail>().Get()
                .Where(y => y.StockOutCard.ApprovalDate >= dtFrom.Date && y.StockOutCard.ApprovalDate <= dtTo.Date && y.StockOutCard.Type == 1
                            && (y.StockOutCard.Status == 1 || y.StockOutCard.Status == 2))
                .Select(x => new { ID = x.GoodID, x.Good.Name, x.Good.InputPrice }).GroupBy(gr => gr.Name);

            //var goodManager = new GenericManager<TMT.ERP.DataAccess.Model.Good>();
            //var good = goodManager.Get().Select(x =>
            //{
            //    var stockOutCardsDetails = x.StockOutCardsDetails
            //        .Where(y => y.StockOutCard.ApprovalDate >= dtFrom.Date && y.StockOutCard.ApprovalDate <= dtTo.Date && y.StockOutCard.Type == 1 && (y.StockOutCard.Status == 1 || y.StockOutCard.Status == 2)).ToList();
            //    return new { x.ID, x.Name, x.InputPrice };
            //}).GroupBy(gr => gr.Name);

            if (good != null)
            {
                for (int i = 0; i < good.Count(); i++)
                {
                    if (good.ElementAt(i).Select(x => x.Name).FirstOrDefault() != null)
                    {
                        try
                        {
                            var test = objFin[i];
                        }
                        catch (Exception)
                        {
                            var obj = new Inventory_Item_Summary();
                            objFin.Add(obj);
                        }
                        var objCol = new SummaryForInventoryItem();
                        objFin[i].Item = good.ElementAt(i).Select(x => x.Name).FirstOrDefault();
                        int goodId = good.ElementAt(i).Select(x => x.ID).FirstOrDefault();
                        var dValue = good.ElementAt(i).Select(x => x.InputPrice).FirstOrDefault();
                        GetQty_Price_Outgoing(objCol, goodId, dValue, dtFrom, dtTo);
                        objFin[i].WO_Price = objCol;
                        objCol = null;
                    }
                }
            }            
        }
        #endregion Private Outgoing

        #region Private Incoming
        #region Private Calculate Incoming
        /// <summary>
        /// Get Qty Price for Incoming
        /// </summary> 
        private void GetQty_Price_Incoming(SummaryForInventoryItem obj, int GoodId, double? dValue, DateTime dtFrom, DateTime dtTo)
        {
            obj.Value.Value = dValue;
            obj.Value.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

            var sicd = new GenericManager<TMT.ERP.DataAccess.Model.StockInCardsDetail>().Get().Where(s => s.GoodID == GoodId && s.StockInCard.ApprovalDate >= dtFrom.Date && s.StockInCard.ApprovalDate <= dtTo.Date);
            var iQty = sicd.Sum(x => x.Quantity);
            obj.Qty = iQty;
            obj.s_Qty = SetNegativeNumber(obj.Qty.Value.ToString(FORMATNUMBER_2));

            var dTotal = sicd.Sum(x => x.TotalMoney);
            obj.Total.Value = dTotal;
            obj.Total.Name = SetNegativeNumber(dTotal.Value.ToString(FORMATNUMBER_1));

            obj.Avg.Value = iQty != 0 ? dTotal / iQty : 0;
            obj.Avg.Name = SetNegativeNumber(obj.Avg.Value.Value.ToString(FORMATNUMBER_1));            
        }
        #endregion Private Calculate Incoming

        /// <summary>
        /// Get value Work Order StockInCard for Inventory Item Summary
        /// </summary>
        private void GetIncoming(List<Inventory_Item_Summary> objFin, DateTime dtFrom, DateTime dtTo)
        {
            var good = new GenericManager<TMT.ERP.DataAccess.Model.StockInCardsDetail>().Get()
                .Where(y => y.StockInCard.ApprovalDate >= dtFrom.Date && y.StockInCard.ApprovalDate <= dtTo.Date &&
                        (y.StockInCard.Type == 1 || y.StockInCard.Type == 2) && (y.StockInCard.Status == 1 || y.StockInCard.Status == 2))
                .Select(x => new { ID = x.GoodID, x.Good.Name, x.Good.InputPrice }).GroupBy(gr => gr.Name);
            
            //var goodManager = new GenericManager<TMT.ERP.DataAccess.Model.Good>();
            //var good = goodManager.Get().Select(x =>
            //{
            //    var stockInCardsDetails = x.StockInCardsDetails
            //        .Where(y => y.StockInCard.ApprovalDate >= dtFrom.Date && y.StockInCard.ApprovalDate <= dtTo.Date &&
            //            (y.StockInCard.Type == 1 || y.StockInCard.Type == 2) && (y.StockInCard.Status == 1 || y.StockInCard.Status == 2))
            //            .ToList();
            //    return new { x.ID, x.Name, x.InputPrice };
            //}).GroupBy(gr => gr.Name);

            if (good != null)
            {
                for (int i = 0; i < good.Count(); i++)
                {
                    if (good.ElementAt(i).Select(x => x.Name).FirstOrDefault() != null)
                    {
                        try
                        {
                            var test = objFin[i];
                        }
                        catch (Exception)
                        {
                            var obj = new Inventory_Item_Summary();
                            objFin.Add(obj);
                        }
                        var objCol = new SummaryForInventoryItem();
                        objFin[i].Item = good.ElementAt(i).Select(x => x.Name).FirstOrDefault();
                        int goodId = good.ElementAt(i).Select(x => x.ID).FirstOrDefault();
                        var dValue = good.ElementAt(i).Select(x => x.InputPrice).FirstOrDefault();
                        GetQty_Price_Incoming(objCol, goodId, dValue, dtFrom, dtTo);
                        objFin[i].MPS_Price = objCol;
                        objCol = null;
                    }
                }
            }
        }
        #endregion Private Incoming

        #region Private SaleInvoice
        #region Private Calculate SaleInvoice
        /// <summary>
        /// Sell - iStatus: 2, iType: 0;
        /// Return - iStatus: 2, iType: 1;
        /// </summary>        
        private int Sell_Return_ItemQty_SaleInvoice(int iStatus, int iType, int GoodId, DateTime dtFrom, DateTime dtTo)
        {
            return new GenericManager<TMT.ERP.DataAccess.Model.SaleInvoiceDetail>().Get().Where(sid => sid.GoodID == GoodId && sid.SaleInvoice.Status == iStatus
                && sid.SaleInvoice.Type == iType && sid.SaleInvoice.ApprovalDate >= dtFrom.Date && sid.SaleInvoice.ApprovalDate <= dtTo.Date).Sum(p => p.Quantity);
        }

        /// <summary>
        /// Sell - iStatus: 2, iType: 0;
        /// Return - iStatus: 2, iType: 1;
        /// </summary> 
        private double? Sell_Return_ItemPrice_SaleInvoice(int iStatus, int iType, int GoodId, DateTime dtFrom, DateTime dtTo)
        {
            var obj = new GenericManager<TMT.ERP.DataAccess.Model.SaleInvoiceDetail>().Get().Where(sid => sid.GoodID == GoodId && sid.SaleInvoice.Status == iStatus
                && sid.SaleInvoice.Type == iType && sid.SaleInvoice.ApprovalDate >= dtFrom.Date && sid.SaleInvoice.ApprovalDate <= dtTo.Date);

            return obj.Sum(p => p.TotalMoney);
        }

        /// <summary>
        /// Get Qty Price for SaleInvoice
        /// </summary> 
        private void GetQty_Price_SaleInvoice(SummaryForInventoryItem obj, int GoodId, double? dValue, DateTime dtFrom, DateTime dtTo)
        {
            obj.Value.Value = dValue;
            obj.Value.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

            var iQty = Sell_Return_ItemQty_SaleInvoice(2, 0, GoodId, dtFrom, dtTo);
            iQty -= Sell_Return_ItemQty_SaleInvoice(2, 1, GoodId, dtFrom, dtTo);
            obj.Qty = iQty;
            obj.s_Qty = SetNegativeNumber(obj.Qty.Value.ToString(FORMATNUMBER_2));

            var dPrice = Sell_Return_ItemPrice_SaleInvoice(2, 0, GoodId, dtFrom, dtTo);
            dPrice -= Sell_Return_ItemPrice_SaleInvoice(2, 1, GoodId, dtFrom, dtTo);
            obj.Total.Value = dPrice;
            obj.Total.Name = SetNegativeNumber(dPrice.Value.ToString(FORMATNUMBER_1));

            obj.Avg.Value = iQty != 0 ? dPrice / iQty : 0;
            obj.Avg.Name = SetNegativeNumber(obj.Avg.Value.Value.ToString(FORMATNUMBER_1));            
        }
        #endregion Private Calculate SaleInvoice

        /// <summary>
        /// Get value Sale Invoice for Inventory Item Summary
        /// </summary>
        private void GetSaleInvoice(List<Inventory_Item_Summary> objFin, DateTime dtFrom, DateTime dtTo, int CompanyID)
        {
            var good = new GenericManager<TMT.ERP.DataAccess.Model.SaleInvoiceDetail>().Get()
                .Where(y => y.SaleInvoice.ApprovalDate >= dtFrom.Date && y.SaleInvoice.ApprovalDate <= dtTo.Date && y.SaleInvoice.Status == 2)
                .Select(x => new { ID = x.GoodID, x.Good.Name, x.Good.OutputPrice }).GroupBy(gr => gr.Name);
            
            //var goodManager = new GenericManager<TMT.ERP.DataAccess.Model.Good>();
            //var good = goodManager.Get().Select(x =>
            //{
            //    var saleInvoiceDetails = x.SaleInvoiceDetails
            //        .Where(y => y.SaleInvoice.ApprovalDate >= dtFrom.Date && y.SaleInvoice.ApprovalDate <= dtTo.Date && y.SaleInvoice.Status == 2).ToList();
            //    return new { x.ID, x.Name, x.OutputPrice };
            //}).GroupBy(gr => gr.Name);

            if (good != null)
            {
                for (int i = 0; i < good.Count(); i++)
                {
                    if (good.ElementAt(i).Select(x => x.Name).FirstOrDefault() != null)
                    {
                        try
                        {
                            var test = objFin[i];
                        }
                        catch (Exception)
                        {
                            var obj = new Inventory_Item_Summary();
                            objFin.Add(obj);
                        }
                        var objCol = new SummaryForInventoryItem();
                        objFin[i].Item = good.ElementAt(i).Select(x => x.Name).FirstOrDefault();
                        int goodId = good.ElementAt(i).Select(x => x.ID).FirstOrDefault();
                        var dValue = good.ElementAt(i).Select(x => x.OutputPrice).FirstOrDefault();
                        GetQty_Price_SaleInvoice(objCol, goodId, dValue, dtFrom, dtTo);
                        objFin[i].Sales_Price = objCol;
                        objCol = null;
                        objCol = new SummaryForInventoryItem();                       
                        GetQty_Price_Net(objCol, goodId, CompanyID, dtFrom, dtTo);
                        objFin[i].Net = objCol;
                        objCol = null;
                    }
                }
            }
        }
        #endregion Private SaleInvoice

        #region Private Net
        private void GetQty_Price_Net(SummaryForInventoryItem obj, int GoodId, int CompanyID, DateTime dtFrom, DateTime dtTo)
        {
            var agis = new GenericManager<TMT.ERP.DataAccess.Model.ActualGoodInStock>().Get().Where(ag => ag.CompanyID == CompanyID && ag.GoodID == GoodId &&
                                            ag.DateIn >= dtFrom.Date && ag.DateIn <= dtTo.Date);
            var iQty = agis.Sum(ag=>ag.InputQuantity - ag.OutputQuantity);
            obj.Qty = iQty;
            obj.s_Qty = SetNegativeNumber(obj.Qty.Value.ToString(FORMATNUMBER_2));

            var dPrice = agis.Sum(ag => (ag.InputQuantity - ag.OutputQuantity) * ag.InputPrice);
            obj.Total.Value = dPrice;
            obj.Total.Name = SetNegativeNumber(dPrice.Value.ToString(FORMATNUMBER_1));
        }
        #endregion Private Net
        #endregion Private Sub Inventory_Item_Summary

        /// <summary>
        /// Get object for inventory item summary
        /// </summary>
        private void GetObjectForInventory_Item_Summary(Inventory_Items_Summary objFin, DateTime dtFrom, DateTime dtTo, int CompanyID)
        {            
            //Purchase
            objFin.Inventory_Item_Summary = GetPurchase(dtFrom, dtTo);

            objFin.Total_Purchases_Price_Qty = objFin.Inventory_Item_Summary.Sum(x => x.Purchases_Price.Qty);
            objFin.s_Total_Purchases_Price_Qty = SetNegativeNumber(objFin.Total_Purchases_Price_Qty.Value.ToString(FORMATNUMBER_2));
            objFin.Total_Purchases_Price_Total = objFin.Inventory_Item_Summary.Sum(x => x.Purchases_Price.Total.Value);
            objFin.s_Total_Purchases_Price_Total = SetNegativeNumber(objFin.Total_Purchases_Price_Total.Value.ToString(FORMATNUMBER_1));

            //Outgoing
            GetOutgoing(objFin.Inventory_Item_Summary, dtFrom, dtTo);
            
            objFin.Total_WO_Price_Qty = objFin.Inventory_Item_Summary.Sum(x => x.WO_Price.Qty);
            objFin.s_Total_WO_Price_Qty = SetNegativeNumber(objFin.Total_WO_Price_Qty.Value.ToString(FORMATNUMBER_2));
            objFin.Total_WO_Price_Total = objFin.Inventory_Item_Summary.Sum(x => x.WO_Price.Total.Value);
            objFin.s_Total_WO_Price_Total = SetNegativeNumber(objFin.Total_WO_Price_Total.Value.ToString(FORMATNUMBER_1));

            //incoming shipment
            GetIncoming(objFin.Inventory_Item_Summary, dtFrom, dtTo);

            objFin.Total_MPS_Price_Qty = objFin.Inventory_Item_Summary.Sum(x => x.MPS_Price.Qty);
            objFin.s_Total_MPS_Price_Qty = SetNegativeNumber(objFin.Total_MPS_Price_Qty.Value.ToString(FORMATNUMBER_2));
            objFin.Total_MPS_Price_Total = objFin.Inventory_Item_Summary.Sum(x => x.MPS_Price.Total.Value);
            objFin.s_Total_MPS_Price_Total = SetNegativeNumber(objFin.Total_MPS_Price_Total.Value.ToString(FORMATNUMBER_1));

            //sale invoice - net
            GetSaleInvoice(objFin.Inventory_Item_Summary, dtFrom, dtTo, CompanyID);
            
            objFin.Total_Sales_Price_Qty = objFin.Inventory_Item_Summary.Sum(x => x.Sales_Price.Qty);
            objFin.s_Total_Sales_Price_Qty = SetNegativeNumber(objFin.Total_Sales_Price_Qty.Value.ToString(FORMATNUMBER_2));
            objFin.Total_Sales_Price_Total = objFin.Inventory_Item_Summary.Sum(x => x.Sales_Price.Total.Value);
            objFin.s_Total_Sales_Price_Total = SetNegativeNumber(objFin.Total_Sales_Price_Total.Value.ToString(FORMATNUMBER_1));

            objFin.Total_Net_Qty = objFin.Inventory_Item_Summary.Sum(x => x.Net.Qty);
            objFin.s_Total_Net_Qty = SetNegativeNumber(objFin.Total_Net_Qty.Value.ToString(FORMATNUMBER_2));
            objFin.Total_Net_Total = objFin.Inventory_Item_Summary.Sum(x => x.Net.Total.Value);
            objFin.s_Total_Net_Total = SetNegativeNumber(objFin.Total_Net_Total.Value.ToString(FORMATNUMBER_1));
        }
        #endregion Private Inventory_Item_Summary

        #region Private Cash_Summary
        private List<I_LOE_PMiE_TM> GetObjectForIncome(DateTime dt)
        {
            List<I_LOE_PMiE_TM> objCol = null;

            var account = new GenericManager<TMT.ERP.DataAccess.Model.Account>().Get().Where(a=>a.AccountType.Type == 1 || a.AccountType.Type == 4);


            if (account != null)
            {
                objCol = new List<I_LOE_PMiE_TM>();
                if (account != null && account.Count() > 0)
                {
                    for (int i = 0; i < account.Count(); i++)
                    {
                        I_LOE_PMiE_TM oObj = new I_LOE_PMiE_TM();
                        oObj.AccountID = account.ElementAt(i).ID;
                        oObj.AccountName = account.ElementAt(i).Name;

                        var dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_01.Value = dValue;
                        oObj.Month_01.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                        
                        var dResult = dValue != 0 ? (dValue.Value / dValue.Value) * 100 : 0;
                        oObj.IncomePer_CurrMonth.Value = dResult;
                        oObj.IncomePer_CurrMonth.Name = dResult.ToString(FORMATNUMBER_2) + "%";

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_02.Value = dValue;
                        oObj.Month_02.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_03.Value = dValue;
                        oObj.Month_03.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_04.Value = dValue;
                        oObj.Month_04.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_05.Value = dValue;
                        oObj.Month_05.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Received - a.Spent);                            
                        oObj.Month_06.Value = dValue;
                        oObj.Month_06.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_07.Value = dValue;
                        oObj.Month_07.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Received - a.Spent);                            
                        oObj.Month_08.Value = dValue;
                        oObj.Month_08.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_09.Value = dValue;
                        oObj.Month_09.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_10.Value = dValue;
                        oObj.Month_10.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_11.Value = dValue;
                        oObj.Month_11.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Received - a.Spent);
                        oObj.Month_12.Value = dValue;
                        oObj.Month_12.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                        
                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt))
                            .Sum(a => a.Received - a.Spent);
                        oObj.YTD.Value = dValue;
                        oObj.YTD.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                        
                        dResult = dValue != 0 ? (dValue.Value / dValue.Value) * 100 : 0;
                        oObj.IncomePer_YTD.Value = dResult;
                        oObj.IncomePer_YTD.Name = dResult.ToString(FORMATNUMBER_2) + "%";

                        objCol.Add(oObj);
                        oObj = null;
                    }
                }
            }            
            return objCol;
        }

        private List<I_LOE_PMiE_TM> GetObjectForOperatingExpense(DateTime dt, double? dCurrMonth, double? dYTD)
        {
            List<I_LOE_PMiE_TM> objCol = null;

            var account = new GenericManager<TMT.ERP.DataAccess.Model.Account>().Get().Where(a => a.AccountType.Type == 0 || a.AccountType.Type == 2);

            if (account != null)
            {
                objCol = new List<I_LOE_PMiE_TM>();
                if (account != null && account.Count() > 0)
                {
                    for (int i = 0; i < account.Count(); i++)
                    {
                        I_LOE_PMiE_TM oObj = new I_LOE_PMiE_TM();
                        oObj.AccountID = account.ElementAt(i).ID;
                        oObj.AccountName = account.ElementAt(i).Name;

                        var dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_01.Value = dValue;
                        oObj.Month_01.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                        
                        var dResult = (dCurrMonth != 0) ? dValue / dCurrMonth : 0;
                        oObj.IncomePer_CurrMonth.Value = dResult;
                        oObj.IncomePer_CurrMonth.Name = dResult.Value.ToString(FORMATNUMBER_2) + "%";

                        dResult = (dYTD != 0) ? dValue / dYTD : 0;
                        oObj.IncomePer_YTD.Value = dResult;
                        oObj.IncomePer_YTD.Name = dResult.Value.ToString(FORMATNUMBER_2) + "%";

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_02.Value = dValue;
                        oObj.Month_02.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_03.Value = dValue;
                        oObj.Month_03.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_04.Value = dValue;
                        oObj.Month_04.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_05.Value = dValue;
                        oObj.Month_05.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_06.Value = dValue;
                        oObj.Month_06.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_07.Value = dValue;
                        oObj.Month_07.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_08.Value = dValue;
                        oObj.Month_08.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_09.Value = dValue;
                        oObj.Month_09.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_10.Value = dValue;
                        oObj.Month_10.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_11.Value = dValue;
                        oObj.Month_11.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Spent - a.Received);
                        oObj.Month_12.Value = dValue;
                        oObj.Month_12.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(
                                x => x.AccountID == account.ElementAt(i).ID && (x.Type == 1 || x.Type == 0) &&
                                    x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt))
                            .Sum(a => a.Spent - a.Received);
                        oObj.YTD.Value = dValue;
                        oObj.YTD.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        objCol.Add(oObj);
                        oObj = null;
                    }
                }
            }           
            return objCol;
        }

        private List<I_LOE_PMiE_TM> GetObjectForTaxMovements(DateTime dt, double? dCurrMonth, double? dYTD)
        {
            List<I_LOE_PMiE_TM> objCol = null;

            var account = new GenericManager<TMT.ERP.DataAccess.Model.Account>().Get().Where(a => a.ID == 49);

            if (account != null)
            {
                objCol = new List<I_LOE_PMiE_TM>();
                if (account != null && account.Count() > 0)
                {
                    for (int i = 0; i < account.Count(); i++)
                    {
                        I_LOE_PMiE_TM oObj = new I_LOE_PMiE_TM();

                        oObj.AccountID = 1;
                        oObj.AccountName = "Tax Inputs";

                        var dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Received);
                        oObj.Month_01.Value = dValue;
                        oObj.Month_01.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Received);
                        oObj.Month_02.Value = dValue;
                        oObj.Month_02.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Received);
                        oObj.Month_03.Value = dValue;
                        oObj.Month_03.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Received);
                        oObj.Month_04.Value = dValue;
                        oObj.Month_04.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Received);
                        oObj.Month_05.Value = dValue;
                        oObj.Month_05.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Received);
                        oObj.Month_06.Value = dValue;
                        oObj.Month_06.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Received);
                        oObj.Month_07.Value = dValue;
                        oObj.Month_07.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Received);
                        oObj.Month_08.Value = dValue;
                        oObj.Month_08.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Received);
                        oObj.Month_09.Value = dValue;
                        oObj.Month_09.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Received);
                        oObj.Month_10.Value = dValue;
                        oObj.Month_10.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Received);
                        oObj.Month_11.Value = dValue;
                        oObj.Month_11.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Received);
                        oObj.Month_12.Value = dValue;
                        oObj.Month_12.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt.AddYears(0)))
                                .Sum(a => a.Received);
                        oObj.YTD.Value = dValue;
                        oObj.YTD.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                        
                        objCol.Add(oObj);
                        oObj = null;

                        oObj = new I_LOE_PMiE_TM();

                        oObj.AccountID = 2;
                        oObj.AccountName = "Tax Outputs";

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Spent);
                        oObj.Month_01.Value = dValue;
                        oObj.Month_01.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Spent);
                        oObj.Month_02.Value = dValue;
                        oObj.Month_02.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Spent);
                        oObj.Month_03.Value = dValue;
                        oObj.Month_03.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Spent);
                        oObj.Month_04.Value = dValue;
                        oObj.Month_04.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Spent);
                        oObj.Month_05.Value = dValue;
                        oObj.Month_05.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Spent);
                        oObj.Month_06.Value = dValue;
                        oObj.Month_06.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Spent);
                        oObj.Month_07.Value = dValue;
                        oObj.Month_07.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Spent);
                        oObj.Month_08.Value = dValue;
                        oObj.Month_08.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Spent);
                        oObj.Month_09.Value = dValue;
                        oObj.Month_09.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Spent);
                        oObj.Month_10.Value = dValue;
                        oObj.Month_10.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Spent);
                        oObj.Month_11.Value = dValue;
                        oObj.Month_11.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Spent);
                        oObj.Month_12.Value = dValue;
                        oObj.Month_12.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = account.ElementAt(i).AccountTrans.Where(x => x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt.AddYears(0)))
                                .Sum(a => a.Spent);
                        oObj.YTD.Value = dValue;
                        oObj.YTD.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        objCol.Add(oObj);
                        oObj = null;
                    }
                }
            }           
            return objCol;
        }

        private List<I_LOE_PMiE_TM> GetObjectForTaxAccounts(DateTime dt)
        {
            List<I_LOE_PMiE_TM> objCol = null;

            var val = new GenericManager<TMT.ERP.DataAccess.Model.BankAccount>().Get();

            if (val != null)
            {
                objCol = new List<I_LOE_PMiE_TM>();
                if (val != null && val.Count() > 0)
                {
                    for (int i = 0; i < val.Count(); i++)
                    {
                        I_LOE_PMiE_TM oObj = new I_LOE_PMiE_TM();
                        oObj.AccountID = val.ElementAt(i).ID;
                        oObj.AccountName = val.ElementAt(i).BankName;

                        var dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                            x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt) && x.Date >= GetFirstDayOfMonth(dt))
                            .Sum(a => a.Spent);
                        oObj.Month_01.Value = dValue;
                        oObj.Month_01.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-1)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-1)))
                            .Sum(a => a.Spent);
                        oObj.Month_02.Value = dValue;
                        oObj.Month_02.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-2)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-2)))
                            .Sum(a => a.Spent);
                        oObj.Month_03.Value = dValue;
                        oObj.Month_03.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-3)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-3)))
                            .Sum(a => a.Spent);
                        oObj.Month_04.Value = dValue;
                        oObj.Month_04.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-4)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-4)))
                            .Sum(a => a.Spent);
                        oObj.Month_05.Value = dValue;
                        oObj.Month_05.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-5)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-5)))
                            .Sum(a => a.Spent);
                        oObj.Month_06.Value = dValue;
                        oObj.Month_06.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-6)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-6)))
                            .Sum(a => a.Spent);
                        oObj.Month_07.Value = dValue;
                        oObj.Month_07.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-7)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-7)))
                            .Sum(a => a.Spent);
                        oObj.Month_08.Value = dValue;
                        oObj.Month_08.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-8)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-8)))
                            .Sum(a => a.Spent);
                        oObj.Month_09.Value = dValue;
                        oObj.Month_09.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-9)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-9)))
                            .Sum(a => a.Spent);
                        oObj.Month_10.Value = dValue;
                        oObj.Month_10.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-10)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-10)))
                            .Sum(a => a.Spent);
                        oObj.Month_11.Value = dValue;
                        oObj.Month_11.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date <= GetLastDayOfMonth(dt.AddMonths(-11)) && x.Date >= GetFirstDayOfMonth(dt.AddMonths(-11)))
                            .Sum(a => a.Spent);
                        oObj.Month_12.Value = dValue;
                        oObj.Month_12.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        dValue = val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 &&
                                x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt.AddYears(0)))
                            .Sum(a => a.Received);
                        dValue -= val.ElementAt(i).AccountTrans.Where(
                            x => x.BankAccountID == oObj.AccountID && x.Type == 0 && x.AccountType == 0 && 
                                x.Date > GetLastDayOfYear(dt.AddYears(-1)) && x.Date <= GetLastDayOfYear(dt.AddYears(0)))
                            .Sum(a => a.Spent);
                        oObj.YTD.Value = dValue;
                        oObj.YTD.Name = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));

                        objCol.Add(oObj);
                        oObj = null;
                    }                    
                }
            }
            return objCol;
        }

        private void SetValueResult_Income(Cash_Summary cs)
        {
            cs.Set_Income.Total_Month_01.Value = cs.Income.Sum(x => x.Month_01.Value);
            cs.Set_Income.Total_Month_01.Name = SetNegativeNumber(cs.Set_Income.Total_Month_01.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_02.Value = cs.Income.Sum(x => x.Month_02.Value);
            cs.Set_Income.Total_Month_02.Name = SetNegativeNumber(cs.Set_Income.Total_Month_02.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_03.Value = cs.Income.Sum(x => x.Month_03.Value);
            cs.Set_Income.Total_Month_03.Name = SetNegativeNumber(cs.Set_Income.Total_Month_03.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_04.Value = cs.Income.Sum(x => x.Month_04.Value);
            cs.Set_Income.Total_Month_04.Name = SetNegativeNumber(cs.Set_Income.Total_Month_04.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_05.Value = cs.Income.Sum(x => x.Month_05.Value);
            cs.Set_Income.Total_Month_05.Name = SetNegativeNumber(cs.Set_Income.Total_Month_05.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_06.Value = cs.Income.Sum(x => x.Month_06.Value);
            cs.Set_Income.Total_Month_06.Name = SetNegativeNumber(cs.Set_Income.Total_Month_06.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_07.Value = cs.Income.Sum(x => x.Month_07.Value);
            cs.Set_Income.Total_Month_07.Name = SetNegativeNumber(cs.Set_Income.Total_Month_07.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_08.Value = cs.Income.Sum(x => x.Month_08.Value);
            cs.Set_Income.Total_Month_08.Name = SetNegativeNumber(cs.Set_Income.Total_Month_08.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_09.Value = cs.Income.Sum(x => x.Month_09.Value);
            cs.Set_Income.Total_Month_09.Name = SetNegativeNumber(cs.Set_Income.Total_Month_09.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_10.Value = cs.Income.Sum(x => x.Month_10.Value);
            cs.Set_Income.Total_Month_10.Name = SetNegativeNumber(cs.Set_Income.Total_Month_10.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_11.Value = cs.Income.Sum(x => x.Month_11.Value);
            cs.Set_Income.Total_Month_11.Name = SetNegativeNumber(cs.Set_Income.Total_Month_11.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_Month_12.Value = cs.Income.Sum(x => x.Month_12.Value);
            cs.Set_Income.Total_Month_12.Name = SetNegativeNumber(cs.Set_Income.Total_Month_12.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_Income.Total_YTD.Value = cs.Income.Sum(x => x.YTD.Value);
            cs.Set_Income.Total_YTD.Name = SetNegativeNumber(cs.Set_Income.Total_YTD.Value.Value.ToString(FORMATNUMBER_1));

            for (int i = 0; i < cs.Income.Count; i++)
            {
                var dValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Income[i].Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
                cs.Income[i].IncomePer_CurrMonth.Value = dValue;
                cs.Income[i].IncomePer_CurrMonth.Name = dValue.Value.ToString(FORMATNUMBER_2) + "%";

                dValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Income[i].YTD.Value / cs.Set_Income.Total_YTD.Value) * 100 : 0;
                cs.Income[i].IncomePer_YTD.Value = dValue;
                cs.Income[i].IncomePer_YTD.Name = dValue.Value.ToString(FORMATNUMBER_2) + "%";
            }

            cs.Set_Income.Total_IncomePer_CurrMonth.Value = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Set_Income.Total_Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_Income.Total_IncomePer_CurrMonth.Name = cs.Set_Income.Total_IncomePer_CurrMonth.Value.Value.ToString(FORMATNUMBER_2) + "%";
            cs.Set_Income.Total_IncomePer_YTD.Value = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Set_Income.Total_YTD.Value / cs.Set_Income.Total_YTD.Value) * 100 : 0;
            cs.Set_Income.Total_IncomePer_YTD.Name = cs.Set_Income.Total_IncomePer_YTD.Value.Value.ToString(FORMATNUMBER_2) + "%";
        }

        private void SetValueResult_OperatingExpense(Cash_Summary cs)
        {
            cs.Set_OperatingExpenses.Total_Month_01.Value = cs.OperatingExpenses.Sum(x => x.Month_01.Value);
            cs.Set_OperatingExpenses.Total_Month_01.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_01.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_02.Value = cs.OperatingExpenses.Sum(x => x.Month_02.Value);
            cs.Set_OperatingExpenses.Total_Month_02.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_02.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_03.Value = cs.OperatingExpenses.Sum(x => x.Month_03.Value);
            cs.Set_OperatingExpenses.Total_Month_03.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_03.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_04.Value = cs.OperatingExpenses.Sum(x => x.Month_04.Value);
            cs.Set_OperatingExpenses.Total_Month_04.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_04.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_05.Value = cs.OperatingExpenses.Sum(x => x.Month_05.Value);
            cs.Set_OperatingExpenses.Total_Month_05.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_05.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_06.Value = cs.OperatingExpenses.Sum(x => x.Month_06.Value);
            cs.Set_OperatingExpenses.Total_Month_06.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_06.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_07.Value = cs.OperatingExpenses.Sum(x => x.Month_07.Value);
            cs.Set_OperatingExpenses.Total_Month_07.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_07.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_08.Value = cs.OperatingExpenses.Sum(x => x.Month_08.Value);
            cs.Set_OperatingExpenses.Total_Month_08.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_08.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_09.Value = cs.OperatingExpenses.Sum(x => x.Month_09.Value);
            cs.Set_OperatingExpenses.Total_Month_09.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_09.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_10.Value = cs.OperatingExpenses.Sum(x => x.Month_10.Value);
            cs.Set_OperatingExpenses.Total_Month_10.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_10.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_11.Value = cs.OperatingExpenses.Sum(x => x.Month_11.Value);
            cs.Set_OperatingExpenses.Total_Month_11.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_11.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_Month_12.Value = cs.OperatingExpenses.Sum(x => x.Month_12.Value);
            cs.Set_OperatingExpenses.Total_Month_12.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_Month_12.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_OperatingExpenses.Total_YTD.Value = cs.OperatingExpenses.Sum(x => x.YTD.Value);
            cs.Set_OperatingExpenses.Total_YTD.Name = SetNegativeNumber(cs.Set_OperatingExpenses.Total_YTD.Value.Value.ToString(FORMATNUMBER_1));

            for (int i = 0; i < cs.OperatingExpenses.Count; i++)
            {
                var dValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.OperatingExpenses[i].Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
                cs.OperatingExpenses[i].IncomePer_CurrMonth.Value = dValue;
                cs.OperatingExpenses[i].IncomePer_CurrMonth.Name = dValue.Value.ToString(FORMATNUMBER_2) + "%";

                dValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.OperatingExpenses[i].YTD.Value / cs.Set_Income.Total_YTD.Value) * 100 : 0;
                cs.OperatingExpenses[i].IncomePer_YTD.Value = dValue;
                cs.OperatingExpenses[i].IncomePer_YTD.Name = dValue.Value.ToString(FORMATNUMBER_2) + "%";
            }
            var vValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Set_OperatingExpenses.Total_Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_OperatingExpenses.Total_IncomePer_CurrMonth.Value = vValue;
            cs.Set_OperatingExpenses.Total_IncomePer_CurrMonth.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
            vValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Set_OperatingExpenses.Total_YTD.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_OperatingExpenses.Total_IncomePer_YTD.Value = vValue;
            cs.Set_OperatingExpenses.Total_IncomePer_YTD.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
        }

        private void SetValueResult_OperatingSurplus(Cash_Summary cs)
        {
            var vValue = cs.Set_Income.Total_Month_01.Value - cs.Set_OperatingExpenses.Total_Month_01.Value;
            cs.Set_OperatingSurPlus.Total_Month_01.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_01.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_02.Value - cs.Set_OperatingExpenses.Total_Month_02.Value;
            cs.Set_OperatingSurPlus.Total_Month_02.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_02.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_03.Value - cs.Set_OperatingExpenses.Total_Month_03.Value;
            cs.Set_OperatingSurPlus.Total_Month_03.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_03.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_04.Value - cs.Set_OperatingExpenses.Total_Month_04.Value;
            cs.Set_OperatingSurPlus.Total_Month_04.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_04.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_05.Value - cs.Set_OperatingExpenses.Total_Month_05.Value;
            cs.Set_OperatingSurPlus.Total_Month_05.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_05.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_06.Value - cs.Set_OperatingExpenses.Total_Month_06.Value;
            cs.Set_OperatingSurPlus.Total_Month_06.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_06.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_07.Value - cs.Set_OperatingExpenses.Total_Month_07.Value;
            cs.Set_OperatingSurPlus.Total_Month_07.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_07.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_08.Value - cs.Set_OperatingExpenses.Total_Month_08.Value;
            cs.Set_OperatingSurPlus.Total_Month_08.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_08.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_09.Value - cs.Set_OperatingExpenses.Total_Month_09.Value;
            cs.Set_OperatingSurPlus.Total_Month_09.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_09.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_10.Value - cs.Set_OperatingExpenses.Total_Month_10.Value;
            cs.Set_OperatingSurPlus.Total_Month_10.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_10.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_11.Value - cs.Set_OperatingExpenses.Total_Month_11.Value;
            cs.Set_OperatingSurPlus.Total_Month_11.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_11.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_12.Value - cs.Set_OperatingExpenses.Total_Month_12.Value;
            cs.Set_OperatingSurPlus.Total_Month_12.Value = vValue;
            cs.Set_OperatingSurPlus.Total_Month_12.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_YTD.Value - cs.Set_OperatingExpenses.Total_YTD.Value;
            cs.Set_OperatingSurPlus.Total_YTD.Value = vValue;
            cs.Set_OperatingSurPlus.Total_YTD.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Set_OperatingSurPlus.Total_Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_OperatingSurPlus.Total_IncomePer_CurrMonth.Value = vValue;
            cs.Set_OperatingSurPlus.Total_IncomePer_CurrMonth.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";

            vValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Set_OperatingSurPlus.Total_YTD.Value / cs.Set_Income.Total_YTD.Value) * 100 : 0;
            cs.Set_OperatingSurPlus.Total_IncomePer_YTD.Value = vValue;
            cs.Set_OperatingSurPlus.Total_IncomePer_YTD.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
        }

        private void SetValueResult_TaxMovements(Cash_Summary cs)
        {
            cs.Set_TaxMovements.Total_Month_01.Value = cs.TaxMovements.Sum(x => x.Month_01.Value);
            cs.Set_TaxMovements.Total_Month_01.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_01.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_02.Value = cs.TaxMovements.Sum(x => x.Month_02.Value);
            cs.Set_TaxMovements.Total_Month_02.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_02.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_03.Value = cs.TaxMovements.Sum(x => x.Month_03.Value);
            cs.Set_TaxMovements.Total_Month_03.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_03.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_04.Value = cs.TaxMovements.Sum(x => x.Month_04.Value);
            cs.Set_TaxMovements.Total_Month_04.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_04.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_05.Value = cs.TaxMovements.Sum(x => x.Month_05.Value);
            cs.Set_TaxMovements.Total_Month_05.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_05.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_06.Value = cs.TaxMovements.Sum(x => x.Month_06.Value);
            cs.Set_TaxMovements.Total_Month_06.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_06.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_07.Value = cs.TaxMovements.Sum(x => x.Month_07.Value);
            cs.Set_TaxMovements.Total_Month_07.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_07.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_08.Value = cs.TaxMovements.Sum(x => x.Month_08.Value);
            cs.Set_TaxMovements.Total_Month_08.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_08.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_09.Value = cs.TaxMovements.Sum(x => x.Month_09.Value);
            cs.Set_TaxMovements.Total_Month_09.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_09.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_10.Value = cs.TaxMovements.Sum(x => x.Month_10.Value);
            cs.Set_TaxMovements.Total_Month_10.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_10.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_11.Value = cs.TaxMovements.Sum(x => x.Month_11.Value);
            cs.Set_TaxMovements.Total_Month_11.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_11.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_Month_12.Value = cs.TaxMovements.Sum(x => x.Month_12.Value);
            cs.Set_TaxMovements.Total_Month_12.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_Month_12.Value.Value.ToString(FORMATNUMBER_1));
            cs.Set_TaxMovements.Total_YTD.Value = cs.TaxMovements.Sum(x => x.YTD.Value);
            cs.Set_TaxMovements.Total_YTD.Name = SetNegativeNumber(cs.Set_TaxMovements.Total_YTD.Value.Value.ToString(FORMATNUMBER_1));

            var vValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Set_TaxMovements.Total_Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_TaxMovements.Total_IncomePer_CurrMonth.Value = vValue;
            cs.Set_TaxMovements.Total_IncomePer_CurrMonth.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
            vValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Set_TaxMovements.Total_YTD.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_TaxMovements.Total_IncomePer_YTD.Value = vValue;
            cs.Set_TaxMovements.Total_IncomePer_YTD.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
        }

        private void SetValueResult_NetCashMovement(Cash_Summary cs)
        {
            var vValue = cs.Set_OperatingSurPlus.Total_Month_01.Value + cs.Set_TaxMovements.Total_Month_01.Value;//+
            cs.Set_NetCashMovement.Total_Month_01.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_01.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_02.Value + cs.Set_TaxMovements.Total_Month_02.Value;//+
            cs.Set_NetCashMovement.Total_Month_02.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_02.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_03.Value + cs.Set_TaxMovements.Total_Month_03.Value;//+
            cs.Set_NetCashMovement.Total_Month_03.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_03.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_04.Value + cs.Set_TaxMovements.Total_Month_04.Value;//+
            cs.Set_NetCashMovement.Total_Month_04.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_04.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_05.Value + cs.Set_TaxMovements.Total_Month_05.Value;//+
            cs.Set_NetCashMovement.Total_Month_05.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_05.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_06.Value + cs.Set_TaxMovements.Total_Month_06.Value;//+
            cs.Set_NetCashMovement.Total_Month_06.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_06.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_07.Value + cs.Set_TaxMovements.Total_Month_07.Value;//+
            cs.Set_NetCashMovement.Total_Month_07.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_07.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_08.Value + cs.Set_TaxMovements.Total_Month_08.Value;//+
            cs.Set_NetCashMovement.Total_Month_08.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_08.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_09.Value + cs.Set_TaxMovements.Total_Month_09.Value;//+
            cs.Set_NetCashMovement.Total_Month_09.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_09.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_10.Value + cs.Set_TaxMovements.Total_Month_10.Value;//+
            cs.Set_NetCashMovement.Total_Month_10.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_10.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_11.Value + cs.Set_TaxMovements.Total_Month_11.Value;//+
            cs.Set_NetCashMovement.Total_Month_11.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_11.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_Month_12.Value + cs.Set_TaxMovements.Total_Month_12.Value;//+
            cs.Set_NetCashMovement.Total_Month_12.Value = vValue;
            cs.Set_NetCashMovement.Total_Month_12.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_OperatingSurPlus.Total_YTD.Value + cs.Set_TaxMovements.Total_YTD.Value;//+
            cs.Set_NetCashMovement.Total_YTD.Value = vValue;
            cs.Set_NetCashMovement.Total_YTD.Name = SetNegativeNumber(vValue.Value.ToString(FORMATNUMBER_1));

            vValue = cs.Set_Income.Total_Month_01.Value != 0 ? (cs.Set_NetCashMovement.Total_Month_01.Value / cs.Set_Income.Total_Month_01.Value) * 100 : 0;
            cs.Set_NetCashMovement.Total_IncomePer_CurrMonth.Value = vValue;
            cs.Set_NetCashMovement.Total_IncomePer_CurrMonth.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
            vValue = cs.Set_Income.Total_YTD.Value != 0 ? (cs.Set_NetCashMovement.Total_YTD.Value / cs.Set_Income.Total_YTD.Value) * 100 : 0;
            cs.Set_NetCashMovement.Total_IncomePer_YTD.Value = vValue;
            cs.Set_NetCashMovement.Total_IncomePer_YTD.Name = vValue.Value.ToString(FORMATNUMBER_2) + "%";
        }

        private void SetValueResult_OpeningBalance(Cash_Summary cs)
        {

        }

        private void SetValueResult_PlusNetCashMovement(Cash_Summary cs)
        {
            cs.Set_PlusNetCashMovement = cs.Set_NetCashMovement;
        }

        private void SetValueResult_ClosingBalance(Cash_Summary cs)
        {

        }

        private void SetValueResult(Cash_Summary cs)
        {
            SetValueResult_Income(cs);
            SetValueResult_OperatingExpense(cs);
            SetValueResult_OperatingSurplus(cs);
            SetValueResult_TaxMovements(cs);
            SetValueResult_NetCashMovement(cs);
            SetValueResult_PlusNetCashMovement(cs);            
        }
        #endregion Private Cash_Summary

        #region Private Customer_Invoice & Sale_Invoice
        private List<CustomerInvoice_SupplierInvoice> GetCustomerInvoice(DateTime dtFrom, DateTime dtTo)
        {
            List<CustomerInvoice_SupplierInvoice> objFin = new List<CustomerInvoice_SupplierInvoice>();

            var sale = new GenericManager<TMT.ERP.DataAccess.Model.SaleInvoice>().Get()
                .Where(p => p.ApprovalDate >= dtFrom.Date &&
                    p.ApprovalDate <= dtTo.Date && p.Type == 0 || p.Type == 1);

            if (sale != null)
            {
                objFin = new List<CustomerInvoice_SupplierInvoice>();
                for (int i = 0; i < sale.Count(); i++)
                {
                    var payment = sale.ElementAt(i).Payments.Where(x => x.SaleInvoice.Status == 3).FirstOrDefault();
                    var obj = new CustomerInvoice_SupplierInvoice();
                    obj.ID = sale.ElementAt(i).ID;
                    obj.Invoice_Number = sale.ElementAt(i).Code;
                    obj.Reference = sale.ElementAt(i).Reference;
                    obj.Type = sale.ElementAt(i).Type;
                    obj.ContactName = sale.ElementAt(i).ContactName;
                    obj.Date = GetDateTime(sale.ElementAt(i).ApprovalDate, FORMATDATETIME_6);
                    obj.Due_Date = GetDateTime(sale.ElementAt(i).DueDate, FORMATDATETIME_6);
                    obj.Paid_Date = payment != null ? payment.DatePaid.ToString(FORMATDATETIME_6): string.Empty;
                    double? dValue = sale.ElementAt(i).TotalMoney.Value;
                    obj.Invoice_Total = sale.ElementAt(i).TotalMoney;
                    obj.s_Invoice_Total = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    dValue = payment != null ? payment.TotalMoney : 0;
                    obj.Paid = dValue;
                    obj.s_Paid = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    dValue = sale.ElementAt(i).RemainMoney;
                    obj.Due = dValue;
                    obj.s_Due = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    obj.Status = sale.ElementAt(i).Status;
                    objFin.Add(obj);
                    obj = null;
                }
            }
            return objFin;
        }

        private List<CustomerInvoice_SupplierInvoice> GetSupplierInvoice(DateTime dtFrom, DateTime dtTo)
        {
            List<CustomerInvoice_SupplierInvoice> objFin = new List<CustomerInvoice_SupplierInvoice>();

            var sale = new GenericManager<TMT.ERP.DataAccess.Model.Purchase>().Get()
                .Where(p => p.ApprovalDate >= dtFrom.Date &&
                    p.ApprovalDate <= dtTo.Date && p.Type == 0 || p.Type == 1);

            if (sale != null)
            {
                objFin = new List<CustomerInvoice_SupplierInvoice>();
                for (int i = 0; i < sale.Count(); i++)
                {
                    var payment = sale.ElementAt(i).Payments.Where(x => x.Purchase.Status == 3).FirstOrDefault();
                    var obj = new CustomerInvoice_SupplierInvoice();
                    obj.ID = sale.ElementAt(i).ID;
                    obj.Invoice_Number = sale.ElementAt(i).Code;
                    obj.Reference = sale.ElementAt(i).Reference;
                    obj.Type = sale.ElementAt(i).Type;
                    obj.ContactName = sale.ElementAt(i).ContactName;
                    obj.Date = GetDateTime(sale.ElementAt(i).ApprovalDate,FORMATDATETIME_6);
                    obj.Due_Date = GetDateTime(sale.ElementAt(i).DueDate,FORMATDATETIME_6);
                    obj.Paid_Date = payment != null ? payment.DatePaid.ToString(FORMATDATETIME_6) : string.Empty;
                    double? dValue = GetNumber(sale.ElementAt(i).TotalMoney);
                    obj.Invoice_Total = GetNumber(sale.ElementAt(i).TotalMoney);
                    obj.s_Invoice_Total = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    dValue = payment != null ? GetNumber(payment.TotalMoney) : 0;                    
                    obj.Paid = dValue;
                    obj.s_Paid = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    dValue = GetNumber(sale.ElementAt(i).RemainMoney);
                    obj.Due = dValue;
                    obj.s_Due = SetNegativeNumber(dValue.Value.ToString(FORMATNUMBER_1));
                    obj.Status = sale.ElementAt(i).Status;
                    objFin.Add(obj);
                    obj = null;
                }
            }
            return objFin;
        }
        #endregion Private Customer_Invoice & Sale_Invoice

        /// <summary>
        /// FileName .xlsx, byte ExcelPackage
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="pckArr"></param>
        private void Export_Report(string FileName, byte[] pckArr)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + FileName);
            Response.BinaryWrite(pckArr);
            Response.End();
        }

        private string GetDateTime(DateTime? dt, string format)
        {
            return dt.HasValue ? dt.Value.ToString(format) : "";
        }

        private double? GetNumber(double? dValue)
        {
            return dValue.HasValue ? dValue.Value : 0;
        }

        #endregion Private Function
        
        public ActionResult ExportReport(int iType)
        {
            byte[] pck = null;
            string fileSave = string.Empty;
            
            if (Obj != null)
            {
                switch (iType)
                {
                    case 1:
                        var obj_1 = (Profit_Loss)Obj;

                        pck = obj_1.ExcelPackageProcess(obj_1.ReportDate, obj_1.WorkSheetName, obj_1.TitleDocument, obj_1.CompanyName, obj_1.ColumnName,
                                                    obj_1.Title_Income, obj_1.Title_TotalIncome, obj_1.Income, obj_1.Title_GrossProfit,
                                                    obj_1.Title_OperatingExpenses, obj_1.Title_TotalOperatingExpenses, obj_1.OperatingExpenses, obj_1.Title_NetProfit);
                        fileSave = obj_1.ReportSaveAs;
                        break;
                    case 2:
                        var obj_2 = (Inventory_Items_Summary)Obj;
                        pck = obj_2.ExcelPackageProcess(obj_2.ReportDate, obj_2.WorkSheetName, obj_2.TitleDocument, obj_2.CompanyName, obj_2.ColumnName, obj_2.label_Total,obj_2.Inventory_Item_Summary);
                        fileSave = obj_2.ReportSaveAs;
                        break;

                    case 3:
                        var obj_3 = (Cash_Summary)Obj;
                        pck = obj_3.ExcelPackageProcess(obj_3);
                        fileSave = obj_3.ReportSaveAs;
                        break;

                    case 4:
                        var obj_4 = (CustomerInvoice_SupplierInvoice_Report)Obj;
                        pck = obj_4.ExcelPackageProcess(obj_4);
                        fileSave = obj_4.ReportSaveAs;
                        break;

                    case 5:
                        var obj_5 = (CustomerInvoice_SupplierInvoice_Report)Obj;
                        pck = obj_5.ExcelPackageProcess(obj_5);
                        fileSave = obj_5.ReportSaveAs;
                        break;
                }
                
            }
            
            Export_Report(fileSave, pck);
            
            return View();
        }        
    }
}