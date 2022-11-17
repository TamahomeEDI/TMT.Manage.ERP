using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class TotalSaleObj
    {
        int Id;

        public int id
        {
            get { return Id; }
            set { Id = value; }
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

    }
}