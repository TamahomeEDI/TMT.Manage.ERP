using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Text;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class UOMController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /UOM/

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
            if (searchString == null || searchString == "")
            {
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(db.UOMs.OrderByDescending(s => s.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<UOM> uom = db.UOMs.Where(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(uom.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /UOM/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /UOM/Create

        [HttpPost]
        public string Create(string code, string name, string description)
        {
            UOM uom = new UOM();
            uom.Code = code;
            uom.Name = name;
            uom.Description = description;
            db.UOMs.Add(uom);
            try
            {
                db.SaveChanges();
                result = "Create";
            }
            catch (Exception e) {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /UOM/Edit/5

        public ActionResult Edit(int id = 0)
        {
            UOM uom = db.UOMs.Find(id);
            if (uom == null)
            {
                return HttpNotFound();
            }
            return View(uom);
        }

        //
        // POST: /UOM/Edit/5

        [HttpPost]
        public string Edit(string code, string name, string description, int id)
        {
            UOM uom = db.UOMs.Find(id);
            uom.Code = code;
            uom.Name = name;
            uom.Description = description;
            db.Entry(uom).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                result = "Edit";
            }
            catch (Exception e) {
                result = "Error";
            }
            return result;
        }

        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            UOM uom = db.UOMs.Where(p => p.ID != id && p.Code == code).FirstOrDefault();
            if (uom == null)
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
        // POST: /UOM/Delete/5
        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                UOM uom = db.UOMs.Find(id);
                if (uom != null)
                {
                    try
                    {
                        db.UOMs.Remove(uom);
                        db.SaveChanges();
                        result = "Success";
                    }
                    catch (Exception e)
                    {
                        result = e.ToString();
                    }
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