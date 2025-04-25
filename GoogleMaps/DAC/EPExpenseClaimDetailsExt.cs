using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Data.BQL;
using PX.Objects.AP;
using MileageCustomisation.Attributes;
using MileageCustomisation.Graph;
using PX.Objects.SO;

namespace MileageCustomisation.DAC
{
    public class EPExpenseClaimDetailsExt : PXCacheExtension<EPExpenseClaimDetails>
    {
        // Non-DB fields

        #region UsrVirtualEmployeeID
        [PXInt]
        [PXSelector(typeof(EPEmployee.bAccountID))]
        [PXFormula(typeof(EPExpenseClaimDetails.employeeID))]
        public virtual int? UsrVirtualEmployeeID { get; set; }
        public abstract class usrVirtualEmployeeID : BqlInt.Field<usrVirtualEmployeeID> { }
        #endregion

        #region UsrPaymentTermsID
        [PXString(10)]
        [PXUIField(DisplayName = "Payment Terms", Enabled = false)]
        [PXSelector(typeof(Terms.termsID), DescriptionField = typeof(Terms.descr))]
        [PXFormula(typeof(Selector<usrVirtualEmployeeID, EPEmployee.termsID>))]
        public virtual string UsrPaymentTermsID { get; set; }
        public abstract class usrPaymentTermsID : BqlString.Field<usrPaymentTermsID> { }
        #endregion

        #region UsrPLDepartmentID
        [PXString(10)]
        [PXUIField(DisplayName = "Department ID", Enabled = false)]
        [PXSelector(typeof(EPDepartment.departmentID), DescriptionField = typeof(EPDepartment.description))]
        [PXFormula(typeof(Selector<usrVirtualEmployeeID, EPEmployee.departmentID>))]
        public virtual string UsrPLDepartmentID { get; set; }
        public abstract class usrPLDepartmentID : BqlString.Field<usrPLDepartmentID> { }
        #endregion

        #region UsrMileageType
        [PXInt]
        [PXUIField(DisplayName = "Mileage Type", Enabled = false)]
        [PXSelector(typeof(ArcMileageType.mileageID),
            SubstituteKey = typeof(ArcMileageType.mileageCD),
            DescriptionField = typeof(ArcMileageType.description))]
        [PXFormula(typeof(Selector<usrVirtualEmployeeID, EPEmployeeExt.usrMileageType>))]
        public virtual int? UsrMileageType { get; set; }
        public abstract class usrMileageType : BqlInt.Field<usrMileageType> { }
        #endregion

        #region UsrReceiptAttached
        [PXBool]
        [PXUIField(DisplayName = "Receipt Attached", Enabled = false)]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [ClaimDetailsReceiptAttached]
        public virtual bool? UsrReceiptAttached { get; set; }
        public abstract class usrReceiptAttached : BqlBool.Field<usrReceiptAttached> { }
        #endregion

        #region UsrTotalRate
        [PXDecimal(4)]
        [PXUIField(DisplayName = "Total Rate", Enabled = false)]
        [PXFormula(typeof(Selector<usrMileageType, ArcMileageType.rate>))]
        public virtual decimal? UsrTotalRate { get; set; }
        public abstract class usrTotalRate : BqlDecimal.Field<usrTotalRate> { }
        #endregion

        #region UsrWearAndTearRate
        [PXDecimal(4)]
        [PXUIField(DisplayName = "Wear and Tear Rate", Enabled = false)]
        [PXFormula(typeof(Selector<usrMileageType, ArcMileageType.wearAndTearPart>))]
        public virtual decimal? UsrWearAndTearRate { get; set; }
        public abstract class usrWearAndTearRate : BqlDecimal.Field<usrWearAndTearRate> { }
        #endregion

        // DB fields

        #region UsrReasonForVariance
        [PXDBString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Reason for Variance")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual string UsrReasonForVariance { get; set; }
        public abstract class usrReasonForVariance : BqlString.Field<usrReasonForVariance> { }
        #endregion

        #region UsrFromLocation
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "From Location")]
        public virtual string UsrFromLocation { get; set; }
        public abstract class usrFromLocation : BqlString.Field<usrFromLocation> { }
        #endregion

        #region UsrToLocation
        [PXDBString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "To Location")]
        public virtual string UsrToLocation { get; set; }
        public abstract class usrToLocation : BqlString.Field<usrToLocation> { }
        #endregion

        #region UsrCalculatedMileage
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Calculated Mileage", Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? UsrCalculatedMileage { get; set; }
        public abstract class usrCalculatedMileage : BqlDecimal.Field<usrCalculatedMileage> { }
        #endregion

        #region UsrClaimedMileage
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Claimed Mileage")]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? UsrClaimedMileage { get; set; }
        public abstract class usrClaimedMileage : BqlDecimal.Field<usrClaimedMileage> { }
        #endregion

        #region UsrRoundTrip
        [PXDBBool]
        [PXUIField(DisplayName = "Round Trip")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrRoundTrip { get; set; }
        public abstract class usrRoundTrip : BqlBool.Field<usrRoundTrip> { }
        #endregion

    }
}
