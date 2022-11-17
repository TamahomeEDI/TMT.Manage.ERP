using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.Models.Lookups
{
    public enum CodeType
    {
        [Description("0")]
        SaleOrder = 0,
        [Description("1")]
        PurchaseOrder = 1,
        [Description("2")]
        IncomingShipment = 2,
        [Description("3")]
        OutgomingShipment = 3,
        [Description("4")]
        WorkProcess = 4,
        [Description("5")]
        WorkOrder = 5,
        [Description("6")]
        TranferStock = 6,
        [Description("7")]
        ReturnWorkOrder = 7,
        [Description("8")]
        AllocateWorkOrder = 8,
        [Description("9")]
        PayRun = 9,       
        [Description("10")]
        ExpenseClaim = 10,
        [Description("11")]
        Employee = 11,
        [Description("12")]
        FixedAssetItem = 12
    }
}