using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMT.ERP.Controllers
{
    public class TreeViewController : BaseController
    {
        //
        // GET: /TreeView/

        public ActionResult Index()
        {
            return View("itemControl");
        }

    }
}
