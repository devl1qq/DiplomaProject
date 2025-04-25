using PX.Data;
using PX.Data.BQL;
using PX.Objects.AM;
using PX.Objects.AP;
using PX.Objects.CR.Standalone;
using PX.Objects.CS;
using PX.TM;
using System.Collections.Generic;
using System.Linq;

namespace MainProject.DAC
{
    public class SalesTerritoryExt : PXCacheExtension<SalesTerritory>
    {
        #region UsrBusinessDeveloper
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = "Business Developer")]
        [PXSelector(typeof(Search<EPEmployee.acctCD>), ValidateValue = false)]
        public virtual string UsrBusinessDeveloper { get; set; }
        public abstract class usrBusinessDeveloper : PX.Data.BQL.BqlString.Field<usrBusinessDeveloper> { }
        #endregion
    }
}
