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
    
    public partial class AccountFeature : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _featurename;
        public string FeatureName { get { return _featurename; } set { SetNotifyingProperty(ref _featurename, value); } }
    	private int _type;
        public int Type { get { return _type; } set { SetNotifyingProperty(ref _type, value); } }
    	private Nullable<int> _accountid;
        public Nullable<int> AccountID { get { return _accountid; } set { SetNotifyingProperty(ref _accountid, value); } }
    
    	private Account _account;
        public virtual Account Account { get { return _account; } set { SetNotifyingProperty(ref _account, value); } }
    }
}
