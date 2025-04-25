using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcDiscrepancyTypeMaint : PXGraph<ArcDiscrepancyTypeMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcDiscrepancyType> DiscrepancyTypes;
        public PXSavePerRow<ArcDiscrepancyType> Save;
        public PXCancel<ArcDiscrepancyType> Cancel;
    }
}
