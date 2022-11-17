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
    public class ProductTypesController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /ProductTypes/Index

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
                return View(db.ProductTypes.OrderByDescending(p => p.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<ProductType> proType = db.ProductTypes.Where(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)).OrderByDescending(p => p.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(proType.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /ProductTypes/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /ProductTypes/Create

        [HttpPost]
        public string Create(string name, string description)
        {
            ProductType pro = new ProductType();
            pro.Name = name;
            pro.Description = description;
            db.ProductTypes.Add(pro);
            try
            {
                db.SaveChanges();
                result = "Create";
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /ProductTypes/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ProductType proType = db.ProductTypes.Find(id);
            if (proType == null)
            {
                return HttpNotFound();
            }
            return View(proType);
        }

        //
        // POST: /ProductTypes/Edit/5

        [HttpPost]
        public string Edit(string name, string description, int id)
        {
            ProductType proType = db.ProductTypes.Find(id);
            proType.Name = name;
            proType.Description = description;
            db.Entry(proType).State = EntityState.Modified;
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
        // GET: /ProductTypes/ExistsName/

        public int ExistsName(string name, int id)
        {
            var checkresult = 0;
            ProductType proType = db.ProductTypes.Where(p => p.Name == name && p.ID != id).FirstOrDefault();
            if (proType == null)
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
        // POST: /ProductTypes/Delete/5
        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                ProductType proType = db.ProductTypes.Find(id);
                if (proType != null)
                {
                    try
                    {
                        db.ProductTypes.Remove(proType);
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