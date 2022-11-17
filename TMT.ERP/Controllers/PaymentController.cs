using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly GenericManager<Payment> _manager = new GenericManager<Payment>();
        private readonly GenericManager<Currency> _managerCurrency = new GenericManager<Currency>();
        public string Create(int paymentId, string paymentTo, string totalMoney, string currentId, string datePaid, string paytoAccount, string reference, int status, int type)
        {
            //type : 0 SaleInvoiceID,1 PurchaseID,2 ExpenseClaimID,3 PaymentForPayItemID,4 PayRunEmployeeID
           string result = "success";
            try
            {
                var oPayment = new Payment();
                oPayment.CreatedDate = DateTime.Now;
                oPayment.CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;
                oPayment.ApprovalUserID = AppContext.RequestVariables.CurrentUser.ID;
                switch (type)
                {
                    case 0:
                        oPayment.SaleInvoiceID = paymentId;
                        break;
                    case 1:
                        oPayment.PurchaseID = paymentId;
                        break;
                    case 2:
                        oPayment.ExpenseClaimID = paymentId;
                        break;
                    case 3:
                        oPayment.PaymentForPayItemID = paymentId;
                        break;
                    case 4:
                        oPayment.PayRunEmployeeID = paymentId;
                        break;
                    default:
                        oPayment.SaleInvoiceID = paymentId;
                        break;

                }
                oPayment.PaymetnTo = !string.IsNullOrEmpty(paymentTo) ? paymentTo : "";
                oPayment.TotalMoney = !string.IsNullOrEmpty(totalMoney) ? Convert.ToDouble(totalMoney) : 0;
                oPayment.CurrencyID = !string.IsNullOrEmpty(currentId) ? Convert.ToInt32(currentId) : _managerCurrency.Get().First().ID;
                oPayment.DatePaid = !string.IsNullOrEmpty(datePaid) ? DateTime.Parse(datePaid) : DateTime.Now;
                oPayment.PayToAccount = !string.IsNullOrEmpty(paytoAccount) ? Convert.ToInt32(paytoAccount) : 0;
                oPayment.Reference = !string.IsNullOrEmpty(reference) ? reference : "";
                oPayment.Status = 0;
                _manager.Add(oPayment);
                _manager.Save();
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

    }
}
