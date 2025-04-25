using PX.Data;
using PX.Objects.CR;

namespace MainProject.DAC
{
    public class BAccountExt : PXCacheExtension<BAccount>
    {
        #region UsrSectorID
        [PXDBInt]
        [PXUIField(DisplayName = "Sector")]
        [PXSelector(typeof(Search<ArcSector.codeID, Where<ArcSector.active, Equal<True>>>),
            DescriptionField = typeof(ArcSector.codeCD),
            SubstituteKey = typeof(ArcSector.description))]
        public virtual int? UsrSectorID { get; set; }
        public abstract class usrSectorID : PX.Data.BQL.BqlInt.Field<usrSectorID> { }
        #endregion
    }
}
