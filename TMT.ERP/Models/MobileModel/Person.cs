using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class Person
    {
        int PersonId;

        public int personId
        {
            get { return PersonId; }
            set { PersonId = value; }
        }

        string PersonName;

        public string personName
        {
            get { return PersonName; }
            set { PersonName = value; }
        }
        double? Amount;

        public double? amount
        {
            get { return Amount; }
            set { Amount = value; }
        }
        string Company;

        public string company
        {
            get { return Company; }
            set { Company = value; }
        } 
    }
}