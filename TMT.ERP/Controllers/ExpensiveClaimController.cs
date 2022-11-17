using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using PagedList;
using TMT.ERP.Commons;
using System.Data;
using TMT.ERP.Models.Lookups;
namespace TMT.ERP.Controllers
{
    public class ExpensiveClaimController : BaseController
    {
        //
        // GET: /ExpensiveClaim/
        private ErpEntities db = new ErpEntities();
        User user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
        public static int PageSize = 2;
        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;

        }

        public ActionResult Create()
        {
            initDropDowlist();
                      
            //For Note Function
            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.Note);
            ViewBag.NoteFromID = 0;

            if(user != null)
            ViewBag.CurrentUserID = user.ID;
            //End
            return View();
        }

        public ActionResult ExpenseClaimSummary(int id = 0)
        {
           
            ExpenseClaim claim = db.ExpenseClaims.Where(ex => ex.ID == id).SingleOrDefault();
            var expenseClaim = db.ExpenseClaims.Where(ex => ex.ID == id).ToList();
            ViewBag.Amount = claim.TotalMoney;
            ViewBag.expenseClaimID = id;
            ViewBag.To = user.UserName;
            ViewBag.ApproveAmount = claim.TotalMoney;
            DateTime due = claim.DueDate != null ? Convert.ToDateTime(claim.DueDate) : DateTime.Now;
            DateTime submitDate = claim.CreatedDate != null ? Convert.ToDateTime(claim.CreatedDate) : DateTime.Now;
            ViewBag.DueDate = due.ToString("d");
            ViewBag.SubmitDate = submitDate.ToString("d");
            double total = claim.TotalMoney;
            #region Information Payment

            Payment payment = db.Payments.FirstOrDefault(p => p.ExpenseClaimID == id);
            ViewBag.AmountDate = payment == null ? DateTime.Now : payment.DatePaid;

            var LsByExpenseClaimId = db.Payments.Where(p => p.ExpenseClaimID == id).ToList();
            double paymentTotalMoney = 0;
            if (LsByExpenseClaimId.Count > 0)
            {
                paymentTotalMoney += LsByExpenseClaimId.Sum(itemPay => itemPay.TotalMoney);
            }
            ViewBag.PaidPaymentAmount = paymentTotalMoney;

            double amountRemains = total - paymentTotalMoney;
            ViewBag.amountRemain = amountRemains;

            ViewBag.PayList = LsByExpenseClaimId;
            #endregion

            return View(expenseClaim);
        }

      
       [HttpPost]
        public ActionResult CreatePaymentByExpenseClaimID(string id, string money, string createDate, string datePaid, string reference)
        {
            var accountList = new SelectList(db.Accounts, "ID", "Name");
            List<SelectListItem> accountItems = accountList.ToList();
           // accountItems.Insert(0, (new SelectListItem { Text = "+ Add new account...", Value = "0" }));
            accountItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));

            ViewBag.AccountID = accountItems;

            Payment payment = new Payment();
            payment.ExpenseClaimID = Convert.ToInt32(id);
            payment.PaymetnTo = "Dữ liệu không xác định";
            payment.TotalMoney = Convert.ToDouble(money);
            payment.CreatedDate = Convert.ToDateTime(createDate);
            ExpenseClaim oExpense = db.ExpenseClaims.Find(Int32.Parse(id));
            payment.CurrencyID = 4;
            payment.DatePaid = DateTime.Now;
            payment.PayToAccount = 0;
            payment.Reference = reference;
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                //Create Transaction for Payment
               // string status = Utility.CreateAccountTransaction(1, Int32.Parse(id), payment);
                db.SaveChanges();
            }

            #region Process when TotalMoney > LessPayment or TotalMonet < LessPayment

            int expenseClaimID = Convert.ToInt32(id);
            var expenseClaimDetailList = db.ExpenseClaimDetails.Where(i => i.ExpenseClaimID == expenseClaimID);
            var paymentList = db.Payments.Where(p => p.ExpenseClaimID == expenseClaimID).ToList();

            double totalMoneyExclaim = Enumerable.Sum(expenseClaimDetailList, invoid => Convert.ToDouble(invoid.TotalMoney));
            double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));

            if (totalPayment >= totalMoneyExclaim)
            {
                ExpenseClaim claim = db.ExpenseClaims.Find(Convert.ToInt32(id));
                claim.Status = 3;
                db.Entry(claim).State = EntityState.Modified;
                db.SaveChanges();
            }

            #endregion
            return View();
        }

       #region view bill

       public ActionResult ViewBill(int id, string tongtien)
       {
           Payment payment = db.Payments.Find(id);
           ExpenseClaim claim = db.ExpenseClaims.Where(ex => ex.ID == payment.ExpenseClaimID).FirstOrDefault();
           ViewBag.ContactName = claim.Contact.ContactName;
           ViewBag.Date = claim.CreatedDate;
           ViewBag.DueDate = claim.DueDate;
           double Total = 0;
           var expenseClaimDetails =
               db.ExpenseClaimDetails.Where(s => s.ExpenseClaimID == id).ToList();
           foreach (var item in expenseClaimDetails)
           {
               Total += Convert.ToDouble(item.TotalMoney);
           }
           ViewBag.Total = Total;
           //ExpenseClaimDetail details =
           //    db.ExpenseClaimDetails.Where(s => s.ExpenseClaimID == payment.ExpenseClaimID).
           //        FirstOrDefault();
           //ViewBag.GoodCode = purdetails.Good.Code;
           ViewBag.TongTien = tongtien;
           return View(payment);
       }
       #endregion
       
        public ActionResult ApproveExpensiveClaim(int id = 0)
        {
            ExpenseClaim claim = db.ExpenseClaims.Where(ex => ex.ID == id).SingleOrDefault();
            var expenseClaim = db.ExpenseClaims.Where(ex => ex.ID == id).ToList();
            ViewBag.Amount = claim.TotalMoney;
            ViewBag.expenseClaimID = id;
            ViewBag.To = user.UserName;
            DateTime due = claim.DueDate != null ? Convert.ToDateTime(claim.DueDate) : DateTime.Now;
            DateTime submitDate = claim.SubmitDate != null ? Convert.ToDateTime(claim.SubmitDate) : DateTime.Now;
            ViewBag.DueDate = due.ToString("d");
            ViewBag.SubmitDate = submitDate.ToString("d");
            return View(expenseClaim);
        }

        public ActionResult Edit(int id = 0)
        {
            ExpenseClaim claim = db.ExpenseClaims.Where(ex => ex.ID == id).SingleOrDefault();
            var claimDetails = db.ExpenseClaimDetails.Where(de => de.ExpenseClaimID == id).ToList();
            ViewBag.SubTotal = claimDetails.Sum(de => de.TotalMoney);

            var contactQuery = from d in db.Contacts
                               where d.Type == 2
                               orderby d.ContactName
                               select d;
            ViewData["AccList"] = new SelectList(contactQuery, "ID", "ContactName", claim.ReceiptFrom);

            var accountsQuery = from d in db.Accounts
                                orderby d.Name
                                select d;
            var list = new SelectList(accountsQuery, "ID", "Name");
            List<SelectListItem> accItems = list.ToList();
            accItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            ViewBag.Account = accItems;

            var taxRateQuery = from d in db.TaxRates
                               orderby d.Name
                               select d;
            var listTaxRate = new SelectList(taxRateQuery, "ID", "Name");
            List<SelectListItem> taxRateItems = listTaxRate.ToList();
            taxRateItems.Insert(0, (new SelectListItem { Text = "", Value = "0", Selected = true }));
            ViewBag.TaxRate = taxRateItems;


            ViewBag.NoteType = CommonLib.EnumHelper.GetDescription(NodeType.ExpenseClaim);
            ViewBag.NoteFromID = claim.ID;
            if(user != null)
            ViewBag.CurrentUserID = user.ID;

            return View(claim);
        }

        private void initDropDowlist()
        {
            var contactQuery = from d in db.Contacts 
                               where d.Type == 2
                               orderby d.ContactName
                               select d;
            var listAcc = new SelectList(contactQuery, "ID", "ContactName");        
            List<SelectListItem> contactItems = listAcc.ToList();
            contactItems.Insert(0, (new SelectListItem { Text = "", Value = "0", Selected = true }));
            ViewData["AccList"] = contactItems;

            var accountsQuery = from d in db.Accounts
                                orderby d.Name
                                select d;
            var list = new SelectList(accountsQuery, "ID", "Name");
            List<SelectListItem> accItems = list.ToList();           
            accItems.Insert(0, (new SelectListItem { Text = "", Value = "-1", Selected = true }));
            ViewData["AccountList"] = accItems;

            var taxRateQuery = from d in db.TaxRates
                               orderby d.Name
                               select d;
            var listTaxRate = new SelectList(taxRateQuery, "ID", "Name");
            List<SelectListItem> taxRateItems = listTaxRate.ToList();
            taxRateItems.Insert(0, (new SelectListItem { Text = "", Value = "0", Selected = true }));
            ViewData["TaxRateList"] = taxRateItems;
        }

        public ViewResult CurrentClaim(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var expenseClaim = db.ExpenseClaims.Where(ex => ex.Status == 0).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                expenseClaim = SearchExpense(expenseClaim, searchString).ToList();
            }
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(expenseClaim.ToPagedList(pageNumber, pageSize));
        }

        public ViewResult PreviousClaim(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var expenseClaim = db.ExpenseClaims.Where(x => x.Status != 0).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                expenseClaim = SearchExpense(expenseClaim, searchString).ToList();
            }
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, float>(GetTotalMoneyOfExpenseClaim);
            return View(expenseClaim.ToPagedList(pageNumber, pageSize));
        }

        public ViewResult AwaitingAuthorisation(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var expenseClaim = db.ExpenseClaims.Where(x => x.Status == 1).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                expenseClaim = SearchExpense(expenseClaim, searchString).ToList();
            }
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            return View(expenseClaim.ToPagedList(pageNumber, pageSize));
        }

        public ViewResult AwaitingPayment(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var expenseClaim = db.ExpenseClaims.Where(x => x.Status == 2).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                expenseClaim = SearchExpense(expenseClaim, searchString).ToList();
            }
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, float>(GetTotalMoneyOfExpenseClaim);

            return View(expenseClaim.ToPagedList(pageNumber, pageSize));
        }



        public ViewResult Achive(string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var expenseClaim = db.ExpenseClaims.Where(x => x.Status == 3 || x.Status == 2).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                expenseClaim = SearchExpense(expenseClaim, searchString).ToList();
            }
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            ViewBag.TotalMoney = new Func<int, float>(GetTotalMoneyPaid);
            ViewBag.TotalMoneySale = new Func<int, float>(GetTotalMoneyOfExpenseClaim);
            return View(expenseClaim.ToPagedList(pageNumber, pageSize));
        }


        public IList<ExpenseClaim> SearchExpense(IList<ExpenseClaim> expenses, string sStr)
        {           
            var expensesRef = expenses.Where(i => i.Reference.Contains(sStr)).ToList();
            return expensesRef;
        }

        [HttpPost]
        public string DelExpenseClaim(string[] expenseClaimID)
        {
            string result = "success";
            try
            {
                for (int i = 0; i < expenseClaimID.Length; i++)
                {
                    if (!String.IsNullOrEmpty(expenseClaimID[i]))
                    {
                        int x = Int32.Parse(expenseClaimID[i]);
                        var lstExpenseClaim = db.ExpenseClaimDetails.Where(p => p.ExpenseClaimID == x).ToList();
                        foreach (ExpenseClaimDetail oDetail in lstExpenseClaim)
                        {
                            db.ExpenseClaimDetails.Remove(oDetail);
                        }
                        ExpenseClaim oClaim = db.ExpenseClaims.Find(Int64.Parse(expenseClaimID[i]));
                        db.ExpenseClaims.Remove(oClaim);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public string Approved(string[] expenseClaimID, int? status)
        {
            string result = "";
            ExpenseClaim oClaim = null;
            try
            {
                foreach (string item in expenseClaimID)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        //Create Account Transaction for report later
                       /* if (status == 2)
                        {
                            result = Utility.CreateAccountTransaction(1, Int32.Parse(item), null);
                            if (result.Equals("1"))
                            {
                                ExpenseClaim oClaim = db.ExpenseClaims.Find(Int64.Parse(item));
                                oClaim.Status = status ?? 0;                               
                                db.Entry(oClaim).State = EntityState.Modified;
                            }
                        }
                        else
                        { */
                            oClaim = db.ExpenseClaims.Find(Int64.Parse(item));
                            oClaim.Status = status ?? 0;
                            oClaim.SubmitDate = DateTime.Today;
                            db.Entry(oClaim).State = EntityState.Modified;
                       // }
                    }
                }
                DateTime submitDate = oClaim.SubmitDate != null ? Convert.ToDateTime(oClaim.SubmitDate) : DateTime.Now;                
                ViewBag.SubmitDate = submitDate.ToString("d");

                db.SaveChanges();
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        [HttpPost]
        public string Authorize(string Id, string DueDate, string ReportingDate)
        {
            string result = "";
            try
            {

                    if (!String.IsNullOrEmpty(Id))
                    {
 
                        ExpenseClaim oClaim = db.ExpenseClaims.Find(Int64.Parse(Id));
                        oClaim.DueDate = DateTime.Parse(DueDate);
                        oClaim.ReportingDate = DateTime.Parse(ReportingDate);
                        oClaim.SubmitDate = DateTime.Today;
                        oClaim.Status = 2;
                        db.Entry(oClaim).State = EntityState.Modified;
                    
                }
                db.SaveChanges();
                result = "success";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public float GetTotalMoneyPaid(int expenseClaimId)
        {
            var listObj = db.Payments.Where(x => x.ExpenseClaimID == expenseClaimId).ToList();
            float? total = float.Parse(listObj.Sum(x => x.TotalMoney).ToString());
            float z = total ?? 0;
            return z;
        }

        public float GetTotalMoneyOfExpenseClaim(int expenseClaimId)
        {
            var listObj = db.ExpenseClaimDetails.Where(x => x.ExpenseClaimID == expenseClaimId).ToList();
            float? total = float.Parse(listObj.Sum(x => x.Price * x.Quantity).ToString());
            float z = total ?? 0;
            return z;
        }

    }
}
