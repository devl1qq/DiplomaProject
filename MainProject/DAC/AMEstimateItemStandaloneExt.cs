using PX.Data;
using PX.Objects.AM;
using PX.Objects.AM.Standalone;
using PX.Objects.CR;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainProject.DAC.CROpportunityProductsExt;
using AMEstimateItem = PX.Objects.AM.Standalone.AMEstimateItem;

namespace MainProject.DAC
{
    public class AMEstimateItemStandaloneExt : PXCacheExtension<AMEstimateItem>
    {
        #region UsrAlternateID
        [AlternativeItem(INPrimaryAlternateType.CPN,
            typeof(usrBAccountID),
            typeof(AMEstimateItem.inventoryID),
            typeof(AMEstimateItem.subItemID),
            typeof(AMEstimateItem.uOM))]
        public virtual string UsrAlternateID { get; set; }
        public abstract class usrAlternateID : PX.Data.BQL.BqlString.Field<usrAlternateID> { }
        #endregion
        #region UsrBAccountID
        [PXInt]
        [PXDBScalar(typeof(Search<AMEstimateReference.bAccountID, Where<AMEstimateReference.revisionID, Equal<AMEstimateItem.revisionID>,
            And<AMEstimateReference.estimateID, Equal<AMEstimateItem.estimateID>>>>))]
        public virtual int? UsrBAccountID { get; set; }
        public abstract class usrBAccountID : PX.Data.BQL.BqlInt.Field<usrBAccountID> { }
        #endregion
    }
}
