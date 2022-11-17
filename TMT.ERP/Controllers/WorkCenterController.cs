using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using PagedList;
using System.Text;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Commons;

namespace TMT.ERP.Controllers
{
    public class WorkCenterController : BaseController
    {
        private ErpEntities db = new ErpEntities();
        private readonly GenericManager<WorkCenter> _manager = new GenericManager<WorkCenter>();
        private readonly GenericManager<Machine> _managerMachine = new GenericManager<Machine>();
        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        public ActionResult Index(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var ooWorkCenter = db.WorkCenters.OrderByDescending(x => x.ID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                ooWorkCenter = ooWorkCenter.Where(x => x.Code.Contains(searchString)).ToList();
            }
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.Error = result;
            result = "";
            return View(ooWorkCenter.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(int? page)
        {
            var ooMachine = db.Machines.OrderByDescending(x => x.ID).ToList();
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(ooMachine.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Edit(int id = 0)
        {
            var oWorkCenter = db.WorkCenters.Find(id);
            if (oWorkCenter == null)
            {
                return HttpNotFound();
            }
            ViewBag.Code = oWorkCenter.Code;
            ViewBag.Description = oWorkCenter.Description;
            ViewBag.ID = oWorkCenter.ID;
            var machine = db.Machines.Where(m => m.WorkCenterID == id).ToList();
            ViewBag.Count = machine.Count();
            return View(machine);
        }

        public PartialViewResult GetPaging(int? page, string search)
        {
            var ooProduct = db.ProductionMonitors.ToList();
            var ooMachine = db.Machines.Where(x => x.Status != 2).OrderByDescending(x => x.ID).ToList();
            foreach (var oProduct in ooProduct)
            {
                if (ooMachine.Exists(x => x.ID == oProduct.MachineID))
                {
                    ooMachine.Remove(ooMachine.FirstOrDefault(x => x.ID == oProduct.MachineID));
                }
            }
            if (search == "None")
            {
                ooMachine = ooMachine.ToList();
            }
            else
            {
                ooMachine = ooMachine.Where(x => x.Code.Contains(search)).ToList();
            }
            const int pageSize = 5;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return PartialView("_GetPaging", ooMachine.ToPagedList(pageNumber, pageSize));
        }

        public PartialViewResult GetPagingEdit(int? page, string search, int workcenterId)
        {
            var ooMachine = db.Machines.Where(x => x.Status != 2).OrderByDescending(x => x.ID).ToList();
            var ooProduct = db.ProductionMonitors.ToList();
            var oWorkCenter = db.WorkCenters.Find(workcenterId);
            var ooMachinByWorkCenter = oWorkCenter.Machines.ToList();


            foreach (var oProduct in ooProduct)
            {
                if (ooMachine.Exists(x => x.ID == oProduct.MachineID))
                {
                    ooMachine.Remove(ooMachine.FirstOrDefault(x => x.ID == oProduct.MachineID));
                }
            }
            foreach (var oMa in ooMachinByWorkCenter)
            {
                if (ooMachine.Exists(x => x.ID == oMa.ID))
                {
                    ooMachine.Remove(ooMachine.FirstOrDefault(x => x.ID == oMa.ID));
                }
            }


            if (search == "None")
            {
                ooMachine = ooMachine.ToList();
            }
            else
            {
                ooMachine.Where(x => x.Code.Contains(search)).ToList();
            }

            const int pageSize = 3;
            ViewBag.PageSize = pageSize;
            ViewBag.WorkCenterID = workcenterId;
            int pageNumber = (page ?? 1);
            return PartialView("_GetPagingEdit", ooMachine.ToPagedList(pageNumber, pageSize));
        }

        public string Save(int? workCenterId, int companyID, string code, string description, int[] machineID)
        {
            string result = "success";
            var user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
            try
            {
                var oWorkCenter = _manager.FindById(workCenterId);
                if (oWorkCenter == null)
                {
                    oWorkCenter = new WorkCenter();
                    if (user.CompanyID != null)
                    {
                        oWorkCenter.CompanyID = user.CompanyID;
                    }
                    _manager.Add(oWorkCenter);
                }
                oWorkCenter.Code = code;
                oWorkCenter.CompanyID = companyID;
                oWorkCenter.Description = description;
                _manager.Save();

                if (machineID != null)
                {
                    foreach (var item in machineID)
                    {
                        int machineId = Int32.Parse(item.ToString(CultureInfo.InvariantCulture));
                        var oMachine = _managerMachine.FindById(machineId);
                        oMachine.WorkCenterID = oWorkCenter.ID;
                        _managerMachine.Update(oMachine);
                    }
                    _managerMachine.Save();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            WorkCenter center = db.WorkCenters.Where(p => p.ID != id && p.Code == code).FirstOrDefault();
            if (center == null)
            {
                checkresult = 0;
            }
            else
            {
                checkresult = 1;

            }
            return checkresult;
        }


        public string Delete(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                WorkCenter wc = db.WorkCenters.Find(id);
                if (wc != null)
                {
                    try
                    {
                        db.WorkCenters.Remove(wc);
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
