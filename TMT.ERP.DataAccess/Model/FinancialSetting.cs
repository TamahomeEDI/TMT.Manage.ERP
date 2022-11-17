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
    
    public partial class FinancialSetting : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private int _financialyeday;
        public int FinancialYEDay { get { return _financialyeday; } set { SetNotifyingProperty(ref _financialyeday, value); } }
    	private System.DateTime _financialyemonth;
        public System.DateTime FinancialYEMonth { get { return _financialyemonth; } set { SetNotifyingProperty(ref _financialyemonth, value); } }
    	private Nullable<int> _saletaxbasicid;
        public Nullable<int> SaleTaxBasicID { get { return _saletaxbasicid; } set { SetNotifyingProperty(ref _saletaxbasicid, value); } }
    	private string _taxidnumber;
        public string TaxIDNumber { get { return _taxidnumber; } set { SetNotifyingProperty(ref _taxidnumber, value); } }
    	private string _taxiddisplayname;
        public string TaxIDDisplayName { get { return _taxiddisplayname; } set { SetNotifyingProperty(ref _taxiddisplayname, value); } }
    	private string _taxperiod;
        public string TaxPeriod { get { return _taxperiod; } set { SetNotifyingProperty(ref _taxperiod, value); } }
    	private Nullable<System.DateTime> _lockdate;
        public Nullable<System.DateTime> LockDate { get { return _lockdate; } set { SetNotifyingProperty(ref _lockdate, value); } }
    	private string _timezone;
        public string TimeZone { get { return _timezone; } set { SetNotifyingProperty(ref _timezone, value); } }
    }
}
