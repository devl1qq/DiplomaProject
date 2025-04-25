using PX.Data;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.CM;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

namespace MainProject.Helper
{
    [PXProjection(typeof(Select2<PX.Objects.IN.InventoryItem,
       LeftJoin<INSiteStatus, On<INSiteStatus.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>, And<PX.Objects.IN.InventoryItem.stkItem, Equal<boolTrue>, And<INSiteStatus.siteID, NotEqual<SiteAttribute.transitSiteID>>>>,
           LeftJoin<INSubItem, On<INSiteStatus.FK.SubItem>,
               LeftJoin<INSite, On2<INSiteStatus.FK.Site, And<INSite.baseCuryID, EqualBaseCuryID<Current2<CRQuote.curyID>>>>,
                   LeftJoin<INItemXRef, On<INItemXRef.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>, And2<Where<INItemXRef.subItemID, Equal<INSiteStatus.subItemID>, Or<INSiteStatus.subItemID, PX.Data.IsNull>>, And<Where<CurrentValue<INSiteStatusFilter.barCode>, PX.Data.IsNotNull, And<INItemXRef.alternateType, In3<INAlternateType.barcode, INAlternateType.gIN>>>>>>,
                       LeftJoin<INItemPartNumber, On<INItemPartNumber.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>, And<INItemPartNumber.alternateID, Like<CurrentValue<INSiteStatusFilter.inventory_Wildcard>>, And2<Where<INItemPartNumber.bAccountID, Equal<PX.Data.Zero>, Or<INItemPartNumber.bAccountID, Equal<CurrentValue<CRQuote.bAccountID>>, Or<INItemPartNumber.alternateType, Equal<INAlternateType.vPN>>>>, And<Where<INItemPartNumber.subItemID, Equal<INSiteStatus.subItemID>, Or<INSiteStatus.subItemID, PX.Data.IsNull>>>>>>,
                           LeftJoin<INItemClass, On<PX.Objects.IN.InventoryItem.FK.ItemClass>,
                               LeftJoin<INPriceClass, On<INPriceClass.priceClassID, Equal<PX.Objects.IN.InventoryItem.priceClassID>>,
                                   LeftJoin<InventoryItemCurySettings, On<InventoryItemCurySettings.inventoryID, Equal<PX.Objects.IN.InventoryItem.inventoryID>>,
                                       LeftJoin<BAccountR, On<BAccountR.bAccountID, Equal<InventoryItemCurySettings.preferredVendorID>>,
                                           LeftJoin<INItemCustSalesStats,
                                               On<CurrentValue<CRSiteStatusFilter.mode>, Equal<SOAddItemMode.byCustomer>,
                                                   And<INItemCustSalesStats.inventoryID, Equal<InventoryItem.inventoryID>,
                                                       And<INItemCustSalesStats.subItemID, Equal<INSiteStatus.subItemID>,
                                                           And<INItemCustSalesStats.siteID, Equal<INSiteStatus.siteID>,
                                                               And<INItemCustSalesStats.bAccountID, Equal<CurrentValue<CRQuote.bAccountID>>,
                                                                   And<Where<INItemCustSalesStats.lastDate, GreaterEqual<CurrentValue<CRSiteStatusFilter.historyDate>>,
                                                                       Or<CurrentValue<CRSiteStatusFilter.dropShipSales>, Equal<True>,
                                                                           And<INItemCustSalesStats.dropShipLastDate, GreaterEqual<CurrentValue<CRSiteStatusFilter.historyDate>>>>>>>>>>>,
                                               LeftJoin<INUnit,
                                                   On<INUnit.inventoryID, Equal<InventoryItem.inventoryID>,
                                                       And<INUnit.unitType, Equal<INUnitType.inventoryItem>,
                                                           And<INUnit.fromUnit, Equal<InventoryItem.salesUnit>,
                                                               And<INUnit.toUnit, Equal<InventoryItem.baseUnit>>>>>
                                               >>>>>>>>>>>,
       Where<CurrentValue<CRQuote.bAccountID>, IsNotNull,
           And2<Where<CurrentValue<INSiteStatusFilter.onlyAvailable>, Equal<boolFalse>,
                   Or<INSiteStatus.qtyAvail, Greater<PX.Objects.CS.decimal0>>>,
                   And<InventoryItem.isTemplate, Equal<False>,
                       And<InventoryItem.itemStatus, NotIn3<
                           InventoryItemStatus.unknown,
                           InventoryItemStatus.inactive,
                           InventoryItemStatus.markedForDeletion,
                           InventoryItemStatus.noSales>>>>>>), Persistent = false)]

    [Serializable]
    public class CRSiteStatusSelected : PXBqlTable, IBqlTable
    {
        protected bool? _Selected = new bool?(false);
        protected int? _InventoryID;
        protected string _InventoryCD;
        protected string _Descr;
        protected int? _ItemClassID;
        protected string _ItemClassCD;
        protected string _ItemClassDescription;
        protected string _PriceClassID;
        protected string _PriceClassDescription;
        protected int? _PreferredVendorID;
        protected string _PreferredVendorDescription;
        protected string _BarCode;
        protected string _AlternateID;
        protected string _AlternateType;
        protected string _AlternateDescr;
        protected int? _SiteID;
        protected string _SiteCD;
        protected int? _SubItemID;
        protected string _SubItemCD;
        protected string _BaseUnit;
        protected string _CuryID;
        protected long? _CuryInfoID;
        protected string _SalesUnit;
        protected Decimal? _QtySelected;
        protected Decimal? _QtyOnHand;
        protected Decimal? _QtyAvail;
        protected Decimal? _QtyLast;
        protected Decimal? _BaseUnitPrice;
        protected Decimal? _CuryUnitPrice;
        protected Decimal? _QtyAvailSale;
        protected Decimal? _QtyOnHandSale;
        protected Decimal? _QtyLastSale;
        protected DateTime? _LastSalesDate;
        protected Guid? _NoteID;

        [PXBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Selected")]
        public virtual bool? Selected
        {
            get => this._Selected;
            set => this._Selected = value;
        }

        [Inventory(BqlField = typeof(PX.Objects.IN.InventoryItem.inventoryID), IsKey = true)]
        [PXDefault]
        public virtual int? InventoryID
        {
            get => this._InventoryID;
            set => this._InventoryID = value;
        }

        [PXDefault]
        [InventoryRaw(BqlField = typeof(PX.Objects.IN.InventoryItem.inventoryCD))]
        public virtual string InventoryCD
        {
            get => this._InventoryCD;
            set => this._InventoryCD = value;
        }

        [PXDBLocalizableString(256, BqlField = typeof(PX.Objects.IN.InventoryItem.descr), IsProjection = true,
            IsUnicode = true)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Descr
        {
            get => this._Descr;
            set => this._Descr = value;
        }

        [PXDBInt(BqlField = typeof(PX.Objects.IN.InventoryItem.itemClassID))]
        [PXUIField(DisplayName = "Item Class ID", Visible = false)]
        [PXDimensionSelector("INITEMCLASS", typeof(INItemClass.itemClassID), typeof(INItemClass.itemClassCD),
            ValidComboRequired = true)]
        public virtual int? ItemClassID
        {
            get => this._ItemClassID;
            set => this._ItemClassID = value;
        }

        [PXDBString(30, BqlField = typeof(INItemClass.itemClassCD), IsUnicode = true)]
        public virtual string ItemClassCD
        {
            get => this._ItemClassCD;
            set => this._ItemClassCD = value;
        }

        [PXDBLocalizableString(256, BqlField = typeof(INItemClass.descr), IsProjection = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Item Class Description", ErrorHandling = PXErrorHandling.Always, Visible = false)]
        public virtual string ItemClassDescription
        {
            get => this._ItemClassDescription;
            set => this._ItemClassDescription = value;
        }

        [PXDBString(10, BqlField = typeof(PX.Objects.IN.InventoryItem.priceClassID), IsUnicode = true)]
        [PXUIField(DisplayName = "Price Class ID", Visible = false)]
        public virtual string PriceClassID
        {
            get => this._PriceClassID;
            set => this._PriceClassID = value;
        }

        [PXDBString(256, BqlField = typeof(INPriceClass.description), IsUnicode = true)]
        [PXUIField(DisplayName = "Price Class Description", ErrorHandling = PXErrorHandling.Always, Visible = false)]
        public virtual string PriceClassDescription
        {
            get => this._PriceClassDescription;
            set => this._PriceClassDescription = value;
        }

        [VendorNonEmployeeActive(BqlField = typeof(InventoryItemCurySettings.preferredVendorID),
            DescriptionField = typeof(BAccountR.acctName), DisplayName = "Preferred Vendor ID",
            ErrorHandling = PXErrorHandling.Always, Required = false, Visible = false)]
        public virtual int? PreferredVendorID
        {
            get => this._PreferredVendorID;
            set => this._PreferredVendorID = value;
        }

        [PXDBString(250, BqlField = typeof(BAccountR.acctName), IsUnicode = true)]
        [PXUIField(DisplayName = "Preferred Vendor Name", ErrorHandling = PXErrorHandling.Always, Visible = false)]
        public virtual string PreferredVendorDescription
        {
            get => this._PreferredVendorDescription;
            set => this._PreferredVendorDescription = value;
        }

        [PXDBString(255, BqlField = typeof(INItemXRef.alternateID), IsUnicode = true)]
        [PXUIField(DisplayName = "Barcode", Visible = false)]
        public virtual string BarCode
        {
            get => this._BarCode;
            set => this._BarCode = value;
        }

        [PXDBString(225, BqlField = typeof(INItemPartNumber.alternateID), InputMask = "", IsUnicode = true)]
        [PXUIField(DisplayName = "Alternate ID")]
        [PXExtraKey]
        public virtual string AlternateID
        {
            get => this._AlternateID;
            set => this._AlternateID = value;
        }

        [PXDBString(4, BqlField = typeof(INItemPartNumber.alternateType))]
        [INAlternateType.List]
        [PXDefault("GLBL")]
        [PXUIField(DisplayName = "Alternate Type")]
        public virtual string AlternateType
        {
            get => this._AlternateType;
            set => this._AlternateType = value;
        }

        [PXDBString(60, BqlField = typeof(INItemPartNumber.descr), IsUnicode = true)]
        [PXUIField(DisplayName = "Alternate Description", Visible = false)]
        public virtual string AlternateDescr
        {
            get => this._AlternateDescr;
            set => this._AlternateDescr = value;
        }

        [PXUIField(DisplayName = "Warehouse")]
        [PX.Objects.IN.Site(BqlField = typeof(INSiteStatus.siteID))]
        public virtual int? SiteID
        {
            get => this._SiteID;
            set => this._SiteID = value;
        }

        [PXString(IsKey = true, IsUnicode = true)]
        [PXDBCalced(typeof(PX.Data.IsNull<PX.Data.RTrim<INSite.siteCD>, PX.Data.Empty>), typeof(string))]
        public virtual string SiteCD
        {
            get => this._SiteCD;
            set => this._SiteCD = value;
        }

        [PX.Objects.IN.SubItem(typeof(CRSiteStatusSelected.inventoryID), BqlField = typeof(INSubItem.subItemID))]
        public virtual int? SubItemID
        {
            get => this._SubItemID;
            set => this._SubItemID = value;
        }

        [PXString(IsKey = true, IsUnicode = true)]
        [PXDBCalced(typeof(PX.Data.IsNull<PX.Data.RTrim<INSubItem.subItemCD>, PX.Data.Empty>), typeof(string))]
        public virtual string SubItemCD
        {
            get => this._SubItemCD;
            set => this._SubItemCD = value;
        }

        [INUnit(BqlField = typeof(PX.Objects.IN.InventoryItem.baseUnit), DisplayName = "Base Unit",
            Visibility = PXUIVisibility.Visible)]
        public virtual string BaseUnit
        {
            get => this._BaseUnit;
            set => this._BaseUnit = value;
        }

        [PXString(5, InputMask = ">LLLLL", IsUnicode = true)]
        [PXUIField(DisplayName = "Currency", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string CuryID
        {
            get => this._CuryID;
            set => this._CuryID = value;
        }

        [PXLong]
        [PX.Objects.CM.CurrencyInfo]
        public virtual long? CuryInfoID
        {
            get => this._CuryInfoID;
            set => this._CuryInfoID = value;
        }

        [INUnit(typeof(CRSiteStatusSelected.inventoryID), BqlField = typeof(PX.Objects.IN.InventoryItem.salesUnit),
            DisplayName = "Sales Unit")]
        public virtual string SalesUnit
        {
            get => this._SalesUnit;
            set => this._SalesUnit = value;
        }

        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Selected")]
        public virtual Decimal? QtySelected
        {
            get => new Decimal?(this._QtySelected.GetValueOrDefault());
            set
            {
                int num1;
                if (value.HasValue)
                {
                    Decimal? nullable = value;
                    Decimal num2 = 0M;
                    num1 = !(nullable.GetValueOrDefault() == num2 & nullable.HasValue) ? 1 : 0;
                }
                else
                    num1 = 0;

                if (num1 != 0)
                    this._Selected = new bool?(true);
                this._QtySelected = value;
            }
        }

        [PXDBQuantity(BqlField = typeof(INSiteStatus.qtyOnHand))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. On Hand")]
        public virtual Decimal? QtyOnHand
        {
            get => this._QtyOnHand;
            set => this._QtyOnHand = value;
        }

        [PXDBQuantity(BqlField = typeof(INSiteStatus.qtyAvail))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Available")]
        public virtual Decimal? QtyAvail
        {
            get => this._QtyAvail;
            set => this._QtyAvail = value;
        }

        [PXDBQuantity(BqlField = typeof(INItemCustSalesStats.lastQty))]
        public virtual Decimal? QtyLast
        {
            get => this._QtyLast;
            set => this._QtyLast = value;
        }

        [PXDBPriceCost(true, BqlField = typeof(INItemCustSalesStats.lastUnitPrice))]
        public virtual Decimal? BaseUnitPrice
        {
            get => this._BaseUnitPrice;
            set => this._BaseUnitPrice = value;
        }

        [PXUnitPriceCuryConv(typeof(CRSiteStatusSelected.curyInfoID), typeof(CRSiteStatusSelected.baseUnitPrice))]
        [PXUIField(DisplayName = "Last Unit Price", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? CuryUnitPrice
        {
            get => this._CuryUnitPrice;
            set => this._CuryUnitPrice = value;
        }

        [PXDBCalced(
            typeof(Switch<
                Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>, Mult<INSiteStatus.qtyAvail, INUnit.unitRate>>,
                Div<INSiteStatus.qtyAvail, INUnit.unitRate>>), typeof(Decimal))]
        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. Available")]
        public virtual Decimal? QtyAvailSale
        {
            get => this._QtyAvailSale;
            set => this._QtyAvailSale = value;
        }

        [PXDBCalced(
            typeof(Switch<
                Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>, Mult<INSiteStatus.qtyOnHand, INUnit.unitRate>>,
                Div<INSiteStatus.qtyOnHand, INUnit.unitRate>>), typeof(Decimal))]
        [PXQuantity]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Qty. On Hand")]
        public virtual Decimal? QtyOnHandSale
        {
            get => this._QtyOnHandSale;
            set => this._QtyOnHandSale = value;
        }

        [PXDBCalced(
            typeof(Switch<
                Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
                    Mult<INItemCustSalesStats.lastQty, INUnit.unitRate>>,
                Div<INItemCustSalesStats.lastQty, INUnit.unitRate>>), typeof(Decimal))]
        [PXQuantity]
        [PXUIField(DisplayName = "Qty. Last Sales")]
        public virtual Decimal? QtyLastSale
        {
            get => this._QtyLastSale;
            set => this._QtyLastSale = value;
        }

        [PXDBDate(BqlField = typeof(INItemCustSalesStats.lastDate))]
        [PXUIField(DisplayName = "Last Sales Date")]
        public virtual DateTime? LastSalesDate
        {
            get => this._LastSalesDate;
            set => this._LastSalesDate = value;
        }

        [PXDBQuantity(BqlField = typeof(INItemCustSalesStats.dropShipLastQty))]
        public virtual Decimal? DropShipLastBaseQty { get; set; }

        [PXDBCalced(
            typeof(Switch<
                Case<Where<INUnit.unitMultDiv, Equal<MultDiv.divide>>,
                    Mult<INItemCustSalesStats.dropShipLastQty, INUnit.unitRate>>,
                Div<INItemCustSalesStats.dropShipLastQty, INUnit.unitRate>>), typeof(Decimal))]
        [PXQuantity]
        [PXUIField(DisplayName = "Qty. of Last Drop Ship")]
        public virtual Decimal? DropShipLastQty { get; set; }

        [PXDBPriceCost(true, BqlField = typeof(INItemCustSalesStats.dropShipLastUnitPrice))]
        public virtual Decimal? DropShipLastUnitPrice { get; set; }

        [PXUnitPriceCuryConv(typeof(CRSiteStatusSelected.curyInfoID),
            typeof(CRSiteStatusSelected.dropShipLastUnitPrice))]
        [PXUIField(DisplayName = "Unit Price of Last Drop Ship", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? DropShipCuryUnitPrice { get; set; }

        [PXDBDate(BqlField = typeof(INItemCustSalesStats.dropShipLastDate))]
        [PXUIField(DisplayName = "Date of Last Drop Ship")]
        public virtual DateTime? DropShipLastDate { get; set; }

        [PXNote(BqlField = typeof(PX.Objects.IN.InventoryItem.noteID))]
        public virtual Guid? NoteID
        {
            get => this._NoteID;
            set => this._NoteID = value;
        }

        public abstract class selected : BqlType<IBqlBool, bool>.Field<CRSiteStatusSelected.selected>
        {
        }

        public abstract class inventoryID : BqlType<IBqlInt, int>.Field<CRSiteStatusSelected.inventoryID>
        {
        }

        public abstract class inventoryCD :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.inventoryCD>
        {
        }

        public abstract class descr : BqlType<IBqlString, string>.Field<CRSiteStatusSelected.descr>
        {
        }

        public abstract class itemClassID : BqlType<IBqlInt, int>.Field<CRSiteStatusSelected.itemClassID>
        {
        }

        public abstract class itemClassCD :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.itemClassCD>
        {
        }

        public abstract class itemClassDescription :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.itemClassDescription>
        {
        }

        public abstract class priceClassID :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.priceClassID>
        {
        }

        public abstract class priceClassDescription :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.priceClassDescription>
        {
        }

        public abstract class preferredVendorID :
            BqlType<IBqlInt, int>.Field<CRSiteStatusSelected.preferredVendorID>
        {
        }

        public abstract class preferredVendorDescription :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.preferredVendorDescription>
        {
        }

        public abstract class barCode : BqlType<IBqlString, string>.Field<CRSiteStatusSelected.barCode>
        {
        }

        public abstract class alternateID :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.alternateID>
        {
        }

        public abstract class alternateType :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.alternateType>
        {
        }

        public abstract class alternateDescr :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.alternateDescr>
        {
        }

        public abstract class siteID : BqlType<IBqlInt, int>.Field<CRSiteStatusSelected.siteID>
        {
        }

        public abstract class siteCD : BqlType<IBqlString, string>.Field<CRSiteStatusSelected.siteCD>
        {
        }

        public abstract class subItemID : BqlType<IBqlInt, int>.Field<CRSiteStatusSelected.subItemID>
        {
        }

        public abstract class subItemCD :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.subItemCD>
        {
        }

        public abstract class baseUnit : BqlType<IBqlString, string>.Field<CRSiteStatusSelected.baseUnit>
        {
        }

        public abstract class curyID : BqlType<IBqlString, string>.Field<CRSiteStatusSelected.curyID>
        {
        }

        public abstract class curyInfoID : BqlType<IBqlLong, long>.Field<CRSiteStatusSelected.curyInfoID>
        {
        }

        public abstract class salesUnit :
            BqlType<IBqlString, string>.Field<CRSiteStatusSelected.salesUnit>
        {
        }

        public abstract class qtySelected :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtySelected>
        {
        }

        public abstract class qtyOnHand :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyOnHand>
        {
        }

        public abstract class qtyAvail :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyAvail>
        {
        }


        public abstract class qtyLast : BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyLast>
        {
        }

        public abstract class baseUnitPrice :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.baseUnitPrice>
        {
        }


        public abstract class curyUnitPrice :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.curyUnitPrice>
        {
        }

        public abstract class qtyAvailSale :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyAvailSale>
        {
        }

        public abstract class qtyOnHandSale :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyOnHandSale>
        {
        }

        public abstract class qtyLastSale :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.qtyLastSale>
        {
        }

        public abstract class lastSalesDate :
            BqlType<IBqlDateTime, DateTime>.Field<CRSiteStatusSelected.lastSalesDate>
        {
        }

        public abstract class dropShipLastBaseQty :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.dropShipLastBaseQty>
        {
        }

        public abstract class dropShipLastQty :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.dropShipLastQty>
        {
        }

        public abstract class dropShipLastUnitPrice :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.dropShipLastUnitPrice>
        {
        }

        public abstract class dropShipCuryUnitPrice :
            BqlType<IBqlDecimal, Decimal>.Field<CRSiteStatusSelected.dropShipCuryUnitPrice>
        {
        }

        public abstract class dropShipLastDate :
            BqlType<IBqlDateTime, DateTime>.Field<CRSiteStatusSelected.dropShipLastDate>
        {
        }

        public abstract class noteID : BqlType<IBqlGuid, Guid>.Field<CRSiteStatusSelected.noteID>
        {
        }
    }
}
