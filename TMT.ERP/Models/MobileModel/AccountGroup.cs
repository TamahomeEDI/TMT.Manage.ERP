using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    
    public enum AccountGroup
    {
        [Description("1")]
        System = 1,
        [Description("2")]
        Sales = 2,
        [Description("3")]
        Taxes = 3,
        [Description("4")]
        Payment = 4,
        [Description("5")]
        Revenue = 5
    }
}