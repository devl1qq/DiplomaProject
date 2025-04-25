  using PX.Api;
using PX.Common;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.PO;
using MainProject.DAC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.CR.BusinessAccountMaint;

namespace MainProject.Graph
{
    public class POReceiptEntryExt : PXGraphExtension<POReceiptEntry>
    {
        #region Views
        [PXVirtualDAC]
        [PXReadOnlyView]
        public PXSelect<ArcCreateNC> splitsCreateNC;
        public IEnumerable SplitsCreateNC()
        {
            if (!splitsCreateNC.Cache.Cached.Any_())
            {
                foreach (POReceiptLineSplit receiptsplit in GetAllSplits(Base.Document.Current))
                    splitsCreateNC.Cache.Insert(CreateNCCopyFromSplit(receiptsplit));
            }

            splitsCreateNC.Cache.IsDirty = false;
            return splitsCreateNC.Cache.Cached;
        }
        public PXSetup<CRSetup> CRSetup;
        #endregion
        #region Events
        public void _(Events.RowSelected<POReceipt> e)
        {
            var row = e.Row;
            if (row == null)
                return;
            CreateNC.SetEnabled(Base.transactions.Select().Any() && !row.ReceiptNbr.Contains("<NEW>"));
        }
        #endregion
        #region Actions
        public PXAction<POReceipt> CreateNC;
        [PXUIField(DisplayName = "CREATE NC", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXButton(Connotation = PX.Data.WorkflowAPI.ActionConnotation.Success)]
        public virtual IEnumerable createNC(PXAdapter adapter)
        {
            if (GetPOOrderNoteID() == null)
                throw new PXException(Helper.Constants.Messages.POReceiptDoesNotHavePurchaseOrderLinked);
            
            if (splitsCreateNC.AskExt(InitCreateNC) == WebDialogResult.OK)
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
        #region Methods
        private int? GetBAccountContactID()
        {
            var vendor = BAccount.PK.Find(Base, Base.Document.Current.VendorID);
            int? contactID = PXSelect<Contact,
                    Where<Contact.bAccountID, Equal<Required<Contact.bAccountID>>>>
                    .Select(Base, vendor.BAccountID).RowCast<Contact>().Where(e => e.ContactType != ContactTypesAttribute.BAccountProperty).FirstOrDefault()?.ContactID;
            if (vendor?.PrimaryContactID != null)
                contactID = vendor.PrimaryContactID;

            return contactID;
        }
        private int? GetCurrentUserContactID() => 
            PXSelect<Contact, Where<Contact.userID, Equal<Current<AccessInfo.userID>>>>.Select(Base).RowCast<Contact>().FirstOrDefault()?.ContactID ?? null;
        private Guid? GetPOOrderNoteID()
        {
            POReceiptLine current = Base.transactions.Current;

            POOrderEntry pOOrderEntry = PXGraph.CreateInstance<POOrderEntry>();
            var poOrderNoteID = pOOrderEntry.Document.Search<POOrder.orderNbr>(current.PONbr, new object[1] { current.POType }).RowCast<POOrder>().FirstOrDefault()?.NoteID;

            return poOrderNoteID ?? null;
        }
        private void CreateCase(ArcCreateNC split)
        {
            var caseGraph = PXGraph.CreateInstance<CRCaseMaint>();
            var crCase = new CRCase()
            {
                CaseClassID = split.CaseClassID,
                CustomerID = Base.Document.Current.VendorID,
                ContactID = GetBAccountContactID(),
                LocationID = Base.Document.Current.VendorLocationID,
                Subject = string.Format(Helper.Constants.Messages.CaseForPO, Base.Document.Current.ReceiptNbr),
                OwnerID = GetCurrentUserContactID()
            };
            caseGraph.Case.Current = crCase;
            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            caseGraph.Case.Current.GetExtension<CRCaseExt>().UsrPOOrderID = GetPOOrderNoteID();
            caseGraph.Case.UpdateCurrent();
            caseGraph.Save.Press();

            var details = caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Select().RowCast<ArcSOPODetails>().FirstOrDefault();
            details.SOPOLineNbr = split.LineNbr;
            details.LotNbr = split.LotSerialNbr;
            details.InventoryItemID = split.InventoryID;
            details.NCQty = split.UsrNCQty ?? 0;

            caseGraph.GetExtension<CRCaseMaintExt>().SOPODetailsView.Update(details);

            caseGraph.Save.Press();
        }
        private void InitCreateNC(PXGraph graph, string s) => splitsCreateNC.Cache.Clear();
        private List<POReceiptLineSplit> GetAllSplits(POReceipt doc)
        {
            return PXSelect<POReceiptLineSplit,
                   Where<POReceiptLineSplit.receiptNbr, Equal<Required<POReceipt.receiptNbr>>,
                   And<POReceiptLineSplit.receiptType, Equal<Required<POReceipt.receiptType>>>>>
                   .Select(Base, doc?.ReceiptNbr, doc?.ReceiptType)
                   .RowCast<POReceiptLineSplit>()
                   .ToList();
        }
        private ArcCreateNC CreateNCCopyFromSplit(POReceiptLineSplit receiptsplit)
        {
            return new ArcCreateNC()
            {
                RefSplitNbr = receiptsplit.ReceiptNbr,
                SplitType = receiptsplit.ReceiptType,
                InventoryID = receiptsplit.InventoryID,
                LineNbr = receiptsplit.LineNbr,
                SplitLineNbr = receiptsplit.SplitLineNbr,
                LotSerialNbr = receiptsplit.LotSerialNbr,
                Qty = receiptsplit.Qty ?? 0,
                UsrNCQty = 0
            };
        }
        #endregion
    }
}
