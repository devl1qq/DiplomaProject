using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR.Standalone;
using System;
using System.Linq;
using static PX.Objects.AR.ARDocumentEnq.ARDocumentFilter;
using static PX.Objects.AR.CustomerMaint;
using static MainProject.DAC.CROpportunityExt.usrAvailableCredit;
using static MainProject.DAC.CROpportunityExt.usrCreditLimit;

namespace MainProject.DAC
{
    public class CROpportunityStandaloneExt : PXCacheExtension<CROpportunity>
    {
        #region UsrSalesPerson
        [PXDBString(15, IsUnicode = true)]
        public virtual string UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlString.Field<usrSalesPerson> { }
        #endregion
    }
}
