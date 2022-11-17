using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class Inventory
    {
        public string InventoryCode {get;set;}
        public string description {get;set;}
        public int? quantity {get;set;}
        public double? price {get;set;}
        public string type {get;set;}
        public int? companyId {get;set;}
        public string companyName {get;set;}
    }
}  
