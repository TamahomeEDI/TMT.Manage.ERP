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
    public class BudgetsController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Budget/

        public ActionResult Index()
        {
            return View(db.Budgets.ToList());
        }

        //
        // GET: /Budget/Details/5

        public ActionResult Details(int id = 0)
        {
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        //
        // GET: /Budget/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Budget/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(budget);
        }

        //
        // GET: /Budget/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        //
        // POST: /Budget/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budget);
        }

        //
        // GET: /Budget/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        //
        // POST: /Budget/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Budget budget = db.Budgets.Find(id);
                if (budget != null)
                {
                    db.Budgets.Remove(budget);
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
    }
}