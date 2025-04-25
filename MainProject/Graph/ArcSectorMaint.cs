using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcSectorMaint : PXGraph<ArcSectorMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcSector> Sectors;
        public PXSavePerRow<ArcSector> Save;
        public PXCancel<ArcSector> Cancel;
    }
}
