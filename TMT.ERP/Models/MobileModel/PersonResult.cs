using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class PersonResult
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
        private IEnumerable<Person> Data;

        public IEnumerable<Person> data
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
    }
}