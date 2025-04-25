using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.CS;
using PX.SM;

namespace MainProject.DAC
{
    public class CarrierExt : PXCacheExtension<Carrier>
    {
        #region UsrDefaultPrinterID
        [PXPrinterSelector]
        public virtual Guid? UsrDefaultPrinterID { get; set; }
        public abstract class usrDefaultPrinterID : PX.Data.BQL.BqlGuid.Field<usrDefaultPrinterID> { }
        #endregion
    }
}
