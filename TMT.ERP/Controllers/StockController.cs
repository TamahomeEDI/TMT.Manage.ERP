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
    public class StockController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Stock/

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
                return View(db.Stocks.OrderByDescending(s => s.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<Stock> stock = db.Stocks.Where(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(stock.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /Stock/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Stock/Create

        [HttpPost]
        public string Create(string code, string name, string description, string address, string maxQuantity)
        {
            Stock stock = new Stock();
            stock.Name = name;
            stock.Address = address;
            stock.Code = code;
            if (maxQuantity != null || maxQuantity != "")
            {
                stock.MaxQuatity = Int32.Parse(maxQuantity);
            }
            stock.Description = description;
            db.Stocks.Add(stock);
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


        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            Stock stock = db.Stocks.Where(p => p.ID != id && p.Code == code).FirstOrDefault();
            if (stock == null)
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
        // GET: /Stock/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Stock stock = db.Stocks.Find(id);
            if (stock == null)
            {
                return HttpNotFound();
            }
            return View(stock);
        }

        //
        // POST: /Stock/Edit/5

        [HttpPost]
        public string Edit(string code, string name, string description, string address, string maxQuantity, int id)
        {

            Stock stock = db.Stocks.Find(id);
            stock.Name = name;
            stock.Address = address;
            stock.Code = code;
            if (maxQuantity != null || maxQuantity != "")
            {
                stock.MaxQuatity = Int32.Parse(maxQuantity);
            }
            stock.Description = description;
            db.Entry(stock).State = EntityState.Modified;
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
        // POST: /Stock/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
                foreach (var item in delConfirm)
                {
                    int id = Int32.Parse(item);
                    Stock stock = db.Stocks.Find(id);
                    if (stock != null)
                    {
                        try
                        {
                            db.Stocks.Remove(stock);
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