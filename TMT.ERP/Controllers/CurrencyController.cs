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
    public class CurrencyController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        //
        // GET: /Currency/

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
                var currency = db.Currencies.OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(currency.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var currency = db.Currencies.Where(a => a.Name.Contains(searchString) || a.Code.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(currency.ToPagedList(pageNumber, pageSize));
            }
        }

        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            Currency currency = db.Currencies.Where(p => p.ID != id && p.Code == code).FirstOrDefault();
            if (currency == null)
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
        // GET: /Currency/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Currency/Create

        [HttpPost]
        public string Create(string code, string name, string description)
        {
            Currency currency = new Currency();
            currency.Code = code;
            currency.Name = name;
            currency.Descripttion = description;
            currency.Status = 0;
            db.Currencies.Add(currency);
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
        // GET: /Currency/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Currency currency = db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        //
        // POST: /Currency/Edit/5

        [HttpPost]
        public string Edit(string code, string name, string description, int id)
        {
            Currency currency = db.Currencies.Find(id);

                currency.Code = code;
                currency.Name = name;
                currency.Descripttion = description;
                currency.Status = 0;
                db.Entry(currency).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    result = "Edit";
                }
                catch(Exception e) 
                {
                    result = "Error";
                }
                return result;
            
        }

        //
        // POST: /Currency/Delete/5

        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Currency currency = db.Currencies.Find(id);
                if (currency != null)
                {
                    try
                    {
                        db.Currencies.Remove(currency);
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