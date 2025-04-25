using PX.Data;
using PX.Objects.AP;
using System;

namespace MainProject.DAC
{
    public class VendorExt : PXCacheExtension<Vendor>
    {
        #region UsrHoldReason
        [PXDBString(256)]
        [PXUIField(DisplayName = "Hold Reason")]
        public virtual string UsrHoldReason { get; set; }
        public abstract class usrHoldReason : PX.Data.BQL.BqlString.Field<usrHoldReason> { }
        #endregion
        #region UsrExternalNotes
        [PXDBString(1024)]
        [PXUIField(DisplayName = "External Notes")]
        public virtual string UsrExternalNotes { get; set; }
        public abstract class usrExternalNotes : PX.Data.BQL.BqlString.Field<usrExternalNotes> { }
        #endregion
        #region UsrLastOrderDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Order Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? UsrLastOrderDate { get; set; }
        public abstract class usrLastOrderDate : PX.Data.BQL.BqlDateTime.Field<usrLastOrderDate> { }
        #endregion
        #region UsrVendorScore
        [PXDBInt]
        [PXUIField(DisplayName = "Vendor Score")]
        [PXSelector(typeof(Search<ArcVendorScore.scoreID, Where<ArcVendorScore.active, Equal<True>>>),
            DescriptionField = typeof(ArcVendorScore.score),
            SubstituteKey = typeof(ArcVendorScore.priority))]
        public virtual int? UsrVendorScore { get; set; }
        public abstract class usrVendorScore : PX.Data.BQL.BqlInt.Field<usrVendorScore> { }
        #endregion
        #region UsrLastSurveyDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Last Survey Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? UsrLastSurveyDate { get; set; }
        public abstract class usrLastSurveyDate : PX.Data.BQL.BqlDateTime.Field<usrLastSurveyDate> { }
        #endregion
        #region UsrSurveyExpirationDate
        [PXDBDate()]
        [PXUIField(DisplayName = "Survey Expiration Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? UsrSurveyExpirationDate { get; set; }
        public abstract class usrSurveyExpirationDate : PX.Data.BQL.BqlDateTime.Field<usrSurveyExpirationDate> { }
        #endregion
        #region UsrVendorScoreValidation
        [PXDBBool()]
        [PXDefault(true)]
        [PXUIField(DisplayName = "Vendor Score Validation")]
        public virtual bool? UsrVendorScoreValidation { get; set; }
        public abstract class usrVendorScoreValidation : PX.Data.BQL.BqlBool.Field<usrVendorScoreValidation> { }
        #endregion
        #region UsrAllowBusinessFromBannedCountry
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allow Business From Banned Country")]
        public virtual bool? UsrAllowBusinessFromBannedCountry { get; set; }
        public abstract class usrAllowBusinessFromBannedCountry : PX.Data.BQL.BqlBool.Field<usrAllowBusinessFromBannedCountry> { }
        #endregion
        #region UsrRestricted
        [PXDBBool]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Restricted")]
        public virtual bool? UsrRestricted { get; set; }
        public abstract class usrRestricted : PX.Data.BQL.BqlBool.Field<usrRestricted> { }
        #endregion

        #region UsrSharePointURL
        [PXDBWeblink]
        [PXUIField(DisplayName = "SharePoint URL")]
        public virtual string UsrSharePointURL { get; set; }
        public abstract class usrSharePointURL : PX.Data.BQL.BqlString.Field<usrSharePointURL> { }
        #endregion

    }
}
