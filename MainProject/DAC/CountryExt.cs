using PX.Data;
using PX.Objects.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class CountryExt : PXCacheExtension<Country>
    {
        #region UsrBanned
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Banned")]
        public virtual bool? UsrBanned { get; set; }
        public abstract class usrBanned : PX.Data.BQL.BqlBool.Field<usrBanned> { }
        #endregion
    }
}
