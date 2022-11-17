using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class CompanyObj
    {
        int ComId;

        public int? comId
        {
            get { return ComId; }
            set { ComId = value?? 0; }
        }
        DateTime Date;

        public DateTime? date
        {
            get { return Date; }
            set { Date = value ?? DateTime.Now; }
        }
        double Amount;

        public double? amount
        {
            get { return Amount; }
            set { Amount = value ?? 0; }
        }

        string Name;

        public string name
        {
            get { return Name; }
            set { Name = value; }
        }

    }
}