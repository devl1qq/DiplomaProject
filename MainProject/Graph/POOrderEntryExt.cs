using PX.Api;
using PX.Data;
using PX.Objects.AP;
using PX.Objects.PO;
using PX.Objects.IN;
using MainProject.DAC;
using System.Linq;
using PX.Common;
using System.Collections;
using PX.Objects.AM;
using static PX.Objects.PO.POOrderEntry;
using static PX.Objects.PO.POCreate;
using PX.Objects.CR;

namespace MainProject.Graph
{
    public class POOrderEntryExt : PXGraphExtension<POOrderEntry>
    {
        #region Views
        [PXVirtualDAC]
        public PXFilter<POLineSplitForm> POLineSplitForm;
        #endregion
        #region Events
        public void _(Events.RowSelected<POOrder> e)
        {
            var row = e.Row;
            if (row == null)
                return;
            bool isTrackingNbrEnabled = row.Status == POOrderStatus.Hold ||
                row.Status == POOrderStatus.Open ||
                row.Status == POOrderStatus.PendingApproval ||
                row.Status == POOrderStatus.PendingPrint ||
                row.Status == POOrderStatus.PendingEmail;
            if(Base.Document.Cache.AllowUpdate == false || Base.Document.AllowUpdate == false)
            {
                Base.Document.AllowUpdate = true;
                Base.Document.Cache.AllowUpdate = true;
            }
            
            PXUIFieldAttribute.SetEnabled<POOrder.orderNbr>(e.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<POOrder.orderType>(e.Cache, row, true);
            PXUIFieldAttribute.SetEnabled<POOrderExt.usrTrackingNumber>(e.Cache, row, isTrackingNbrEnabled);

            
        }
        public void _(Events.RowSelected<POLineSplitForm> e)
        {
            var row = (POLineSplitForm)e.Row;
            if (row == null)
                return;

            var splitQtySum = POLineSplitForm.Cache.Cached.RowCast<POLineSplitForm>().Sum(split => split.POLineQuantity);
            if(Base.Transactions.Current != null)
                row.IsButtonEnabled = splitQtySum <= Base.Transactions.Current.OrderQty;
        }
        public void _(Events.FieldUpdated<POLineSplitForm.pOLineQuantity> e)
        {
            var row = (POLineSplitForm)e.Row;
            if (row == null)
                return;
            
            var splitQtySum = POLineSplitForm.Cache.Cached.RowCast<POLineSplitForm>().Sum(split => split.POLineQuantity);
            if (splitQtySum > Base.Transactions.Current.OrderQty)
            {
                POLineSplitForm.Cache.RaiseExceptionHandling<POLineSplitForm.pOLineQuantity>(
                   row,
                   row.POLineQuantity,
                   new PXSetPropertyException<POLineSplitForm.pOLineQuantity>(Helper.Constants.Messages.SplitSumGreaterThanPOLine, PXErrorLevel.Error));
            }
        }
        public void _(Events.RowDeleted<POLine> e)
        {
            var row = e.Row;
            if (e.Row == null)
                return;
            if(HasAnyVendorScore() != true)
                Base.releaseFromHold.SetEnabled(true);
        }
        public void _(Events.FieldUpdated<POLine.inventoryID> e)
        {
            var row = e.Row;
            if (e.Row == null)
                return;

            VendorScoreWarningValidation();
        }

        public void _(Events.RowSelected<POLine> e)
        {
            var row = e.Row;
            if (e.Row == null)
                return;
            bool isTrackingNbrEnabled = Base.Document.Current?.Status == POOrderStatus.Hold ||
                                        Base.Document.Current?.Status == POOrderStatus.Open ||
                                        Base.Document.Current?.Status == POOrderStatus.PendingApproval ||
                                        Base.Document.Current?.Status == POOrderStatus.PendingPrint ||
                                        Base.Document.Current?.Status == POOrderStatus.PendingEmail;

            row.GetExtension<POLineExt>().IsSplitPOLineEnable =
                isTrackingNbrEnabled && Base.Transactions.Select().Any();
            VendorScoreWarningValidation();
            if (Base.Transactions.Cache.AllowUpdate == false || Base.Transactions.AllowUpdate == false)
            {
                Base.Transactions.AllowUpdate = true;
                Base.Transactions.Cache.AllowUpdate = true;
            }

            PXUIFieldAttribute.SetEnabled<POLineExt.usrTrackingNumber>(Base.Transactions.Cache, null,
                isTrackingNbrEnabled);

            //Done like this to avoid CostLayer.dll usage
            var countryOfOrigin = Base.Transactions.Cache.GetValue(row, "UsrDimension2ID");
            var errorLevel = countryOfOrigin == null ? PXErrorLevel.Warning : PXErrorLevel.Undefined;
            var message = countryOfOrigin == null
                ? Helper.Constants.Messages.CountryOfOriginIsEmptyWarning
                : string.Empty;

            Base.Transactions.Cache.RaiseExceptionHandling("UsrDimension2ID",
                row,
                countryOfOrigin,
                new PXSetPropertyException(message, errorLevel));
        }

        public void _(Events.FieldUpdated<POOrderExt.usrVendorScoreReason> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            VendorScoreWarningValidation();
        }
        public void _(Events.FieldUpdated<POOrderExt.usrTrackingNumber> e)
        {
            var row = (POOrder)e.Row;
            if(row == null || e.NewValue == null) 
                return;

            var poLines = Base.Transactions.Select().RowCast<POLine>().ToList();
            foreach (POLine poLine in poLines)
            {
                poLine.GetExtension<POLineExt>().UsrTrackingNumber = (string)e.NewValue;
                Base.Transactions.Update(poLine);
            }
        }
        #endregion
        #region Actions
        public PXAction<POLine> SplitPOLine;
        [PXUIField(DisplayName = "Split PO Line", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable splitPOLine(PXAdapter adapter)
        {
            if (Base.Transactions.Current == null)
                return adapter.Get();
            
            var mainPOLine = Base.Transactions.Current;
            
            var dialogResult = POLineSplitForm.AskExt((graph, view) => {
                POLineSplitForm.Cache.Clear();
                POLineSplitForm.View.RequestRefresh();
                POLineSplitForm.Cache.Insert(new DAC.POLineSplitForm()
                {
                    POLineNbr = Base.Transactions.Current.LineNbr,
                    POLineQuantity = Base.Transactions.Current.OrderQty,
                    POLineTrackingNbr = Base.Transactions.Current.GetExtension<POLineExt>().UsrTrackingNumber,
                    POLinePromisedDate = Base.Transactions.Current.PromisedDate
                }); ;
            }, true);

            if (dialogResult == WebDialogResult.OK)
            {
                foreach (POLineSplitForm mainDetails in POLineSplitForm.Cache.Cached)
                {
                    if (mainDetails.POLineNbr != null)
                    {
                        Base.Transactions.Cache.SetValueExt<POLine.orderQty>(Base.Transactions.Current, mainDetails.POLineQuantity);
                        Base.Transactions.Cache.SetValueExt<POLineExt.usrTrackingNumber>(Base.Transactions.Current, mainDetails.POLineTrackingNbr);
                        Base.Transactions.Update(mainPOLine);
                        
                    }
                    else
                    {
                        var splittedPOLine = Base.Transactions.Insert(new POLine());
                        Base.Transactions.Cache.SetValueExt<POLine.inventoryID>(splittedPOLine, mainPOLine.InventoryID);
                        Base.Transactions.Cache.SetValueExt<POLine.siteID>(splittedPOLine, mainPOLine.SiteID);
                        Base.Transactions.Cache.SetValueExt<POLine.orderQty>(splittedPOLine, mainDetails.POLineQuantity);
                        Base.Transactions.Cache.SetValueExt<POLine.promisedDate>(splittedPOLine, mainDetails.POLinePromisedDate);
                        Base.Transactions.Cache.SetValueExt<POLineExt.usrTrackingNumber>(splittedPOLine, mainDetails.POLineTrackingNbr);
                        Base.Transactions.Update(splittedPOLine);
                    }
                }
                Base.Save.Press();
            }

            return adapter.Get();
        }

        #endregion
        #region Methods
        private bool? HasAnyVendorScore()
        {
            foreach (var line in Base.Transactions.Select().RowCast<POLine>())
            {
                var scoreItem = GetLowestVendorItemScore(line);
                if (scoreItem != null)
                    return true;
            }
            return false;
        }
        private void VendorScoreWarningValidation()
        {
            var anyVendorScore = HasAnyVendorScore();
            var currentVendorScore = Base.Document.Current.GetExtension<POOrderExt>().UsrVendorScore;
            var currentVendorPriority = ArcVendorScore.PK.Find(Base, currentVendorScore)?.Priority;

            if (currentVendorScore == null || anyVendorScore == false)
                return;

            bool isVendorReasonNotEmpty = !Base.Document.Current.GetExtension<POOrderExt>().UsrVendorScoreReason.IsNullOrEmpty();
            bool oneItemHasWarning = false;

            foreach (var line in Base.Transactions.Select().RowCast<POLine>())
            {
                var scoreItem = GetLowestVendorItemScore(line);
                if (scoreItem == null)
                    continue;

                if (currentVendorPriority > scoreItem)
                {
                    PXUIFieldAttribute.SetWarning<POLine.inventoryID>(Base.Transactions.Cache, line, Helper.Constants.Messages.HigherScoreItem);
                    oneItemHasWarning = true;
                }
            }

            if (oneItemHasWarning)
            {
                Base.releaseFromHold.SetEnabled(isVendorReasonNotEmpty);
                Base.printPurchaseOrder.SetEnabled(isVendorReasonNotEmpty);
            }
        }
        private int? GetLowestVendorItemScore(POLine row)
        {
            
            var invItem = InventoryItem.PK.Find(Base, row.InventoryID);
            var graph = PXGraph.CreateInstance<InventoryItemMaint>();
            graph.Item.Current = invItem;

            var vendorScore = graph.VendorItems
                .Select()
                .RowCast<Vendor>()
                .Select(vendor => ArcVendorScore.PK.Find(Base, vendor.GetExtension<VendorExt>().UsrVendorScore)?.Priority)
                .Where(score => score.HasValue)
                .Min();

            return vendorScore;
        }
        #endregion
        #region Override

        public delegate void FillPOLineFromDemandDelegate(POLine dest, POFixedDemand demand, string OrderType, SOLineSplit3 solinesplit);

        [PXOverride]
        public virtual void FillPOLineFromDemand(POLine dest, POFixedDemand demand, string OrderType, SOLineSplit3 solinesplit, FillPOLineFromDemandDelegate baseHandler)
        {
            baseHandler(dest, demand, OrderType, solinesplit);
            Base.Transactions.Cache.SetValueExt<POLineExt.usrSOLineNbr>(dest, demand?.LineNbr);
            Base.Transactions.Cache.SetValueExt<POLineExt.usrSONbr>(dest, demand?.OrderNbr);
            Base.Transactions.Cache.SetValueExt<POLineExt.usrSOType>(dest, demand?.OrderType);
        }

        #endregion
    }
}
