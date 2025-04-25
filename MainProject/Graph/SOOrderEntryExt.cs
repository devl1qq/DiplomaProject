using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.AM;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.GL.DAC;
using PX.Objects.IN;
using PX.Objects.SO;
using MainProject.DAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Constants = MainProject.Helper.Constants;
using SOLineSplitExt = MainProject.DAC.SOLineSplitExt;
using SOOrderExt = MainProject.DAC.SOOrderExt;

namespace MainProject.Graph
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        #region Views
        [PXReadOnlyView]
        public PXSelect<ArcSOCreateNCGrid, 
            Where<ArcSOCreateNCGrid.orderType, Equal<Current<SOOrder.orderType>>, 
            And<ArcSOCreateNCGrid.orderNbr, Equal<Current<SOOrder.orderNbr>>, 
            And<ArcSOCreateNCGrid.isAllocated, Equal<True>,
            And<ArcSOCreateNCGrid.lotSerialNbr, IsNotNull>>>>> splitsCreateNC;

        #endregion
        #region Events
        public void _(Events.RowSelected<SOOrder> e)
        {
            var row = e.Row;
            if (row == null) return;
            var rowExt = row.GetExtension<SOOrderExt>();
            if (row.GetExtension<SOOrderExt>().UsrHold == true)
                PXUIFieldAttribute.SetWarning<SOOrderExt.usrHold>(e.Cache, row, Constants.Messages.CustomerHold);
            if (row.Status == SOOrderStatus.Shipping)
            {
                PXUIFieldAttribute.SetEnabled<SOOrderExt.usrFlowDownNotes>(e.Cache, row, true);
                PXUIFieldAttribute.SetEnabled<SOOrderExt.usrOrderRating>(e.Cache, row, true);
            }
            PXUIFieldAttribute.SetEnabled<SOOrderExt.usrConfirmed>(e.Cache, row, row.Status == SOOrderStatus.Hold || row.Status == SOOrderStatus.Open);
            Base.releaseFromHold.SetEnabled(rowExt.UsrConfirmed == true && rowExt.UsrHold == false && row.Status == SOOrderStatus.Hold);
            Base.createShipmentIssue.SetEnabled(!(rowExt.UsrCustomerApproved == false && rowExt.UsrCustomerApproveSalesOrder == true) && rowExt.UsrHold == false);
            CreateNC.SetEnabled(Base.Transactions.Select().Any() && row.OrderNbr != "<NEW>" && row.Status != SOOrderStatus.Cancelled);
            splitsCreateNC.AllowUpdate = true;
        }
        public void _(Events.FieldUpdated<SOOrderExt.usrConfirmed> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            SOOrder row = (SOOrder)e.Row;

            if (row == null) return;

            SOOrderExt orderExt = row.GetExtension<SOOrderExt>();

            if (orderExt.UsrConfirmed == false)
            {
                Base.putOnHold.Press();
                orderExt.UsrDraft = "D";
            }
            else orderExt.UsrDraft = "C";
        }
        public void _(Events.FieldUpdated<SOOrder.salesPersonID> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            var row = e.Row;
            if (row == null || e.NewValue == null) return;

            foreach (var soLine in Base.Transactions.Select().RowCast<SOLine>())
            {
                soLine.SalesPersonID = (int?)e.NewValue;
                Base.Transactions.Update(soLine);
            }
        }
        public void _(Events.FieldUpdated<SOBillingAddress.countryID> e)
        {
            var row = (SOBillingAddress)e.Row;
            if (row == null || e.NewValue == null) return;

            var rowExt = Base.Document.Current.GetExtension<SOOrderExt>();

            var country = PXSelect<Country,
                Where<Country.countryID, Equal<Required<SOAddress.countryID>>>>
                .Select(Base, e.NewValue)
                .RowCast<Country>()
                .FirstOrDefault();
            if (country == null) return;

            var countryExt = country?.GetExtension<CountryExt>();
            rowExt.UsrBannedCountry = countryExt?.UsrBanned;
        }
        public void _(Events.RowUpdated<SOLine> e, PXRowUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            SOLine row = (SOLine)e.Row;

            if (row == null) return;

            SOOrder order = SOOrder.PK.Find(Base, row.OrderType, row.OrderNbr);
            if (order == null) return;

            Branch branch = Branch.PK.Find(Base, order?.BranchID);
            if (branch == null) return;

            Organization organization = Organization.PK.Find(Base, branch.OrganizationID);
            if (organization == null) return;

            var organizationExt = organization.GetExtension<OrganizationExt>();

            if (organizationExt?.UsrDefaultVendorOnSalesTransactions == true && row.POCreate == true)
                e.Cache.SetValueExt<SOLine.vendorID>(e.Row, null);

        }
        public void _(Events.FieldUpdated<SOOrder.customerID> e)
        {
            var row = (SOOrder)e.Row;
            if(row == null) 
                return;
            var rowExt = row.GetExtension<SOOrderExt>();

            var allSalesPersons = PXSelect<CustSalesPeople, 
                Where<CustSalesPeople.bAccountID, Equal<Required<Customer.bAccountID>>>>
                .Select(Base, row.CustomerID)
                .RowCast<CustSalesPeople>()
                .ToList();
            foreach(var person in allSalesPersons)
            {
                var salesPerson = SalesPerson.PK.Find(Base, person?.SalesPersonID);
                var salesPersonext = salesPerson?.GetExtension<SalesPersonExt>();
                var personExt = person.GetExtension<CustSalesPeopleExt>();
                if(salesPersonext?.UsrRole == "BDM" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrBDM = person?.SalesPersonID;
                if (salesPersonext?.UsrRole == "KAM" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrKAM = person?.SalesPersonID;
                if (salesPersonext?.UsrRole == "CSR" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrCSR = person?.SalesPersonID;
            }
        }
        #endregion
        #region Actions
        public PXAction<SOOrder> CreateNC;
        [PXUIField(DisplayName = "CREATE NC", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton(Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        public virtual IEnumerable createNC(PXAdapter adapter)
        {
            if (splitsCreateNC.AskExt() == WebDialogResult.OK)
            {
                var selectedSplits = splitsCreateNC
                    .Select()
                    .RowCast<ArcSOCreateNCGrid>()
                    .Where(split => split.GetExtension<SOLineSplitExt>().UsrSelected == true)
                    .ToList();
                foreach (var split in selectedSplits)
                {
                    var splitExt = split.GetExtension<SOLineSplitExt>();
                    if (splitExt.UsrCaseClassID.IsNullOrEmpty())
                    {
                        splitsCreateNC.Cache.RaiseExceptionHandling<SOLineSplitExt.usrCaseClassID>(split, splitExt.UsrCaseClassID,
                           new PXSetPropertyException<SOLineSplitExt.usrCaseClassID>(Helper.Constants.Messages.CaseClassCannotBeEmpty, PXErrorLevel.Error));
                        continue;
                    }

                    if(splitExt.UsrNCQty > split.Qty)
                    {
                        splitsCreateNC.Cache.RaiseExceptionHandling<SOLineSplitExt.usrNCQty>(split, splitExt.UsrNCQty,
                            new PXSetPropertyException<SOLineSplitExt.usrNCQty>(Helper.Constants.Messages.NCQtyGreaterThanQty, PXErrorLevel.Error));
                        continue;
                    }

                    CreateCase(split);
                }
            };
            return adapter.Get();
        }
        public PXAction<SOOrder> PrintProformaInvoice;
        [PXUIField(DisplayName = "Proforma Invoice")]
        [PXButton(DisplayOnMainToolbar = false, Category = "Printing and Emailing")]
        protected virtual IEnumerable printProformaInvoice(PXAdapter adapter)
        {
            SOOrder order = Base?.Document.Current;
            string orderType = string.Empty;
            string orderNbr = string.Empty;

            if (order != null && Base.Document.Cache.GetStatus(order) != PXEntryStatus.Inserted)
            {
                orderType = order.OrderType;
                orderNbr = order.OrderNbr;

                Base.Document.Update(order);
                Base.Save.Press();
            }

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["SOOrder.OrderType"] = orderType;
            parameters["SOOrder.OrderNbr"] = orderNbr;

            throw new PXReportRequiredException(parameters, "SO641011", PXBaseRedirectException.WindowMode.NewWindow, "Proforma Invoice");
        }
        #endregion

        #region Methods
        private int? GetCurrentUserContactID() =>
            PXSelect<Contact, Where<Contact.userID, Equal<Current<AccessInfo.userID>>>>.Select(Base).RowCast<Contact>().FirstOrDefault()?.ContactID ?? null;
        private Guid? GetSOOrderNoteID(SOLineSplit split)
        {
            var order = SOOrder.PK.Find(Base, split.OrderType, split.OrderNbr);
            return order?.NoteID ?? null;
        }
        private void CreateCase(ArcSOCreateNCGrid split)
        {
            var splitExt = split.GetExtension<SOLineSplitExt>();
            var caseGraph = PXGraph.CreateInstance<CRCaseMaint>();
            var crCase = new CRCase()
            {
                CustomerID = Base.Document.Current.CustomerID,
                CaseClassID = splitExt.UsrCaseClassID,
                LocationID = Base.Document.Current.CustomerLocationID,
                ContactID = Base.Document.Current.ContactID,
                Subject = string.Format(Helper.Constants.Messages.CaseForSO, Base.Document.Current.OrderNbr),
                OwnerID = GetCurrentUserContactID()
            };
            caseGraph.Case.Current = crCase;
            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            caseGraph.Case.Current.GetExtension<CRCaseExt>().UsrSOOrderID = GetSOOrderNoteID(split);
            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            var details = caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Select().RowCast<ArcSOPODetails>().FirstOrDefault();
            details.SOPOLineNbr = split.LineNbr;
            details.LotNbr = split.LotSerialNbr;
            details.InventoryItemID = split.InventoryID;
            details.NCQty = splitExt.UsrNCQty ?? 0;

            caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Update(details);

            caseGraph.Save.Press();
        }
        #endregion
    }
    public class ArcSOCreateNCGrid : SOLineSplit
    {
        [PXDBQuantity(typeof(uOM), typeof(baseQty))]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Quantity", IsReadOnly = true)]
        public virtual decimal? Qty { get; set; }

        [SOLotSerialNbrAttribute.SOAllocationLotSerialNbr(typeof(inventoryID), typeof(subItemID), typeof(siteID), typeof(locationID), typeof(SOLine.lotSerialNbr), typeof(costCenterID), Enabled = false)]
        [PXUIField(DisplayName = "Lot/Serial Nbr.", FieldClass = "LotSerial", IsReadOnly = true)]
        public virtual string LotSerialNbr { get; set; }
        
    }
}
