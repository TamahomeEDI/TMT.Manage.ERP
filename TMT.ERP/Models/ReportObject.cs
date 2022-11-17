using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public abstract class ReportObject
    {
        public string SERVER_MAPPATH { get; set; }
        public string ReportFileName { get; set; }
        public string FORMATNUMBEREXPORT_1 { get; set; }
        public string FORMATNUMBEREXPORT_2 { get; set; }
        public string FORMATNUMBEREXPORT_3 { get; set; }
        public string ReportSaveAs { get; set; }
        public int ReportType { get; set; }
        public string WorkSheetName { get; set; }
        public string TitleDocument { get; set; }
        public string ExtentTitleDocument { get; set; }
        public string CompanyName { get; set; }
        public string ReportDate { get; set; }
        public List<string> ColumnName { get; set; }

        public DateTime GetFirstDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1).Date;
        }

        public DateTime GetLastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month)).Date;
        }

        /// <summary>
        /// iType: 0 - Profit_Loss;
        /// iType: 1 - Inventory_Items_Summary;
        /// iType: 2 - Cash Summary;
        /// </summary>
        /// <param name="iType"></param>
        /// <param name="iStep"></param>
        /// <param name="dtColumn"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> GetColumnName(int iType, int iStep, DateTime dtColumn, params object[] list)
        {
            List<string> oMonth = new List<string>();            
            if (iStep != 0)
            {
                var FormatDate = (string)list[0];
                bool bFlag = false;
                for (int i = 0; i > iStep; i--)
                {
                    if ((i == -1) && (iType == 2) && !bFlag) 
                    {
                        oMonth.Add((string)list[2]);
                        i = 0;
                        bFlag = true;
                    }
                    else
                    {
                        oMonth.Add(dtColumn.AddMonths(i).Date.ToString(FormatDate));
                    }                    
                }
            }
            if (list != null)
            {
                int j = 1;
                if (iStep == 0)
                {
                    j = 0;
                }
                for (int i = j; i < list.Length; i++)
                {
                    oMonth.Add(list[i].ToString());
                }
            }
            return oMonth;
        }

        /// <summary>
        /// Set Column for work sheet
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="obj"></param>
        public void SetColumnWorkSheet(ExcelWorksheet ws, object obj, int iRow, int iCol)
        {
            if (obj != null)
            {
                var lst = (List<string>)obj;
                for (int i = 0; i <= lst.Count - 1; i++)
                {
                    ws.Cells[iRow, iCol + i].Value = lst[i];
                }
            }
        }
    }

    #region public class process Profit Loss
    public class Income_LOE
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public int AccountType { get; set; }
        public double? TotalAccountTran_1 { get; set; }
        public double? TotalAccountTran_2 { get; set; }
        public double? TotalAccountTran_3 { get; set; }
        public double? TotalAccountTran_4 { get; set; }
        public double? TotalAccountTran_YTD { get; set; }

        public string s_TotalAccountTran_1 { get; set; }
        public string s_TotalAccountTran_2 { get; set; }
        public string s_TotalAccountTran_3 { get; set; }
        public string s_TotalAccountTran_4 { get; set; }
        public string s_TotalAccountTran_YTD { get; set; }
    }

    public class Profit_Loss : ReportObject
    {
        public List<Income_LOE> Income { get; set; }
        public List<Income_LOE> OperatingExpenses { get; set; }

        public string Title_Income { get; set; }
        public string Title_TotalIncome { get; set; }
        public string Title_GrossProfit { get; set; }
        public string Title_OperatingExpenses { get; set; }
        public string Title_TotalOperatingExpenses { get; set; }
        public string Title_NetProfit { get; set; }

        public string s_TotalIncome_1 { get; set; }
        public string s_TotalIncome_2 { get; set; }
        public string s_TotalIncome_3 { get; set; }
        public string s_TotalIncome_4 { get; set; }
        public string s_TotalIncome_YTD { get; set; }

        public string s_TotalOperatingExpenses_1 { get; set; }
        public string s_TotalOperatingExpenses_2 { get; set; }
        public string s_TotalOperatingExpenses_3 { get; set; }
        public string s_TotalOperatingExpenses_4 { get; set; }
        public string s_TotalOperatingExpenses_YTD { get; set; }

        public string s_NetProfit_1 { get; set; }
        public string s_NetProfit_2 { get; set; }
        public string s_NetProfit_3 { get; set; }
        public string s_NetProfit_4 { get; set; }
        public string s_NetProfit_YTD { get; set; }

        /// <summary>
        /// list[0]: SERVER_MAPPATH;
        /// list[1]: ReportFileName;
        /// list[2]: FORMATNUMBEREXPORT;
        /// list[3]: WorkSheetName;
        /// list[4]: TitleDocument;
        /// list[5]: Title_Income;
        /// list[6]: Title_TotalIncome;
        /// list[7]: Title_GrossProfit;
        /// list[8]: Title_OperatingExpenses;
        /// list[9]: Title_TotalOperatingExpenses;
        /// list[10]: Title_NetProfit;
        /// list[11]: date time report;
        /// list[12]: FORMATDATETIME_1;
        /// list[13]: added string;
        /// list[14]: CompanyName;
        /// list[15]: ReportName;
        /// list[16]: PrintDate;
        /// list[17]: date column;
        /// list[18]: column added;
        /// </summary>
        public Profit_Loss SetPropertyAll(params object[] list)
        {
            var obj = new Profit_Loss();
            obj.SERVER_MAPPATH = (string)list[0];
            obj.ReportFileName = (string)list[1];
            obj.FORMATNUMBEREXPORT_1 = (string)list[2];
            obj.WorkSheetName = (string)list[3];
            obj.TitleDocument = (string)list[4];
            obj.Title_Income = (string)list[5];
            obj.Title_TotalIncome = (string)list[6];
            obj.Title_GrossProfit = (string)list[7];
            obj.Title_OperatingExpenses = (string)list[8];
            obj.Title_TotalOperatingExpenses = (string)list[9];
            obj.Title_NetProfit = (string)list[10];
            DateTime dt = (DateTime)list[11];
            obj.ReportDate = GetDateMonthReport((string)list[12], (string)list[13]);
            obj.CompanyName = (string)list[14];

            obj.ReportSaveAs = obj.CompanyName + " - " + (string)list[15] + DateTime.Now.ToString((string)list[16]) + ".xlsx";
            obj.ColumnName = GetColumnName(0, -4, DateTime.Now, (string)list[17], (string)list[18]);
            return obj;
        }

        /// <summary>
        /// list[0]: report date -
        /// list[1]: name worksheet -
        /// list[2]: title document -
        /// list[3]: company name -
        /// list[4]: column
        /// list[5..]: row
        /// list[5,6]: row income
        /// list[6]: title income
        /// list[7]: list income
        /// list[8]: title gross profit
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public byte[] ExcelPackageProcess(params object[] list)
        {
            var fileTemp = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName);
            var fileNew = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName.Replace("Temp.xlsx", ".xlsx"));
            ExcelPackage pck = new ExcelPackage(fileNew, fileTemp);
            var ws = pck.Workbook.Worksheets[1];
            ws.Name = list[1].ToString();
            ws.Cells[1, 1].Value = list[2];
            ws.Cells[2, 1].Value = list[3];
            ws.Cells[3, 1].Value = list[0];

            SetColumnWorkSheet(ws, list[4], 5, 2);

            ws.Cells[7, 1].Value = list[5];
            int iNextRow = 0;
            List<string> lstNetProfitStart = new List<string>();

            SetRowForIncome_LOE(ws, list[6], list[7], list[8], 8, 1, ref iNextRow, ref lstNetProfitStart);

            ws.Cells[iNextRow, 1].Value = list[9];
            iNextRow += 1;
            SetRowForIncome_LOE(ws, list[10], list[11], list[12], iNextRow, 2, ref iNextRow, ref lstNetProfitStart);
            ws.Cells[8, 2, iNextRow, 6].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;
            return pck.GetAsByteArray();
        }
        
        private string GetDateMonthReport(string FormatDate,string sAdded)
        {
            string sDateReport = string.Empty;
            sDateReport += sAdded;
            sDateReport += GetLastDayOfMonth(DateTime.Now).ToString(FormatDate);
            return sDateReport;
        }

        #region Private function
        
        /// <summary>
        /// Set Row for income less operating expense
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="Title_totalIncome_LOE"></param>
        /// <param name="obj"></param>
        /// <param name="Title_End"></param>
        /// <param name="iCurrentRow"></param>
        /// <param name="iStatus"></param>
        /// <param name="iNextRow"></param>
        /// <param name="lstNetProfitStart"></param>
        private void SetRowForIncome_LOE(ExcelWorksheet ws, object Title_totalIncome_LOE, object obj, object Title_End, int iCurrentRow, int iStatus, ref int iNextRow, ref List<string> lstNetProfitStart)
        {
            if (obj != null)
            {
                var lst = (List<Income_LOE>)obj;
                ws.InsertRow(iCurrentRow, lst.Count - 1);//them dong trong
                List<string> lstStart = new List<string>(), lstEnd = new List<string>(), lstNetProfitEnd = new List<string>();
                for (int i = 0; i < lst.Count; i++)
                {
                    if (i == 0)
                    {//start
                        lstStart.Add(ws.Cells[iCurrentRow, 2].Address);
                        lstStart.Add(ws.Cells[iCurrentRow, 3].Address);
                        lstStart.Add(ws.Cells[iCurrentRow, 4].Address);
                        lstStart.Add(ws.Cells[iCurrentRow, 5].Address);
                        lstStart.Add(ws.Cells[iCurrentRow, 6].Address);
                    }
                    ws.Cells[iCurrentRow, 1].Value = lst[i].AccountName;
                    ws.Cells[iCurrentRow, 2].Value = lst[i].TotalAccountTran_1;
                    ws.Cells[iCurrentRow, 3].Value = lst[i].TotalAccountTran_2;
                    ws.Cells[iCurrentRow, 4].Value = lst[i].TotalAccountTran_3;
                    ws.Cells[iCurrentRow, 5].Value = lst[i].TotalAccountTran_4;
                    ws.Cells[iCurrentRow, 6].Value = lst[i].TotalAccountTran_YTD;

                    if (i == lst.Count - 1)
                    {//end
                        lstEnd.Add(ws.Cells[iCurrentRow, 2].Address);
                        lstEnd.Add(ws.Cells[iCurrentRow, 3].Address);
                        lstEnd.Add(ws.Cells[iCurrentRow, 4].Address);
                        lstEnd.Add(ws.Cells[iCurrentRow, 5].Address);
                        lstEnd.Add(ws.Cells[iCurrentRow, 6].Address);
                    }
                    iCurrentRow += 1;
                    iNextRow = iCurrentRow;
                }

                if (lstStart.Count > 0 && lstEnd.Count > 0 && lstStart.Count == lstEnd.Count)
                {
                    ws.Cells[iCurrentRow, 1].Value = Title_totalIncome_LOE;
                    ws.Cells[iCurrentRow, 2].Formula = string.Format("SUM({0}:{1})", lstStart[0], lstEnd[0]);
                    ws.Cells[iCurrentRow, 3].Formula = string.Format("SUM({0}:{1})", lstStart[1], lstEnd[1]);
                    ws.Cells[iCurrentRow, 4].Formula = string.Format("SUM({0}:{1})", lstStart[2], lstEnd[2]);
                    ws.Cells[iCurrentRow, 5].Formula = string.Format("SUM({0}:{1})", lstStart[3], lstEnd[3]);
                    ws.Cells[iCurrentRow, 6].Formula = string.Format("SUM({0}:{1})", lstStart[4], lstEnd[4]);
                    iCurrentRow += 2;
                    switch (iStatus)
                    {
                        case 1:

                            ws.Cells[iCurrentRow, 1].Value = Title_End;
                            ws.Cells[iCurrentRow, 2].Formula = string.Format("SUM({0}:{1})", lstStart[0], lstEnd[0]);
                            ws.Cells[iCurrentRow, 3].Formula = string.Format("SUM({0}:{1})", lstStart[1], lstEnd[1]);
                            ws.Cells[iCurrentRow, 4].Formula = string.Format("SUM({0}:{1})", lstStart[2], lstEnd[2]);
                            ws.Cells[iCurrentRow, 5].Formula = string.Format("SUM({0}:{1})", lstStart[3], lstEnd[3]);
                            ws.Cells[iCurrentRow, 6].Formula = string.Format("SUM({0}:{1})", lstStart[4], lstEnd[4]);

                            lstNetProfitStart.Add(ws.Cells[iCurrentRow, 2].Address);
                            lstNetProfitStart.Add(ws.Cells[iCurrentRow, 3].Address);
                            lstNetProfitStart.Add(ws.Cells[iCurrentRow, 4].Address);
                            lstNetProfitStart.Add(ws.Cells[iCurrentRow, 5].Address);
                            lstNetProfitStart.Add(ws.Cells[iCurrentRow, 6].Address);
                            break;

                        case 2:

                            ws.Cells[iCurrentRow, 1].Value = Title_End;
                            lstNetProfitEnd.Add(ws.Cells[iNextRow, 2].Address);
                            lstNetProfitEnd.Add(ws.Cells[iNextRow, 3].Address);
                            lstNetProfitEnd.Add(ws.Cells[iNextRow, 4].Address);
                            lstNetProfitEnd.Add(ws.Cells[iNextRow, 5].Address);
                            lstNetProfitEnd.Add(ws.Cells[iNextRow, 6].Address);

                            ws.Cells[iCurrentRow, 2].Formula = string.Format("= {0} - {1}", lstNetProfitStart[0], lstNetProfitEnd[0]);
                            ws.Cells[iCurrentRow, 3].Formula = string.Format("= {0} - {1}", lstNetProfitStart[1], lstNetProfitEnd[1]);
                            ws.Cells[iCurrentRow, 4].Formula = string.Format("= {0} - {1}", lstNetProfitStart[2], lstNetProfitEnd[2]);
                            ws.Cells[iCurrentRow, 5].Formula = string.Format("= {0} - {1}", lstNetProfitStart[3], lstNetProfitEnd[3]);
                            ws.Cells[iCurrentRow, 6].Formula = string.Format("= {0} - {1}", lstNetProfitStart[4], lstNetProfitEnd[4]);

                            break;
                    }
                    iNextRow = iCurrentRow + 2;
                }
            }
        }
        #endregion Private function
    }
    #endregion public class process Profit Loss

    #region public class Inventory Items Summary
    public class SummaryForInventoryItem
    {
        public SummaryForInventoryItem()
        {
            Value = new BaseValue();
            s_Qty = string.Empty;
            Total = new BaseValue();
            Avg = new BaseValue();
            Qty = 0;            
        }

        public BaseValue Value { get; set; }        
        public BaseValue Total { get; set; }
        public BaseValue Avg { get; set; }
        public string s_Qty { get; set; }
        public int? Qty { get; set; }        
    }

    public class Inventory_Item_Summary
    {
        public Inventory_Item_Summary()
        {
            this.Purchases_Price = new SummaryForInventoryItem();
            this.WO_Price = new SummaryForInventoryItem();
            this.MPS_Price = new SummaryForInventoryItem();
            this.Sales_Price = new SummaryForInventoryItem();
            this.Net = new SummaryForInventoryItem();
            Item = string.Empty;
        }

        public SummaryForInventoryItem Purchases_Price { get; set; }
        public SummaryForInventoryItem WO_Price { get; set; }
        public SummaryForInventoryItem MPS_Price { get; set; }
        public SummaryForInventoryItem Sales_Price { get; set; }
        public SummaryForInventoryItem Net { get; set; }

        public string Item { get; set; }        
    }
    
    public class Inventory_Items_Summary : ReportObject
    {
        public List<Inventory_Item_Summary> Inventory_Item_Summary { get; set; }
        
        public string s_Total_Purchases_Price_Qty { get; set; }
        public string s_Total_Purchases_Price_Total { get; set; }
        public string s_Total_WO_Price_Qty { get; set; }
        public string s_Total_WO_Price_Total { get; set; }
        public string s_Total_MPS_Price_Qty { get; set; }
        public string s_Total_MPS_Price_Total { get; set; }
        public string s_Total_Sales_Price_Qty { get; set; }
        public string s_Total_Sales_Price_Total { get; set; }
        public string s_Total_Net_Qty { get; set; }
        public string s_Total_Net_Total { get; set; }

        public string label_Total { get; set; }

        public int? Total_Purchases_Price_Qty { get; set; }
        public double? Total_Purchases_Price_Total { get; set; }
        public int? Total_WO_Price_Qty { get; set; }
        public double? Total_WO_Price_Total { get; set; }
        public int? Total_MPS_Price_Qty { get; set; }
        public double? Total_MPS_Price_Total { get; set; }
        public int? Total_Sales_Price_Qty { get; set; }
        public double? Total_Sales_Price_Total { get; set; }
                
        public int? Total_Net_Qty { get; set; }
        public double? Total_Net_Total { get; set; }

        public Inventory_Items_Summary()
        {
            s_Total_Purchases_Price_Qty = string.Empty;
            s_Total_Purchases_Price_Total = string.Empty;
            s_Total_WO_Price_Qty = string.Empty;
            s_Total_WO_Price_Total = string.Empty;
            s_Total_MPS_Price_Qty = string.Empty;
            s_Total_MPS_Price_Total = string.Empty;
            s_Total_Sales_Price_Qty = string.Empty;
            s_Total_Sales_Price_Total = string.Empty;
            s_Total_Net_Qty = string.Empty;
            s_Total_Net_Total = string.Empty;
            Total_Purchases_Price_Qty = 0;
            Total_Purchases_Price_Total = 0;
            Total_WO_Price_Qty = 0;
            Total_WO_Price_Total = 0;
            Total_MPS_Price_Qty = 0;
            Total_MPS_Price_Total = 0;
            Total_Sales_Price_Qty = 0;
            Total_Sales_Price_Total = 0;            
            Total_Net_Total = 0;
        }
        
        private string GetDateMonthReport(DateTime dtFrom, DateTime dtTo, string FormatDate, string sAdded)
        {
            string sDateReport = string.Empty;
            sDateReport += dtFrom.ToString(FormatDate);
            sDateReport += " ";
            sDateReport += sAdded;
            sDateReport += " ";
            sDateReport += dtTo.ToString(FormatDate);
            return sDateReport;
        }

        /// <summary>
        /// list[0]: report date -
        /// list[1]: name worksheet -
        /// list[2]: title document -
        /// list[3]: company name -
        /// list[4]: column
        /// list[5]: label Total
        /// list[6]: list Item
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public byte[] ExcelPackageProcess(params object[] list)
        {
            var fileTemp = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName);
            var fileNew = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName.Replace("Temp.xlsx", ".xlsx"));
            ExcelPackage pck = new ExcelPackage(fileNew, fileTemp);
            var ws = pck.Workbook.Worksheets[1];
            ws.Name = list[1].ToString();
            ws.Cells[1, 1].Value = list[2];
            ws.Cells[2, 1].Value = list[3];
            ws.Cells[3, 1].Value = list[0];

            SetColumnWorkSheet(ws, list[4], 5, 1);
             
            SetRowForInventory_Item_Summary(ws, (string)list[5], list[6]);

            return pck.GetAsByteArray();
        }

        private void SetRowForInventory_Item_Summary(ExcelWorksheet ws, string p1, object p2)
        {
            var lst = (List<Inventory_Item_Summary>)p2;
            int iCurrentRow = 7;
            string format_1 = "= IF(ISERROR({0}/{1})," + "\"" + "\"" + ",{0}/{1})";
            string format_2 = "= {0} - {1}";
            string format_3 = "= SUM({0}:{1})";
            ws.InsertRow(iCurrentRow, lst.Count - 1);//them dong trong
            List<string> lstStart = new List<string>(), lstEnd = new List<string>(), lstNetProfitEnd = new List<string>();
            for (int i = 0; i < lst.Count; i++)
            {   
                if (i == 0)
                {
                    lstStart.Add(ws.Cells[iCurrentRow, 3].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 4].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 7].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 8].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 11].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 12].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 15].Address);
                    lstStart.Add(ws.Cells[iCurrentRow, 16].Address);
                }
                ws.Cells[iCurrentRow, 1].Value = lst[i].Item;

                ws.Cells[iCurrentRow, 2].Value = lst[i].Purchases_Price.Value;
                ws.Cells[iCurrentRow, 3].Value = lst[i].Purchases_Price.Qty;
                ws.Cells[iCurrentRow, 4].Value = lst[i].Purchases_Price.Total;
                ws.Cells[iCurrentRow, 5].Formula = string.Format(format_1, ws.Cells[iCurrentRow, 3].Address, ws.Cells[iCurrentRow, 4].Address);
                ws.Cells[iCurrentRow, 6].Value = lst[i].WO_Price.Value;
                ws.Cells[iCurrentRow, 7].Value = lst[i].WO_Price.Qty;
                ws.Cells[iCurrentRow, 8].Value = lst[i].WO_Price.Total;
                ws.Cells[iCurrentRow, 9].Formula = string.Format(format_1, ws.Cells[iCurrentRow, 7].Address, ws.Cells[iCurrentRow, 8].Address);
                ws.Cells[iCurrentRow, 10].Value = lst[i].MPS_Price.Value;
                ws.Cells[iCurrentRow, 11].Value = lst[i].MPS_Price.Qty;
                ws.Cells[iCurrentRow, 12].Value = lst[i].MPS_Price.Total;
                ws.Cells[iCurrentRow, 13].Formula = string.Format(format_1, ws.Cells[iCurrentRow, 11].Address, ws.Cells[iCurrentRow, 12].Address);
                ws.Cells[iCurrentRow, 14].Value = lst[i].Sales_Price.Value;
                ws.Cells[iCurrentRow, 15].Value = lst[i].Sales_Price.Qty;
                ws.Cells[iCurrentRow, 16].Value = lst[i].Sales_Price.Total;
                ws.Cells[iCurrentRow, 17].Formula = string.Format(format_1, ws.Cells[iCurrentRow, 15].Address, ws.Cells[iCurrentRow, 16].Address);
                ws.Cells[iCurrentRow, 18].Formula = string.Format(format_2, ws.Cells[iCurrentRow, 3].Address, ws.Cells[iCurrentRow, 15].Address);
                ws.Cells[iCurrentRow, 19].Formula = string.Format(format_2, ws.Cells[iCurrentRow, 16].Address, ws.Cells[iCurrentRow, 4].Address);
                if (i == lst.Count-1)
                {
                    lstEnd.Add(ws.Cells[iCurrentRow, 3].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 4].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 7].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 8].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 11].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 12].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 15].Address);
                    lstEnd.Add(ws.Cells[iCurrentRow, 16].Address);
                }
                iCurrentRow += 1;
            }
            ws.Cells[iCurrentRow, 1].Value = p1;//label total
            //total
            if (lstStart.Count > 0 && lstEnd.Count > 0 && lstStart.Count == lstEnd.Count)
            {
                ws.Cells[iCurrentRow, 3].Formula = string.Format(format_3, lstStart[0], lstEnd[0]);
                ws.Cells[iCurrentRow, 4].Formula = string.Format(format_3, lstStart[1], lstEnd[1]);
                ws.Cells[iCurrentRow, 7].Formula = string.Format(format_3, lstStart[2], lstEnd[2]);
                ws.Cells[iCurrentRow, 8].Formula = string.Format(format_3, lstStart[3], lstEnd[3]);
                ws.Cells[iCurrentRow, 11].Formula = string.Format(format_3, lstStart[4], lstEnd[4]);
                ws.Cells[iCurrentRow, 12].Formula = string.Format(format_3, lstStart[5], lstEnd[5]);
                ws.Cells[iCurrentRow, 15].Formula = string.Format(format_3, lstStart[6], lstEnd[6]);
                ws.Cells[iCurrentRow, 16].Formula = string.Format(format_3, lstStart[7], lstEnd[7]);
                ws.Cells[iCurrentRow, 18].Formula = string.Format(format_2, ws.Cells[iCurrentRow, 3].Address, ws.Cells[iCurrentRow, 15].Address);
                ws.Cells[iCurrentRow, 19].Formula = string.Format(format_2, ws.Cells[iCurrentRow, 16].Address, ws.Cells[iCurrentRow, 4].Address);
            }            
            //format number
            //ws.Cells[7, 2, iCurrentRow, 19].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //ws.Cells[7, 2, iCurrentRow, 19].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //ws.Cells[7, 2, iCurrentRow, 19].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            //ws.Cells[7, 2, iCurrentRow, 19].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
            ws.Cells[7, 2, iCurrentRow, 19].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;
            ws.Cells[7, 3, iCurrentRow, 3].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
            ws.Cells[7, 7, iCurrentRow, 7].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
            ws.Cells[7, 11, iCurrentRow, 11].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
            ws.Cells[7, 15, iCurrentRow, 15].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
            ws.Cells[7, 18, iCurrentRow, 18].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
        }

        /// <summary>
        /// list[0]: SERVER_MAPPATH;
        /// list[1]: ReportFileName;
        /// list[2]: FORMATNUMBEREXPORT;
        /// list[3]: WorkSheetName;
        /// list[4]: TitleDocument;
        /// list[5..23]: Title_Column;
        /// list[24]: CompanyName;
        /// list[25]: Start DateTime;
        /// list[26]: End DateTime;
        /// list[27]: FormatDate;
        /// list[28]: StringAdded;
        /// list[29]: StringlabelTotal;
        /// list[30]: Report Name;
        /// list[31]: FormatDateReport;
        /// list[32]: Format Number;
        /// </summary>
        public Inventory_Items_Summary SetPropertyAll(params object[] list)
        {
            var obj = new Inventory_Items_Summary();
            obj.SERVER_MAPPATH = (string)list[0];
            obj.ReportFileName = (string)list[1];
            obj.FORMATNUMBEREXPORT_1 = (string)list[2];
            obj.WorkSheetName = (string)list[3];
            obj.TitleDocument = (string)list[4];
            obj.ColumnName = GetColumnName(1, 0, DateTime.Now, list[5], list[6], list[7], list[8], list[9], list[10], list[11], list[12], list[13], list[14], list[15], list[16], list[17], list[18], list[19], list[20], list[21], list[22], list[23]);
            obj.CompanyName = (string)list[24];
            obj.ReportDate = GetDateMonthReport((DateTime)list[25], (DateTime)list[26], (string)list[27], (string)list[28]);
            obj.label_Total = (string)list[29];

            obj.ReportSaveAs = obj.CompanyName + " - " + (string)list[30] + DateTime.Now.ToString((string)list[31]) + ".xlsx";
            obj.FORMATNUMBEREXPORT_2 = (string)list[32];
            return obj;
        }
    }
    #endregion public class Inventory Items Summary

    #region public class process Cash Summary
    public class BaseValue
    {
        public BaseValue()
        {
            Value = 0;
            Name = string.Empty;
        }
        public double? Value { get; set; }
        public string Name { get; set; }
    }

    public class I_LOE_PMiE_TM
    {
        public I_LOE_PMiE_TM()
        {
            AccountName = string.Empty;
            Month_01 = new BaseValue();
            Month_02 = new BaseValue();
            Month_03 = new BaseValue();
            Month_04 = new BaseValue();
            Month_05 = new BaseValue();
            Month_06 = new BaseValue();
            Month_07 = new BaseValue();
            Month_08 = new BaseValue();
            Month_09 = new BaseValue();
            Month_10 = new BaseValue();
            Month_11 = new BaseValue();
            Month_12 = new BaseValue();
            IncomePer_CurrMonth = new BaseValue();
            YTD = new BaseValue();
            IncomePer_YTD = new BaseValue();            
        }
        
        public int? AccountID { get; set; }
        public string AccountName { get; set; }
        public BaseValue Month_01 { get; set; }
        public BaseValue Month_02 { get; set; }
        public BaseValue Month_03 { get; set; }
        public BaseValue Month_04 { get; set; }
        public BaseValue Month_05 { get; set; }
        public BaseValue Month_06 { get; set; }
        public BaseValue Month_07 { get; set; }
        public BaseValue Month_08 { get; set; }
        public BaseValue Month_09 { get; set; }
        public BaseValue Month_10 { get; set; }
        public BaseValue Month_11 { get; set; }
        public BaseValue Month_12 { get; set; }
        public BaseValue IncomePer_CurrMonth { get; set; }
        public BaseValue YTD { get; set; }
        public BaseValue IncomePer_YTD { get; set; }
    }

    public class Set_Item
    {
        public Set_Item()
        {
            Title = string.Empty;
            Title_Result = string.Empty;
            Total_Month_01 = new BaseValue();
            Total_Month_02 = new BaseValue();
            Total_Month_03 = new BaseValue();
            Total_Month_04 = new BaseValue();
            Total_Month_05 = new BaseValue();
            Total_Month_06 = new BaseValue();
            Total_Month_07 = new BaseValue();
            Total_Month_08 = new BaseValue();
            Total_Month_09 = new BaseValue();
            Total_Month_10 = new BaseValue();
            Total_Month_11 = new BaseValue();
            Total_Month_12 = new BaseValue();
            Total_IncomePer_CurrMonth = new BaseValue();
            Total_YTD = new BaseValue();
            Total_IncomePer_YTD = new BaseValue();
        }

        public string Title { get; set; }
        public string Title_Result { get; set; }
        public BaseValue Total_Month_01 { get; set; }//12
        public BaseValue Total_Month_02 { get; set; }//11
        public BaseValue Total_Month_03 { get; set; }//10
        public BaseValue Total_Month_04 { get; set; }//9
        public BaseValue Total_Month_05 { get; set; }//8
        public BaseValue Total_Month_06 { get; set; }//7
        public BaseValue Total_Month_07 { get; set; }//6
        public BaseValue Total_Month_08 { get; set; }//5
        public BaseValue Total_Month_09 { get; set; }//4
        public BaseValue Total_Month_10 { get; set; }//3
        public BaseValue Total_Month_11 { get; set; }//2
        public BaseValue Total_Month_12 { get; set; }//1
        public BaseValue Total_IncomePer_CurrMonth { get; set; }
        public BaseValue Total_YTD { get; set; }
        public BaseValue Total_IncomePer_YTD { get; set; }
    }

    public class Cash_Summary : ReportObject
    {
        public Cash_Summary()
        {
            Set_Income = new Set_Item();
            Set_OperatingExpenses = new Set_Item();
            Set_OperatingSurPlus = new Set_Item();
            Set_PlusMovementsInEquity = new Set_Item();
            Set_TaxMovements = new Set_Item();
            Set_NetCashMovement = new Set_Item();
            Title_Summary = string.Empty;
            Set_PlusNetCashMovement = new Set_Item();
            Set_ClosingBalance = new Set_Item();
        }

        public List<I_LOE_PMiE_TM> Income { get; set; }
        public List<I_LOE_PMiE_TM> OperatingExpenses { get; set; }
        public List<I_LOE_PMiE_TM> PlusMovementsInEquity { get; set; }
        public List<I_LOE_PMiE_TM> TaxMovements { get; set; }
        public List<I_LOE_PMiE_TM> Summary { get; set; }

        public Set_Item Set_Income { get; set; }
        public Set_Item Set_OperatingExpenses { get; set; }
        public Set_Item Set_OperatingSurPlus { get; set; }
        public Set_Item Set_PlusMovementsInEquity { get; set; }
        public Set_Item Set_TaxMovements { get; set; }
        public Set_Item Set_NetCashMovement { get; set; }

        public string Title_Summary { get; set; }

        //public Set_Item Set_OpeningBalance { get; set; }
        public Set_Item Set_PlusNetCashMovement { get; set; }
        public Set_Item Set_ClosingBalance { get; set; }

        private string GetDateMonthReport(DateTime dtReport, string FormatDate, string sAdded)
        {
            string sDateReport = string.Empty;
            sDateReport += sAdded;
            sDateReport += " ";

            sDateReport += GetLastDayOfMonth(dtReport).ToString(FormatDate);
            return sDateReport;
        }

        /// <summary>
        /// list[0]: SERVER_MAPPATH;
        /// list[1]: ReportFileName;
        /// list[2]: FORMATNUMBEREXPORT;
        /// list[3]: WorkSheetName;
        /// list[4]: TitleDocument;
        /// list[5]: CompanyName;
        /// list[6]: DateReport;
        /// list[7]: FormatDate;
        /// list[8]: StringAdded;
        /// list[9]: Report Name;
        /// list[10]: FormatDateReport;
        /// list[11]: Format Number;
        /// list[12]: ExtentTitleDocument;
        /// list[13]: Title_Income;
        /// list[14]: Title_TotalIncome;
        /// list[15]: Title_OperatingExpenses;
        /// list[16]: Title_TotalOperatingExpenses;
        /// list[17]: Title_OperatingSurPlus;
        /// list[18]: Title_PlusMovementsInEquity;
        /// list[19]: Title_TotalPlusMovementsInEquity;
        /// list[20]: Title_TaxMovements;
        /// list[21]: Title_NetTaxMovements;
        /// list[22]: Title_NetCashMovement;
        /// list[23]: Title_Summary;
        /// list[24]: Title_OpeningBalance;
        /// list[25]: Title_PlusNetCashMovement;
        /// list[26]: Title_ClosingBalance;
        /// list[27]: FORMATNUMBEREXPORT_3;
        /// list[28..31]: Title_Column;
        /// </summary>
        public Cash_Summary SetPropertyAll(params object[] list)
        {
            var obj = new Cash_Summary();
            obj.SERVER_MAPPATH = (string)list[0];
            obj.FORMATNUMBEREXPORT_1 = (string)list[2];
            obj.ReportFileName = (string)list[1];
            obj.WorkSheetName = (string)list[3];
            obj.TitleDocument = (string)list[4];

            obj.CompanyName = (string)list[5];
            obj.ReportDate = GetDateMonthReport((DateTime)list[6], (string)list[7], (string)list[8]);
            obj.ReportSaveAs = obj.CompanyName + " - " + (string)list[9] + DateTime.Now.ToString((string)list[10]) + ".xlsx";
            obj.FORMATNUMBEREXPORT_2 = (string)list[11];
            obj.ExtentTitleDocument = (string)list[12];

            obj.Set_Income.Title = (string)list[13];
            obj.Set_Income.Title_Result = (string)list[14];
            obj.Set_OperatingExpenses.Title = (string)list[15];
            obj.Set_OperatingExpenses.Title_Result = (string)list[16];
            obj.Set_OperatingSurPlus.Title_Result = (string)list[17];
            obj.Set_PlusMovementsInEquity.Title = (string)list[18];
            obj.Set_PlusMovementsInEquity.Title_Result = (string)list[19];
            obj.Set_TaxMovements.Title = (string)list[20];
            obj.Set_TaxMovements.Title_Result = (string)list[21];
            obj.Set_NetCashMovement.Title_Result = (string)list[22];
            obj.Title_Summary = (string)list[23];
            //obj.Set_OpeningBalance.Title_Result = (string)list[24];
            obj.Set_PlusNetCashMovement.Title_Result = (string)list[25];
            obj.Set_ClosingBalance.Title_Result = (string)list[26];
            obj.FORMATNUMBEREXPORT_3 = (string)list[27];

            obj.ColumnName = GetColumnName(2, -12, (DateTime)list[28], list[29], list[30], list[31]);
            return obj;
        }
        
        public byte[] ExcelPackageProcess(Cash_Summary obj)
        {
            string format_1 = "= IF(ISERROR({0}/{1})," + "\"" + "\"" + ",{0}/{1})";
            string format_2 = "= {0} - {1}";
            string format_3 = "= SUM({0}:{1})";
            string format_4 = "= {0} + {1}";

            var fileTemp = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName);
            var fileNew = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName.Replace("Temp.xlsx", ".xlsx"));
            ExcelPackage pck = new ExcelPackage(fileNew, fileTemp);
            var ws = pck.Workbook.Worksheets[1];
            ws.Name = obj.WorkSheetName;
            ws.Cells[1, 1].Value = obj.TitleDocument;
            ws.Cells[2, 1].Value = obj.CompanyName;
            ws.Cells[3, 1].Value = obj.ReportDate;
            ws.Cells[4, 1].Value = obj.ExtentTitleDocument;

            SetColumnWorkSheet(ws, obj.ColumnName, 6, 2);
            int  iNextRow = 8;
            //income
            ws.Cells[iNextRow, 1].Value = obj.Set_Income.Title;
            iNextRow += 1;
            List<string> lst_1 = new List<string>();
            double dCurrentMonthValue = 0, dYTDValue = 0;
            SetRowForItemCashSummary(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, ref lst_1, format_1, format_2, format_3, obj.Income, obj.Set_Income.Title_Result, false);

            //Operating Expenses
            ws.Cells[iNextRow, 1].Value = obj.Set_OperatingExpenses.Title;
            iNextRow += 1;
            List<string> lst_2 = new List<string>();
            SetRowForItemCashSummary(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, ref lst_2, format_1, format_2, format_3, obj.OperatingExpenses, obj.Set_OperatingExpenses.Title_Result, false);

            //Operating Surplus
            List<string> lst_Operating_Surplus = new List<string>();
            SetRowForOperating_Surplus(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, lst_1, lst_2, format_1, format_2, ref lst_Operating_Surplus, obj.Set_OperatingSurPlus.Title_Result);
            iNextRow += 2;
            lst_1 = lst_2 = null;
            lst_1 = new List<string>();
            ws.Cells[iNextRow, 1].Value = obj.Set_TaxMovements.Title;
            iNextRow += 1;
            SetRowForItemCashSummary(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, ref lst_1, format_1, format_2, format_3, obj.TaxMovements, obj.Set_TaxMovements.Title_Result, true);

            iNextRow += 1;
            SetRowForOperating_Surplus(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, lst_Operating_Surplus, lst_1, format_1, format_4, ref lst_Operating_Surplus, obj.Set_NetCashMovement.Title_Result);

            iNextRow += 2;
            ws.Cells[9, 2, iNextRow, 16].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;

            ws.Cells[9, 3, iNextRow, 3].Style.Numberformat.Format = FORMATNUMBEREXPORT_2;
            ws.Cells[9, 16, iNextRow, 16].Style.Numberformat.Format = FORMATNUMBEREXPORT_3;

            return pck.GetAsByteArray();
        }
        #region Private function

        /// <summary>
        /// Set Row for income less operating expense
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="Title_totalIncome_LOE"></param>
        /// <param name="obj"></param>
        /// <param name="Title_End"></param>
        /// <param name="iCurrentRow"></param>
        /// <param name="iStatus"></param>
        /// <param name="iNextRow"></param>
        /// <param name="lstNetProfitStart"></param>
        private void SetRowForItemCashSummary(ExcelWorksheet ws, ref int iNextRow, ref double dCurrentMonthValue, ref double dYTDValue, ref List<string> lstOperatingSurPlus, string format_1, string format_2, string format_3, List<I_LOE_PMiE_TM> obj, string s_Title_Total, bool bTax)
        {
            if (obj != null)
            {
                ws.InsertRow(iNextRow, obj.Count - 1);//them dong trong
                List<string> lstStart = new List<string>(), lstEnd = new List<string>(), lstNetProfitEnd = new List<string>();
                for (int i = 0; i < obj.Count; i++)
                {
                    if (i == 0)
                    {//start
                        lstStart.Add(ws.Cells[iNextRow, 2].Address);
                        lstStart.Add(ws.Cells[iNextRow, 4].Address);
                        lstStart.Add(ws.Cells[iNextRow, 5].Address);
                        lstStart.Add(ws.Cells[iNextRow, 6].Address);
                        lstStart.Add(ws.Cells[iNextRow, 7].Address);
                        lstStart.Add(ws.Cells[iNextRow, 8].Address);
                        lstStart.Add(ws.Cells[iNextRow, 9].Address);
                        lstStart.Add(ws.Cells[iNextRow, 10].Address);
                        lstStart.Add(ws.Cells[iNextRow, 11].Address);
                        lstStart.Add(ws.Cells[iNextRow, 12].Address);
                        lstStart.Add(ws.Cells[iNextRow, 13].Address);
                        lstStart.Add(ws.Cells[iNextRow, 14].Address);
                        lstStart.Add(ws.Cells[iNextRow, 15].Address);
                        if (!bTax)
                        {
                            dCurrentMonthValue = obj[i].Month_01.Value.Value;
                            dYTDValue = obj[i].YTD.Value.Value;
                        }                        
                    }
                    ws.Cells[iNextRow, 1].Value = obj[i].AccountName;
                    ws.Cells[iNextRow, 2].Value = obj[i].Month_01.Value;

                    if (!bTax)
                    {
                        ws.Cells[iNextRow, 3].Formula = string.Format(format_1, ws.Cells[iNextRow, 2].Address, dCurrentMonthValue);
                        ws.Cells[iNextRow, 16].Formula = string.Format(format_1, ws.Cells[iNextRow, 15].Address, dYTDValue);
                    }
                    
                    ws.Cells[iNextRow, 4].Value = obj[i].Month_02.Value;
                    ws.Cells[iNextRow, 5].Value = obj[i].Month_03.Value;
                    ws.Cells[iNextRow, 6].Value = obj[i].Month_04.Value;
                    ws.Cells[iNextRow, 7].Value = obj[i].Month_05.Value;
                    ws.Cells[iNextRow, 8].Value = obj[i].Month_06.Value;
                    ws.Cells[iNextRow, 9].Value = obj[i].Month_07.Value;
                    ws.Cells[iNextRow, 10].Value = obj[i].Month_08.Value;
                    ws.Cells[iNextRow, 11].Value = obj[i].Month_09.Value;
                    ws.Cells[iNextRow, 12].Value = obj[i].Month_10.Value;
                    ws.Cells[iNextRow, 13].Value = obj[i].Month_11.Value;
                    ws.Cells[iNextRow, 14].Value = obj[i].Month_12.Value;
                    ws.Cells[iNextRow, 15].Value = obj[i].YTD.Value;
                    
                    if (i == obj.Count - 1)
                    {//end
                        lstEnd.Add(ws.Cells[iNextRow, 2].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 4].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 5].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 6].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 7].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 8].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 9].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 10].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 11].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 12].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 13].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 14].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 15].Address);
                    }

                    iNextRow += 1;
                }

                if (lstStart.Count > 0 && lstEnd.Count > 0 && lstStart.Count == lstEnd.Count)
                {
                    SetRowForOperating_Surplus(ws, ref iNextRow, ref dCurrentMonthValue, ref dYTDValue, lstStart, lstEnd, format_1, format_3, ref lstOperatingSurPlus, s_Title_Total);                                        
                    iNextRow += 2;
                }
            }
        }


        private void SetRowForOperating_Surplus(ExcelWorksheet ws, ref int iNextRow, ref double dCurrentMonthValue, ref double dYTDValue, List<string> lst_1, List<string> lst_2, string s_format_1, string s_format_2, ref List<string> lst_Operating_Surplus, string s_Title_Total)
        {
            ws.Cells[iNextRow, 1].Value = s_Title_Total;
            ws.Cells[iNextRow, 2].Formula = string.Format(s_format_2, lst_1[0], lst_2[0]);
            ws.Cells[iNextRow, 3].Formula = string.Format(s_format_1, ws.Cells[iNextRow, 2].Address, dCurrentMonthValue);
            ws.Cells[iNextRow, 4].Formula = string.Format(s_format_2, lst_1[1], lst_2[1]);
            ws.Cells[iNextRow, 5].Formula = string.Format(s_format_2, lst_1[2], lst_2[2]);
            ws.Cells[iNextRow, 6].Formula = string.Format(s_format_2, lst_1[3], lst_2[3]);
            ws.Cells[iNextRow, 7].Formula = string.Format(s_format_2, lst_1[4], lst_2[4]);
            ws.Cells[iNextRow, 8].Formula = string.Format(s_format_2, lst_1[5], lst_2[5]);
            ws.Cells[iNextRow, 9].Formula = string.Format(s_format_2, lst_1[6], lst_2[6]);
            ws.Cells[iNextRow, 10].Formula = string.Format(s_format_2, lst_1[7], lst_2[7]);
            ws.Cells[iNextRow, 11].Formula = string.Format(s_format_2, lst_1[8], lst_2[8]);
            ws.Cells[iNextRow, 12].Formula = string.Format(s_format_2, lst_1[9], lst_2[9]);
            ws.Cells[iNextRow, 13].Formula = string.Format(s_format_2, lst_1[10], lst_2[10]);
            ws.Cells[iNextRow, 14].Formula = string.Format(s_format_2, lst_1[11], lst_2[11]);
            ws.Cells[iNextRow, 15].Formula = string.Format(s_format_2, lst_1[12], lst_2[12]);
            ws.Cells[iNextRow, 16].Formula = string.Format(s_format_1, ws.Cells[iNextRow, 15].Address, dYTDValue);

            if (lst_1.Count > 0 && lst_2.Count > 0 && lst_1.Count == lst_2.Count)
            {
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 2].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 4].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 5].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 6].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 7].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 8].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 9].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 10].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 11].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 12].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 13].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 14].Address);
                lst_Operating_Surplus.Add(ws.Cells[iNextRow, 15].Address);                
            }
        }
        #endregion Private function
    }
    #endregion public class process Cash Summary

    #region public class process Customer Invoice - Supplier Invoice
    public class CustomerInvoice_SupplierInvoice
    {
        public CustomerInvoice_SupplierInvoice()
        {
            ID = 0;
            Invoice_Number = string.Empty;
            Reference = string.Empty;
            Type = 0;
            ContactName = string.Empty;
            Date = string.Empty;
            Due_Date = string.Empty;
            Expected_Planned_Date = string.Empty;
            Paid_Date = string.Empty;
            Invoice_Total = 0;
            s_Invoice_Total = string.Empty;
            Paid = 0;
            s_Paid = string.Empty;
            Due = 0;
            s_Due = string.Empty;
            Sent = string.Empty;
            Status = 0;
        }

        public int? ID { get; set; }
        public string Invoice_Number { get; set; }
        public string Reference { get; set; }
        public int? Type { get; set; }
        public string ContactName { get; set; }
        public string Date { get; set; }
        public string Due_Date { get; set; }
        public string Expected_Planned_Date { get; set; }
        public string Paid_Date { get; set; }
        public double? Invoice_Total { get; set; }
        public string s_Invoice_Total { get; set; }
        public double? Paid { get; set; }
        public string s_Paid { get; set; }
        public double? Due { get; set; }
        public string s_Due { get; set; }
        public string Sent { get; set; }
        public int? Status { get; set; }        
    }

    public class CustomerInvoice_SupplierInvoice_Report : ReportObject
    {
        public CustomerInvoice_SupplierInvoice_Report()
        {
            s_Total_Invoice_Total = string.Empty;
            s_Total_Paid = string.Empty;
            s_Total_Due = string.Empty;
            TypeReport = 0;
        }

        public string label_Total { get; set; }
        public string s_Total_Invoice_Total { get; set; }
        public string s_Total_Paid { get; set; }
        public string s_Total_Due { get; set; }
        public List<CustomerInvoice_SupplierInvoice> CustomerInvoice_SupplierInvoices { get; set; }
        public List<string> s_Type { get; set; }
        public List<string> s_Status { get; set; }
        public int TypeReport { get; set; }
        private string GetDateMonthReport(DateTime dtFrom, DateTime dtTo, string FormatDate, string sAdded_1, string sAdded_2)
        {
            string sDateReport = string.Empty;
            sDateReport += sAdded_1;
            sDateReport += dtFrom.ToString(FormatDate);
            sDateReport += " ";
            sDateReport += sAdded_2;
            sDateReport += " ";
            sDateReport += dtTo.ToString(FormatDate);
            return sDateReport;
        }

        /// <summary>
        /// list[0]: SERVER_MAPPATH;
        /// list[1]: ReportFileName;
        /// list[2]: WorkSheetName;
        /// list[3]: TitleDocument;
        /// list[4]: CompanyName;
        /// list[5]: DateTime from;
        /// list[6]: DateTime to;
        /// list[7]: format date;
        /// list[8]: string added 1;
        /// list[9]: string added 2;
        /// list[10]: StringlabelTotal;
        /// list[11]: Report Name;
        /// list[12]: FormatDateReport;        
        /// list[13]: trạng thái;
        /// list[14..28]: Title_Column;
        /// list[29]: Format Number;        
        /// </summary>
        public CustomerInvoice_SupplierInvoice_Report SetPropertyAll(params object[] list)
        {
            var obj = new CustomerInvoice_SupplierInvoice_Report();
            obj.SERVER_MAPPATH = (string)list[0];
            obj.ReportFileName = (string)list[1];
            obj.WorkSheetName = (string)list[2];
            obj.TitleDocument = (string)list[3];
            obj.CompanyName = (string)list[4];
            obj.ReportDate = GetDateMonthReport((DateTime)list[5], (DateTime)list[6], (string)list[7], (string)list[8], (string)list[9]);
            obj.label_Total = (string)list[10];
            obj.ReportSaveAs = obj.CompanyName + " - " + (string)list[11] + DateTime.Now.ToString((string)list[12]) + ".xlsx";            
            switch ((int)list[13])
            {
                case 1:
                    obj.ColumnName = GetColumnName(1, 0, DateTime.Now, list[14], list[15], list[16], list[17], list[18], list[19], list[20], list[21], list[22], list[23], list[24]);
                    obj.FORMATNUMBEREXPORT_1 = (string)list[25];
                    break;
                case 2:
                    obj.ColumnName = GetColumnName(1, 0, DateTime.Now, list[14], list[15], list[16], list[17], list[18], list[19], list[20], list[21], list[22], list[23]);
                    obj.FORMATNUMBEREXPORT_1 = (string)list[24];
                    break;
            }
            obj.TypeReport = (int)list[13];                        
            return obj;
        }

        /// <summary>
        /// list[0]: report date -
        /// list[1]: name worksheet -
        /// list[2]: title document -
        /// list[3]: company name -
        /// list[4]: column
        /// list[5..]: row
        /// list[5,6]: row income
        /// list[6]: title income
        /// list[7]: list income
        /// list[8]: title gross profit
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public byte[] ExcelPackageProcess(CustomerInvoice_SupplierInvoice_Report obj)
        {
            var fileTemp = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName);
            var fileNew = new System.IO.FileInfo(SERVER_MAPPATH + ReportFileName.Replace("Temp.xlsx", ".xlsx"));
            ExcelPackage pck = new ExcelPackage(fileNew, fileTemp);
            var ws = pck.Workbook.Worksheets[1];
            ws.Name = obj.WorkSheetName;
            ws.Cells[1, 1].Value = obj.TitleDocument;
            ws.Cells[2, 1].Value = obj.CompanyName;
            ws.Cells[3, 1].Value = obj.ReportDate;
            SetColumnWorkSheet(ws, obj.ColumnName, 4, 1);


            SetRowForCustomerInvoice_SupplierInvoice(ws, obj.CustomerInvoice_SupplierInvoices, obj.s_Type, obj.s_Status, obj.label_Total, obj.TypeReport);

            //ws.Cells[7, 1].Value = list[5];
            //int iNextRow = 0;
            //List<string> lstNetProfitStart = new List<string>();

            //SetRowForIncome_LOE(ws, list[6], list[7], list[8], 8, 1, ref iNextRow, ref lstNetProfitStart);

            //ws.Cells[iNextRow, 1].Value = list[9];
            //SetRowForIncome_LOE(ws, list[10], list[11], list[12], 14, 2, ref iNextRow, ref lstNetProfitStart);
            //ws.Cells[8, 2, iNextRow, 6].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;
            return pck.GetAsByteArray();
        }

        #region Private function

        private void SetRowForCustomerInvoice(ExcelWorksheet ws, List<CustomerInvoice_SupplierInvoice> obj, List<string> s_Type, List<string> s_Status, string s_Title_Total)
        {
            List<string> lstStart = new List<string>(), lstEnd = new List<string>(), lstNetProfitEnd = new List<string>();
            if (obj != null)
            {
                int iNextRow = 5;
                ws.InsertRow(iNextRow, obj.Count - 1);
                for (int i = 0; i < obj.Count; i++)
                {
                    if (i == 0)
                    {
                        lstStart.Add(ws.Cells[iNextRow, 8].Address);
                        lstStart.Add(ws.Cells[iNextRow, 9].Address);
                        lstStart.Add(ws.Cells[iNextRow, 10].Address);
                    }
                    ws.Cells[iNextRow, 1].Value = obj[i].Invoice_Number;
                    ws.Cells[iNextRow, 2].Value = obj[i].Reference;
                    ws.Cells[iNextRow, 3].Value = (obj[i].Type.HasValue && obj[i].Type.Value != -1) ? s_Type[obj[i].Type.Value] : string.Empty;
                    ws.Cells[iNextRow, 4].Value = obj[i].ContactName;
                    ws.Cells[iNextRow, 5].Value = obj[i].Date;
                    ws.Cells[iNextRow, 6].Value = obj[i].Due_Date;
                    ws.Cells[iNextRow, 7].Value = obj[i].Paid_Date;
                    ws.Cells[iNextRow, 8].Value = obj[i].Invoice_Total;
                    ws.Cells[iNextRow, 9].Value = obj[i].Paid;
                    ws.Cells[iNextRow, 10].Value = obj[i].Due;                    
                    ws.Cells[iNextRow, 11].Value = (obj[i].Status.HasValue && obj[i].Status.Value != -1) ? s_Status[obj[i].Status.Value] : string.Empty;
                    ws.Cells[iNextRow, 11].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;
                    if (i == obj.Count - 1)
                    {
                        lstEnd.Add(ws.Cells[iNextRow, 8].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 9].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 10].Address);
                    }
                    iNextRow += 1;
                }
                if (lstStart.Count > 0 && lstEnd.Count > 0 && lstStart.Count == lstEnd.Count)
                {
                    ws.Cells[iNextRow, 1].Value = s_Title_Total;
                    ws.Cells[iNextRow, 8].Formula = string.Format("SUM({0}:{1})", lstStart[0], lstEnd[0]);
                    ws.Cells[iNextRow, 9].Formula = string.Format("SUM({0}:{1})", lstStart[1], lstEnd[1]);
                    ws.Cells[iNextRow, 10].Formula = string.Format("SUM({0}:{1})", lstStart[2], lstEnd[2]);
                }

                ws.Cells[5, 8, iNextRow, 10].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;                
            }
        }

        private void SetRowForSaleInvoice(ExcelWorksheet ws, List<CustomerInvoice_SupplierInvoice> obj, List<string> s_Type, List<string> s_Status, string s_Title_Total)
        {
            List<string> lstStart = new List<string>(), lstEnd = new List<string>(), lstNetProfitEnd = new List<string>();
            if (obj != null)
            {
                int iNextRow = 5;
                ws.InsertRow(iNextRow, obj.Count - 1);
                for (int i = 0; i < obj.Count; i++)
                {
                    if (i == 0)
                    {
                        lstStart.Add(ws.Cells[iNextRow, 7].Address);
                        lstStart.Add(ws.Cells[iNextRow, 8].Address);
                        lstStart.Add(ws.Cells[iNextRow, 9].Address);
                    }
                    ws.Cells[iNextRow, 1].Value = obj[i].Reference;
                    ws.Cells[iNextRow, 2].Value = (obj[i].Type.HasValue && obj[i].Type.Value != -1) ? s_Type[obj[i].Type.Value] : string.Empty;
                    ws.Cells[iNextRow, 3].Value = obj[i].ContactName;
                    ws.Cells[iNextRow, 4].Value = obj[i].Date;
                    ws.Cells[iNextRow, 5].Value = obj[i].Due_Date;
                    ws.Cells[iNextRow, 6].Value = obj[i].Paid_Date;
                    ws.Cells[iNextRow, 7].Value = obj[i].Invoice_Total;
                    ws.Cells[iNextRow, 8].Value = obj[i].Paid;
                    ws.Cells[iNextRow, 9].Value = obj[i].Due;
                    ws.Cells[iNextRow, 10].Value = (obj[i].Status.HasValue && obj[i].Status.Value != -1) ? s_Status[obj[i].Status.Value] : string.Empty;
                    ws.Cells[iNextRow, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Double;
                    if (i == obj.Count - 1)
                    {
                        lstEnd.Add(ws.Cells[iNextRow, 7].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 8].Address);
                        lstEnd.Add(ws.Cells[iNextRow, 9].Address);
                    }
                    iNextRow += 1;
                }
                if (lstStart.Count > 0 && lstEnd.Count > 0 && lstStart.Count == lstEnd.Count)
                {
                    ws.Cells[iNextRow, 1].Value = s_Title_Total;
                    ws.Cells[iNextRow, 7].Formula = string.Format("SUM({0}:{1})", lstStart[0], lstEnd[0]);
                    ws.Cells[iNextRow, 8].Formula = string.Format("SUM({0}:{1})", lstStart[1], lstEnd[1]);
                    ws.Cells[iNextRow, 9].Formula = string.Format("SUM({0}:{1})", lstStart[2], lstEnd[2]);
                }

                ws.Cells[5, 7, iNextRow, 9].Style.Numberformat.Format = FORMATNUMBEREXPORT_1;
            }
        }

        /// <summary>
        /// Set Row for income less operating expense
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="Title_totalIncome_LOE"></param>
        /// <param name="obj"></param>
        /// <param name="Title_End"></param>
        /// <param name="iCurrentRow"></param>
        /// <param name="iStatus"></param>
        /// <param name="iNextRow"></param>
        /// <param name="lstNetProfitStart"></param>
        private void SetRowForCustomerInvoice_SupplierInvoice(ExcelWorksheet ws, List<CustomerInvoice_SupplierInvoice> obj, List<string> s_Type, List<string> s_Status, string s_Title_Total, int iTypeReport)
        {
            if (obj != null)
            {                
                switch (iTypeReport)
                {
                    case 1:
                        SetRowForCustomerInvoice(ws, obj, s_Type, s_Status, s_Title_Total);
                        break;
                    case 2:
                        SetRowForSaleInvoice(ws, obj, s_Type, s_Status, s_Title_Total);
                        break;
                }                
            }
        }


        
        #endregion Private function
    }
    #endregion public class process Customer Invoice - Supplier Invoice


    #region public class process Sale By Item
    public class ItemSale
    {
        public ItemSale()
        {
            ItemName = string.Empty;
            CurrentUnitPrice = new BaseValue();
            QuantitySold = new BaseValue();
            Total = new BaseValue();
            AveragePrice = new BaseValue();
        }
        public string ItemName { get; set; }
        public BaseValue CurrentUnitPrice { get; set; }
        public BaseValue QuantitySold { get; set; }
        public BaseValue Total { get; set; }
        public BaseValue AveragePrice { get; set; }
    }

    public class SaleByItem : ReportObject
    {
        public SaleByItem()
        {
            Title = string.Empty;
            Title_Result = string.Empty;
            s_QuantitySold_Total = string.Empty;
            s_Total_Total = string.Empty;
            s_AveragePrice_Total = string.Empty;
            s_SalesByItem = string.Empty;
            s_OtherSales = string.Empty;
            s_CashSales = string.Empty;
            s_Credits = string.Empty;
            s_Total_Sales = string.Empty;            
        }
        public List<ItemSale> ItemSales { get; set; }
        public string Title { get; set; }
        public string Title_Result { get; set; }
        public string s_QuantitySold_Total { get; set; }
        public string s_Total_Total { get; set; }
        public string s_AveragePrice_Total { get; set; }
        public string s_SalesByItem { get; set; }
        public string s_OtherSales { get; set; }
        public string s_CashSales { get; set; }
        public string s_Credits { get; set; }
        public string s_Total_Sales { get; set; }
    }
    #endregion public class process Sale By Item
}