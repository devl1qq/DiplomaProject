using PX.Data;
using PX.Objects.AM;
using PX.Objects.AM.Attributes;
using PX.Objects.AM.Standalone;
using PX.Objects.CR;
using PX.Objects.CR.Standalone;
using PX.Objects.IN;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.AR;
using static MainProject.DAC.CROpportunityProductsExt;
using AMEstimateItem = PX.Objects.AM.AMEstimateItem;

namespace MainProject.DAC
{
    public class AMEstimateItemExt : PXCacheExtension<AMEstimateItem>
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
        [PXSelector(typeof(Customer.bAccountID))]
        [PXDBScalar(typeof(Search<AMEstimateReference.bAccountID, Where<AMEstimateReference.revisionID, Equal<AMEstimateItem.revisionID>,
            And<AMEstimateReference.estimateID, Equal<AMEstimateItem.estimateID>>>>))]
        public virtual int? UsrBAccountID { get; set; }
        public abstract class usrBAccountID : PX.Data.BQL.BqlInt.Field<usrBAccountID> { }
        #endregion
    }
}
