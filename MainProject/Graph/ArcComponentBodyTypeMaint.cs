using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ComponentBodyTypeMaint : PXGraph<ComponentBodyTypeMaint>
    {
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcComponentBodyType> BodyTypes;
        public PXSavePerRow<ArcComponentBodyType> Save;
        public PXCancel<ArcComponentBodyType> Cancel;
        
    }
}
