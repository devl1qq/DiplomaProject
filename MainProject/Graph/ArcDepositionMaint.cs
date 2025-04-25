using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcDispositionMaint : PXGraph<ArcDispositionMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcDisposition> Dispositions;
        public PXSavePerRow<ArcDisposition> Save;
        public PXCancel<ArcDisposition> Cancel;
    }
}
