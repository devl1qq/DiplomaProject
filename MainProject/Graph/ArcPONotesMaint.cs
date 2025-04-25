using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcPONotesMaint : PXGraph<ArcPONotesMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcPONotes> PONotes;
        public PXSavePerRow<ArcPONotes> Save;
        public PXCancel<ArcPONotes> Cancel;
    }
}
