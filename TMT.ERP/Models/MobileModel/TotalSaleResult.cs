using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class TotalSaleResult
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
        private IEnumerable<TotalSaleObj> Data;

        public IEnumerable<TotalSaleObj> data
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

        private string MinAmount;

        public string minAmount
        {
            get { return MinAmount; }
            set { MinAmount = value; }
        }
        private string MaxAmount;

        public string maxAmount
        {
            get { return MaxAmount; }
            set { MaxAmount = value; }
        }

    }
}