using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{

        public enum InType
        {
            [Description("0")]
            Purchase = 0,
            [Description("1")]
            WokOrder = 1,
            [Description("2")]
            Return = 2,
            [Description("3")]
            StockMovement = 3,
           
        }
}