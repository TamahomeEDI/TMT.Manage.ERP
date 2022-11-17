using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models.Lookups
{
    public enum NoteStatus        
    {
        [Description("Created")]
        Created,
        [Description("Note")]
        Note,
        [Description("Partially Paid")]
        PartiallyPaid,
        [Description("Approved")]
        Approved,
        [Description("Edited")]
        Edited
    }
}