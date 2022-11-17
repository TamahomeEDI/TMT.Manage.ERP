using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.Models.Lookups
{
    public enum NodeType
    {
        [Description("0")]
        SaleOrder,
        [Description("1")]
        PurchaseOrder,
        [Description("2")]
        SaleRepeating,
        [Description("3")]
        PurchaseRepeating,
        [Description("4")]
        SaleAwaitingPayment,
        [Description("5")]
        PurchaseAwaitingPayment,
        [Description("6")]
        Note,
        [Description("7")]
        PurchaseCreditNote,
        [Description("8")]
        SaleCreditNote,
        [Description("9")]
        ExpenseClaim
    }
}