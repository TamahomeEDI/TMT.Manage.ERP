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
    
    public partial class BomDetail : Entity
    {
        public BomDetail()
        {
            this.BomDetails1 = new HashSet<BomDetail>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private Nullable<int> _bomid;
        public Nullable<int> BomID { get { return _bomid; } set { SetNotifyingProperty(ref _bomid, value); } }
    	private Nullable<int> _goodid;
        public Nullable<int> GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    	private Nullable<int> _parentgoodid;
        public Nullable<int> ParentGoodID { get { return _parentgoodid; } set { SetNotifyingProperty(ref _parentgoodid, value); } }
    	private Nullable<int> _parentbomid;
        public Nullable<int> ParentBomID { get { return _parentbomid; } set { SetNotifyingProperty(ref _parentbomid, value); } }
    	private Nullable<int> _quantity;
        public Nullable<int> Quantity { get { return _quantity; } set { SetNotifyingProperty(ref _quantity, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    
    	private ICollection<BomDetail> _bomdetails1;
        public virtual ICollection<BomDetail> BomDetails1 { get { return _bomdetails1; } set { SetNotifyingProperty(ref _bomdetails1, value); } }
    	private BomDetail _bomdetail1;
        public virtual BomDetail BomDetail1 { get { return _bomdetail1; } set { SetNotifyingProperty(ref _bomdetail1, value); } }
    	private BOM _bom;
        public virtual BOM BOM { get { return _bom; } set { SetNotifyingProperty(ref _bom, value); } }
    	private Good _good;
        public virtual Good Good { get { return _good; } set { SetNotifyingProperty(ref _good, value); } }
    	private Good _good1;
        public virtual Good Good1 { get { return _good1; } set { SetNotifyingProperty(ref _good1, value); } }
    }
}
