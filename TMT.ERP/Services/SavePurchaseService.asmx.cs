using CommonLib;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Data;
using TMT.ERP.Commons;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
         private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SavePurchase(int? purchaseID, int? contactID, string contactName, string createDate, string dueDate,  string invoiceNo, string reference,
                                        int currencyID, int createdEmployeeID, double? tax, double? totalMoney, int status, int type, int?UseVAT, double? SubTotal, int? StockID, string purchaseDetail)
        {
            object result = 0;
            try
            {
                ErpEntities db = new ErpEntities();
                Purchase oPurchase =null;
                if (purchaseID.GetValueOrDefault() > 0)
                {
                    oPurchase = db.Purchases.Find(purchaseID);
                    var oPurchaseDetails = db.PurchaseDetails.Where(p => p.PurchaseID == purchaseID);
                    foreach (var item in oPurchaseDetails)
                    {
                        db.Entry(item).State = System.Data.EntityState.Deleted;
                    }
                }
                else
                {
                    if (invoiceNo != "" && db.Purchases.Where(x => x.Code == invoiceNo).ToList().Count > 0)
                    {

                        result = "ERROR: " + Resources.Resource.Common_CNUnique;
                        if (type != 1)
                        {
                            result = "ERROR: " + Resources.Resource.Common_POUnique;
                        }
                        return serializer.Serialize(result);
                    }
                    oPurchase = new Purchase();                    
                    db.Purchases.Add(oPurchase);
                    oPurchase.RemainMoney = totalMoney;
                    oPurchase.CompanyID = AppContext.RequestVariables.CurrentUser.CompanyID;
                }
                oPurchase.ContactName = contactName;
                oPurchase.ContactID = new ContactService().GetContactByName(contactName, 2);
                oPurchase.CreatedDate =DateTime.Parse(createDate);
                if (dueDate != "" && dueDate != null)
                {
                    oPurchase.DueDate = DateTime.Parse(dueDate);
                }
                oPurchase.Code = invoiceNo;
                //if (status == 4 || status == 2)
                //{
                //    if (invoiceNo == "" || invoiceNo == null)
                //    {
                //        oPurchase.Code = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.PurchaseOrder));
                //    }
                //}
                oPurchase.Reference = reference;             
                oPurchase.CreatedUserID = createdEmployeeID;
                oPurchase.CurrencyID = currencyID;               
                oPurchase.Status = status;
                oPurchase.Tax = tax ?? 0;
                oPurchase.TotalMoney = totalMoney ;
                oPurchase.Type = type;
                oPurchase.UseVAT = UseVAT;
                oPurchase.SubTotal = SubTotal;
                oPurchase.StockID = StockID;
                db.SaveChanges();
                if (!(purchaseID.GetValueOrDefault() > 0))
                {
                     Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.PurchaseOrder), Constant.CODE_LENGTH, "");
                }
                dynamic array = JsonConvert.DeserializeObject(purchaseDetail);

                foreach (var item in array)
                {
                    PurchaseDetail oPurchase_Details = new PurchaseDetail();
                    oPurchase_Details.PurchaseID = oPurchase.ID;
                    //oSaleInvoice_Details.SaleInvoice = oSaleInvoice;
                    oPurchase_Details.GoodID = item.goodId;
                    int UOMID = 0;
                   Good oGood = db.Goods.Find(oPurchase_Details.GoodID);
                    UOMID = oGood.UOMID;
                    oPurchase_Details.UOMID = UOMID;
                    oPurchase_Details.Quantity = item.quantity;
                    oPurchase_Details.Price = item.price;
                    if(item.taxRateID != null)
                        oPurchase_Details.TaxRateID = SafeConvert.ToNullable<int>(((string)item.taxRateID).Replace("{", "").Replace("}", ""));
                    if (item.accountID != null)
                        oPurchase_Details.AccountID = SafeConvert.ToNullable<int>(((string)item.accountID).Replace("{", "").Replace("}", ""));

                    if(item.Discount != null)
                    oPurchase_Details.Discount = SafeConvert.ToNullable<double>(((string)item.discount).Replace("{","").Replace("}",""));

                    if (item.totalMoney != null && item.totalMoney != "")
                        oPurchase_Details.TotalMoney = item.totalMoney;

                    oPurchase_Details.Description = item.description;
                    db.PurchaseDetails.Add(oPurchase_Details);
                }
                db.SaveChanges();               
                result = oPurchase.ID;
                //Create incoming shipping if status = 2 (approve)
                //if (status == 2 && type != 1)
                //{
                //    string[] purID = new string[1]{oPurchase.ID.ToString()};
                //    var currentUserID = Commons.AppContext.RequestVariables.CurrentUser.ID;    
                //    StockService stockSer = new StockService();
                //    stockSer.SavePurchaseStockInAppvored(purID, currentUserID);
                //    //Create transaction
                //    Utility.CreateAccountTransaction(1, oPurchase.ID, null, null);
                //}
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

        private void SaveInContact(ref Contact objCon, string contactName)
        {
            ErpEntities db = new ErpEntities();
            objCon = new Contact();
            objCon.ContactName = contactName;
            db.Contacts.Add(objCon);
            db.SaveChanges();            
        }

    }
    }

