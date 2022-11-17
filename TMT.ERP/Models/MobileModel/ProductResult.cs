using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class ProductResult
    {
        public string code { get; set; }
        public string result{get ; set; }        
        public IEnumerable<SaleProduct> data { get ;set; }
        public string message { get; set; }
    }
}