using PX.Data;
using PX.Objects.SO.GraphExtensions.SOOrderEntryExt;
using PX.Objects.SO;
using System;
using System.Linq;
using PX.Objects.PO;
using MainProject.DAC;

namespace MainProject.Graph
{
    public class POLinkDialogExt : PXGraphExtension<PurchaseToSOLinkDialog, POLinkDialog, SOOrderEntry>
    {
        [PXOverride]
        public virtual void LinkPOSupply(SOLine currentSOLine, Action<SOLine> baseHandler)
        {
            var supplyPOLines = Base1.SupplyPOLines
                .Select()
                .RowCast<SupplyPOLine>()
                .Where(supplyPO => supplyPO.Selected == true);
            foreach (var supplyPOLine in supplyPOLines)
            {
                supplyPOLine.GetExtension<SupplyPOLineExt>().UsrSONbr = currentSOLine.OrderNbr;
                supplyPOLine.GetExtension<SupplyPOLineExt>().UsrSOLineNbr = currentSOLine.LineNbr;
                supplyPOLine.GetExtension<SupplyPOLineExt>().UsrSOType = currentSOLine.OrderType;
                Base1.SupplyPOLines.Update(supplyPOLine);
            }
            baseHandler(currentSOLine);
        }
    }
}
