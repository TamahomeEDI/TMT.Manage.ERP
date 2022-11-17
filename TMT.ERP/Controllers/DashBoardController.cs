using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models;
using TMT.ERP.Models.Lookups;


namespace TMT.ERP.Controllers
{
    public class DashBoardController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        SelectList _ListAction = new SelectList(new[]{ 
            new SelectListItem {Value = "0", Text = "Bill"},
             new SelectListItem {Value = "1", Text = "Repeating bill"},
              new SelectListItem {Value = "2", Text = "Credit note"},
        }, "Value", "Text");

        // GET: /Purchase/

        public ActionResult List(int? tabNum)
        {
            ViewBag.GT01 = db.Purchases.Count();
            ViewBag.GT02 = db.Purchases.Count(s => s.Status == 0);
            ViewBag.GT03 = db.Purchases.Count(s => s.Status == 1);
            ViewBag.GT04 = db.Purchases.Count(s => s.Status == 2);
            ViewBag.GT05 = db.Purchases.Count(s => s.Status == 3);
            ViewBag.GT06 = db.Purchases.Count(s => s.Status == 4);
            PopulateSearchWithinDropDownList();
            PopulateTaxDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User);
            ViewBag.TypeStatus = tabNum;
            return View("_List", purchases.ToList());
        }

        public ActionResult All()
        {
            // PopulateTaxDropDownList();
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User);
            // return View(purchases.ToList());
            return PartialView("_All", purchases.ToList());
        }

        public ActionResult Draft()
        {
            // PopulateTaxDropDownList();
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).Where(p => p.Status == 0);
            // return View( purchases.ToList());
            return PartialView("_Draft", purchases.ToList());
        }

        public ActionResult AwaitingApprove()
        {
            // PopulateTaxDropDownList();
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).Where(p => p.Status == 1);
            // return View( purchases.ToList());
            return PartialView("_AwaitingApprove", purchases.ToList());
        }

        public ActionResult AwaitingPayment()
        {
            // PopulateTaxDropDownList();
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).Where(p => p.Status == 2);
            // return View( purchases.ToList());
            return PartialView("_AwaitingPayment", purchases.ToList());
        }

        public ActionResult Paid()
        {
            // PopulateTaxDropDownList();
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).Where(p => p.Status == 3);
            // return View( purchases.ToList());
            return PartialView("_Paid", purchases.ToList());
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
            PopulateContactsDropDownList();
            PopulateTaxDropDownList();
            PopulateGoodsDropDownList();
            PopulateAccountsDropDownList();
            PopulateTaxRateDropDownList();
            // //  ViewBag.ContactID = new SelectList(db.Contacts, "ID", "Code");
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code");
            ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code");
            return View();
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

        private void PopulateGoodsDropDownList(object selectedGoods = null)
        {
            //var goodsQuery = from d in db.Goods
            //                 orderby d.Name
            //                 select d;
            var list = new SelectList(db.Goods, "ID", "Code", selectedGoods);
            List<SelectListItem> goodItems = list.ToList();
            goodItems.Insert(0, (new SelectListItem { Text = "+ Add new inventory...", Value = "0" }));
            goodItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));

            ViewData["GoodList"] = goodItems;
        }

        private void PopulateAccountsDropDownList(object selectedAccounts = null)
        {
            var accountsQuery = from d in db.Accounts
                                orderby d.Name
                                select d;
            var list = new SelectList(accountsQuery, "ID", "Name", selectedAccounts);
            List<SelectListItem> accountItems = list.ToList();
            accountItems.Insert(0, (new SelectListItem { Text = "+ Add new account...", Value = "0" }));
            accountItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            ViewData["AccountList"] = accountItems;
        }

        private void PopulateTaxRateDropDownList(object selectedTaxRate = null)
        {
            var taxRateQuery = from d in db.TaxRates
                               orderby d.Name
                               select d;
            var list = new SelectList(taxRateQuery, "ID", "Name", selectedTaxRate);
            List<SelectListItem> taxRateItems = list.ToList();
            taxRateItems.Insert(0, (new SelectListItem { Text = "", Value = "0", Selected = true }));
            ViewData["TaxRateList"] = taxRateItems;
        }

        private void PopulateContactsDropDownList(object selectedContact = null)
        {
            var contactQuery = from d in db.Contacts
                               orderby d.ContactName
                               select d;
            ViewBag.ContactID = new SelectList(contactQuery, "ID", "ContactName", selectedContact);
        }
        [HttpPost]
        public ActionResult SearchPurchase(string tab, string referenceName, DateTime? CreatedDate, DateTime? DueDate)
        {
            PopulateSearchWithinDropDownList();
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User);
            if (!String.IsNullOrEmpty(referenceName))
            {
                purchases = purchases.Where(p => p.Reference.StartsWith(referenceName));
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
                    purchases = purchases.Where(p => p.Status == 0);
                    return PartialView("_Draft", purchases.ToList());
                case "#tabs-3":
                    purchases = purchases.Where(p => p.Status == 1);
                    return PartialView("_AwaitingApprove", purchases.ToList());
                case "#tabs-4":
                    purchases = purchases.Where(p => p.Status == 2);
                    return PartialView("_AwaitingPayment", purchases.ToList());
                case "#tabs-5":
                    purchases = purchases.Where(p => p.Status == 3);
                    return PartialView("_Paid", purchases.ToList());

                default:
                    break;

            }
            return null;
            // return PartialView("_All", purchases.ToList());
            // return View(purchases.ToList());
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

        public ActionResult Edit(int id = 0)
        {
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContactID = new SelectList(db.Contacts, "ID", "Code", purchase.ContactID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Code", purchase.CurrencyID);
           // ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code", purchase.EmployeeID);
            return View(purchase);
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
          //  ViewBag.EmployeeID = new SelectList(db.Employees, "ID", "Code", purchase.EmployeeID);
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
        public ActionResult Index()
        {
            var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency).Include(p => p.User).ToList();
            //   var purchases = db.Purchases.Include(p => p.Contact).Include(p => p.Currency);
            ViewBag.Draft = purchases.Count(s => s.Status == 0);
            ViewBag.Awaiting = purchases.Count(s => s.Status == 1);
            ViewBag.AwaitingPay = purchases.Count(s => s.Status == 2);


            ViewBag.MDraft = purchases.Where(s => s.Status == 0).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaiting = purchases.Where(s => s.Status == 1).ToList().Sum(pay => pay.TotalMoney);
            ViewBag.MAwaitingPay = purchases.Where(s => s.Status == 2).ToList().Sum(pay => pay.TotalMoney);

            List<SelectListItem> actionItems = _ListAction.ToList();
            actionItems.Insert(0, (new SelectListItem { Text = "+ New", Value = "-1", Selected = true }));
            ViewBag.ActionID = actionItems;

            return View();
        }


    }
}