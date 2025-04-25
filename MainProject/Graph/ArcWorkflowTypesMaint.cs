using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcWorkflowTypesMaint : PXGraph<ArcWorkflowTypesMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcWorkflowType> WorkflowTypes;
        public PXSavePerRow<ArcWorkflowType> Save;
        public PXCancel<ArcWorkflowType> Cancel;
    }
}
