using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL;

namespace Opayo.DAC
{
    public class BranchExt : PXCacheExtension<Branch>
    {
        #region UsrOpayoAccountID
        [PXDBInt]
        public virtual int? UsrOpayoAccountID { get; set; }
        public abstract class usrOpayoAccountID : BqlInt.Field<usrOpayoAccountID> { }
        #endregion
    }
}
