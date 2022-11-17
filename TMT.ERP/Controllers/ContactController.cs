using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.DataAccess.Model;
using TMT.ERP.Models;

namespace TMT.ERP.Controllers
{
    public class ContactController : BaseController
    {
        //
        // GET: /Contact/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateObject()
        {
            //var rfidTag = new RFIDTag();
            return null;
        }

        ////load machine
        //public ActionResult LoadGridContact()
        //{
        //    var lstContact = iContactRepository.All.ToList();
        //    return PartialView("", lstContact);
        //}

        ////add Contact
        //[HttpPost]
        //public JsonResult AddContact(Contact _model)
        //{
        //    if (_model != null)
        //    {
        //        try
        //        {
        //            iContactRepository.Add(_model);
        //            iContactRepository.Save();
        //        }
        //        catch (Exception er)
        //        {
        //            return this.Json(JsonResponseFactory.ErrorResponse(er.Message), JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return this.Json(JsonResponseFactory.SuccessResponse(), JsonRequestBehavior.AllowGet);
        //}


        ////Delete machine
        //[HttpPost]
        //public JsonResult RemoveContact(int id)
        //{
        //    var contactID = iContactRepository.FindBy(r => r.ID == id).FirstOrDefault();
        //    if (contactID != null)
        //    {
        //        try
        //        {
        //            iContactRepository.Delete(contactID);
        //            iContactRepository.Save();
        //        }
        //        catch (Exception er)
        //        {
        //            return this.Json(JsonResponseFactory.ErrorResponse(er.Message), JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    return this.Json(JsonResponseFactory.SuccessResponse(), JsonRequestBehavior.AllowGet);
        //}


        //
        // GET: /Currency/Details/5

        //public ActionResult Details(int id = 0)
        //{
        //    Currency currency = db.Currencies.Find(id);
        //    if (currency == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(currency);
        //}

        ////
        //// GET: /Currency/Create

        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /Currency/Create

        //[HttpPost]
        //public ActionResult Create(Currency currency)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Currencies.Add(currency);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(currency);
        //}

        ////
        //// GET: /Currency/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
        //    Currency currency = db.Currencies.Find(id);
        //    if (currency == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(currency);
        //}

        ////
        //// POST: /Currency/Edit/5

        //[HttpPost]
        //public ActionResult Edit(Currency currency)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(currency).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(currency);
        //}

        ////
        //// GET: /Currency/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    Currency currency = db.Currencies.Find(id);
        //    if (currency == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(currency);
        //}

        ////
        //// POST: /Currency/Delete/5

        //[HttpPost, ActionName("Delete")]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Currency currency = db.Currencies.Find(id);
        //    db.Currencies.Remove(currency);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}