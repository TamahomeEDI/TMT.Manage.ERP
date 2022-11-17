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
using PagedList;
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

namespace TMT.ERP.Controllers
{
    public class AccountTransController : BaseController
    {
        private ErpEntities db = new ErpEntities();
        #region Minh

        public static int PageSize = Constant.DefaultPageSize;
        public static string tmtString = "";
        public static string balanceString = "";
        public static string result = "";
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        //
        // GET: /AccountTrans/

        public ActionResult Index(string currentFilter, int? page, string searchString, int id = 0)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var formatString = "#,0.00";
            List<AccountTran> bank = db.AccountTrans.Where(d => d.BankAccountID == id && d.Status == 2).ToList();
            double? balance = 0;
            foreach (var item in bank)
            {
                if (item.Received != null)
                {
                    balance += item.Received;
                }
                if (item.Spent != null)
                {
                    balance = balance - item.Spent;
                }
            }
            var balanceValue = balance.Value.ToString(formatString);
            balanceString = (balanceValue.IndexOf("-") > -1) ? balanceValue.Replace("-", "(") + ")" : balanceValue;
            ViewBag.Balance = balanceString;
            List<AccountTran> transaction = db.AccountTrans.Where(d => d.BankAccountID == id).ToList();
            double? tmtBalance = 0;
            foreach (var item in transaction)
            {
                if (item.Received != null)
                {
                    tmtBalance += item.Received;
                }
                if (item.Spent != null)
                {
                    tmtBalance = tmtBalance - item.Spent;
                }
            }
            var tmtValue = tmtBalance.Value.ToString(formatString);
            tmtString = (tmtValue.IndexOf("-") > -1) ? tmtValue.Replace("-", "(") + ")" : tmtValue;
            ViewBag.TMTBalance = tmtString;
            if (searchString == null)
            {
                List<AccountTran> accTrans = db.AccountTrans.Where(a => a.BankAccountID == id).OrderByDescending(b => b.ID).ToList();
                if (accTrans == null)
                {
                    return HttpNotFound();
                }
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Name = BankAccountName(id);
                ViewBag.Num = BankAccountNum(id);
                ViewBag.ID = id;
                return View(accTrans.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                List<AccountTran> accTrans = db.AccountTrans.Where(a => a.BankAccountID == id && a.Description == searchString).OrderByDescending(b => b.ID).ToList();

                if (accTrans == null)
                {
                    return HttpNotFound();
                }
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Name = BankAccountName(id);
                ViewBag.Num = BankAccountNum(id);
                ViewBag.ID = id;
                return View(accTrans.ToPagedList(pageNumber, pageSize));
            }
        }

        public string BankAccountName(int id)
        {
            BankAccount bankacc = db.BankAccounts.Find(id);
            return bankacc.AccoutName;
        }
        public string BankAccountNum(int id)
        {
            BankAccount bankacc = db.BankAccounts.Find(id);
            return bankacc.AccoutNum;
        }

        //Bank Statement
        public ActionResult BankStatement(string currentFilter, int? page, string searchString, int id = 0)
        {
            List<BankStatement> bankStt = db.BankStatements.Where(b => b.BankAccountID == id).ToList();

            if (bankStt == null)
            {
                return HttpNotFound();
            }
            var formatString = "#,0.00";
            List<AccountTran> bank = db.AccountTrans.Where(d => d.BankAccountID == id && d.Status == 2).ToList();
            double? balance = 0;
            foreach (var item in bank)
            {
                if (item.Received != null)
                {
                    balance += item.Received;
                }
                if (item.Spent != null)
                {
                    balance = balance - item.Spent;
                }
            }
            var balanceValue = balance.Value.ToString(formatString);
            balanceString = (balanceValue.IndexOf("-") > -1) ? balanceValue.Replace("-", "(") + ")" : balanceValue;
            ViewBag.Balance = balanceString;
            List<AccountTran> transaction = db.AccountTrans.Where(d => d.BankAccountID == id).ToList();
            double? tmtBalance = 0;
            foreach (var item in transaction)
            {
                if (item.Received != null)
                {
                    tmtBalance += item.Received;
                }
                if (item.Spent != null)
                {
                    tmtBalance = tmtBalance - item.Spent;
                }
            }
            var tmtValue = tmtBalance.Value.ToString(formatString);
            tmtString = (tmtValue.IndexOf("-") > -1) ? tmtValue.Replace("-", "(") + ")" : tmtValue;
            ViewBag.TMTBalance = tmtString;
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.Name = BankAccountName(id);
            ViewBag.Num = BankAccountNum(id);
            ViewBag.ID = id;
            return View(bankStt.ToPagedList(pageNumber, pageSize));
        }

        //
        // POST: /Account Transaction/Delete/

        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm, int? id)
        {
            foreach (var item in delConfirm)
            {
                int _id = Int32.Parse(item);
                AccountTran acc = db.AccountTrans.Find(id);
                if (acc != null)
                {
                    db.AccountTrans.Remove(acc);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("/Index/" + id);
        }

        //
        // POST: /Account Transaction/Reconcile/

        [HttpPost]
        public ActionResult MarkasReconcile(string[] accTransID, int id)
        {
            for (int i = 0; i < accTransID.Length; i++)
            {
                if (!String.IsNullOrEmpty(accTransID[i]))
                {
                    // Add to Bank Statement
                    AccountTran accTrans = db.AccountTrans.Find(Int64.Parse(accTransID[i]));
                    if (accTrans.Status == 0 || accTrans.Status == null)
                    {
                        //Update Status for Account Trans
                        accTrans.Status = 2;
                        db.Entry(accTrans).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
            }
            return RedirectToAction("/AccountTrans/Index?id=" + id);
        }

        public ActionResult BankTransDetail(int? id, int type)
        {
            BankAccountBill bill = db.BankAccountBills.Find(id);
            ViewBag.Type = bill.Type;
            ViewBag.VAT = bill.AmountAre;
            if (type == 0)
            {
                ViewBag.To = "To";
            }
            else
                ViewBag.To = "From";
            ViewBag.Contact = bill.ContactName;
            ViewBag.Date = bill.CreatedDate;
            ViewBag.Ref = bill.Reference;
            ViewBag.SubTotal = bill.Subtotal;
            ViewBag.Total = bill.Total;
            BankAccount bank = db.BankAccounts.Find(bill.BankAccountID);
            ViewBag.Name = bank.AccoutName;
            ViewBag.ID = bank.ID;
            var bankdetail = db.BankAccountDetails.Where(b => b.BankAccountBillID == id).
                Include(a => a.BankAccountBill).Include(t => t.TaxRate).Include(z => z.Account).ToList();
            return View("BankTransDetail", bankdetail);
        }

        #endregion
        #region HungVT-Reconcile
        public ActionResult Reconcile(int id = 0)
        {
            var formatString = "#,0.00";
            List<AccountTran> bank = db.AccountTrans.Where(d => d.BankAccountID == id && d.Status == 2).ToList();
            double? balance = 0;
            foreach (var item in bank)
            {
                if (item.Received != null)
                {
                    balance += item.Received;
                }
                if (item.Spent != null)
                {
                    balance = balance - item.Spent;
                }
            }
            var balanceValue = balance.Value.ToString(formatString);
            balanceString = (balanceValue.IndexOf("-") > -1) ? balanceValue.Replace("-", "(") + ")" : balanceValue;
            ViewBag.Balance = balanceString;
            List<AccountTran> transaction = db.AccountTrans.Where(d => d.BankAccountID == id).ToList();
            double? tmtBalance = 0;
            foreach (var item in transaction)
            {
                if (item.Received != null)
                {
                    tmtBalance += item.Received;
                }
                if (item.Spent != null)
                {
                    tmtBalance = tmtBalance - item.Spent;
                }
            }
            var tmtValue = tmtBalance.Value.ToString(formatString);
            tmtString = (tmtValue.IndexOf("-") > -1) ? tmtValue.Replace("-", "(") + ")" : tmtValue;
            ViewBag.TMTBalance = tmtString;
            var bankStatementDetails = db.BankStatementDetails.Where(x => x.Status == 0).ToList();
            ViewBag.Name = BankAccountName(id);
            ViewBag.Num = BankAccountNum(id);
            ViewBag.ID = id;
            ViewBag.AccountID = new SelectList(db.Accounts.ToList(), "ID",
                                             "Name");
            ViewBag.AccountBankID = new SelectList(db.BankAccounts.Where(x => x.ID != id).ToList(), "ID",
                                                 "AccoutName");
            ViewBag.TaxRateID = new SelectList(db.TaxRates, "ID", "Name");
            return View(bankStatementDetails);
        }

        public string Transfer(int accountBankId, string received, int bankStatementDetailsId)
        {
            //Account Type (1 - Bank; 0 - Account)
            //AmountAre (0 - Tax inclusive, 1- Tax exclusive, 2 - No Tax)
            //Status (0 - Transaction has not been reconciled 1 - Transaction has been marked as reconciled 2 - Transaction has been reconciled)
            //Type Received money:0 - Direct money 1 - Prepayment 2 - Overpaymentstring 
            var result = "success";
            try
            {
                var manager = new GenericManager<AccountTran>();

                var objBankStatementDetails = db.BankStatementDetails.Find(bankStatementDetailsId);
                var objBankStatement = objBankStatementDetails.BankStatement;
                var objAccountGeneral = db.AccountGenerals.FirstOrDefault(x => x.AccountID == objBankStatement.BankAccountID);

                //Add BankStatement into Transaction
                var objAccountTrans = new AccountTran();
                objAccountTrans.AccountID = objAccountGeneral.AccountID;
                objAccountTrans.AccountType = 1;
                objAccountTrans.Description = "Transfer";
                objAccountTrans.Date = DateTime.Now;
                objAccountTrans.Reference = objBankStatementDetails.Reference;
                objAccountTrans.Spent = 0;
                objAccountTrans.Received = float.Parse(received);
                objAccountTrans.AmountAre = 2;
                objAccountTrans.Status = 2;
                objAccountTrans.Type = 0;
                db.AccountTrans.Add(objAccountTrans);

                //Add BankAccount into Transaction
                var objAccountGeneralTemp = db.AccountGenerals.FirstOrDefault(x => x.AccountID == accountBankId);
                var objTemp = new AccountTran();
                objTemp.AccountID = objAccountGeneralTemp.AccountID;
                objTemp.AccountType = 1;
                objTemp.Description = "Transfer";
                objTemp.Date = DateTime.Now;
                objTemp.Reference = objBankStatementDetails.Reference;
                objTemp.Spent = 0;
                objTemp.Received = float.Parse(received);
                objTemp.AmountAre = 2;
                objTemp.Status = 2;
                objTemp.Type = 0;
                db.AccountTrans.Add(objTemp);
                db.SaveChanges();
                //Update BankStatementDetails
                objBankStatementDetails.Status = 2;
                db.Entry(objBankStatementDetails).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string Create(string name, string desc, int account, int taxrate, int id)
        {
            var result = "success";
            try
            {
                var objTax = db.TaxRates.Find(taxrate);
                if (objTax.Rate == 0)
                {
                    //Add Account Tran BankStatement
                    var objBankStatementDetail = db.BankStatementDetails.Find(id);
                    var objBankStatement = objBankStatementDetail.BankStatement;

                    var objaccountTrans = new AccountTran();
                    objaccountTrans.AccountID = objBankStatement.BankAccountID;
                    objaccountTrans.AccountType = 1;
                    objaccountTrans.Description = desc;
                    objaccountTrans.Date = DateTime.Now;
                    objaccountTrans.Reference = objBankStatementDetail.Reference;
                    objaccountTrans.Spent = 0;
                    objaccountTrans.Received = objBankStatementDetail.Received;
                    objaccountTrans.AmountAre = 2;
                    objaccountTrans.Status = 2;
                    objaccountTrans.Type = 0;
                    db.AccountTrans.Add(objaccountTrans);
                    //Add Account Tran by account
                    var objTemp = new AccountTran();
                    objTemp.AccountID = account;
                    objTemp.AccountType = 0;
                    objTemp.Description = desc;
                    objTemp.Date = DateTime.Now;
                    objTemp.Reference = objBankStatementDetail.Reference;
                    objTemp.Spent = 0;
                    objTemp.Received = objBankStatementDetail.Received;
                    objTemp.AmountAre = 2;
                    objTemp.Status = 2;
                    objTemp.Type = 0;
                    db.AccountTrans.Add(objTemp);

                    db.SaveChanges();
                    objBankStatementDetail.Status = 2;
                    db.Entry(objBankStatementDetail).State = EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    var objBankStatementDetail = db.BankStatementDetails.Find(id);
                    var objBankStatement = objBankStatementDetail.BankStatement;

                    var objaccountTrans = new AccountTran();
                    objaccountTrans.AccountID = objBankStatement.BankAccountID;
                    objaccountTrans.AccountType = 1;
                    objaccountTrans.Description = desc;
                    objaccountTrans.Date = DateTime.Now;
                    objaccountTrans.Reference = objBankStatementDetail.Reference;
                    objaccountTrans.Spent = 0;
                    objaccountTrans.Received = objBankStatementDetail.Received;
                    objaccountTrans.AmountAre = 2;
                    objaccountTrans.Status = 2;
                    objaccountTrans.Type = 0;
                    db.AccountTrans.Add(objaccountTrans);

                    //Add accountTrang by account
                    var x = 100 - objTax.Rate;
                    var received = x * objBankStatementDetail.Received / 100;

                    var objTemp = new AccountTran();
                    objTemp.AccountID = account;
                    objTemp.AccountType = 0;
                    objTemp.Description = desc;
                    objTemp.Date = DateTime.Now;
                    objTemp.Reference = objBankStatementDetail.Reference;
                    objTemp.Spent = 0;
                    objTemp.Received = received;
                    objTemp.AmountAre = 2;
                    objTemp.Status = 2;
                    objTemp.Type = 0;
                    db.AccountTrans.Add(objTemp);

                    //Add accountTran by account tax

                    db.SaveChanges();
                    objBankStatementDetail.Status = 2;
                    db.Entry(objBankStatementDetail).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
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
