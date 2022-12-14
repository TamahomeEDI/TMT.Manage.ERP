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
    
    public partial class StockOutCard : Entity
    {
        public StockOutCard()
        {
            this.StockOutCardsDetails = new HashSet<StockOutCardsDetail>();
            this.AllocWOes = new HashSet<AllocWO>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _code;
        public string Code { get { return _code; } set { SetNotifyingProperty(ref _code, value); } }
    	private int _createduserid;
        public int CreatedUserID { get { return _createduserid; } set { SetNotifyingProperty(ref _createduserid, value); } }
    	private Nullable<int> _approvaluserid;
        public Nullable<int> ApprovalUserID { get { return _approvaluserid; } set { SetNotifyingProperty(ref _approvaluserid, value); } }
    	private int _stockid;
        public int StockID { get { return _stockid; } set { SetNotifyingProperty(ref _stockid, value); } }
    	private System.DateTime _createddate;
        public System.DateTime CreatedDate { get { return _createddate; } set { SetNotifyingProperty(ref _createddate, value); } }
    	private Nullable<int> _targetid;
        public Nullable<int> TargetID { get { return _targetid; } set { SetNotifyingProperty(ref _targetid, value); } }
    	private int _type;
        public int Type { get { return _type; } set { SetNotifyingProperty(ref _type, value); } }
    	private string _receiver;
        public string Receiver { get { return _receiver; } set { SetNotifyingProperty(ref _receiver, value); } }
    	private Nullable<double> _tax;
        public Nullable<double> Tax { get { return _tax; } set { SetNotifyingProperty(ref _tax, value); } }
    	private Nullable<double> _totalmoney;
        public Nullable<double> TotalMoney { get { return _totalmoney; } set { SetNotifyingProperty(ref _totalmoney, value); } }
    	private Nullable<int> _status;
        public Nullable<int> Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    	private string _reason;
        public string Reason { get { return _reason; } set { SetNotifyingProperty(ref _reason, value); } }
    	private Nullable<int> _companyid;
        public Nullable<int> CompanyID { get { return _companyid; } set { SetNotifyingProperty(ref _companyid, value); } }
    	private Nullable<System.DateTime> _approvaldate;
        public Nullable<System.DateTime> ApprovalDate { get { return _approvaldate; } set { SetNotifyingProperty(ref _approvaldate, value); } }
    
    	private User _user;
        public virtual User User { get { return _user; } set { SetNotifyingProperty(ref _user, value); } }
    	private User _user1;
        public virtual User User1 { get { return _user1; } set { SetNotifyingProperty(ref _user1, value); } }
    	private ICollection<StockOutCardsDetail> _stockoutcardsdetails;
        public virtual ICollection<StockOutCardsDetail> StockOutCardsDetails { get { return _stockoutcardsdetails; } set { SetNotifyingProperty(ref _stockoutcardsdetails, value); } }
    	private Company _company;
        public virtual Company Company { get { return _company; } set { SetNotifyingProperty(ref _company, value); } }
    	private Stock _stock;
        public virtual Stock Stock { get { return _stock; } set { SetNotifyingProperty(ref _stock, value); } }
    	private ICollection<AllocWO> _allocwoes;
        public virtual ICollection<AllocWO> AllocWOes { get { return _allocwoes; } set { SetNotifyingProperty(ref _allocwoes, value); } }
    }
}
