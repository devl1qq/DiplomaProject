using PX.Data;
using PX.Objects.CR;
using PX.Objects.CR.CRCaseMaint_Extensions;
using PX.Objects.PO;
using PX.Objects.IN;
using PX.Objects.SO;
using MainProject.DAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Data.BQL.BqlPlaceholder;
using static MainProject.DAC.ArcSOPODetails;
using PX.Objects.AM;
using PX.Data.Update;

namespace MainProject.Graph
{
    public class CRCaseMaintExt : PXGraphExtension<CRCaseMaint>
    {
        #region Views
        public PXSelect<ArcSOPODetails, Where<ArcSOPODetails.refNoteID, Equal<Current<CRCase.noteID>>>> SOPODetailsView;

        public PXSelect<SOLine, Where<SOLine.orderNbr, Equal<Optional<ArcSOPODetails.relatedEntityOrderNbr>>>> SOLines;

        public PXSelect<POLine, Where<POLine.orderNbr, Equal<Optional<ArcSOPODetails.relatedEntityOrderNbr>>>> POLines;
        #endregion
        #region Cache Attached
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(CRMBAccountAttribute))]
        [CRMBAccount]
        public void _(Events.CacheAttached<CRCase.customerID> e) { }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(ContactRawAttribute))]
        [ContactRaw(typeof(CRCase.customerID), new System.Type[] { typeof(ContactTypesAttribute.employee) }, null, null, null, null, WithContactDefaultingByBAccount = true, DisplayName = "Case Contact")]

        public void _(Events.CacheAttached<CRCase.contactID> e) { }
        #endregion
        #region Events
        public void _(Events.RowSelected<CRCase> e)
        {
            var row = e.Row;
            if (row == null) return;

            var rowExt = row.GetExtension<CRCaseExt>();
            bool isNew = Base.CaseCurrent.Cache.GetStatus(row) == PXEntryStatus.Inserted;
            PXUIFieldAttribute.SetEnabled<CRCaseExt.usrPOOrderID>(e.Cache, row, rowExt.UsrIsVendor == true && !isNew);
            PXUIFieldAttribute.SetEnabled<CRCaseExt.usrSOOrderID>(e.Cache, row, !isNew);
            PXUIFieldAttribute.SetEnabled<CRCaseExt.usrProdOrdID>(e.Cache, row, !isNew);
            Base.Close.SetEnabled(rowExt.UsrDispositionID.HasValue && rowExt.UsrResolutionID.HasValue);
        }
        public void _(Events.RowSelected<ArcSOPODetails> e)
        {
            var row = e.Row;
            if (row == null) return;

            PXUIFieldAttribute.SetEnabled<ArcSOPODetails.sOPOLineNbr>(e.Cache, row, row.Type != "PX.Objects.AM.AMProdItem");
            PXUIFieldAttribute.SetEnabled<ArcSOPODetails.relatedEntityID>(e.Cache, row, row.Type != "PX.Objects.AM.AMProdItem");
        }
        public void _(Events.FieldUpdated<CRCase.customerID> e)
        {
            var row = (CRCase)e.Row;
            if (row == null) return;

            var rowExt = row.GetExtension<CRCaseExt>();
            var bAccount = BAccount.PK.Find(Base, row.CustomerID);

            if (row.CustomerID != (int?)e.OldValue)
            {
                var graphExt = Base.GetExtension<CRCaseMaint_CRRelationDetailsExt>();

                rowExt.UsrPOOrderID = null;
                rowExt.UsrSOOrderID = null;

                var relations = graphExt.Relations.Select();
                foreach (var relation in relations)
                    graphExt.Relations.Delete(relation);

                var details = SOPODetailsView.Select();
                foreach (var detail in details)
                    SOPODetailsView.Delete(detail);
            }

            rowExt.UsrIsVendor = bAccount == null || (bAccount.Type != BAccountType.VendorType && bAccount.Type != BAccountType.CombinedType) ? false : true;
        }
        public void _(Events.FieldUpdated<ArcSOPODetails.sOPOLineNbr> e)
        {
            var row = (ArcSOPODetails)e.Row;
            if (row == null) 
                return;

            UpdateInventoryItem(row);
        }
        public void _(Events.FieldUpdated<CRCaseExt.usrPOOrderID> e)
        {
            var row = (CRCase)e.Row;
            if (row == null) return;

            UpdateOrder<POOrder, POOrder.noteID>((Guid?)e.OldValue, (Guid?)e.NewValue, row, "PX.Objects.PO.POOrder", "PX.Objects.PO.POLine");
        }
        public void _(Events.FieldUpdated<CRCaseExt.usrSOOrderID> e)
        {
            var row = (CRCase)e.Row;
            if (row == null) return;

            UpdateOrder<SOOrder, SOOrder.noteID>((Guid?)e.OldValue, (Guid?)e.NewValue, row, "PX.Objects.SO.SOOrder", "PX.Objects.SO.SOLine");
        }
        public void _(Events.FieldUpdated<CRCaseExt.usrProdOrdID> e)
        {
            var row = (CRCase)e.Row;
            if (row == null) return;

            UpdateOrder<AMProdItem, AMProdItem.noteID>((Guid?)e.OldValue, (Guid?)e.NewValue, row, "PX.Objects.AM.AMProdItem", null, true);
        }
        public void _(Events.FieldUpdated<CRCaseExt.usrDiscrepancyTypeID> e)
        {
            var row = (CRCase)e.Row; 
            if (row == null || e.NewValue == null) return;

            Base.Case.Current.Severity = Base.Case.Current.GetExtension<CRCaseExt>().UsrDiscrepanceTypeSeverity;        
        }
        #region Field Selectings
        public void _(Events.FieldSelecting<ArcSOPODetails.typeLine> e)
        {
            var row = (ArcSOPODetails)e.Row;
            if (row == null) return;

            if (row.Type == "PX.Objects.SO.SOOrder")
                row.TypeLine = "PX.Objects.SO.SOLine";

            if (row.Type == "PX.Objects.PO.POOrder")
                row.TypeLine = "PX.Objects.PO.POLine";
        }
        public void _(Events.FieldSelecting<ArcSOPODetails.sOPOLineNbr> e)
        {
            var row = (ArcSOPODetails)e.Row;
            if (row == null) return;

            var state = PXFieldState.CreateInstance(e.ReturnState, typeof(Guid), false, true, 1);
            e.ReturnState = state;

            if (row.Type == "PX.Objects.SO.SOOrder")
            {
                state.ViewName = nameof(SOLines);
                state.DescriptionName = nameof(SOLine.lineNbr);
                state.SelectorMode = PXSelectorMode.DisplayModeHint;
                state.FieldList = new string[]
                {
                    nameof(SOLine.inventoryID),
                    nameof(SOLine.lineNbr),
                    nameof(SOLine.orderType),
                    nameof(SOLine.orderNbr),
                    nameof(SOLine.orderQty),
                    nameof(SOLine.salesPersonID)
                };
                state.HeaderList = new string[]
                {
                    PXUIFieldAttribute.GetDisplayName<SOLine.inventoryID>(SOLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<SOLine.lineNbr>(SOLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<SOLine.orderType>(SOLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<SOLine.orderNbr>(SOLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<SOLine.orderQty>(SOLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<SOLine.salesPersonID>(SOLines.Cache)
                };
            }
            else
            {
                state.ViewName = nameof(POLines);
                state.DescriptionName = nameof(POLine.lineNbr);
                state.FieldList = new string[]
                {
                    nameof(POLine.lineNbr),
                    nameof(POLine.orderType),
                    nameof(POLine.orderNbr),
                    nameof(POLine.orderQty),
                    nameof(POLine.inventoryID),
                };
                state.HeaderList = new string[]
                {
                    PXUIFieldAttribute.GetDisplayName<POLine.lineNbr>(POLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<POLine.orderType>(POLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<POLine.orderNbr>(POLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<POLine.orderQty>(POLines.Cache),
                    PXUIFieldAttribute.GetDisplayName<POLine.inventoryID>(POLines.Cache),
                };
            }
            state.Enabled = true;
        }
        #endregion
        #endregion
        #region Methods
        private void UpdateInventoryItem(ArcSOPODetails row)
        {
            if (row.Type == "PX.Objects.SO.SOOrder")
            {
                var soOrder = PXSelect<SOOrder, Where<SOOrder.noteID, Equal<Required<ArcSOPODetails.relatedEntityID>>>>.Select(Base, row.RelatedEntityID).RowCast<SOOrder>().FirstOrDefault();
                if (soOrder == null)
                    return;

                row.InventoryItemID = SOLine.PK.Find(Base, soOrder.OrderType, soOrder.OrderNbr, row.SOPOLineNbr)?.InventoryID;
            }
            if (row.Type == "PX.Objects.PO.POOrder")
            {
                var poOrder = PXSelect<POOrder, Where<POOrder.noteID, Equal<Required<ArcSOPODetails.relatedEntityID>>>>.Select(Base, row.RelatedEntityID).RowCast<POOrder>().FirstOrDefault();
                if (poOrder == null)
                    return;

                row.InventoryItemID = POLine.PK.Find(Base, poOrder.OrderType, poOrder.OrderNbr, row.SOPOLineNbr)?.InventoryID;
            }
        }
        private void UpdateOrder<TOrder, TOperand>(Guid? oldRelationNoteID, Guid? newOrderNoteID, CRCase row, string orderType, string orderLineType = null, bool isProdItem = false)
            where TOrder : class, IBqlTable, new()
            where TOperand : class, IBqlOperand
        {
            var graphExt = Base.GetExtension<CRCaseMaint_CRRelationDetailsExt>();

            if (!isProdItem)
            {
                var oldRelation = PXSelect<CRRelation,
                Where<CRRelation.targetNoteID, Equal<Required<CRRelation.targetNoteID>>>>
                .Select(Base, oldRelationNoteID)
                .RowCast<CRRelation>()
                .FirstOrDefault();
                if (oldRelation != null)
                    graphExt.Relations.Delete(oldRelation);
            }

            var oldDetails = PXSelect<ArcSOPODetails,
                Where<ArcSOPODetails.type, Equal<Required<ArcSOPODetails.type>>,
                    And<ArcSOPODetails.refNoteID, Equal<Required<ArcSOPODetails.refNoteID>>>>>
                .Select(Base, orderType, row.NoteID)
                .RowCast<ArcSOPODetails>()
                .FirstOrDefault();
            if (oldDetails != null)
                SOPODetailsView.Delete(oldDetails);

            var newOrder = PXSelect<TOrder,
                Where<TOperand, Equal<Required<SOOrder.noteID>>>>
                .Select(Base, newOrderNoteID)
                .RowCast<TOrder>()
                .FirstOrDefault();

            if (newOrder != null)
            {
                var noteIDProperty = newOrder.GetType().GetProperty("NoteID");
                if (noteIDProperty != null)
                {
                    var noteIDValue = noteIDProperty.GetValue(newOrder);

                    if(!isProdItem) 
                    {
                        graphExt.Relations.Insert(new CRRelation()
                        {
                            Role = CRRoleTypeList.Source,
                            TargetType = orderType,
                            TargetNoteID = (Guid?)noteIDValue,
                        });
                        SOPODetailsView.Insert(new ArcSOPODetails()
                        {
                            RefNoteID = row.NoteID,
                            Type = orderType,
                            TypeLine = orderLineType,
                            RelatedEntityID = (Guid?)noteIDValue,
                        });
                    }
                    else
                    {
                        var amInventoryItemID =
                            PXSelect<AMProdItem, Where<AMProdItem.noteID, Equal<Required<AMProdItem.noteID>>>>
                                .Select(Base, noteIDValue)
                                .RowCast<AMProdItem>()
                                .FirstOrDefault()
                                .InventoryID;
                        SOPODetailsView.Insert(new ArcSOPODetails()
                        {
                            RefNoteID = row.NoteID,
                            Type = orderType,
                            ProductionOrderNoteID = (Guid?)noteIDValue,
                            InventoryItemID = amInventoryItemID
                        });
                    }
                }
            }
        }
        #endregion
    }
}
