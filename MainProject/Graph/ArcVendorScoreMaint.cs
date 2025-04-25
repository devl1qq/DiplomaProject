using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcVendorScoreMaint : PXGraph<ArcVendorScoreMaint>
    { 
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcVendorScore> VendorScores;
        public PXSavePerRow<ArcVendorScore> Save;
        public PXCancel<ArcVendorScore> Cancel;
    }
}
