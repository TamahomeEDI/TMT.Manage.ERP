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
    public class DueDateTypeController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /DueDateType/

        public ActionResult Index()
        {
            return View(db.DueDateTypes.ToList());
        }

        //
        // GET: /DueDateType/Details/5

        public ActionResult Details(int id = 0)
        {
            DueDateType duedatetype = db.DueDateTypes.Find(id);
            if (duedatetype == null)
            {
                return HttpNotFound();
            }
            return View(duedatetype);
        }

        //
        // GET: /DueDateType/Create

        public ActionResult Create()
        {            
            return View();
        }

        //
        // POST: /DueDateType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DueDateType duedatetype)
        {
            if (ModelState.IsValid)
            {
                db.DueDateTypes.Add(duedatetype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(duedatetype);
        }

        //
        // GET: /DueDateType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            DueDateType duedatetype = db.DueDateTypes.Find(id);
            if (duedatetype == null)
            {
                return HttpNotFound();
            }
            return View(duedatetype);
        }

        //
        // POST: /DueDateType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DueDateType duedatetype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(duedatetype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(duedatetype);
        }

        //
        // GET: /DueDateType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            DueDateType duedatetype = db.DueDateTypes.Find(id);
            if (duedatetype == null)
            {
                return HttpNotFound();
            }
            return View(duedatetype);
        }

        //
        // POST: /DueDateType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DueDateType duedatetype = db.DueDateTypes.Find(id);
            db.DueDateTypes.Remove(duedatetype);
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