using PX.Data;
using PX.Objects.SO;

namespace MainProject.DAC
{
    public class SOLineExt : PXCacheExtension<SOLine>
    {
        #region UsrItemURL
        [PXString(100)]
        [PXUIField(DisplayName = "Item URL", Enabled = false, IsReadOnly = true)]
        [PXFormula(typeof(Selector<SOLine.inventoryID, InventoryItemExt.usrItemURL> ))]
        public virtual string UsrItemURL { get; set; }
        public abstract class usrItemURL : PX.Data.BQL.BqlString.Field<usrItemURL> { }
        #endregion
        #region UsrTestReportID
        [PXString]
        [PXUIField(DisplayName = "Test Report ID", Enabled = false, IsReadOnly = true)]
        [PXFormula(typeof(Selector<PX.Objects.AM.CacheExtensions.SOLineExt.aMProdOrdID, AMProdItemExt.usrTestReportID>))]
        public virtual string UsrTestReportID { get; set; }
        public abstract class usrTestReportID : PX.Data.BQL.BqlString.Field<usrTestReportID> { }
        #endregion
        #region UsrExternalNotes
        [PXDBString(256)]
        [PXUIField(DisplayName = "External Notes")]
        public virtual string UsrExternalNotes { get; set; }
        public abstract class usrExternalNotes : PX.Data.BQL.BqlString.Field<usrExternalNotes> { }
        #endregion
        #region UsrCustomerPOLine
        [PXDBInt]
        [PXUIField(DisplayName = "Customer PO Line")]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrCustomerPOLine { get; set; }
        public abstract class usrCustomerPOLine : PX.Data.BQL.BqlInt.Field<usrCustomerPOLine> { }
        #endregion
        #region UsrCostGroupNbr
        [PXDBInt]
        [PXUIField(DisplayName = "Cost Group Nbr")]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrCostGroupNbr { get; set; }
        public abstract class usrCostGroupNbr : PX.Data.BQL.BqlInt.Field<usrCostGroupNbr> { }
        #endregion
    }
}
