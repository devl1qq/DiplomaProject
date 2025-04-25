using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    [Serializable]
    [PXCacheName("ArcEstimateAndSalesQuote")]
    public class ArcEstimateAndSalesQuote : PXBqlTable, IBqlTable
    {
        #region Keys

        public class PK : PrimaryKeyOf<ArcEstimateAndSalesQuote>.By<noteid>
        {
            public static ArcEstimateAndSalesQuote Find(PXGraph graph, Guid? noteid) => FindBy(graph, noteid);
        }

        #endregion
        #region Noteid
        [PXNote(IsKey = true)]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion

        #region QuoteNbr
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Quote Nbr")]
        public virtual string QuoteNbr { get; set; }
        public abstract class quoteNbr : PX.Data.BQL.BqlString.Field<quoteNbr> { }
        #endregion

        #region QuoteID
        [PXDBGuid()]
        [PXUIField(DisplayName = "Quote ID")]
        public virtual Guid? QuoteID { get; set; }
        public abstract class quoteID : PX.Data.BQL.BqlGuid.Field<quoteID> { }
        #endregion

        #region EstimateID
        [PXDBString(15, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Estimate ID")]
        public virtual string EstimateID { get; set; }
        public abstract class estimateID : PX.Data.BQL.BqlString.Field<estimateID> { }
        #endregion

        #region RevisionID
        [PXDBString(10, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Revision ID")]
        public virtual string RevisionID { get; set; }
        public abstract class revisionID : PX.Data.BQL.BqlString.Field<revisionID> { }
        #endregion

        #region InventoryID
        [PXDBInt()]
        [PXUIField(DisplayName = "Inventory ID")]
        public virtual int? InventoryID { get; set; }
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        #endregion

        #region InventoryCD
        [PXDBString(70, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Inventory CD")]
        public virtual string InventoryCD { get; set; }
        public abstract class inventoryCD : PX.Data.BQL.BqlString.Field<inventoryCD> { }
        #endregion

        #region Descr
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Descr")]
        public virtual string Descr { get; set; }
        public abstract class descr : PX.Data.BQL.BqlString.Field<descr> { }
        #endregion

        #region Quantity
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Quantity")]
        public virtual Decimal? Quantity { get; set; }
        public abstract class quantity : PX.Data.BQL.BqlDecimal.Field<quantity> { }
        #endregion

        #region CuryUnitPrice
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Unit Price")]
        public virtual Decimal? CuryUnitPrice { get; set; }
        public abstract class curyUnitPrice : PX.Data.BQL.BqlDecimal.Field<curyUnitPrice> { }
        #endregion

        #region CuryExtPrice
        [PXDBDecimal()]
        [PXUIField(DisplayName = "Cury Ext Price")]
        public virtual Decimal? CuryExtPrice { get; set; }
        public abstract class curyExtPrice : PX.Data.BQL.BqlDecimal.Field<curyExtPrice> { }
        #endregion

        #region UsrExternalNotes
        [PXDBString(256, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr External Notes")]
        public virtual string UsrExternalNotes { get; set; }
        public abstract class usrExternalNotes : PX.Data.BQL.BqlString.Field<usrExternalNotes> { }
        #endregion

        #region LineNbr
        [PXDBInt()]
        [PXUIField(DisplayName = "Line Nbr")]
        public virtual int? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion

        #region SortOrder
        [PXDBInt()]
        [PXUIField(DisplayName = "Sort Order")]
        public virtual int? SortOrder { get; set; }
        public abstract class sortOrder : PX.Data.BQL.BqlInt.Field<sortOrder> { }
        #endregion

        #region UsrAlternateID
        [PXDBString(50, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Alternate ID")]
        public virtual string UsrAlternateID { get; set; }
        public abstract class usrAlternateID : PX.Data.BQL.BqlString.Field<usrAlternateID> { }
        #endregion

        #region Uom
        [PXDBString(6, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Uom")]
        public virtual string Uom { get; set; }
        public abstract class uom : PX.Data.BQL.BqlString.Field<uom> { }
        #endregion

        #region UsrBoolean1
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean1")]
        public virtual bool? UsrBoolean1 { get; set; }
        public abstract class usrBoolean1 : PX.Data.BQL.BqlBool.Field<usrBoolean1> { }
        #endregion

        #region UsrBoolean2
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean2")]
        public virtual bool? UsrBoolean2 { get; set; }
        public abstract class usrBoolean2 : PX.Data.BQL.BqlBool.Field<usrBoolean2> { }
        #endregion

        #region UsrBoolean3
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean3")]
        public virtual bool? UsrBoolean3 { get; set; }
        public abstract class usrBoolean3 : PX.Data.BQL.BqlBool.Field<usrBoolean3> { }
        #endregion

        #region UsrBoolean4
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean4")]
        public virtual bool? UsrBoolean4 { get; set; }
        public abstract class usrBoolean4 : PX.Data.BQL.BqlBool.Field<usrBoolean4> { }
        #endregion

        #region UsrBoolean5
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean5")]
        public virtual bool? UsrBoolean5 { get; set; }
        public abstract class usrBoolean5 : PX.Data.BQL.BqlBool.Field<usrBoolean5> { }
        #endregion

        #region UsrBoolean6
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean6")]
        public virtual bool? UsrBoolean6 { get; set; }
        public abstract class usrBoolean6 : PX.Data.BQL.BqlBool.Field<usrBoolean6> { }
        #endregion

        #region UsrBoolean7
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean7")]
        public virtual bool? UsrBoolean7 { get; set; }
        public abstract class usrBoolean7 : PX.Data.BQL.BqlBool.Field<usrBoolean7> { }
        #endregion

        #region UsrBoolean8
        [PXDBBool()]
        [PXUIField(DisplayName = "Usr Boolean8")]
        public virtual bool? UsrBoolean8 { get; set; }
        public abstract class usrBoolean8 : PX.Data.BQL.BqlBool.Field<usrBoolean8> { }
        #endregion

        #region UsrText1
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Text1")]
        public virtual string UsrText1 { get; set; }
        public abstract class usrText1 : PX.Data.BQL.BqlString.Field<usrText1> { }
        #endregion

        #region UsrText2
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Text2")]
        public virtual string UsrText2 { get; set; }
        public abstract class usrText2 : PX.Data.BQL.BqlString.Field<usrText2> { }
        #endregion

        #region UsrDimension1ID
        [PXDBInt()]
        [PXUIField(DisplayName = "Usr Dimension1 ID")]
        public virtual int? UsrDimension1ID { get; set; }
        public abstract class usrDimension1ID : PX.Data.BQL.BqlInt.Field<usrDimension1ID> { }
        #endregion

        #region UsrDimension2ID
        [PXDBInt()]
        [PXUIField(DisplayName = "Usr Dimension2 ID")]
        public virtual int? UsrDimension2ID { get; set; }
        public abstract class usrDimension2ID : PX.Data.BQL.BqlInt.Field<usrDimension2ID> { }
        #endregion

        #region UsrDimension3ID
        [PXDBInt()]
        [PXUIField(DisplayName = "Usr Dimension3 ID")]
        public virtual int? UsrDimension3ID { get; set; }
        public abstract class usrDimension3ID : PX.Data.BQL.BqlInt.Field<usrDimension3ID> { }
        #endregion

        #region UsrDimension4ID
        [PXDBInt()]
        [PXUIField(DisplayName = "Usr Dimension4 ID")]
        public virtual int? UsrDimension4ID { get; set; }
        public abstract class usrDimension4ID : PX.Data.BQL.BqlInt.Field<usrDimension4ID> { }
        #endregion

        #region LeadTime
        [PXDBInt()]
        [PXUIField(DisplayName = "Lead Time")]
        public virtual int? LeadTime { get; set; }
        public abstract class leadTime : PX.Data.BQL.BqlInt.Field<leadTime> { }
        #endregion

        #region UsrManufacturer
        [PXDBString(4, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Usr Manufacturer")]
        public virtual string UsrManufacturer { get; set; }
        public abstract class usrManufacturer : PX.Data.BQL.BqlString.Field<usrManufacturer> { }
        #endregion
    }
}
