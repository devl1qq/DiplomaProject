using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRLocation = PX.Objects.CR.Standalone.Location;


namespace MainProject.Graph
{
    public class BusinessAccountMaintExt : PXGraphExtension<BusinessAccountMaint>
    {
        #region Cache Attached
        #region CustSalesPeople
        [SalesPerson(IsKey = true, DescriptionField = typeof(SalesPerson.descr))]
        [PXParent(typeof(Select<SalesPerson, Where<SalesPerson.salesPersonID, Equal<Current<CustSalesPeople.salesPersonID>>>>))]
        public virtual void CustSalesPeople_SalesPersonID_CacheAttached(PXCache sender)
        {
        }
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(BAccount.bAccountID))]
        [PXParent(typeof(Select<Customer, Where<Customer.bAccountID, Equal<Current<CustSalesPeople.bAccountID>>>>))]
        public virtual void CustSalesPeople_BAccountID_CacheAttached(PXCache sender)
        {
        }
        [PXDBInt(IsKey = true)]
        [PXUIField(DisplayName = "Location ID", Visibility = PXUIVisibility.Visible)]
        [PXDimensionSelector(LocationIDAttribute.DimensionName, typeof(Search<CRLocation.locationID, Where<CRLocation.bAccountID,
            Equal<Current<CustSalesPeople.bAccountID>>>>), typeof(CRLocation.locationCD),
            typeof(Location.locationCD), typeof(Location.descr),
            DirtyRead = true, DescriptionField = typeof(CRLocation.descr))]
        [PXDefault(typeof(Search<Customer.defLocationID, Where<Customer.bAccountID, Equal<Current<CustSalesPeople.bAccountID>>>>))]
        public virtual void CustSalesPeople_LocationID_CacheAttached(PXCache sender)
        {
        }
        [PXDBDecimal(6)]
        [PXDefault(typeof(Search<SalesPerson.commnPct, Where<SalesPerson.salesPersonID, Equal<Current<CustSalesPeople.salesPersonID>>>>))]
        [PXUIField(DisplayName = "Commission %")]
        public virtual void CustSalesPeople_CommisionPct_CacheAttached(PXCache sender)
        {
        }
        #endregion
        #endregion
        #region Views
        [PXCopyPasteHiddenView]
        public PXSelect<CustSalesPeople, Where<CustSalesPeople.bAccountID, Equal<Current<BAccount.bAccountID>>>, OrderBy<Asc<CustSalesPeople.salesPersonID, Asc<CustSalesPeople.locationID>>>> SalesPersons;
        #endregion
        #region Events
        public void _(Events.FieldUpdated<CustSalesPeople.isDefault> e)
        {
            var row = (CustSalesPeople)e.Row;
            if (row == null || e.NewValue == null || (bool?)e.NewValue == false)
                return;

            var sameRoleSalesPersons = SalesPersons
                .Select()
                .RowCast<CustSalesPeople>()
                .Where(per => per.SalesPersonID != row.SalesPersonID);

            foreach (var person in sameRoleSalesPersons)
            {
                person.IsDefault = false;
                SalesPersons.Update(person);
            }
            SalesPersons.View.RequestRefresh();
        }
        public void _(Events.FieldUpdated<CustSalesPeopleExt.usrDefaultByRole> e)
        {
            var row = (CustSalesPeople)e.Row;
            if (row == null || e.NewValue == null || (bool?)e.NewValue == false)
                return;

            var sameRoleSalesPersons = SalesPersons
                .Select()
                .RowCast<CustSalesPeople>()
                .Where(per => per.GetExtension<CustSalesPeopleExt>().UsrRole == row.GetExtension<CustSalesPeopleExt>().UsrRole && per.SalesPersonID != row.SalesPersonID);

            foreach (var person in sameRoleSalesPersons)
            {
                person.GetExtension<CustSalesPeopleExt>().UsrDefaultByRole = false;
                SalesPersons.Update(person);
            }
            SalesPersons.View.RequestRefresh();
        }
        #endregion
    }
}
