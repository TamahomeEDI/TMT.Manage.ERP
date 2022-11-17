using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class MobileWSController : ApiController
    {        
        ErpEntities db = new ErpEntities();
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        private string sessionTimeout {get { return "Session timeout";}}
        [System.Web.Http.HttpGet]
        public HttpResponseMessage login(String deviceId, String userName, String passWord)
        {
            //   string  returnJson = JsonConvert.SerializeObject("tesssssssssssssst login");
            //    HttpResponseMessage returnResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
            //    returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            
            //return returnResponse;


            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            LoginResult loginResult = new LoginResult();
            try
            {
                var pass = Password.Encode(passWord);
                var manager = new GenericManager<User>();
                var userInfo = manager.Find(x => x.UserName == userName);
                if (userInfo != null)
                {
                    if (userInfo.Password == pass)
                    {
                        if (userInfo.Active == true)
                        {
                            loginResult.code = "0";
                            loginResult.result = "sucessfull";
                            loginResult.message = "success";
                            userInfo.LastLoginDate = DateTime.Now;
                            manager.Update(userInfo);
                            manager.Save();
                            var session = HttpContext.Current.Session;
                            if (session != null)
                            {
                                session["DeviceID"] = deviceId;
                                if (session["SessionID"] == null)
                                    session["SessionID"] = System.Guid.NewGuid().ToString();
                                loginResult.sessionId = session["SessionID"].ToString();
                            }
                        }
                        else
                        {
                            loginResult.code = "101";
                            loginResult.result = "failed";
                            loginResult.message = "Your account has been locked. Please contact the system administrator.";
                        }
                    }
                    else
                    {
                        loginResult.code = "101";
                        loginResult.result = "failed";
                        loginResult.message = "Password is invalid.";
                    }
                }
                else
                {
                    loginResult.code = "101";
                    loginResult.result = "failed";
                    loginResult.message = "Unable to log in. Please check your login and password.";
                }
                returnJson = JsonConvert.SerializeObject(loginResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            catch (Exception e)
            {
                loginResult.code = "101";
                loginResult.result = "failed";
                loginResult.message = e.Message;
                returnJson = JsonConvert.SerializeObject(loginResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }
        
        [System.Web.Http.HttpGet]
        public HttpResponseMessage logout(String sessionId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            LoginResult loginResult = new LoginResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    //if (session.ToString() == sessionId)
                    //{
                        HttpContext.Current.Session["SessionID"] = null;
                //        loginResult.code = "0";
                //        loginResult.result = "sucessfull";
                //    }
                //    else
                //    {
                //        loginResult.code = "101";
                //        loginResult.result = "failed";
                //        loginResult.message = "Session is invalid.";
                //        loginResult.sessionId = HttpContext.Current.Session["SessionID"].ToString();
                //    }
                }
                loginResult.code = "0";
                loginResult.result = "sucessfull";
            }
            catch (Exception e)
            {
                loginResult.code = "101";
                loginResult.result = "failed";
                loginResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(loginResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Total Sale
        /// </summary>
        /// <param name="sessionId">SessionID of login user</param>
        /// <param name="companyId">Company</param>
        /// <param name="fromDate">Total Sale From date</param>
        /// <param name="toDate">Total Sale To date</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getTotalSale(String sessionId, int? companyId, DateTime fromDate, DateTime toDate)        
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            //TotalSaleResult result = new TotalSaleResult();
            //try
            //{
            //    //var session = AppContext.SessionVariables.SessionID;
            //    //if (HttpContext.Current.Session["SessionID"] != null)
            //    //{
            //    //    if (HttpContext.Current.Session["SessionID"].ToString() == sessionId)
            //    //    {
            //            db.Configuration.ProxyCreationEnabled = false;
            //            List<TotalSaleObj> lsTotal = new List<TotalSaleObj>();
            //            DateTime dtTemp = fromDate;
            //            var saleInvoices = db.SaleInvoices.Where(sa => sa.CompanyID == companyId).ToList();
            //            while (dtTemp <= toDate)
            //            {
            //                var total = saleInvoices.Where(x => x.DueDate == dtTemp).Sum(x => x.TotalMoney);
            //                if (total == null)
            //                    total = 0;

            //                TotalSaleObj obj = new TotalSaleObj();
            //                obj.id = 1;//item.ID;
            //                obj.date = dtTemp;
            //                obj.amount = total;
            //                lsTotal.Add(obj);
            //                dtTemp = dtTemp.AddDays(1);
            //            }
                        
            //            string minValue = lsTotal.Min(sa => sa.amount).ToString();
            //            string maxValue = lsTotal.Max(sa => sa.amount).ToString();
            //            result.code = "0";
            //            result.result = "successful";
            //            result.minAmount = minValue;
            //            result.maxAmount = maxValue;
            //            result.data = lsTotal;
            //    //    }
            //    //    else
            //    //    {
            //    //        result.code = "101";
            //    //        result.result = "failed";
            //    //        result.message = "Invalid SessionId";
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    result.code = "102";
            //    //    result.result = "failed";
            //    //    result.message = this.sessionTimeout;
            //    //}
            //}
            //catch (Exception ex)
            //{
            //    result.code = "101";
            //    result.result = "failed";
            //    result.message = ex.Message;
            //}
            ////
            //returnJson = JsonConvert.SerializeObject(result);
            //returnResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
            //returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            //return returnResponse;

            TotalSaleResult totalSaleResult = new TotalSaleResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        List<TotalSaleObj> lsTotal = new List<TotalSaleObj>();
                        DateTime dtTemp = fromDate;
                        var saleInvoices = db.SaleInvoices.Where(sa => sa.CompanyID == companyId).ToList();
                        while (dtTemp <= toDate)
                        {
                            var total = saleInvoices.Where(x => x.DueDate == dtTemp).Sum(x => x.TotalMoney);
                            if (total == null)
                                total = 0;

                            TotalSaleObj obj = new TotalSaleObj();
                            obj.id = 1;//item.ID;
                            obj.date = dtTemp;
                            obj.amount = total;
                            lsTotal.Add(obj);
                            dtTemp = dtTemp.AddDays(1);
                        }

                        string minValue = lsTotal.Min(sa => sa.amount).ToString();
                        string maxValue = lsTotal.Max(sa => sa.amount).ToString();
                        totalSaleResult.code = "0";
                        totalSaleResult.result = "successful";
                        totalSaleResult.minAmount = minValue;
                        totalSaleResult.maxAmount = maxValue;
                        totalSaleResult.data = lsTotal;
                    }
                    else
                    {
                        totalSaleResult.code = "101";
                        totalSaleResult.result = "failed";
                        totalSaleResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    totalSaleResult.code = "102";
                    totalSaleResult.result = "failed";
                    totalSaleResult.message = this.sessionTimeout;                
                }
            }
            catch (Exception e)
            {
                totalSaleResult.code = "101";
                totalSaleResult.result = "failed";
                totalSaleResult.message = e.Message;
            }
            finally {
                returnJson = JsonConvert.SerializeObject(totalSaleResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");            
            }
            return returnResponse;
        }

        /// <summary>
        /// Get company
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="fromDate">From date Sale of company</param>
        /// <param name="toDate">To date Sale of company</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getCompany(String sessionId, DateTime? fromDate, DateTime? toDate)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            CompanyResult companyResult = new CompanyResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        var saleInvoiceLst = db.SaleInvoices.Where(sa => sa.DueDate >= fromDate && sa.DueDate <= toDate).ToList();
                        var group = saleInvoiceLst.GroupBy(sa => sa.CompanyID).ToList();

                        /* var companysQuery = from d in db.SaleInvoices
                                             where d.DueDate >= fromDate && d.DueDate <= toDate
                                             group d by d.CompanyID into grouped
                                             select new { amount = grouped.Sum(b => b.TotalMoney), comId = grouped.Key, name = select new{ name = select db.Companies.Where(co => co.ID == grouped.Key).} */
                        // select new { name = grouped.Key, count = grouped.Count() }).AsEnumerable();
                        //var sells = (from d in db.SaleInvoices join c in db.Companies on d.CompanyID equals c.ID  into cm1
                        //              from cm2 in cm1.DefaultIfEmpty()
                        //               group cm2 by d.CompanyID into ComGroup
                        //               select new { amount = ComGroup.Sum  
                        // .Select(a => new { amount = a.Sum(b => b.TotalMoney), comId = a.Key, name = c.DisplayName}))
                        // .OrderByDescending(a => a.comId).ToList()
                        //  select d;

                        List<CompanyObj> lsCom = new List<CompanyObj>();
                        foreach (var item in group)
                        {
                            CompanyObj obj = new CompanyObj();
                            obj.comId = item.Key;
                            obj.name = db.Companies.Where(i => i.ID == item.Key)
                                                  .Select(i => i.DisplayName).SingleOrDefault();
                            obj.amount = item.Sum(ie => ie.TotalMoney);
                            lsCom.Add(obj);
                        }
                        companyResult.code = "0";
                        companyResult.result = "successful";
                        companyResult.data = lsCom;
                    }
                    else
                    {
                        companyResult.code = "101";
                        companyResult.result = "failed";
                        companyResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    companyResult.code = "102";
                    companyResult.result = "failed";
                    companyResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                companyResult.code = "101";
                companyResult.result = "failed";
                companyResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(companyResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Products
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="fromDate">Sale From date </param>
        /// <param name="toDate">Sale To date </param>
        /// <param name="companyId">Company of Product. if value is empty , return all products </param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getProducts(String sessionId, DateTime? fromDate, DateTime? toDate, int? companyId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            ProductResult productResult = new ProductResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        var products = new GenericManager<Good>().Get().Select(x =>
                        {
                            IEnumerable<SaleInvoiceDetail> saleInvoiceDetails;
                            if (companyId.HasValue)
                                saleInvoiceDetails = x.SaleInvoiceDetails.Where(y => y.SaleInvoice.DueDate >= fromDate && y.SaleInvoice.DueDate <= toDate && y.SaleInvoice.CompanyID == companyId);
                            else
                                saleInvoiceDetails = x.SaleInvoiceDetails.Where(y => y.SaleInvoice.DueDate >= fromDate && y.SaleInvoice.DueDate <= toDate);

                            return new
                            {
                                x.ID,
                                x.Name,
                                Quantity = saleInvoiceDetails.Sum(y => y.Quantity),
                                Total = saleInvoiceDetails.Sum(y => y.TotalMoney)
                            };
                        });

                        List<SaleProduct> lsProduct = new List<SaleProduct>();
                        foreach (var item in products)
                        {
                            SaleProduct obj = new SaleProduct();
                            obj.productId = item.ID;
                            obj.productName = item.Name;
                            obj.company = "";
                            obj.qtySold = item.Quantity;
                            obj.total = item.Total;
                            lsProduct.Add(obj);
                        }
                        productResult.code = "0";
                        productResult.result = "successful";
                        productResult.data = lsProduct;
                    }
                    else
                    {
                        productResult.code = "101";
                        productResult.result = "failed";
                        productResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    productResult.code = "102";
                    productResult.result = "failed";
                    productResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                productResult.code = "101";
                productResult.result = "failed";
                productResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(productResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Persons
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="fromDate">Sale From date </param>
        /// <param name="toDate">Sale To date </param>
        /// <param name="companyId">Company of Persons . if value is empty , return all persons</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getPersons(String sessionId, DateTime? fromDate, DateTime? toDate, int? companyId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            PersonResult personResult = new PersonResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        List<Person> lsPersons = new List<Person>();
                        //var persons = GetUser(companyId, fromDate, toDate);
                        var persons = new GenericManager<User>().Get().Select(x =>
                        {
                            IEnumerable<SaleInvoice> saleInvoices;
                            if (companyId.HasValue)
                                saleInvoices = x.SaleInvoices.Where(y => y.DueDate >= fromDate && y.DueDate <= toDate && y.CompanyID == companyId);
                            else
                                saleInvoices = x.SaleInvoices.Where(y => y.DueDate >= fromDate && y.DueDate <= toDate);

                            var amount = (saleInvoices != null) ? saleInvoices.Sum(y => y.TotalMoney) : 0;
                            var company = "";//(saleInvoices != null) ? saleInvoices.FirstOrDefault().Company.DisplayName : "";
                            try
                            {
                                company = saleInvoices.FirstOrDefault().Company.DisplayName;
                            }
                            catch { }

                            return new
                            {
                                x.ID,
                                x.UserName,
                                Amount = amount,
                                Company = company,
                            };
                        });

                        foreach (var item in persons)
                        {
                            Person obj = new Person();
                            obj.personId = item.ID;
                            obj.personName = item.UserName;
                            obj.amount = item.Amount;
                            obj.company = item.Company;
                            lsPersons.Add(obj);
                        }

                        personResult.code = "0";
                        personResult.result = "successful";
                        personResult.data = lsPersons;
                    }
                    else
                    {
                        personResult.code = "101";
                        personResult.result = "failed";
                        personResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    personResult.code = "102";
                    personResult.result = "failed";
                    personResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                personResult.code = "101";
                personResult.result = "failed";
                personResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(personResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        private IEnumerable<User> GetUser(int? companyId, DateTime? fromDate, DateTime? toDate)
        {
            //List<Person> lsPersons = new List<Person>();
            var persons = new GenericManager<User>().Get().Select(x =>
            {
                IEnumerable<SaleInvoice> saleInvoices;
                if (companyId.HasValue)
                    saleInvoices = x.SaleInvoices.Where(y => y.DueDate >= fromDate && y.DueDate <= toDate && y.CompanyID == companyId);
                else
                    saleInvoices = x.SaleInvoices.Where(y => y.DueDate >= fromDate && y.DueDate <= toDate);

                return new
                {
                    x.ID,
                    x.UserName,
                    Amount = 0,//(saleInvoices!=null)?saleInvoices.Sum(y => y.TotalMoney):0,
                    Company = (saleInvoices != null) ? saleInvoices.FirstOrDefault().Company.DisplayName : "",
                };
            });
            return (IEnumerable<User>)persons;
        }

        /// <summary>
        /// Get Clients
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="fromDate">Sale From date</param>
        /// <param name="toDate">Sale To date</param>
        /// <param name="companyId">Company of Clients . if value is empty , return all Clients </param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getClients(String sessionId, DateTime? fromDate, DateTime? toDate, int? companyId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            ClientResult clientResult = new ClientResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        List<SaleInvoice> saleInvoiceLst;
                        if (companyId.HasValue)
                            saleInvoiceLst = db.SaleInvoices.Where(sa => sa.Status == 2 && sa.DueDate >= fromDate && sa.DueDate <= toDate && sa.CompanyID == companyId).ToList();
                        else
                            saleInvoiceLst = db.SaleInvoices.Where(sa => sa.Status == 2 && sa.DueDate >= fromDate && sa.DueDate <= toDate).ToList();

                        List<SaleInvoice> lstSale = new List<SaleInvoice>();
                        foreach (var invoice in saleInvoiceLst)
                        {
                            var contact = db.Contacts.Where(co => co.ID == invoice.ContactID && (co.Type == 1 || co.Type == 2)).FirstOrDefault();
                            if (contact != null)
                            {
                                lstSale.Add(invoice);
                            }
                        }
                        // saleInvoiceLst.Where(s => saleInvoiceLst.Contains in db.Contacts.Where(c => c.Type == 1 || c.Type == 2));                                                                                                         
                        List<Client> lsClients = new List<Client>();
                        foreach (var item in lstSale)
                        {
                            Client obj = new Client();
                            obj.clientId = item.ContactID ?? 0;
                            obj.clientName = db.Contacts.Where(i => i.ID == item.ContactID)
                                                  .Select(i => i.ContactName).SingleOrDefault();
                            obj.date = item.DueDate ?? DateTime.Now;
                            obj.amountPurchase = item.TotalMoney ?? 0;
                            if (item.Company != null)
                                obj.company = item.Company.DisplayName;
                            else
                                obj.company = "";
                            lsClients.Add(obj);
                        }
                        clientResult.code = "0";
                        clientResult.result = "successful";
                        clientResult.data = lsClients;
                    }
                    else
                    {
                        clientResult.code = "101";
                        clientResult.result = "failed";
                        clientResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    clientResult.code = "102";
                    clientResult.result = "failed";
                    clientResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                clientResult.code = "101";
                clientResult.result = "failed";
                clientResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(clientResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get List Invoices
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="fromDate">Sale From date</param>
        /// <param name="toDate">Sale To date</param>
        /// <param name="companyId">Company of Invoices . if value is empty , return all Invoices</param>
        /// <param name="category">Category report : SO(sale) and PO(purchase). If value is empty , return all invoices</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getListInvoice(String sessionId, DateTime? fromDate, DateTime? toDate, int? companyId, String category)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            InvoiceObjResult invoiceResult = new InvoiceObjResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        if (category.Equals("SO"))
                        {
                            var saleInvoiceLst = db.SaleInvoices.Where(sa => sa.Status == 2 && sa.DueDate >= fromDate && sa.DueDate <= toDate && sa.CompanyID == companyId);
                            setDataFromSale(saleInvoiceLst, invoiceResult);
                        }
                        else if (category.Equals("PO"))
                        {
                            var salePurchaseLst = db.Purchases.Where(sa => sa.Status == 2 && sa.DueDate >= fromDate && sa.DueDate <= toDate && sa.CompanyID == companyId);
                            setDataFromPurchase(salePurchaseLst, invoiceResult);
                        }
                        returnJson = JsonConvert.SerializeObject(invoiceResult);
                        returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                        returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
                    }
                    else
                    {
                        invoiceResult.code = "101";
                        invoiceResult.result = "failed";
                        invoiceResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    invoiceResult.code = "102";
                    invoiceResult.result = "failed";
                    invoiceResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                invoiceResult.code = "101";
                invoiceResult.result = "failed";
                invoiceResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(invoiceResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Detail Invoices
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="invoiceId">Invoice Code</param>
        /// <param name="category">Category report : SO (sale), PO(purchase)</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getDetailInvoice(String sessionId, int invoiceId, string category)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            InvoiceItemResult invoiceItemResult = new InvoiceItemResult();
            List<InvoiceSubItem> lstInvoiceSubItem = new List<InvoiceSubItem>();
            double? totalTaxRate = 0;
            double? totalDisCount = 0;
            double? total = 0;
            try
            {
                //var session = HttpContext.Current.Session["SessionID"];
                //if (session != null)
                //{
                //    if (session.ToString() == sessionId)
                //    {
                        switch (category)
                        {
                            case "SO":
                                {
                                    var saleItem = new GenericManager<SaleInvoice>().FindById(invoiceId);
                                    foreach (var item in saleItem.SaleInvoiceDetails)
                                    {
                                        var invoiceSubItem = new InvoiceSubItem();
                                        invoiceSubItem.itemId = item.ID;
                                        invoiceSubItem.quantity = item.Quantity;
                                        invoiceSubItem.priceItem = item.Price;
                                        invoiceSubItem.uom = item.Good.UOM.Name;
                                        invoiceSubItem.subTotal = item.Subtotal;
                                        invoiceSubItem.invoiceId = item.SaleInvoiceID;
                                        invoiceSubItem.category = "SO";
                                        lstInvoiceSubItem.Add(invoiceSubItem);
                                        if (item.TaxRate != null)
                                            totalDisCount += (item.Quantity * item.Price * item.TaxRate.Rate) / 100;
                                    }
                                    totalTaxRate = saleItem.TotalTax;
                                    total = saleItem.TotalMoney;
                                    break;
                                }
                            case "PO":
                                {
                                    var purchaseItem = new GenericManager<Purchase>().FindById(invoiceId);
                                    foreach (var item in purchaseItem.PurchaseDetails)
                                    {
                                        var invoiceSubItem = new InvoiceSubItem();
                                        invoiceSubItem.itemId = item.ID;
                                        invoiceSubItem.quantity = item.Quantity;
                                        invoiceSubItem.priceItem = item.Price;
                                        invoiceSubItem.uom = item.Good.UOM.Name;
                                        invoiceSubItem.subTotal = item.TotalMoney;
                                        invoiceSubItem.invoiceId = item.PurchaseID;
                                        invoiceSubItem.category = "PO";
                                        lstInvoiceSubItem.Add(invoiceSubItem);
                                        if (item.TaxRate != null)
                                            totalDisCount += (item.Quantity * item.Price * item.TaxRate.Rate) / 100;
                                    }
                                    totalTaxRate = purchaseItem.Tax;
                                    total = purchaseItem.TotalMoney;
                                    break;
                                }

                            default: break;
                        }

                        invoiceItemResult.code = "0";
                        invoiceItemResult.result = "successful";
                        invoiceItemResult.data = lstInvoiceSubItem;
                        invoiceItemResult.taxRate = totalTaxRate;
                        invoiceItemResult.disCount = totalDisCount;
                        invoiceItemResult.total = total;
                //    }
                //    else
                //    {
                //        invoiceItemResult.code = "101";
                //        invoiceItemResult.result = "failed";
                //        invoiceItemResult.message = "Invalid SessionId";
                //    }
                //}
                //else
                //{
                //    invoiceItemResult.code = "102";
                //    invoiceItemResult.result = "failed";
                //    invoiceItemResult.message = this.sessionTimeout;
                //}
            }
            catch (Exception ex)
            {
                invoiceItemResult.code = "101";
                invoiceItemResult.result = "failed";
                invoiceItemResult.message = ex.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(invoiceItemResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Detail Item
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="invoiceId">Item Code</param>
        /// <param name="itemId"></param>
        /// <param name="category">Category report : SO , PO</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getDetailItem(String sessionId, int invoiceId, int itemId, string category)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            var itemDetailResult = new ItemDetail();
            try
            {
                //var session = HttpContext.Current.Session["SessionID"];
                //if (session != null)
                //{
                //    if (session.ToString() == sessionId)
                //    {
                        switch (category)
                        {
                            case "SO":
                                {
                                    var item = new GenericManager<SaleInvoiceDetail>().FindById(itemId);
                                    if (item != null)
                                    {
                                        itemDetailResult.itemId = itemId;
                                        itemDetailResult.itemName = item.Good.Name;
                                        itemDetailResult.description = item.Good.Description;
                                        itemDetailResult.imageUrl = item.Good.Attachment;
                                        itemDetailResult.quantity = item.Quantity;
                                        itemDetailResult.priceItem = item.Price;
                                        itemDetailResult.uom = item.Good.UOM.Name;
                                        itemDetailResult.disCount = item.Discount;
                                        if (item.TaxRate != null)
                                            itemDetailResult.taxRate = item.TaxRate.Rate;
                                        else
                                            itemDetailResult.taxRate = 0;
                                        itemDetailResult.subTotal = item.Subtotal;
                                        itemDetailResult.company = item.SaleInvoice.Company.DisplayName;
                                        itemDetailResult.invoiceId = invoiceId;
                                        itemDetailResult.category = "SO";
                                    }
                                    break;
                                }
                            case "PO":
                                {
                                    var item = new GenericManager<PurchaseDetail>().FindById(itemId);
                                    if (item != null)
                                    {
                                        itemDetailResult.itemId = itemId;
                                        itemDetailResult.itemName = item.Good.Name;
                                        itemDetailResult.description = item.Good.Description;
                                        itemDetailResult.imageUrl = item.Good.Attachment;
                                        itemDetailResult.quantity = item.Quantity;
                                        itemDetailResult.priceItem = item.Price;
                                        itemDetailResult.uom = item.Good.UOM.Name;
                                        itemDetailResult.disCount = item.Discount;
                                        if (item.TaxRate != null)
                                            itemDetailResult.taxRate = item.TaxRate.Rate;
                                        else
                                            itemDetailResult.taxRate = 0;
                                        itemDetailResult.subTotal = item.TotalMoney;
                                        itemDetailResult.company = item.Purchase.Company.DisplayName;
                                        itemDetailResult.invoiceId = invoiceId;
                                        itemDetailResult.category = "PO";
                                    }
                                    break;
                                }
                            default: break;
                        }

                        itemDetailResult.code = "0";
                        itemDetailResult.result = "successful";
                //    }
                //    else
                //    {
                //        itemDetailResult.code = "101";
                //        itemDetailResult.result = "failed";
                //        itemDetailResult.message = "Invalid SessionId";
                //    }
                //}
                //else
                //{
                //    itemDetailResult.code = "102";
                //    itemDetailResult.result = "failed";
                //    itemDetailResult.message = this.sessionTimeout;
                //}
            }
            catch (Exception ex)
            {
                itemDetailResult.code = "101";
                itemDetailResult.result = "failed";
                itemDetailResult.message = ex.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(itemDetailResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.InternalServerError);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Inventories  
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="companyId">Company ID of inventories . if companyId is empty , return all inventories</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getInventories(String sessionId, int? companyId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            InventoryResult inventoryResult = new InventoryResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        List<Inventory> lsInventorys = new List<Inventory>();
                        IEnumerable<ActualGoodInStock> goods = new GenericManager<ActualGoodInStock>().Get();
                        if (companyId.HasValue)
                            goods = goods.Where(x => x.CompanyID == companyId);

                        foreach (var item in goods)
                        {
                            Inventory obj = new Inventory();
                            obj.InventoryCode = item.Good.Code;
                            obj.description = item.Good.Description;
                            obj.quantity = item.RemainQuantity;
                            obj.price = item.Good.InputPrice;
                            obj.type = item.Good.ProductType.Name;
                            if (item.Company != null)
                            {
                                obj.companyId = item.CompanyID;
                                obj.companyName = item.Company.DisplayName;
                            }
                            else
                            {
                                obj.companyId = 0;
                                obj.companyName = "";
                            }
                            lsInventorys.Add(obj);
                        }

                        inventoryResult.code = "0";
                        inventoryResult.result = "successful";
                        inventoryResult.data = lsInventorys;
                    }
                    else
                    {
                        inventoryResult.code = "101";
                        inventoryResult.result = "failed";
                        inventoryResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    inventoryResult.code = "102";
                    inventoryResult.result = "failed";
                    inventoryResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                inventoryResult.code = "101";
                inventoryResult.result = "failed";
                inventoryResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(inventoryResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        /// <summary>
        /// Get Unpaid Amount Of Companies
        /// </summary>
        /// <param name="sessionId">Session ID when login system</param>
        /// <param name="companyId">Company ID of inventories . if companyId is empty , return all inventories</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage getUnpaidAmountOfCompanies(String sessionId, int? companyId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            ClientResult clientResult = new ClientResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        IEnumerable<SaleInvoice> saleInvoices;
                        //= new GenericManager<SaleInvoice>().Get().Where(y => y.DueDate < DateTime.Now.Date && y.RemainMoney > 0 && y.Status>1);
                        if (companyId.HasValue)
                            saleInvoices = new GenericManager<SaleInvoice>().Get().Where(y => y.CompanyID == companyId && y.DueDate < DateTime.Now.Date && y.RemainMoney > 0 && y.Status > 1);
                        else
                            saleInvoices = new GenericManager<SaleInvoice>().Get().Where(y => y.DueDate < DateTime.Now.Date && y.RemainMoney > 0 && y.Status > 1);
                        var customers = saleInvoices.GroupBy(x => x.ContactID).Select(x => new
                        {
                            customerID = x.First().ContactID,
                            customerName = x.First().Contact.FirstName + ' ' + x.First().Contact.LastName,
                            amountUnpaid = x.Sum(y => y.RemainMoney),
                            companyId = x.First().CompanyID,
                            companyName = x.First().Company.DisplayName,
                        });


                        List<Client> lsClients = new List<Client>();
                        foreach (var item in customers)
                        {
                            Client obj = new Client();
                            obj.clientId = item.customerID;
                            obj.clientName = item.customerName;
                            obj.amountUnpaid = item.amountUnpaid;
                            obj.companyId = item.companyId;
                            obj.company = item.companyName;

                            lsClients.Add(obj);
                        }
                        clientResult.code = "0";
                        clientResult.result = "successful";
                        clientResult.data = lsClients;
                    }
                    else
                    {
                        clientResult.code = "101";
                        clientResult.result = "failed";
                        clientResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    clientResult.code = "102";
                    clientResult.result = "failed";
                    clientResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                clientResult.code = "101";
                clientResult.result = "failed";
                clientResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(clientResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }


        [System.Web.Http.HttpGet]
        public HttpResponseMessage getProductTypes(String sessionId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            ProductTypeResult productTypeResult = new ProductTypeResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        var productTypes = new GenericManager<TMT.ERP.DataAccess.Model.ProductType>().Get();
                        List<TMT.ERP.Models.ProductType> lsProductTypes = new List<Models.ProductType>();
                        foreach (var item in productTypes)
                        {
                            TMT.ERP.Models.ProductType obj = new Models.ProductType();
                            obj.productTypeId = item.ID;
                            obj.productTypeName = item.Name;
                            lsProductTypes.Add(obj);
                        }
                        productTypeResult.code = "0";
                        productTypeResult.result = "successful";
                        productTypeResult.total = productTypes.Count;
                        productTypeResult.data = lsProductTypes;
                    }
                    else
                    {
                        productTypeResult.code = "101";
                        productTypeResult.result = "failed";
                        productTypeResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    productTypeResult.code = "102";
                    productTypeResult.result = "failed";
                    productTypeResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                productTypeResult.code = "101";
                productTypeResult.result = "failed";
                productTypeResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(productTypeResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage getAllContacts(String sessionId)
        {
            string returnJson = "";
            HttpResponseMessage returnResponse = null;
            ClientResult clientResult = new ClientResult();
            try
            {
                var session = HttpContext.Current.Session["SessionID"];
                if (session != null)
                {
                    if (session.ToString() == sessionId)
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        var clients = db.Contacts.ToList();
                        List<Client> lsClients = new List<Client>();
                        foreach (var item in clients)
                        {
                            var clientObj = new Client();
                            clientObj.clientId = item.ID;
                            clientObj.clientName = item.ContactName;
                            lsClients.Add(clientObj);
                        }

                        clientResult.code = "0";
                        clientResult.message = "Ok";
                        clientResult.data = lsClients;
                    }
                    else
                    {
                        clientResult.code = "101";
                        clientResult.result = "failed";
                        clientResult.message = "Invalid SessionId";
                    }
                }
                else
                {
                    clientResult.code = "102";
                    clientResult.result = "failed";
                    clientResult.message = this.sessionTimeout;
                }
            }
            catch (Exception e)
            {
                clientResult.code = "101";
                clientResult.result = "failed";
                clientResult.message = e.Message;
            }
            finally
            {
                returnJson = JsonConvert.SerializeObject(clientResult);
                returnResponse = this.Request.CreateResponse(HttpStatusCode.OK);
                returnResponse.Content = new StringContent(returnJson, Encoding.UTF8, "application/json");
            }
            return returnResponse;
        }



        #region setData
        private void setDataFromSaleAndPurchase(IQueryable<SaleInvoice> saleInvoiceLst, IQueryable<Purchase> salePurchaseLst, InvoiceObjResult invoiceResult)
        {
            List<InvoiceObj> lsInvoiceObjs = new List<InvoiceObj>();
            foreach (var item in saleInvoiceLst)
            {
                InvoiceObj obj = new InvoiceObj();
                obj.invoiceId = item.ID;
                obj.customer = db.Contacts.Where(i => i.ID == item.ContactID)
                                                  .Select(i => i.ContactName).SingleOrDefault();
                obj.dueDate = item.DueDate ?? DateTime.Now;
                var paymentList = db.Payments.Where(p => p.SaleInvoiceID == item.ID).ToList();
                double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
                obj.paid = totalPayment;
                obj.amountDue = item.TotalMoney ?? 0;
                obj.category = "Sale";
                lsInvoiceObjs.Add(obj);
            }
            foreach (var item in salePurchaseLst)
            {
                InvoiceObj obj = new InvoiceObj();
                obj.invoiceId = item.ID;
                obj.customer = db.Contacts.Where(i => i.ID == item.ContactID)
                                                  .Select(i => i.ContactName).SingleOrDefault();
                obj.dueDate = item.DueDate ?? DateTime.Now;
                var paymentList = db.Payments.Where(p => p.PurchaseID == item.ID).ToList();
                double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
                obj.paid = totalPayment;
                obj.amountDue = item.TotalMoney ?? 0;
                obj.category = "Purchase";
                lsInvoiceObjs.Add(obj);
            }
            invoiceResult.code = "0";
            invoiceResult.result = "successful";
            invoiceResult.data = lsInvoiceObjs;
        }

        private void setDataFromPurchase(IQueryable<Purchase> salePurchaseLst, InvoiceObjResult invoiceResult)
        {
            List<InvoiceObj> lsInvoiceObjs = new List<InvoiceObj>();
            foreach (var item in salePurchaseLst)
            {
                InvoiceObj obj = new InvoiceObj();
                obj.invoiceId = item.ID;
                obj.customer = db.Contacts.Where(i => i.ID == item.ContactID)
                                                  .Select(i => i.ContactName).SingleOrDefault();
                obj.dueDate = item.DueDate ?? DateTime.Now;
                var paymentList = db.Payments.Where(p => p.PurchaseID == item.ID).ToList();
                double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
                obj.paid = totalPayment;
                obj.amountDue = item.TotalMoney ?? 0;
                obj.category = "Purchase";
                if (item.Company != null)
                    obj.company = item.Company.DisplayName;
                else
                    obj.company = "";
                lsInvoiceObjs.Add(obj);
            }
            invoiceResult.code = "0";
            invoiceResult.result = "successful";
            invoiceResult.data = lsInvoiceObjs;
        }

        private void setDataFromSale(IQueryable<SaleInvoice> saleInvoiceLst, InvoiceObjResult invoiceResult)
        {
            List<InvoiceObj> lsInvoiceObjs = new List<InvoiceObj>();
            foreach (var item in saleInvoiceLst)
            {
                InvoiceObj obj = new InvoiceObj();
                obj.invoiceId = item.ID;
                obj.customer = db.Contacts.Where(i => i.ID == item.ContactID)
                                                  .Select(i => i.ContactName).SingleOrDefault();
                obj.dueDate = item.DueDate ?? DateTime.Now;
                var paymentList = db.Payments.Where(p => p.SaleInvoiceID == item.ID).ToList();
                double totalPayment = paymentList.Sum(pay => Convert.ToDouble(pay.TotalMoney));
                obj.paid = totalPayment;
                obj.amountDue = item.TotalMoney ?? 0;
                obj.category = "Sale";
                if (item.Company != null)
                    obj.company = item.Company.DisplayName;
                else
                    obj.company = "";
                lsInvoiceObjs.Add(obj);
            }
            invoiceResult.code = "0";
            invoiceResult.result = "successful";
            invoiceResult.data = lsInvoiceObjs;
        }

        #endregion setData

    }
}
