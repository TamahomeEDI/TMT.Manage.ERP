using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Collections.Generic;
using TMT.ERP.Services;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;
using PagedList;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace TMT.ERP.Controllers
{
    public class SaleInvoiceController : BaseController
    {
        private ErpEntities db = new ErpEntities();
        private int idForPayment = 0;
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static int TypeSearch = 0;
        public static string SortDateAsc;
        SelectList _ListDateSetting = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Weeks(s)"},
            new SelectListItem {Value = "1", Text = "Month(s)"}
        }, "Value", "Text");

        SelectList _ListDateSettingDuaDate = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "of the following month"},
            new SelectListItem {Value = "1", Text = "day(s) after the invoice date"},
            new SelectListItem {Value = "2", Text = "of the current month"}
        }, "Value", "Text");


        SelectList _ListDate = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Any date"},
            new SelectListItem {Value = "1", Text = "Start date"},
            new SelectListItem {Value = "2", Text = "End date"}
        }, "Value", "Text");

        SelectList _ListAction = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Invoice"},
            new SelectListItem {Value = "1", Text = "Repeating Invoice"},
            new SelectListItem {Value = "2", Text = "Create Note"},
            new SelectListItem {Value = "3", Text = "Invoice contact group"},
            new SelectListItem {Value = "4", Text = "Add contact group"}
        }, "Value", "Text");

        private List<SelectListItem> InitGoods()
        {
            var goodList = new SelectList(db.Goods, "ID", "Code");
            List<SelectListItem> goodItems = goodList.ToList();
            goodItems.Insert(0, (new SelectListItem { Text = "+ Add new inventory...", Value = "0" }));
            //goodItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            return goodItems;
        }

        private object CreateGoodList(int stockID)
        {
            var manager = new GenericManager<ActualGoodInStock>();
            var goodItems = manager.Get().Where(x => x.StockID == stockID && x.Good.ProductTypeID == 2).Select(x => new { Text = x.Good.Code, Value = x.GoodID }).Distinct().ToList();
            //goodItems.Insert(0, new { Text = "+ Add new inventory...", Value = "0" });
            return goodItems;
        }

        private object CreateGoodList()
        {
            var manager = new GenericManager<ActualGoodInStock>();
            var goodItems = manager.Get().Select(x => new { Text = x.Good.Code, Value = x.GoodID }).Distinct().ToList();
            return goodItems;
        }

        private object CreateCurrencyList()
        {
            var manager = new GenericManager<Currency>();
            var currencyItems = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            return currencyItems;
        }

        private List<SelectListItem> InitTaxRates()
        {
            var taxRates = new SelectList(db.TaxRates, "ID", "Name");
            List<SelectListItem> taxRateItems = taxRates.ToList();
            return taxRateItems;
        }
        private List<SelectListItem> InitAccount()
        {
            var accountList = new SelectList(db.Accounts, "ID", "Name");
            List<SelectListItem> accountItems = accountList.ToList();
            //accountItems.Insert(0, (new SelectListItem { Text = "+ Add new account...", Value = "0" }));
            //accountItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            return accountItems;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.BillCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.SaleOrder));
            //ViewBag.CurrencyID = CreateCurrencyList();
            //ViewData["CurrencyID"] = new SelectList(new GenericManager<Currency>().Get().Select(x => new { Name = x.Code + "-" + x.Name, ID = x.ID }).Distinct().ToList(), "ID", "Name", Constant.DefaultCurrency);
            ViewBag.CurrencyID = new SelectList(new GenericManager<Currency>().Get().Select(x => new { ID = x.ID, Name = x.Code + "-" + x.Name }).Distinct().ToList(), "ID", "Name", Constant.DefaultCurrency);
            ViewBag.GoodID = CreateGoodList();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();
            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            //End
            return View();
        }
        public ActionResult CreateRepeatingInvoice()
        {
            var setting = db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 0);
            if (setting != null)
            {
                ViewBag.BillDefaultDuaDate = setting.BillDefaultDueDate;
                ViewBag.SaleInvoiceDefaultDueDate = setting.SaleInvoiceDefaultDueDate;
            }
            ViewData["ListDateSetting"] = _ListDateSetting;
            ViewData["ListDateSettingDuaDate"] = _ListDateSettingDuaDate;
            ViewBag.BillCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.SaleOrder));
            ViewBag.CurrencyID = CreateCurrencyList();
            ViewBag.GoodID = CreateGoodList();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;

            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            //End

            return View();
        }
        public ActionResult EditRepeating(int id = 0)
        {
            ViewData["ListDateSetting"] = _ListDateSetting;
            ViewData["ListDateSettingDuaDate"] = _ListDateSettingDuaDate;
            ViewBag.objRepeat = db.Repeatings.FirstOrDefault(x => x.SaleInvoiceID == id);
            SaleInvoice saleInvoice = db.SaleInvoices.Find(id);
            if (saleInvoice == null)
            {
                return HttpNotFound();
            }
            //ViewData["Currency"] = new SelectList(db.Currencies.ToList(), "ID", "Name", saleInvoice.CurrencyID);
            ViewBag.CurrencyID = CreateCurrencyList();
            ViewData["Stock"] = new SelectList(db.Stocks.ToList(), "ID", "Name", saleInvoice.StockID);
            ViewBag.GoodID = CreateGoodList();//InitGoods();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.SaleOrder);
            ViewBag.NoteFromID = saleInvoice.ID;

            //For function Note
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.SaleRepeating);
            ViewBag.NoteFromID = saleInvoice.ID;
            //End

            return View(saleInvoice);
        }

        public ActionResult Edit(int id = 0)
        {
            SaleInvoice saleinvoice = db.SaleInvoices.Find(id);
            if (saleinvoice == null)
            {
                return HttpNotFound();
            }
            //ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name");

            ViewData["Currency"] = new SelectList(db.Currencies.ToList(), "ID", "Name", saleinvoice.CurrencyID);
            ViewData["Stock"] = new SelectList(db.Stocks.ToList(), "ID", "Name", saleinvoice.StockID);
            ViewBag.GoodID = CreateGoodList();//InitGoods();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();

            //For function Note
            if (saleinvoice.Type == 1)
            {
                ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.SaleCreditNote);
            }
            else
            {
                ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.SaleOrder);
            }
            ViewBag.NoteFromID = saleinvoice.ID;
            //End
            return View(saleinvoice);
        }
        public ActionResult CreateCreditNote()
        {
            var item = db.SaleInvoiceSettings.Where(x => x.Type == 0).FirstOrDefault();
            var billNo = "CN-00001";
            if (item != null)
                billNo = item.CreditNotePrefix + item.NextNumber;

            ViewBag.BillCode = billNo;
            //ViewBag.StockID = new SelectList(db.Stocks, "ID", "Name");
            ViewBag.CurrencyID = CreateCurrencyList();//new SelectList(db.Currencies, "ID", "Name");
            ViewBag.GoodID = CreateGoodList();//InitGoods();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();

            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            //End
            return View();
        }
        public ActionResult Update()
        {
            return View();
        }
        public PartialViewResult AddNewLine()
        {
            ViewBag.GoodID = CreateGoodList();//InitGoods();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();
            return PartialView("_AddNewLine");
        }
        #region Create by HungVT
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public int SetTypeSearch(string typeSearch)
        {
            //0- Any Date - True
            //1- Transaction Date - True
            //2- Due Date - True
            //3- Paid Date - True
            //4- Next invoice Date - True
            //5- End Date - True
            //6- Planned Date - True
            TypeSearch = Convert.ToInt32(typeSearch);
            return TypeSearch;
        }
        public string SetDateAsc(string value)
        {
            SortDateAsc = value;
            return SortDateAsc;
        }
        public ViewResult All(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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

            var invoice = db.SaleInvoices.Where(x => x.Status >= 0).OrderByDescending(x => x.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                invoice = SearchInvoices(invoice, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                invoice = SearchInvoices(invoice, "", TypeSearch, createdate, duadate).ToList();
            }
            if (SortDateAsc == "asc")
            {
                invoice = invoice.OrderBy(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            if (SortDateAsc == "desc")
            {
                invoice = invoice.OrderByDescending(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }


            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, float>(GetTotalMoneyOfSaleInvoice);

            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(invoice.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult Draft(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            var invoices = db.SaleInvoices.Where(i => i.Status == 0 || i.Status == 5).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = SearchInvoices(invoices, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                invoices = SearchInvoices(invoices, "", TypeSearch, createdate, duadate).ToList();
            }
            if (SortDateAsc == "asc")
            {
                invoices = invoices.OrderBy(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            if (SortDateAsc == "desc")
            {
                invoices = invoices.OrderByDescending(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.DatePaid = new Func<int, DateTime>(GetDatePaid);
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Awaiting(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            var invoices = db.SaleInvoices.Where(i => i.Status == 1).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = SearchInvoices(invoices, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                invoices = SearchInvoices(invoices, "", TypeSearch, createdate, duadate).ToList();
            }
            if (SortDateAsc == "asc")
            {
                invoices = invoices.OrderBy(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            if (SortDateAsc == "desc")
            {
                invoices = invoices.OrderByDescending(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            ViewBag.TotalMoneySale = new Func<int, float>(GetTotalMoneyOfSaleInvoice);
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult AwaitingPayment(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            var invoices = db.SaleInvoices.Where(i => i.Status == 2).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = SearchInvoices(invoices, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                invoices = SearchInvoices(invoices, "", TypeSearch, createdate, duadate).ToList();
            }
            if (SortDateAsc == "asc")
            {
                invoices = invoices.OrderBy(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            if (SortDateAsc == "desc")
            {
                invoices = invoices.OrderByDescending(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.DatePaid = new Func<int, DateTime>(GetDatePaid);

            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            TypeSearch = 0;
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Paid(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            var invoices = db.SaleInvoices.Where(i => i.Status == 3).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = SearchInvoices(invoices, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                invoices = SearchInvoices(invoices, "", TypeSearch, createdate, duadate).ToList();
            }
            if (SortDateAsc == "asc")
            {
                invoices = invoices.OrderBy(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            if (SortDateAsc == "desc")
            {
                invoices = invoices.OrderByDescending(x => x.CreatedDate).ToList();
                SortDateAsc = null;
            }
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.DatePaid = new Func<int, DateTime>(GetDatePaid);
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Repeating(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            var invoices = db.SaleInvoices.Where(i => i.Status == 4).OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                invoices = SearchInvoices(invoices, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                if (type == -1)
                {
                    TypeSearch = -1;
                }
                invoices = SearchInvoices(invoices, "", TypeSearch, createdate, duadate).ToList();
                TypeSearch = 0;
            }
            ViewBag.ObjRepeating = new Func<int, Repeating>(GetObjectRepeating);
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(invoices.ToPagedList(pageNumber, pageSize));
        }
        public IList<SaleInvoice> SearchInvoices(IList<SaleInvoice> saleInvoices, string sStr, int? type, string createdate, string enddate)
        {
            var invoices = new List<SaleInvoice>();
            DateTime sDateTime = string.IsNullOrEmpty(createdate) ? Convert.ToDateTime("01/01/0001") : Convert.ToDateTime(createdate);
            DateTime eDateTime = string.IsNullOrEmpty(enddate) ? Convert.ToDateTime("01/01/0001") : Convert.ToDateTime(enddate);
            List<SaleInvoice> invoicesName;
            List<SaleInvoice> invoicesRef;
            List<SaleInvoice> invoicesStartDateCreatDate;
            List<SaleInvoice> invoicesStartDateDueDate;
            List<SaleInvoice> invoicesEndDateCreatDate;
            List<SaleInvoice> invoicesEndDateDueDate;
            //List<SaleInvoice> invoicesSPaidDate;
            //List<SaleInvoice> invoicesEPaidDate;
            //0- Any Date 1- Transaction Date 2- Due Date 3- Paid Date 4- Next invoice Date 5- End Date 6- Planned Date 
            #region

            #endregion
            #region AnyDate
            if (type == 0)
            {

                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }

                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateCreatDate = saleInvoices.Where(x => x.CreatedDate >= sDateTime).ToList();
                    invoicesStartDateDueDate = saleInvoices.Where(x => x.DueDate >= sDateTime).ToList();
                    saleInvoices = invoicesStartDateCreatDate.Union(invoicesStartDateDueDate).ToList();
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateCreatDate = saleInvoices.Where(x => x.CreatedDate <= eDateTime).ToList();
                    invoicesEndDateDueDate = saleInvoices.Where(x => x.DueDate <= eDateTime).ToList();
                    saleInvoices = invoicesEndDateCreatDate.Union(invoicesEndDateDueDate).ToList();
                }
                invoices = saleInvoices.ToList();
            }
            #endregion AnyDate
            #region Transaction Date
            if (type == 1)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateCreatDate = saleInvoices.Where(x => x.CreatedDate >= sDateTime).ToList();
                    saleInvoices = invoicesStartDateCreatDate;
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateCreatDate = saleInvoices.Where(x => x.CreatedDate <= eDateTime).ToList();
                    saleInvoices = invoicesEndDateCreatDate;
                }
                invoices = saleInvoices.ToList();
            }
            #endregion
            #region Due Date
            if (type == 2)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateDueDate = saleInvoices.Where(x => x.DueDate >= sDateTime).ToList();
                    saleInvoices = invoicesStartDateDueDate;
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateDueDate = saleInvoices.Where(x => x.DueDate <= eDateTime).ToList();
                    saleInvoices = invoicesEndDateDueDate;
                }
                invoices = saleInvoices.ToList();
            }
            #endregion
            #region Paid Date
            if (type == 3)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }

                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {
                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Payments.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DatePaid >= sDateTime
                                       select item).ToList();
                        saleInvoices = invoice;
                    }
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {
                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Payments.LastOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DatePaid >= sDateTime
                                       select item).ToList();
                        saleInvoices = invoice;
                    }
                }
                return saleInvoices;
            }
            #endregion
            #region Next Invoice Date
            if (type == 4)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {
                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DateRun >= sDateTime
                                       select item).ToList();
                        saleInvoices = invoice;
                    }

                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {
                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DateRun <= eDateTime
                                       select item
                                      ).ToList();
                        saleInvoices = invoice;
                    }
                }
                return saleInvoices;
            }
            #endregion
            #region End Date
            if (type == 5)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = saleInvoices.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = saleInvoices.Where(x => x.Reference.Contains(sStr)).ToList();
                    saleInvoices = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {
                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.EndDate >= sDateTime
                                       select item).ToList();
                        saleInvoices = invoice;
                    }

                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (saleInvoices != null)
                    {

                        var invoice = (from item in saleInvoices
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.EndDate <= eDateTime
                                       select item
                                      ).ToList();
                        saleInvoices = invoice;
                    }
                }
                if (saleInvoices != null) invoices = saleInvoices.ToList();
            }
            #endregion
            #region Planned Date
            if (type == 6)
            {

            }
            #endregion
            return invoices;
        }
        [HttpPost]
        public string UpdateStatus(int id, int status)
        {
            string result = "success";
            try
            {
                SaleInvoice saleInvoice = db.SaleInvoices.Find(id);
                saleInvoice.Status = status;
                if (ModelState.IsValid)
                {
                    db.Entry(saleInvoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;

            }
            return result;
        }
        public string SubmitForApproval(string[] saleId, int status)
        {
            string result = "success";
            try
            {
                // Create StockOutCard if Approved SaleInvoice
                if (status == 2)
                {
                    result = CreateInvoiceStockOut(saleId, AppContext.RequestVariables.CurrentUser.ID);
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "success";
                    }
                    foreach (var item in saleId)
                    {
                        int oSaleID = Convert.ToInt32(item);
                        string strStatus = Utility.CreateAccountTransaction(0, oSaleID, null, null);
                        if (strStatus.Equals("1"))
                        {
                            SaleInvoice saleInvoice = db.SaleInvoices.Find(oSaleID);
                            saleInvoice.Status = status;
                            db.Entry(saleInvoice).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (var item in saleId)
                    {
                        int oSaleID = Convert.ToInt32(item);
                        var oSaleInvoice = db.SaleInvoices.Find(oSaleID);
                        oSaleInvoice.Status = status;
                        db.Entry(oSaleInvoice).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        [HttpPost]
        public ActionResult DeleteConfirmed(int id)
        {
            SaleInvoice saleInvoice = db.SaleInvoices.Find(id);
            db.SaleInvoices.Remove(saleInvoice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult AwaitingPaymentDetails(int id)
        {
            ViewBag.BankAccountID = new SelectList(db.BankAccounts, "ID", "BankName");
            #region Set saleinvoiceId for Payment
            idForPayment = (int)id;
            ViewBag.IDPayment = idForPayment;
            #endregion
            #region Information Bill
            double sub = 0;
            double total = 0;
            var saleInvoidDetails = db.SaleInvoiceDetails.Where(s => s.SaleInvoiceID == id).ToList();
            foreach (var x in saleInvoidDetails)
            {
                sub += Convert.ToDouble(x.Subtotal);
                total += Convert.ToDouble(x.TotalMoney);
            }
            ViewBag.Sub = sub;
            ViewBag.Total = total;
            #endregion
            #region Information Contact
            var saleInvoice = db.SaleInvoices.Find(id);
            ViewBag.objSale = saleInvoice;
            //User user = db.Users.Find(saleInvoice.CreatedUserID);
            //ViewBag.EmployeeID = user.ID;            
            #endregion
            #region Information Payment

            Payment payment = db.Payments.FirstOrDefault(p => p.SaleInvoiceID == id);
            ViewBag.AmountDate = payment == null ? DateTime.Now : payment.DatePaid;

            var paymentList = db.Payments.Where(p => p.SaleInvoiceID == id).ToList();
            ViewBag.PayList = paymentList;
            double paymentTotalMoney = 0;
            if (paymentList.Count > 0)
            {
                paymentTotalMoney += paymentList.Sum(itemPay => itemPay.TotalMoney);
            }
            ViewBag.LessPayment = paymentTotalMoney;
            ViewBag.STCL = total - paymentTotalMoney;

            #endregion
            return View(saleInvoidDetails);
        }
        public ActionResult PaidDetails(int id)
        {

            #region Set saleinvoiceId for Payment
            idForPayment = (int)id;
            ViewBag.IDPayment = idForPayment;
            #endregion
            #region Information Bill
            double sub = 0;
            double total = 0;
            var saleInvoidDetails = db.SaleInvoiceDetails.Where(s => s.SaleInvoiceID == id).ToList();
            foreach (var x in saleInvoidDetails)
            {
                sub += Convert.ToDouble(x.Subtotal);
                total += Convert.ToDouble(x.TotalMoney);
            }
            ViewBag.Sub = sub;
            ViewBag.Total = total;
            #endregion
            #region Information Contact
            var saleInvoice = db.SaleInvoices.Find(id);
            ViewBag.objSale = saleInvoice;
            #endregion
            #region Information Payment

            Payment payment = db.Payments.FirstOrDefault(p => p.SaleInvoiceID == id);
            ViewBag.AmountDate = payment == null ? DateTime.Now : payment.DatePaid;

            var paymentList = db.Payments.Where(p => p.SaleInvoiceID == id).ToList();
            ViewBag.PayList = paymentList;
            double paymentTotalMoney = 0;
            if (paymentList.Count > 0)
            {
                paymentTotalMoney += paymentList.Sum(itemPay => itemPay.TotalMoney);
            }
            ViewBag.LessPayment = paymentTotalMoney;
            ViewBag.STCL = total - paymentTotalMoney;

            #endregion
            return View(saleInvoidDetails);
        }
        [HttpPost]
        public string AddPament(string id, string paymentTo, string money, string datepaid, string paidTo, int? type, string reference)
        {
            string result = "success";
            try
            {
                if (ModelState.IsValid)
                {
                    var bank = db.BankAccounts.FirstOrDefault();
                    int saleId = Convert.ToInt32(id);
                    var oSaleInvoices = db.SaleInvoices.Find(saleId);
                    var oCurrency = db.Currencies.FirstOrDefault();
                    if (oSaleInvoices.CurrencyID != null)
                    {
                        oCurrency = oSaleInvoices.Currency;
                    }
                    var oPayment = new Payment();
                    oPayment.SaleInvoiceID = Convert.ToInt32(oSaleInvoices.ID);
                    oPayment.PaymetnTo = paymentTo ?? "No data";
                    oPayment.TotalMoney = Convert.ToDouble(money);
                    oPayment.CreatedDate = DateTime.Now;
                    oPayment.CurrencyID = oCurrency.ID;
                    oPayment.DatePaid = datepaid != null ? Convert.ToDateTime(datepaid) : DateTime.Now;
                    oPayment.PayToAccount = paidTo != null ? Convert.ToInt32(paidTo) : bank.ID;
                    oPayment.Reference = reference;
                    db.Payments.Add(oPayment);
                    //Create Transaction for Payment
                    string status = Utility.CreateAccountTransaction(0, Int32.Parse(id), oPayment, type);
                    db.SaveChanges();
                }

                #region Process when TotalMoney > LessPayment or TotalMonet < LessPayment

                int saleinvoiceID = Convert.ToInt32(id);
                var saleinvoiceList = db.SaleInvoiceDetails.Where(i => i.SaleInvoiceID == saleinvoiceID);
                var paymentList = db.Payments.Include(p => p.SaleInvoice).Where(p => p.SaleInvoiceID == saleinvoiceID).ToList();

                double totalInvoices = Enumerable.Sum(saleinvoiceList, invoid => Convert.ToDouble(invoid.TotalMoney));
                double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));

                if (totalPayment >= totalInvoices)
                {
                    SaleInvoice sale = db.SaleInvoices.Find(Convert.ToInt32(id));
                    sale.Status = 3;
                    db.Entry(sale).State = EntityState.Modified;
                    db.SaveChanges();
                }

                #endregion
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public ActionResult Dashboard()
        {
            var x = db.SaleInvoices;

            ViewBag.Draft = x.Count(s => s.Status == 0 | s.Status == 5);
            ViewBag.Awaiting = x.Count(s => s.Status == 1);
            ViewBag.AwaitingPay = x.Count(s => s.Status == 2);
            ViewBag.OverDue = x.Count(s => s.Status == 2 && DateTime.Now > s.DueDate);


            var invoiceDraftCN = x.Where(y => y.Status == 0 | y.Status == 5 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceDraft = x.Where(z => z.Status == 0 | z.Status == 5 && z.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MDraft = invoiceDraft - invoiceDraftCN;

            var invoiceAwaitCN = x.Where(y => y.Status == 1 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceAwait = x.Where(y => y.Status == 1 && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaiting = invoiceAwait - invoiceAwaitCN;

            var invoiceAwaitPaymentCN = x.Where(y => y.Status == 2 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceAwaitPayment = x.Where(y => y.Status == 2 && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaitingPay = invoiceAwaitPayment - invoiceAwaitPaymentCN;


            var invoiceOverDueCN = x.Where(y => y.Status == 2 && DateTime.Now > y.DueDate && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceOverDue = x.Where(y => y.Status == 2 && DateTime.Now > y.DueDate && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MOverDue = invoiceOverDue - invoiceOverDueCN;

            return View();
        }
        public ActionResult ViewBill(int id, string totalmoney)
        {
            var payment = db.Payments.Find(id);
            ViewBag.ContactName = payment.SaleInvoice.ContactName;
            ViewBag.Date = payment.SaleInvoice.CreatedDate;
            ViewBag.DueDate = payment.SaleInvoice.DueDate;
            var saleInvoidDetails =
                db.SaleInvoiceDetails.Where(s => s.SaleInvoiceID == payment.SaleInvoice.ID).ToList();
            ViewBag.Total = saleInvoidDetails.Sum(item => Convert.ToDouble(item.TotalMoney));
            string goodCode = saleInvoidDetails.Aggregate("", (current, x) => current + (x.Good.Code + "-"));
            ViewBag.GoodCode = goodCode;
            ViewBag.TongTien = totalmoney;
            return View(payment);
        }
        public Repeating GetObjectRepeating(int saleInvoiceId)
        {
            var obj = db.Repeatings.FirstOrDefault(x => x.SaleInvoiceID == saleInvoiceId);
            return obj;

        }
        public float GetTotalMoneyPaid(int saleinvoiceId)
        {
            var listObj = db.Payments.Where(x => x.SaleInvoiceID == saleinvoiceId).ToList();
            float? total = float.Parse(listObj.Sum(x => x.TotalMoney).ToString());
            float z = total ?? 0;
            return z;
        }
        public DateTime GetDatePaid(int saleinvoiceId)
        {
            var objPaid =
                db.Payments.Where(x => x.SaleInvoiceID == saleinvoiceId).OrderByDescending(x => x.ID).FirstOrDefault();
            DateTime date = objPaid != null ? objPaid.DatePaid : DateTime.Now;
            return date;
        }
        public float GetTotalMoneyOfSaleInvoice(int saleinvoiceId)
        {
            var listObj = db.SaleInvoiceDetails.Where(x => x.SaleInvoiceID == saleinvoiceId).ToList();
            float? total = float.Parse(listObj.Sum(x => x.Price * x.Quantity).ToString());
            float z = total ?? 0;
            return z;
        }
        #endregion

        #region allocate creditnote
        [HttpPost]
        public String AlloCreditNote(int? CreditNoteID, string creditNoteDetail)
        {
            object result = null;
            dynamic array = JsonConvert.DeserializeObject(creditNoteDetail);
            var manager = new GenericManager<SaleInvoice>();
            var creditNote = manager.FindById(CreditNoteID);
            double amountToCreditTotal = 0;

            foreach (var item in array)
            {
                int saleInvoiceID = item.saleInvoiceID;
                double amountToCredit = item.amountToCredit;
                amountToCreditTotal += amountToCredit;
                SaleInvoice oSaleInvoice = manager.FindById(saleInvoiceID);
                if (oSaleInvoice != null)
                {
                    DecreaseAmountInvoice(oSaleInvoice, amountToCredit);
                }
            }
            // Decrease creditNote
            DecreaseAmountInvoice(creditNote, amountToCreditTotal);
            manager.Save();
            result = creditNote.Status;
            return serializer.Serialize(result);
        }

        private void DecreaseAmountInvoice(SaleInvoice oSaleInvoice, double amountToCredit)
        {
            oSaleInvoice.TotalMoney = oSaleInvoice.TotalMoney - amountToCredit;
            if (oSaleInvoice.TotalMoney == 0)
            {
                oSaleInvoice.Status = 3;
            }
            Utility.CreateAccountTransaction(0, oSaleInvoice.ID, null, null);
        }

        #endregion

        #region show view allocate credit screen
        public ActionResult AllocCreditBalance(int id = 0)
        {
            var manager = new GenericManager<SaleInvoice>();
            var creditNote = manager.FindById(id);
            if (creditNote == null)
            {
                return HttpNotFound();
            }

            #region title
            ViewBag.CreditNoteID = id;
            ViewBag.CreditNoteCode = creditNote.Code;
            ViewBag.OutStandingCreditBalance = creditNote.TotalMoney;
            #endregion

            List<int> lsGoodItems = new List<int>();
            List<int> lsSaleID = new List<int>();
            lsGoodItems = creditNote.SaleInvoiceDetails.Select(i => i.GoodID).Distinct().ToList();
            int? contactID = creditNote.ContactID;
            var saleInvoiceList = manager.Get().Where(x => x.ContactID == contactID && x.Type == 0 && x.Status == 2).ToList();
            List<SaleInvoice> lsInvoice = new List<SaleInvoice>();

            if (saleInvoiceList.Count > 0)
            {
                foreach (var item in saleInvoiceList)
                {
                    var saleDetails = item.SaleInvoiceDetails.Where(y => lsGoodItems.Contains(y.GoodID)).ToList();
                    foreach (var itemDetail in saleDetails)
                    {
                        if (!lsSaleID.Contains(item.ID))
                        {
                            lsSaleID.Add(item.ID);
                            lsInvoice.Add(item);
                        }
                    }

                }
                return View("AllocateCreditNote", lsInvoice);
            }
            else
            {
                return RedirectToAction("AwaitingPaymentDetails", "SaleInvoice", new { id = id });
            }

        }

        #endregion

        #region Update 02-01-2014
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
        public string CreateInvoiceStockOut(string[] saleInvoiceId, int userId)
        {
            string result = string.Empty;
            try
            {
                var manager = new GenericManager<StockOutCard>();
                var managerSale = new GenericManager<SaleInvoice>();
                var managerStock = new GenericManager<Stock>();
                var managerGood = new GenericManager<Good>();
                var managerUOM = new GenericManager<UOM>();
                foreach (var x in saleInvoiceId)
                {
                    int oSaleID = Convert.ToInt32(x);
                    var oSaleInvoice = managerSale.FindById(oSaleID);
                    // oSaleInvoice.Type==1 : Invoice is Credit Note . No create StockoutCard 
                    // oSaleInvoice.Type==0 : Inoivce . Create create StockoutCard
                    if (oSaleInvoice.Type == 0)
                    {

                        var oStockOutCard = new StockOutCard();
                        oStockOutCard.Code = GetCodeStockOut();
                        oStockOutCard.CreatedUserID = userId;
                        oStockOutCard.ApprovalUserID = userId;
                        var oStock = managerStock.Get().FirstOrDefault();
                        if (oStock != null)
                            oStockOutCard.StockID = oStock.ID; //  oSaleInvoice.StockID;
                        oStockOutCard.CreatedDate = DateTime.Now;
                        oStockOutCard.TargetID = oSaleInvoice.ID;
                        oStockOutCard.Type = 0;
                        oStockOutCard.Receiver = oSaleInvoice.Description ?? "None";
                        oStockOutCard.Status = 0;
                        oStockOutCard.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                        manager.Add(oStockOutCard);

                        result += oStockOutCard.Code + ",";

                        var ooSaleInvoiceDetails = oSaleInvoice.SaleInvoiceDetails.ToList();
                        foreach (var item in ooSaleInvoiceDetails)
                        {
                            var oStockOutCardsDetail = new StockOutCardsDetail();
                            oStockOutCardsDetail.StockOutCardID = oStockOutCard.ID;
                            var good = managerGood.FindById(item.GoodID);
                            oStockOutCardsDetail.GoodID = good.ID;
                            var uom = managerUOM.FindById(good.UOMID);
                            oStockOutCardsDetail.UOMID = uom.ID;
                            oStockOutCardsDetail.Quantity = Convert.ToInt32(item.Quantity);
                            oStockOutCardsDetail.DateOut = DateTime.Now;
                            oStockOutCardsDetail.Price = good.OutputPrice;
                            oStockOutCardsDetail.TotalMoney = item.Quantity * good.OutputPrice;
                            oStockOutCard.StockOutCardsDetails.Add(oStockOutCardsDetail);
                        }
                        manager.Save();
                    }
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
            if (!string.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        #endregion
        #region OverPayment
        public string OverPayment()
        {
            var manager = new GenericManager<AccountFeature>();
            var managerSale = new GenericManager<SaleInvoice>();
            var oAccountFeature = manager.Get().FirstOrDefault(x => x.Type == 0);

            if (oAccountFeature != null)
            {
                var oSaleAccountID = oAccountFeature.AccountID;

                var oAccountTrans = new AccountTran();
                oAccountTrans.AccountID = oSaleAccountID;
                oAccountTrans.Date = DateTime.Now;
                oAccountTrans.Reference = "";
                //oAccountTrans.
            }
            return "success";
        }
        #endregion
    }
}