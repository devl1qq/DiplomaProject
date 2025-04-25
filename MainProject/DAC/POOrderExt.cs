using PX.Data;
using PX.Objects.PO;
using System;

namespace MainProject.DAC
{
    public class POOrderExt : PXCacheExtension<POOrder>
    {
        #region UsrChaseDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Chase Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? UsrChaseDate { get; set; }
        public abstract class usrChaseDate : PX.Data.BQL.BqlDateTime.Field<usrChaseDate> { }
        #endregion
        #region UsrPONotesID
        [PXDBInt]
        [PXUIField(DisplayName = "PO Notes")]
        [PXSelector(typeof(Search<ArcPONotes.codeID, Where<ArcPONotes.active, Equal<True>>>),
            DescriptionField = typeof(ArcPONotes.description),
            SubstituteKey = typeof(ArcPONotes.codeCD))]
        public virtual int? UsrPONotesID { get; set; }
        public abstract class usrPONotesID : PX.Data.BQL.BqlInt.Field<usrPONotesID> { }
        #endregion
        #region UsrVendorScoreReason
        [PXDBString(5)]
        [PXUIField(DisplayName = "Vendor Score Reason")]
        [PXSelector(typeof(Search<ArcVendorScoreReason.reasonCD, Where<ArcVendorScoreReason.active, Equal<True>>>),
            DescriptionField = typeof(ArcVendorScoreReason.reason),
            SubstituteKey = typeof(ArcVendorScoreReason.reasonCD))]
        public virtual string UsrVendorScoreReason { get; set; }
        public abstract class usrVendorScoreReason : PX.Data.BQL.BqlString.Field<usrVendorScoreReason> { }
        #endregion
        #region UsrTrackingNumber
        [PXDBString(100)]
        [PXUIField(DisplayName = "Tracking Number")]
        public virtual string UsrTrackingNumber { get; set; }
        public abstract class usrTrackingNumber : PX.Data.BQL.BqlString.Field<usrTrackingNumber> { }
        #endregion
        #region UsrVendorLastOrderDate
        [PXDate()]
        [PXFormula(typeof(Selector<POOrder.vendorID, VendorExt.usrLastOrderDate>))]
        [PXUIField(DisplayName = "Last Order Date", IsReadOnly = true)]
        public virtual DateTime? UsrVendorLastOrderDate { get; set; }
        public abstract class usrVendorLastOrderDate : PX.Data.BQL.BqlDateTime.Field<usrVendorLastOrderDate> { }
        #endregion
        #region UsrVendorLastSurveyDate
        [PXDate()]
        [PXFormula(typeof(Selector<POOrder.vendorID, VendorExt.usrLastSurveyDate>))]
        [PXUIField(DisplayName = "Last Survey Date", IsReadOnly = true)]
        public virtual DateTime? UsrVendorLastSurveyDate { get; set; }
        public abstract class usrVendorLastSurveyDate : PX.Data.BQL.BqlDateTime.Field<usrVendorLastSurveyDate> { }
        #endregion
        #region UsrVendorSurveyExpirationDate
        [PXDate()]
        [PXFormula(typeof(Selector<POOrder.vendorID, VendorExt.usrSurveyExpirationDate>))]
        [PXUIField(DisplayName = "Survey Expiration Date", IsReadOnly = true)]
        public virtual DateTime? UsrVendorSurveyExpirationDate { get; set; }
        public abstract class usrVendorSurveyExpirationDate : PX.Data.BQL.BqlDateTime.Field<usrVendorSurveyExpirationDate> { }
        #endregion
        #region UsrVendorScore
        [PXInt]
        [PXUIField(DisplayName = "Vendor Score", IsReadOnly = true)]
        [PXFormula(typeof(Selector<POOrder.vendorID, VendorExt.usrVendorScore>))]
        [PXSelector(typeof(Search<ArcVendorScore.scoreID, Where<ArcVendorScore.active, Equal<True>>>),
            DescriptionField = typeof(ArcVendorScore.score),
            SubstituteKey = typeof(ArcVendorScore.priority))]
        public virtual int? UsrVendorScore { get; set; }
        public abstract class usrVendorScore : PX.Data.BQL.BqlInt.Field<usrVendorScore> { }
        #endregion
        #region UsrWorkflowTypeID
        [PXDBInt]
        [PXUIField(DisplayName = "Workflow")]
        [PXSelector(typeof(Search<ArcWorkflowType.codeID, Where<ArcWorkflowType.active, Equal<True>>>),
            DescriptionField = typeof(ArcWorkflowType.description),
            SubstituteKey = typeof(ArcWorkflowType.codeCD))]
        public virtual int? UsrWorkflowTypeID { get; set; }
        public abstract class usrWorkflowTypeID : PX.Data.BQL.BqlInt.Field<usrWorkflowTypeID> { }
        #endregion
    }
}
