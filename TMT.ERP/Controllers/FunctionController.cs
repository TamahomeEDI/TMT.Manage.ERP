using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    //[AjaxAuthorize(Roles = "1,2,3,4,5")]
    public class FunctionController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        private readonly GenericManager<Function> _manager = new GenericManager<Function>();
        private readonly GenericManager<Feature> _managerFeature = new GenericManager<Feature>();
        private readonly GenericManager<Control> _managerControl = new GenericManager<Control>();
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        public static int FeatureID = 0;
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public int SetFeature(string featureId)
        {
            FeatureID = Convert.ToInt32(featureId);
            return FeatureID;
        }
        public ActionResult Index(int? page)
        {
            ViewBag.FeatureID = _managerFeature.Get().ToList();
            var functions = _manager.Get().OrderByDescending(x => x.ID).ToList();
            functions = FeatureID == 0 ? functions.ToList() : functions.Where(x => x.FeatureID == FeatureID).ToList();
            ViewBag.Feature = FeatureID;
            FeatureID = 0;
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
            return View(functions.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {            
            ViewBag.FeatureID = _db.Features.ToList();
            return View();
        }        
        public ActionResult Edit(int? id)
        {
            var oFunction = _manager.FindById(id);
            if (oFunction == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = id;
            ViewBag.FeatureIDSelect = oFunction.FeatureID ?? 0;
            ViewBag.FeatureID = _db.Features.ToList();
            return View(oFunction);
        }
        public string Delete(string[] id)
        {
            string result = "";
            try
            {
                var ooControls = _managerControl.Get().ToList();
                var noDelete = new List<int>();
                foreach (var item in id)
                {
                    int functionID = Convert.ToInt32(item);
                    if (ooControls.Exists(x => x.FunctionID == functionID))
                    {
                        noDelete.Add(functionID);
                    }
                    else
                    {
                        var oFunction = _manager.FindById(functionID);
                        _manager.Delete(oFunction);
                    }
                }
                _manager.Save();
                foreach (var x in noDelete)
                {
                    var oFunction = _manager.FindById(x);
                    result += "Func: " + oFunction.Name;
                }
            }
            catch (Exception ex)
            {
                result = ex.ToString();
            }
            return result;
        }
        public string Save(int? functionId, string name, string description, int featureId)
        {
            string result = "success";
            try
            {
                var oFunction = _manager.FindById(functionId);
                if (oFunction == null)
                {
                    oFunction = new Function();
                    oFunction.Name = name;
                    oFunction.Description = description ?? name;
                    oFunction.FeatureID = featureId;
                    _manager.Add(oFunction);
                }
                else
                {
                    oFunction.Name = name;
                    oFunction.Description = description ?? name;
                    oFunction.FeatureID = featureId;
                    _manager.Update(oFunction);
                }
                _manager.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
//public class AjaxAuthorizeAttribute : AuthorizeAttribute
//{
//    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
//    {
//        if (filterContext.HttpContext.Request.IsAjaxRequest())
//        {
//            // Fire back an unauthorized response
//            filterContext.HttpContext.Response.StatusCode = 403;
//        }
//        else
//            base.HandleUnauthorizedRequest(filterContext);
//    }
//}