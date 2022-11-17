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
    
    public partial class StockMovementTemp : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private Nullable<int> _stockmovementid;
        public Nullable<int> StockMovementID { get { return _stockmovementid; } set { SetNotifyingProperty(ref _stockmovementid, value); } }
    	private Nullable<int> _fromstockid;
        public Nullable<int> FromStockID { get { return _fromstockid; } set { SetNotifyingProperty(ref _fromstockid, value); } }
    	private Nullable<int> _goodid;
        public Nullable<int> GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    	private Nullable<System.DateTime> _datein;
        public Nullable<System.DateTime> DateIn { get { return _datein; } set { SetNotifyingProperty(ref _datein, value); } }
    	private Nullable<System.DateTime> _updateddate;
        public Nullable<System.DateTime> UpdatedDate { get { return _updateddate; } set { SetNotifyingProperty(ref _updateddate, value); } }
    	private Nullable<int> _uomid;
        public Nullable<int> UOMID { get { return _uomid; } set { SetNotifyingProperty(ref _uomid, value); } }
    	private Nullable<int> _quantity;
        public Nullable<int> Quantity { get { return _quantity; } set { SetNotifyingProperty(ref _quantity, value); } }
    	private Nullable<double> _inputprice;
        public Nullable<double> InputPrice { get { return _inputprice; } set { SetNotifyingProperty(ref _inputprice, value); } }
    
    	private StockMovement _stockmovement;
        public virtual StockMovement StockMovement { get { return _stockmovement; } set { SetNotifyingProperty(ref _stockmovement, value); } }
    }
}