using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class LoginResult
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

        private string Message;

        public string message
        {
            get { return Message; }
            set { Message = value; }
        }

        private string SessionId;

        public string sessionId
        {
            get { return SessionId; }
            set { SessionId = value; }
        }


    }
}