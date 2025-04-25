using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.SO;

namespace Opayo.DAC
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        #region UsrPaymentLink
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Link")]
        public virtual string UsrPaymentLink { get; set; }
        public abstract class usrPaymentLink : PX.Data.BQL.BqlString.Field<usrPaymentLink> { }
        #endregion
    }
}
