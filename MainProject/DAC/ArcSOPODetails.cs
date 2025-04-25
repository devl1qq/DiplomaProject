using PX.Data;
using PX.Data.BQL;
using PX.Data.EP;
using PX.Objects.AM;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.SO;
using MainProject.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class poTypeConstant : BqlString.Constant<poTypeConstant>
    {
        public poTypeConstant() : base("PX.Objects.PO.POOrder") { }
    }
    public class soTypeConstant : BqlString.Constant<soTypeConstant>
    {
        public soTypeConstant() : base("PX.Objects.SO.SOOrder") { }
    }

    [Serializable]
    [PXCacheName("ArcSOPODetails")]
    public class ArcSOPODetails : PXBqlTable, IBqlTable
    {
        #region SOPODetailsID
        [PXDBIdentity(IsKey = true)]
        public virtual int? SOPODetailsID { get; set; }
        public abstract class sOPODetailsID : PX.Data.BQL.BqlInt.Field<sOPODetailsID> { }
        #endregion

        #region RefNoteID
        [PXDBGuid()]
        [PXSelector(typeof(Search<CRCase.noteID>), SubstituteKey = typeof(CRCase.caseCD))]
        [PXUIField(DisplayName = "Case ID", Visibility = PXUIVisibility.SelectorVisible, Visible = true)]
        [PXFieldDescription]
        public virtual Guid? RefNoteID { get; set; }
        public abstract class refNoteID : PX.Data.BQL.BqlGuid.Field<refNoteID> { }
        #endregion

        #region Type
        [PXDBString(64, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Type", IsReadOnly = true)]
        [PXStringList(new[] { "PX.Objects.SO.SOOrder", "PX.Objects.PO.POOrder", "PX.Objects.AM.AMProdItem" }, new[] { "Sales Order", "Purchase Order", "Production Order" })]
        public virtual string Type { get; set; }
        public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
        #endregion

        #region POOrderNoteID
        [PXGuid()]
        [PXFormula(typeof(Switch<
            Case<Where<type, Equal<poTypeConstant>>, relatedEntityID>,
            Null>))]
        [PXSelector(typeof(POOrder.noteID), ValidateValue = false)]
        public virtual Guid? POOrderNoteID { get; set; }
        public abstract class pOOrderNoteID : PX.Data.BQL.BqlGuid.Field<pOOrderNoteID> { }
        #endregion

        #region SOOrderNoteID
        [PXGuid()]
        [PXFormula(typeof(Switch<
            Case<Where<type, Equal<soTypeConstant>>, relatedEntityID>,
            Null>))]
        [PXSelector(typeof(SOOrder.noteID), ValidateValue =false)]
        public virtual Guid? SOOrderNoteID { get; set; }
        public abstract class sOOrderNoteID : PX.Data.BQL.BqlGuid.Field<sOOrderNoteID> { }
        #endregion

        #region RelatedEntityOrderNbr
        [PXString(15)]
        [PXFormula(typeof(Switch<
            Case<Where<type, Equal<poTypeConstant>>, Selector<pOOrderNoteID, POOrder.orderNbr>>,
            Selector<sOOrderNoteID, SOOrder.orderNbr>>))]

        public virtual string RelatedEntityOrderNbr { get; set; }
        public abstract class relatedEntityOrderNbr : PX.Data.BQL.BqlString.Field<relatedEntityOrderNbr> { }
        #endregion

        #region RelatedEntityID
        [PXDBGuid()]
        [EntityIDSelector(typeof(type))]
        [PXUIField(DisplayName = "SO/PO Nbr", Visibility = PXUIVisibility.SelectorVisible, Visible = true)]
        public virtual Guid? RelatedEntityID { get; set; }
        public abstract class relatedEntityID : PX.Data.BQL.BqlGuid.Field<relatedEntityID> { }
        #endregion

        #region ProductionOrderNoteID
        [PXDBGuid()]
        [PXUIField(DisplayName = "Production Order Nbr", IsReadOnly = true)]
        [PXSelector(typeof(Search<AMProdItem.noteID>), 
            DescriptionField = typeof(AMProdItem.orderType),
            SubstituteKey = typeof(AMProdItem.prodOrdID))]
        public virtual Guid? ProductionOrderNoteID { get; set; }
        public abstract class productionOrderNoteID : PX.Data.BQL.BqlGuid.Field<productionOrderNoteID> { }
        #endregion

        #region TypeLine
        [PXString(64, IsUnicode = true, InputMask = "")]
        public virtual string TypeLine { get; set; }
        public abstract class typeLine : PX.Data.BQL.BqlString.Field<typeLine> { }
        #endregion

        #region SOPOLineNbr
        [PXDBInt]
        [PXUIField(DisplayName = "SO/PO Line Nbr")]
        public virtual int? SOPOLineNbr { get; set; }
        public abstract class sOPOLineNbr : PX.Data.BQL.BqlInt.Field<sOPOLineNbr> { }
        #endregion

        #region InventoryItemID
        [PXDBInt]
        [PXSelector(typeof(Search<InventoryItem.inventoryID>), 
            SubstituteKey = typeof(InventoryItem.inventoryCD))]
        [PXUIField(DisplayName = "Inventory Item", IsReadOnly = true, Visibility = PXUIVisibility.SelectorVisible, Visible = true)]
        public virtual int? InventoryItemID { get; set; }
        public abstract class inventoryItemID : PX.Data.BQL.BqlInt.Field<inventoryItemID> { }
        #endregion

        #region LotNbr
        [PXDBString(100, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Lot Nbr", Visibility = PXUIVisibility.SelectorVisible, Visible = true)]
        public virtual string LotNbr { get; set; }
        public abstract class lotNbr : PX.Data.BQL.BqlString.Field<lotNbr> { }
        #endregion

        #region NCQty
        [PXDBInt()]
        [PXUIField(DisplayName = "NC Qty", Visibility = PXUIVisibility.SelectorVisible, Visible = true)]
        [PXDefault(0)]
        public virtual int? NCQty { get; set; }
        public abstract class nCQty : PX.Data.BQL.BqlInt.Field<nCQty> { }
        #endregion

        #region System Columns
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
        #endregion
    }
}
