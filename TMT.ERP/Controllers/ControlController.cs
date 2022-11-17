using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    public class ControlController : BaseController
    {
        //
        // GET: /Control/
        private readonly ErpEntities _db = new ErpEntities();
        private readonly GenericManager<Function> _manager = new GenericManager<Function>();
        private readonly GenericManager<Feature> _managerFeature = new GenericManager<Feature>();
        private readonly GenericManager<Control> _managerControl = new GenericManager<Control>();
        private readonly GenericManager<ControlInRole> _managerControlInRole = new GenericManager<ControlInRole>();
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        public static int FunctionID = 0;
        public static int FeatureID = 0;
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public int SetFunction(string functionId)
        {
            FunctionID = Convert.ToInt32(functionId);
            return FunctionID;
        }
        public int SetFeature(string featureId)
        {
            FeatureID = Convert.ToInt32(featureId);
            return FeatureID;
        }
        public ActionResult Index(int? page)
        {
            ViewBag.FunctionID = _manager.Get().ToList();
            var controls = _managerControl.Get().OrderByDescending(x => x.ID).ToList();
            controls = FunctionID == 0 ? controls.ToList() : controls.Where(x => x.FunctionID == FunctionID).ToList();
            ViewBag.Function = FunctionID;
            FunctionID = 0;
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber;
            if (page == null || page == 0)
            {
                pageNumber = 1;
            }
            else
            {
                pageNumber = page ?? 1;
            }
            return View(controls.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            ViewBag.FeatureID = _db.Features.ToList();
            ViewBag.FunctionID = _db.Functions.ToList();
            return View();
        }
        public ActionResult Edit(int? id)
        {
            var oControl = _managerControl.FindById(id);
            if (oControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            ViewBag.Function = oControl.Function;
            ViewBag.FeatureID = _db.Features.ToList();
            ViewBag.FunctionID = _db.Functions.ToList();
            return View(oControl);
        }
        public string Delete(string[] id)
        {
            string result = "";
            try
            {
                var ooControlInRole = _managerControlInRole.Get().ToList();
                var noDelete = new List<int>();

                foreach (var x in id)
                {
                    var controlId = Convert.ToInt32(x);
                    if (ooControlInRole.Exists(c => c.ControlID == controlId))
                    {
                        noDelete.Add(controlId);
                    }
                    else
                    {
                        var oControl = _managerControl.FindById(controlId);
                        _managerControl.Delete(oControl);
                    }
                }
                _managerControl.Save();
                foreach (var i in noDelete)
                {
                    var oControl = _managerControl.FindById(i);
                    result += "Control: " + oControl.ControlName;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string Save(int? id, int functionId, string controlId, string controlName)
        {
            string result = "success";
            try
            {
                var oControl = _managerControl.FindById(id);
                if (oControl == null)
                {
                    oControl = new Control();
                    oControl.FunctionID = functionId;
                    oControl.ControlID = controlId;
                    oControl.ControlName = controlName;
                    _managerControl.Add(oControl);
                }
                else
                {
                    oControl.FunctionID = functionId;
                    oControl.ControlID = controlId;
                    oControl.ControlName = controlName;
                    _managerControl.Update(oControl);
                }
                _managerControl.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public PartialViewResult GetFunction(int featureId)
        {
            var functions = _manager.Get(x => x.FeatureID == featureId).ToList();
            return PartialView("_GetFunction", functions);
        }
        public PartialViewResult GetControl(int functionId)
        {
            var controls = _managerControl.Get(x => x.FunctionID == functionId).ToList();
            return PartialView("_GetControl", controls);
        }
    }
}
