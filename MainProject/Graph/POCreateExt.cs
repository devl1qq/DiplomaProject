using PX.Data;
using PX.Objects.AM;
using PX.Objects.PO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.Objects.SO.SOCreate;

namespace MainProject.Graph
{
    public class POCreateExt : PXGraphExtension<POCreate>
    {
        public delegate PXRedirectRequiredException CreatePOOrdersDelegate(List<POFixedDemand> list, DateTime? PurchDate, bool extSort, int? branchID = null);
        [PXOverride]
        public virtual PXRedirectRequiredException CreatePOOrders(List<POFixedDemand> list, DateTime? PurchDate, bool extSort, int? branchID, CreatePOOrdersDelegate baseHandler)
        {
            foreach (var demand in list)
            {
                var prodMatlSplit = (AMProdMatlSplitPlan)PXSelect<AMProdMatlSplitPlan,
                    Where<AMProdMatlSplitPlan.planID, Equal<Required<AMProdMatlSplitPlan.planID>>>>
                        .Select(Base, demand.PlanID);
                if (prodMatlSplit == null)
                    continue;

                var prodItem = AMProdItem.PK.Find(Base, prodMatlSplit?.OrderType, prodMatlSplit?.ProdOrdID);
                demand.OrderNbr = prodItem?.OrdNbr;
                demand.OrderType = prodItem?.OrdTypeRef;
                demand.LineNbr = prodItem?.OrdLineRef;
            }

            var baseResult = baseHandler(list, PurchDate, extSort, branchID);
            return baseResult;
        }
    }
}
