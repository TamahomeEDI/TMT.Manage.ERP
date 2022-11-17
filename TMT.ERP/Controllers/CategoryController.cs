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
    public class CategoryController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Category/

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
                return View(db.Categories.OrderByDescending(p => p.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<Category> category = db.Categories.Where(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)).OrderByDescending(p => p.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(category.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /Category/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Category/Create

        [HttpPost]
        public string Create(string name, string description)
        {
            Category category = new Category();
            category.Name = name;
            category.Description = description;
            db.Categories.Add(category);
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
        // GET: /Category/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public int ExistsName(string name, int id)
        {
            var checkresult = 0;
            Category category = db.Categories.Where(p => p.Name == name && p.ID != id).FirstOrDefault();
            if (category == null)
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
        // POST: /Category/Edit/5

        [HttpPost]
        public string Edit(string name, string description, int id)
        {
            Category category = db.Categories.Find(id);
            category.Name = name;
            category.Description = description;
            db.Entry(category).State = EntityState.Modified;
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

        //
        // POST: /Category/Delete/5
        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Category category = db.Categories.Find(id);
                if (category != null)
                {
                    try
                    {
                        db.Categories.Remove(category);
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