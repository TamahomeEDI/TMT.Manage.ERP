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
    public class CompanyController : BaseController //Controller
    {
        private ErpEntities db = new ErpEntities();

        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        //
        // GET: /Company/

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
            if (searchString == null)
            {
                var comp = db.Companies.OrderByDescending(p => p.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                if (result != "")
                {
                    result = "";
                }
                return View(comp.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var comp = db.Companies.Where(a => a.DisplayName.Contains(searchString) || a.TradingName.Contains(searchString)).OrderByDescending(p => p.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                if (result != "")
                {
                    result = "";
                }
                return View(comp.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /Company/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Company/Create

        [HttpPost]
        public ActionResult Create(string url, string name, string legal, string bussiness,
            string attention, string address, string zipcode, string state, string city, string telCountry,
            string telArea, string telNo, string description, string email, string website, string country,
            string pCountry, string pAttention, string pAddress, string pZipcode, string pState, string pCity,
            string organization)
        {
            Company company = new Company();
            company.DisplayName = name;
            company.TradingName = legal;
            company.Business = bussiness;
            company.OrgaType = organization;
            company.Description = description;
            company.PostAttention = attention;
            company.PostAddress = address;
            company.PostCity = city;
            company.PostState = state;
            company.PostZipCode = zipcode;
            company.PostCountry = country;
            company.PhysAttention = pAttention;
            company.PhysAddress = pAddress;
            company.PhysCity = pCity;
            company.PhysState = pState;
            company.PhysZipCode = pZipcode;
            company.PhysCountry = pCountry;
            company.Tel_Country = telCountry;
            company.Tel_Area = telArea;
            company.Tel_Num = telNo;
            company.Email = email;
            company.Website = website;
            company.Logo = !string.IsNullOrEmpty(url) ? url : "/Uploads/noimage.jpg";
            db.Companies.Add(company);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //
        // GET: /Company/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            ViewBag.Logo = company.Logo;
            return View(company);
        }

        //
        // POST: /Company/Edit/5

        [HttpPost]
        public ActionResult Edit(string url, string name, string legal, string bussiness,
            string attention, string address, string zipcode, string state, string city, string telCountry,
            string telArea, string telNo, string description, string email, string website, string country,
            string pCountry, string pAttention, string pAddress, string pZipcode, string pState, string pCity,
            string organization, int id)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            else
            {
                company.DisplayName = name;
                company.TradingName = legal;
                company.Business = bussiness;
                company.OrgaType = organization;
                company.Description = description;
                company.PostAttention = attention;
                company.PostAddress = address;
                company.PostCity = city;
                company.PostState = state;
                company.PostZipCode = zipcode;
                company.PostCountry = country;
                company.PhysAttention = pAttention;
                company.PhysAddress = pAddress;
                company.PhysCity = pCity;
                company.PhysState = pState;
                company.PhysZipCode = pZipcode;
                company.PhysCountry = pCountry;
                company.Tel_Country = telCountry;
                company.Tel_Area = telArea;
                company.Tel_Num = telNo;
                company.Email = email;
                company.Website = website;
                company.Logo = !string.IsNullOrEmpty(url) ? url : "/Uploads/noimage.jpg";
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        //
        // POST: /Company/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Company comp = db.Companies.Find(id);
                if (comp != null)
                {
                    try
                    {
                        db.Companies.Remove(comp);
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