using System.IO;
using PagedList;
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
    public class UsersController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        //
        // GET: /User/

        public ActionResult Index(int? page)
        {
            var users = _db.Users.OrderByDescending(x => x.ID).ToList();
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.PageSize = PageSize;
            return View(users.ToPagedList(pageNumber, pageSize));
        }
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        //
        // GET: /UOM/Details/5

        public ActionResult Details(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            ViewBag.Role = _db.Roles.ToList();
            ViewBag.Company = _db.Companies.ToList();
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file, User user, FormCollection formCollection)
        {
            string newRole = Request.Form["cbRol"] ?? "0,0";
            string company = Request.Form["ddlCompany"];
            var cbrol = newRole.Split(',').Reverse().ToList<string>();
            string url = "";
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/Users/"), fileName);
                file.SaveAs(path);
                url = "~/Uploads/Users/" + fileName;
            }
            else
            {
                url = "~/Uploads/noimage.jpg";
            }
            if (ModelState.IsValid)
            {
                user.Avartar = url;
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = "123456";
                    user.Password = Password.Encode(user.Password);
                }
                user.CreatedDate = DateTime.Now;
                user.Active = false;
                if (string.IsNullOrEmpty(company))
                {
                    if (TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser != null)
                    {
                        user.CompanyID = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.CompanyID;
                    }
                    else
                    {
                        RedirectToAction("Index", "Login");
                    }

                }
                else
                {
                    user.CompanyID = Convert.ToInt32(company);
                }
                _db.Users.Add(user);
                _db.SaveChanges();

                if (newRole != "0,0")
                {
                    foreach (var item in cbrol)
                    {
                        var userInRole = new UsersInRole();
                        userInRole.UserID = user.ID;
                        userInRole.RoleID = Convert.ToInt32(item);
                        _db.UsersInRoles.Add(userInRole);
                    }
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(user);
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.Role = _db.Roles.ToList();
            var list = (_db.UsersInRoles.Where(u => u.UserID == id).ToList()).Select(item => item.RoleID).ToList();
            ViewBag.RoleCheck = list;
            ViewBag.Company = _db.Companies.ToList();
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();

            }

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]

        public ActionResult Edit(HttpPostedFileBase file, User user, FormCollection formCollection)
        {
            string roleOld = Request.Form["cbRol"] ?? "0,0";
            var cbrolOld = roleOld.Split(',').Reverse().ToList<string>();
            string company = Request.Form["ddlCompany"];
            string roleNew = Request.Form["cbRol2"] ?? "0,0";
            var cbrolNew = roleNew.Split(',').Reverse();
            var listTemp = new List<UsersInRole>();
            //Update role cũ
            if (roleOld != "0,0")
            {
                var userInroles = _db.UsersInRoles.Where(u => u.UserID == user.ID).ToList();
                foreach (var usersInRole in userInroles)
                {
                    if (!cbrolOld.Contains(usersInRole.RoleID.ToString()))
                    {
                        listTemp.Add(usersInRole);
                    }
                }
                foreach (var usersInRole in listTemp)
                {
                    _db.UsersInRoles.Remove(usersInRole);
                }
                _db.SaveChanges();
            }
            //Role mới
            if (roleNew != "0,0")
            {
                foreach (var itemNew in cbrolNew)
                {
                    var userInRole = new UsersInRole();
                    userInRole.UserID = user.ID;
                    userInRole.RoleID = Convert.ToInt32(itemNew);
                    _db.UsersInRoles.Add(userInRole);
                }
                _db.SaveChanges();
            }
            string url = "";
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads/Users/"), fileName);
                file.SaveAs(path);
                url = "~/Uploads/Users/" + fileName;
            }
            else
            {
                url = formCollection["Avartar"];
            }
            if (ModelState.IsValid)
            {
                user.Password = Password.Encode(user.Password);
                user.Active = user.Active ?? false;
                user.Avartar = url;
                if (string.IsNullOrEmpty(company))
                {
                    if (TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser != null)
                    {
                        user.CompanyID = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser.CompanyID;
                    }
                    else
                    {
                        RedirectToAction("Index", "Login");
                    }

                }
                else
                {
                    user.CompanyID = Convert.ToInt32(company);
                }
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        public ActionResult Delete(int id = 0)
        {
            User user = _db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5
        [HttpPost]
        public string DeleteConfirmed(string[] id)
        {
            string result = "success";
            try
            {
                foreach (var item in id)
                {
                    int userId = Convert.ToInt32(item);
                    List<UsersInRole> usersInRoles = _db.UsersInRoles.Where(u => u.UserID == userId).ToList();
                    foreach (var x in usersInRoles)
                    {
                        _db.UsersInRoles.Remove(x);
                    }
                    _db.SaveChanges();
                    User user = _db.Users.Find(Convert.ToInt32(item));
                    _db.Users.Remove(user);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public bool UpdateActive(int id)
        {
            bool result = true;
            try
            {
                var user = _db.Users.Find(id);
                if (user != null)
                {
                    user.Active = user.Active != true;
                    _db.Entry(user).State = EntityState.Modified;
                }
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;

        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}