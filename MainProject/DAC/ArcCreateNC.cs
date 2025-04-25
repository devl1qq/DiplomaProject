using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.Objects.PO;
using System;

namespace MainProject.DAC
{
    [PXVirtual]
    public class ArcCreateNC : PXBqlTable, IBqlTable
    {
        #region RecordID
        [PXDBIdentity(IsKey = true)]
        public virtual int? RecordID { get; set; }
        public abstract class recordID : PX.Data.BQL.BqlInt.Field<recordID> { }

        #endregion
        #region InventoryID
        public abstract class inventoryID : PX.Data.BQL.BqlInt.Field<inventoryID> { }
        [PXInt]
        [PXUIField(DisplayName = "Inventory",Visible = false, IsReadOnly = true)]
        [PXSelector(typeof(InventoryItem.inventoryID), SubstituteKey = typeof(InventoryItem.inventoryCD))]
        public virtual int? InventoryID { get; set; }
        #endregion
        #region LineNbr
        [PXInt]
        [PXUIField(DisplayName = "Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, IsReadOnly = true)]
        public virtual Int32? LineNbr { get; set; }
        public abstract class lineNbr : PX.Data.BQL.BqlInt.Field<lineNbr> { }
        #endregion
        #region SplitLineNbr
        [PXInt]
        [PXUIField(DisplayName = "Split Line Nbr.", Visibility = PXUIVisibility.Visible, Visible = false, IsReadOnly = true)]
        public virtual Int32? SplitLineNbr { get; set; }
        public abstract class splitLineNbr : PX.Data.BQL.BqlInt.Field<splitLineNbr> { }
        #endregion
        #region UsrNCQty
        [PXInt]
        [PXUIField(DisplayName = "NC Qty")]
        public virtual int? UsrNCQty { get; set; }
        public abstract class usrNCQty : PX.Data.BQL.BqlInt.Field<usrNCQty> { }
        #endregion
        #region UsrSelected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSelected { get; set; }
        public abstract class usrSelected : PX.Data.BQL.BqlBool.Field<usrSelected> { }
        #endregion
        #region LotSerialNbr
        [PXString]
        [PXUIField(DisplayName = "Lot/Serial Number", IsReadOnly = true)]
        public virtual String LotSerialNbr { get; set; }
        public abstract class lotSerialNbr : PX.Data.BQL.BqlString.Field<lotSerialNbr> { }
        #endregion
        #region Qty
        [PXDecimal]
        [PXUIField(DisplayName = "Quantity", IsReadOnly = true)]
        public virtual Decimal? Qty { get; set; }
        public abstract class qty : PX.Data.BQL.BqlDecimal.Field<qty> { }
        #endregion
        #region SplitType
        [PXString(2)]
        public virtual string SplitType { get; set; }
        public abstract class splitType : PX.Data.BQL.BqlString.Field<splitType> { }
        #endregion
        #region RefSplitNbr
        [PXString(15)]
        public virtual String RefSplitNbr { get; set; }
        public abstract class refSplitNbr : PX.Data.BQL.BqlString.Field<refSplitNbr> { }
        #endregion
        #region IsProdSplit
        [PXBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? IsProdSplit { get; set; }
        public abstract class isProdSplit : PX.Data.BQL.BqlBool.Field<isProdSplit> { }
        #endregion
        #region CaseClassID
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXDefault()]
        [PXUIField(DisplayName = "Case Class")]
        [PXSelector(typeof(CRCaseClass.caseClassID),
            DescriptionField = typeof(CRCaseClass.description),
            CacheGlobal = true)]
        public virtual string CaseClassID { get; set; }
        public abstract class caseClassID : PX.Data.BQL.BqlString.Field<caseClassID> { }
        #endregion
    }
}
