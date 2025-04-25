using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AM;
using PX.Objects.EP;

namespace MainProject.DAC
{
    public class AMClockTranExt : PXCacheExtension<AMClockTran>
    {
        #region UsrEmployeeName
        [PXString]
        [PXFormula(typeof(Selector<AMClockTran.employeeID, EPEmployee.acctName>))]
        [PXUIField(DisplayName = "Employee Name")]
        public virtual string UsrEmployeeName { get; set; }
        public abstract class usrEmployeeName : PX.Data.BQL.BqlString.Field<usrEmployeeName> { }
        #endregion

        #region UsrWCID
        [PXString]
        [PXFormula(typeof(AMClockTran.wcID))]
        [PXSelector(typeof(AMWC.wcID))]
        public virtual string UsrWCID { get; set; }
        public abstract class usrWCID : PX.Data.BQL.BqlString.Field<usrWCID> { }
        #endregion

        #region UsrWorkCentreDescription
        [PXString]
        [PXFormula(typeof(Selector<usrWCID, AMWC.descr>))]
        [PXUIField(DisplayName = "Work Centre Description")]
        public virtual string UsrWorkCentreDescription { get; set; }
        public abstract class usrWorkCentreDescription : PX.Data.BQL.BqlString.Field<usrWorkCentreDescription> { }
        #endregion

        #region UsrOperationDescription
        [PXString]
        [PXFormula(typeof(Selector<AMClockTran.operationID, AMProdOper.descr>))]
        [PXUIField(DisplayName = "Operation Description")]
        public virtual string UsrOperationDescription { get; set; }
        public abstract class usrOperationDescription : PX.Data.BQL.BqlString.Field<usrOperationDescription> { }
        #endregion
    }
}
