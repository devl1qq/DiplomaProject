using PX.Data;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class ArcVendorScoreReasonMaint : PXGraph<ArcVendorScoreReasonMaint>
    { 
        [PXImport]
        [PXFilterable]
        public PXSelect<ArcVendorScoreReason> VendorScoreReasons;
        public PXSavePerRow<ArcVendorScoreReason> Save;
        public PXCancel<ArcVendorScoreReason> Cancel;
    }
}
