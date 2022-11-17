using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Models
{
    public class Response
    {
        string code;
        string result;
        object data;
        public Response(string codestr, string resultstr, object objdata)
        {
            code = codestr;
            result = resultstr;
            data = objdata;
        }
    }
}