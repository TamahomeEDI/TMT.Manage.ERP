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
    public class AccountBalancesController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /AccountBalances/

        public ActionResult Index()
        {
            return View(db.AccountBalances.ToList());
        }

        //
        // GET: /AccountBalances/Details/5

        public ActionResult Details(int id = 0)
        {
            AccountBalance accountbalance = db.AccountBalances.Find(id);
            if (accountbalance == null)
            {
                return HttpNotFound();
            }
            return View(accountbalance);
        }

        //
        // GET: /AccountBalances/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AccountBalances/Create

        [HttpPost]
        public ActionResult Create(AccountBalance accountbalance)
        {
            if (ModelState.IsValid)
            {
                db.AccountBalances.Add(accountbalance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accountbalance);
        }

        //
        // GET: /AccountBalances/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AccountBalance accountbalance = db.AccountBalances.Find(id);
            if (accountbalance == null)
            {
                return HttpNotFound();
            }
            return View(accountbalance);
        }

        //
        // POST: /AccountBalances/Edit/5

        [HttpPost]
        public ActionResult Edit(AccountBalance accountbalance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountbalance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accountbalance);
        }

        //
        // GET: /AccountBalances/Delete/5

        public ActionResult Delete(int id = 0)
        {
            AccountBalance accountbalance = db.AccountBalances.Find(id);
            if (accountbalance == null)
            {
                return HttpNotFound();
            }
            return View(accountbalance);
        }

        //
        // POST: /AccountBalances/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountBalance accountbalance = db.AccountBalances.Find(id);
            db.AccountBalances.Remove(accountbalance);
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