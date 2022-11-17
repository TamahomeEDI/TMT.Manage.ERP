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
    
    public partial class StockInCardsDetail : Entity
    {
    	private int _id;
        public int ID { get { return _id; } set { SetNotifyingProperty(ref _id, value); } }
    	private int _stockincardid;
        public int StockInCardID { get { return _stockincardid; } set { SetNotifyingProperty(ref _stockincardid, value); } }
    	private int _goodid;
        public int GoodID { get { return _goodid; } set { SetNotifyingProperty(ref _goodid, value); } }
    	private int _uomid;
        public int UOMID { get { return _uomid; } set { SetNotifyingProperty(ref _uomid, value); } }
    	private Nullable<int> _quantity;
        public Nullable<int> Quantity { get { return _quantity; } set { SetNotifyingProperty(ref _quantity, value); } }
    	private System.DateTime _datein;
        public System.DateTime DateIn { get { return _datein; } set { SetNotifyingProperty(ref _datein, value); } }
    	private Nullable<int> _quantityreq;
        public Nullable<int> QuantityReq { get { return _quantityreq; } set { SetNotifyingProperty(ref _quantityreq, value); } }
    	private Nullable<double> _discount;
        public Nullable<double> Discount { get { return _discount; } set { SetNotifyingProperty(ref _discount, value); } }
    	private Nullable<double> _price;
        public Nullable<double> Price { get { return _price; } set { SetNotifyingProperty(ref _price, value); } }
    	private Nullable<double> _totalmoney;
        public Nullable<double> TotalMoney { get { return _totalmoney; } set { SetNotifyingProperty(ref _totalmoney, value); } }
    
    	private UOM _uom;
        public virtual UOM UOM { get { return _uom; } set { SetNotifyingProperty(ref _uom, value); } }
    	private Good _good;
        public virtual Good Good { get { return _good; } set { SetNotifyingProperty(ref _good, value); } }
    	private StockInCard _stockincard;
        public virtual StockInCard StockInCard { get { return _stockincard; } set { SetNotifyingProperty(ref _stockincard, value); } }
    }
}
