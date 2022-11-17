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
    public class MachinesController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Machines/

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
                return View(db.Machines.Where(m => m.Status != 2).OrderByDescending(s => s.ID).ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<Machine> machine = db.Machines.Where(c => c.Code.Contains(searchString) || c.Description.Contains(searchString)).Where(m => m.Status != 2).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(machine.ToPagedList(pageNumber, pageSize));
            }
        }

        // Check Exists Code
        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            Machine machine = db.Machines.Where(m => m.ID != id && m.Code == code).FirstOrDefault();
            if (machine == null)
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
        // GET: /Machine/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code");

            return View();
        }

        //
        // POST: /Machine/Create

        [HttpPost]
        public string Create(string code, string name, string description, int status, int wcenterID)
        {

            Machine machine = new Machine();
            machine.Code = code;
            machine.Name = name;
            machine.Description = description;
            machine.Status = status;
            machine.WorkCenterID = wcenterID;
            machine.CostPerMinute = 0;
            machine.DefaultRunTime = 0;
            machine.DefaultSetupTime = DateTime.Now.Date;
            db.Machines.Add(machine);
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
        // GET: /Machine/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Machine machine = db.Machines.Find(id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            ViewBag.WorkCenterID = new SelectList(db.WorkCenters, "ID", "Code", machine.WorkCenterID);
            return View(machine);
        }

        //
        // POST: /Machine/Edit/5

        [HttpPost]
        public string Edit(string code, string name, string description, int status, int wcenterID, int id)
        {
            Machine machine = db.Machines.Find(id);

                machine.Code = code;
                machine.Name = name;
                machine.Description = description;
                machine.Status = status;
                machine.WorkCenterID = wcenterID;
                machine.CostPerMinute = 0;
                machine.DefaultRunTime = 0;
                machine.DefaultSetupTime = DateTime.Now.Date;
                db.Entry(machine).State = EntityState.Modified;
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

        // POST: /Machine/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Machine machine = db.Machines.Find(id);
                if (machine != null)
                {
                    try
                    {
                        db.Machines.Remove(machine);
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
