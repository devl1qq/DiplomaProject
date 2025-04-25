using PX.Data;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using PX.Objects.AM;
using PX.Objects.AM.Attributes;
using static MainProject.DAC.CROpportunityProductsExt;

namespace MainProject.DAC
{
    public class CROpportunityProductsExt : PXCacheExtension<CROpportunityProducts>
    {
        #region UsrSelectorQuoteID
        [PXGuid]
        [PXFormula(typeof(CROpportunityProducts.quoteID))]
        [PXSelector(typeof(Search<CRQuote.noteID>))]
        public virtual Guid? UsrSelectorQuoteID { get; set; }
        public abstract class usrSelectorQuoteID : PX.Data.BQL.BqlGuid.Field<usrSelectorQuoteID> { }
        #endregion
        #region UsrQuoteNbr
        [PXString]
        [PXFormula(typeof(Selector<usrSelectorQuoteID, CRQuote.quoteNbr>))]
        public virtual string UsrQuoteNbr { get; set; }
        public abstract class usrQuoteNbr : PX.Data.BQL.BqlString.Field<usrQuoteNbr> { }
        #endregion
        #region UsrItemURL
        [PXString(100)]
        [PXUIField(DisplayName = "Item URL", Enabled = false, IsReadOnly = true)]
        [PXFormula(typeof(Selector<CROpportunityProducts.inventoryID, InventoryItemExt.usrItemURL> ))]
        public virtual string UsrItemURL { get; set; }
        public abstract class usrItemURL : PX.Data.BQL.BqlString.Field<usrItemURL> { }
        #endregion
        #region UsrExternalNotes
        [PXDBString(256)]
        [PXUIField(DisplayName = "External Notes")]
        public virtual string UsrExternalNotes { get; set; }
        public abstract class usrExternalNotes : PX.Data.BQL.BqlString.Field<usrExternalNotes> { }
        #endregion
        #region UsrLeadTime
        [PXDBString(50)]
        [PXUIField(DisplayName = "Lead Time")]
        public virtual string UsrLeadTime { get; set; }
        public abstract class usrLeadTime : PX.Data.BQL.BqlString.Field<usrLeadTime> { }
        #endregion
        #region UsrSupplierQuoteExpiryDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Supplier Quote Expiry Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? UsrSupplierQuoteExpiryDate { get; set; }
        public abstract class usrSupplierQuoteExpiryDate : PX.Data.BQL.BqlDateTime.Field<usrSupplierQuoteExpiryDate> { }
        #endregion
        #region UsrQuoteBranchID
        [PXInt]
        [PXFormula(typeof(Selector<CROpportunityProducts.quoteID, CRQuote.branchID>))]
        public virtual int? UsrQuoteBranchID { get; set; }
        public abstract class usrQuoteBranchID : PX.Data.BQL.BqlInt.Field<usrQuoteBranchID> { }
        #endregion
        #region UsrOrganizationID
        [PXInt]
        [PXFormula(typeof(Selector<usrQuoteBranchID, Branch.organizationID>))]
        public virtual int? UsrOrganizationID { get; set; }
        public abstract class usrOrganizationID : PX.Data.BQL.BqlInt.Field<usrOrganizationID> { }
        #endregion
        #region UsrSubItemID
        [PXDefault(typeof(Search<InventoryItem.defaultSubItemID,
            Where<InventoryItem.inventoryID, Equal<Current<CROpportunityProducts.inventoryID>>,
            And<InventoryItem.defaultSubItemOnEntry, Equal<boolTrue>>>>),
            PersistingCheck = PXPersistingCheck.Nothing)]
        [PXFormula(typeof(Default<CROpportunityProducts.inventoryID>))]
        [SubItem(typeof(CROpportunityProducts.inventoryID))]
        [SubItemStatusVeryfier(typeof(CROpportunityProducts.inventoryID), typeof(CROpportunityProducts.siteID), InventoryItemStatus.Inactive, InventoryItemStatus.NoSales)]
        public virtual int? UsrSubItemID { get; set; }
        public abstract class usrSubItemID : PX.Data.BQL.BqlInt.Field<usrSubItemID> { }
        #endregion
        #region UsrAlternateID
        [AlternativeItem(INPrimaryAlternateType.CPN, 
            typeof(CROpportunityProducts.customerID), 
            typeof(CROpportunityProducts.inventoryID), 
            typeof(usrSubItemID), 
            typeof(CROpportunityProducts.uOM))]
        public virtual string UsrAlternateID {  get; set; }
        public abstract class usrAlternateID : PX.Data.BQL.BqlString.Field<usrAlternateID> { }
        #endregion

        #region UsrEstimateID
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Estimate ID", Visibility = PXUIVisibility.SelectorVisible, FieldClass = "MFGESTIMATING")]
        [PXSelector(typeof(Search<AMEstimateReference.estimateID, Where<AMEstimateReference.quoteNbr, Equal<Current<usrQuoteNbr>>>>))]
        public virtual string UsrEstimateID { get; set; }
        public abstract class usrEstimateID : PX.Data.BQL.BqlString.Field<usrEstimateID> { }
        #endregion

        #region UsrEstimateRevisionID
        [PXDBString(10, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = "Revision ID", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<AMEstimateReference.revisionID, Where<AMEstimateReference.quoteNbr, Equal<Current<usrQuoteNbr>>>>))]
        public virtual string UsrEstimateRevisionID { get; set; }
        public abstract class usrEstimateRevisionID : PX.Data.BQL.BqlString.Field<usrEstimateRevisionID> { }
        #endregion
    }
}
