using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class ItemDetail
    {
        public string code { get; set; }
        public string result { get; set; }
        public string message {get; set; }

        public int invoiceId { get; set; }
        public string category { get; set; }
        public int itemId { get; set; }
        public string itemName { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }
        public int quantity { get; set; }
        public double? priceItem { get; set; }
        public string uom { get; set; }
        public double? disCount { get; set; }
        public double? taxRate { get; set; }
        public double? subTotal { get; set; }
        string Company;
        public string company
        {
            get { return Company; }
            set { Company = value; }
        } 

    }
}