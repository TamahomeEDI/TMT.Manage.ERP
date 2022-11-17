using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                ProcessPending(Int32.Parse(args[0]));
            }

        }

        private static void ProcessPending(int repeatType)
        {
            try
            {
                TMT.ERP.BusinessLogic.Utils.ScheduleTask.ExecRepeating(repeatType);
            }
            catch { }
        }


    }
}
