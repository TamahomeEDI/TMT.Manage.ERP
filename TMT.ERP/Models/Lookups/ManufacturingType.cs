using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{

    public enum ManufacturingType
        {
            [Description("NoneContinue")]
            N=1,
            [Description("Continue")]
            C=2
        }
}