using PX.Data;
using PX.Data.BQL;
using PX.Objects.AM;
using PX.Objects.AP;
using PX.TM;

namespace MainProject.DAC
{
    public class AMBomOperExt : PXCacheExtension<AMBomOper>
    {
        #region UsrLeadDays
        [PXDBInt]
        [PXUIField(DisplayName = "Lead Days")]
        public virtual int? UsrLeadDays { get; set; }
        public abstract class usrLeadDays : PX.Data.BQL.BqlInt.Field<usrLeadDays> { }
        #endregion
        #region UsrOperationOwner
        [Owner(DisplayName = "Operation Owner")]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrOperationOwner { get; set; }
        public abstract class usrOperationOwner : PX.Data.BQL.BqlInt.Field<usrOperationOwner> { }
        #endregion
        #region UsrAMWCLocationID
        [PXDBInt]
        [PXUIField(DisplayName = "Location")]
        [PXSelector(typeof(Search<ArcAMWCLocations.locationID,
            Where<ArcAMWCLocations.active, Equal<True>,
                And<ArcAMWCLocations.wCID, Equal<Current<AMBomOper.wcID>>>>>),
            DescriptionField = typeof(ArcAMWCLocations.locationCD),
            SubstituteKey = typeof(ArcAMWCLocations.description))]
        public virtual int? UsrAMWCLocationID { get; set; }
        public abstract class usrAMWCLocationID : PX.Data.BQL.BqlInt.Field<usrAMWCLocationID> { }
        #endregion
    }
}
