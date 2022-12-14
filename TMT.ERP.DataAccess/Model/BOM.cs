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
    
    public partial class BOM : Entity
    {
        public BOM()
        {
            this.BomDetails = new HashSet<BomDetail>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _name;
        public string Name { get { return _name; } set { SetNotifyingProperty(ref _name, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    	private Nullable<System.DateTime> _effectivedate;
        public Nullable<System.DateTime> EffectiveDate { get { return _effectivedate; } set { SetNotifyingProperty(ref _effectivedate, value); } }
    	private Nullable<bool> _active;
        public Nullable<bool> Active { get { return _active; } set { SetNotifyingProperty(ref _active, value); } }
    	private Nullable<int> _goodid;
        public Nullable<int> GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    
    	private ICollection<BomDetail> _bomdetails;
        public virtual ICollection<BomDetail> BomDetails { get { return _bomdetails; } set { SetNotifyingProperty(ref _bomdetails, value); } }
    	private Good _good;
        public virtual Good Good { get { return _good; } set { SetNotifyingProperty(ref _good, value); } }
    }
}
