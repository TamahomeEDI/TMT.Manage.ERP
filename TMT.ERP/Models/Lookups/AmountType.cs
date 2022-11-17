using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{
    public enum AmountType        
    {
        [Description("Tax Exclusive")]
        TaxExclusive,
        [Description("Tax Inclusive")]
        TaxInclusive,
        [Description("No Tax")]
        NoTax        
    }
}