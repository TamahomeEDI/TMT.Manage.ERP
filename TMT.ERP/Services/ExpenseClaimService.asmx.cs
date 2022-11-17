using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for ExpenseClaim
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ExpenseClaimService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
       // User user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SaveExpenseClaim(int? expenseClaimID, int? receiptFrom, string receiptDate, string reference, double totalMoney, int includeTax, int amountAre, int status, double? totalTax, int currentUserID, string expensiveDetail)
        {
            object result = 0;
            try
            {
                ErpEntities db = new ErpEntities();
                TMT.ERP.DataAccess.Model.ExpenseClaim oExpense = null;
                if (expenseClaimID.GetValueOrDefault() > 0)
                {
                    oExpense = db.ExpenseClaims.Find(expenseClaimID);
                    var oExpenseDetails = db.ExpenseClaimDetails.Where(ex => ex.ExpenseClaimID == expenseClaimID);
                    foreach (var item in oExpenseDetails)
                    {
                        db.Entry(item).State = System.Data.EntityState.Deleted;
                    }
                }
                else
                {
                    oExpense = new ExpenseClaim();
                    db.ExpenseClaims.Add(oExpense);
                }

                oExpense.CreatedUserID = currentUserID;
                oExpense.ApprovalUserID = currentUserID;
                oExpense.CreatedDate = DateTime.Today;
                oExpense.Reference = reference;
                oExpense.ReceiptFrom = receiptFrom;
                oExpense.ReceiptDate = DateTime.Parse(receiptDate);
                oExpense.TotalMoney = totalMoney;
                oExpense.IncludeTax = includeTax;
                oExpense.AmountAre = amountAre;
                oExpense.Status = status;
                oExpense.TotalTax = totalTax;
                db.SaveChanges();
                dynamic array = JsonConvert.DeserializeObject(expensiveDetail);
                foreach (var item in array)
                {
                    if (((ICollection)item).Count > 0)
                    {
                        ExpenseClaimDetail oDetail = new ExpenseClaimDetail();
                        oDetail.ExpenseClaimID = oExpense.ID;

                        oDetail.Quantity = item.quantity;
                        oDetail.Price = item.unitPrice;
                        oDetail.TotalMoney = item.totalMoney;
                        oDetail.AccountID = item.accountID;
                        oDetail.TaxRateID = item.taxRateID;
                        oDetail.Description = item.description;
                        db.ExpenseClaimDetails.Add(oDetail);
                    }
                }

                db.SaveChanges();
                result = oExpense.ID;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }
    }
}
