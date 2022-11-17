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
using TMT.ERP.BusinessLogic.Utils;
using TMT.ERP.Commons;


namespace TMT.ERP.Controllers
{
    public class AccountTypesController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /AccountType/
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
                return View(db.AccountTypes.ToList().ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountType> accType = db.AccountTypes.Where(c => c.Name.Contains(searchString)).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(accType.ToPagedList(pageNumber, pageSize));
            }
        }

        public int ExistsName(string name, int id)
        {
            var checkresult = 0;
            AccountType proType = db.AccountTypes.Where(p => p.Name == name && p.ID != id).FirstOrDefault();
            if (proType == null)
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
        // GET: /AccountType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AccountType/Create

        [HttpPost]
        public string Create(string name)
        {
            AccountType accounttype = new AccountType();
            accounttype.Name = name;
            accounttype.ParentID = 0;
            db.AccountTypes.Add(accounttype);
            try
            {
                db.SaveChanges();
                result = "Create";
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /AccountType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            AccountType accounttype = db.AccountTypes.Find(id);
            if (accounttype == null)
            {
                return HttpNotFound();
            }
            return View(accounttype);
        }

        //
        // POST: /AccountType/Edit/5

        [HttpPost]
        public string Edit(string name, int id)
        {
            AccountType accounttype = db.AccountTypes.Find(id);

            accounttype.Name = name;
            accounttype.ParentID = 0;
            db.Entry(accounttype).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                result = "Edit";
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;
        }
        //
        // POST: /AccountType/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
                foreach (var item in delConfirm)
                {
                    int id = Int32.Parse(item);
                    AccountType acc = db.AccountTypes.Find(id);
                    try
                    {
                        db.AccountTypes.Remove(acc);
                        db.SaveChanges();
                        result = "Success";
                    }
                    catch (Exception e) {
                        result = e.ToString();
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
