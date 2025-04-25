using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcFlowDownNotesMaint : PXGraph<ArcFlowDownNotesMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcFlowDownNotes> Notes;
        public PXSavePerRow<ArcFlowDownNotes> Save;
        public PXCancel<ArcFlowDownNotes> Cancel;
    }
}
