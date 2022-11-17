using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class InvoiceSubItem
    {
        public int itemId { get; set; }
        public int quantity { get; set; }
        public double? priceItem { get; set; }
        public string uom { get; set; }
        public double? subTotal { get; set; }
        public int invoiceId { get; set; }
        public string category { get; set; }
    }
}