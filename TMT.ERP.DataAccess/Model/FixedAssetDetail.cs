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
    
    public partial class FixedAssetDetail : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private Nullable<int> _fixedassetid;
        public Nullable<int> FixedAssetID { get { return _fixedassetid; } set { SetNotifyingProperty(ref _fixedassetid, value); } }
    	private string _item;
        public string Item { get { return _item; } set { SetNotifyingProperty(ref _item, value); } }
    	private string _code;
        public string Code { get { return _code; } set { SetNotifyingProperty(ref _code, value); } }
    	private Nullable<int> _accountid;
        public Nullable<int> AccountID { get { return _accountid; } set { SetNotifyingProperty(ref _accountid, value); } }
    	private Nullable<System.DateTime> _purchasedate;
        public Nullable<System.DateTime> PurchaseDate { get { return _purchasedate; } set { SetNotifyingProperty(ref _purchasedate, value); } }
    	private Nullable<double> _purchaseprice;
        public Nullable<double> PurchasePrice { get { return _purchaseprice; } set { SetNotifyingProperty(ref _purchaseprice, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    	private string _assettype;
        public string AssetType { get { return _assettype; } set { SetNotifyingProperty(ref _assettype, value); } }
    	private Nullable<double> _depreciationrate;
        public Nullable<double> DepreciationRate { get { return _depreciationrate; } set { SetNotifyingProperty(ref _depreciationrate, value); } }
    	private string _depreciationmethod;
        public string DepreciationMethod { get { return _depreciationmethod; } set { SetNotifyingProperty(ref _depreciationmethod, value); } }
    	private Nullable<int> _depreciationaccountid;
        public Nullable<int> DepreciationAccountID { get { return _depreciationaccountid; } set { SetNotifyingProperty(ref _depreciationaccountid, value); } }
    	private Nullable<int> _createdemployeeid;
        public Nullable<int> CreatedEmployeeID { get { return _createdemployeeid; } set { SetNotifyingProperty(ref _createdemployeeid, value); } }
    	private Nullable<int> _approvalemployeeid;
        public Nullable<int> ApprovalEmployeeID { get { return _approvalemployeeid; } set { SetNotifyingProperty(ref _approvalemployeeid, value); } }
    
    	private FixedAsset _fixedasset;
        public virtual FixedAsset FixedAsset { get { return _fixedasset; } set { SetNotifyingProperty(ref _fixedasset, value); } }
    }
}
