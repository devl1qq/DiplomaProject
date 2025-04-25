using PX.Api;
using PX.Common;
using PX.Data;
using PX.Objects.AM;
using PX.Objects.AM.Attributes;
using PX.Objects.AP;
using PX.Objects.CN.Common.Extensions;
using PX.Objects.CR;
using PX.Objects.PO;
using MainProject.DAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PX.Data.BQL.Fluent.FbqlJoins;
using static PX.Objects.AM.AMBomOperCury.FK;
using static MainProject.DAC.ArcCreateNC;

namespace MainProject.Graph
{
    public class ProdMaintExt : PXGraphExtension<ProdMaint>
    {
        #region Views
        [PXHidden]
        [PXVirtualDAC]
        [PXCopyPasteHiddenView]
        [PXReadOnlyView]
        public PXSelectOrderBy<ArcCreateNC, OrderBy<Asc<ArcCreateNC.lotSerialNbr>>> splitsCreateNC;
        public PXSetup<CRSetup> CRSetup;
        public IEnumerable SplitsCreateNC()
        {
            if (splitsCreateNC.Cache.Cached.Count() == 0)
            {
                foreach (AMProdItemSplit prodSplit in Base.splits.Select())
                    splitsCreateNC.Cache.Insert(CopyNCFromLineDetailsSplit(prodSplit));

                var batchSplits = GetRelatedAMMTranSplitsFromEvents();
                
                foreach(var batchSplit in batchSplits)
                {
                    if (GetRelatedReceiptSplitFromBatch(batchSplit) != null)
                        splitsCreateNC.Cache.Insert(CopyNCFromReceiptSplit(GetRelatedReceiptSplitFromBatch(batchSplit), batchSplit));
                }
            }
            splitsCreateNC.Cache.IsDirty = false;
            foreach (var row in splitsCreateNC.Cache.Cached)
                yield return row;
        }
        #endregion
        #region Overrides
        [PXOverride]
        public virtual IEnumerable productionDetails(PXAdapter adapter, Func<PXAdapter, IEnumerable> baseHandler)
        {
            try
            {
                return baseHandler.Invoke(adapter);
            }
            catch(PXRedirectRequiredException e)
            {
                if(e.Graph is ProdDetail graph)
                {
                    var prodOpers = Base.ProdOperRecords.Select().RowCast<AMProdOper>().ToList();
                    if (Base.ProdItemSelected.Current.EstimateID != null && Base.ProdItemSelected.Current.BOMID == null)
                        UpdateProdOpersBaseOnEstimate(prodOpers);
                    if (Base.ProdItemSelected.Current.BOMID != null && Base.ProdItemSelected.Current.EstimateID == null)
                        UpdateProdOpersBaseOnBOM(prodOpers);
                }
                throw;
            }
        }
        #endregion
        #region Actions
        public PXAction<AMProdItem> CreateNC;
        [PXUIField(DisplayName = "CREATE NC", MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        public virtual IEnumerable createNC(PXAdapter adapter)
        {
            if (splitsCreateNC.AskExt() == WebDialogResult.OK)
            {
                var selectedSplits = splitsCreateNC
                    .Select()
                    .RowCast<ArcCreateNC>()
                    .Where(split => split.UsrSelected == true)
                    .ToList();
                foreach (var split in selectedSplits)
                {
                    if (split.CaseClassID.IsNullOrEmpty())
                    {
                        splitsCreateNC.Cache.RaiseExceptionHandling<ArcCreateNC.caseClassID>(split, split.CaseClassID,
                           new PXSetPropertyException<ArcCreateNC.caseClassID>(Helper.Constants.Messages.CaseClassCannotBeEmpty, PXErrorLevel.Error));
                        continue;
                    }
                    if (split.UsrNCQty > split.Qty)
                    {
                        splitsCreateNC.Cache.RaiseExceptionHandling<ArcCreateNC.usrNCQty>(split, split.UsrNCQty,
                            new PXSetPropertyException<ArcCreateNC.usrNCQty>(Helper.Constants.Messages.NCQtyGreaterThanQty, PXErrorLevel.Error));
                        continue;
                    }

                    CreateCase(split);
                }
            };
            return adapter.Get();
        }
        #endregion
        #region Events
        public void _(Events.RowSelected<AMProdItem> e)
        {
            var row = e.Row;
            if (row == null)
                return;

            PXUIFieldAttribute.SetEnabled<AMProdItemExt.usrTestReportID>(e.Cache, row, row.StatusID != ProductionOrderStatus.Locked && row.StatusID != ProductionOrderStatus.Closed);
        }
        #endregion
        #region Methods
        private void UpdateProdOpersBaseOnBOM(List<AMProdOper> prodOpers)
        {
            var bom = AMBomItem.PK.Find(Base, Base.ProdItemSelected.Current.BOMID, Base.ProdItemSelected.Current.BOMRevisionID);
            var bomGraph = PXGraph.CreateInstance<BOMMaint>();
            bomGraph.Documents.Current = bom;
            bomGraph.Documents.UpdateCurrent();
            var bomOpers = bomGraph.BomOperRecords.Select().RowCast<AMBomOper>().ToList();
            foreach (var bomOper in bomOpers)
            {
                var matchingProdOper = prodOpers.FirstOrDefault(prodOper => prodOper.OperationCD == bomOper.OperationCD || prodOper.OperationID == bomOper.OperationID);
                if (matchingProdOper != null)
                {
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrOperationOwner = bomOper.GetExtension<AMBomOperExt>().UsrOperationOwner;
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrLeadDays = bomOper.GetExtension<AMBomOperExt>().UsrLeadDays;
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrAMWCLocationID = bomOper.GetExtension<AMBomOperExt>().UsrAMWCLocationID;
                    Base.ProdOperRecords.Update(matchingProdOper);
                }
            }
            Base.Save.Press();
        }
        private void UpdateProdOpersBaseOnEstimate(List<AMProdOper> prodOpers)
        {
            var estimate = AMEstimateItem.PK.Find(Base, Base.ProdItemSelected.Current.EstimateID, Base.ProdItemSelected.Current.EstimateRevisionID);
            var estimateGraph = PXGraph.CreateInstance<EstimateMaint>();
            estimateGraph.Documents.Current = estimate;
            estimateGraph.Documents.UpdateCurrent();
            var estimateOpers = estimateGraph.EstimateOperRecords.Select().RowCast<AMEstimateOper>().ToList();
            foreach (var estimateOper in estimateOpers)
            {
                var matchingProdOper = prodOpers.FirstOrDefault(prodOper => prodOper.OperationCD == estimateOper.OperationCD || prodOper.OperationID == estimateOper.OperationID);
                if (matchingProdOper != null)
                {
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrOperationOwner = estimateOper.GetExtension<AMEstimateOperExt>().UsrOperationOwner;
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrLeadDays = estimateOper.GetExtension<AMEstimateOperExt>().UsrLeadDays;
                    matchingProdOper.GetExtension<AMProdOperExt>().UsrAMWCLocationID = estimateOper.GetExtension<AMEstimateOperExt>().UsrAMWCLocationID;
                    Base.ProdOperRecords.Update(matchingProdOper);
                }
            }
            Base.Save.Press();
        }
        private void CreateCase(ArcCreateNC split)
        {
            var caseGraph = PXGraph.CreateInstance<CRCaseMaint>();

            var relatedPOReceipt = POReceipt.PK.Find(Base, split.SplitType, split.RefSplitNbr);
            var receiptLine = POReceiptLine.PK.Find(Base, split.SplitType, split.RefSplitNbr, split.LineNbr);

            var crCase = new CRCase()
            {
                CaseClassID = split.CaseClassID,
                CustomerID = split.IsProdSplit == true ? null : relatedPOReceipt?.VendorID,
                LocationID = split.IsProdSplit == true ? null : relatedPOReceipt?.VendorLocationID,
                Subject = string.Format(Helper.Constants.Messages.CaseForProdOrder, Base.ProdMaintRecords.Current.ProdOrdID),
                ContactID = split.IsProdSplit == true ? null : GetBAccountContactID(relatedPOReceipt),
                OwnerID = GetCurrentUserContactID()
            };

            caseGraph.Case.Current = crCase;
            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            var caseExt = caseGraph.Case.Current.GetExtension<CRCaseExt>();

            caseExt.UsrProdOrdID = split.IsProdSplit == true ? Base.ProdMaintRecords.Current.NoteID : null;
            caseExt.UsrPOOrderID = split.IsProdSplit == false ? GetPOOrderNoteID(receiptLine) : null;

            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            var details = caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Select().RowCast<ArcSOPODetails>().FirstOrDefault();
            details.InventoryItemID = split.InventoryID;
            details.LotNbr = split.LotSerialNbr;
            details.NCQty = split.UsrNCQty ?? 0;
            details.SOPOLineNbr = split.IsProdSplit == true ? null : split.LineNbr;
            
            caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Update(details);
            caseGraph.Save.Press();
        }
        private int? GetCurrentUserContactID() =>
            PXSelect<Contact, Where<Contact.userID, Equal<Current<AccessInfo.userID>>>>.Select(Base).RowCast<Contact>().FirstOrDefault()?.ContactID ?? null;
        private int? GetBAccountContactID(POReceipt receipt)
        {
            var vendor = BAccount.PK.Find(Base, receipt.VendorID);
            int? contactID = PXSelect<Contact,
                    Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>
                    .Select(Base, vendor.BAccountID).RowCast<Contact>().Where(e => e.ContactType != ContactTypesAttribute.BAccountProperty).FirstOrDefault()?.ContactID;
            if (vendor?.PrimaryContactID != null)
                contactID = vendor.PrimaryContactID;

            return contactID;
        }
        private Guid? GetPOOrderNoteID(POReceiptLine receiptLine)
        {
            POOrderEntry pOOrderEntry = PXGraph.CreateInstance<POOrderEntry>();
            var poOrderNoteID = pOOrderEntry.Document.Search<POOrder.orderNbr>(receiptLine.PONbr, new object[1] { receiptLine.POType }).RowCast<POOrder>().FirstOrDefault()?.NoteID;

            return poOrderNoteID ?? null;
        }
        private ArcCreateNC CopyNCFromLineDetailsSplit(AMProdItemSplit prodSplit)
        {
            return new ArcCreateNC()
            {
                InventoryID = prodSplit.InventoryID,
                LotSerialNbr = prodSplit.LotSerialNbr,
                Qty = prodSplit.Qty ?? 0,
                UsrNCQty = 0,
                IsProdSplit = true
            };
        }
        private List<AMMTranSplit> GetRelatedAMMTranSplitsFromEvents()
        {
            return PXSelectJoin<AMMTranSplit, 
                   InnerJoin<AMMTran,
                   On<AMMTran.batNbr, Equal<AMMTranSplit.batNbr>,
                   And<AMMTran.docType, Equal<AMMTranSplit.docType>>>>,
                   Where<AMMTran.prodOrdID, Equal<Required<AMProdItem.prodOrdID>>>>
                   .Select(Base, Base.ProdMaintRecords.Current.ProdOrdID)
                   .RowCast<AMMTranSplit>()
                   .ToList();
        }
        private POReceiptLineSplit GetRelatedReceiptSplitFromBatch(AMMTranSplit batchSplit)
        {
            return PXSelect<POReceiptLineSplit,
                   Where<POReceiptLineSplit.lotSerialNbr, Equal<Required<AMMTranSplit.lotSerialNbr>>>>
                   .Select(Base, batchSplit.LotSerialNbr)
                   .RowCast<POReceiptLineSplit>()
                   .FirstOrDefault();
        }
        private ArcCreateNC CopyNCFromReceiptSplit(POReceiptLineSplit relatedReceipt, AMMTranSplit batchSplit)
        {
            return new ArcCreateNC()
            {
                RefSplitNbr = relatedReceipt.ReceiptNbr,
                SplitLineNbr = relatedReceipt.SplitLineNbr,
                LineNbr = relatedReceipt.LineNbr,
                SplitType = relatedReceipt.ReceiptType,
                InventoryID = relatedReceipt.InventoryID,
                LotSerialNbr = batchSplit.LotSerialNbr,
                Qty = batchSplit.Qty ?? 0,
                UsrNCQty = 0,
                IsProdSplit = false
            };
        }
        #endregion
    }
}
