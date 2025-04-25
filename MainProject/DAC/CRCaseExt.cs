using PX.Data;
using PX.Objects.CR;
using PX.Objects.SO;
using PX.Objects.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data.BQL;
using PX.Objects.AM;
using PX.SM;
using PX.Objects.EP;

namespace MainProject.DAC
{
    public class CRCaseExt : PXCacheExtension<CRCase>
    {
        #region UsrDiscrepanceTypeSeverity
        [PXString]
        [PXFormula(typeof(Selector<usrDiscrepancyTypeID, ArcDiscrepancyType.severity>))]
        public virtual string UsrDiscrepanceTypeSeverity { get; set; }
        public abstract class usrDiscrepanceTypeSeverity : PX.Data.BQL.BqlString.Field<usrDiscrepanceTypeSeverity> { }
        #endregion
        #region UsrDispositionID
        [PXDBInt]
        [PXUIField(DisplayName = "Disposition")]
        [PXSelector(
            typeof(Search2<ArcDisposition.codeID,
                    LeftJoin<EPEmployee,
                        On<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>,
                    Where<ArcDisposition.severity, IsNotNull,
                        And<ArcDisposition.active, Equal<True>,
                            And<Where2<Brackets<Where<EPEmployeeExt.usrCaseRoleLevel, Equal<Helper.Constants.BQL.HighLevel>>>, 
                                Or<EPEmployeeExt.usrCaseRoleLevel, Equal<ArcDisposition.severity>>>>>>>
            ),
            SubstituteKey = typeof(ArcDisposition.codeCD)
        )]
        public virtual int? UsrDispositionID { get; set; }
        public abstract class usrDispositionID : PX.Data.BQL.BqlInt.Field<usrDispositionID> { }
        #endregion

        #region UsrDiscrepancyTypeID 
        [PXDBInt]
        [PXUIField(DisplayName = "Discrepancy Type")]
        [PXSelector(typeof(Search<ArcDiscrepancyType.codeID, Where<ArcDiscrepancyType.active, Equal<True>>>),
            SubstituteKey = typeof(ArcDiscrepancyType.codeCD))]
        public virtual int? UsrDiscrepancyTypeID { get; set; }
        public abstract class usrDiscrepancyTypeID : PX.Data.BQL.BqlInt.Field<usrDiscrepancyTypeID> { }
        #endregion

        #region UsrResolutionID
        [PXDBInt]
        [PXUIField(DisplayName = "Resolution")]
        [PXSelector(
            typeof(Search2<ArcResolution.codeID,
                    LeftJoin<EPEmployee,
                        On<EPEmployee.userID, Equal<Current<AccessInfo.userID>>>>,
                    Where<ArcResolution.severity, IsNotNull,
                        And<ArcResolution.active, Equal<True>,
                            And<Where2<Brackets<Where<EPEmployeeExt.usrCaseRoleLevel, Equal<Helper.Constants.BQL.HighLevel>>>,
                                Or<EPEmployeeExt.usrCaseRoleLevel, Equal<ArcResolution.severity>>>>>>>
            ),
            SubstituteKey = typeof(ArcResolution.codeCD)
        )]
        public virtual int? UsrResolutionID { get; set; }
        public abstract class usrResolutionID : PX.Data.BQL.BqlInt.Field<usrResolutionID> { }
        #endregion

        #region UsrIsVendor
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrIsVendor { get; set; }
        public abstract class usrIsVendor : PX.Data.BQL.BqlBool.Field<usrIsVendor> { }
        #endregion
        #region UsrSOOrderID
        [PXDBGuid]
        [PXUIField(DisplayName = "Sales Order")]
        [PXSelector(typeof(Search<SOOrder.noteID, 
            Where<SOOrder.customerID, Equal<Current<CRCase.customerID>>>>),
            DescriptionField = typeof(SOOrder.orderType),
            SubstituteKey = typeof(SOOrder.orderNbr))]
        public virtual Guid? UsrSOOrderID { get; set; }
        public abstract class usrSOOrderID : PX.Data.BQL.BqlGuid.Field<usrSOOrderID> { }
        #endregion
        #region UsrPOOrderID
        [PXDBGuid]
        [PXUIField(DisplayName = "Purchase Order")]
        [PXSelector(typeof(Search<POOrder.noteID,
            Where<POOrder.vendorID, Equal<Current<CRCase.customerID>>, 
                And<True, Equal<Current<usrIsVendor>>>>>),
            DescriptionField = typeof(POOrder.orderType),
            SubstituteKey = typeof(POOrder.orderNbr))]
        public virtual Guid? UsrPOOrderID { get; set; }
        public abstract class usrPOOrderID : PX.Data.BQL.BqlGuid.Field<usrPOOrderID> { }
        #endregion
        #region UsrProdOrdID
        [PXDBGuid]
        [PXUIField(DisplayName = "Production Order")]
        [PXSelector(typeof(Search<AMProdItem.noteID>),
            DescriptionField = typeof(AMProdItem.orderType),
            SubstituteKey = typeof(AMProdItem.prodOrdID))]
        public virtual Guid? UsrProdOrdID { get; set; }
        public abstract class usrProdOrdID : PX.Data.BQL.BqlGuid.Field<usrProdOrdID> { }
        #endregion
    }
}
