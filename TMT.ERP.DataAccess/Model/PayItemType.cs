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
    
    public partial class PayItemType : Entity
    {
        public PayItemType()
        {
            this.PayItems = new HashSet<PayItem>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _name;
        public string Name { get { return _name; } set { SetNotifyingProperty(ref _name, value); } }
    	private Nullable<int> _status;
        public Nullable<int> Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    
    	private ICollection<PayItem> _payitems;
        public virtual ICollection<PayItem> PayItems { get { return _payitems; } set { SetNotifyingProperty(ref _payitems, value); } }
    }
}
