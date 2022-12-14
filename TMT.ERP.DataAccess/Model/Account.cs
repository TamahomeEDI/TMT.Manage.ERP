//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TMT.ERP.DataAccess.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Account : Entity
    {
        public Account()
        {
            this.AccountBalances = new HashSet<AccountBalance>();
            this.AccountBudgetTrans = new HashSet<AccountBudgetTran>();
            this.AccountFeatures = new HashSet<AccountFeature>();
            this.AccountGoodTrans = new HashSet<AccountGoodTran>();
            this.AccountInGroups = new HashSet<AccountInGroup>();
            this.AccountTranDetails = new HashSet<AccountTranDetail>();
            this.AccountTrans = new HashSet<AccountTran>();
            this.BankAccountDetails = new HashSet<BankAccountDetail>();
            this.ExpenseClaimDetails = new HashSet<ExpenseClaimDetail>();
            this.FixedAssets = new HashSet<FixedAsset>();
            this.FixedAssets1 = new HashSet<FixedAsset>();
            this.FixedAssets2 = new HashSet<FixedAsset>();
            this.Goods = new HashSet<Good>();
            this.Goods1 = new HashSet<Good>();
            this.JournalDetails = new HashSet<JournalDetail>();
            this.PayItems = new HashSet<PayItem>();
            this.PurchaseDetails = new HashSet<PurchaseDetail>();
            this.SaleInvoiceDetails = new HashSet<SaleInvoiceDetail>();
            this.SaleInvoiceDetails1 = new HashSet<SaleInvoiceDetail>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _code;
        public string Code { get { return _code; } set { SetNotifyingProperty(ref _code, value); } }
    	private string _name;
        public string Name { get { return _name; } set { SetNotifyingProperty(ref _name, value); } }
    	private int _accounttypeid;
        public int AccountTypeID { get { return _accounttypeid; } set { SetNotifyingProperty(ref _accounttypeid, value); } }
    	private int _status;
        public int Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    	private int _taxrateid;
        public int TaxRateID { get { return _taxrateid; } set { SetNotifyingProperty(ref _taxrateid, value); } }
    	private Nullable<double> _totaldebit;
        public Nullable<double> TotalDebit { get { return _totaldebit; } set { SetNotifyingProperty(ref _totaldebit, value); } }
    	private Nullable<double> _totalcredit;
        public Nullable<double> TotalCredit { get { return _totalcredit; } set { SetNotifyingProperty(ref _totalcredit, value); } }
    	private Nullable<double> _ytd;
        public Nullable<double> YTD { get { return _ytd; } set { SetNotifyingProperty(ref _ytd, value); } }
    	private Nullable<int> _payment;
        public Nullable<int> Payment { get { return _payment; } set { SetNotifyingProperty(ref _payment, value); } }
    	private Nullable<int> _payroll;
        public Nullable<int> Payroll { get { return _payroll; } set { SetNotifyingProperty(ref _payroll, value); } }
    	private Nullable<int> _expenseclaim;
        public Nullable<int> ExpenseClaim { get { return _expenseclaim; } set { SetNotifyingProperty(ref _expenseclaim, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    	private Nullable<bool> _showondashboard;
        public Nullable<bool> ShowOnDashboard { get { return _showondashboard; } set { SetNotifyingProperty(ref _showondashboard, value); } }
    	private Nullable<bool> _showinexpenseclaims;
        public Nullable<bool> ShowInExpenseClaims { get { return _showinexpenseclaims; } set { SetNotifyingProperty(ref _showinexpenseclaims, value); } }
    	private Nullable<bool> _enablepayments;
        public Nullable<bool> EnablePayments { get { return _enablepayments; } set { SetNotifyingProperty(ref _enablepayments, value); } }
    
    	private ICollection<AccountBalance> _accountbalances;
        public virtual ICollection<AccountBalance> AccountBalances { get { return _accountbalances; } set { SetNotifyingProperty(ref _accountbalances, value); } }
    	private ICollection<AccountBudgetTran> _accountbudgettrans;
        public virtual ICollection<AccountBudgetTran> AccountBudgetTrans { get { return _accountbudgettrans; } set { SetNotifyingProperty(ref _accountbudgettrans, value); } }
    	private ICollection<AccountFeature> _accountfeatures;
        public virtual ICollection<AccountFeature> AccountFeatures { get { return _accountfeatures; } set { SetNotifyingProperty(ref _accountfeatures, value); } }
    	private ICollection<AccountGoodTran> _accountgoodtrans;
        public virtual ICollection<AccountGoodTran> AccountGoodTrans { get { return _accountgoodtrans; } set { SetNotifyingProperty(ref _accountgoodtrans, value); } }
    	private ICollection<AccountInGroup> _accountingroups;
        public virtual ICollection<AccountInGroup> AccountInGroups { get { return _accountingroups; } set { SetNotifyingProperty(ref _accountingroups, value); } }
    	private AccountType _accounttype;
        public virtual AccountType AccountType { get { return _accounttype; } set { SetNotifyingProperty(ref _accounttype, value); } }
    	private TaxRate _taxrate;
        public virtual TaxRate TaxRate { get { return _taxrate; } set { SetNotifyingProperty(ref _taxrate, value); } }
    	private ICollection<AccountTranDetail> _accounttrandetails;
        public virtual ICollection<AccountTranDetail> AccountTranDetails { get { return _accounttrandetails; } set { SetNotifyingProperty(ref _accounttrandetails, value); } }
    	private ICollection<AccountTran> _accounttrans;
        public virtual ICollection<AccountTran> AccountTrans { get { return _accounttrans; } set { SetNotifyingProperty(ref _accounttrans, value); } }
    	private ICollection<BankAccountDetail> _bankaccountdetails;
        public virtual ICollection<BankAccountDetail> BankAccountDetails { get { return _bankaccountdetails; } set { SetNotifyingProperty(ref _bankaccountdetails, value); } }
    	private ICollection<ExpenseClaimDetail> _expenseclaimdetails;
        public virtual ICollection<ExpenseClaimDetail> ExpenseClaimDetails { get { return _expenseclaimdetails; } set { SetNotifyingProperty(ref _expenseclaimdetails, value); } }
    	private ICollection<FixedAsset> _fixedassets;
        public virtual ICollection<FixedAsset> FixedAssets { get { return _fixedassets; } set { SetNotifyingProperty(ref _fixedassets, value); } }
    	private ICollection<FixedAsset> _fixedassets1;
        public virtual ICollection<FixedAsset> FixedAssets1 { get { return _fixedassets1; } set { SetNotifyingProperty(ref _fixedassets1, value); } }
    	private ICollection<FixedAsset> _fixedassets2;
        public virtual ICollection<FixedAsset> FixedAssets2 { get { return _fixedassets2; } set { SetNotifyingProperty(ref _fixedassets2, value); } }
    	private ICollection<Good> _goods;
        public virtual ICollection<Good> Goods { get { return _goods; } set { SetNotifyingProperty(ref _goods, value); } }
    	private ICollection<Good> _goods1;
        public virtual ICollection<Good> Goods1 { get { return _goods1; } set { SetNotifyingProperty(ref _goods1, value); } }
    	private ICollection<JournalDetail> _journaldetails;
        public virtual ICollection<JournalDetail> JournalDetails { get { return _journaldetails; } set { SetNotifyingProperty(ref _journaldetails, value); } }
    	private ICollection<PayItem> _payitems;
        public virtual ICollection<PayItem> PayItems { get { return _payitems; } set { SetNotifyingProperty(ref _payitems, value); } }
    	private ICollection<PurchaseDetail> _purchasedetails;
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get { return _purchasedetails; } set { SetNotifyingProperty(ref _purchasedetails, value); } }
    	private ICollection<SaleInvoiceDetail> _saleinvoicedetails;
        public virtual ICollection<SaleInvoiceDetail> SaleInvoiceDetails { get { return _saleinvoicedetails; } set { SetNotifyingProperty(ref _saleinvoicedetails, value); } }
    	private ICollection<SaleInvoiceDetail> _saleinvoicedetails1;
        public virtual ICollection<SaleInvoiceDetail> SaleInvoiceDetails1 { get { return _saleinvoicedetails1; } set { SetNotifyingProperty(ref _saleinvoicedetails1, value); } }
    }
}
