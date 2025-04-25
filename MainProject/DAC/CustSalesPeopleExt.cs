using PX.Data.BQL;
using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Objects.AR;

namespace MainProject.DAC
{
    public class CustSalesPeopleExt : PXCacheExtension<CustSalesPeople>
    {
        #region UsrRole
        [PXString(3)]
        [PXUIField(DisplayName = "Role", IsReadOnly = true)]
        [PXFormula(typeof(Selector<CustSalesPeople.salesPersonID, SalesPersonExt.usrRole>))]
        public virtual string UsrRole { get; set; }
        public abstract class usrRole : BqlString.Field<usrRole> { }
        #endregion

        #region UsrDefaultByRole
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Default By Role")]
        public virtual bool? UsrDefaultByRole { get; set; }
        public abstract class usrDefaultByRole : BqlBool.Field<usrDefaultByRole> { }
        #endregion
    }
}
