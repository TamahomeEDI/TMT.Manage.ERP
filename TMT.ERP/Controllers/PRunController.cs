using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMT.ERP.Controllers
{
    public class PRunController : BaseController
    {
        //
        // GET: /PRun/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /PRun/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /PRun/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /PRun/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PRun/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /PRun/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PRun/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /PRun/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
