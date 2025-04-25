using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AR;

namespace Opayo.DAC
{
    public class ARStatementExt : PXCacheExtension<ARStatement>
    {
        #region UsrPaymentLink
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Link")]
        public virtual string UsrPaymentLink { get; set; }
        public abstract class usrPaymentLink : PX.Data.BQL.BqlString.Field<usrPaymentLink> { }
        #endregion
    }
}
