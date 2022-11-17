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
    public class AccountsController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /Account/
        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        // All Accounts
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
                List<AccountInfo> account = AccountManager.GetAllAccount().OrderByDescending(b => b.AccountType).ThenByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString)).OrderByDescending(b => b.AccountType).ThenByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Assets
        public ActionResult Assets(string currentFilter, int? page, string searchString)
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
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.TypeID == 24).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString) && a.TypeID == 24).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Liabilities
        public ActionResult Liabilities(string currentFilter, int? page, string searchString)
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
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.TypeID == 25).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString) && a.TypeID == 25).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Equity
        public ActionResult Equity(string currentFilter, int? page, string searchString)
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
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.TypeID == 26).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString) && a.TypeID == 26).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Expenses
        public ActionResult Expenses(string currentFilter, int? page, string searchString)
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
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.TypeID == 27).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString) && a.TypeID == 27).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Revenue
        public ActionResult Revenue(string currentFilter, int? page, string searchString)
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
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.TypeID == 29).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountInfo> account = AccountManager.GetAllAccount().Where(a => a.AccountName.Contains(searchString) && a.TypeID == 29).OrderByDescending(a => a.AccountID).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
                ViewBag.Error = result;
                result = "";
                return View(account.ToPagedList(pageNumber, pageSize));
            }
        }

        //Check Exists Name
        public int ExistsName(string name, int id)
        {
            var checkresult = 0;
            Account account = db.Accounts.Where(p => p.ID != id && p.Name == name).FirstOrDefault();
            if (account == null)
            {
                checkresult = 0;
            }
            else
            {
                checkresult = 1;

            }
            return checkresult;
        }

        //Check Exists Code
        public int ExistsCode(string code, int id)
        {
            var checkresult = 0;
            Account account = db.Accounts.Where(p => p.ID != id && p.Code == code).FirstOrDefault();
            if (account == null)
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
        // GET: /Account/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "ID", "Name");
            ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");

            return View(db.AccountTypes.Where(a => a.ID != 1).ToList());
        }
        //
        // POST: /Account/Create

        [HttpPost]
        public string Create(string code, string name, int taxID, int status, int type, int dash, int claim, int payment)
        {
            Account account = new Account();
            account.Code = code;
            account.AccountTypeID = type;
            account.Name = name;
            account.TaxRateID = taxID;
            account.Status = status;
            account.ShowOnDashboard = Convert.ToBoolean(dash);
            account.ShowInExpenseClaims = Convert.ToBoolean(claim);
            account.EnablePayments = Convert.ToBoolean(payment);
            db.Accounts.Add(account);
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
        // GET: /Account/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }

            ViewBag.Code = account.Code;
            ViewBag.Name = account.Name;
            ViewBag.Status = account.Status;
            ViewBag.TypeID = account.AccountTypeID;
            ViewBag.ID = account.ID;
            if (account.ShowOnDashboard == true)
            {
                ViewBag.DashBroad = 1;
            }
            else
                ViewBag.DashBroad = 0;
            if (account.ShowInExpenseClaims == true)
            {
                ViewBag.ExpClaim = 1;
            }
            else
                ViewBag.ExpClaim = 0;
            if (account.EnablePayments == true)
            {
                ViewBag.Payment = 1;
            }
            else
                ViewBag.Payment = 0;

            ViewBag.AccountTypeID = new SelectList(db.AccountTypes, "ID", "Name", account.AccountTypeID);
            ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name", account.TaxRateID);

            return View(db.AccountTypes.Where(a => a.ID != 1).ToList());
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public string Edit(string code, string name, int taxID, int status, int type, int id, int dash, int claim, int payment)
        {
            Account account = db.Accounts.Find(id);

            account.Code = code;
            account.Name = name;
            account.TaxRateID = taxID;
            account.Status = status;
            account.AccountTypeID = type;
            account.ShowOnDashboard = Convert.ToBoolean(dash);
            account.ShowInExpenseClaims = Convert.ToBoolean(claim);
            account.EnablePayments = Convert.ToBoolean(payment);
            db.Entry(account).State = EntityState.Modified;
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
        // POST: /Account/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm, string[] bankDelConfirm)
        {
            if (delConfirm != null)
            {
                foreach (var item in delConfirm)
                {
                    int id = Int32.Parse(item);
                    Account account = db.Accounts.Find(id);
                    if (account != null)
                    {
                        try
                        {
                            db.Accounts.Remove(account);
                            db.SaveChanges();
                            result = "Success";
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                    }

                }
            }
            if (bankDelConfirm != null)
            {
                foreach (var item in bankDelConfirm)
                {
                    int id = Int32.Parse(item);
                    BankAccount bank = db.BankAccounts.Find(id);
                    if (bank != null)
                    {
                        try
                        {
                            db.BankAccounts.Remove(bank);
                            db.SaveChanges();
                            result = "Success";
                        }
                        catch (Exception e)
                        {
                            result = e.ToString();
                        }
                    }

                }
            }
            return result;
        }

        //Change Tax
        [HttpPost]
        public string ChangeTax(string[] accID, string[] bankID, int taxID)
        {
            if (accID != null)
            {
                foreach (var item in accID)
                {
                    int id = Int32.Parse(item);
                    Account acc = db.Accounts.Find(id);
                    if (acc != null)
                    {
                        acc.TaxRateID = taxID;
                        db.Entry(acc).State = EntityState.Modified;
                        try
                        {
                            db.SaveChanges();
                            if (bankID == null)
                            {
                                result = "Edit";
                            }
                            else
                                result = "Change";
                        }
                        catch (Exception e)
                        {
                            result = "Error";
                        }
                    }
                }
            }
            return result;
        }

        //
        // GET: /Accounts/BankAdd/
        public ActionResult BankAdd()
        {
            //For Dropdownlist
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName");
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name");

            return View();
        }

        //
        // POST: /BankAccounts/Create

        [HttpPost]
        public string BankAdd(int companyID, int currencyID, string bankName,
            string accountName, string accountNo, string description)
        {
            BankAccount bankaccount = new BankAccount();
            bankaccount.CompanyID = companyID;
            bankaccount.BankName = bankName;
            bankaccount.AccoutName = accountName;
            bankaccount.AccoutNum = accountNo;
            bankaccount.CurrencyID = currencyID;
            bankaccount.Description = description;
            db.BankAccounts.Add(bankaccount);
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
        // GET: /Accounts/BankEdit/

        public ActionResult BankEdit(int id = 0)
        {
            BankAccount bankaccount = db.BankAccounts.Find(id);
            if (bankaccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName", bankaccount.CompanyID);
            ViewBag.CurrencyID = new SelectList(db.Currencies, "ID", "Name", bankaccount.CurrencyID);
            return View(bankaccount);
        }

        //
        // POST: /Accounts/BankEdit/

        [HttpPost]
        public string BankEdit(int companyID, string bankName, string accountName, string accountNo, string description, int id)
        {
            BankAccount bankaccount = db.BankAccounts.Find(id);

            bankaccount.CompanyID = companyID;
            bankaccount.BankName = bankName;
            bankaccount.AccoutName = accountName;
            bankaccount.AccoutNum = accountNo;
            bankaccount.Description = description;
            db.Entry(bankaccount).State = EntityState.Modified;
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        //Account Transactions
        public ActionResult AccountDetails(int? page, int id, int type)
        {
            var formatString = "#,0.00";
            int year = DateTime.Now.Year;
            double? sum = 0;
            if (type == 0)
            {
                AccountInfo account = AccountManager.GetAllAccount().Where(a => a.AccountID == id && a.Type == "Bank").FirstOrDefault();
                var accountTrans = new TMT.ERP.BusinessLogic.Managers.GenericManager<AccountTran>().Get().Where(a => a.BankAccountID == id);
                foreach (var item in accountTrans) {
                    if (DateTime.Parse(item.Date.ToString()).Year == year) {
                        if (item.Received != null && item.Spent != null)
                        {
                            sum = sum + item.Received + item.Spent;
                        }
                        else if (item.Received != null) {
                            sum = sum + item.Received;
                        }
                        else if (item.Spent != null)
                        {
                            sum = sum - item.Spent;
                        }
                    }
                }
                var sumValue = sum.Value.ToString(formatString);
                string sumString = (sumValue.IndexOf("-") > -1) ? sumValue.Replace("-", "(") + ")" : sumValue;
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                if (account.AccountName.Length > 30)
                {
                    ViewBag.Name = account.AccountName.Substring(0, 10) + "...." + account.AccountName.Substring(account.AccountName.Length - 10);
                }
                else
                    ViewBag.Name = account.AccountName;
                ViewBag.ID = id;
                ViewBag.Tax = account.TaxRate;
                ViewBag.YTD = account.s_YTD;
                ViewBag.Type = type;
                ViewBag.Error = result;
                ViewBag.SumYear = sumString;
                result = "";
                return View(accountTrans.ToPagedList(pageNumber, pageSize));     
            }
            else if (type == 1)
            {
                AccountInfo accountInfo = AccountManager.GetAllAccount().Where(a => a.AccountID == id && a.Type != "Bank").FirstOrDefault();
                Account account = db.Accounts.Find(id);
                var accountTrans = new TMT.ERP.BusinessLogic.Managers.GenericManager<AccountTran>().Get().Where(a => a.AccountID == id);
                foreach (var item in accountTrans)
                {
                    if (DateTime.Parse(item.Date.ToString()).Year == year)
                    {
                        if (item.Received != null && item.Spent != null)
                        {
                            sum = sum + item.Received + item.Spent;
                        }
                        else if (item.Received != null)
                        {
                            sum = sum + item.Received;
                        }
                        else if (item.Spent != null)
                        {
                            sum = sum - item.Spent;
                        }
                    }
                }
                var sumValue = sum.Value.ToString(formatString);
                string sumString = (sumValue.IndexOf("-") > -1) ? sumValue.Replace("-", "(") + ")" : sumValue;
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                if (accountInfo.AccountName.Length > 30)
                {
                    ViewBag.Name = accountInfo.AccountName.Substring(0, 10) + "...." + accountInfo.AccountName.Substring(accountInfo.AccountName.Length - 10);
                }
                else
                    ViewBag.Name = accountInfo.AccountName;
                ViewBag.ID = id;
                ViewBag.Tax = accountInfo.TaxRate;
                ViewBag.YTD = accountInfo.s_YTD;
                ViewBag.Code = accountInfo.AccountCode;
                ViewBag.Type = type;
                if (account.ShowOnDashboard == true)
                {
                    ViewBag.Dashbroad = 1;
                }
                else
                    ViewBag.DashBbroad = 0;
                if (account.ShowInExpenseClaims == true)
                {
                    ViewBag.Expense = 1;
                }
                else
                    ViewBag.Expense = 0;
                if (account.EnablePayments == true)
                {
                    ViewBag.Payment = 1;
                }
                else
                    ViewBag.Payment = 0;
                ViewBag.SumYear = sumString;
                ViewBag.Error = result;
                result = "";
                return View(accountTrans.ToPagedList(pageNumber, pageSize));
            }
            else {
                return HttpNotFound();
            }
                  
        }
    }
}
