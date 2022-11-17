using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;


namespace TMT.ERP.Controllers
{
    public class FixedAssetDetailsController : BaseController
    {
        private ErpEntities db = new ErpEntities();

        //
        // GET: /FixedAssetDetail/

        public ActionResult Index()
        {
            return View(db.FixedAssetDetails.ToList());
        }

        //
        // GET: /FixedAssetDetail/Details/5

        public ActionResult Details(int id = 0)
        {
            FixedAssetDetail asset = db.FixedAssetDetails.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // GET: /FixedAssetDetail/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /FixedAssetDetail/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FixedAssetDetail asset)
        {
            if (ModelState.IsValid)
            {
                db.FixedAssetDetails.Add(asset);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(asset);
        }

        //
        // GET: /FixedAssetDetail/Edit/5

        public ActionResult Edit(int id = 0)
        {
            FixedAssetDetail asset = db.FixedAssetDetails.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /FixedAssetDetail/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FixedAssetDetail asset)
        {
            if (ModelState.IsValid)
            {
                db.Entry(asset).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(asset);
        }

        //
        // GET: /FixedAssetDetail/Delete/5

        public ActionResult Delete(int id = 0)
        {
            FixedAssetDetail asset = db.FixedAssetDetails.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            return View(asset);
        }

        //
        // POST: /FixedAssetDetail/Delete/5

        [HttpPost]
        public ActionResult DeleteConfirmed(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                FixedAssetDetail asset = db.FixedAssetDetails.Find(id);
                if (asset != null)
                {
                    db.FixedAssetDetails.Remove(asset);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
