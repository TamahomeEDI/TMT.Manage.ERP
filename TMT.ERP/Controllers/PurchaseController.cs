using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models;
using TMT.ERP.Models.Lookups;
using TMT.ERP.BusinessLogic.Managers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using TMT.ERP.Services;
using TMT.ERP.Commons;
using TMT.ERP.BusinessLogic.Utils;
using System.Data.Entity.Validation;
using System.Diagnostics;


namespace TMT.ERP.Controllers
{
    public class PurchaseController : BaseController
    {
        private int idForPayment = 0;
        public static int TypeSearch = 0;
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        User currentUser = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        private readonly GenericManager<Note> _manager = new GenericManager<Note>();
        private ErpEntities db = new ErpEntities();
        // For ddlist
        SelectList _ListAction = new SelectList(new[]{ 
            new SelectListItem {Value = "0", Text = "Bill"},
             new SelectListItem {Value = "1", Text = "Repeating bill"},
              new SelectListItem {Value = "2", Text = "Credit note"},
        }, "Value", "Text");

        SelectList _ListActionBill = new SelectList(new[]{ 
            new SelectListItem {Value = "0", Text = "New Bill"},
             new SelectListItem {Value = "1", Text = "New Repeating bill"},             
        }, "Value", "Text");

        SelectList _ListActionCreditNote = new SelectList(new[]{ 
            new SelectListItem {Value = "0", Text = "New Credit Note"},          
        }, "Value", "Text");

        SelectList _ListDateSetting = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Month(s)"},
            new SelectListItem {Value = "1", Text = "Week(s)"}
        }, "Value", "Text");

        SelectList _ListDateSettingDuaDate = new SelectList(new[]{
            new SelectListItem{Value = "0" , Text ="of the following month"},
            new SelectListItem {Value = "1", Text = "day(s) after the invoice date"},
            new SelectListItem {Value = "2", Text = "of the current month"}}, "Value", "Text");
        //End ddlist
        // GET: /Purchase/

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
        public ActionResult Index(int? tabNum)
        {
            resetTab();
            PopulateSearchWithinDropDownList();
            PopulateTaxDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User);
            ViewBag.TypeStatus = tabNum;
            populateActionIndexDropDown();
            return View(purchases.ToList());
        }

        private void populateActionIndexDropDown()
        {
            List<SelectListItem> actionBillItems = _ListActionBill.ToList();
            // actionBillItems.Insert(0, (new SelectListItem { Text = "+ New Bill", Value = "-1", Selected = true }));
            ViewBag.ActionBill = actionBillItems;


            List<SelectListItem> actionCreaditItems = _ListActionCreditNote.ToList();
            actionCreaditItems.Insert(0, (new SelectListItem { Text = "+ New Credit Note", Value = "-1", Selected = true }));
            ViewBag.ActionCreditNote = actionCreaditItems;
        }


        //
        // GET: /Purchase/Details/5

        public ActionResult Details(int id = 0)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        //
        // GET: /Purchase/Create

        public ActionResult Create()
        {
            initDropDowlist();
            createInvoiceNo();

            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code");

            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            ViewBag.NumberItems = "";
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            //End
            return View();
        }
        private void createInvoiceNo()
        {
            ViewBag.BillCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.PurchaseOrder));

        }

        private void PopulateTaxDropDownList(object selectedTax = null)
        {
            var list = new SelectList(new[] { new {ID = "0", Name = AmountType.NoTax}, 
                                                new {ID = "1", Name = AmountType.TaxInclusive}, 
                                                new {ID = "2", Name = AmountType.TaxExclusive}}, "ID", "Name", selectedTax);
            ViewData["TaxList"] = list;
        }

        private void PopulateSearchWithinDropDownList(object selectedSearch = null)
        {
            var list = new SelectList(new[] { new {ID = "0", Name = SearchWithinType.AnyDate}, 
                                                new {ID = "1", Name = SearchWithinType.DueDate}, 
                                                new {ID = "2", Name = SearchWithinType.TransactionDate}}, "ID", "Name", selectedSearch);
            ViewData["SearchWithinList"] = list;
        }

        private List<SelectListItem> PopulateGoodsDropDownList(object selectedGoods = null)
        {
            var goodsQuery = from d in db.Goods
                             where d.Active == 1
                             orderby d.Name
                             select d;
            var list = new SelectList(goodsQuery, "ID", "Code", selectedGoods);
            List<SelectListItem> goodItems = list.ToList();
            //  goodItems.Insert(0, (new SelectListItem { Text = "+ Add new inventory...", Value = "0" }));
            goodItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));

            ViewData["GoodList"] = goodItems;
            return goodItems;
        }

        private List<SelectListItem> PopulateAccountsDropDownList(object selectedAccounts = null)
        {
            var accountsQuery = from d in db.Accounts
                                orderby d.Name
                                select d;
            var list = new SelectList(accountsQuery, "ID", "Name", selectedAccounts);
            List<SelectListItem> accountItems = list.ToList();
            accountItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["AccountList"] = accountItems;
            return accountItems;
        }

        private List<SelectListItem> PopulateTaxRateDropDownList(object selectedTaxRate = null)
        {
            var taxRateQuery = from d in db.TaxRates
                               orderby d.Name
                               select d;
            var list = new SelectList(taxRateQuery, "ID", "Name", selectedTaxRate);
            List<SelectListItem> taxRateItems = list.ToList();
            taxRateItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["TaxRateList"] = taxRateItems;
            return taxRateItems;
        }

        private void PopulateContactsDropDownList(object selectedContact = null)
        {
            var contactQuery = from d in db.Contacts
                               orderby d.ContactName
                               select d;
            var listAcc = new SelectList(contactQuery, "ID", "ContactName", selectedContact);
            ViewBag.ContactID = listAcc;
            List<SelectListItem> accountItems = listAcc.ToList();
            accountItems.Insert(0, (new SelectListItem { Text = "", Value = "0", Selected = true }));
            ViewData["AccList"] = accountItems;
        }
        [HttpPost]
        public ActionResult SearchPurchase(string tab, string referenceName, DateTime? CreatedDate, DateTime? DueDate)
        {
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User);
            if (!String.IsNullOrEmpty(referenceName))
            {
                purchases = purchases.Where(p => p.Reference.StartsWith(referenceName.Trim()) || p.Reference.Equals(referenceName.Trim()));
            }
            if (CreatedDate != null)
            {
                purchases = purchases.Where(p => p.CreatedDate >= CreatedDate);
            }
            if (DueDate != null)
            {
                purchases = purchases.Where(p => p.DueDate <= DueDate);
            }
            switch (tab)
            {

                case "#tabs-1":
                    return PartialView("_All", purchases.ToList());
                case "#tabs-2":
                    purchases = purchases.Where(p => p.Status == 0 && p.Type == 0);
                    return PartialView("_Draft", purchases.ToList());
                case "#tabs-3":
                    purchases = purchases.Where(p => p.Status == 1 && p.Type == 0);
                    return PartialView("_AwaitingApprove", purchases.ToList());
                case "#tabs-4":
                    purchases = purchases.Where(p => p.Status == 2 && p.Type == 0);
                    return PartialView("_AwaitingPayment", purchases.ToList());
                case "#tabs-5":
                    purchases = purchases.Where(p => p.Status == 3 && p.Type == 0);
                    return PartialView("_Paid", purchases.ToList());

                case "#tabs-6":
                    purchases = purchases.Where(p => p.Status == 4 && p.Type == 0);
                    return PartialView("_Repeating", purchases.ToList());

                default:
                    break;

            }
            return null;
            // return PartialView("_All", purchases.ToList());
            // return View(purchases.ToList());
        }

        public string GetBillNoPurchase()
        {
            var item = db.SaleInvoiceSettings.FirstOrDefault(s => s.Type == 1);
            var billNo = "PO-00001";
            if (item != null)
            {
                billNo = item.InvoicePrefix + item.NextNumber;
                var nextNumber = Int32.Parse(item.NextNumber) + 1;
                var strNext = nextNumber.ToString();
                while (strNext.Length < 5)
                    strNext = "0" + strNext;

                item.NextNumber = strNext;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
            return billNo;
        }

        #region Incoming Shipment
        public string CreateInvoiceStockIn(string[] purchaseID, int userId)
        {
            StockService st = new StockService();
            string result = string.Empty;
            try
            {
                var db = new ErpEntities();
                foreach (var item in purchaseID)
                {                   
                    Purchase purchase = db.Purchases.Find(Convert.ToInt32(item));
                    if (purchase.Type != 1)
                    {
                        #region Add StockIncard
                        var oStockInCard = new StockInCard();

                        oStockInCard.Code = st.GetCodeStockIn();
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
                        result += oStockInCard.Code + ",";
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
                    }
                }
                db.SaveChanges();
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

        [HttpPost]
        public string Approved(string[] purchaseID, int? status)
        {
            string result = "";         
            try
            {           
                // Create Incoming Shippment if Approved Purchase
                if (status == 2)
                {
                    result = CreateInvoiceStockIn(purchaseID, AppContext.RequestVariables.CurrentUser.ID);
                    if (string.IsNullOrEmpty(result))
                    {
                        result = "success";
                    }
                    foreach (string item in purchaseID)
                    {
                        int oPurID = Convert.ToInt32(item);                       
                        string strStatus = Utility.CreateAccountTransaction(1, Int32.Parse(item), null, null);
                        if (strStatus.Equals("1"))
                        {
                            Purchase oPurchase = db.Purchases.Find(oPurID);
                            oPurchase.Status = status ??0;
                            db.Entry(oPurchase).State = EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (string item in purchaseID)
                    {
                        int oPurID = Convert.ToInt32(item);
                        Purchase oPurchase = db.Purchases.Find(oPurID);
                        oPurchase.Status = status ??0;
                        db.Entry(oPurchase).State = EntityState.Modified;
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
        public string Draft(string[] purchaseID, int? status)
        {
            string result = "";
            //  dynamic array = JsonConvert.DeserializeObject(purchaseID);
            try
            {
                foreach (string item in purchaseID)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        //Create Account Transaction for report later

                        Purchase oPurchase = db.Purchases.Find(Int64.Parse(item));
                        oPurchase.Status = status ?? 0;
                        //oPurchase.Code = GetBillNoPurchase();
                        db.Entry(oPurchase).State = EntityState.Modified;

                    }
                }
                db.SaveChanges();
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public string DelPurchase(string[] purchaseID)
        {
            string result = "success";
            try
            {
                for (int i = 0; i < purchaseID.Length; i++)
                {
                    if (!String.IsNullOrEmpty(purchaseID[i]))
                    {
                        int x = Int32.Parse(purchaseID[i]);
                        Repeating repeating = null;
                        //For repeating purchase
                        repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == x).FirstOrDefault();
                        if (repeating != null)
                        {
                            db.Repeatings.Remove(repeating);
                        }
                        var lstPurchaseDetail = db.PurchaseDetails.Where(p => p.PurchaseID == x).ToList();
                        foreach (PurchaseDetail oDetail in lstPurchaseDetail)
                        {
                            db.PurchaseDetails.Remove(oDetail);
                        }
                        Purchase oPurchase = db.Purchases.Find(Int64.Parse(purchaseID[i]));
                        db.Purchases.Remove(oPurchase);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        //
        // POST: /Purchase/Create

        //[HttpPost]
        //public ActionResult Create(HttpPostedFileBase file, Purchase purchase, FormCollection formCollection)
        //{

        //    // Verify that the user selected a file
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        // extract only the fielname
        //        var fileName = Path.GetFileName(file.FileName);
        //        // store the file inside ~/App_Data/uploads folder
        //        var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
        //        file.SaveAs(path);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        db.Purchases.Add(purchase);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ContactID = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);
        //    ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code", purchase.CurrencyID);
        //    ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code", purchase.EmployeeID);
        //    return View(purchase);
        //}

        //
        // GET: /Purchase/Edit/5
        private List<SelectListItem> InitGoods()
        {
            var goodList = new SelectList(db.Goods, "ID", "Code");
            List<SelectListItem> goodItems = goodList.ToList();
            goodItems.Insert(0, (new SelectListItem { Text = "+ Add new inventory...", Value = "0" }));
            //goodItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            return goodItems;
        }

        public ActionResult Edit(int id = 0)
        {

            #region init DropDownlist
            PopulateGoodsDropDownList();
            PopulateAccountsDropDownList();
            PopulateTaxRateDropDownList();
            #endregion

            Purchase purchase = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            if (purchase != null)
            {
                //Count discount
                ViewBag.Discount = db.PurchaseDetails.Where(de => de.PurchaseID == purchase.ID).Sum(de => de.Discount);

                //For purchase
                if (purchase.Type == 0)
                {
                    ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseOrder);
                    // ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 1).Count();
                    ViewBag.NumberItems = db.Notes.Count(x => x.NoteFromID == purchase.ID && x.Type == 1);
                }
                //For creadit note
                else if (purchase.Type == 1)
                {
                    return RedirectToAction("EditCreditNote", "Purchase", new { id = id });
                }
            }

            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.BillCode = purchase.Code;
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }


            #region dropdown list
            var manager = new GenericManager<Currency>();
            var Items = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            List<SelectListItem> currencyItems = new SelectList(Items, "Value", "Text", purchase.CurrencyID).ToList();
            currencyItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["CurrencyList"] = currencyItems;

            var stockList = new SelectList(db.Stocks, "ID", "Code", purchase.StockID);
            List<SelectListItem> stockItems = stockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["StockList"] = stockItems;

            ViewData["AccList"] = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);


            ViewBag.Good = PopulateGoodsDropDownList();
            ViewBag.AccountID = PopulateAccountsDropDownList();
            ViewBag.TaxRateID = PopulateTaxRateDropDownList();
            #endregion

            //For function Note
            ViewBag.NoteFromID = purchase.ID;
            //End

            return View(purchase);
        }

        public ActionResult EditRepeating(int id = 0)
        {

            #region init DropDownlist
            PopulateGoodsDropDownList();
            PopulateAccountsDropDownList();
            PopulateTaxRateDropDownList();
            #endregion

            Purchase purchase = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            Repeating repeating = null;
            if (purchase != null)
            {
                //Count discount
                ViewBag.Discount = db.PurchaseDetails.Where(de => de.PurchaseID == purchase.ID).Sum(de => de.Discount);
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == purchase.ID).FirstOrDefault();
                if (repeating != null)
                {

                    List<SelectListItem> listDateSetting = _ListDateSetting.ToList();
                    List<SelectListItem> listDateSettingDuaDate = _ListDateSettingDuaDate.ToList();
                    ViewBag.DateSetting = listDateSetting;
                    ViewBag.DateSettingDuaDate = listDateSettingDuaDate;

                    ViewBag.Repeating = repeating;
                    ViewBag.RepeatingID = repeating.ID;

                    //For Note function
                    ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseRepeating);
                    ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 3).Count();
                }
            }

            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.BillCode = purchase.Code;
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }


            #region dropdown list
            var manager = new GenericManager<Currency>();
            var Items = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            List<SelectListItem> currencyItems = new SelectList(Items, "Value", "Text", purchase.CurrencyID).ToList();
            currencyItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["CurrencyList"] = currencyItems;

            var stockList = new SelectList(db.Stocks, "ID", "Code", purchase.StockID);
            List<SelectListItem> stockItems = stockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["StockList"] = stockItems;

            ViewData["AccList"] = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);


            ViewBag.Good = PopulateGoodsDropDownList();
            ViewBag.AccountID = PopulateAccountsDropDownList();
            ViewBag.TaxRateID = PopulateTaxRateDropDownList();
            #endregion

            //For function Note
            ViewBag.NoteFromID = purchase.ID;
            //End

            return View("EditRepeating", purchase);
        }
        public ActionResult EditCreditNote(int id = 0)
        {

            //initDropDowlist();
            // PopulateGoodsDropDownList();
            // PopulateAccountsDropDownList();
            // PopulateTaxRateDropDownList();
            Purchase purchase = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            if (purchase != null)
            {
                ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseCreditNote);
                ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 7).Count();

            }

            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.BillCode = purchase.Code;
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }



            var manager = new GenericManager<Currency>();
            var Items = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            List<SelectListItem> currencyItems = new SelectList(Items, "Value", "Text", purchase.CurrencyID).ToList();
            currencyItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["CurrencyList"] = currencyItems;

            var stockList = new SelectList(db.Stocks, "ID", "Code", purchase.StockID);
            List<SelectListItem> stockItems = stockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["StockList"] = stockItems;

            ViewData["AccList"] = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);


            ViewBag.Good = PopulateGoodsDropDownList();
            ViewBag.AccountID = PopulateAccountsDropDownList();
            ViewBag.TaxRateID = PopulateTaxRateDropDownList();
            //ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code", purchase.EmployeeID);
            //For function Note
            ViewBag.NoteFromID = purchase.ID;
            //End
            return View("EditCreditNote", purchase);
        }

        //
        // POST: /Purchase/Edit/5

        [HttpPost]
        public ActionResult Edit(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContactID = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code", purchase.CurrencyID);
            //ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code", purchase.EmployeeID);
            return View(purchase);
        }

        //
        // GET: /Purchase/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        //
        // POST: /Purchase/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            db.Purchases.Remove(purchase);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult FileUpload()
        {
            return View();
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase uploadFile)
        {
            if (uploadFile.ContentLength > 0)
            {
                string relativePath = "~/img/" + System.IO.Path.GetFileName(uploadFile.FileName);
                string physicalPath = Server.MapPath(relativePath);
                uploadFile.SaveAs(physicalPath);
                return View((object)relativePath);
            }
            return View();
        }

        // Dashboard
        //public ActionResult Dashboard()
        //{
        //    var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).ToList();
        //    ViewBag.Draft = purchases.Count(s => s.Status == 0);
        //    ViewBag.Awaiting = purchases.Count(s => s.Status == 1);
        //    ViewBag.AwaitingPay = purchases.Count(s => s.Status == 2);


        //    ViewBag.MDraft = purchases.Where(s => s.Status == 0).ToList().Sum(pay => pay.TotalMoney);
        //    ViewBag.MAwaiting = purchases.Where(s => s.Status == 1).ToList().Sum(pay => pay.TotalMoney);
        //    ViewBag.MAwaitingPay = purchases.Where(s => s.Status == 2).ToList().Sum(pay => pay.TotalMoney);

        //    List<SelectListItem> actionItems = _ListAction.ToList();
        //    actionItems.Insert(0, (new SelectListItem { Text = "+ New", Value = "-1", Selected = true }));
        //    ViewBag.ActionID = actionItems;

        //    return View();
        //}

        //Init Function

        public void resetTab()
        {
            ViewBag.GT01 = db.Purchases.Count();
            ViewBag.GT02 = db.Purchases.Count(s => s.Status == 0 && s.Type == 0);
            ViewBag.GT03 = db.Purchases.Count(s => s.Status == 1 && s.Type == 0);
            ViewBag.GT04 = db.Purchases.Count(s => s.Status == 2 && s.Type == 0);
            ViewBag.GT05 = db.Purchases.Count(s => s.Status == 3 && s.Type == 0);
            ViewBag.GT06 = db.Purchases.Count(s => s.Status == 4 && s.Type == 0);
        }
        public void initDropDowlist()
        {
            PopulateContactsDropDownList();
            PopulateTaxDropDownList();
            PopulateGoodsDropDownList();
            PopulateAccountsDropDownList();
            PopulateTaxRateDropDownList();

            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code");
            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code");
            ViewBag.StockID = new SelectList(db.Stocks, "ID", "Code");

            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code");

            var manager = new GenericManager<Currency>();
            var Items = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            List<SelectListItem> currencyItems = new SelectList(Items, "Value", "Text").ToList();
            currencyItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["CurrencyList"] = currencyItems;







            var stockList = new SelectList(db.Stocks, "ID", "Code");
            List<SelectListItem> stockItems = stockList.ToList();
            stockItems.Insert(0, (new SelectListItem { Text = "", Value = "", Selected = true }));
            ViewData["StockList"] = stockItems;
        }

        //End Init Function

        //Repeating Invoice
        public ActionResult CreateRepeatingPurchase()
        {
            var saleInvoiceSetting = db.SaleInvoiceSettings.FirstOrDefault();
            if (saleInvoiceSetting != null)
            {
                ViewBag.BillDefaultDuaDate = saleInvoiceSetting.BillDefaultDueDate;
                ViewBag.SaleInvoiceDefaultDueDate = saleInvoiceSetting.SaleInvoiceDefaultDueDate;
            }
            ViewData["ListDateSetting"] = _ListDateSetting;
            ViewData["ListDateSettingDuaDate"] = _ListDateSettingDuaDate;

            initDropDowlist();
            createInvoiceNo();

            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            ViewBag.NumberItems = "";
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            //End

            return View();
        }
        //End Repeating Invoice

        //Credit Note
        public ActionResult CreateCreditNotePurchase()
        {

            initDropDowlist();

            var item = db.SaleInvoiceSettings.Where(x => x.Type == 1).FirstOrDefault();
            var billNo = "CN-00001";
            if (item != null)
                billNo = item.CreditNotePrefix + item.NextNumber;
            ViewBag.BillCode = billNo;

            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;
            ViewBag.NumberItems = "";
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            //End

            return View();
        }
        //End CreditNote

        #region view awating payment details
        public ActionResult AwaitingPaymentDetails(int? id)
        {

            //#region Account
            ////Get all account payment with id :4
            //int[] groupID = new int[1] { 4 };
            //List<AccountInfo> account = AccountManager.GetAccountByGroups(groupID).ToList();
            //var list = new SelectList(account, "AccountID", "AccountCode");
            //List<SelectListItem> accountItems = list.ToList();         
            //accountItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            //ViewData["AccountPaymentList"] = accountItems;
            //#endregion


            #region title
            Purchase purchaseObj = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            Repeating repeating = null;
            if (purchaseObj != null)
            {
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == purchaseObj.ID).FirstOrDefault();
                if (repeating != null)
                {

                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseRepeating);
                    ViewBag.Repeat = @Resources.Resource.AwaitingPayment_Repeating + " " + purchaseObj.Reference;
                }
                else if (repeating == null)
                {
                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseOrder);
                    ViewBag.Purchase = @Resources.Resource.AwaitingPayment_Purchase + " " + purchaseObj.Code;
                }
                //For creadit note
                else if (purchaseObj.Type == 1)
                {
                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseCreditNote);
                    ViewBag.CreditNote = @Resources.Resource.AwaitingPayment_CreditNote + " " + purchaseObj.Code;
                }
            }
            #endregion

            #region Information Contact
            Purchase purchase = db.Purchases.Find(id);
            User user = db.Users.Find(purchase.CreatedUserID);
            ViewBag.EmployeeID = user.ID;
            ViewBag.To = purchase.Contact.ContactName ?? "Is Update";
            ViewBag.Address = purchase.Contact.PostAddress + "  " + purchase.Contact.PostCity ?? "Is Update";

            ViewBag.DueDate = purchase.DueDate;
            ViewBag.InvoiceCode = purchase.Code;
            ViewBag.Ref = purchase.Reference;
            ViewBag.ContactID = purchase.ContactID;
            #endregion

            #region Information Bill

            var oPurchase = db.Purchases.Find(id);
            double totalMoneyPurchase = 0;
            double totalTaxPurchase = 0;
            double sub = 0;
            double total = 0;
            int? useVAT = 0;
            int status = 0;
            if (oPurchase != null)
            {
                var purchaseDetailList = db.PurchaseDetails.Where(i => i.PurchaseID == oPurchase.ID).OrderBy(i => i.TaxRateID);
                totalMoneyPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TotalMoney));
                totalTaxPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TaxRate != null ? invoid.TaxRate.Rate : 0));

                sub = oPurchase.SubTotal ?? 0;
                total = oPurchase.TotalMoney ?? 0;
                useVAT = oPurchase.UseVAT;
                status = oPurchase.Status;
            }
            ViewBag.Sub = sub;
            // ViewBag.TotalTaxRate = totalTaxPurchase;
            ViewBag.TotalTax = totalMoneyPurchase * totalTaxPurchase / 100;
            ViewBag.UseVAT = useVAT;
            ViewBag.Total = total;
            ViewBag.Status = status;
            ViewBag.PurchaseID = id;
            #endregion

            #region Information Payment

            Payment payment = db.Payments.FirstOrDefault(p => p.PurchaseID == id);
            ViewBag.AmountDate = payment == null ? DateTime.Now : payment.DatePaid;

            var paylistByPurchaseId = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == id).ToList();
            double paymentTotalMoney = 0;
            if (paylistByPurchaseId.Count > 0)
            {
                paymentTotalMoney += paylistByPurchaseId.Sum(itemPay => itemPay.TotalMoney);
            }
            ViewBag.PaidPaymentAmount = paymentTotalMoney;
            double amountRemains = total - paymentTotalMoney;
            ViewBag.amountRemain = amountRemains;
            ViewBag.PayList = paylistByPurchaseId;
            #endregion

            #region SET PurchaseID for Payment
            idForPayment = (int)id;
            ViewBag.IDPayment = idForPayment;

            #endregion

            //        #region
            //        var purchasesDetails =
            //db.PurchaseDetails.Where(s => s.PurchaseID == id).Include(s => s.Good).Include(
            //    s => s.Purchase).Include(s => s.UOM).Include(s => s.TaxRate).Include(s => s.Account).ToList();
            //        #endregion
            return View("_AwaitingPaymentDetails", purchaseObj);
        }
        #endregion

        #region view paid purchase
        public ActionResult PaidPurchase(int? id)
        {
            Purchase purchase = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            Repeating repeating = null;
            if (purchase != null)
            {
                #region Information Contact
                User user = db.Users.Find(purchase.CreatedUserID);
                ViewBag.EmployeeID = user.ID;
                ViewBag.ContactID = purchase.ContactID;
                ViewBag.To = purchase.Contact.ContactName ?? "Is Update";
                ViewBag.Address = purchase.Contact.PostAddress + "  " + purchase.Contact.PostCity ?? "Is Update";
                ViewBag.CreatedDate = purchase.CreatedDate;
                ViewBag.DueDate = purchase.DueDate;
                ViewBag.Ref = purchase.Reference;
                var paymentList = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == id).OrderByDescending(p => p.DatePaid);
                if (paymentList.Count() > 0)
                {
                    Payment oPay = paymentList.FirstOrDefault();
                    ViewBag.DatePaid = oPay.DatePaid;
                }
                #endregion
                //Count discount
                ViewBag.Discount = db.PurchaseDetails.Where(de => de.PurchaseID == purchase.ID).Sum(de => de.Discount);
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == purchase.ID).FirstOrDefault();
                if (repeating != null)
                {
                    //For Note function
                    ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseRepeating);
                    ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 3).Count();
                }
                else if (repeating == null && purchase.Type == 0)
                {
                    ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseOrder);
                    // ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 1).Count();
                    ViewBag.NumberItems = db.Notes.Count(x => x.NoteFromID == purchase.ID && x.Type == 1);

                }
                //For creadit note
                else if (purchase.Type == 1)
                {
                    ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseCreditNote);
                    ViewBag.NumberItems = _manager.Get(x => x.NoteFromID == purchase.ID && x.Type == 7).Count();
                }
                var paylistByPurchaseId = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == id).ToList();
                ViewBag.PayList = paylistByPurchaseId;
            }
            if (purchase == null)
            {
                return HttpNotFound();
            }

            //For function Note
            ViewBag.NoteFromID = purchase.ID;
            //End

            return View("PaidPurchase1", purchase);
        }
        #endregion

        #region create payment
        [HttpPost]
        public ActionResult CreatePaymentByPurchaseID(string id, string money, string datepaid, int? paidTo, int? type, string reference)
        {
            var accountList = new SelectList(db.Accounts, "ID", "Name");
            List<SelectListItem> accountItems = accountList.ToList();
            accountItems.Insert(0, (new SelectListItem { Text = "+ Add new account...", Value = "0" }));
            accountItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));

            ViewBag.AccountID = accountItems;

            Payment payment = new Payment();
            payment.PurchaseID = Convert.ToInt32(id);
            payment.PaymetnTo = "Dữ liệu không xác định";
            payment.TotalMoney = Convert.ToDouble(money);
            payment.CreatedDate = DateTime.Now;

            //Update remain money
            Purchase oPurchase = db.Purchases.Find(Int32.Parse(id));
            oPurchase.RemainMoney -= Convert.ToDouble(money);
            db.Entry(oPurchase).State = EntityState.Modified;

            payment.CurrencyID = oPurchase.CurrencyID;
            payment.DatePaid = Convert.ToDateTime(datepaid);
            payment.PayToAccount = paidTo ?? 0;
            payment.Reference = reference;
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                //Create Transaction for Payment
                string status = Utility.CreateAccountTransaction(1, Int32.Parse(id), payment, type);
                db.SaveChanges();
            }

            #region Process when TotalMoney > LessPayment or TotalMonet < LessPayment

            int purchaseID = Convert.ToInt32(id);
            var purchaseDetailList = db.PurchaseDetails.Where(i => i.PurchaseID == purchaseID);
            var paymentList = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == purchaseID).ToList();

            double totalMoneyPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TotalMoney));
            double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
            ViewBag.amountRemain = totalMoneyPurchase - totalPayment;
            if (totalPayment >= totalMoneyPurchase)
            {
                Purchase pur = db.Purchases.Find(Convert.ToInt32(id));
                pur.Status = 3;
                db.Entry(pur).State = EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
            return PartialView("_addPayment");
            //return View();
        }
        #endregion

        #region view bill

        public ActionResult ViewBill(int id, string tongtien)
        {
            Payment payment = db.Payments.Find(id);
            Purchase purchase = db.Purchases.Include(contact => contact.Contact).Where(pur => pur.ID == payment.PurchaseID).FirstOrDefault();
            ViewBag.ContactName = purchase.Contact.ContactName;
            ViewBag.Date = purchase.CreatedDate;
            ViewBag.DueDate = purchase.DueDate;
            double Total = 0;
            var purchaseDetails =
                db.PurchaseDetails.Where(s => s.PurchaseID == id).ToList();
            foreach (var item in purchaseDetails)
            {
                Total += Convert.ToDouble(item.TotalMoney);
            }
            ViewBag.Total = Total;
            PurchaseDetail purdetails =
                db.PurchaseDetails.Include(g => g.Good).Where(s => s.PurchaseID == payment.PurchaseID).
                    FirstOrDefault();
            ViewBag.GoodCode = purdetails.Good.Code;
            ViewBag.TongTien = tongtien;
            return View(payment);
        }
        #endregion


        #region Paging - HungVT
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public ActionResult Dashboard()
        {
            var x = db.Purchases;
            double totalPayment = 0;
            ViewBag.Draft = x.Count(s => s.Status == 0);
            ViewBag.Awaiting = x.Count(s => s.Status == 1);
            ViewBag.AwaitingPay = x.Count(s => s.Status == 2);
            ViewBag.OverDue = x.Count(s => s.Status == 2 && DateTime.Now > s.DueDate);

            var invoiceDraftCN = x.Where(y => y.Status == 0 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceDraft = x.Where(z => z.Status == 0 && z.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MDraft = invoiceDraft - invoiceDraftCN;

            var invoiceAwaitCN = x.Where(y => y.Status == 1 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceAwait = x.Where(y => y.Status == 1 && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaiting = invoiceAwait - invoiceAwaitCN;

            var invoiceAwaitPaymentCN = x.Where(y => y.Status == 2 && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceAWPaymentLst = x.Where(y => y.Status == 2).ToList();
            foreach (var item in invoiceAWPaymentLst)
            {
                var paymentList = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == item.ID).ToList();
                totalPayment += paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
            }
            var invoiceAwaitPayment = x.Where(y => y.Status == 2 && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaitingPay = invoiceAwaitPayment - totalPayment - invoiceAwaitPaymentCN;


            var invoiceOverDueCN = x.Where(y => y.Status == 2 && DateTime.Now > y.DueDate && y.Type == 1).ToList().Sum(pay => pay.TotalMoney);
            var invoiceOverDue = x.Where(y => y.Status == 2 && DateTime.Now > y.DueDate && y.Type == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MOverDue = invoiceOverDue - totalPayment - invoiceOverDueCN;

            return View();
        }

        private double GetPaidOverDue(ref int countOverDue, int purchaseID)
        {
            double UnPaidOverDue = 0;
            double? totalPaid = 0;
            Purchase opur = db.Purchases.Find(purchaseID);
            if (opur != null)
            {
                if (opur.DueDate > DateTime.Today)
                {
                    var listPaid = db.Payments.Where(x => x.PurchaseID == opur.ID).ToList();
                    totalPaid = double.Parse(listPaid.Sum(x => x.TotalMoney).ToString());
                    if (totalPaid < opur.TotalMoney)
                    {
                        countOverDue += 1;
                        UnPaidOverDue = opur.TotalMoney - totalPaid ?? 0;
                    }
                }
            }
            return UnPaidOverDue;

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

            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, double>(GetTotalMoneyOfSaleInvoice);
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            // var purchases = db.Purchases.OrderByDescending(pur => pur.Code).ToList();
            List<Purchase> purchases = db.Purchases.OrderByDescending(pur => pur.ID).ToList();
            for (int i = 0; i < purchases.Count; i++)
            {
                Purchase oPur = purchases.ElementAt(i);
                Repeating repeating = null;
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == oPur.ID && re.DateRun > DateTime.Now).FirstOrDefault();
                if (repeating != null)
                {
                    purchases.Remove(oPur);
                }
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                purchases = SearchPurchase(purchases, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purchases = SearchPurchase(purchases, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purchases.ToPagedList(pageNumber, pageSize));
        }

        public float GetTotalMoneyPaid(int purchaseId)
        {
            var listObj = db.Payments.Where(x => x.PurchaseID == purchaseId).ToList();
            float? total = float.Parse(listObj.Sum(x => x.TotalMoney).ToString());
            float z = total ?? 0;
            return z;
        }

        public double GetTotalMoneyOfSaleInvoice(int purchaseId)
        {
            //var listObj = db.PurchaseDetails.Where(x => x.PurchaseID == purchaseId).ToList();
            //float? total = float.Parse(listObj.Sum(x => x.Price * x.Quantity).ToString());
            //float z = total ?? 0;
            //return z;
            double total = db.Purchases.Where(s => s.ID == purchaseId).ToList().Sum(pay => pay.TotalMoney) ?? 0;
            return total;
        }
        public ViewResult Draft(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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

            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, double>(GetTotalMoneyOfSaleInvoice);
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            List<Purchase> purchases = db.Purchases.Where(p => p.Status == 0).OrderByDescending(pur => pur.ID).ToList();
            /* for (int i = 0; i < purchases.Count; i++)
             {
                 Purchase oPur = purchases.ElementAt(i);
                 Repeating repeating = null;
                 //For repeating purchase
                 repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == oPur.ID && re.DateRun <= DateTime.Now && re.Status == 0).FirstOrDefault();
                 if (repeating == null && oPur.Status == 4)
                 {
                     purchases.Remove(oPur);
                 }
             } */

            //bool isDelete = false;
            //var user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
            //foreach (var item in purchases)
            //{
            //    if (item.CreatedUserID == user.ID)
            //    {
            //        isDelete = true;
            //    }
            //    else
            //    {
            //        isDelete = false;
            //    }
            //}
            // ViewBag.IsDelete = isDelete;
            if (!String.IsNullOrEmpty(searchString))
            {
                purchases = SearchPurchase(purchases, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purchases = SearchPurchase(purchases, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purchases.ToPagedList(pageNumber, pageSize));
        }
        public ViewResult Awaiting(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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

            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, double>(GetTotalMoneyOfSaleInvoice);
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }

            List<Purchase> purchases = db.Purchases.Where(p => p.Status == 1).OrderByDescending(pur => pur.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                purchases = SearchPurchase(purchases, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purchases = SearchPurchase(purchases, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purchases.ToPagedList(pageNumber, pageSize));
        }
        public ViewResult AwaitingPayment(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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

            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, double>(GetTotalMoneyOfSaleInvoice);
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }

            List<Purchase> purchases = db.Purchases.Where(p => p.Status == 2).OrderByDescending(pur => pur.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                purchases = SearchPurchase(purchases, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purchases = SearchPurchase(purchases, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purchases.ToPagedList(pageNumber, pageSize));
        }
        public ViewResult Paid(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            var purchases = db.Purchases.Where(p => p.Status == 3).OrderByDescending(pur => pur.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                purchases = SearchPurchase(purchases, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purchases = SearchPurchase(purchases, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purchases.ToPagedList(pageNumber, pageSize));
        }
        public ViewResult Repeating(string currentFilter, string searchString, int? page, int? type, string createdate, string duadate)
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
            if (currentUser != null)
            {
                ViewBag.currentUser = currentUser.ID;
            }
            //var purchases = db.Purchases.Where(p => p.Status == 4).OrderByDescending(pur => pur.Code).ToList();
            List<Purchase> purchases = db.Purchases.OrderByDescending(pur => pur.ID).ToList();
            List<Purchase> purRepeating = new List<Purchase>();
            for (int i = 0; i < purchases.Count; i++)
            {
                Purchase oPur = purchases.ElementAt(i);
                Repeating repeating = null;
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == oPur.ID).FirstOrDefault();
                if (repeating != null)
                {
                    purRepeating.Add(oPur);
                }
            }
            purRepeating.OrderByDescending(pur => pur.ID).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                purRepeating = SearchPurchase(purRepeating, searchString, TypeSearch, createdate, duadate).ToList();
            }
            else
            {
                purRepeating = SearchPurchase(purRepeating, "", TypeSearch, createdate, duadate).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(purRepeating.ToPagedList(pageNumber, pageSize));
        }
        public IList<Purchase> SearchPurchase(IList<Purchase> purchases, string sStr, int? type, string createdate, string enddate)
        {

            var purchase = new List<Purchase>();
            DateTime sDateTime = string.IsNullOrEmpty(createdate) ? Convert.ToDateTime("01/01/0001") : Convert.ToDateTime(createdate);
            DateTime eDateTime = string.IsNullOrEmpty(enddate) ? Convert.ToDateTime("01/01/0001") : Convert.ToDateTime(enddate);
            List<Purchase> invoicesName;
            List<Purchase> invoicesRef;
            List<Purchase> invoicesStartDateCreatDate;
            List<Purchase> invoicesStartDateDueDate;
            List<Purchase> invoicesEndDateCreatDate;
            List<Purchase> invoicesEndDateDueDate;
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
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }

                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateCreatDate = purchases.Where(x => x.CreatedDate >= sDateTime).ToList();
                    invoicesStartDateDueDate = purchases.Where(x => x.DueDate >= sDateTime).ToList();
                    purchases = invoicesStartDateCreatDate.Union(invoicesStartDateDueDate).ToList();
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateCreatDate = purchases.Where(x => x.CreatedDate <= eDateTime).ToList();
                    invoicesEndDateDueDate = purchases.Where(x => x.DueDate <= eDateTime).ToList();
                    purchases = invoicesEndDateCreatDate.Union(invoicesEndDateDueDate).ToList();
                }
                purchase = purchases.ToList();
            }
            #endregion AnyDate
            #region Transaction Date
            if (type == 1)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateCreatDate = purchases.Where(x => x.CreatedDate >= sDateTime).ToList();
                    purchases = invoicesStartDateCreatDate;
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateCreatDate = purchases.Where(x => x.CreatedDate <= eDateTime).ToList();
                    purchases = invoicesEndDateCreatDate;
                }
                purchase = purchases.ToList();
            }
            #endregion
            #region Due Date
            if (type == 2)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesStartDateDueDate = purchases.Where(x => x.DueDate >= sDateTime).ToList();
                    purchases = invoicesStartDateDueDate;
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    invoicesEndDateDueDate = purchases.Where(x => x.DueDate <= eDateTime).ToList();
                    purchases = invoicesEndDateDueDate;
                }
                purchase = purchases.ToList();
            }
            #endregion
            #region Paid Date
            if (type == 3)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }

                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {
                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Payments.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DatePaid >= sDateTime
                                       select item).ToList();
                        purchases = invoice;
                    }
                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {
                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Payments.LastOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DatePaid >= sDateTime
                                       select item).ToList();
                        purchases = invoice;
                    }
                }
                return purchases;
            }
            #endregion
            #region Next Invoice Date
            if (type == 4)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {
                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DateRun >= sDateTime
                                       select item).ToList();
                        purchases = invoice;
                    }

                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {
                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.DateRun <= eDateTime
                                       select item
                                      ).ToList();
                        purchases = invoice;
                    }
                }
                return purchases;
            }
            #endregion
            #region End Date
            if (type == 5)
            {
                if (!string.IsNullOrEmpty(sStr))
                {
                    invoicesName = purchases.Where(x => x.ContactName.Contains(sStr)).ToList();
                    invoicesRef = purchases.Where(x => x.Reference.Contains(sStr)).ToList();
                    purchases = invoicesName.Union(invoicesRef).ToList();
                }
                if (sDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {
                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.EndDate >= sDateTime
                                       select item).ToList();
                        purchases = invoice;
                    }

                }
                if (eDateTime.GetDateTimeFormats()[6] != Convert.ToDateTime("01/01/0001").GetDateTimeFormats()[6])
                {
                    if (purchases != null)
                    {

                        var invoice = (from item in purchases
                                       let firstOrDefault = item.Repeatings.FirstOrDefault()
                                       where firstOrDefault != null && firstOrDefault.EndDate <= eDateTime
                                       select item
                                      ).ToList();
                        purchases = invoice;
                    }
                }
                if (purchases != null) purchase = purchases.ToList();
            }
            #endregion
            #region Planned Date
            if (type == 6)
            {

            }
            #endregion
            return purchase;


        }
        #endregion

        #region allocate creditnote
        [HttpPost]
        public String AlloCreditNote(int? CreditNoteID, string creditNoteDetail)
        {
            object result = null;
            dynamic array = JsonConvert.DeserializeObject(creditNoteDetail);
            Purchase creditNote = db.Purchases.Find(CreditNoteID);
            double amountToCreditTotal = 0;

            foreach (var item in array)
            {
                int purchaseID = item.purchaseID;
                double amountToCredit = item.amountToCredit;
                amountToCreditTotal += amountToCredit;
                Purchase oPurchase = db.Purchases.Find(purchaseID);
                // CreatePaymentForPurchase(oPurchase, amountToCredit);

                DecreaseAmountInvoice(oPurchase, amountToCredit);
            }
            // Decrease creditNote
            DecreaseAmountInvoice(creditNote, amountToCreditTotal);
            result = creditNote.Status;
            //return RedirectToAction("CreditNoteAwaitingPayment", "Purchase", new { id = CreditNoteID });
            return serializer.Serialize(result); ;

            //return View();
        }

        private void DecreaseAmountInvoice(Purchase oPurchase, double amountToCredit)
        {
            oPurchase.TotalMoney = oPurchase.TotalMoney - amountToCredit;
            if (oPurchase.TotalMoney == 0)
            {
                oPurchase.Status = 3;
            }
            Utility.CreateAccountTransaction(1, oPurchase.ID, null, null);
            db.Entry(oPurchase).State = EntityState.Modified;
            db.SaveChanges();

        }

        private void CreatePaymentForPurchase(Purchase oPurchase, double amountToCredit)
        {
            Payment payment = new Payment();
            payment.PurchaseID = oPurchase.ID;
            payment.PaymetnTo = "Dữ liệu không xác định";
            payment.TotalMoney = amountToCredit;
            payment.CreatedDate = DateTime.Now;
            payment.CurrencyID = oPurchase.CurrencyID;
            payment.DatePaid = DateTime.Now;
            payment.PayToAccount = 1;
            payment.Reference = oPurchase.Reference;
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                //Create Transaction for Payment
                string status = Utility.CreateAccountTransaction(1, oPurchase.ID, payment, 0);
                db.SaveChanges();
            }

            #region Process when TotalMoney > LessPayment or TotalMonet < LessPayment

            int purchaseID = oPurchase.ID;
            var purchaseDetailList = db.PurchaseDetails.Where(i => i.PurchaseID == purchaseID);
            var paymentList = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == purchaseID).ToList();

            double totalMoneyPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TotalMoney));
            double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
            ViewBag.amountRemain = totalMoneyPurchase - totalPayment;
            if (totalPayment >= totalMoneyPurchase)
            {
                Purchase pur = db.Purchases.Find(Convert.ToInt32(purchaseID));
                pur.Status = 3;
                db.Entry(pur).State = EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
        }
        #endregion

        #region show view allocate credit screen
        public ActionResult AllocCreditBalance(int id = 0)
        {
            List<Purchase> lsInvoice = new List<Purchase>();
            List<int> lsPurID = new List<int>();
            List<Purchase> lsPurchase = new List<Purchase>();
            Purchase creditNote = db.Purchases.Find(id);

            #region title
            ViewBag.CreditNoteID = id;
            ViewBag.CreditNoteCode = creditNote.Code;
            ViewBag.OutStandingCreditBalance = creditNote.TotalMoney;
            #endregion

            int? supplier = creditNote.ContactID;
            List<int> lsGoodItems = new List<int>();
            var creditDetailList = db.PurchaseDetails.Where(details => details.PurchaseID == creditNote.ID).ToList();
            lsGoodItems = creditDetailList.Select(i => i.GoodID).Distinct().ToList();

            //Query purchases have the same supplier and good
            var purchaseList = db.Purchases.Where(pur => pur.ContactID == supplier && pur.Type == 0 && pur.Status == 2).ToList();
            foreach (var item in purchaseList)
            {
                Repeating repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == item.ID).FirstOrDefault();
                if (repeating == null)
                {
                    var purchaseDetailsList = db.PurchaseDetails.Where(pur => pur.PurchaseID == item.ID).ToList();
                    GetInvoiceSameGood(ref lsInvoice, ref lsPurID, lsGoodItems, purchaseDetailsList);
                }

            }

            if (lsInvoice.Count == 0)
            {
                return RedirectToAction("CreditNoteAwaitingPayment", "Purchase", new { id = id });

            }

            return View("AllocateCreditNote", lsInvoice);
        }

        private void GetInvoiceSameGood(ref List<Purchase> lsInvoice, ref List<int> lsPurID, List<int> lsGoodItems, List<PurchaseDetail> purchaseDetailsList)
        {
            for (int i = 0; i < lsGoodItems.Count(); i++)
            {
                foreach (var item in purchaseDetailsList)
                {
                    if (item.GoodID == (int)lsGoodItems[i])
                    {
                        Purchase oPur = db.Purchases.Find(item.PurchaseID);
                        if (!lsPurID.Contains(oPur.ID))
                        {
                            lsPurID.Add(oPur.ID);
                            lsInvoice.Add(oPur);
                        }
                    }
                }
            }
        }


        #endregion

        #region view awating payment credit note
        public ActionResult CreditNoteAwaitingPayment(int? id)
        {

            #region title
            Purchase purchaseObj = db.Purchases.Include(pur => pur.PurchaseDetails).Where(pur => pur.ID == id).SingleOrDefault();
            Repeating repeating = null;
            if (purchaseObj != null)
            {
                //For repeating purchase
                repeating = db.Repeatings.Where(re => re.RepeatType == 1 && re.PurchaseID == purchaseObj.ID).FirstOrDefault();
                if (repeating != null)
                {

                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseRepeating);
                    ViewBag.Repeat = @Resources.Resource.AwaitingPayment_Repeating + " " + purchaseObj.Reference;
                }
                else if (repeating == null)
                {
                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseOrder);
                    ViewBag.Purchase = @Resources.Resource.AwaitingPayment_Purchase + " " + purchaseObj.Code;
                }
                //For creadit note
                else if (purchaseObj.Type == 1)
                {
                    ViewBag.Type = CommonLib.EnumHelper.GetDescription(NodeType.PurchaseCreditNote);
                    ViewBag.CreditNote = @Resources.Resource.AwaitingPayment_CreditNote + " " + purchaseObj.Code;
                }
            }
            #endregion

            #region Information Contact
            Purchase purchase = db.Purchases.Find(id);
            User user = db.Users.Find(purchase.CreatedUserID);
            ViewBag.EmployeeID = user.ID;
            ViewBag.To = purchase.Contact.ContactName ?? "Is Update";
            ViewBag.Address = purchase.Contact.PostAddress + "  " + purchase.Contact.PostCity ?? "Is Update";

            ViewBag.DueDate = purchase.DueDate;
            ViewBag.InvoiceCode = purchase.Code;
            ViewBag.Ref = purchase.Reference;
            ViewBag.ContactID = purchase.ContactID;
            #endregion

            #region Information Bill

            var oPurchase = db.Purchases.Find(id);
            double totalMoneyPurchase = 0;
            double totalTaxPurchase = 0;
            double sub = 0;
            double total = 0;
            int? useVAT = 0;
            int status = 0;
            if (oPurchase != null)
            {
                var purchaseDetailList = db.PurchaseDetails.Where(i => i.PurchaseID == oPurchase.ID).OrderBy(i => i.TaxRateID);
                totalMoneyPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TotalMoney));
                totalTaxPurchase = Enumerable.Sum(purchaseDetailList, invoid => Convert.ToDouble(invoid.TaxRate.Rate));

                sub = oPurchase.SubTotal ?? 0;
                total = oPurchase.TotalMoney ?? 0;
                useVAT = oPurchase.UseVAT;
                status = oPurchase.Status;
            }
            ViewBag.Sub = sub;
            // ViewBag.TotalTaxRate = totalTaxPurchase;
            ViewBag.TotalTax = totalMoneyPurchase * totalTaxPurchase / 100;
            ViewBag.UseVAT = useVAT;
            ViewBag.Total = total;
            ViewBag.Status = status;
            ViewBag.PurchaseID = id;
            #endregion

            #region Information Payment

            Payment payment = db.Payments.FirstOrDefault(p => p.PurchaseID == id);
            ViewBag.AmountDate = payment == null ? DateTime.Now : payment.DatePaid;

            var paylistByPurchaseId = db.Payments.Include(p => p.Purchase).Where(p => p.PurchaseID == id).ToList();
            double paymentTotalMoney = 0;
            if (paylistByPurchaseId.Count > 0)
            {
                paymentTotalMoney += paylistByPurchaseId.Sum(itemPay => itemPay.TotalMoney);
            }
            ViewBag.PaidPaymentAmount = paymentTotalMoney;
            double amountRemains = total - paymentTotalMoney;
            ViewBag.amountRemain = amountRemains;
            ViewBag.PayList = paylistByPurchaseId;
            #endregion

            #region SET PurchaseID for Payment
            idForPayment = (int)id;
            ViewBag.IDPayment = idForPayment;

            #endregion

            #region
            var purchasesDetails = db.PurchaseDetails.Where(s => s.PurchaseID == id).Include(s => s.Good).Include(s => s.Purchase).Include(s => s.UOM).Include(s => s.TaxRate).Include(s => s.Account).ToList();
            #endregion
            return View("CreditNoteAwaitingPayment", purchaseObj);
        }
        #endregion

        #region compare date
        [HttpPost]
        public string CompareDate(string strDate)
        {
            string result = "false";
            try
            {
                DateTime sDateTime = string.IsNullOrEmpty(strDate) ? DateTime.Now : Convert.ToDateTime(strDate);
                if (sDateTime <= DateTime.Now)
                    result = "true";
                else
                    result = "false";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion

    }
}