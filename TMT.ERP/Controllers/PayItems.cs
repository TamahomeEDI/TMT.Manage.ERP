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
    public class PayItemsController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /PayItem/

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
                return View(db.PayItems.OrderByDescending(s => s.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<PayItem> item = db.PayItems.Where(c => c.Name.Contains(searchString) || c.Description.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(item.ToPagedList(pageNumber, pageSize));
            }
        }


        //
        // GET: /PayItem/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Name");
            ViewBag.PayItemTypeID = new SelectList(db.PayItemTypes, "ID", "Name");
            return View();
        }

        //
        // POST: /PayItem/Create

        [HttpPost]
        public string Create(string name, string description, int type, int account, float? rate, int? onslip, int onDefault)
        {
            PayItem payitem = new PayItem();
            payitem.Name = name;
            payitem.Description = description;
            payitem.PayItemTypeID = type;
            payitem.AccountID = account;
            if (rate != null)
            {
                payitem.DefaultRate = rate;
            }
            payitem.ShowOnSlip = onslip;
            payitem.IsShow = Convert.ToBoolean(onDefault);
            db.PayItems.Add(payitem);
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
        // GET: /PayItem/Edit/5

        public ActionResult Edit(int id = 0)
        {
            PayItem payitem = db.PayItems.Find(id);
            if (payitem == null)
            {
                return HttpNotFound();
            }
            if (payitem.IsShow == true)
            {
                ViewBag.OnDefault = 1;
            }
            else
                ViewBag.OnDefault = 0;

            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Name", payitem.AccountID);
            ViewBag.PayItemTypeID = new SelectList(db.PayItemTypes, "ID", "Name", payitem.PayItemTypeID);
            return View(payitem);
        }

        //
        // POST: /PayItem/Edit/5

        [HttpPost]
        public string Edit(string name, string description, int type, int account, float? rate, int? onslip, int onDefault, int id)
        {
            PayItem payitem = db.PayItems.Find(id);
            payitem.Name = name;
            payitem.Description = description;
            payitem.PayItemTypeID = type;
            payitem.AccountID = account;
            if (rate != null)
            {
                payitem.DefaultRate = rate;
            }
            payitem.ShowOnSlip = onslip;
            payitem.IsShow = Convert.ToBoolean(onDefault);
            db.Entry(payitem).State = EntityState.Modified;
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
        // POST: /PayItem/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                PayItem Payitem = db.PayItems.Find(id);
                if (Payitem != null)
                {
                    try
                    {
                        db.PayItems.Remove(Payitem);
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
