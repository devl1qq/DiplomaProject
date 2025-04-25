using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.SO;
using MainProject.Helper;
using System.Collections;
using System.Linq;
using static PX.Objects.CR.QuoteMaint;
using PX.Objects.CR.Extensions.CRCreateSalesOrder;
using MainProject.DAC;
using PX.Objects.GL;
using PX.Objects.GL.DAC;
using PX.Objects.AM;
using CROpportunityProductsExt = MainProject.DAC.CROpportunityProductsExt;
using CRQuoteExt = MainProject.DAC.CRQuoteExt;
using SOOrderExt = MainProject.DAC.SOOrderExt;
using PX.Objects.AM.GraphExtensions;
using PX.Objects.IN;



namespace MainProject.Graph
{
    public class QuoteMaintExt : PXGraphExtension<CRCreateSalesOrderExt, QuoteMaintAMExtension, QuoteMaint>
    {
        #region Events
        public void _(Events.FieldUpdated<CROpportunityProducts.pOCreate> e, PXFieldUpdated baseHandler)
        {
            baseHandler?.Invoke(e.Cache, e.Args);
            CROpportunityProducts row = (CROpportunityProducts)e.Row;

            if (row == null) return;

            CRQuote quote = Base.Quote.Current;

            Branch branch = Branch.PK.Find(Base, quote.BranchID);
            if (branch == null) return;
            
            Organization organization = Organization.PK.Find(Base, branch.OrganizationID); 
            if (organization == null) return;

            var organizationExt = organization?.GetExtension<OrganizationExt>();

            if (organizationExt?.UsrDefaultVendorOnSalesTransactions == true && row.POCreate == true)
                row.VendorID = null;
        }
        public void _(Events.FieldUpdated<CRQuote.bAccountID> e)
        {
            var row = (CRQuote)e.Row;
            if (row == null)
                return;
            UpdateSalesPersons(row);
        }
        public void _(Events.FieldUpdated<CRQuote.opportunityID> e, PXFieldUpdated baseHandler)
        {
            var row = (CRQuote)e.Row;
            if (row == null)
                return;
            
            baseHandler.Invoke(e.Cache, e.Args);
            UpdateSalesPersons(row, CROpportunity.PK.Find(Base, row.OpportunityID).BAccountID);
        }

        public void _(Events.FieldUpdated<CROpportunityProductsExt.usrEstimateID> e)
        {
            var row = (CROpportunityProducts)e.Row;
            if(row == null || e.NewValue == null) 
                return;
            var rowExt = row.GetExtension<CROpportunityProductsExt>();
            var revisionID =
                PXSelect<AMEstimateReference, Where<AMEstimateReference.quoteNbr,
                    Equal<Required<CROpportunityProductsExt.usrQuoteNbr>>,
                        And<AMEstimateReference.estimateID, Equal<Required<CROpportunityProductsExt.usrEstimateID>>>>>
                    .Select(Base, rowExt.UsrQuoteNbr, e.NewValue)?
                    .RowCast<AMEstimateReference>()?
                    .FirstOrDefault()?
                    .RevisionID;
            rowExt.UsrEstimateRevisionID = revisionID;
        }
        #endregion
        #region Views
        public PXFilter<CRSiteStatusFilter> sitestatusfilter;

        [PXFilterable]
        [PXCopyPasteHiddenView]
        public CRSiteStatusLookup<CRSiteStatusSelected, CRSiteStatusFilter> sitestatus;
        #endregion
        #region Overrides
        public delegate void DoCreateSalesOrderDelegate(SOOrderEntry docgraph, Document masterEntity, CreateSalesOrderFilter filter);
        [PXOverride]
        public virtual void DoCreateSalesOrder(SOOrderEntry docgraph, Document masterEntity, CreateSalesOrderFilter filter, DoCreateSalesOrderDelegate baseHandler)
        {
            try
            {
                baseHandler(docgraph, masterEntity, filter);
            }
            catch (PXRedirectRequiredException ex)
            {
                UpdateSOOrderEntryValues(docgraph);
                throw ex;
            }

            UpdateSOOrderEntryValues(docgraph);
            docgraph.Save.Press();
        }
        public delegate EstimateMaint AddEstimateToQuoteDelegate(AMOrderEstimateItemFilter estimateItemFilter, CRQuote primaryRow);
        [PXOverride]
        public virtual EstimateMaint AddEstimateToQuote(AMOrderEstimateItemFilter estimateItemFilter, CRQuote primaryRow, AddEstimateToQuoteDelegate baseHandler)
        {
            var graph = baseHandler(estimateItemFilter, primaryRow);
            var currentEstimate = graph.Documents.Current;
            currentEstimate.GetExtension<AMEstimateItemExt>().UsrBAccountID = graph.EstimateReferenceRecord.Current.BAccountID;

            var oldInventoryItem = currentEstimate.InventoryID;
            
            graph.Documents.Cache.SetValueExt<AMEstimateItem.inventoryID>(currentEstimate, null);
            graph.Documents.Cache.SetValueExt<AMEstimateItem.inventoryID>(currentEstimate, oldInventoryItem);
            
            graph.Save.Press();
            return graph;
        }
        #endregion
        #region Actions


        public PXAction<CROpportunityProducts> AddInvBySite;

        [PXUIField(DisplayName = "Add Items", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select)]
        [PXLookupButton]
        public virtual IEnumerable addInvBySite(PXAdapter adapter)
        {
            if (sitestatus.AskExt((PXGraph g, string viewName) => sitestatusfilter.Cache.Clear()) ==
                WebDialogResult.OK)
            {
                AddInvSelBySite.Press();
            }

            sitestatusfilter.Cache.Clear();
            sitestatus.Cache.Clear();

            return adapter.Get();
        }

        public PXAction<CROpportunityProducts> AddInvSelBySite;

        [PXUIField(DisplayName = "Add", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select,
            Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable addInvSelBySite(PXAdapter adapter)
        {
            Base.Products.Cache.ForceExceptionHandling = true;

            foreach (CRSiteStatusSelected line in sitestatus.Cache.Cached)
            {
                if (line.Selected == true)
                {
                    CROpportunityProducts newline =
                       PXCache<CROpportunityProducts>.CreateCopy(Base.Products.Insert(new CROpportunityProducts()));

                    newline.SiteID = line.SiteID ?? newline.SiteID;
                    newline.InventoryID = line.InventoryID;

                    if (line.SubItemID != null)
                        newline.SubItemID = line.SubItemID;

                    newline.UOM = line.SalesUnit;
                    newline.Quantity = line.QtySelected;

                    Base.Products.Update(newline);
                }
            }
            sitestatus.Cache.Clear();

            return adapter.Get();
        }
        #endregion
        #region Methods
        private void UpdateSalesPersons(CRQuote row, int? BAccountID = null)
        {
            var rowExt = row.GetExtension<CRQuoteExt>();

            var allSalesPersons = PXSelect<CustSalesPeople,
                Where<CustSalesPeople.bAccountID, Equal<Required<Customer.bAccountID>>>>
                .Select(Base, BAccountID ?? row.BAccountID)
                .RowCast<CustSalesPeople>()
                .ToList();
            foreach (var person in allSalesPersons)
            {
                var salesPerson = SalesPerson.PK.Find(Base, person?.SalesPersonID);
                var salesPersonext = salesPerson?.GetExtension<SalesPersonExt>();
                var personExt = person.GetExtension<CustSalesPeopleExt>();
                if (salesPersonext?.UsrRole == "BDM" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrBDM = person?.SalesPersonID;
                if (salesPersonext?.UsrRole == "KAM" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrKAM = person?.SalesPersonID;
                if (salesPersonext?.UsrRole == "CSR" && personExt?.UsrDefaultByRole == true)
                    rowExt.UsrCSR = person?.SalesPersonID;
            }
        }
        private void UpdateSOOrderEntryValues(SOOrderEntry docgraph)
        {
            var salesPersonID = PXSelect<SalesPerson,
                    Where<SalesPerson.salesPersonCD, Equal<Required<CRQuoteExt.usrSalesPerson>>>>
                    .Select(docgraph, Base.Quote.Current.GetExtension<CRQuoteExt>().UsrSalesPerson)
                    .RowCast<SalesPerson>()
                    .FirstOrDefault()?.SalesPersonID;

            docgraph.Document.Current.GetExtension<SOOrderExt>().UsrFlowDownNotes = Base.Quote.Current.GetExtension<CRQuoteExt>().UsrFlowDownNotes;
            docgraph.Document.Current.GetExtension<SOOrderExt>().UsrBDM = Base.Quote.Current.GetExtension<CRQuoteExt>().UsrBDM;
            docgraph.Document.Current.GetExtension<SOOrderExt>().UsrKAM = Base.Quote.Current.GetExtension<CRQuoteExt>().UsrKAM;
            docgraph.Document.Current.GetExtension<SOOrderExt>().UsrCSR = Base.Quote.Current.GetExtension<CRQuoteExt>().UsrCSR;
            docgraph.Document.Current.SalesPersonID = salesPersonID;
            docgraph.Document.UpdateCurrent();
            foreach (SOLine line in docgraph.Transactions.Select())
            {

                var relatedProduct = Base.Products.Select()
                    .RowCast<CROpportunityProducts>()
                    .Where(i => i.SortOrder == line.SortOrder)
                    .FirstOrDefault();
                docgraph.Transactions.Cache.SetValue<SOLine.alternateID>(line, relatedProduct?.GetExtension<CROpportunityProductsExt>().UsrAlternateID);
                if(line.GetExtension<PX.Objects.AM.CacheExtensions.SOLineExt>().AMEstimateID != null)
                {
                    var alternateID = AMEstimateItem.PK.Find(Base,
                                            line.GetExtension<PX.Objects.AM.CacheExtensions.SOLineExt>().AMEstimateID,
                                            line.GetExtension<PX.Objects.AM.CacheExtensions.SOLineExt>().AMEstimateRevisionID)?
                                            .GetExtension<AMEstimateItemExt>()?
                                            .UsrAlternateID;
                    docgraph.Transactions.Cache.SetValue<SOLine.alternateID>(line, alternateID);
                }

            }
        }
        #endregion
    }
}
