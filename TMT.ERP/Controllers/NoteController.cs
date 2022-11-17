using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMT.ERP.BusinessLogic.Managers;
using TMT.ERP.Commons;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.Controllers
{
    public class NoteController : BaseController
    {
        private int _saleInvoicesID = 0;
        private int _CreatedUserID = 0;
        private readonly GenericManager<Note> _manager= new GenericManager<Note>();
        private readonly GenericManager<SaleInvoice> _managerSaleInvoice = new GenericManager<SaleInvoice>();
        private readonly GenericManager<Purchase> _managerPurchase = new GenericManager<Purchase>();
        private readonly GenericManager<Repeating> _managerRepeating = new GenericManager<Repeating>();
        private readonly GenericManager<Payment> _managerPayment = new GenericManager<Payment>();
        private readonly GenericManager<ExpenseClaim> _managerExpenseClaim = new GenericManager<ExpenseClaim>();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(int noteFromId, string createDate, string notes, int type, int status, string changes, string expectedDate)
        {
            var note = new Note
                           {
                               NoteFromID = noteFromId,
                               CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
                               CreatedDate =
                                   !string.IsNullOrEmpty(createDate) ? Convert.ToDateTime(createDate) : DateTime.Now,
                               Note1 = !string.IsNullOrEmpty(notes) ? notes : "Updating",
                               Type = type,
                               Status = !string.IsNullOrEmpty(status.ToString()) ? status : 0,
                               Changes = !string.IsNullOrEmpty(changes) ? changes : Models.Lookups.NoteStatus.Note.ToString(),
                               ExpectedPaymentDate = !string.IsNullOrEmpty(expectedDate)
                                                         ? Convert.ToDateTime(expectedDate)
                                                         : DateTime.Now
                           };
            if (ModelState.IsValid)
            {
                _manager.Add(note);
                _manager.Save();
            }
            ViewBag.CountItems = _manager.Get(x => x.NoteFromID == noteFromId && x.Type == type).Count();
            return View(note);
        }
        public int ReturnInvoicesID(int id)
        {
            _saleInvoicesID = id;
            return _saleInvoicesID;
        }
        public ActionResult AddNoteCreateRepeatingInvoice(string saleInvoiceId, int? noteType)
        {
            int id = Convert.ToInt32(saleInvoiceId);
            var note = new Note
                           {
                               NoteFromID = id,
                               CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
                               CreatedDate = DateTime.Now,
                               Note1 = "Create note repeating in " + DateTime.Now + ".",
                               Type = noteType ?? 0,
                               Status = 0,
                               Changes = Models.Lookups.NoteStatus.Created.ToString(),
                               ExpectedPaymentDate = DateTime.Now
                           };
            if (ModelState.IsValid)
            {
                _manager.Add(note);
                _manager.Save();
            }
            return View();
        }

        public ActionResult AddNoteByNoteFromID(int? noteFromID, int? noteType, string[] arrNoteID, string change)
        {
            _CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;

            var note = new Note
            {
                NoteFromID = noteFromID ?? 0,
                CreatedUserID = AppContext.RequestVariables.CurrentUser.ID,
                CreatedDate = DateTime.Now,
                Note1 = "Create note repeating in " + DateTime.Now + ".",
                Type = noteType ?? 0,
                Status = 0,
                Changes = change,
                ExpectedPaymentDate = DateTime.Now
            };
            //Add new note with ID from screen contains note
            if (ModelState.IsValid)
            {
                _manager.Add(note);                
            }
            // Update NoteFromID for Note with type is Note
           // dynamic array = JsonConvert.DeserializeObject(arrNoteID);
            if (arrNoteID != null)
            {
                foreach (var item in arrNoteID)
                {
                    Note oNote = _manager.FindById(Int32.Parse(item));
                    oNote.NoteFromID = noteFromID ?? 0;
                    //oNote.Type = noteType ?? 0;
                    _manager.Update(oNote);
                }
            }

            //Delete all note with noteFromID = 0
            var listNote = _manager.Get(no => no.NoteFromID == 0 && no.Type == noteType && no.CreatedUserID == _CreatedUserID);
            listNote.ToList().ForEach(x => _manager.Delete(x));   

            _manager.Save();
            ViewBag.NoteType = noteType;
            ViewBag.NoteFromID = noteFromID;
            return new EmptyResult();
        }
        public ActionResult GetNotesAwaitingPaymentDetail(int? saleInvoidId)
        {
            var notes = _manager.Get(x => x.NoteFromID == saleInvoidId && x.Type == 4).OrderByDescending(x => x.ID);
            return PartialView("_GetNotesAwaitingPaymentDetail", notes);
        }

        /// <summary>
        /// Get Reference
        /// iType - 0 : Sale Invoice;
        /// iType - 1 : Purchase;
        /// iType - 2..3 : Repeating;
        /// iType - 4..5 : Awaiting Payment;
        /// iType - 7..8 : Credit Note;
        /// iType - 9 : Expensive Claim;
        /// </summary>
        /// <param name="noteFromID"></param>
        /// <param name="iType"></param>
        /// <returns></returns>
        private string GetReference(int? noteFromID,int iType)
        {
            string sReturn = string.Empty;
            switch (iType)
            {
                case 0:
                case 8:
                    if (_managerSaleInvoice.FindById(noteFromID) != null)
                    {
                        sReturn = _managerSaleInvoice.FindById(noteFromID).Reference;
                    }    
                    break;
                case 1:
                case 7:
                    if (_managerPurchase.FindById(noteFromID) != null)
                    {
                        sReturn = _managerPurchase.FindById(noteFromID).Reference;
                    }
                    break;
                case 2:
                    if (_managerRepeating.FindById(noteFromID) != null)
                    {
                        sReturn = _managerRepeating.FindById(noteFromID).SaleInvoice.Reference;
                    }
                    break;
                case 3:
                    if (_managerRepeating.FindById(noteFromID) != null)
                    {
                        sReturn = _managerRepeating.FindById(noteFromID).Purchase.Reference;
                    }
                    break;
                case 4:
                    if (_managerPayment.FindById(noteFromID) != null)
                    {
                        sReturn = _managerPayment.FindById(noteFromID).SaleInvoice.Reference;
                    }
                    break;
                case 5:
                    if (_managerPayment.FindById(noteFromID) != null)
                    {
                        sReturn = _managerPayment.FindById(noteFromID).Purchase.Reference;
                    }
                    break;
                case 9:
                    if (_managerExpenseClaim.FindById(noteFromID) != null)
                    {
                        sReturn = _managerExpenseClaim.FindById(noteFromID).Reference;
                    }
                    break;
            }
            return sReturn;
        }

        public ActionResult GetNotesList(int? noteType, int? noteFromID, int Type, double? dTotal)
        {
            _CreatedUserID = AppContext.RequestVariables.CurrentUser.ID;
            var notes = _manager.Get(x => x.NoteFromID == noteFromID && x.Type == noteType).OrderByDescending(x => x.ID);
            //var notes = _manager.Get(x => x.NoteFromID == noteFromID && x.Type == noteType && x.CreatedUserID == _CreatedUserID).OrderByDescending(x => x.ID);
           // var notes = _manager.Get(x => x.NoteFromID == noteFromID &&  x.CreatedUserID == _CreatedUserID).OrderByDescending(x => x.ID);
            ViewBag.CountItems = _manager.Get(x => x.NoteFromID == noteFromID && x.Type == noteType).Count();
            if (notes.Count() != 0)
            {
                ViewBag.NoteLatest = notes.First();    
            }            

            double d_Total = 0;
            if (dTotal.HasValue)
            {
                d_Total = dTotal.Value;
            }
            ViewBag.Reference = GetReference(noteFromID, Type);
            ViewBag.Total = d_Total.ToString("#,0.00");
            return PartialView("~/Views/Shared/Note/_ViewNote.cshtml", notes);
        }
    }
}
