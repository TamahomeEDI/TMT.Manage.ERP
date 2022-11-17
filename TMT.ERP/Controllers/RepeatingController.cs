using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    public class RepeatingController : BaseController
    {
        private readonly GenericManager<Repeating> _manager = new GenericManager<Repeating>();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(int numday, int repeatTime, string dateRun, int dueDateNum, int dueDateType, string endDate, int status, int repeatType, int saleId, int purchaseId)
        {
            // string FMT = "yyyy-MM-dd HH:mm:ss.fff";
            var repeating = new Repeating();
            repeating.NumDayRepeat = numday;
            repeating.RepeatTime = repeatTime;
            repeating.DateRun = Convert.ToDateTime(dateRun);
            // repeating.DateRun = DateTime.ParseExact(dateRun, FMT, CultureInfo.InvariantCulture);
            repeating.DueDateNum = dueDateNum;
            repeating.DueDateTypeID = dueDateType;
            
            if (endDate != "" && endDate != null)
            {
                repeating.EndDate = Convert.ToDateTime(endDate);
            }
            repeating.Status = status;
            repeating.RepeatType = repeatType;
            if (saleId > 0)
                repeating.SaleInvoiceID = saleId;
            if (purchaseId > 0)
                repeating.PurchaseID = purchaseId;

            if (ModelState.IsValid)
            {
                _manager.Add(repeating);
                _manager.Save();
            }
            return View(repeating);
        }

        [HttpPost]
        public string Edit(int repeatingID, int numday, int repeatTime, string dateRun, int dueDateNum, int dueDateType, string endDate, int status, int repeatType, int saleId, int purchaseId)
        {
            string result = "success";
            try
            {
                Repeating reObj = _manager.FindById(repeatingID);
                reObj.NumDayRepeat = numday;
                reObj.RepeatTime = repeatTime;
                reObj.DateRun = Convert.ToDateTime(dateRun);
                reObj.DueDateNum = dueDateNum;
                reObj.DueDateTypeID = dueDateType;
                reObj.EndDate = Convert.ToDateTime(endDate);
                reObj.Status = status;
                reObj.RepeatType = repeatType;
                reObj.SaleInvoiceID = saleId;
                reObj.PurchaseID = purchaseId;


                if (ModelState.IsValid)
                {
                    _manager.Update(reObj);
                    _manager.Save();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

    }
}
