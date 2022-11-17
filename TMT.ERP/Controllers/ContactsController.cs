using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.DataAccess.Model;
using System.Text;
using System.Data.Entity.Validation;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;
using TMT.ERP.BusinessLogic.Managers;


namespace TMT.ERP.Controllers
{
    public class ContactsController : BaseController
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


        #region Contacts
        //
        // GET: /Contact/
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
                var contacts = db.Contacts.OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var contacts = db.Contacts.Where(c => c.ContactName.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
        }



        //Supplier
        public ActionResult Supplier(string currentFilter, int? page, string searchString)
        {
            if (searchString == null)
            {
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                var contacts = db.Contacts.Where(c => c.Type == 0 || c.Type == 2).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var contacts = db.Contacts.Where(c => c.ContactName.Contains(searchString) && (c.Type == 0 || c.Type == 2)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
        }

        //Customer
        public ActionResult Customer(string currentFilter, int? page, string searchString)
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
                var contacts = db.Contacts.Where(c => c.Type == 1 || c.Type == 2).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var contacts = db.Contacts.Where(c => c.ContactName.Contains(searchString) && (c.Type == 1 || c.Type == 2)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(contacts.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /Contact/Create

        public ActionResult Create()
        {
            ViewBag.SaleTaxID = new SelectList(db.TaxRates, "ID", "Name");
            ViewBag.PurchaseTaxID = new SelectList(db.TaxRates, "ID", "Name");
            ViewBag.CurrencyID = CreateCurrencyList();
            return View();
        }

        private object CreateCurrencyList()
        {
            var manager = new GenericManager<Currency>();
            var currencyItems = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().OrderBy(x=>x.Text).ToList();
            return currencyItems;
        }

        public int ExistsContactName(string contactName, int id)
        {
            var checkresult = 0;
            Contact contact = db.Contacts.Where(p => p.ID != id && p.ContactName == contactName).FirstOrDefault();
            if (contact == null)
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
        // POST: /Contact/Create

        [HttpPost]
        public string Create(string contactname, string fname, string lname, string attention, string address,
            string zipcode, string state, string city, string telCountry, string telArea, string telNo,
            string description, string type, string email, string website, string skyName, string taxNo,
            int? saleTaxID, int? purchaseTaxID, string country, string pCountry, string pAttention,
            string pAddress, string pZipcode, string pState, string pCity, string dCountry, string dArea,
            string dNo, string mCountry, string mArea, string mNo, string fCountry, string fArea,
            string fNo, float? discount, int? currencyID, string baName, string baNumber, string baDetails)
        {
            Contact contact = new Contact();
            contact.ContactName = contactname;
            contact.FirstName = fname;
            contact.LastName = lname;
            contact.PostAttention = attention;
            contact.PostAddress = address;
            contact.PostZipCode = zipcode;
            contact.PostState = state;
            contact.PostCity = city;
            contact.PostCountry = country;
            contact.PhysAttention = pAttention;
            contact.PhysAddress = pAddress;
            contact.PhysCity = pCity;
            contact.PhysState = pState;
            contact.PhysZipCode = pZipcode;
            contact.PhysCountry = pCountry;
            contact.SkypeName = skyName;
            contact.Website = website;
            contact.Email = email;
            contact.Tel_Country = telCountry;
            contact.Tel_Area = telArea;
            contact.Tel_Num = telNo;
            contact.Direct_Country = dCountry;
            contact.Direct_Area = dArea;
            contact.Direct_Num = dNo;
            contact.Mobi_Country = mCountry;
            contact.Mobi_Area = mArea;
            contact.Mobi_Num = mNo;
            contact.Fax_Country = fCountry;
            contact.Fax_Area = fArea;
            contact.Fax_Num = fNo;
            contact.TaxNumber = taxNo;
            contact.SaleTaxID = saleTaxID;
            contact.PurchasesTaxID = purchaseTaxID;
            if (discount != null)
            {
                contact.Discount = discount;
            }
            contact.CurrencyID = currencyID;
            contact.BankAccountName = baName;
            contact.BankAccoutNum = baNumber;
            contact.BankAccountDetail = baDetails;
            contact.Type = Int32.Parse(type);
            contact.Description = description;
            db.Contacts.Add(contact);
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
        // GET: /Contact/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            ViewBag.SaleTaxID = new SelectList(db.TaxRates, "ID", "Name", contact.SaleTaxID);
            ViewBag.PurchaseTaxID = new SelectList(db.TaxRates, "ID", "Name", contact.PurchasesTaxID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name", contact.CurrencyID);
            return View(contact);
        }

        //
        // POST: /Contact/Edit/5

        [HttpPost]
        public string Edit(string contactname, string fname, string lname, string attention, string address,
            string zipcode, string state, string city, string telCountry, string telArea, string telNo,
            string description, string type, string email, string website, string skyName, string taxNo,
            int? saleTaxID, int? purchaseTaxID, string country, int? id, string pCountry,
            string pAttention, string pAddress, string pZipcode, string pState, string pCity, string dCountry,
            string dArea, string dNo, string mCountry, string mArea, string mNo, string fCountry, string fArea,
            string fNo, float? discount, int? currencyID, string baName, string baNumber, string baDetails)
        {
            Contact contact = db.Contacts.Find(id);
            contact.ContactName = contactname;
            contact.FirstName = fname;
            contact.LastName = lname;
            contact.PostAttention = attention;
            contact.PostAddress = address;
            contact.PostZipCode = zipcode;
            contact.PostState = state;
            contact.PostCity = city;
            contact.PostCountry = country;
            contact.PhysAttention = pAttention;
            contact.PhysAddress = pAddress;
            contact.PhysCity = pCity;
            contact.PhysState = pState;
            contact.PhysZipCode = pZipcode;
            contact.PhysCountry = pCountry;
            contact.SkypeName = skyName;
            contact.Website = website;
            contact.Email = email;
            contact.Tel_Country = telCountry;
            contact.Tel_Area = telArea;
            contact.Tel_Num = telNo;
            contact.Direct_Country = dCountry;
            contact.Direct_Area = dArea;
            contact.Direct_Num = dNo;
            contact.Mobi_Country = mCountry;
            contact.Mobi_Area = mArea;
            contact.Mobi_Num = mNo;
            contact.Fax_Country = fCountry;
            contact.Fax_Area = fArea;
            contact.Fax_Num = fNo;
            contact.TaxNumber = taxNo;
            contact.SaleTaxID = saleTaxID;
            contact.PurchasesTaxID = purchaseTaxID;
            if (discount != null)
            {
                contact.Discount = discount;
            }
            contact.CurrencyID = currencyID;
            contact.BankAccountName = baName;
            contact.BankAccoutNum = baNumber;
            contact.BankAccountDetail = baDetails;
            contact.Type = Int32.Parse(type);
            contact.Description = description;
            db.Entry(contact).State = EntityState.Modified;
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
        // POST: /Contact/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Contact contact = db.Contacts.Find(id);
                if (contact != null)
                {
                    try
                    {
                        db.Contacts.Remove(contact);
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

        #endregion

        #region Employee
        //Employee
        public ActionResult Employee(string currentFilter, int? page, string searchString)
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
                var employee = db.Employees.OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(employee.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var employee = db.Employees.Where(c => c.Code.Contains(searchString)).OrderByDescending(s => s.ID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(employee.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /Contacts/AddEmployee

        public ActionResult AddEmployee()
        {
            //For Dropdownlist
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName");
            ViewBag.EmpCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.Employee));
            return View();
        }

        //
        // POST: /Contacts/AddEmployee

        [HttpPost]
        public string AddEmployee(string fname, string lname, string attention, string address, string code,
            string zipcode, string state, string city, string telCountry, string telArea, string telNo,
            string email, string taxNo, string country, string pCountry, string pAttention, string twi,
            string ordRate, string pAddress, string pZipcode, string pState, string pCity, string dCountry,
            string dArea, string dNo, string mCountry, string mArea, string mNo, string fCountry,
            string fArea, string fNo, string baName, string baNumber, string baDetails, int company)
        {
            Employee employee = new Employee();
            employee.FirstName = fname;
            employee.LastName = lname;
            employee.Code = code;
            employee.CompanyID = company;
            employee.PostAttention = attention;
            employee.PostAddress = address;
            employee.PostZipCode = zipcode;
            employee.PostState = state;
            employee.PostCity = city;
            employee.PostCountry = country;
            employee.PhysAttention = pAttention;
            employee.PhysAddress = pAddress;
            employee.PhysCity = pCity;
            employee.PhysState = pState;
            employee.PhysZipCode = pZipcode;
            employee.PhysCountry = pCountry;
            employee.Twitter = twi;
            employee.Email = email;
            employee.Tel_Country = telCountry;
            employee.Tel_Area = telArea;
            employee.Tel_Num = telNo;
            employee.Direct_Country = dCountry;
            employee.Direct_Area = dArea;
            employee.Direct_Num = dNo;
            employee.Mobi_Country = mCountry;
            employee.Mobi_Area = mArea;
            employee.Mobi_Num = mNo;
            employee.Fax_Country = fCountry;
            employee.Fax_Area = fArea;
            employee.Fax_Num = fNo;
            employee.TaxNumber = taxNo;
            employee.OrdinaryRate = ordRate;
            employee.BankAccountName = baName;
            employee.BankAccountNumber = baNumber;
            employee.PaymentDetails = baDetails;
            db.Employees.Add(employee);
            try
            {
                db.SaveChanges();
                Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.Employee), Constant.CODE_LENGTH, "");
                result = "Create";
            }
            catch (Exception e) {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /Contacts/EditEmployee/

        public ActionResult EditEmployee(int id = 0)
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
        // POST: /Employee/EditEmployee/

        [HttpPost]
        public string EditEmployee(string fname, string lname, string attention, string address,
            string zipcode, string state, string city, string telCountry, string telArea, string telNo,
            string email, string taxNo, string country, string pCountry, string pAttention, string twi,
            string ordRate, string pAddress, string pZipcode, string pState, string pCity, string dCountry,
            string dArea, string dNo, string mCountry, string mArea, string mNo, string fCountry, int id,
            string fArea, string fNo, string baName, string baNumber, string baDetails, int company)
        {

            Employee employee = db.Employees.Find(id);

                employee.FirstName = fname;
                employee.LastName = lname;
                employee.CompanyID = company;
                employee.PostAttention = attention;
                employee.PostAddress = address;
                employee.PostZipCode = zipcode;
                employee.PostState = state;
                employee.PostCity = city;
                employee.PostCountry = country;
                employee.PhysAttention = pAttention;
                employee.PhysAddress = pAddress;
                employee.PhysCity = pCity;
                employee.PhysState = pState;
                employee.PhysZipCode = pZipcode;
                employee.PhysCountry = pCountry;
                employee.Twitter = twi;
                employee.Email = email;
                employee.Tel_Country = telCountry;
                employee.Tel_Area = telArea;
                employee.Tel_Num = telNo;
                employee.Direct_Country = dCountry;
                employee.Direct_Area = dArea;
                employee.Direct_Num = dNo;
                employee.Mobi_Country = mCountry;
                employee.Mobi_Area = mArea;
                employee.Mobi_Num = mNo;
                employee.Fax_Country = fCountry;
                employee.Fax_Area = fArea;
                employee.Fax_Num = fNo;
                employee.TaxNumber = taxNo;
                employee.OrdinaryRate = ordRate;
                employee.BankAccountName = baName;
                employee.BankAccountNumber = baNumber;
                employee.PaymentDetails = baDetails;
                db.Entry(employee).State = EntityState.Modified;
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

        //
        // POST: /Contacts/Delete/5

        [HttpPost]
        public string Delete(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                Employee emp = db.Employees.Find(id);
                if (emp != null)
                {
                    try
                    {
                        db.Employees.Remove(emp);
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

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
