using PX.Data;
using PX.Objects.GL.DAC;

namespace MainProject.DAC
{
    public class OrganizationExt : PXCacheExtension<Organization>
    {
        #region UsrDefaultVendorOnSalesTransactions
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Default Vendor On Sales Transactions", Visible = true)]
        public virtual bool? UsrDefaultVendorOnSalesTransactions { get; set; }
        public abstract class usrDefaultVendorOnSalesTransactions : PX.Data.BQL.BqlBool.Field<usrDefaultVendorOnSalesTransactions> { }
        #endregion
    }
}
