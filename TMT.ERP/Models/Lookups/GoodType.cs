using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{
    public enum GoodType        
    {
        [Description("1")]
        Raw = 1,
        [Description("2")]
        Semi = 2,
        [Description("3")]
        Finish = 3
    }
}