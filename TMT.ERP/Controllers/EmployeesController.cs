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
    public class EmployeesController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Employee/

        public ActionResult Index()
        {
            return View(db.Employees.ToList());
        }

        //
        // GET: /Employee/Details/5

        public ActionResult Details(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // GET: /Employee/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName");

            return View();
        }

        //
        // POST: /Employee/Create

        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName", employee.CompanyID);
            return View(employee);
        }

        //
        // GET: /Employee/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName", employee.CompanyID);
            return View(employee);
        }

        //
        // POST: /Employee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName", employee.CompanyID);
            return View(employee);
        }

        //
        // GET: /Employee/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        //
        // POST: /Employee/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Employee emp = db.Employees.Find(id);
                if (emp != null)
                {
                    db.Employees.Remove(emp);
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
