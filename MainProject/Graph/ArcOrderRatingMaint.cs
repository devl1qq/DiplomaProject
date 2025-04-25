using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcOrderRatingMaint : PXGraph<ArcOrderRatingMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcOrderRating> Ratings;
        public PXSavePerRow<ArcOrderRating> Save;
        public PXCancel<ArcOrderRating> Cancel;
    }
}
