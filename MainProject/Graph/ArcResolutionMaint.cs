using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcResolutionMaint : PXGraph<ArcResolutionMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcResolution> Resolutions;
        public PXSavePerRow<ArcResolution> Save;
        public PXCancel<ArcResolution> Cancel;
    }
}
