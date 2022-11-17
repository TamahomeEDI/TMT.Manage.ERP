using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class SaleProduct
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string company { get; set; }
        public int qtySold { get; set; }
        public double? total { get; set; }
    }
}