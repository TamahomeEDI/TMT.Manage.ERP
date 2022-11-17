using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMT.ERP.Models
{
    public class InvoiceItemResult
    {
        private string Code;

        public string code
        {
            get { return Code; }
            set { Code = value; }
        }
        private string Result;

        public string result
        {
            get { return Result; }
            set { Result = value; }
        }


        private IEnumerable<InvoiceSubItem> Data;
        public IEnumerable<InvoiceSubItem> data
        {
            get { return Data; }
            set { Data = value; }
        }

        private string Message;

        public string message
        {
            get { return Message; }
            set { Message = value; }
        }

        public double? disCount { get; set; }
        public double? taxRate { get; set; }
        public double? total { get; set; }
        
    }
}