using System;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TMT.ERP.DataAccess.Model;
using System.Text;
using System.Data.Entity.Validation;
using CommonLib;
using System.Globalization;
using TMT.ERP.BusinessLogic.Utils;
using TMT.ERP.Commons;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using TMT.ERP.Models.Lookups;

namespace TMT.ERP.Controllers
{
    public class FixedAssetsController : BaseController
    {
        private ErpEntities db = new ErpEntities();


        //
        // GET: /FixedAssets/

        public static int PageSize = Constant.DefaultPageSize;
        public static string result = "";

        [HttpPost]
        public int SetPageSize(string pageSize)
        {
            PageSize = Convert.ToInt32(pageSize);
            return PageSize;
        }

        //Starting Fixed Asset
        public ActionResult StartFixedAsset()
        {
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.NextYear = DateTime.Now.Year + 1;
            return View();
        }
        public ActionResult SetYear(int year)
        {
            FixedAssetSetting fixedAsset = db.FixedAssetSettings.Find(1);
            if (fixedAsset == null)
            {
                FixedAssetSetting setting = new FixedAssetSetting();
                setting.Year = year;
                db.FixedAssetSettings.Add(setting);
                db.SaveChanges();
            }
            else
            {
                fixedAsset.Year = year;
                db.Entry(fixedAsset).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public int CheckYear(string year)
        {
            DateTime date = DateTime.Parse(year.ToString(CultureInfo.InvariantCulture));
            int check = date.Year;
            FixedAssetSetting fixedAsset = db.FixedAssetSettings.Find(1);
            if (fixedAsset.Year <= check)
            {
                var checkresult = 0;
                return checkresult;
            }
            else
            {
                var checkresult = 1;
                return checkresult;
            }
        }


        // Pending Items
        public ActionResult Index(int? page, string searchString)
        {
            int? userID = TMT.ERP.Commons.AppContext.SessionVariables.CurrentUserID;
            FixedAssetSetting fixedAsset = db.FixedAssetSettings.Find(1);
            if (fixedAsset == null)
            {
                return RedirectToAction("StartFixedAsset");
            }
            else
            {
                string year = fixedAsset.Year.ToString();
                if (year == "" || year == null)
                {
                    return RedirectToAction("StartFixedAsset");
                }
                if (searchString == null)
                {
                    var asset = db.FixedAssets.Where(f => f.Status == 0).OrderByDescending(f => f.ID).Include(a => a.Account).ToList();
                    int pageSize = PageSize;
                    ViewBag.PageSize = PageSize;
                    int pageNumber = (page ?? 1);
                    ViewBag.Error = result;
                    result = "";
                    return View(asset.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var asset = db.FixedAssets.Where(c => c.Name.Contains(searchString) & c.Status == 0).OrderByDescending(c => c.ID).Include(a => a.Account).ToList();
                    int pageSize = PageSize;
                    ViewBag.PageSize = PageSize;
                    int pageNumber = (page ?? 1);
                    ViewBag.Error = result;
                    result = "";
                    return View(asset.ToPagedList(pageNumber, pageSize));
                }
            }
        }

        // Registered Items
        public ActionResult Register(int? page, string searchString)
        {
            if (searchString == null)
            {
                var asset = db.FixedAssets.Where(f => f.Status == 1).OrderByDescending(f => f.ID).Include(a => a.Account).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(asset.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var asset = db.FixedAssets.Where(c => c.Name.Contains(searchString) & c.Status == 1).OrderByDescending(c => c.ID).Include(a => a.Account).ToList();
                int pageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(asset.ToPagedList(pageNumber, pageSize));
            }
        }

        // Sold & Disposed Items
        public ActionResult Dispose(int? page, string searchString)
        {
            if (searchString == null)
            {
                var asset = db.FixedAssets.Where(f => f.Status == 2).OrderByDescending(f => f.ID).Include(a => a.Account).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(asset.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var asset = db.FixedAssets.Where(c => c.Name.Contains(searchString) & c.Status == 2).OrderByDescending(c => c.ID).Include(a => a.Account).ToList();
                int pageSize = PageSize;
                ViewBag.PageSize = PageSize;
                int pageNumber = (page ?? 1);
                ViewBag.Error = result;
                result = "";
                return View(asset.ToPagedList(pageNumber, pageSize));
            }
        }

        //
        // GET: /FixedAsset/Create

        public ActionResult Create()
        {
            FixedAssetSetting setting = db.FixedAssetSettings.Find(1);
            ViewBag.Year = setting.Year;
            ViewBag.AccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name");
            ViewBag.DepreciationAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 27), "ID", "Name");
            ViewBag.AccumDepreAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name");
            ViewBag.AssetCode = Utility.GetCode(CommonLib.EnumHelper.GetDescription(TMT.ERP.Models.Lookups.CodeType.FixedAssetItem));
            ViewBag.Error = result;
            result = "";
            return View();
        }

        //
        // POST: /FixedAsset/Create

        [HttpPost]
        public string Create(string name, string code, int? accountID, string date, string price, string accummulate,
            string description, string type, string rate, string method, int? depreciationID, int save, int? accumAccID)
        {
            string returnValue = "";
            int? userID = TMT.ERP.Commons.AppContext.SessionVariables.CurrentUserID;
            FixedAsset asset = new FixedAsset();
            asset.Status = save;
            asset.Name = name;
            asset.Code = code;
            asset.AccountID = accountID;
            asset.CreatedDate = DateTime.Now;
            asset.PurchaseDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            asset.PurchasePrice = float.Parse(price);
            asset.Description = description;
            asset.AssetType = type;
            asset.DepreciationRate = Double.Parse(rate);
            asset.DepreciationMethod = Int32.Parse(method);
            asset.DepreciationAccountID = depreciationID;
            asset.AccumulatedDepreciationAccountID = accumAccID;
            asset.CreatedUserID = userID;
            if (accummulate == null || accummulate == "")
            {
                asset.AccumulatedDepreciation = null;
            }
            else
            {
                asset.AccumulatedDepreciation = Double.Parse(accummulate);
            }
            db.FixedAssets.Add(asset);
            try
            {
                db.SaveChanges();
                Utility.UpdateNextNumber(CommonLib.EnumHelper.GetDescription(CodeType.FixedAssetItem), Constant.CODE_LENGTH, "");
                returnValue = "Create";
            }
            catch (Exception e)
            {
                returnValue = "Error";
            }
            return returnValue;
        }

        //
        // GET: /FixedAsset/Edit/5

        public ActionResult Details(int id = 0)
        {
            FixedAsset asset = db.FixedAssets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            ViewBag.Code = asset.Code;
            ViewBag.AccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name", asset.AccountID);
            ViewBag.DepreciationAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 27), "ID", "Name", asset.DepreciationAccountID);
            ViewBag.AccumDepreAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name", asset.AccumulatedDepreciationAccountID);
            return View(asset);
        }

        //
        // GET: /FixedAsset/Edit/5

        public ActionResult Edit(int id = 0, int tab = 0)
        {
            FixedAsset asset = db.FixedAssets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            FixedAssetSetting setting = db.FixedAssetSettings.Find(1);
            ViewBag.Year = setting.Year;
            ViewBag.AccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name", asset.AccountID);
            ViewBag.DepreciationAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 27), "ID", "Name", asset.DepreciationAccountID);
            ViewBag.AccumDepreAccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name", asset.AccumulatedDepreciationAccountID);
            ViewBag.Error = result;
            ViewBag.Tab = tab;
            result = "";
            ViewBag.Code = asset.Code;
            return View(asset);
        }

        //
        // POST: /FixedAsset/Edit/5

        [HttpPost]
        public string Edit(string name, string code, int? accountID, string date, string price, int id, int? accumAccID,
            string description, string type, string rate, string method, int? depreciationID, int save, string accummulate)
        {
            string returnValue = "";
            int? userID = TMT.ERP.Commons.AppContext.SessionVariables.CurrentUserID;
            FixedAsset asset = db.FixedAssets.Find(id);
            asset.Status = save;
            asset.Name = name;
            asset.Code = code;
            asset.AccountID = accountID;
            asset.CreatedDate = DateTime.Now;
            asset.PurchaseDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            asset.PurchasePrice = float.Parse(price);
            asset.Description = description;
            asset.AssetType = type;
            asset.DepreciationRate = Double.Parse(rate);
            asset.DepreciationMethod = Int32.Parse(method);
            asset.DepreciationAccountID = depreciationID;
            asset.AccumulatedDepreciationAccountID = accumAccID;
            asset.CreatedUserID = userID;
            if (accummulate == null || accummulate == "")
            {
                asset.AccumulatedDepreciation = null;
            }
            else
            {
                asset.AccumulatedDepreciation = Double.Parse(accummulate);
            }
            db.Entry(asset).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
                returnValue = "Edit";
            }
            catch (Exception e)
            {
                returnValue = "Error";
            }
            return returnValue;

        }

        //POST : /FixedAssets/Registeritems
        [HttpPost]
        public string Registeritems(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                FixedAsset asset = db.FixedAssets.Find(id);
                try
                {
                    asset.Status = 1;
                    db.Entry(asset).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "Resgister";
                }
                catch (Exception e)
                {
                    result = "Error";
                }
            }
            return result;
        }


        //POST : /FixedAssets/SoldItems
        [HttpPost]
        public string SoldItems(string[] delConfirm)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                FixedAsset asset = db.FixedAssets.Find(id);
                try
                {
                    asset.Status = 2;
                    db.Entry(asset).State = EntityState.Modified;
                    db.SaveChanges();
                    result = "Sold";
                }
                catch (Exception e)
                {
                    result = "Error";
                }
            }
            return result;
        }

        //
        // GET : /FixedAsset/Depreciation/
        public ActionResult Depreciation()
        {
            FixedAssetSetting setting = db.FixedAssetSettings.Find(1);
            ViewBag.Year = setting.Year;
            ViewBag.AccountID = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 24), "ID", "Name");
            List<Account> account = db.Accounts.Where(a => a.AccountTypeID == 24).ToList();
            ViewBag.Count = account.Count();
            return View(account);
        }

        //Calculate Depreciation
        public double? DepreciationAmount(string date, int? id, int? depreAccID, int? accumAccID)
        {
            double? sum = 0;
            DateTime datetime = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
            FixedAssetSetting setting = db.FixedAssetSettings.Find(1);
            List<FixedAsset> asset = new List<FixedAsset>();
            if (id != null && id > 0)
            {
                asset = db.FixedAssets.Where(a => a.AccountID == id && a.Status == 1).ToList();
            }
            else if (depreAccID != null && depreAccID > 0)
            {
                asset = db.FixedAssets.Where(a => a.DepreciationAccountID == depreAccID && a.Status == 1).ToList();
            }
            else if (accumAccID != null && accumAccID > 0)
            {
                asset = db.FixedAssets.Where(a => a.AccumulatedDepreciationAccountID == accumAccID && a.Status == 1).ToList();
            }
            if (asset == null)
            {
                sum = 0;
            }
            else
            {
                foreach (var item in asset)
                {
                    DateTime purDate = item.PurchaseDate ?? DateTime.Now;
                    string startYear = "1 Jan " + setting.Year;
                    string EndYear = "31 Dec " + setting.Year;
                    DateTime startDate = DateTime.Parse(startYear.ToString(CultureInfo.InvariantCulture));
                    DateTime EndDate = DateTime.Parse(EndYear.ToString(CultureInfo.InvariantCulture));
                    Double doy = ((EndDate - startDate).TotalDays + 1);
                    TimeSpan ts = startDate.Subtract(purDate);
                    // Have not calculate depreciation
                    if (item.ApprovedDate != null)
                    {
                        DateTime approveDdate = item.ApprovedDate ?? DateTime.Now;
                        TimeSpan time = datetime.Subtract(approveDdate);
                        Double day = time.TotalDays + 1;
                        if (day > 1)
                        {
                            if (item.DepreciationMethod == 0)
                            {
                                sum += (item.PurchasePrice * item.DepreciationRate * day / 100) / doy;
                            }
                            else if (item.DepreciationMethod == 1)
                            {
                                if (item.AccumulatedDepreciation == null)
                                {
                                    item.AccumulatedDepreciation = 0;
                                }
                                sum += ((item.PurchasePrice - item.AccumulatedDepreciation) * item.DepreciationRate * day / 100) / doy;
                            }
                        }
                    }
                    else
                    {
                        //Start Depreciation Date > Purchase Date
                        if (ts.TotalDays > 0)
                        {
                            TimeSpan time = datetime.Subtract(startDate);
                            Double day = time.TotalDays + 1;
                            if (day > 1)
                            {
                                if (item.DepreciationMethod == 0)
                                {
                                    sum += (item.PurchasePrice * item.DepreciationRate * day / 100) / doy;
                                }
                                else if (item.DepreciationMethod == 1)
                                {
                                    if (item.AccumulatedDepreciation == null)
                                    {
                                        item.AccumulatedDepreciation = 0;
                                    }
                                    sum += ((item.PurchasePrice - item.AccumulatedDepreciation) * item.DepreciationRate * day / 100) / doy;
                                }
                            }
                        }
                        //Start Depreciation Date < Purchase Date
                        else
                        {
                            TimeSpan time = datetime.Subtract(purDate);
                            Double day = time.TotalDays + 1;
                            if (day > 1)
                            {
                                if (item.DepreciationMethod == 0)
                                {
                                    sum += (item.PurchasePrice * item.DepreciationRate * day / 100) / doy;
                                }
                                else if (item.DepreciationMethod == 1)
                                {
                                    if (item.AccumulatedDepreciation == null)
                                    {
                                        item.AccumulatedDepreciation = 0;
                                    }
                                    sum += ((item.PurchasePrice - item.AccumulatedDepreciation) * item.DepreciationRate * day / 100) / doy;
                                }
                            }
                        }
                    }
                }
            }
            return sum;
        }

        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();
        //Approve Depreciation
        public string ApproveDepreciation(string depreInfo, string date)
        {
            dynamic array = JsonConvert.DeserializeObject(depreInfo);
            foreach (var item in array)
            {
                int id = item.accID;
                //Update for Account Credit
                Account account = db.Accounts.Find(id);
                if (account.TotalCredit != null)
                {
                    account.TotalCredit += Double.Parse(item.depreciation.ToString());
                }
                else
                {
                    account.TotalCredit = Double.Parse(item.depreciation.ToString());
                }
                db.Entry(account).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                    result = "Approve";
                }
                catch (Exception e)
                {
                    result = "Error";
                }
            }
            //Update for Fixed Assets Items
            List<FixedAsset> asset = db.FixedAssets.Where(a => a.Status == 1).ToList();
            foreach (var item in asset)
            {
                FixedAsset fixedAsset = db.FixedAssets.Find(item.ID);

                // Calculate Debit For Item Depreciation Account
                Account depreAccount = db.Accounts.Find(fixedAsset.DepreciationAccountID);
                double? sum = 0;
                sum = DepreciationAmount(date, 0, depreAccount.ID, 0);
                if (depreAccount.TotalDebit != null)
                {
                    depreAccount.TotalDebit += sum;
                }
                else
                {
                    depreAccount.TotalDebit = sum;
                }
                db.Entry(depreAccount).State = EntityState.Modified;

                //Update Approve Date
                fixedAsset.ApprovedDate = DateTime.Parse(date.ToString(CultureInfo.InvariantCulture));
                db.Entry(fixedAsset).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    result = "Approve";
                }
                catch (Exception e)
                {
                    result = "Error";
                }
            }
            return result;
        }

        // Sell / Dispose Fixed Asset Item
        public ActionResult SellDispose(int id = 0)
        {
            FixedAsset asset = db.FixedAssets.Find(id);
            ViewBag.ID = asset.ID;
            ViewBag.Name = asset.Name;
            ViewBag.Code = asset.Code;
            ViewBag.AccountID = new SelectList(db.Accounts.ToList(), "ID", "Name");
            return View();
        }

        // Review Journal
        public ActionResult ReviewJournal(int? id, int? account, string price, string dateSold,
            string depreciationDate, int method, int type)
        {
            ViewBag.Method = method;
            ViewBag.Type = type;
            FixedAsset asset = db.FixedAssets.Find(id);
            if (asset == null)
            {
                return HttpNotFound();
            }
            Account acc = db.Accounts.Find(asset.AccountID);
            ViewBag.Acc = acc.Name;
            ViewBag.ID = id;
            Account depreAcc = db.Accounts.Find(asset.AccumulatedDepreciationAccountID);
            if (depreAcc != null)
            {
                ViewBag.DepreAcc = depreAcc.Name;
            }
            DateTime purDate = asset.PurchaseDate ?? DateTime.Now;
            ViewBag.PurchaseDate = purDate.ToString("dd-MMM-yyyy");
            ViewBag.DateSold = dateSold;
            ViewBag.DepreciationDate = depreciationDate;
            double? sum = 0;
            double? totalDepre = 0;
            double? saleDepre = 0;
            int reversal = 0;
            var formatString = "#,0.00";
            string sumString = "";
            if (type == 1)
            {
                if (asset.ApprovedDate == null)
                {
                    reversal = 1;
                    ViewBag.Reversal = reversal;
                    sum = DepreciationAmount(depreciationDate, 0, 0, asset.AccumulatedDepreciationAccountID);
                    var sumValue = sum.Value.ToString(formatString);
                    sumString = (sumValue.IndexOf("-") > -1) ? sumValue.Replace("-", "(") + ")" : sumValue;
                }
                else
                {
                    reversal = 2;
                    ViewBag.Reversal = reversal;
                    totalDepre = DepreciationAmount(asset.ApprovedDate.ToString(), 0, 0, asset.AccumulatedDepreciationAccountID);
                    saleDepre = DepreciationAmount(depreciationDate, 0, 0, asset.AccumulatedDepreciationAccountID);
                    sum = saleDepre - totalDepre;
                    var sumValue = sum.Value.ToString(formatString);
                    sumString = (sumValue.IndexOf("-") > -1) ? sumValue.Replace("-", "(") + ")" : sumValue;
                }
            }
            DateTime date = asset.ApprovedDate ?? DateTime.Now;
            ViewBag.AprroveDate = date.ToString("dd-MMM-yyyy");
            ViewBag.CurrentDepre = totalDepre;
            ViewBag.ReversalDepre = sumString;
            ViewBag.GainLoss = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 27 || a.AccountTypeID == 29).ToList(), "ID", "Name");
            ViewBag.TotalCapital = new SelectList(db.Accounts.Where(a => a.AccountTypeID == 26 || a.AccountTypeID == 29).ToList(), "ID", "Name");
            if (method == 0)
            {
                sum = sum = DepreciationAmount(depreciationDate, 0, 0, asset.AccumulatedDepreciationAccountID);
            }
            ViewBag.Depreciation = sum;
            if (method == 0)
            {
                Account saleAcc = db.Accounts.Find(account);
                ViewBag.SaleAccount = account;
                ViewBag.Account = saleAcc.Name;
            }
            ViewBag.Price = price;
            double? income = Double.Parse(price) - asset.PurchasePrice;
            int gain = 0;
            if (income < 0)
            {
                ViewBag.Gain = gain;
                income = asset.PurchasePrice - Double.Parse(price);
            }
            else
            {
                gain = 1;
                ViewBag.Gain = gain;
            }
            ViewBag.Income = income;
            if (gain == 1)
            {
                ViewBag.Debit = sum + Double.Parse(price);
                ViewBag.Credit = income + asset.PurchasePrice + sum;
            }
            else if (gain == 0)
            {
                double? loss = asset.PurchasePrice - sum - Double.Parse(price);
                ViewBag.Loss = loss;
                ViewBag.Debit = loss + sum + Double.Parse(price);
                ViewBag.Credit = asset.PurchasePrice;
            }
            return View(asset);
        }

        //Sold & Dispose item
        [HttpPost]
        public string SoldAndDispose(int? id, int gain, int reversal, int? totalCapitalID, int? gainLossID,
            string income, string dateSold, int? saleAccountID, string salePrice, string reversalAmount,
            string lossAmount, string depre, string currentDepre, int method, int type)
        {
            var user = TMT.ERP.Commons.AppContext.RequestVariables.CurrentUser;

            //Update status for item
            FixedAsset asset = db.FixedAssets.Find(id);
            asset.Status = 2;
            asset.ApprovedDate = DateTime.Parse(dateSold.ToString(CultureInfo.InvariantCulture));
            db.Entry(asset).State = EntityState.Modified;

            #region Depreciation Account

            if (Double.Parse(currentDepre) > 0 && Double.Parse(depre) > 0)
            {
                //Update for Depreciation Account
                Account depreAccount = db.Accounts.Find(asset.DepreciationAccountID);
                if (depreAccount.TotalCredit != null)
                {
                    depreAccount.TotalCredit += Double.Parse(currentDepre) - Double.Parse(depre);
                }
                else
                {
                    depreAccount.TotalCredit = Double.Parse(currentDepre) - Double.Parse(depre);
                }
                db.Entry(depreAccount).State = EntityState.Modified;

                //Create Account Trans
                AccountTran DepreTrans = new AccountTran();
                DepreTrans.AccountID = depreAccount.ID;
                DepreTrans.AccountType = 1;
                DepreTrans.Spent = Double.Parse(depre);
                DepreTrans.CompanyID = user.CompanyID;
                DepreTrans.Date = DateTime.Now;
                DepreTrans.SourceID = id;
                DepreTrans.SourceType = 4;
                DepreTrans.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                db.AccountTrans.Add(DepreTrans);
            }

            #endregion

            #region Transaction for Accummulated Account of item

            // Account Trans for Accummulated Account of item
            if (Double.Parse(currentDepre) > 0 && reversal == 2)
            {
                //Account Trans for the diffrent cash between Current Depreciation and last Depreciation of item before sell
                AccountTran tranAccumulated = new AccountTran();
                tranAccumulated.AccountID = asset.AccumulatedDepreciationAccountID;
                tranAccumulated.Spent = Double.Parse(currentDepre) - Double.Parse(depre);
                tranAccumulated.AccountType = 1;
                tranAccumulated.CompanyID = user.CompanyID;
                tranAccumulated.Date = DateTime.Now;
                tranAccumulated.SourceID = id;
                tranAccumulated.SourceType = 4;
                tranAccumulated.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                db.AccountTrans.Add(tranAccumulated);
            }
            if (type == 1)
            {
                //Account Trans for the last Depreciation of item before sell
                AccountTran accountDebit = new AccountTran();
                accountDebit.AccountID = asset.AccumulatedDepreciationAccountID;
                accountDebit.Spent = Double.Parse(depre);
                accountDebit.AccountType = 1;
                accountDebit.CompanyID = user.CompanyID;
                accountDebit.Date = DateTime.Now;
                accountDebit.SourceID = id;
                accountDebit.SourceType = 4;
                accountDebit.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                db.AccountTrans.Add(accountDebit);
            }

            #endregion

            #region Account Debit for Sale

            if (method == 0)
            {
                //Update Account Debit for sale
                Account saleAccount = db.Accounts.Find(saleAccountID);
                if (saleAccount.TotalDebit != null)
                {
                    saleAccount.TotalDebit += Double.Parse(salePrice);
                }
                else
                    saleAccount.TotalDebit = Double.Parse(salePrice);

                if (reversal == 2)
                {
                    if (saleAccount.TotalCredit != null)
                    {
                        saleAccount.TotalCredit += Double.Parse(reversalAmount);
                    }
                    else
                        saleAccount.TotalCredit = Double.Parse(reversalAmount);
                }
                db.Entry(saleAccount).State = EntityState.Modified;

                if (Double.Parse(depre) > 0)
                {
                    //Account Trans for Account Debit for sale
                    AccountTran tranDebit = new AccountTran();
                    tranDebit.AccountID = asset.AccumulatedDepreciationAccountID;
                    tranDebit.Spent = Double.Parse(depre);
                    tranDebit.AccountType = 1;
                    tranDebit.CompanyID = user.CompanyID;
                    tranDebit.Date = DateTime.Now;
                    tranDebit.SourceID = id;
                    tranDebit.SourceType = 4;
                    tranDebit.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                    db.AccountTrans.Add(tranDebit);
                }
            }
            #endregion

            #region Transaction & Update for Loss Account / Gain Account + Capital Account
            if (gain == 0)
            {
                //Update Loss Account
                Account lossAccount = db.Accounts.Find(gainLossID);
                if (lossAccount != null)
                {
                    if (lossAccount.TotalDebit != null)
                    {
                        lossAccount.TotalDebit += Double.Parse(lossAmount);
                    }
                    else
                        lossAccount.TotalDebit = Double.Parse(lossAmount);
                }
                db.Entry(lossAccount).State = EntityState.Modified;

                //Update to Account Trans
                AccountTran transLoss = new AccountTran();
                transLoss.AccountID = lossAccount.ID;
                transLoss.AccountType = 1;
                transLoss.CompanyID = user.CompanyID;
                transLoss.Date = DateTime.Now;
                transLoss.Spent = Double.Parse(lossAmount);
                transLoss.SourceID = id;
                transLoss.SourceType = 4;
                transLoss.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                db.AccountTrans.Add(transLoss);
            }
            else
            {
                //Update Gain Account
                Account gainAccount = db.Accounts.Find(gainLossID);
                if (gainAccount != null && Double.Parse(depre) > 0)
                {
                    if (gainAccount.TotalCredit != null)
                    {
                        gainAccount.TotalCredit += Double.Parse(depre);
                    }
                    else
                        gainAccount.TotalCredit = Double.Parse(depre);
                }
                db.Entry(gainAccount).State = EntityState.Modified;

                if (Double.Parse(depre) > 0)
                {
                    //Update to Account Trans
                    AccountTran transGain = new AccountTran();
                    transGain.AccountID = gainAccount.ID;
                    transGain.AccountType = 1;
                    transGain.CompanyID = user.CompanyID;
                    transGain.Date = DateTime.Now;
                    transGain.Received = Double.Parse(depre);
                    transGain.SourceID = id;
                    transGain.SourceType = 4;
                    transGain.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                    db.AccountTrans.Add(transGain);
                }

                //Update Total Gain Capital Account
                Account captailAccount = db.Accounts.Find(totalCapitalID);
                if (captailAccount != null)
                {
                    if (captailAccount.TotalCredit != null)
                    {
                        captailAccount.TotalCredit += Double.Parse(income);
                    }
                    else
                        captailAccount.TotalCredit = Double.Parse(income);
                }
                db.Entry(captailAccount).State = EntityState.Modified;

                if (Double.Parse(depre) > 0)
                {
                    //Update to Account Trans
                    AccountTran transCapital = new AccountTran();
                    transCapital.AccountID = captailAccount.ID;
                    transCapital.AccountType = 1;
                    transCapital.CompanyID = user.CompanyID;
                    transCapital.Date = DateTime.Now;
                    transCapital.Received = Double.Parse(depre);
                    transCapital.SourceID = id;
                    transCapital.SourceType = 4;
                    transCapital.TransactionName = "Disposal of " + asset.Code + " on " + dateSold;
                    db.AccountTrans.Add(transCapital);
                }
            }

            #endregion

            try
            {
                db.SaveChanges();
                result = " Sold ";
            }
            catch (Exception e)
            {
                result = " Error ";
            }

            return result;
        }

        //Starting Fixed Asset
        public ActionResult ChangeStartDate()
        {
            FixedAssetSetting setting = db.FixedAssetSettings.Find(1);
            ViewBag.CurrentYear = setting.Year;
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.NextYear = DateTime.Now.Year + 1;
            return View();
        }

        //
        // POST: /FixedAsset/Delete/5

        [HttpPost]
        public string DeleteConfirmed(string[] delConfirm, string tab)
        {
            foreach (var item in delConfirm)
            {
                int id = Int32.Parse(item);
                FixedAsset asset = db.FixedAssets.Find(id);
                try
                {
                    db.FixedAssets.Remove(asset);
                    db.SaveChanges();
                    result = "Success";
                }
                catch (Exception e)
                {
                    result = e.ToString();
                }
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
