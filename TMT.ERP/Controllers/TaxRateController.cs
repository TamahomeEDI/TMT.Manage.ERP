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
using TMT.ERP.BusinessLogic.Utils;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class TaxRateController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /TaxRate/

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
                return View(db.TaxRates.OrderByDescending(s => s.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<TaxRate> tax = db.TaxRates.Where(c => c.Name.Contains(searchString) || c.Code.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(tax.ToPagedList(pageNumber, pageSize));
            }
        }

        public int ExistsName(string name, int id)
        {
            var checkresult = 0;
            TaxRate tax = db.TaxRates.Where(t => t.ID != id && t.Name == name).FirstOrDefault();
            if (tax == null)
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
        // GET: /TaxRate/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /TaxRate/Create

        [HttpPost]
        public string Create(string name, string description, string rate)
        {
            TaxRate tax = new TaxRate();
            tax.Name = name;
            tax.Rate = float.Parse(rate);
            tax.Description = description;
            db.TaxRates.Add(tax);
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
        // GET: /TaxRate/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TaxRate taxrate = db.TaxRates.Find(id);
            if (taxrate == null)
            {
                return HttpNotFound();
            }
            return View(taxrate);
        }

        //
        // POST: /TaxRate/Edit/5

        [HttpPost]
        public string Edit(string name, string description, string rate, int id)
        {
            TaxRate tax = db.TaxRates.Find(id);
            tax.Name = name;
            tax.Rate = float.Parse(rate);
            tax.Description = description;
            db.Entry(tax).State = EntityState.Modified;
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
        // POST: /TaxRate/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                TaxRate tax = db.TaxRates.Find(id);
                if (tax != null)
                {
                    try
                    {
                        db.TaxRates.Remove(tax);
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