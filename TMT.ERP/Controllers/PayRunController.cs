using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using CommonLib;
using Newtonsoft.Json;
using PagedList;
using TMT.ERP.BusinessLogic.Utils;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using System.Collections.Generic;
using TMT.ERP.Models.Lookups;
using TMT.ERP.Services;

namespace TMT.ERP.Controllers
{
    public class PayRunController : BaseController
    {
        private readonly ErpEntities _db = new ErpEntities();
        private readonly GenericManager<PayRun> _manager = new GenericManager<PayRun>();
        private readonly GenericManager<PayRunEmployee> _managerPayRunEmployee = new GenericManager<PayRunEmployee>();
        private readonly GenericManager<PayRunEmployeeDetail> _managerPayRunEmployeeDetail = new GenericManager<PayRunEmployeeDetail>();
        private readonly GenericManager<Employee> _managerEmployee = new GenericManager<Employee>();
        private readonly GenericManager<PayItem> _managerPayItem = new GenericManager<PayItem>();
        private readonly GenericManager<PaymentForPayItem> _managerPaymentForPayment = new GenericManager<PaymentForPayItem>();
        private readonly GenericManager<Payment> _managerPayment = new GenericManager<Payment>();
        //private readonly GenericManager<PayItemType> _managerPayItemType = new GenericManager<PayItemType>();
        //private readonly GenericManager<Account> _managerAccount = new GenericManager<Account>();
        private readonly GenericManager<AccountTran> _managerAccountTran = new GenericManager<AccountTran>();
        private readonly GenericManager<AccountFeature> _managerAccountFeature = new GenericManager<AccountFeature>();
        public static int PageSize = TMT.ERP.Commons.Constant.DefaultPageSize;
        private readonly CultureInfo _cultureInfo = new CultureInfo("en-US");
        private List<SelectListItem> InitPayItem()
        {
            var payList = new SelectList(_db.PayItems, "ID", "Name");
            List<SelectListItem> payItems = payList.ToList();
            return payItems;
        }
        private List<AccountInfo> InitAccount()
        {
            int[] groupId = new int[] { Convert.ToInt32(TMT.ERP.Models.Lookups.AccountGroup.Payment) };
            var listAccount = TMT.ERP.BusinessLogic.Utils.AccountManager.GetAccountByGroups(groupId);
            return listAccount;
        }
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }
        #region View
        public ActionResult Index(int? page)
        {
            var objList = _db.PayRuns.ToList();
            int pageSize = PageSize;
            ViewBag.PageSize = PageSize;
            int pageNumber = (page ?? 1);

            return View(objList.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult SetDate()
        {
            return View();
        }
        public ActionResult SetDateEmployee(int payrunId)
        {
            var oPayRun = _manager.FindById(payrunId);
            if (oPayRun == null)
            {
                return HttpNotFound();
            }
            ViewBag.PayRunID = payrunId;
            return View(oPayRun);
        }
        public ActionResult EnterEmployee(int payrunId, int? payRunEmployeeID)
        {
            var oPayRun = _manager.FindById(payrunId);
            PayRunEmployee oPayRunEmployee = payRunEmployeeID == 0 ? oPayRun.PayRunEmployees.First() : _managerPayRunEmployee.FindById(payRunEmployeeID);
            var ooPayRunEmployeeDetail = _managerPayRunEmployeeDetail.Get(x => x.PayRunEmployeeID == oPayRunEmployee.ID);
            ViewBag.Employee = oPayRun.PayRunEmployees.OrderBy(x => x.ID).ToList();
            ViewBag.PayItemList = InitPayItem();
            ViewBag.PayRunID = payrunId;
            ViewBag.PayRunEmployeeID = oPayRunEmployee.ID;
            ViewBag.ooPayRunEmployeeDetail = ooPayRunEmployeeDetail;
            return View(ooPayRunEmployeeDetail);
        }
        #endregion

        #region Process Logic
        public string SavePayRun(int? payRunID, int userId, int payFrequence, string fromDate, string toDate, string paymentDate, int status)
        {
            string result = "success";
            try
            {
                #region PayRun
                var listEmployee = _managerEmployee.Get().ToList();
                var oPayRun = _manager.FindById(payRunID);
                if (oPayRun == null)
                {
                    oPayRun = new PayRun
                                  {
                                      Code = Utility.GetCode(CodeType.PayRun.GetDescription()),
                                      CreatedUserID = userId,
                                      ApprovalUserID = userId,
                                      NumOfEmployee = 0,
                                      EmployeePayment = 0,
                                      OtherPayment = 0,
                                      Total = 0,
                                  };
                    _manager.Add(oPayRun);
                    Utility.UpdateNextNumber(CodeType.PayRun.GetDescription(), Constant.CODE_LENGTH, "");
                }
                oPayRun.PayFrequency = payFrequence;
                oPayRun.FromDate = !string.IsNullOrEmpty(fromDate) ? Convert.ToDateTime(fromDate, _cultureInfo) : DateTime.Now;
                oPayRun.ToDate = !string.IsNullOrEmpty(toDate) ? Convert.ToDateTime(toDate, _cultureInfo) : DateTime.Now;
                oPayRun.PaymentDate = !string.IsNullOrEmpty(paymentDate) ? Convert.ToDateTime(paymentDate, _cultureInfo) : DateTime.Now;
                oPayRun.Status = status;
                #endregion
                #region PayRunEmploye
                if (status == 1)
                {
                    var listPayItem = _managerPayItem.Get().ToList();
                    foreach (var employee in listEmployee)
                    {
                        var oPayRunEmployee = new PayRunEmployee();
                        oPayRunEmployee.PayRunID = oPayRun.ID;
                        oPayRunEmployee.EmployeeID = employee.ID;
                        oPayRunEmployee.Active = true;
                        oPayRun.PayRunEmployees.Add(oPayRunEmployee);
                        foreach (var xx in listPayItem)
                        {
                            var oPayRunEmployeeDetail = new PayRunEmployeeDetail();
                            oPayRunEmployeeDetail.PayRunEmployeeID = oPayRunEmployee.ID;
                            oPayRunEmployeeDetail.PayItemID = xx.ID;
                            oPayRunEmployeeDetail.Quantity = 0;
                            oPayRunEmployeeDetail.Rate = xx.DefaultRate ?? 0;
                            oPayRunEmployeeDetail.Amount = 0;
                            oPayRunEmployeeDetail.TotalPay = 0;
                            oPayRunEmployeeDetail.Status = 0;
                            oPayRunEmployee.PayRunEmployeeDetails.Add(oPayRunEmployeeDetail);
                        }
                    }
                }
                #endregion
                _manager.Save();
                result = oPayRun.ID.ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string Update(int payrunEmployeeId, string payrunEmployeeDetails, int payrunId, int status)
        {
            string result = "success";
            try
            {
                var oPayRunEmployee = _managerPayRunEmployee.FindById(payrunEmployeeId);
                dynamic array = JsonConvert.DeserializeObject(payrunEmployeeDetails);
                if (oPayRunEmployee != null)
                {
                    foreach (var item in array)
                    {
                        var id = Int32.Parse(((string)item.id).Replace("{", "").Replace("}", ""));
                        var payItemID = Int32.Parse(((string)item.payitem).Replace("{", "").Replace("}", ""));
                        var payItemDetail =
                            _managerPayRunEmployeeDetail.FindById(id);
                        if (payItemDetail == null)
                        {
                            payItemDetail = new PayRunEmployeeDetail();
                            payItemDetail.PayRunEmployeeID = oPayRunEmployee.EmployeeID;
                        }
                        var quantity = Int32.Parse(((string)item.quanlity));
                        var rate = double.Parse(((string)item.rate));
                        payItemDetail.PayItemID = payItemID;
                        payItemDetail.Quantity = quantity;
                        payItemDetail.Rate = rate;
                        payItemDetail.Amount = quantity * rate;
                        payItemDetail.TotalPay = quantity * rate;
                    }
                    _managerPayRunEmployeeDetail.Save();
                }

                #region Update PayRun
                var oPayRun = _manager.FindById(payrunId);
                if (status == 2)
                {
                    var ooPayRunEmployee = oPayRun.PayRunEmployees.Where(x => x.Active == true).ToList();
                    double wages = 0;
                    double allowances = 0;
                    double deductions = 0;
                    double taxes = 0;
                    double nontaxableAllowances = 0;
                    double posttaxDeductions = 0;
                    double employerContributions = 0;

                    foreach (var item in ooPayRunEmployee)
                    {
                        var ooPayRunEmployeeDetail = item.PayRunEmployeeDetails.ToList();
                        wages += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.Wages)).Sum(x => Convert.ToDouble(x.TotalPay));
                        allowances += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.Allowances)).Sum(x => Convert.ToDouble(x.TotalPay));
                        deductions += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.Deductions)).Sum(x => Convert.ToDouble(x.TotalPay));
                        taxes += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.Taxes)).Sum(x => Convert.ToDouble(x.TotalPay));
                        nontaxableAllowances += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.NonTaxableAllowances)).Sum(x => Convert.ToDouble(x.TotalPay));
                        posttaxDeductions += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.PostTaxDeductions)).Sum(x => Convert.ToDouble(x.TotalPay));
                        employerContributions += ooPayRunEmployeeDetail.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(PayItemTypeValue.EmployerContributions)).Sum(x => Convert.ToDouble(x.TotalPay));
                    }
                    double ortherPayment = employerContributions + deductions + taxes + posttaxDeductions;
                    double employeePayments = wages + allowances + nontaxableAllowances - deductions - taxes - posttaxDeductions;
                    double total = ortherPayment + employeePayments;

                    oPayRun.NumOfEmployee = ooPayRunEmployee.Count();
                    oPayRun.OtherPayment = Convert.ToInt32(ortherPayment);
                    oPayRun.EmployeePayment = Convert.ToInt32(employeePayments);
                    oPayRun.Total = Convert.ToInt32(total);
                }
                oPayRun.Status = status;
                _manager.Update(oPayRun);
                _manager.Save();
                #endregion
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string AddRemove(int payItemOld, int payItemNew, int payRunEmployeeId)
        {
            string result = "success";
            try
            {
                var oObj = _managerPayRunEmployeeDetail.Get(x => x.PayItemID == payItemOld && x.PayRunEmployeeID == payRunEmployeeId).First();
                _managerPayRunEmployeeDetail.Delete(oObj);

                var oObjNew = new PayRunEmployeeDetail();
                oObjNew.PayRunEmployeeID = payRunEmployeeId;
                oObjNew.PayItemID = payItemNew;
                oObjNew.Quantity = 0;
                oObjNew.Rate = 0;
                oObjNew.Amount = 0;
                oObjNew.TotalPay = 0;
                oObjNew.Status = 0;
                _managerPayRunEmployeeDetail.Add(oObjNew);

                _managerPayRunEmployeeDetail.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string RemoveRow(int id)
        {
            var oPay = _managerPayRunEmployeeDetail.FindById(id);
            _managerPayRunEmployeeDetail.Delete(oPay);
            _managerPayRunEmployeeDetail.Save();
            return "success";
        }
        public string AddNewLine(int payrunEmployeeId)
        {
            var obj = _managerPayRunEmployeeDetail.Get(x => x.PayRunEmployeeID == payrunEmployeeId).Last();
            var objNew = new PayRunEmployeeDetail();
            objNew.PayRunEmployeeID = obj.PayRunEmployeeID;
            objNew.PayItemID = obj.PayItemID;
            objNew.Quantity = obj.Quantity;
            objNew.Rate = obj.Rate;
            objNew.Amount = obj.Amount;
            objNew.TotalPay = obj.TotalPay;
            objNew.Status = obj.Status;

            _managerPayRunEmployeeDetail.Add(objNew);
            _managerPayRunEmployeeDetail.Save();
            return "success";
        }
        public string UpdatePayRunEmployee(int id, string flag)
        {
            string result = "success";
            try
            {
                var oPayRunEmployee = _managerPayRunEmployee.FindById(id);
                if (oPayRunEmployee != null)
                {
                    if (flag == "true")
                    {
                        oPayRunEmployee.Active = true;
                    }
                    if (flag == "false")
                    {
                        oPayRunEmployee.Active = false;
                    }
                    _managerPayRunEmployee.Update(oPayRunEmployee);
                    _managerPayRunEmployee.Save();

                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string ReturnUpdatePayRunEmployee(int payrunId)
        {
            string result = "success";
            try
            {
                var oPayRun = _manager.FindById(payrunId);
                var ooPayRunEmployee = oPayRun.PayRunEmployees.Where(x => x.Active == true).ToList();
                foreach (var v in ooPayRunEmployee)
                {
                    var ooPay = v.PayRunEmployeeDetails.ToList();
                    double wages = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Wages)).Sum(x => Convert.ToDouble(x.TotalPay));
                    double allowances = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Allowances)).Sum(x => Convert.ToDouble(x.TotalPay));
                    double deductions = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Deductions)).Sum(x => Convert.ToDouble(x.TotalPay));
                    double taxes = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Taxes)).Sum(x => Convert.ToDouble(x.TotalPay));
                    double nontaxableAllowances = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.NonTaxableAllowances)).Sum(x => Convert.ToDouble(x.TotalPay));
                    double posttaxDeductions = ooPay.Where(x => x.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.PostTaxDeductions)).Sum(x => Convert.ToDouble(x.TotalPay));
                    if (wages + allowances + nontaxableAllowances - deductions - taxes - posttaxDeductions <= 0)
                    {
                        result = "/PayRun/EnterEmployee?payrunId=" + payrunId + "&payRunEmployeeID=" + v.ID;
                        break;
                    }
                    else
                    {
                        result = "/PayRun/Details?payrunId=" + payrunId;
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        #endregion

        public double GetTotal(int emp, int payrun, int payitem)
        {
            var oPayRunEmployee = _managerPayRunEmployee.Get(x => x.EmployeeID == emp && x.PayRunID == payrun).First();
            var oPayRunEmployeeDetail =
                _managerPayRunEmployeeDetail.Get(x => x.PayRunEmployeeID == oPayRunEmployee.ID && x.PayItemID == payitem)
                    .First();
            return oPayRunEmployeeDetail.TotalPay ?? 0;
        }
        public ActionResult ViewPayRun(int payrunId)
        {
            ViewBag.PaidTo = new Func<int, string>(GetPaidTo);
            ViewBag.ID = payrunId;
            var oPayRun = _manager.FindById(payrunId);
            return View(oPayRun);
        }
        public ActionResult ViewVoid(int payrunId)
        {
            ViewBag.PaidTo = new Func<int, string>(GetPaidTo);
            ViewBag.ID = payrunId;
            var oPayRun = _manager.FindById(payrunId);
            return View(oPayRun);
        }
        public ActionResult BatchPayEmployees(int payrunId)
        {
            ViewBag.ID = payrunId;
            var oPayRun = _manager.FindById(payrunId);
            ViewBag.AccountID = InitAccount();
            return View(oPayRun);
        }
        public ActionResult SendPayslips(int payrunId)
        {
            ViewBag.ID = payrunId;
            var oPayRun = _manager.FindById(payrunId);
            return View(oPayRun);
        }
        public string GetPaidTo(int payitemTypeId)
        {
            var result = "Employee";
            return result;
        }
        public string VoidPayRun(int payrunId)
        {
            string result = "success";
            try
            {
                var oPayRun = _manager.FindById(payrunId);
                oPayRun.EmployeePayment = 0;
                oPayRun.OtherPayment = 0;
                oPayRun.Status = 4;

                _manager.Update(oPayRun);
                _manager.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string DeletePayRun(int payrunId)
        {
            string result = "success";
            try
            {
                var oPayRun = _manager.FindById(payrunId);
                var ooPayRunEmployee = oPayRun.PayRunEmployees.ToList();
                var ooPaymentForPayItem = oPayRun.PaymentForPayItems.ToList();

                foreach (var x in ooPayRunEmployee)
                {
                    var ooPayRunEmployeeDetails = x.PayRunEmployeeDetails.ToList();
                    foreach (var xx in ooPayRunEmployeeDetails)
                    {
                        _managerPayRunEmployeeDetail.Delete(xx);
                    }
                    _managerPayRunEmployee.Delete(x);
                }                                                
                _manager.Delete(oPayRun);
                _manager.Save();
                result = "Delete PayRun success";
            }
            catch (Exception ex)
            {
                result = "Error delete PayRun";
            }
            return result;
        }
        public ActionResult Details(int payrunId)
        {
            PayRun oPayRun = _manager.FindById(payrunId);
            ViewBag.ID = payrunId;
            ViewBag.PayItem = _managerPayItem.Get().ToList();
            return View(oPayRun);
        }
        public string SavePaymentForPayItem(int payrunId, string payDetails, string payEmployeeDetails, string paySlipNotes, bool showEmployeeTax)
        {
            string result = "success";
            try
            {

                dynamic array = JsonConvert.DeserializeObject(payDetails);
                var oAccount800 = _managerAccountFeature.Get(x => x.ID == 11).First().Account;
                var oAccount803 = _managerAccountFeature.Get(x => x.ID == 12).First().Account;
                foreach (var oPay in array)
                {
                    var oPayment = new PaymentForPayItem();
                    oPayment.PayRunID = payrunId;
                    oPayment.PayItemID = Int32.Parse((string)oPay.payItemID);
                    oPayment.DueDate = DateTime.Parse((string)oPay.dueDate);
                    oPayment.Amount = Double.Parse((string)oPay.amount);
                    oPayment.Payment = 0;
                    oPayment.RemainAmount = Double.Parse((string)oPay.amount);
                    oPayment.Status = 0;
                    _managerPaymentForPayment.Add(oPayment);
                    _managerPaymentForPayment.Save();
                    //Transaction for OrtherPayment
                    //Tao 1 transaction amount=tat ca other payment cong lai va account=800                    
                    //1 Account 800

                    var oAccountTran = new AccountTran();
                    oAccountTran.AccountID = oAccount800.ID;
                    oAccountTran.AccountType = oAccount800.AccountTypeID;
                    oAccountTran.Description = "Orther Payment";
                    oAccountTran.Date = DateTime.Now;
                    oAccountTran.Received = oPayment.Amount;
                    oAccountTran.SourceID = oPayment.ID;
                    oAccountTran.SourceType = 1;
                    _managerAccountTran.Add(oAccountTran);
                    //2 Account Employer
                }

                dynamic arrayPayRunEmployee = JsonConvert.DeserializeObject(payEmployeeDetails);
                foreach (var x in arrayPayRunEmployee)
                {
                    int id = Int32.Parse((string)x.Id);
                    double amount = Double.Parse((string)x.amount);
                    var oPayRunEmployee = _managerPayRunEmployee.FindById(id);
                    oPayRunEmployee.Amount = amount;
                    oPayRunEmployee.Payment = 0;
                    oPayRunEmployee.RemainAmount = amount;
                    oPayRunEmployee.Status = 0;
                    oPayRunEmployee.PayslipNotes = paySlipNotes;
                    oPayRunEmployee.ShowEmployeeTax = showEmployeeTax;
                    _managerPayRunEmployee.Update(oPayRunEmployee);

                }
                //Transaction for Employee payments
                var ooPayRunEmployee =
                    _managerPayRunEmployee.Get(x => x.PayRunID == payrunId && x.Active == true).ToList();
                var ooPayRunEmployeeDetails = ooPayRunEmployee.SelectMany(item => item.PayRunEmployeeDetails.ToList()).ToList();
                foreach (var xx in ooPayRunEmployeeDetails)
                {
                    if (xx.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Wages) || xx.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.Allowances) || xx.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.NonTaxableAllowances) || xx.PayItem.PayItemTypeID == Convert.ToInt32(TMT.ERP.Models.Lookups.PayItemTypeValue.EmployerContributions))
                    {
                        var oAccountTran = new AccountTran();
                        var oPayItem = _managerPayItem.FindById(xx.PayItemID);
                        var oAccount = oPayItem.Account;
                        oAccountTran.AccountID = oAccount.ID;
                        oAccountTran.AccountType = oAccount.AccountTypeID;
                        oAccountTran.Description = "Employee Payment";
                        oAccountTran.Date = DateTime.Now;

                        oAccountTran.Spent = xx.Amount;

                        oAccountTran.SourceID = xx.PayRunEmployeeID;
                        oAccountTran.SourceType = 0;
                        _managerAccountTran.Add(oAccountTran);
                    }
                }
                //Transaction for account 803               
                foreach (var item in ooPayRunEmployee)
                {
                    var oAccountTran = new AccountTran();
                    oAccountTran.AccountID = oAccount803.ID;
                    oAccountTran.AccountType = oAccount803.AccountTypeID;
                    oAccountTran.Description = "Employee Payment";
                    oAccountTran.Date = DateTime.Now;
                    oAccountTran.Spent = item.Amount;
                    oAccountTran.SourceID = item.ID;
                    oAccountTran.SourceType = 0;
                    _managerAccountTran.Add(oAccountTran);
                }
                _managerAccountTran.Save();

                _managerPayRunEmployee.Save();
                var oPayRun = _manager.FindById(payrunId);
                oPayRun.Status = 3;
                _manager.Update(oPayRun);
                _manager.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public ActionResult ViewPaySlip(int payrunId, int payrunEmployeeId)
        {
            var oPayRun = _manager.FindById(payrunId);
            var oPayRunEmployee = _managerPayRunEmployee.FindById(payrunEmployeeId);
            ViewBag.PayRunEmployee = oPayRunEmployee;
            ViewBag.AccountID = InitAccount();
            return View(oPayRun);
        }
        public ActionResult ViewWagePayable(int payrunId, int payItemId)
        {
            var oPayRun = _manager.FindById(payrunId);
            var oPayItem = _managerPayItem.FindById(payItemId);
            var oPayment = _managerPaymentForPayment.Get(x => x.PayRunID == payrunId && x.PayItemID == payItemId).First();
            var ooPayDetails = _managerPayRunEmployeeDetail.Get().ToList();
            var ooPayment = oPayment.Payments.ToList();

            ViewBag.ListPayment = ooPayment;
            ViewBag.PayRun = oPayRun;
            ViewBag.Payment = oPayment;
            ViewBag.PayItem = oPayItem;
            ViewBag.PayItemID = oPayItem.ID;
            ViewBag.PayRunDetails = ooPayDetails;
            ViewBag.AccountID = InitAccount();
            return View(oPayRun);
        }
        public string UpdateStatusPayment(int paymentId, string totalMoney, int type)
        {
            //0 Id for paymentForPayItem, 1 id for PayrunEmployee
            string result = "success";
            try
            {
                if (type == 0)
                {
                    var oPaymentForPayItem = _managerPaymentForPayment.FindById(paymentId);
                    var ooPayment = _managerPayment.Get(x => x.PaymentForPayItemID == paymentId);
                    var totalPay = ooPayment.Sum(x => Convert.ToDouble(x.TotalMoney));
                    var money = Convert.ToDouble(totalMoney);
                    if (Math.Abs(totalPay - 0.1) < money)
                    {
                        oPaymentForPayItem.Status = 1;
                        _managerPaymentForPayment.Update(oPaymentForPayItem);
                        _managerPaymentForPayment.Save();
                    }

                }
                if (type == 1)
                {
                    var oPayRunEmployee = _managerPayRunEmployee.FindById(paymentId);
                    var ooPayment = _managerPayment.Get(x => x.PayRunEmployeeID == paymentId);
                    var totalPay = ooPayment.Sum(x => Convert.ToDouble(x.TotalMoney));
                    var money = Convert.ToDouble(totalMoney);
                    if (Math.Abs(money - 0.1) < totalPay)
                    {
                        oPayRunEmployee.Status = 1;
                        _managerPayRunEmployee.Update(oPayRunEmployee);
                        _managerPayRunEmployee.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
        public string CheckDate(string dateFrom,string dateTo)
        {
            string result = "success";
            try
            {
                DateTime dateFromx = Convert.ToDateTime(dateFrom);
                DateTime dateTox = Convert.ToDateTime(dateTo);
                TimeSpan span = dateTox.Subtract(dateFromx);
                double value = span.TotalMinutes;
                if(value>0)
                {
                    result = "success";
                }else
                {
                    result = "fail";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;            
        }
    }
}
