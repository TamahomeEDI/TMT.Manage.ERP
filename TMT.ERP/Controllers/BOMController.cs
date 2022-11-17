using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using PagedList;
using System.Globalization;
using TMT.ERP.Commons;
using System.Data;


namespace TMT.ERP.Controllers
{
    public class BOMController : BaseController
    {
        public static int PageSize = Constant.DefaultPageSize;
        public static int ItemID = 0;
        public static string result = "";
        ErpEntities db = new ErpEntities();

        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentFilter = searchString;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var bomLs = db.BOMs.ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                bomLs = SearchBOM(bomLs, searchString).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            ViewBag.Error = result;
            result = "";
            int pageNumber = (page ?? 1);

            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.GoodID = new SelectList(db.Goods, "ID", "Name");
            return View(bomLs.ToPagedList(pageNumber, pageSize));   
        }

        private List<BOM> SearchBOM(List<BOM> productLs, string searchString)
        {
            var product = new List<BOM>();

            var productrCode = productLs.Where(wo => wo.Name.Contains(searchString));

            return productrCode.ToList();
        }

        //Change Category
        public ActionResult ChangeCategory(int? id) 
        {
            ViewBag.GoodID = new SelectList(db.Goods.Where(g => g.CategoryID == id), "ID", "Code");
            return View("ChangeCategory");
        }

        //Change Category
        public ActionResult ChangeItem(int id, string currentFilter, string searchString, int? page)
        {
            ItemID = id;
            List<BOM> bom = db.BOMs.Where(b => b.GoodID == id).ToList();
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.CurrentFilter = searchString;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                bom = SearchBOM(bom, searchString).ToList();
            }
            
            return View("ChangeItem",bom.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /BOM/Create

        public ActionResult Create()
        {
            ViewBag.ItemID = ItemID;
            return View();
        }

        //
        // POST: /BOM/Create

        [HttpPost]
        public string Create(string name, string date , string description, int active, int? itemID)
        {
            BOM bom = new BOM();
            bom.Name = name;
            bom.Description = description;
            if (date != null & date != "")
            {
                bom.EffectiveDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            }
            bom.Active = Convert.ToBoolean(active);
            bom.GoodID = itemID;
            db.BOMs.Add(bom);

            if (bom.Active == true)
            {
                List<BOM> bomlist = db.BOMs.Where(b => b.Active == true && b.GoodID == itemID && b.ID != bom.ID).ToList();
                foreach (var item in bomlist)
                {
                    item.Active = false;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
            try
            {
                db.SaveChanges();;
                result = "Create";
            }
            catch (Exception e) {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /BOM/Edit

        public ActionResult Edit(int id = 0)
        {
            BOM bom = db.BOMs.Find(id);
            if (bom == null)
            {
                return HttpNotFound();
            }
            if (bom.Active == true)
            {
                ViewBag.Active = 1;
            }
            else
                ViewBag.Active = 0;
            ViewBag.ItemID = ItemID;
            return View(bom);
        }

        //
        // POST: /BOM/Create

        [HttpPost]
        public string Edit(int id, string name, string date, string description, int active, int? itemID)
        {
            BOM bom = db.BOMs.Find(id);
            bom.Name = name;
            bom.Description = description;
            if (date != null & date != "")
            {
                bom.EffectiveDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            }
            bom.Active = Convert.ToBoolean(active);
            bom.GoodID = itemID;
            db.Entry(bom).State = EntityState.Modified;

            if (bom.Active == true)
            {
                List<BOM> bomlist = db.BOMs.Where(b => b.Active == true && b.GoodID == itemID && b.ID != id).ToList();
                foreach (var item in bomlist)
                {
                    item.Active = false;
                    db.Entry(item).State = EntityState.Modified;
                }
            }
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
        // POST: /BOM/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                BOM bom = db.BOMs.Find(id);
                if (bom != null)
                {
                    db.BOMs.Remove(bom);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult AddComponent(int? BOMID, int? itemID)
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
            ViewBag.Good = new SelectList(db.Goods, "ID", "Name");
            var BOM = db.BOMs.Find(BOMID);
            var Good = db.Goods.Find(itemID);
            ViewBag.BOMName = BOM.Name;
            ViewBag.GoodName = Good.Code + "  " + Good.Name;
            ViewBag.GoodID = Good.ID;
            var good = db.BomDetails.Where(bom => bom.BomID == BOMID && bom.ParentGoodID == null).ToList();

            ViewData["BOM"] = good;


            return View(BOM);
        }

        //[HttpGet]
        //public ActionResult ViewDetailBOM(int? BomId, int? GoodId, int? ParentGoodId, int? quantity)
        //{
        //    ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
        //    ViewBag.GoodID = new SelectList(db.Goods, "ID", "Name");

        //    var Good = db.Goods.Find(GoodId);
        //    ViewBag.Category = Good.CategoryID;

        //    ViewBag.BomID = BomId;
        //    ViewBag.Good = GoodId;
        //    ViewBag.ParentGoodID = ParentGoodId;
        //    ViewBag.Quantity = quantity.ToString();
        //    return View();
        //}

        [HttpPost]
        public ActionResult ViewDetailBOM(int? bomDetailID)
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "ID", "Name");
           
            BomDetail detail = db.BomDetails.Find(bomDetailID);
            int? GoodId = detail.GoodID;
            var Good = db.Goods.Find(GoodId);
            ViewBag.Category = Good.CategoryID;
            ViewBag.GoodID = new SelectList(db.Goods.Where(gd => gd.CategoryID == Good.CategoryID).ToList(), "ID", "Code");
            ViewBag.BomID = detail.BomID;
            ViewBag.Good = Good.ID;
            ViewBag.ParentGoodID = detail.ParentGoodID;
            ViewBag.Quantity = detail.Quantity;
            return View("ViewDetailBOM", detail);
        }

        [HttpPost]
        public ActionResult AddTreeView(int? BomID, int? GoodID, int? ParentGoodID, int? quantity, string description)
        {
            BomDetail detail = new BomDetail();
            detail.BomID = BomID;
            detail.GoodID = GoodID;
            detail.ParentGoodID = ParentGoodID;
            detail.Quantity = quantity;
            detail.Description = description;
            db.BomDetails.Add(detail);
            db.SaveChanges();
            var good = db.BomDetails.Where(bom => bom.BomID == BomID && bom.ParentGoodID == null).ToList();
            ViewData["BOM"] = good;
            return View("TreeView");
        }

        [HttpPost]
        public string Delete(int? BOMid)
        {
            string del = "Success";
            List<BomDetail> detail = db.BomDetails.Where(d => d.BomID == BOMid).ToList();
            foreach (var item in detail) {
                db.BomDetails.Remove(item);
            }
            BOM bom = db.BOMs.Find(BOMid);
            db.BOMs.Remove(bom);
            db.SaveChanges();
            return del;
        }

        [HttpPost]
        public ActionResult DeleteBom(int? BomDetailID, int? itemID)
        {
            BomDetail detail = db.BomDetails.Find(BomDetailID);

            db.BomDetails.Remove(detail);
            db.SaveChanges();
            var good = db.BomDetails.Where(bom => bom.BomID == detail.BomID && bom.ParentGoodID == null).ToList();
            ViewData["BOM"] = good;
            return View("TreeView");
        }
    }
}
