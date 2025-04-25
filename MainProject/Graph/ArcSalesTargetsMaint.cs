using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class SalesTargetsMaint : PXGraph<SalesTargetsMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcSalesTargets> Targets;
        public PXSavePerRow<ArcSalesTargets> Save;
        public PXCancel<ArcSalesTargets> Cancel;
    }
}
