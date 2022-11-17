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
    
    public partial class BankStatement : Entity
    {
        public BankStatement()
        {
            this.BankStatementDetails = new HashSet<BankStatementDetail>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private int _bankaccountid;
        public int BankAccountID { get { return _bankaccountid; } set { SetNotifyingProperty(ref _bankaccountid, value); } }
    	private System.DateTime _importeddate;
        public System.DateTime ImportedDate { get { return _importeddate; } set { SetNotifyingProperty(ref _importeddate, value); } }
    	private System.DateTime _startdate;
        public System.DateTime StartDate { get { return _startdate; } set { SetNotifyingProperty(ref _startdate, value); } }
    	private System.DateTime _enddate;
        public System.DateTime EndDate { get { return _enddate; } set { SetNotifyingProperty(ref _enddate, value); } }
    	private Nullable<double> _startbalance;
        public Nullable<double> StartBalance { get { return _startbalance; } set { SetNotifyingProperty(ref _startbalance, value); } }
    	private Nullable<double> _endbalance;
        public Nullable<double> EndBalance { get { return _endbalance; } set { SetNotifyingProperty(ref _endbalance, value); } }
    	private int _status;
        public int Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    
    	private ICollection<BankStatementDetail> _bankstatementdetails;
        public virtual ICollection<BankStatementDetail> BankStatementDetails { get { return _bankstatementdetails; } set { SetNotifyingProperty(ref _bankstatementdetails, value); } }
    	private BankAccount _bankaccount;
        public virtual BankAccount BankAccount { get { return _bankaccount; } set { SetNotifyingProperty(ref _bankaccount, value); } }
    }
}