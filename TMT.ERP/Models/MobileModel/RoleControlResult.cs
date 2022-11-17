using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class RoleControlResult
    {
        private string ControlID;

        public string controlID
        {
            get { return ControlID; }
            set { ControlID = value; }
        }
 
        private IEnumerable<RoleObj> Data;

        public IEnumerable<RoleObj> data
        {
            get { return Data; }
            set { Data = value; }
        }

    }
}