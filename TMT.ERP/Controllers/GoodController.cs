using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Controllers
{
    public class GoodController : BaseController
    {
        private ErpEntities db = new ErpEntities();
        //Defin Type of Good 0 - Raw material,1 - Semi-good,2-Finish-good
        /*SelectList _ListGoodType = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Raw Material"},
            new SelectListItem {Value = "1", Text = "Semi Product"},
            new SelectListItem {Value = "2", Text = "Finish Product"}
        }, "Value", "Text");


        SelectList _ListStatus = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Active"},
            new SelectListItem {Value = "1", Text = "InActive"}            
        }, "Value", "Text");
        //Defind Status Lock and Unlock 0 - Unlock 1 - Lock

        SelectList _ListLock = new SelectList(new[]{
            new SelectListItem {Value = "0", Text = "Unlock"},
            new SelectListItem {Value = "1", Text = "Lock"}            
        }, "Value", "Text");*/

        //
        // GET: /Good/

        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        public ActionResult Index(string currentFilter, int? page, string searchString)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (searchString == null)
            {
                var good = db.Goods.Where(g => g.Active == 1).Include(c => c.Category).Include(u => u.UOM).OrderByDescending(a => a.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(good.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var good = db.Goods.Where(c => c.Code.Contains(searchString) || c.Description.Contains(searchString) && c.Active == 1).Include(c => c.Category).Include(u => u.UOM).OrderByDescending(a => a.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(good.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult Archive(string currentFilter, int? page, string searchString)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (searchString == null)
            {
                var good = db.Goods.Where(g => g.Active == 0).Include(c => c.Category).Include(u => u.UOM).OrderByDescending(a => a.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(good.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var good = db.Goods.Where(c => c.Code.Contains(searchString) || c.Description.Contains(searchString) && c.Active == 0).Include(c => c.Category).Include(u => u.UOM).OrderByDescending(a => a.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(good.ToPagedList(pageNumber, pageSize));
            }
        }


        //
        // GET: /Good/SubGood/5

        /*public ActionResult SubGood(int id)
        {
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }

            //List<Good> goods = db.Goods.Where(g => g.ID != id).Include(s => s.SubGoods.Where(a => a.GoodParentID == id)).ToList();

            List<SubGood> subGood = db.SubGoods.Where(s => s.GoodParentID == id).Include(g => g.Good).ToList();
            ViewBag.ListSubGood = subGood;
            ViewBag.GoodName = new Func<int, string>(GoodName);
            ViewBag.GoodCode = new Func<int, string>(GoodCode);
            ViewBag.GoodDescription = new Func<int, string>(GoodDescription);
            ViewBag.ParentID = id;
            return View(db.Goods.Where(g => g.ID != id).ToList());
        }

        public string GoodName(int id)
        {
            Good good = db.Goods.Find(id);
            return good.Name;
        }
        public string GoodCode(int id)
        {
            Good good = db.Goods.Find(id);
            return good.Code;
        }
        public string GoodDescription(int id)
        {
            Good good = db.Goods.Find(id);
            return good.Description;
        }
        
        // Add Sub-Good
        [HttpPost]
        public ActionResult AddSubGood(int? _id, int? parentID)
        {
            SubGood sgood = new SubGood();
            sgood.GoodParentID = parentID;
            sgood.GoodID = _id;
            db.SubGoods.Add(sgood);
            db.SaveChanges();
            return View();
        }*/

        [HttpPost]
        public string RateForAccount(int id)
        {
            string rateID = "";
            Account account = db.Accounts.Find(id);
            rateID = account.TaxRateID.ToString();
            return rateID;
        }

        // Check Exists Code
        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            Good good = db.Goods.Where(m => m.ID != id && m.Code == code).FirstOrDefault();
            if (good == null)
            {
                checkresult = 0;
            }
            else
            {
                checkresult = 1;

            }
            return checkresult;
        }

        //
        // GET: /Good/Create

        public ActionResult Create()
        {
            ViewBag.ContactID = new SelectList(db.Contacts.Where(c => c.Type == 0), "ID", "ContactName");
            ViewBag.UOMID = new SelectList(db.UOMs, "ID", "Name");
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID", "Name");
            ViewBag.SaleAccountID = new SelectList(db.Accounts, "ID", "Name");
            ViewBag.SaleTaxRateID = new SelectList(db.TaxRates, "ID", "Name");
            ViewBag.PurchaseAccountID = new SelectList(db.Accounts, "ID", "Name");
            ViewBag.PurchaseTaxRateID = new SelectList(db.TaxRates, "ID", "Name");
            //ViewBag.Code = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.InventoryItem));
            /*ViewData["GoodType"] = _ListGoodType;
            ViewData["StatusType"] = _ListStatus;
            ViewData["ListLock"] = _ListLock;*/
            return View();
        }

        //
        // POST: /Good/Create

        [HttpPost]
        public string Create(FormCollection collection, string code, int? category, int type, int uom, string inPrice,string name,
            string outPrice, int? saleAccID, int? saleTaxID, int? purchaseAccID, int? purchaseTaxID, string description, string url)
        {
            // Add new goods
            Good good = new Good();
            //good.Name = name;
            good.Attachment = !string.IsNullOrEmpty(url) ? url : "/Uploads/noimage.jpg";
            good.Code = code;
            good.CategoryID = category;
            good.ProductTypeID = type;
            good.UOMID = uom;
            good.StartedDate = DateTime.Now;
            good.Description = description;
            good.InputPrice = Double.Parse(inPrice);
            good.OutputPrice = Double.Parse(outPrice);
            good.SaleAccountID = saleAccID;
            good.SaleTaxRateID = saleTaxID;
            good.PurchaseAccountID = purchaseAccID;
            good.PurchaseTaxRateID = purchaseTaxID;
            good.Lock = 0;
            good.Active = 1;
            db.Goods.Add(good);
            try
            {
                db.SaveChanges();
                //Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.InventoryItem), Constant.CODE_LENGTH, "");
                result = "Create";
            }
            catch (Exception e)
            {
                result = "Error";
            }

            return result;
        }

        //
        // GET: /Good/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            ViewBag.Images = good.Attachment;
            ViewBag.ContactID = new SelectList(db.Contacts.Where(c => c.Type == 0), "ID", "ContactName");
            ViewBag.UOMID = new SelectList(db.UOMs, "ID", "Name", good.UOMID);
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", good.CategoryID);
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID", "Name", good.ProductTypeID);
            ViewBag.SaleAccountID = new SelectList(db.Accounts, "ID", "Name", good.SaleAccountID);
            ViewBag.SaleTaxRateID = new SelectList(db.TaxRates, "ID", "Name", good.SaleTaxRateID);
            ViewBag.PurchaseAccountID = new SelectList(db.Accounts, "ID", "Name", good.PurchaseAccountID);
            ViewBag.PurchaseTaxRateID = new SelectList(db.TaxRates, "ID", "Name", good.PurchaseTaxRateID);
            /*ViewData["GoodType"] = _ListGoodType;
            ViewData["StatusType"] = _ListStatus;
            ViewData["ListLock"] = _ListLock;*/
            return View(good);
        }

        //
        // POST: /Good/Edit/5

        [HttpPost]
        public string Edit(FormCollection collection, string code, int? category, int type, int uom, string inPrice, string url,
            string outPrice, int? saleAccID, int? saleTaxID, int? purchaseAccID, int? purchaseTaxID, string description, int id, string name)
        {
            // Add new goods
            Good good = db.Goods.Find(id);
            good.Name = name;
            good.Attachment = !string.IsNullOrEmpty(url) ? url : "/Uploads/noimage.jpg";
            good.Code = code;
            good.CategoryID = category;
            good.ProductTypeID = type;
            good.UOMID = uom;
            good.StartedDate = DateTime.Now;
            good.Description = description;
            good.InputPrice = Double.Parse(inPrice);
            good.OutputPrice = Double.Parse(outPrice);
            good.SaleAccountID = saleAccID;
            good.SaleTaxRateID = saleTaxID;
            good.PurchaseAccountID = purchaseAccID;
            good.PurchaseTaxRateID = purchaseTaxID;
            db.Entry(good).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                result = "Edit";
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;

        }

        //
        // POST: /Good/Delete/5

        [HttpPost]
        public string Archived(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Good good = db.Goods.Find(id);
                try
                {
                    good.Active = 0;
                    db.Entry(good).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "Archive";
                }
                catch(Exception e)
                {
                    result = "Error";
                }
            }
            return result;
        }

        //
        // GET: /Good/Details/5
        public ActionResult Details(int id = 0)
        {
            Good good = db.Goods.Find(id);
            if (good == null)
            {
                return HttpNotFound();
            }
            ViewBag.Images = good.Attachment;
            ViewBag.ContactID = new SelectList(db.Contacts.Where(c => c.Type == 0), "ID", "ContactName");
            ViewBag.UOMID = new SelectList(db.UOMs, "ID", "Name", good.UOMID);
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name", good.CategoryID);
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID", "Name", good.ProductTypeID);
            ViewBag.SaleAccountID = new SelectList(db.Accounts, "ID", "Name", good.SaleAccountID);
            ViewBag.SaleTaxRateID = new SelectList(db.TaxRates, "ID", "Name", good.SaleTaxRateID);
            ViewBag.PurchaseAccountID = new SelectList(db.Accounts, "ID", "Name", good.PurchaseAccountID);
            ViewBag.PurchaseTaxRateID = new SelectList(db.TaxRates, "ID", "Name", good.PurchaseTaxRateID);
            /*ViewData["GoodType"] = _ListGoodType;
            ViewData["StatusType"] = _ListStatus;
            ViewData["ListLock"] = _ListLock;*/
            return View(good);
        }

        //
        // POST: /Good/Delete/5

        /*[HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            string result = "Success";
            try
            {
                foreach (var item in delConfirm)
                {
                    int id = Int32.Parse(item);
                    Good good = db.Goods.Find(id);
                    if (good != null)
                    {
                        db.Goods.Remove(good);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            return result;
        }*/

        // POST : /Good/Restore 

        [HttpPost]
        public string Restore(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Good good = db.Goods.Find(id);
                try
                {
                    good.Active = 1;
                    db.Entry(good).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "Restore";
                }
                catch(Exception e)
                {
                    result = "Error";
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}