using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{
    public enum PayFrequency
    {
        [Description("0")]
        Weekly = 0,
        [Description("1")]
        Every2Weeks = 1,
        [Description("2")]
        Monthly = 2,
        [Description("3")]
        Custom = 3
    }
}