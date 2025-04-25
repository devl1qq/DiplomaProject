using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.EP;
using static MileageCustomisation.DAC.EPExpenseClaimDetailsExt;
using static MileageCustomisation.DAC.EPExpenseClaimExt;

namespace MileageCustomisation.DAC
{
    public class EPExpenseClaimExt : PXCacheExtension<EPExpenseClaim>
    {
        #region UsrVirtualEmployeeID
        [PXInt]
        [PXSelector(typeof(EPEmployee.bAccountID))]
        [PXFormula(typeof(EPExpenseClaim.employeeID))]
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

        #region UsrEstimatedDueDate
        [PXDate]
        [PXUIField(DisplayName = "Estimated Due Date", Enabled = false)]
        [PXFormula(typeof(Add<EPExpenseClaim.docDate, Selector<usrPaymentTermsID, Terms.dayDue00>>))]
        public virtual DateTime? UsrEstimatedDueDate { get; set; }
        public abstract class usrEstimatedDueDate : BqlDateTime.Field<usrEstimatedDueDate> { }
        #endregion
    }
}
