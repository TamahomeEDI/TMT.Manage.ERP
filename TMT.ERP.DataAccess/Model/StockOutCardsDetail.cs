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
    
    public partial class StockOutCardsDetail : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private int _stockoutcardid;
        public int StockOutCardID { get { return _stockoutcardid; } set { SetNotifyingProperty(ref _stockoutcardid, value); } }
    	private int _goodid;
        public int GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    	private Nullable<int> _uomid;
        public Nullable<int> UOMID { get { return _uomid; } set { SetNotifyingProperty(ref _uomid, value); } }
    	private int _quantity;
        public int Quantity { get { return _quantity; } set { SetNotifyingProperty(ref _quantity, value); } }
    	private Nullable<System.DateTime> _dateout;
        public Nullable<System.DateTime> DateOut { get { return _dateout; } set { SetNotifyingProperty(ref _dateout, value); } }
    	private Nullable<int> _quantityref;
        public Nullable<int> QuantityRef { get { return _quantityref; } set { SetNotifyingProperty(ref _quantityref, value); } }
    	private Nullable<double> _discount;
        public Nullable<double> Discount { get { return _discount; } set { SetNotifyingProperty(ref _discount, value); } }
    	private Nullable<double> _price;
        public Nullable<double> Price { get { return _price; } set { SetNotifyingProperty(ref _price, value); } }
    	private Nullable<double> _totalmoney;
        public Nullable<double> TotalMoney { get { return _totalmoney; } set { SetNotifyingProperty(ref _totalmoney, value); } }
    	private Nullable<int> _remainquantity;
        public Nullable<int> RemainQuantity { get { return _remainquantity; } set { SetNotifyingProperty(ref _remainquantity, value); } }
    
    	private UOM _uom;
        public virtual UOM UOM { get { return _uom; } set { SetNotifyingProperty(ref _uom, value); } }
    	private StockOutCard _stockoutcard;
        public virtual StockOutCard StockOutCard { get { return _stockoutcard; } set { SetNotifyingProperty(ref _stockoutcard, value); } }
    	private Good _good;
        public virtual Good Good { get { return _good; } set { SetNotifyingProperty(ref _good, value); } }
    }
}
