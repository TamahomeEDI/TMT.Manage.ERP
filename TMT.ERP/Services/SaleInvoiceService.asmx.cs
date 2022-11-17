using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.Controllers;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using Newtonsoft.Json;
using CommonLib;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for SaleInvoiceService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.     
    [System.Web.Script.Services.ScriptService]
    public class SaleInvoiceService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //saleType: 0-SaleInvoice; 1-CreditNote
        public string SaveSaleInvoice(int? saleInvoiceID, int? supplierID, string supplierName, string createDate, string dueDate,
                                         string invoiceNo, string reference, int? currencyID, int? vatID, int userID, float? subTotal,
                                         float? totalTax, float? totalMoney, int status, int saleType, string saleInvoiceDetail)
        {
            string result = string.Empty;
            try
            {
                //ErpEntities db = new ErpEntities();
                //SaleInvoice oSaleInvoice = null;
                //var manager = new GenericManager<SaleInvoice>();
                //if (saleInvoiceID.HasValue)
                //{
                //    oSaleInvoice = manager.FindById(saleInvoiceID);
                //    if (oSaleInvoice != null)
                //    {
                //        oSaleInvoice.SaleInvoiceDetails.ToList().ForEach(x => manager.Delete(x));
                //    }
                //}

                var manager = new GenericManager<SaleInvoice>();
                SaleInvoice oSaleInvoice = manager.FindById(saleInvoiceID);
                if (oSaleInvoice != null)
                {
                    oSaleInvoice.SaleInvoiceDetails.ToList().ForEach(x => manager.Delete(x));
                }
                else
                {
                    if (manager.Get().Where(x => x.Code == invoiceNo).ToList().Count > 0)
                    {
                        result = "ERROR: " + Resources.Resource.Common_InvoiceUnique;
                        Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.SaleOrder), Constant.CODE_LENGTH, "");
                        invoiceNo = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.SaleOrder));
                        //return serializer.Serialize(result);
                    }

                    oSaleInvoice = new SaleInvoice();
                    manager.Add(oSaleInvoice);
                    oSaleInvoice.CreatedUserID = userID;
                    oSaleInvoice.RemainMoney = totalMoney;
                    oSaleInvoice.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                }

                oSaleInvoice.Status = status;
                oSaleInvoice.CreatedDate = !string.IsNullOrEmpty(createDate) ? Convert.ToDateTime(createDate) : DateTime.Now;
                oSaleInvoice.Code = invoiceNo;
                oSaleInvoice.ContactName = supplierName;
                oSaleInvoice.ContactID = new ContactService().GetContactByName(supplierName, 1);
                oSaleInvoice.DueDate = !string.IsNullOrEmpty(dueDate) ? Convert.ToDateTime(dueDate) : DateTime.Now;
                if (status == 1)
                    oSaleInvoice.ApprovalUserID = userID;
                oSaleInvoice.StockID = Constant.CURRENT_STOCK_ID;
                oSaleInvoice.CurrencyID = currencyID;
                oSaleInvoice.UseVAT = vatID;
                oSaleInvoice.Reference = reference;
                oSaleInvoice.SubTotal = subTotal;
                oSaleInvoice.TotalTax = totalTax;
                oSaleInvoice.TotalMoney = totalMoney;
                oSaleInvoice.Type = saleType;
                //db.SaveChanges();                
                //result = oSaleInvoice.ID;
                dynamic array = JsonConvert.DeserializeObject(saleInvoiceDetail);
                foreach (var item in array)
                {
                    var quantity = ((string)item.quantity).Replace("{", "").Replace("}", "");
                    SaleInvoiceDetail oSaleInvoiceDetails = new SaleInvoiceDetail();
                    oSaleInvoiceDetails.SaleInvoiceID = oSaleInvoice.ID;
                    oSaleInvoiceDetails.GoodID = item.goodId;
                    oSaleInvoiceDetails.Quantity = Convert.ToInt32(Convert.ToDouble(quantity==""?"0":quantity)); 
                    oSaleInvoiceDetails.Price = item.price;
                    oSaleInvoiceDetails.TaxRateID = SafeConvert.ToNullable<int>(((string)item.taxRateID).Replace("{", "").Replace("}", ""));
                    oSaleInvoiceDetails.AccountID = SafeConvert.ToNullable<int>(((string)item.accountID).Replace("{", "").Replace("}", ""));
                    oSaleInvoiceDetails.Discount = SafeConvert.ToNullable<double>(((string)item.discount).Replace("{", "").Replace("}", ""));
                    oSaleInvoiceDetails.TotalMoney = SafeConvert.ToNullable<double>(((string)item.totalMoney).Replace("{", "").Replace("}", ""));
                    oSaleInvoiceDetails.Description = item.description;
                    oSaleInvoice.SaleInvoiceDetails.Add(oSaleInvoiceDetails);
                    //db.SaleInvoiceDetails.Add(oSaleInvoiceDetails);
                }

                //db.SaveChanges();
                manager.Save();
                if (!saleInvoiceID.HasValue)
                    Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.SaleOrder), Constant.CODE_LENGTH, "");

                //SO is approved by manager   (200- Sale : Credit ; Account Receivable (610) : Debit )
                if (oSaleInvoice.Status == 2)
                {
                    Utility.CreateAccountTransaction(0, oSaleInvoice.ID, null, null);                    
                }
                result = oSaleInvoice.ID.ToString();
            }
            catch (Exception ex)
            {
                result = "ERROR: " + ex.Message;
            }
            return serializer.Serialize(result);
        }



    }
}
