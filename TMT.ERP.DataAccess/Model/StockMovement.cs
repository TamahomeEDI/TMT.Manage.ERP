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
    
    public partial class StockMovement : Entity
    {
        public StockMovement()
        {
            this.StockMovementDetails = new HashSet<StockMovementDetail>();
            this.StockMovementTemps = new HashSet<StockMovementTemp>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _code;
        public string Code { get { return _code; } set { SetNotifyingProperty(ref _code, value); } }
    	private System.DateTime _createddate;
        public System.DateTime CreatedDate { get { return _createddate; } set { SetNotifyingProperty(ref _createddate, value); } }
    	private int _createduserid;
        public int CreatedUserID { get { return _createduserid; } set { SetNotifyingProperty(ref _createduserid, value); } }
    	private Nullable<int> _approvaluserid;
        public Nullable<int> ApprovalUserID { get { return _approvaluserid; } set { SetNotifyingProperty(ref _approvaluserid, value); } }
    	private int _fromstockid;
        public int FromStockID { get { return _fromstockid; } set { SetNotifyingProperty(ref _fromstockid, value); } }
    	private int _tostockid;
        public int ToStockID { get { return _tostockid; } set { SetNotifyingProperty(ref _tostockid, value); } }
    	private Nullable<int> _status;
        public Nullable<int> Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    	private Nullable<int> _fromaccountid;
        public Nullable<int> FromAccountID { get { return _fromaccountid; } set { SetNotifyingProperty(ref _fromaccountid, value); } }
    	private Nullable<int> _toaccountid;
        public Nullable<int> ToAccountID { get { return _toaccountid; } set { SetNotifyingProperty(ref _toaccountid, value); } }
    	private Nullable<int> _companyid;
        public Nullable<int> CompanyID { get { return _companyid; } set { SetNotifyingProperty(ref _companyid, value); } }
    	private Nullable<System.DateTime> _approvaldate;
        public Nullable<System.DateTime> ApprovalDate { get { return _approvaldate; } set { SetNotifyingProperty(ref _approvaldate, value); } }
    
    	private Company _company;
        public virtual Company Company { get { return _company; } set { SetNotifyingProperty(ref _company, value); } }
    	private ICollection<StockMovementDetail> _stockmovementdetails;
        public virtual ICollection<StockMovementDetail> StockMovementDetails { get { return _stockmovementdetails; } set { SetNotifyingProperty(ref _stockmovementdetails, value); } }
    	private Stock _stock;
        public virtual Stock Stock { get { return _stock; } set { SetNotifyingProperty(ref _stock, value); } }
    	private Stock _stock1;
        public virtual Stock Stock1 { get { return _stock1; } set { SetNotifyingProperty(ref _stock1, value); } }
    	private User _user;
        public virtual User User { get { return _user; } set { SetNotifyingProperty(ref _user, value); } }
    	private User _user1;
        public virtual User User1 { get { return _user1; } set { SetNotifyingProperty(ref _user1, value); } }
    	private ICollection<StockMovementTemp> _stockmovementtemps;
        public virtual ICollection<StockMovementTemp> StockMovementTemps { get { return _stockmovementtemps; } set { SetNotifyingProperty(ref _stockmovementtemps, value); } }
    }
}
