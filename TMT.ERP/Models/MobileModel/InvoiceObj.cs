using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class InvoiceObj
    {
        int InvoiceId;

        public int invoiceId
        {
            get { return InvoiceId; }
            set { InvoiceId = value; }
        }
        string Customer;

        public string customer
        {
            get { return Customer; }
            set { Customer = value; }
        }
        DateTime DueDate;

        public DateTime dueDate
        {
            get { return DueDate; }
            set { DueDate = value; }
        }
        double Paid;

        public double paid
        {
            get { return Paid; }
            set { Paid = value; }
        }
        double AmountDue;

        public double amountDue
        {
            get { return AmountDue; }
            set { AmountDue = value; }
        }

        string Category;
        public string category
        {
            get { return Category; }
            set { Category = value; }
        }

        string Company;
        public string company
        {
            get { return Company; }
            set { Company = value; }
        } 

    }
}