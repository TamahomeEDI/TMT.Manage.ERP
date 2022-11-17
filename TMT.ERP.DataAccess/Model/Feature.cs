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
    
    public partial class Feature : Entity
    {
        public Feature()
        {
            this.RolePermissions = new HashSet<RolePermission>();
            this.Functions = new HashSet<Function>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _featurename;
        public string FeatureName { get { return _featurename; } set { SetNotifyingProperty(ref _featurename, value); } }
    
    	private ICollection<RolePermission> _rolepermissions;
        public virtual ICollection<RolePermission> RolePermissions { get { return _rolepermissions; } set { SetNotifyingProperty(ref _rolepermissions, value); } }
    	private ICollection<Function> _functions;
        public virtual ICollection<Function> Functions { get { return _functions; } set { SetNotifyingProperty(ref _functions, value); } }
    }
}
