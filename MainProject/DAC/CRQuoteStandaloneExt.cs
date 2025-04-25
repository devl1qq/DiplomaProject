using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR.Standalone;
using PX.Objects.SO;
using static MainProject.DAC.SOOrderExt.usrCustomerDescription;

namespace MainProject.DAC
{
    public class CRQuoteStandaloneExt : PXCacheExtension<CRQuote>
    {
        #region UsrFlowDownNotes
        [PXDBInt]
        public virtual int? UsrFlowDownNotes { get; set; }
        public abstract class usrFlowDownNotes : PX.Data.BQL.BqlInt.Field<usrFlowDownNotes> { }
        #endregion
        #region UsrSalesPerson
        [PXDBString(15, IsUnicode = true)]
        public virtual string UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlString.Field<usrSalesPerson> { }
        #endregion
        #region UsrBDM
        [PXDBInt]
        [PXUIField(DisplayName = "BDM")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<bdmTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrBDM { get; set; }
        public abstract class usrBDM : PX.Data.BQL.BqlInt.Field<usrBDM> { }
        #endregion
        #region UsrKAM
        [PXDBInt]
        [PXUIField(DisplayName = "KAM")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<kamTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrKAM { get; set; }
        public abstract class usrKAM : PX.Data.BQL.BqlInt.Field<usrKAM> { }
        #endregion
        #region UsrCSR
        [PXDBInt]
        [PXUIField(DisplayName = "CSR")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<csrTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrCSR { get; set; }
        public abstract class usrCSR : PX.Data.BQL.BqlInt.Field<usrCSR> { }
        #endregion
    }
}
