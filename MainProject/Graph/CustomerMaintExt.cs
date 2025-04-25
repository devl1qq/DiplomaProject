using PX.Common;
using PX.Data;
using PX.Objects.AR;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Graph
{
    public class CustomerMaintExt : PXGraphExtension<CustomerMaint>
    {
        public void _(Events.FieldUpdated<CustSalesPeopleExt.usrDefaultByRole> e)
        {
            var row = (CustSalesPeople)e.Row;
            if (row == null || e.NewValue == null || (bool?)e.NewValue == false)
                return;

            var sameRoleSalesPersons = Base.SalesPersons
                .Select()
                .RowCast<CustSalesPeople>()
                .Where(per => per.GetExtension<CustSalesPeopleExt>().UsrRole == row.GetExtension<CustSalesPeopleExt>().UsrRole && per.SalesPersonID != row.SalesPersonID);

            foreach (var person in sameRoleSalesPersons)
            {
                person.GetExtension<CustSalesPeopleExt>().UsrDefaultByRole = false;
                Base.SalesPersons.Update(person);
            }
            Base.SalesPersons.View.RequestRefresh();
        }
    }
}
