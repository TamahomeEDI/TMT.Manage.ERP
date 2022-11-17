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
    public class MachineHistoryController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /MachineHistory/

        public ActionResult Index()
        {
            return View(db.MachineHistories.ToList());
        }


        //
        // GET: /MachineHistory/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.MachineID = new SelectList(db.Machines, "ID", "Name");
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code");
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name");
            return View();
        }

        //
        // POST: /MachineHistory/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MachineHistory machine)
        {
            if (ModelState.IsValid)
            {
                db.MachineHistories.Add(machine);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //For Dropdownlist
            ViewBag.MachineID = new SelectList(db.Machines, "ID", "Name", machine.MachineID);
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code", machine.WorkCenterID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name", machine.CurrencyID);
            return View(machine);
        }

        //
        // GET: /MachineHistory/Edit/5

        public ActionResult Edit(int id = 0)
        {
            MachineHistory machine = db.MachineHistories.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }

            //For Dropdownlist
            ViewBag.MachineID = new SelectList(db.Machines, "ID", "Name", machine.MachineID);
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code", machine.WorkCenterID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name", machine.CurrencyID);
            return View(machine);
        }

        //
        // POST: /MachineHistory/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MachineHistory machine)
        {
            if (ModelState.IsValid)
            {
                db.Entry(machine).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //For Dropdownlist
            ViewBag.MachineID = new SelectList(db.Machines, "ID", "Name", machine.MachineID);
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code", machine.WorkCenterID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name", machine.CurrencyID);
            return View(machine);
        }

        //
        // POST: /MachineHistory/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm) {
                int id = Int32.Parse(item);
                MachineHistory machine = db.MachineHistories.Find(id);
                if (machine != null)
                {
                    db.MachineHistories.Remove(machine);
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