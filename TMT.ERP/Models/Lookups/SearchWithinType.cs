using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{
    public enum SearchWithinType
    {
        [Description("Any Date")]
        AnyDate,
        [Description("Due Date")]
        DueDate,
        [Description("Transaction Date")]
        TransactionDate
    }
}