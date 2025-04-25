using PX.Data;
using PX.Objects.PO;

namespace MainProject.DAC
{
    public class POLineExt : PXCacheExtension<POLine>
    {
        #region UsrItemURL
        [PXString(100)]
        [PXUIField(DisplayName = "Item URL", Enabled = false, IsReadOnly = true)]
        [PXFormula(typeof(Selector<POLine.inventoryID, InventoryItemExt.usrItemURL>))]
        public virtual string UsrItemURL { get; set; }
        public abstract class usrItemURL : PX.Data.BQL.BqlString.Field<usrItemURL> { }
        #endregion

        #region UsrExternalNotes
        [PXDBString(256)]
        [PXUIField(DisplayName = "External Notes")]
        public virtual string UsrExternalNotes { get; set; }
        public abstract class usrExternalNotes : PX.Data.BQL.BqlString.Field<usrExternalNotes> { }
        #endregion
        #region UsrTrackingNumber
        [PXDBString(100)]
        [PXUIField(DisplayName = "Tracking Number")]
        public virtual string UsrTrackingNumber { get; set; }
        public abstract class usrTrackingNumber : PX.Data.BQL.BqlString.Field<usrTrackingNumber> { }
        #endregion
        #region IsSplitPOLineEnable
        [PXBool]
        [PXUIField(Visible = false)]
        public virtual bool? IsSplitPOLineEnable { get; set; }
        public abstract class isSplitPOLineEnable : PX.Data.BQL.BqlBool.Field<isSplitPOLineEnable> { }
        #endregion

        #region UsrSONbr
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "SO Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual string UsrSONbr { get; set; }
        public abstract class usrSONbr : PX.Data.BQL.BqlString.Field<usrSONbr> { }
        #endregion

        #region UsrSOType
        [PXDBString(2, IsFixed = true, InputMask = ">aa")]
        [PXUIField(DisplayName = "SO Type", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual string UsrSOType { get; set; }
        public abstract class usrSOType : PX.Data.BQL.BqlString.Field<usrSOType> { }
        #endregion

        #region UsrSOLineNbr
        [PXDBInt]
        [PXUIField(DisplayName = "SO Line Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual int? UsrSOLineNbr { get; set; }
        public abstract class usrSOLineNbr : PX.Data.BQL.BqlInt.Field<usrSOLineNbr> { }
        #endregion
    }
}
