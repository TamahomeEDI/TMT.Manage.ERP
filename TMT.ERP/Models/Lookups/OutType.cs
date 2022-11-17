using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.Models.Lookups
{
    public enum OutType
    {
        [Description("0")]
        SaleInvoice = 0,
        [Description("1")]
        GoodProcess = 1,
        [Description("2")]
        StockMovement = 2,
     
    }
}