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
    
    public partial class FixedAssetSetting : Entity
    {
        public FixedAssetSetting()
        {
            this.FixedAssets = new HashSet<FixedAsset>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private Nullable<bool> _iscurrent;
        public Nullable<bool> IsCurrent { get { return _iscurrent; } set { SetNotifyingProperty(ref _iscurrent, value); } }
    	private int _year;
        public int Year { get { return _year; } set { SetNotifyingProperty(ref _year, value); } }
    
    	private ICollection<FixedAsset> _fixedassets;
        public virtual ICollection<FixedAsset> FixedAssets { get { return _fixedassets; } set { SetNotifyingProperty(ref _fixedassets, value); } }
    }
}
