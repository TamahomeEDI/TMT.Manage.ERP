using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class ChartController : BaseController
    {
        private const string formatNumber = "#,0.00";
        private const int HeightDefault = 150;

        /// <summary>
        /// define color 0: negative 1... :
        /// </summary>
        private readonly string[] DefineColor = { "#cc0000", "#2f7ed8", "#64FE2E" };
        
        #region Process Line
        private List<string> GetDayRange(int iType)
        {
            string formatDate = "MMM dd";
            List<string> Obj = new List<string>();
            var dateNow = DateTime.Now.Date;
            double dayUntilNow = 0;
            switch (iType)
            {
                case 1:
                    for (int i = -7; i < 1; i++)
                    {
                        Obj.Add(DateTime.Now.AddDays(i).ToString(formatDate));
                    }
                    break;

                case 2:
                    dayUntilNow = dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;

                    for (var i = 0; i <= dayUntilNow + 1; i++)
                    {
                        Obj.Add(DateTime.Now.AddDays(i).ToString(formatDate));
                    }
                    break;

                case 3:
                    dateNow = dateNow.AddMonths(1);
                    dayUntilNow = dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;
                    dateNow = DateTime.Now.Date;
                    for (var i = 0; i <= dayUntilNow; i++)
                    {
                        Obj.Add(DateTime.Now.AddDays(i).ToString(formatDate));
                    }
                    break;

                case 4:
                    dayUntilNow = dateNow.Subtract(dateNow.AddDays(-7).Date).TotalDays;
                    dateNow = dateNow.AddMonths(1);
                    dayUntilNow += dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;
                    for (var i = -7; i <= dayUntilNow - 7; i++)
                    {
                        var sAdd = (i % 2 == 0) ? string.Empty : DateTime.Now.AddDays(i).ToString(formatDate);
                        Obj.Add(sAdd);                        
                    }
                    break;
            }
            
            
            return Obj;
        }

        private List<double> GetLineBanking(int ID)
        {
            List<double> Obj = new List<double>();
            double dObj = 0;
            var BankAccount = new GenericManager<BankAccount>().FindById(ID);
            if (BankAccount != null)
            {
                for (int i = -7; i < 1; i++)
                {
                    dObj = (BankAccount.AccountTrans.Where(x => x.Date <= DateTime.Now.AddDays(i).Date).Sum(x => x.Received)
                                - BankAccount.AccountTrans.Where(x => x.Date <= DateTime.Now.AddDays(i).Date).Sum(x => x.Spent)).Value;
                    Obj.Add(dObj);
                }                
            }            
            
            return Obj;
        }

        private List<double> GetLineAccountTrans(int ID)
        {
            List<double> Obj = new List<double>();
            double dObj = 0;
            var accountTrans = new GenericManager<AccountTran>().Get().Where(x=>x.BankAccountID == ID);
            if (accountTrans != null)
            {
                var dateNow = DateTime.Now.Date;
                double dayUntilNow = dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;

                for (var i = 0; i <= dayUntilNow + 1; i++)
                {
                    dObj = (accountTrans.Where(x => x.Date <= DateTime.Now.AddDays(i).Date).Sum(x => x.Received)
                        - accountTrans.Where(x => x.Date <= DateTime.Now.AddDays(i).Date).Sum(x => x.Spent)).Value;
                    Obj.Add(dObj);
                }
            }

            return Obj;
        }

        /// <summary>
        /// Line iType: 1 - Banking; 2 - AccountTrans
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="iType"></param>
        /// <returns></returns>
        private List<double> GetLine(int ID, int iType)
        {
            List<double> Obj = new List<double>();
            
            switch (iType)
            {
                case 1:
                    //banking
                    Obj = GetLineBanking(ID);
                    break;   
                case 2:
                    //AccountTrans 
                    Obj = GetLineAccountTrans(ID);
                    break;
            }
            
            return Obj;
        }

        private System.Drawing.Color GetBarColour(double value, int iStatus)
        {//#cc0000
            if (value < 0) return System.Drawing.ColorTranslator.FromHtml(DefineColor[0]);//if value negative
            return System.Drawing.ColorTranslator.FromHtml(DefineColor[iStatus]);
        }

        public ActionResult BasicLine(int ID, int iType, string chartname)
        {
            var dataItems = GetLine(ID, iType).ToArray();
            Data data = new Data(
                dataItems.Select(y => new DotNet.Highcharts.Options.Point { Color = GetBarColour(y, 1), Y = y }).ToArray()
            );
            Highcharts chart = new Highcharts(chartname)
                .InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line,
                    MarginRight = 50,//130
                    //MarginBottom = 25,
                    Width = iType == 1 ? 301 : 768,
                    Height = HeightDefault,
                    ClassName = "chart"
                })

                .SetTitle(new Title
                {
                    Text = ""
                })
                //.SetSubtitle(new Subtitle
                //{
                //    Text = "Source: WorldClimate.com",
                //    X = -20
                //})
                .SetXAxis(new XAxis { Categories = GetDayRange(iType).ToArray() })

                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "" },
                    PlotLines = new[]
                            {
                                new YAxisPlotLines
                                    {
                                        Value = 0,
                                        Width = 1,
                                        Color = ColorTranslator.FromHtml("#808080")
                                    }
                            }
                })
                .SetTooltip(new Tooltip
                {
                    Formatter = @"function() {
                                        return this.x +': '+ Highcharts.numberFormat(this.y);
                                }",


                })
                //.SetPlotOptions(new PlotOptions { Column = new PlotOptionsColumn { ColorByPoint = true, Color = ColorTranslator.FromHtml("#800000") } })
                .SetLegend(new Legend
                {
                    //Layout = Layouts.Vertical,
                    //Align = HorizontalAligns.Right,
                    //VerticalAlign = VerticalAligns.Top,
                    //X = -10,
                    //Y = 100,
                    //BorderWidth = 0,
                    Enabled = false
                })
                .SetSeries(new[]
                    {
                        new Series { Name = "", Data = data }//new Data(dataItems.Cast<object>().ToArray()), Color = GetBarColour(dataItems,1)
                    }
                );

            return View(chart);
        }
        #endregion Process Line

        #region Process Column
        private List<string> GetMonthRange()
        {
            List<string> Obj = new List<string>();
            string formatMonth = "MMM";

            Obj.Add("Older");
            Obj.Add(DateTime.Now.AddMonths(-3).ToString(formatMonth));
            Obj.Add(DateTime.Now.AddMonths(-2).ToString(formatMonth));
            Obj.Add(DateTime.Now.AddMonths(-1).ToString(formatMonth));
            Obj.Add(DateTime.Now.ToString(formatMonth));
            Obj.Add("Future");
            
            return Obj;
        }

        private DateTime GetLastDayOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month)).Date;
        }

        private List<URL_Chart> GetColumnMoneyComingIn()
        {
            var Obj = new List<URL_Chart>();
            double dObj = 0;            
            var oSaleInvoices = new GenericManager<SaleInvoice>().Get().Where(s => s.Status == 2);

            if (oSaleInvoices != null)
            {
                var uRL_Chart = new URL_Chart();
                string sUrl = "/SaleInvoice/AwaitingPayment?searchString=&type=-1&createdate=&duadate";
                
                //older
                var temp = oSaleInvoices;
                dObj = (from item in temp
                        where item.DueDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-4))
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-4))
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;
                
                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now.AddMonths(-4)).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 3 month
                uRL_Chart = new URL_Chart();
                
                dObj = (from item in temp
                        where item.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-4))
                        && item.DueDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-4))
                         && item.CreatedDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;
                
                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now.AddMonths(-3)).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 2 month  
                uRL_Chart = new URL_Chart();

                dObj = (from item in temp
                        where item.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                        && item.DueDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-3))
                         && item.CreatedDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;

                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now.AddMonths(-2)).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 1 month
                uRL_Chart = new URL_Chart();

                dObj = (from item in temp
                        where item.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                        && item.DueDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-2))
                         && item.CreatedDate < GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;

                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now.AddMonths(-1)).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //current month
                uRL_Chart = new URL_Chart();

                dObj = (from item in temp
                        where item.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                        && item.DueDate < GetLastDayOfMonth(DateTime.Now)
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-1))
                         && item.CreatedDate < GetLastDayOfMonth(DateTime.Now)
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;

                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //future month
                uRL_Chart = new URL_Chart();

                dObj = (from item in temp
                        where item.DueDate > GetLastDayOfMonth(DateTime.Now)
                        && item.RemainMoney != 0 && item.Type == 0
                        select item).Sum(r => r.RemainMoney).Value;

                dObj -= (from item in temp
                         where item.CreatedDate > GetLastDayOfMonth(DateTime.Now)
                         && item.RemainMoney != 0 && item.Type == 1
                         select item).Sum(r => r.RemainMoney).Value;

                uRL_Chart.y = dObj;
                uRL_Chart.url = sUrl + "=" + GetLastDayOfMonth(DateTime.Now).ToString("MM-yyyy");
                Obj.Add(uRL_Chart);
                uRL_Chart = null;
            }

            return Obj;
        }

        private List<URL_Chart> GetColumn(int iType)
        {
            List<URL_Chart> Obj = new List<URL_Chart>();

            switch (iType)
            {
                case 1://coming in
                    {
                        Obj = GetColumnMoneyComingIn();
                        break;
                    }
            }


            return Obj;
        }

        public ActionResult BasicColumn(int iType, string chartname)
        {
            var dataItems = GetColumn(iType).ToArray();
            Data data = new Data(dataItems.Select(y => new DotNet.Highcharts.Options.Point { Color = GetBarColour(y.y, 1), Y = y.y, Id = y.url}).ToArray());

            Highcharts chart = new Highcharts(chartname)                
                .SetXAxis(new XAxis { Categories = GetMonthRange().ToArray() })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "" } })
                .SetSeries(new Series { Data = data })
                .SetTitle(new Title { Text = "" })
                .SetTooltip(new Tooltip
                {
                    Formatter = @"function() { var txt;switch (this.point.x){case 0: txt = '4+ months ago';break;case 1: txt = '3 months ago';break;case 2: txt = '2 months ago';break;case 3: txt = 'Last month ago';break;case 4: txt = 'Current month';break;case 5: txt = 'Future months';break;} return ''+ txt +': '+ Highcharts.numberFormat(this.y) + '<br/> <label style=" + "\"color: #048fc2;font-size: 10px;\"" + ">Click to view</label>' ;}"
                })                
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column, Width = 463, Height = HeightDefault, BorderWidth = 1, BorderColor = System.Drawing.ColorTranslator.FromHtml("#C1C1C1"), BorderRadius = 0 })
                
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        PointPadding = 0.2,
                        BorderWidth = 0,
                        ColorByPoint = true
                    },
                    Series = new PlotOptionsSeries
                    {
                        Cursor = Cursors.Pointer,
                        Point = new PlotOptionsSeriesPoint
                        {
                            Events = new PlotOptionsSeriesPointEvents
                            {
                                Click = "function(){window.location.href = this.options.id;}"
                            }
                        }
                    }
                })
                .SetLegend(new Legend { Enabled = false });


            return View(chart);
        }
        #endregion Process Column

        #region Process Stacked Column
        private List<string> GetDayPurchaseRange()
        {
            List<string> Obj = new List<string>();
            string formatDate = "MMM dd";

            for (int i = 0; i < 15; i++)
            {
                Obj.Add(DateTime.Now.AddDays(i).ToString(formatDate));
            }

            return Obj;
        }

        private double GetMoneyValue(int iType, int iStep, IEnumerable<Purchase> oObjCol)
        {
            double dValue = 0;
            switch (iType)
            {
                case 1:
                    dValue = (from item in oObjCol
                              where item.Type == 0 &&
                              item.DueDate == DateTime.Now.AddDays(iStep).Date
                              select item).Sum(p => p.TotalMoney - p.RemainMoney).Value;
                    dValue -= (from item in oObjCol
                              where item.Type == 1 &&
                              item.DueDate == DateTime.Now.AddDays(iStep).Date
                              select item).Sum(p => p.TotalMoney - p.RemainMoney).Value;
                    break;
                case 2:
                    dValue = (from item in oObjCol
                              where item.Type == 0 &&
                              item.DueDate == DateTime.Now.AddDays(iStep).Date
                              select item).Sum(p => p.RemainMoney).Value;
                    dValue -= (from item in oObjCol
                              where item.Type == 1 &&
                              item.DueDate == DateTime.Now.AddDays(iStep).Date
                               select item).Sum(p => p.RemainMoney).Value;
                    break;
            }
            return dValue;
        }

        /// <summary>
        /// iStatus 1 - chua tra; 2 - da tra
        /// </summary>
        /// <param name="iStatus"></param>
        /// <returns></returns>
        private List<URL_Chart> GetColumnMoneyComingOut(int iType)
        {
            List<URL_Chart> Obj = new List<URL_Chart>();
            double dObj = 0;

            var oPurchase = new GenericManager<Purchase>().Get().Where(s => s.Status == 2 || s.Status == 3);
            if (oPurchase != null)
            {
                string sUrl = "/Purchase/AwaitingPayment?searchString=&type=-1&createdate=&duadate=";                
                for (int i = 0; i < 15; i++)
                {
                    URL_Chart uRL_Chart = new URL_Chart();

                    dObj = GetMoneyValue(iType, i, oPurchase);

                    uRL_Chart.y = dObj;
                    uRL_Chart.url = sUrl + DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");

                    Obj.Add(uRL_Chart);
                    uRL_Chart = null;
                }    
            }            

            return Obj;
        }

        private List<URL_Chart> GetColumnMoneySaleInvoice(int iType)
        {
            var Obj = new List<URL_Chart>();
            double dObj = 0;
            var oSaleInvoices = new GenericManager<SaleInvoice>().Get().Where(s => s.Status == 2 || s.Status == 3).Where(s => s.Type == iType);

            if (oSaleInvoices != null)
            {
                var uRL_Chart = new URL_Chart();
                //older
                dObj = oSaleInvoices.Where(s => s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-4))).Sum(s => s.RemainMoney).Value;//;;                
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.y = dObj;
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-4))).Select(s => s.ContactName).FirstOrDefault();
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 3 month
                uRL_Chart = new URL_Chart();
                dObj = oSaleInvoices.Where(s =>s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-4)) && 
                    s.DueDate<= GetLastDayOfMonth(DateTime.Now.AddMonths(-3))).Sum(s => s.RemainMoney).Value;
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.y = dObj;
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-4)) &&
                    s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-3))).Select(s => s.ContactName).FirstOrDefault();
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 2 month  
                uRL_Chart = new URL_Chart();
                dObj = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-3)) &&
                    s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-2)))
                       .Sum(s => s.TotalMoney).Value;
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-3)) &&
                    s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-2))).Select(s => s.ContactName).FirstOrDefault();
                uRL_Chart.y = dObj;                
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //last 1 month
                uRL_Chart = new URL_Chart();
                dObj = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-2)) &&
                    s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-1)))
                       .Sum(s => s.TotalMoney).Value;
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.y = dObj;
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-2)) &&
                   s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(-1))).Select(s => s.ContactName).FirstOrDefault();
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //current month
                uRL_Chart = new URL_Chart();
                dObj = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-1)) &&
                   s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(0)))
                       .Sum(s => s.TotalMoney).Value;
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.y = dObj;
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now.AddMonths(-1)) &&
                   s.DueDate <= GetLastDayOfMonth(DateTime.Now.AddMonths(0))).Select(s => s.ContactName).FirstOrDefault();
                Obj.Add(uRL_Chart);
                uRL_Chart = null;

                //future month
                uRL_Chart = new URL_Chart();
                dObj = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now))
                       .Sum(s => s.TotalMoney).Value;
                dObj *= (iType == 1) ? (-1) : (1);
                uRL_Chart.y = dObj;
                uRL_Chart.name = oSaleInvoices.Where(s => s.DueDate > GetLastDayOfMonth(DateTime.Now)).Select(s => s.ContactName).FirstOrDefault();
                Obj.Add(uRL_Chart);
                uRL_Chart = null;
            }

            return Obj;
        }

        private List<URL_Chart> GetColumnMoneyPurchase(int iType)
        {
            var Obj = new List<URL_Chart>();
            double dObj = 0;
            var oPurchases = new GenericManager<Purchase>().Get().Where(s => s.Status == 2 || s.Status == 3);

            if (oPurchases != null)
            {
                var dateNow = DateTime.Now.Date.AddMonths(1);
                double dayUntilNow = dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;
                dateNow = DateTime.Now.Date;
                for (var i = 0; i <= dayUntilNow; i++)
                {
                    URL_Chart uRL_Chart = new URL_Chart();

                    dObj = GetMoneyValue(iType, i, oPurchases);

                    uRL_Chart.y = dObj;
                    uRL_Chart.url = "/Purchase/Paid?searchString=&type=" + iType + "&createdate=&duadate= " + dateNow.AddDays(i).Date.ToString("yyyy-MM-dd");

                    Obj.Add(uRL_Chart);
                    uRL_Chart = null;
                }
            }

            return Obj;
        
        }

        /// <summary>
        /// iStatus 2 - Approved;4-Paid
        /// </summary>
        /// <param name="iStatus">iStatus: 2 - Approved; 4 - Paid</param>
        /// <returns></returns>
        private List<URL_Chart> GetColumnMoneyPayRun(int iStatus)
        {
            List<URL_Chart> Obj = new List<URL_Chart>();
            double dayUntilNow=0;
            DateTime dateNow = DateTime.Now.Date;
            var oPurchase = new GenericManager<PayRun>().Get().Where(s => s.Status == iStatus);
            string sUrl = "/Purchase/AwaitingPayment?searchString=&type=" + iStatus + "&createdate=&duadate=";
            if (oPurchase != null)
            {
                dayUntilNow = dateNow.Subtract(dateNow.AddDays(-7).Date).TotalDays;
                dateNow = dateNow.AddMonths(1);
                dayUntilNow += dateNow.Subtract(dateNow.AddMonths(-1).Date).TotalDays;
                for (var i = -7; i <= dayUntilNow - 7; i++)
                {
                    URL_Chart uRL_Chart = new URL_Chart();

                    var obj = (from item in oPurchase
                            where item.PaymentDate == DateTime.Now.AddDays(i).Date
                            select item);

                    uRL_Chart.y = (iStatus == 4) ? (obj.Sum(s => s.Total) - obj.Sum(s => s.RemainMoney)).Value : obj.Sum(s => s.RemainMoney).Value;
                    
                    uRL_Chart.url = sUrl + DateTime.Now.AddDays(i).Date.ToString("yyyy-MM-dd");

                    Obj.Add(uRL_Chart);
                }
            }

            return Obj;
        }

        private LabelValue_StackedColumn SetValueStackedColumn(int iType)
        {
            LabelValue_StackedColumn obj = new LabelValue_StackedColumn();
            obj.SetValueDefault();
            obj.SetHeight = HeightDefault;
            switch (iType)
            {
                case 1:
                    obj.DataItems_1 = GetColumnMoneyComingOut(1).ToArray();
                    obj.DataItems_2 = GetColumnMoneyComingOut(2).ToArray();
                    obj.Categories = GetDayPurchaseRange().ToArray();
                    obj.FormatString = @"function() {
                    return this.series.name + ': ' + this.x + ': ' 
                    + Highcharts.numberFormat(this.y) + '<br/> <label style=" + "\"color: #048fc2;font-size: 10px;\"" + ">Click to view</label>' ;}";
                    obj.UrlName = "function(){window.location.href = this.options.id;}";
                    obj.SetName_1 = "Awaiting Payment";
                    obj.SetName_2 = "Paid";
                    break;
                case 2:
                    obj.DataItems_1 = GetColumnMoneySaleInvoice(0).ToArray();
                    obj.DataItems_2 = GetColumnMoneySaleInvoice(1).ToArray();
                    obj.Categories = GetMonthRange().ToArray();
                    obj.FormatString = @"function() {
                    return this.key + '<br/>'+
                    Highcharts.numberFormat(this.y) + ' '+ this.series.name;}";
                    obj.SetName_1 = "Due";
                    obj.SetName_2 = "Owing";
                    break;
                case 3:
                    obj.DataItems_1 = GetColumnMoneyPurchase(1).ToArray();
                    obj.DataItems_2 = GetColumnMoneyPurchase(2).ToArray();
                    obj.Categories = GetDayRange(3).ToArray();
                    obj.FormatString = @"function() {
                    return this.series.name + '<br/>'
                    + this.x + '<br/>'
                    + Highcharts.numberFormat(this.y);}";
                    obj.UrlName = "function(){window.location.href = this.options.id;}";
                    obj.SetName_1 = "Due";
                    obj.SetName_2 = "Paid";
                    break;
                case 4:
                    obj.DataItems_1 = GetColumnMoneyPayRun(2).ToArray();
                    obj.DataItems_2 = GetColumnMoneyPayRun(4).ToArray();
                    obj.Categories = GetDayRange(4).ToArray();
                    obj.FormatString = @"function() {
                    return this.series.name + '<br/>'
                    + this.x + '<br/>'
                    + Highcharts.numberFormat(this.y);}";
                    obj.UrlName = "function(){window.location.href = this.options.id;}";
                    obj.SetName_1 = "Due";
                    obj.SetName_2 = "Paid";
                    break;
            }
            return obj;
        }

        public ActionResult StackedColumn(int iType, string chartname)
        {
            var obj = SetValueStackedColumn(iType);
            Data data1 = new Data(obj.DataItems_1.Select(y => new DotNet.Highcharts.Options.Point { Color = GetBarColour(y.y, 1), Y = y.y, Id = y.url, Name = y.name }).ToArray());
            Data data2 = new Data(obj.DataItems_2.Select(y => new DotNet.Highcharts.Options.Point { Color = GetBarColour(y.y, 2), Y = y.y, Id = y.url, Name = y.name }).ToArray());
            Highcharts chart = new Highcharts(chartname)
                .SetXAxis(new XAxis { Categories = obj.Categories })
                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "" },                    
                })
                .InitChart(new Chart { DefaultSeriesType = ChartTypes.Column, Width = iType != 1 ? 939 : 463, Height = obj.SetHeight, BorderWidth = 1, BorderColor = System.Drawing.ColorTranslator.FromHtml("#C1C1C1"), BorderRadius = 0 })
                .SetTitle(new Title { Text = "" })
                .SetLegend(new Legend
                {                    
                    Enabled = false
                })
                .SetTooltip(new Tooltip
                {
                    Formatter = obj.FormatString
                })
                .SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        Stacking = Stackings.Normal,
                        DataLabels = new PlotOptionsColumnDataLabels
                        {
                            Enabled = false,
                            Color = Color.White
                        }
                    },
                    Series = new PlotOptionsSeries
                    {
                        Cursor = Cursors.Pointer,
                        Point = new PlotOptionsSeriesPoint
                        {
                            Events = new PlotOptionsSeriesPointEvents
                            {
                                Click = obj.UrlName
                            }
                        }
                    }
                })
                .SetSeries(new[]
                    {
                        new Series { Name = obj.SetName_1, Data = data1 },
                        new Series { Name = obj.SetName_2, Data = data2 },
                    });

            return View(chart);
        }

        #endregion Process Stacked Column
        
        #region Banks
        #region private function banking
        /// <summary>
        /// Sale invoice
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="iStatus"></param>
        private void SaleInvoiceMoneyInOut(BankResultCollection obj, int iStatus)
        {
            var oSaleInvoices = new GenericManager<SaleInvoice>().Get()
                .Select(x => new { x.Status, x.TotalMoney, x.DueDate, x.RemainMoney, x.Type });

            if (oSaleInvoices != null)
            {
                if (iStatus == 0)
                {
                    var temp = oSaleInvoices;
                    temp = temp.Where(x => x.Status == 0);
                    obj.DraftComingInCount = temp.Count();
                    var total = temp.Sum(x => x.TotalMoney).Value;
                    temp = null;

                    temp = oSaleInvoices;
                    temp = temp.Where(x => x.Status == 1);
                    total -= temp.Sum(x => x.TotalMoney).Value;
                    temp = null;
                    obj.TotalDraftComingIn = SetNegativeNumber(total.ToString(formatNumber));
                }
                else
                {
                    oSaleInvoices = oSaleInvoices.Where(x => x.Status == iStatus);
                    oSaleInvoices = (from item in oSaleInvoices
                                     where item.DueDate <= GetLastDayOfMonth(DateTime.Now) && item.RemainMoney != 0 && item.Type == 0
                                     select item);
                    obj.OverdueComingInCount = oSaleInvoices.Count();
                    obj.TotalOverdueComingIn = SetNegativeNumber(oSaleInvoices.Sum(x => x.TotalMoney).Value.ToString(formatNumber));
                }
            }
        }

        /// <summary>
        /// Purchase
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="iStatus"></param>
        private void PurchaseMoneyInOut(BankResultCollection obj, int iStatus)
        {
            if (iStatus == 0)
            {
                var oPurchases = new GenericManager<Purchase>().Get()
                .Select(x => new { x.ID, x.Code, x.Status, x.Type, x.TotalMoney }).Where(x => x.Status == iStatus);
                var temp = oPurchases;
                obj.DraftGoingOutCount = temp.Count();

                temp = temp.Where(x => x.Type == 0);
                var total = temp.Sum(x => x.TotalMoney).Value;
                temp = null;

                temp = oPurchases;
                temp = temp.Where(x => x.Type == 1);
                total -= temp.Sum(x => x.TotalMoney).Value;
                temp = null;
                obj.TotalDraftGoingOut = SetNegativeNumber(total.ToString(formatNumber));
            }
            else
            {
                var oPurchases = new GenericManager<Purchase>().Get().Select(x => new { x.ID, x.Code, x.Status, x.Type, x.TotalMoney, x.DueDate, x.RemainMoney }).Where(x => x.Status == iStatus);

                oPurchases = (from item in oPurchases
                              where item.DueDate <= GetLastDayOfMonth(DateTime.Now) && item.RemainMoney != 0 && item.Type == 0
                              select item);
                obj.OverdueGoingOutCount = oPurchases.Count();
                obj.TotalOverdueGoingOut = SetNegativeNumber(oPurchases.Sum(x => x.RemainMoney).Value.ToString(formatNumber));
            }
        }

        /// <summary>
        /// iStatus: 0 - Draft, 1 - Authorisation, 2 - Approve, 3 -  Awaiting Payment, 4 - Paid
        /// </summary>        
        private void ExpenseClaim(BankResultCollection obj, int iStatus, bool bCurrUser)
        {
            var ExpenseClaims = new GenericManager<ExpenseClaim>().Get().Where(x => x.Status == iStatus);

            var objId = bCurrUser ?
                TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.ID :
                TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.CompanyID;

            var userCurrentCompany = new GenericManager<User>().Get().Where(x => bCurrUser ? x.ID == objId : x.CompanyID == objId);
            var temp = (from u in userCurrentCompany
                        join e in ExpenseClaims on u.ID equals e.CreatedUserID                        
                        select new { u, e });

            var results = temp.GroupBy(x => x.e.CreatedUserID, x => new { x.u, x.e }, (key, g) => new { CreatedUserID = key, expenseClaims = g.ToList() });

                        
            if (!bCurrUser)
            {//all - authorisation - payment               
                switch (iStatus)
                {
                    case 0:
                        obj.All_CurrentClaim = results.Count();
                        obj.All_CurrentClaim_Total = SetNegativeNumber(temp.Sum(t => t.e.TotalMoney).ToString(formatNumber));
                        break;
                    case 1:
                        obj.AuthorisationClaim = results.Count();
                        obj.AuthorisationClaim_Total = SetNegativeNumber(temp.Sum(t => t.e.TotalMoney).ToString(formatNumber));
                        break;
                    case 3:
                        obj.AwaitingPaymentClaim = results.Count();
                        obj.AwaitingPaymentClaim_Total = SetNegativeNumber(temp.Sum(t => t.e.TotalMoney).ToString(formatNumber));
                        break;
                }
            }
            else
            {
                obj.Your_CurrentClaim_Total = SetNegativeNumber(temp.Sum(t => t.e.TotalMoney).ToString(formatNumber));
            }
        }

        private string SetNegativeNumber(string sInput)
        {
            return sInput = (sInput.IndexOf("-") > -1) ? sInput.Replace("-", "(") + ")" : sInput;
        }
        #endregion private function banking
        /// <summary>
        /// View detail banking
        /// </summary>
        /// <returns></returns>
        public ActionResult BankingDetail()
        {
            var bc = new BankResultCollection();
            BankResult obj = null;
            var Banks = new GenericManager<BankAccount>().Get()
                .Select(x => new { x.ID, x.BankStatements, x.AccountTrans, x.AccoutName, x.AccoutNum, x.ShowOnDashboard })
                .Where(x => x.ShowOnDashboard == 1).ToList();

            if (Banks != null)
            {
                bc.BankResults = new List<BankResult>();
                foreach (var item in Banks)
                {
                    var bankStatementDetails = item.BankStatements.SelectMany(x => x.BankStatementDetails);
                    obj = new BankResult();
                    obj.BankID = item.ID;
                    obj.AccoutName = item.AccoutName;
                    obj.AccoutNum = item.AccoutNum;
                    var total = item.BankStatements.SelectMany(x => x.BankStatementDetails).Sum(y => y.Received).Value.ToString(formatNumber);
                    obj.Total = SetNegativeNumber(total);
                    obj.ReconcileCount = item.BankStatements.SelectMany(x => x.BankStatementDetails).Where(w => w.Status == 0).Count();
                    var totalBalance = (item.AccountTrans.Sum(x => x.Received) - item.AccountTrans.Sum(x => x.Spent)).Value.ToString(formatNumber);
                    obj.TotalBalance = SetNegativeNumber(totalBalance);
                    obj.MaxDate = (bankStatementDetails.Count() > 0) ? item.BankStatements.SelectMany(x => x.BankStatementDetails).Max(z => z.Date).Value.ToString("d MMM yyyy") : string.Empty;
                    bc.BankResults.Add(obj);
                    obj = null;
                }

                SaleInvoiceMoneyInOut(bc, 0);
                SaleInvoiceMoneyInOut(bc, 2);

                PurchaseMoneyInOut(bc, 0);
                PurchaseMoneyInOut(bc, 2);

                ExpenseClaim(bc, 0, true);
                ExpenseClaim(bc, 0, false);
                ExpenseClaim(bc, 1, false);
                ExpenseClaim(bc, 3, false);
            }
            return View(bc);
        }
        #endregion Banks
    }
}