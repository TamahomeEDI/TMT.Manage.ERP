using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class Client
    {
        int? ClientId;
        /// <summary>
        /// 
        /// </summary>
        public int? clientId
        {
            get { return ClientId; }
            set { ClientId = value; }
        }
        string ClientName;

        public string clientName
        {
            get { return ClientName; }
            set { ClientName = value; }
        }
        DateTime Date;

        public DateTime date
        {
            get { return Date; }
            set { Date = value; }
        }
        double AmountPurchase;

        public double amountPurchase
        {
            get { return AmountPurchase; }
            set { AmountPurchase = value; }
        }

        string Company;
        public string company
        {
            get { return Company; }
            set { Company = value; }
        }

        public int? companyId { get; set; }
        public double? amountUnpaid { get; set; }

    }
}