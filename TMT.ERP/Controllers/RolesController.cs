using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using PagedList;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
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
    public class RolesController : BaseController
    {
        readonly GenericManager<Role> _managerRole = new GenericManager<Role>();
        readonly GenericManager<UsersInRole> _managerUr = new GenericManager<UsersInRole>();
        readonly GenericManager<User> _managerUser = new GenericManager<User>();
        readonly GenericManager<Feature> _managerFeature = new GenericManager<Feature>();
        readonly GenericManager<Function> _managerFunction = new GenericManager<Function>();
        readonly GenericManager<Control> _managerControl = new GenericManager<Control>();
        readonly GenericManager<ControlInRole> _managerControlInRole = new GenericManager<ControlInRole>();

        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        public ActionResult Index(int? page)
        {
            var roles = _managerRole.Get().OrderByDescending(x => x.ID).ToList();
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(roles.ToPagedList(pageNumber, pageSize));
        }

        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        public ActionResult Create()
        {
            ViewBag.ListUser = _managerUser.Get().ToList();
            ViewBag.FeatureID = _managerFeature.Get().ToList();
            ViewBag.FunctionID = _managerFunction.Get().ToList();
            ViewBag.ControlID = _managerControl.Get().ToList();
            return View();
        }

        public string Save(int? roleId, string roleName, string description, string controls, string userId)
        {
            string result = "success";
            try
            {
                var oRole = _managerRole.FindById(roleId);
                if (oRole == null)
                {
                    oRole = new Role();
                    oRole.RoleName = roleName;
                    oRole.Description = description;
                    _managerRole.Add(oRole);
                }
                dynamic arrayuser = JsonConvert.DeserializeObject(userId);
                foreach (var user in arrayuser)
                {
                    var id = Int32.Parse(((string)user.id));
                    var oUserInRole = new UsersInRole();
                    oUserInRole.UserID = id;
                    oUserInRole.RoleID = oRole.ID;
                    oRole.UsersInRoles.Add(oUserInRole);
                }

                dynamic array = JsonConvert.DeserializeObject(controls);
                foreach (var item in array)
                {
                    var controlId = Int32.Parse(((string)item.id));
                    var viewType = Int32.Parse(((string)item.viewtype));
                    var oControlInRole = new ControlInRole();
                    oControlInRole.RoleID = oRole.ID;
                    oControlInRole.ControlID = controlId;
                    oControlInRole.ViewType = viewType;
                    oRole.ControlInRoles.Add(oControlInRole);
                }
                _managerRole.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string Update(int? roleId, string roleName, string description, string controlInroles, string userInRoleNew, string userInRoleOld, string controlNew)
        {
            string result = "success";
            try
            {
                var oRole = _managerRole.FindById(roleId);
                oRole.RoleName = roleName;
                oRole.Description = description;
                dynamic arrayControlNew = JsonConvert.DeserializeObject(controlNew);
                foreach (var item in arrayControlNew)
                {
                    var id = Int32.Parse(((string)item.id));
                    var viewType = Int32.Parse(((string)item.viewtype));
                    var oControlInRole = new ControlInRole();
                    oControlInRole.RoleID = oRole.ID;
                    oControlInRole.ControlID = id;
                    oControlInRole.ViewType = viewType;
                    _managerControlInRole.Add(oControlInRole);
                }
                dynamic array = JsonConvert.DeserializeObject(controlInroles);
                foreach (var item in array)
                {
                    var id = Int32.Parse(((string)item.id));
                    var viewType = Int32.Parse(((string)item.viewtype));
                    var oControlInRole = _managerControlInRole.FindById(id);
                    oControlInRole.ViewType = viewType;
                    _managerControlInRole.Update(oControlInRole);
                }
                dynamic arrayUrNew = JsonConvert.DeserializeObject(userInRoleNew);
                foreach (var item in arrayUrNew)
                {
                    var userID = Int32.Parse(((string)item.id));
                    var oUserInRole = new UsersInRole();
                    oUserInRole.RoleID = oRole.ID;
                    oUserInRole.UserID = userID;
                    oRole.UsersInRoles.Add(oUserInRole);
                }
                dynamic arrayUrOld = JsonConvert.DeserializeObject(userInRoleOld);
                foreach (var item in arrayUrOld)
                {
                    var id = Int32.Parse(((string)item.id));
                    var status = item.status;
                    if (status == "false")
                    {
                        var oUserInRole =
                            oRole.UsersInRoles.FirstOrDefault(x => x.RoleID == oRole.ID && x.UserID == id);
                        _managerUr.Delete(oUserInRole);
                    }
                }
                _managerRole.Save();
                _managerUr.Save();
                _managerControlInRole.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public ActionResult Edit(int? id)
        {
            var oRole = _managerRole.FindById(id);
            ViewBag.FeatureID = _managerFeature.Get().ToList();
            ViewBag.FunctionID = _managerFunction.Get().ToList();

            ViewBag.UserCheck = _managerUr.Get(u => u.RoleID == id).Select(i => i.UserID).ToList();
            ViewBag.UserTotal = _managerUser.Get().ToList();
            ViewBag.ControlInRoles = oRole.ControlInRoles.ToList();


            //New Control
            var firstOrDefault = oRole.ControlInRoles.FirstOrDefault();
            var ooControlNew = new List<Control>();
            if (firstOrDefault != null)
            {
                int functionId = firstOrDefault.Control.FunctionID;
                var ooControl = _managerFunction.FindById(functionId).Controls.ToList();
                foreach (var control in ooControl)
                {
                    if (oRole.ControlInRoles.ToList().Exists(x => x.ControlID == control.ID && x.RoleID == oRole.ID))
                    {

                    }
                    else
                    {
                        ooControlNew.Add(control);
                    }
                }
            }
            ViewBag.NewControl = ooControlNew;
            //End New Control
            return View(oRole);
        }

        public string Delete(string[] roleId)
        {
            string result = "Role:";
            int countDelete = 0;
            var ooRole = new List<Role>();
            try
            {
                foreach (var item in roleId)
                {
                    var id = Convert.ToInt32(item);
                    var oRole = _managerRole.FindById(id);
                    if (oRole.UsersInRoles.Any() || oRole.UsersInRoles.Count > 0)
                    {
                        result += oRole.RoleName + " ";
                    }
                    else
                    {
                        var ooControlInRole = oRole.ControlInRoles.ToList();
                        foreach (var x in ooControlInRole)
                        {
                            _managerControlInRole.Delete(x);
                        }
                        _managerControlInRole.Save();
                        _managerRole.Delete(oRole);
                        countDelete++;
                    }
                }
                _managerRole.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            if (countDelete > 0)
            {
                result = "success";
            }
            return result;
        }

        #region Remove
        //public ActionResult Edit(Role role, FormCollection formCollection)
        //{
        //    var rolePermission = _managerRolePermission.FindById(role.ID);
        //    var cbRolePermissionNew = Request.Form["cbPerNew"] ?? "0,0";
        //    var cbRolePermissionOld = Request.Form["cbPerOld"] ?? "0,0";
        //    var perCheck = PermissionCheck(rolePermission);

        //    #region Set value when no permission checked
        //    if (cbRolePermissionNew == "0,0" && cbRolePermissionOld == "0,0")
        //    {
        //        rolePermission.CreateBillDraft =
        //            rolePermission.ViewAndEditBill =
        //            rolePermission.EditBillCreatedByOther =
        //            rolePermission.DeleteBill =
        //            rolePermission.DeleteBillCreatedByOther =
        //            rolePermission.ApproveBill =
        //            rolePermission.WorkOrderCreate =
        //            rolePermission.DoBatchPayment =
        //            rolePermission.AddNoteToBill =
        //            rolePermission.RepeatingBill =
        //            rolePermission.CreateInvoiceDraft =
        //            rolePermission.ViewAndEditInvoice =
        //            rolePermission.EditInvoiceCreatedByOther =
        //            rolePermission.DeleteInvoice =
        //            rolePermission.DeleteInvoiceCreatedByOther =
        //            rolePermission.ApproveInvoice =
        //            rolePermission.RecordtInvoicesPaymen =
        //            rolePermission.SendInvoiceStatement =
        //            rolePermission.AddNoteToInvoice =
        //            rolePermission.RepeatingInvoice = rolePermission.EditInvoicePrice = rolePermission.RecordBillPayment = rolePermission.WorkOrderCreate = rolePermission.WorkOrderApproval =
        //            rolePermission.AllocateCreate = rolePermission.AllocateApproval = rolePermission.MachinePlanningCreate = rolePermission.PlanningCreate = rolePermission.PlanningUpdateProcess
        //            = rolePermission.PlanningApproval
        //            = false;
        //        _managerRolePermission.Save();
        //    }
        //    #endregion

        //    foreach (var x in cbRolePermissionOld.Split(',').Reverse().ToList<string>())
        //    {
        //        if (perCheck.Contains(x))
        //        {
        //            perCheck.Remove(x);
        //        }
        //    }
        //    #region Update RolePermiss Old When Unchecked
        //    if (cbRolePermissionOld != "0,0")
        //    {
        //        foreach (var item in perCheck)
        //        {
        //            rolePermission.CreateBillDraft = item == "CreateBillDraft" ? false : rolePermission.CreateBillDraft;
        //            rolePermission.ViewAndEditBill = item == "ViewAndEditBill" ? false : rolePermission.ViewAndEditBill;
        //            rolePermission.EditBillCreatedByOther = item == "EditBillCreatedByOther" ? false : rolePermission.EditBillCreatedByOther;
        //            rolePermission.DeleteBill = item == "DeleteBill" ? false : rolePermission.DeleteBill;
        //            rolePermission.DeleteBillCreatedByOther = item == "DeleteBillCreatedByOther" ? false : rolePermission.DeleteBillCreatedByOther;
        //            rolePermission.ApproveBill = item == "ApproveBill" ? false : rolePermission.ApproveBill;
        //            rolePermission.WorkOrderCreate = item == "WorkOrderCreate" ? false : rolePermission.WorkOrderCreate;
        //            rolePermission.DoBatchPayment = item == "DoBatchPayment" ? false : rolePermission.DoBatchPayment;
        //            rolePermission.AddNoteToBill = item == "AddNoteToBill" ? false : rolePermission.AddNoteToBill;
        //            rolePermission.RepeatingBill = item == "RepeatingBill" ? false : rolePermission.RepeatingBill;
        //            rolePermission.CreateInvoiceDraft = item == "CreateInvoiceDraft" ? false : rolePermission.CreateInvoiceDraft;
        //            rolePermission.ViewAndEditInvoice = item == "ViewAndEditInvoice" ? false : rolePermission.ViewAndEditInvoice;
        //            rolePermission.EditInvoiceCreatedByOther = item == "EditInvoiceCreatedByOther" ? false : rolePermission.EditInvoiceCreatedByOther;
        //            rolePermission.DeleteInvoice = item == "DeleteInvoice" ? false : rolePermission.DeleteInvoice;
        //            rolePermission.DeleteInvoiceCreatedByOther = item == "DeleteInvoiceCreatedByOther" ? false : rolePermission.DeleteInvoiceCreatedByOther;
        //            rolePermission.ApproveInvoice = item == "ApproveInvoice" ? false : rolePermission.ApproveInvoice;
        //            rolePermission.RecordtInvoicesPaymen = item == "RecordtInvoicesPaymen" ? false : rolePermission.RecordtInvoicesPaymen;
        //            rolePermission.SendInvoiceStatement = item == "SendInvoiceStatement" ? false : rolePermission.SendInvoiceStatement;
        //            rolePermission.AddNoteToInvoice = item == "AddNoteToInvoice" ? false : rolePermission.AddNoteToInvoice;
        //            rolePermission.RepeatingInvoice = item == "RepeatingInvoice" ? false : rolePermission.RepeatingInvoice;
        //            rolePermission.EditInvoicePrice = item == "EditInvoicePrice" ? false : rolePermission.EditInvoicePrice;
        //            rolePermission.RecordBillPayment = item == "RecordBillPayment" ? false : rolePermission.RecordBillPayment;
        //            rolePermission.WorkOrderCreate = item == "WorkOrderCreate" ? false : rolePermission.WorkOrderCreate;
        //            rolePermission.WorkOrderApproval = item == "WorkOrderApproval" ? false : rolePermission.WorkOrderApproval;
        //            rolePermission.AllocateCreate = item == "AllocateCreate" ? false : rolePermission.AllocateCreate;
        //            rolePermission.AllocateApproval = item == "AllocateApproval" ? false : rolePermission.AllocateApproval;
        //            rolePermission.MachinePlanningCreate = item == "MachinePlanningCreate" ? false : rolePermission.MachinePlanningCreate;
        //            rolePermission.PlanningCreate = item == "PlanningCreate" ? false : rolePermission.PlanningCreate;
        //            rolePermission.PlanningUpdateProcess = item == "PlanningUpdateProcess" ? false : rolePermission.PlanningUpdateProcess;
        //            rolePermission.PlanningApproval = item == "PlanningApproval" ? false : rolePermission.PlanningApproval;
        //        }
        //        _managerRolePermission.Save();
        //    }
        //    #endregion
        //    #region Update RolePermiss New When Checked
        //    if (cbRolePermissionNew != "0,0")
        //    {
        //        foreach (var item in cbRolePermissionNew.Split(',').Reverse().ToList<string>())
        //        {
        //            rolePermission.CreateBillDraft = item == "CreateBillDraft" ? true : rolePermission.CreateBillDraft;
        //            rolePermission.ViewAndEditBill = item == "ViewAndEditBill" ? true : rolePermission.ViewAndEditBill;
        //            rolePermission.EditBillCreatedByOther = item == "EditBillCreatedByOther" ? true : rolePermission.EditBillCreatedByOther;
        //            rolePermission.DeleteBill = item == "DeleteBill" ? true : rolePermission.DeleteBill;
        //            rolePermission.DeleteBillCreatedByOther = item == "DeleteBillCreatedByOther" ? true : rolePermission.DeleteBillCreatedByOther;
        //            rolePermission.ApproveBill = item == "ApproveBill" ? true : rolePermission.ApproveBill;
        //            rolePermission.WorkOrderCreate = item == "WorkOrderCreate" ? true : rolePermission.WorkOrderCreate;
        //            rolePermission.DoBatchPayment = item == "DoBatchPayment" ? true : rolePermission.DoBatchPayment;
        //            rolePermission.AddNoteToBill = item == "AddNoteToBill" ? true : rolePermission.AddNoteToBill;
        //            rolePermission.RepeatingBill = item == "RepeatingBill" ? true : rolePermission.RepeatingBill;
        //            rolePermission.CreateInvoiceDraft = item == "CreateInvoiceDraft" ? true : rolePermission.CreateInvoiceDraft;
        //            rolePermission.ViewAndEditInvoice = item == "ViewAndEditInvoice" ? true : rolePermission.ViewAndEditInvoice;
        //            rolePermission.EditInvoiceCreatedByOther = item == "EditInvoiceCreatedByOther" ? true : rolePermission.EditInvoiceCreatedByOther;
        //            rolePermission.DeleteInvoice = item == "DeleteInvoice" ? true : rolePermission.DeleteInvoice;
        //            rolePermission.DeleteInvoiceCreatedByOther = item == "DeleteInvoiceCreatedByOther" ? true : rolePermission.DeleteInvoiceCreatedByOther;
        //            rolePermission.ApproveInvoice = item == "ApproveInvoice" ? true : rolePermission.ApproveInvoice;
        //            rolePermission.RecordtInvoicesPaymen = item == "RecordtInvoicesPaymen" ? true : rolePermission.RecordtInvoicesPaymen;
        //            rolePermission.SendInvoiceStatement = item == "SendInvoiceStatement" ? true : rolePermission.SendInvoiceStatement;
        //            rolePermission.AddNoteToInvoice = item == "AddNoteToInvoice" ? true : rolePermission.AddNoteToInvoice;
        //            rolePermission.RepeatingInvoice = item == "RepeatingInvoice" ? true : rolePermission.RepeatingInvoice;
        //            rolePermission.EditInvoicePrice = item == "EditInvoicePrice" ? true : rolePermission.EditInvoicePrice;
        //            rolePermission.RecordBillPayment = item == "RecordBillPayment" ? true : rolePermission.RecordBillPayment;
        //            rolePermission.WorkOrderCreate = item == "WorkOrderCreate" ? true : rolePermission.WorkOrderCreate;
        //            rolePermission.WorkOrderApproval = item == "WorkOrderApproval" ? true : rolePermission.WorkOrderApproval;
        //            rolePermission.AllocateCreate = item == "AllocateCreate" ? true : rolePermission.AllocateCreate;
        //            rolePermission.AllocateApproval = item == "AllocateApproval" ? true : rolePermission.AllocateApproval;
        //            rolePermission.MachinePlanningCreate = item == "MachinePlanningCreate" ? true : rolePermission.MachinePlanningCreate;
        //            rolePermission.PlanningCreate = item == "PlanningCreate" ? true : rolePermission.PlanningCreate;
        //            rolePermission.PlanningUpdateProcess = item == "PlanningUpdateProcess" ? true : rolePermission.PlanningUpdateProcess;
        //            rolePermission.PlanningApproval = item == "PlanningApproval" ? true : rolePermission.PlanningApproval;
        //        }
        //        _managerRolePermission.Save();

        //    }

        //    #endregion
        //    #region Update UserInRole Whn Unchecked

        //    var userOldCheck = Request.Form["cbusercheck"] ?? "0,0";
        //    var urOldPending = userOldCheck.Split(',').Reverse().ToList<string>();
        //    var urOld = _managerUr.Get(u => u.RoleID == role.ID).ToList();

        //    var urRemove = new List<UsersInRole>();
        //    foreach (var item in urOld)
        //    {
        //        var userId = item.UserID.ToString(CultureInfo.InvariantCulture);
        //        if (!urOldPending.Contains(userId))
        //        {
        //            urRemove.Add(item);
        //        }
        //    }
        //    foreach (var item in urRemove)
        //    {
        //        _managerUr.Delete(item);
        //    }
        //    #endregion
        //    #region Update UserInRole When Checked
        //    var userCheck = Request.Form["cbuserUncheck"] ?? "0,0";
        //    if (userCheck != "0,0")
        //    {
        //        foreach (var x in userCheck.Split(',').Reverse().ToList<string>())
        //        {
        //            var userInRole = new UsersInRole
        //                                 {
        //                                     RoleID = role.ID,
        //                                     UserID = Convert.ToInt32(x)
        //                                 };
        //            _managerUr.Add(userInRole);
        //        }
        //    }
        //    #endregion
        //    if (ModelState.IsValid)
        //    {
        //        Role roleTemp = _managerRole.FindById(role.ID);
        //        roleTemp.RoleName = role.RoleName;
        //        roleTemp.Description = role.Description;
        //        _managerRole.Save();
        //        return RedirectToAction("Index");
        //    }
        //    _managerRolePermission.Save();
        //    _managerUr.Save();

        //    return View(role);
        //}
        //public ActionResult Delete(int id = 0)
        //{
        //    Role role = _managerRole.FindById(id);
        //    if (role == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(role);
        //}        
        //public ActionResult Delete(string[] roleId)
        //{

        //    foreach (var item in roleId)
        //    {
        //        int _x = Convert.ToInt32(item);
        //        var ur = _managerUr.Get(x => x.RoleID == _x).ToList();
        //        foreach (var x in ur)
        //        {
        //            _managerUr.Delete(x);
        //        }
        //    }
        //    _managerUr.Save();
        //    foreach (var item in roleId)
        //    {
        //        var rolePer = _managerRolePermission.FindById(Convert.ToInt32(item));
        //        var role = _managerRole.FindById(Convert.ToInt32(item));
        //        _managerRolePermission.Delete(rolePer);
        //        _managerRole.Delete(role);
        //    }
        //    _managerRolePermission.Save();
        //    _managerRole.Save();
        //    return View();
        //}        
        //public List<string> GetPermission()
        //{
        //    var oPermission = new RolePermission();
        //    PropertyInfo[] properties = oPermission.GetType().GetProperties();
        //    return properties.Select(pi => pi.Name).Where(field => field != "ID" && field != "Role" && field != "EntityState" && field != "IsTracked").ToList();
        //}
        //public List<string> PermissionCheck(RolePermission rolePermission)
        //{
        //    Type type = rolePermission.GetType();
        //    return (from p in type.GetProperties() let name = p.Name let value = p.GetValue(rolePermission, null) ?? "Role" let x = value.ToString() where x == "True" || x.Equals("True") select name).ToList();
        //}
        #endregion
    }
}
