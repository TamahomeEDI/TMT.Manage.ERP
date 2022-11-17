using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.Models.Lookups
{
    public enum PayItemTypeValue
    {
        [Description("1")]
        Wages = 1,
        [Description("2")]
        Allowances = 2,
        [Description("3")]
        Deductions = 3,
        [Description("4")]
        Taxes = 4,
        [Description("5")]
        NonTaxableAllowances = 5,
        [Description("6")]
        PostTaxDeductions = 6,
        [Description("7")]
        EmployerContributions = 7
    }
}