using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.BusinessLogic.Managers;


namespace TMT.ERP.Services
{
    /// <summary>
    /// Summary description for GoodService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class GoodService : System.Web.Services.WebService
    {
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetGoodByID(int goodID)
        {
            object result = "Success";
            //var goodManage = new GoodManage();
            var manager = new GenericManager<Good>();
            try
            {
                //var good = goodManage.GetById(goodID);
                var good = manager.FindById(goodID);
                if (good != null)
                {
                    result = new
                    {
                        Decription = good.Description,
                        UnitPrice = good.OutputPrice,
                        Disc = "",
                        Accounts = good.SaleAccountID,
                        TaxRate = good.SaleTaxRateID,
                        TaxRateVal = good.TaxRate.Rate,
                        umoCode = good.UOM.Code,
                        umoID = good.UOM.ID
                    };
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetGoodByIDForPurchase(int goodID)
        {
            object result = "Success";
            //var goodManage = new GoodManage();
            var manager = new GenericManager<Good>();
            try
            {
                //var good = goodManage.GetById(goodID);
                var good = manager.FindById(goodID);
                double? rate = 0;
                if(good.TaxRate1 != null){
                    rate = good.TaxRate1.Rate;
                }
                if (good != null)
                {
                    result = new
                    {
                        Decription = good.Description,
                        UnitPrice = good.InputPrice,
                        Disc = "",
                        Accounts = good.PurchaseAccountID,
                        TaxRate = good.PurchaseTaxRateID,
                        TaxRateVal = rate,
                        umoCode = good.UOM.Code,
                        umoID = good.UOM.ID
                    };
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

                [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetGoodByStockID(int goodID, int stockID)
        {
            object result = "Success";
            //var goodManage = new GoodManage();
            var manager = new GenericManager<Good>();
            var managerUMO = new GenericManager<UOM>();
             ErpEntities db = new ErpEntities();
            try
            {
                //var good = goodManage.GetById(goodID);
                var good = manager.FindById(goodID);
                var umo = managerUMO.FindById(good.UOMID);
                var quantityInStock = db.ActualGoodInStocks.Where(ac => ac.StockID == stockID && ac.GoodID == goodID).Sum(ac => ac.RemainQuantity).Value;
                if (good != null)
                {
                    result = new
                    {
                        Decription = good.Description,
                        UnitPrice = good.OutputPrice,
                        Disc = "",
                        Accounts = good.SaleAccountID,
                        TaxRate = good.SaleTaxRateID,
                        umoCode = umo.Code,
                        umoID = umo.ID,
                        quantity = quantityInStock
                    };
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetByParentID(int parentID)
        {
            var manager = new GenericManager<SubGood>();
            var goodItems = manager.Get(filter: x => x.GoodParentID == parentID && x.Good1.ProductTypeID < 2);
            if (goodItems != null)
            {
                var result = serializer.Serialize(goodItems.Select(x => new { ID = x.Good1.ID, Code = x.Good1.Code, Description = x.Good1.Description }).OrderBy(x => x.ID));
                return result;
            }
            else
            {
                return serializer.Serialize("No items");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetParentGoodsByBomID(int bomID)
        {
            var bomItem = new GenericManager<BOM>().FindById(bomID);
            if (bomItem != null)
            {
                var result = new
                {
                    GoodID = bomItem.GoodID,
                    Code = bomItem.Good.Code,
                    Decription = bomItem.Good.Description
                };
                return serializer.Serialize(result);
            }
            else
            {
                return serializer.Serialize("No items");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetGoodsByBomID(int bomID, int? parentGoodID)
        {
            var manager = new GenericManager<BomDetail>();
            IList<BomDetail> goodItems = null;
            if (parentGoodID.GetValueOrDefault()>0)
                goodItems = manager.Get(filter: x => x.BomID == bomID && x.ParentGoodID == parentGoodID);
            else
                goodItems = manager.Get(filter: x => x.BomID == bomID);
            if (goodItems != null)
            {
                var result = serializer.Serialize(goodItems.Select(x => new { ID = x.Good.ID, Code = x.Good.Code, Description = x.Good.Description, BomDetailID = x.ID, Quantity = x.Quantity, ParentGoodID = x.BOM.GoodID, ParentGoodName=x.BOM.Good.Code }).OrderBy(x => x.ID));
                return result;
            }
            else
            {
                return serializer.Serialize("No items");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetWorkOrderByGoodID(int parentID)
        {
            var manager = new GenericManager<WorkOrderDetail>();
            var workOrderItems = manager.Get(filter: x => x.WorkOrderID == parentID);
            if (workOrderItems != null)
            {
                var result = serializer.Serialize(workOrderItems.Select(x => new { ID = x.ID, Code = x.Good.Code, Description = x.Good.Description, Quantity = x.OrderQuantity }).OrderBy(x => x.ID));
                return result;
            }
            else
            {
                return serializer.Serialize("No items");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetTaxByID(int taxID)
        {
            object result = "Success";
            try
            {
                ErpEntities db = new ErpEntities();
                var taxItem = db.TaxRates.Find(taxID);
                if (taxItem != null)
                {
                    result = new
                    {
                        Rate = taxItem.Rate
                    };
                }
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetQuantityByGoodID(int goodID)
        {
            object result = "0";
            var manager = new GenericManager<ActualGoodInStock>();
            try
            {
                result = manager.Get().Where(x => x.ID == goodID).AsQueryable().Sum(x => x.RemainQuantity).GetValueOrDefault();
            }
            catch (Exception ex) { result = ex.Message; }
            return serializer.Serialize(result);
        }

    }
}
