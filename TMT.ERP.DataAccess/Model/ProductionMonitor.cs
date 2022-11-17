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
    
    public partial class ProductionMonitor : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private string _code;
        public string Code { get { return _code; } set { SetNotifyingProperty(ref _code, value); } }
    	private int _workcenterid;
        public int WorkCenterID { get { return _workcenterid; } set { SetNotifyingProperty(ref _workcenterid, value); } }
    	private int _machineid;
        public int MachineID { get { return _machineid; } set { SetNotifyingProperty(ref _machineid, value); } }
    	private int _allocwoid;
        public int AllocWOID { get { return _allocwoid; } set { SetNotifyingProperty(ref _allocwoid, value); } }
    	private Nullable<System.DateTime> _schedulestarttime;
        public Nullable<System.DateTime> ScheduleStartTime { get { return _schedulestarttime; } set { SetNotifyingProperty(ref _schedulestarttime, value); } }
    	private Nullable<System.DateTime> _scheduleendtime;
        public Nullable<System.DateTime> ScheduleEndTime { get { return _scheduleendtime; } set { SetNotifyingProperty(ref _scheduleendtime, value); } }
    	private Nullable<System.DateTime> _realstarttime;
        public Nullable<System.DateTime> RealStartTime { get { return _realstarttime; } set { SetNotifyingProperty(ref _realstarttime, value); } }
    	private Nullable<System.DateTime> _endstarttime;
        public Nullable<System.DateTime> EndStartTime { get { return _endstarttime; } set { SetNotifyingProperty(ref _endstarttime, value); } }
    	private Nullable<int> _processtime;
        public Nullable<int> ProcessTime { get { return _processtime; } set { SetNotifyingProperty(ref _processtime, value); } }
    	private int _status;
        public int Status { get { return _status; } set { SetNotifyingProperty(ref _status, value); } }
    	private Nullable<int> _goodid;
        public Nullable<int> GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    	private Nullable<int> _orderquantity;
        public Nullable<int> OrderQuantity { get { return _orderquantity; } set { SetNotifyingProperty(ref _orderquantity, value); } }
    	private Nullable<int> _implementquantity;
        public Nullable<int> ImplementQuantity { get { return _implementquantity; } set { SetNotifyingProperty(ref _implementquantity, value); } }
    	private Nullable<int> _remainquantity;
        public Nullable<int> RemainQuantity { get { return _remainquantity; } set { SetNotifyingProperty(ref _remainquantity, value); } }
    	private Nullable<System.DateTime> _implementdate;
        public Nullable<System.DateTime> ImplementDate { get { return _implementdate; } set { SetNotifyingProperty(ref _implementdate, value); } }
    	private Nullable<int> _companyid;
        public Nullable<int> CompanyID { get { return _companyid; } set { SetNotifyingProperty(ref _companyid, value); } }
    
    	private Machine _machine;
        public virtual Machine Machine { get { return _machine; } set { SetNotifyingProperty(ref _machine, value); } }
    	private WorkCenter _workcenter;
        public virtual WorkCenter WorkCenter { get { return _workcenter; } set { SetNotifyingProperty(ref _workcenter, value); } }
    	private Company _company;
        public virtual Company Company { get { return _company; } set { SetNotifyingProperty(ref _company, value); } }
    	private AllocWO _allocwo;
        public virtual AllocWO AllocWO { get { return _allocwo; } set { SetNotifyingProperty(ref _allocwo, value); } }
    }
}
