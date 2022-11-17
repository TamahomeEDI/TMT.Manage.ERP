using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMT.ERP.BusinessLogic;

namespace TMT.ERP.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            int repeatType = 0;
            BusinessLogic.Utils.ScheduleTask.ExecRepeating(repeatType);
        }

    }
}
