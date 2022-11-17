using TMT.ERP.DataAccess.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    public class SaleInvoiceSettingController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /SaleInvoiceSetting/

        public ActionResult Index()
        {
            var saleinvoicesettings = db.SaleInvoiceSettings.Include(s => s.DueDateType).Include(s => s.DueDateType1);
            return View(saleinvoicesettings.ToList());
        }

        //
        // GET: /SaleInvoiceSetting/Details/5

        public ActionResult Details(int id = 0)
        {
            SaleInvoiceSetting saleinvoicesetting = db.SaleInvoiceSettings.Find(id);
            if (saleinvoicesetting == null)
            {
                return HttpNotFound();
            }
            return View(saleinvoicesetting);
        }

        //
        // GET: /SaleInvoiceSetting/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SaleInvoiceSetting/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SaleInvoiceSetting saleinvoicesetting)
        {
            if (ModelState.IsValid)
            {
                db.SaleInvoiceSettings.Add(saleinvoicesetting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BillDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Code", saleinvoicesetting.BillDefaultTypeID);
            ViewBag.SaleInvoiceDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Code", saleinvoicesetting.SaleInvoiceDefaultTypeID);
            return View(saleinvoicesetting);
        }

        //
        // GET: /SaleInvoiceSetting/Edit/5

        public ActionResult Edit(int id = 0)
        {
            SaleInvoiceSetting saleinvoicesetting = db.SaleInvoiceSettings.Find(id);
            if (saleinvoicesetting == null)
            {
                return HttpNotFound();
            }
            ViewBag.BillDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Code", saleinvoicesetting.BillDefaultTypeID);
            ViewBag.SaleInvoiceDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Code", saleinvoicesetting.SaleInvoiceDefaultTypeID);
            return View(saleinvoicesetting);
        }

        //
        // POST: /SaleInvoiceSetting/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SaleInvoiceSetting saleinvoicesetting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(saleinvoicesetting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BillDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Name");
            ViewBag.SaleInvoiceDefaultTypeID = new SelectList(db.DueDateTypes, "ID", "Name");
            return View(saleinvoicesetting);
        }

        //
        // GET: /SaleInvoiceSetting/Delete/5

        public ActionResult Delete(int id = 0)
        {
            SaleInvoiceSetting saleinvoicesetting = db.SaleInvoiceSettings.Find(id);
            if (saleinvoicesetting == null)
            {
                return HttpNotFound();
            }
            return View(saleinvoicesetting);
        }

        //
        // POST: /SaleInvoiceSetting/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SaleInvoiceSetting saleinvoicesetting = db.SaleInvoiceSettings.Find(id);
            db.SaleInvoiceSettings.Remove(saleinvoicesetting);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}