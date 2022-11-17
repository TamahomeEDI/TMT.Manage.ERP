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
    
    public partial class SubFunction : Entity
    {
        public SubFunction()
        {
            this.RoleFunctions = new HashSet<RoleFunction>();
        }
    
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private Nullable<int> _functionid;
        public Nullable<int> FunctionID { get { return _functionid; } set { SetNotifyingProperty(ref _functionid, value); } }
    	private string _name;
        public string Name { get { return _name; } set { SetNotifyingProperty(ref _name, value); } }
    	private string _description;
        public string Description { get { return _description; } set { SetNotifyingProperty(ref _description, value); } }
    
    	private Function _function;
        public virtual Function Function { get { return _function; } set { SetNotifyingProperty(ref _function, value); } }
    	private ICollection<RoleFunction> _rolefunctions;
        public virtual ICollection<RoleFunction> RoleFunctions { get { return _rolefunctions; } set { SetNotifyingProperty(ref _rolefunctions, value); } }
    }
}
