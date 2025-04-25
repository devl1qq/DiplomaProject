using PX.Data;
using PX.Data.BQL;
using PX.Objects.AM;

namespace MainProject.DAC
{
    public class AMProdItemExt : PXCacheExtension<AMProdItem>
    {
        #region UsrTestReportID
        [PXDBString(256)]
        [PXUIField(DisplayName = "Test Report ID")]
        public virtual string UsrTestReportID { get; set; }
        public abstract class usrTestReportID : BqlString.Field<usrTestReportID> { }
        #endregion
    }
}
