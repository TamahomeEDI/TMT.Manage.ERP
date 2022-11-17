using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;
using System.Web.Script.Services;
using System.Web.Services;
using CommonLib;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;
using System.Globalization;
using System.IO;
using PagedList;
using System.Text.RegularExpressions;
using TMT.ERP.BusinessLogic.Utils;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class BankAccountsController : BaseController  //Controller
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /BankAccounts/
        public static string result = "";
        private const string formatNumber = "#,0.00";
        public ActionResult Index()
        {
            ViewBag.Error = result;
            result = "";
            var bc = new BankResultCollection();
            BankResult obj = null;
            var Banks = new GenericManager<BankAccount>().Get()
                .Select(x => new { x.ID, x.BankStatements, x.AccountTrans, x.AccoutName, x.AccoutNum, x.ShowOnDashboard }).ToList();
            if (Banks != null)
            {
                bc.BankResults = new List<BankResult>();
                foreach (var item in Banks)
                {
                    var bankStatementDetails = item.BankStatements.SelectMany(x => x.BankStatementDetails);
                    obj = new BankResult();
                    obj.BankID = item.ID;
                    obj.AccoutName = item.AccoutName;
                    obj.AccoutNum = item.AccoutNum;
                    obj.Total = (item.AccountTrans.Where(a => a.Status == 2).Sum(x => x.Received) - item.AccountTrans.Sum(x => x.Spent)).Value.ToString(formatNumber);
                    obj.ReconcileCount = item.BankStatements.SelectMany(x => x.BankStatementDetails).Where(w => w.Status == 0).Count();
                    obj.TotalBalance = (item.AccountTrans.Sum(x => x.Received) - item.AccountTrans.Sum(x => x.Spent)).Value.ToString(formatNumber);
                    obj.MaxDate = (bankStatementDetails.Count() > 0) ? item.BankStatements.SelectMany(x => x.BankStatementDetails).Max(z => z.Date).Value.ToString("d MMM yyyy") : string.Empty;
                    obj.ShowDashboard = item.ShowOnDashboard ?? 0;
                    bc.BankResults.Add(obj);
                    obj = null;
                }
            }
            return View(bc);
        }
        //
        // GET: /BankAccounts/Create

        public ActionResult Create()
        {
            //For Dropdownlist
            ViewBag.CompanyID = new SelectList(db.Companies, "ID", "DisplayName");
            ViewBag.CurrencyID = CreateCurrencyList();

            return View();
        }

        private object CreateCurrencyList()
        {
            var manager = new GenericManager<Currency>();
            var currencyItems = manager.Get().Select(x => new { Text = x.Code + "-" + x.Name, Value = x.ID }).Distinct().ToList();
            return currencyItems;
        }

        //
        // POST: /BankAccounts/Create

        [HttpPost]
        public string Create(int companyID, int currencyID, string bankName,
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
        // GET: /BankAccounts/Edit/5

        public ActionResult Edit(int id = 0)
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
        // POST: /BankAccounts/Edit/5

        [HttpPost]
        public string Edit(int companyID, int currencyID, string bankName,
            string accountName, string accountNo, string description, int id)
        {
            BankAccount bankaccount = db.BankAccounts.Find(id);
            bankaccount.CompanyID = companyID;
            bankaccount.BankName = bankName;
            bankaccount.AccoutName = accountName;
            bankaccount.AccoutNum = accountNo;
            bankaccount.CurrencyID = currencyID;
            bankaccount.Description = description;
            db.Entry(bankaccount).State = EntityState.Modified;
            try
            {
                result = "Edit";
                db.SaveChanges();
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;
        }

        //
        // GET: /BankAccounts/TransferMoney/5

        public ActionResult TransferMoney(int id = 0)
        {
            ViewBag.ID = id;
            if (id != 0)
            {
                ViewBag.BankAccountID2 = new SelectList(db.BankAccounts.Where(b => b.ID != id), "ID", "AccoutName");
                BankAccount bank = db.BankAccounts.Find(id);
                if (bank == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    if (bank.AccoutName.Length > 30)
                    {
                        ViewBag.Name = bank.AccoutName.Substring(0, 10) + "..." + bank.AccoutName.Substring(bank.AccoutName.Length - 10);
                    }
                    else
                        ViewBag.Name = bank.AccoutName;
                }
            }
            else {
                ViewBag.BankAccountID1 = new SelectList(db.BankAccounts, "ID", "AccoutName");
                ViewBag.BankAccountID2 = new SelectList(db.BankAccounts, "ID", "AccoutName");
            }
            return View();
        }

        [HttpPost]
        public string TransferMoneyConfirmed(int? _idsp, int? idre, string date, string amount, string transactionName)
        {
            var managerAT = new GenericManager<AccountTran>();
            AccountTran accSpent = new AccountTran();
            accSpent.AccountType = 0;
            accSpent.BankAccountID = _idsp;
            accSpent.Date = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            accSpent.Spent = Convert.ToDouble(amount);
            accSpent.TransactionName = transactionName;
            managerAT.Add(accSpent);

            AccountTran accReceive = new AccountTran();
            accReceive.BankAccountID = idre;
            accReceive.AccountType = 0;
            accReceive.Date = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            accReceive.Received = Convert.ToDouble(amount);
            accReceive.TransactionName = transactionName;
            managerAT.Add(accReceive);

            try
            {
                result = "Trans";
                managerAT.Save();
            }
            catch (Exception e)
            {
                result = "Error";
            }
            return result;
        }


        // Add New Line
        public PartialViewResult AddNewLine()
        {
            ViewBag.BankBillID = InitBill();
            ViewBag.AccountID = InitAccount();
            ViewBag.TaxRateID = InitTaxRates();
            return PartialView("_AddNewLine");
        }

        private List<SelectListItem> InitBill()
        {
            var billList = new SelectList(db.BankAccountDetails, "ID", "BankAccountBillID");
            List<SelectListItem> billItem = billList.ToList();
            billItem.Insert(0, (new SelectListItem { Text = "+ Add new bill...", Value = "0" }));
            return billItem;
        }

        private List<SelectListItem> InitAccount()
        {
            var accountList = new SelectList(db.Accounts, "ID", "Name");
            List<SelectListItem> accountItems = accountList.ToList();
            accountItems.Insert(0, (new SelectListItem { Text = "+ Add new account...", Value = "0" }));
            return accountItems;
        }

        private List<SelectListItem> InitTaxRates()
        {
            var taxRates = new SelectList(db.TaxRates, "ID", "Name");
            List<SelectListItem> taxRateItems = taxRates.ToList();
            return taxRateItems;
        }// END

        //
        // GET: /BankAccounts/SpendMoney/5
        public ActionResult SpendMoney(int? id)
        {
            BankAccount ba = db.BankAccounts.Find(id);
            if (ba == null)
            {
                return HttpNotFound();
            }

            //Get Sale Invoice Code
            ViewBag.BillCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.SaleOrder));

            //For Dropdownlist
            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Name");
            ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
            ViewBag.Currencies = new SelectList(db.Currencies, "ID", "Name");
            ViewBag.Currency = CurrencyName(ba.CurrencyID);
            ViewBag.Name = ba.AccoutName;
            ViewBag.ID = id;
            ViewBag.Error = result;
            result = "";
            return View();
        }

        public string CurrencyName(int id)
        {
            Currency currency = db.Currencies.Find(id);
            return currency.Name;
        }

        // Spend Money Bill

        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        [HttpPost]
        public string SpendBill(int id, string contactName, string date, string reference, int method, int userID,
            string subtotal, string tax, string total, int amountare, string SpendMoneyDetail, string invoice, string transactionName)
        {
            try
            {
                //Update Sale Tax Account
                AccountFeature account = db.AccountFeatures.Find(3);
                Account saletax = db.Accounts.Find(account.AccountID);
                if (saletax.TotalDebit != null)
                {
                    saletax.TotalDebit = saletax.TotalDebit + Double.Parse(tax);
                }
                else
                    saletax.TotalDebit = Double.Parse(tax);
                db.Entry(saletax).State = EntityState.Modified;
                db.SaveChanges();

                //Save to BankAccountBills
                var managerAT = new GenericManager<BankAccountBill>();
                BankAccountBill bankspendbill = new BankAccountBill();
                bankspendbill.Type = method;
                bankspendbill.Status = 0;
                bankspendbill.BankAccountID = id;
                bankspendbill.ContactName = contactName;
                bankspendbill.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                bankspendbill.Reference = reference;
                bankspendbill.Subtotal = Convert.ToDouble(subtotal);
                bankspendbill.Tax = Convert.ToDouble(tax);
                bankspendbill.Total = Convert.ToDouble(total);
                managerAT.Add(bankspendbill);
                managerAT.Save();

                //Save to Purchase
                //Int32 purchaseID = 0;
                //if (method == 1)
                //{
                //    var managerSale = new GenericManager<Purchase>();
                //    Purchase purchase = new Purchase();
                //    purchase.CreatedUserID = userID;
                //    purchase.ContactName = contactName;
                //    purchase.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                //    purchase.Reference = reference;
                //    purchase.SubTotal = Double.Parse(subtotal);
                //    purchase.Tax = Double.Parse(tax);
                //    purchase.TotalMoney = Double.Parse(total);
                //    purchase.StockID = 1;
                //    managerSale.Add(purchase);
                //    managerSale.Save();
                //    purchaseID = purchase.ID;
                //    Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.SaleOrder), Constant.CODE_LENGTH, "");
                //}

                dynamic array = JsonConvert.DeserializeObject(SpendMoneyDetail);
                foreach (var item in array)
                {
                    //Save to BankAccountDetails
                    Int32 accountID = Int32.Parse(item.accID.ToString());
                    var managerBADetail = new GenericManager<BankAccountDetail>();
                    BankAccountDetail spendetail = new BankAccountDetail();
                    spendetail.BankAccountBillID = bankspendbill.ID;
                    spendetail.Description = item.description;
                    spendetail.RelatedAccountID = item.accID;
                    spendetail.Quantity = item.quantity;
                    spendetail.UnitPrice = item.unitprice;
                    spendetail.TaxRateID = item.taxrateID;
                    if (amountare == 0)
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        spendetail.Amount = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    else
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        spendetail.Amount = Double.Parse(item.amount.ToString()) - (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    spendetail.Status = 1;
                    managerBADetail.Add(spendetail);
                    managerBADetail.Save();

                    //Save to AccountTrans
                    var managerAccTrans = new GenericManager<AccountTran>();
                    AccountTran accTrans = new AccountTran();
                    accTrans.BankAccountID = id;
                    accTrans.AccountType = method;
                    accTrans.AmountAre = amountare;
                    accTrans.Date = bankspendbill.CreatedDate;
                    accTrans.Description = item.description;
                    accTrans.Reference = bankspendbill.Reference;
                    if (amountare == 0)
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        accTrans.Spent = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    else
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        accTrans.Spent = Double.Parse(item.amount.ToString()) - (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    accTrans.Type = method;
                    accTrans.Status = amountare;
                    accTrans.SourceID = bankspendbill.ID;
                    accTrans.SourceType = 3;
                    accTrans.TransactionName = transactionName;
                    managerAccTrans.Add(accTrans);
                    managerAccTrans.Save();

                    //Save to AccountTransDetails
                    var managerAccTranDetails = new GenericManager<AccountTranDetail>();
                    AccountTranDetail accTranDetails = new AccountTranDetail();
                    accTranDetails.AccountID = item.accID;
                    accTranDetails.Description = item.description;
                    accTranDetails.Quantity = item.quantity;
                    accTranDetails.Price = item.unitprice;
                    accTranDetails.TaxRateID = item.taxrateID;
                    accTranDetails.Total = accTrans.Spent;
                    managerAccTranDetails.Add(accTranDetails);
                    managerAccTranDetails.Save();

                    // Update to Account
                    if (amountare == 0)
                    {
                        Account acc = db.Accounts.Find(accountID);
                        if (acc != null)
                        {
                            if (acc.TotalDebit != null)
                            {
                                acc.TotalDebit = acc.TotalDebit + accTrans.Spent;
                            }
                            else
                                acc.TotalDebit = Double.Parse(item.amount.ToString());
                            db.Entry(acc).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Account acc = db.Accounts.Find(accountID);
                        if (acc != null)
                        {
                            TaxRate taxrate = db.TaxRates.Find(acc.TaxRateID);
                            double rate = Double.Parse(taxrate.Rate.ToString());
                            if (acc.TotalDebit != null)
                            {
                                acc.TotalDebit = acc.TotalDebit + Double.Parse(item.amount.ToString());
                            }
                            else
                                acc.TotalDebit = Double.Parse(item.amount.ToString());
                            db.Entry(acc).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //Save to Purchsase Invoice Details
                    //if (method == 1)
                    //{
                    //    var managerDetailInvoice = new GenericManager<PurchaseDetail>();
                    //    PurchaseDetail detail = new PurchaseDetail();
                    //    detail.PurchaseID = purchaseID;
                    //    //*****update later*****
                    //    detail.GoodID = 1;
                    //    //*****end******
                    //    detail.Quantity = item.quantity;
                    //    detail.Price = item.unitprice;
                    //    detail.AccountID = item.accID;
                    //    detail.TaxRateID = item.taxrateID;
                    //    if (amountare == 0)
                    //    {
                    //        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                    //        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                    //        Double rate = Double.Parse(taxRate.Rate.ToString());
                    //        detail.TotalMoney = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    //    }
                    //    else {
                    //        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                    //        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                    //        Double rate = Double.Parse(taxRate.Rate.ToString());
                    //        detail.TotalMoney = Double.Parse(item.amount.ToString());
                    //    }
                    //    detail.Description = item.description;
                    //    managerDetailInvoice.Add(detail);
                    //    managerDetailInvoice.Save();
                    //}
                }
                result = "Spend";
            }
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", error.Message);
                    result = "Error";

                }
            }
            return result;
        }

        [HttpPost]
        public string SpendBillOver(int id, string contactName, string date, string reference, string invoice, int currency,
            string subtotal, string total, double amountover, int accID, string description, int method, int userID, string transactionName)
        {
            try
            {
                // Save to BankAccountBills
                var managerAT = new GenericManager<BankAccountBill>();
                BankAccountBill bankspendbill = new BankAccountBill();
                bankspendbill.Type = method;
                bankspendbill.Status = 0;
                bankspendbill.AmountAre = 3;
                bankspendbill.BankAccountID = id;
                bankspendbill.ContactName = contactName;
                bankspendbill.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                bankspendbill.Reference = reference;
                bankspendbill.Subtotal = Convert.ToDouble(subtotal);
                bankspendbill.Tax = 0;
                bankspendbill.Total = Convert.ToDouble(total);
                managerAT.Add(bankspendbill);
                managerAT.Save(); ;

                //Save to BankAccountDetail
                var managerOver = new GenericManager<BankAccountDetail>();
                BankAccountDetail bankOver = new BankAccountDetail();
                bankOver.BankAccountBillID = bankspendbill.ID;
                bankOver.Description = description;
                bankOver.RelatedAccountID = accID;
                bankOver.Quantity = 0;
                bankOver.UnitPrice = 0;
                bankOver.TaxRateID = 1;
                bankOver.Amount = amountover;
                bankOver.Status = 1;
                managerOver.Add(bankOver);
                managerOver.Save();


                //Save to AccountTrans
                var managerAccTrans = new GenericManager<AccountTran>();
                AccountTran accTrans = new AccountTran();
                accTrans.BankAccountID = id;
                accTrans.AccountType = 1;
                accTrans.Date = bankspendbill.CreatedDate;
                accTrans.Description = bankOver.Description;
                accTrans.Reference = bankspendbill.Reference;
                accTrans.Spent = bankOver.Amount;
                accTrans.Type = method;
                accTrans.Status = 0;
                accTrans.SourceID = bankspendbill.ID;
                accTrans.SourceType = 3;
                accTrans.TransactionName = transactionName;
                managerAccTrans.Add(accTrans);
                managerAccTrans.Save();

                //Save to AccountTransDetails
                var managerAccTranDetails = new GenericManager<AccountTranDetail>();
                AccountTranDetail accTranDetails = new AccountTranDetail();
                accTranDetails.AccountID = accID;
                accTranDetails.Description = description;
                accTranDetails.Quantity = 0;
                accTranDetails.Price = 0;
                accTranDetails.TaxRateID = 1;
                accTranDetails.Total = amountover;
                managerAccTranDetails.Add(accTranDetails);
                managerAccTranDetails.Save();

                // Update to Account
                Account acc = db.Accounts.Where(a => a.ID == accID).FirstOrDefault();
                if (acc != null)
                {
                    if (acc.TotalDebit != null)
                    {
                        acc.TotalDebit = acc.TotalDebit + accTrans.Spent;
                    }
                    else
                        acc.TotalDebit = accTrans.Spent;
                    db.Entry(acc).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //Save to Purchase
                //var managerSale = new GenericManager<Purchase>();
                //Purchase purchase = new Purchase();
                //purchase.CreatedUserID = userID;
                //purchase.ContactName = contactName;
                //purchase.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                //purchase.Reference = reference;
                //purchase.SubTotal = Double.Parse(subtotal);
                //purchase.TotalMoney = Double.Parse(total);
                //purchase.StockID = 2;
                //purchase.CurrencyID = currency;
                //managerSale.Add(purchase);
                //managerSale.Save();

                //Save to Purchase Details
                //var managerDetailInvoice = new GenericManager<PurchaseDetail>();
                //PurchaseDetail detail = new PurchaseDetail();
                //detail.PurchaseID = purchase.ID;
                ////*****update later*****
                //detail.GoodID = 1;
                //detail.UOMID = 1;
                ////*****end******
                //detail.Quantity = 1;
                //detail.Price = Double.Parse(total);
                //detail.AccountID = accID;
                //detail.TotalMoney = Double.Parse(total);
                //detail.Description = description;
                //managerDetailInvoice.Add(detail);
                //managerDetailInvoice.Save();
                result = "Spend";
            }
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", error.Message);
                    result = "Error";
                }
            }
            return result;
        }

        //
        // GET: /BankAccounts/ReceiveMoney/
        public ActionResult ReceiveMoney(int? id)
        {
            BankAccount bank = db.BankAccounts.Find(id);
            if (bank == null)
            {
                return HttpNotFound();
            }

            //Get Sale Invoice Code
            ViewBag.BillCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.SaleOrder));

            //For Dropdownlist
            ViewBag.AccountID = new SelectList(db.Accounts, "ID", "Name");
            ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
            ViewBag.Currencies = new SelectList(db.Currencies, "ID", "Name");
            ViewBag.Currency = CurrencyName(bank.CurrencyID);
            ViewBag.Name = bank.AccoutName;
            ViewBag.ID = id;
            ViewBag.Error = result;
            result = "";
            return PartialView();
        }

        // Receive Money Bill
        [HttpPost]
        public string ReceiveBill(int id, string contactName, string date, string reference, int method, int userID,
            string subtotal, string tax, string total, int amountare, string invoice, string SpendMoneyDetail, string transactionName)
        {
            try
            {
                //Update Sale Tax Account
                AccountFeature account = db.AccountFeatures.Find(3);
                Account saletax = db.Accounts.Find(account.AccountID);
                if (saletax.TotalCredit != null)
                {
                    saletax.TotalCredit = saletax.TotalCredit + Double.Parse(tax);
                }
                else
                    saletax.TotalCredit = Double.Parse(tax);
                db.Entry(saletax).State = EntityState.Modified;
                db.SaveChanges();

                //Save to BankAccountBills
                var managerAT = new GenericManager<BankAccountBill>();
                BankAccountBill bankspendbill = new BankAccountBill();
                bankspendbill.Type = method;
                bankspendbill.Status = 1;
                bankspendbill.AmountAre = amountare;
                bankspendbill.BankAccountID = id;
                bankspendbill.ContactName = contactName;
                bankspendbill.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                bankspendbill.Reference = reference;
                bankspendbill.Subtotal = Convert.ToDouble(subtotal);
                bankspendbill.Tax = Convert.ToDouble(tax);
                bankspendbill.Total = Convert.ToDouble(total);
                managerAT.Add(bankspendbill);
                managerAT.Save(); ;

                //Save to Sale Invoice
                //Int32 saleID = 0;
                //if (method == 1) {
                //    var managerSale = new GenericManager<SaleInvoice>();
                //    SaleInvoice sale = new SaleInvoice();
                //    sale.Code = invoice;
                //    sale.CreatedUserID = userID;
                //    sale.ContactName = contactName;
                //    sale.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                //    sale.Reference = reference;
                //    sale.TaxAmountType = amountare;
                //    sale.SubTotal = Double.Parse(subtotal);
                //    sale.TotalTax = Double.Parse(tax);
                //    sale.TotalMoney = Double.Parse(total);
                //    sale.StockID = 1;
                //    sale.Status = 2;
                //    managerSale.Add(sale);
                //    managerSale.Save();
                //    saleID = sale.ID;
                //    Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.SaleOrder), Constant.CODE_LENGTH, "");
                //}

                dynamic array = JsonConvert.DeserializeObject(SpendMoneyDetail);
                foreach (var item in array)
                {
                    //Save to BankAccountDetails
                    Int32 accountID = Int32.Parse(item.accID.ToString());
                    var managerBADetail = new GenericManager<BankAccountDetail>();
                    BankAccountDetail spendetail = new BankAccountDetail();
                    spendetail.BankAccountBillID = bankspendbill.ID;
                    spendetail.Description = item.description;
                    spendetail.RelatedAccountID = item.accID;
                    spendetail.Quantity = item.quantity;
                    spendetail.UnitPrice = item.unitprice;
                    spendetail.TaxRateID = item.taxrateID;
                    if (amountare == 0)
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        spendetail.Amount = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    else
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        spendetail.Amount = Double.Parse(item.amount.ToString()) - (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    spendetail.Status = 1;
                    managerBADetail.Add(spendetail);
                    managerBADetail.Save();

                    //Save to AccountTrans
                    var managerAccTrans = new GenericManager<AccountTran>();
                    AccountTran accTrans = new AccountTran();
                    accTrans.BankAccountID = id;
                    accTrans.AccountType = method;
                    accTrans.AmountAre = amountare;
                    accTrans.Date = bankspendbill.CreatedDate;
                    accTrans.Description = item.description;
                    accTrans.Reference = bankspendbill.Reference;
                    if (amountare == 0)
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        accTrans.Spent = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    else
                    {
                        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                        Double rate = Double.Parse(taxRate.Rate.ToString());
                        accTrans.Spent = Double.Parse(item.amount.ToString()) - (Double.Parse(item.amount.ToString()) * (rate / 100));
                    }
                    accTrans.Type = method;
                    accTrans.Status = 0;
                    accTrans.SourceID = bankspendbill.ID;
                    accTrans.SourceType = 3;
                    accTrans.TransactionName = transactionName;
                    managerAccTrans.Add(accTrans);
                    managerAccTrans.Save();

                    //Save to AccountTransDetails
                    var managerAccTranDetails = new GenericManager<AccountTranDetail>();
                    AccountTranDetail accTranDetails = new AccountTranDetail();
                    accTranDetails.Description = item.description;
                    accTranDetails.Quantity = item.quantity;
                    accTranDetails.Price = item.unitprice;
                    accTranDetails.TaxRateID = item.taxrateID;
                    accTranDetails.AccountID = item.accID;
                    accTranDetails.Total = item.amount;
                    managerAccTranDetails.Add(accTranDetails);
                    managerAccTranDetails.Save();

                    // Update to Account
                    if (amountare == 0)
                    {
                        Account acc = db.Accounts.Find(accountID);
                        if (acc != null)
                        {
                            if (acc.TotalCredit != null)
                            {
                                acc.TotalCredit = acc.TotalCredit + accTrans.Spent;
                            }
                            else
                                acc.TotalCredit = Double.Parse(item.amount.ToString());
                            db.Entry(acc).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Account acc = db.Accounts.Find(accountID);
                        if (acc != null)
                        {
                            TaxRate taxrate = db.TaxRates.Find(acc.TaxRateID);
                            double rate = Double.Parse(taxrate.Rate.ToString());
                            if (acc.TotalCredit != null)
                            {
                                acc.TotalCredit = acc.TotalCredit + Double.Parse(item.amount.ToString());
                            }
                            else
                                acc.TotalCredit = Double.Parse(item.amount.ToString());
                            db.Entry(acc).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    //Save to Sale Invoice Details
                    //if (method == 1) {
                    //    var managerDetailInvoice = new GenericManager<SaleInvoiceDetail>();
                    //    SaleInvoiceDetail detail = new SaleInvoiceDetail();
                    //    detail.SaleInvoiceID = saleID;
                    //    //*****update later*****
                    //    detail.GoodID = 1;
                    //    //*****end******
                    //    detail.Quantity = item.quantity;
                    //    detail.Price = item.unitprice;
                    //    detail.AccountID = item.accID;
                    //    detail.TaxRateID = item.taxrateID;
                    //    if (amountare == 0)
                    //    {
                    //        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                    //        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                    //        Double rate = Double.Parse(taxRate.Rate.ToString());
                    //        detail.Subtotal = Double.Parse(item.amount.ToString());
                    //        detail.TotalMoney = Double.Parse(item.amount.ToString()) + (Double.Parse(item.amount.ToString()) * (rate / 100));
                    //    }
                    //    else
                    //    {
                    //        Int32 taxRateID = Int32.Parse(item.taxrateID.ToString());
                    //        TaxRate taxRate = db.TaxRates.Find(taxRateID);
                    //        Double rate = Double.Parse(taxRate.Rate.ToString());
                    //        detail.Subtotal = Double.Parse(item.amount.ToString()) - (Double.Parse(item.amount.ToString()) * (rate / 100));
                    //        detail.TotalMoney = Double.Parse(item.amount.ToString());
                    //    }
                    //    detail.Description = item.description;
                    //    managerDetailInvoice.Add(detail);
                    //    managerDetailInvoice.Save();
                    //}
                }
                result = "Receive";

            }
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", error.Message);
                    result = "Error";
                }
            }
            return result;
        }


        [HttpPost]
        public string ReceiveBillOver(int id, string contactName, string date, string reference, string invoice, int currency,
            string subtotal, string total, double amountover, int accID, string description, int method, int userID, string transactionName)
        {
            try
            {
                // Save to BankAccountBills
                var managerAT = new GenericManager<BankAccountBill>();
                BankAccountBill bankspendbill = new BankAccountBill();
                bankspendbill.Type = method;
                bankspendbill.Status = 1;
                bankspendbill.AmountAre = 3;
                bankspendbill.BankAccountID = id;
                bankspendbill.ContactName = contactName;
                bankspendbill.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                bankspendbill.Reference = reference;
                bankspendbill.Subtotal = Convert.ToDouble(subtotal);
                bankspendbill.Tax = 0;
                bankspendbill.Total = Convert.ToDouble(total);
                managerAT.Add(bankspendbill);
                managerAT.Save();

                //Save to BankAccountDetail
                var managerOver = new GenericManager<BankAccountDetail>();
                BankAccountDetail bankOver = new BankAccountDetail();
                bankOver.BankAccountBillID = bankspendbill.ID;
                bankOver.Description = description;
                bankOver.RelatedAccountID = accID;
                bankOver.Quantity = 0;
                bankOver.UnitPrice = 0;
                bankOver.TaxRateID = 1;
                bankOver.Amount = amountover;
                bankOver.Status = 1;
                managerOver.Add(bankOver);
                managerOver.Save();

                //Save to AccountTrans
                var managerAccTrans = new GenericManager<AccountTran>();
                AccountTran accTrans = new AccountTran();
                accTrans.BankAccountID = id;
                accTrans.AccountType = 1;
                accTrans.Date = bankspendbill.CreatedDate;
                accTrans.Description = bankOver.Description;
                accTrans.Reference = bankspendbill.Reference;
                accTrans.Spent = bankOver.Amount;
                accTrans.Type = method;
                accTrans.Status = 0;
                accTrans.SourceID = bankspendbill.ID;
                accTrans.SourceType = 3;
                accTrans.TransactionName = transactionName;
                managerAccTrans.Add(accTrans);
                managerAccTrans.Save();


                //Save to AccountTransDetails
                var managerAccTranDetails = new GenericManager<AccountTranDetail>();
                AccountTranDetail accTranDetails = new AccountTranDetail();
                accTranDetails.AccountID = accID;
                accTranDetails.Description = description;
                accTranDetails.Quantity = 0;
                accTranDetails.Price = 0;
                accTranDetails.TaxRateID = 1;
                accTranDetails.Total = amountover;
                managerAccTranDetails.Add(accTranDetails);
                managerAccTranDetails.Save();

                // Update to Account
                Account acc = db.Accounts.Where(a => a.ID == accID).FirstOrDefault();
                if (acc != null)
                {
                    if (acc.TotalDebit != null)
                    {
                        acc.TotalDebit = acc.TotalDebit + accTrans.Spent;
                    }
                    else
                        acc.TotalDebit = accTrans.Spent;
                    db.Entry(acc).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //Save to Sale Invoice
                //var managerSale = new GenericManager<SaleInvoice>();
                //SaleInvoice sale = new SaleInvoice();
                //sale.CreatedUserID = userID;
                //sale.ContactName = contactName;
                //sale.CreatedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                //sale.Reference = reference;
                //sale.SubTotal = Double.Parse(subtotal);
                //sale.TotalMoney = Double.Parse(total);
                //sale.StockID = 2;
                //sale.CurrencyID = currency;
                //managerSale.Add(sale);
                //managerSale.Save();

                //Save to Sale Invoice Details
                //var managerDetailInvoice = new GenericManager<SaleInvoiceDetail>();
                //SaleInvoiceDetail detail = new SaleInvoiceDetail();
                //detail.SaleInvoiceID = sale.ID;
                ////*****update later*****
                //detail.GoodID = 1;
                //detail.Quantity = 0;
                //detail.Price = 0;
                //detail.AccountID = accID;
                //detail.TaxRateID = 1;
                ////*****end******
                //detail.Subtotal = Double.Parse(subtotal);
                //detail.TotalMoney = Double.Parse(total);
                //detail.Description = description;
                //managerDetailInvoice.Add(detail);
                //managerDetailInvoice.Save();
                result = "Receive";
            }
            catch (DbUpdateException ex)
            {
                UpdateException updateException = (UpdateException)ex.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;

                foreach (SqlError error in sqlException.Errors)
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", error.Message);
                    result = "Error";
                }
            }
            return result;
        }

        public ActionResult ImportStatement(int id = 0)
        {
            BankAccount bank = db.BankAccounts.Find(id);
            if (bank == null)
            {
                HttpNotFound();
            }
            ViewBag.ID = id;
            ViewBag.Error = error;
            error = "";
            return View(bank);
        }

        public string ShowOnDashBoard(int id = 0) {
            result = "Success";
            BankAccount bank = db.BankAccounts.Find(id);
            if (bank.ShowOnDashboard == 0)
            {
                bank.ShowOnDashboard = 1;
            }
            else
                bank.ShowOnDashboard = 0;
            db.Entry(bank).State = EntityState.Modified;
            db.SaveChanges();
            return result;
        }


        private static string error = "";
        //Read Csv File & import to database
        [HttpPost]
        public ActionResult ImportStatement(HttpPostedFileBase file)
        {
            Int32 id = Int32.Parse(Request.Form["hidID"]);
            if (file != null && file.ContentLength > 0)
            {
                var fileName = file.FileName;
                var path = Path.Combine(Server.MapPath("~/Uploads/files/"), fileName);
                file.SaveAs(path);
                var logPath = Path.Combine(Server.MapPath("~/Uploads/CsvTemplate/Logs/"));
                ImportData.ImportDataFromCSV(path, ",", logPath, id);
                return RedirectToAction("Reconcile", "AccountTrans", new { id = id });
            }
            else
            {
                error = "Empty";
                ViewBag.Error = error;
                return RedirectToAction("ImportStatement", "BankAccounts", new { id = id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}