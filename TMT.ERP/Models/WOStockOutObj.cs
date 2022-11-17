using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class WOStockOutObj
    {
        private int stockOutCardID;

        public int StockOutCardID
        {
            get { return stockOutCardID; }
            set { stockOutCardID = value; }
        }

        private int alloWOID;

        public int AlloWOID
        {
            get { return alloWOID; }
            set { alloWOID = value; }
        }
        private int workOrderID;

        public int WorkOrderID
        {
            get { return workOrderID; }
            set { workOrderID = value; }
        }
        private string workOrderNo;

        public string WorkOrderNo
        {
            get { return workOrderNo; }
            set { workOrderNo = value; }
        }
        private string stockOutCardCode;

        public string StockOutCardCode
        {
            get { return stockOutCardCode; }
            set { stockOutCardCode = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string productOrder;

        public string ProductOrder
        {
            get { return productOrder; }
            set { productOrder = value; }
        }
        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        private int qty;

        public int Qty
        {
            get { return qty; }
            set { qty = value; }
        }
        private string status;

        public string Status
        {
            get { return status; }
            set { status = value; }
        }



    }
}