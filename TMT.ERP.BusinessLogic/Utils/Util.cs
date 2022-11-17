using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.BusinessLogic.Utils
{
    public class Util
    {
        public static string GetFullCode(int currentValue, int lengthOfCode)
        {
            var nextNumber = currentValue + 1;
            var nextStr = nextNumber.ToString();
            while (nextStr.Length < lengthOfCode)
                nextStr = "0" + nextStr;
            return nextStr;
        }

        //type: 0-Sale;1-Purchase;2-Stock in;3-Stock out;4- Planning;5-WorkOrder
        public static string GetCode(string type)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().FirstOrDefault(x => x.Type == Int32.Parse(type));
            if (item != null)
                return item.InvoicePrefix + item.NextNumber;
            else
                return "";
        }

        public static bool ExistCode(string code, string type)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().Where(x => x.Type == Int32.Parse(type) && x.InvoicePrefix == code).FirstOrDefault();
            if (item != null)
                return true;
            else
                return false;
        }

        public static void UpdateNextNumber(string type, int lengOfCode, string defaultPrefix)
        {
            var manager = new GenericManager<SaleInvoiceSetting>();
            var item = manager.Get().FirstOrDefault(x => x.Type == Int32.Parse(type));
            int currentValue = 0;
            if (item == null)
            {
                item = new SaleInvoiceSetting();
                if (!string.IsNullOrEmpty(defaultPrefix))
                    item.InvoicePrefix = defaultPrefix;
                manager.Add(item);
            }
            else
            {
                currentValue = Int32.Parse(item.NextNumber.ToString());
            }

            item.NextNumber = GetFullCode(currentValue, lengOfCode);
            manager.Save();
        }
    }
}
