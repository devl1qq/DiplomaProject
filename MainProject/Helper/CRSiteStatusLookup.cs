using PX.Data;
using PX.Objects.CR;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Helper
{
    public class CRSiteStatusLookup<Status, StatusFilter> : INSiteStatusLookup<Status, StatusFilter>
       where Status : class, IBqlTable, new()
       where StatusFilter : CRSiteStatusFilter, new()
    {
        public CRSiteStatusLookup(PXGraph graph)
            : base(graph)
        {
            graph.RowSelecting.AddHandler(typeof(CRSiteStatusSelected), new PXRowSelecting(this.OnRowSelecting));
        }

        public CRSiteStatusLookup(PXGraph graph, Delegate handler)
            : base(graph, handler)
        {
            graph.RowSelecting.AddHandler(typeof(CRSiteStatusSelected), new PXRowSelecting(this.OnRowSelecting));
        }

        protected virtual void OnRowSelecting(PXCache sender, PXRowSelectingEventArgs e)
        {
            if (!sender.Fields.Contains(typeof(CRSiteStatusSelected.curyID).Name) ||
                sender.GetValue<CRSiteStatusSelected.curyID>(e.Row) != null)
                return;
            PXCache cach = sender.Graph.Caches[typeof(CROpportunity)];
            sender.SetValue<CRSiteStatusSelected.curyID>(e.Row, cach.GetValue<CROpportunity.curyID>(cach.Current));
            sender.SetValue<CRSiteStatusSelected.curyInfoID>(e.Row,
                cach.GetValue<CROpportunity.curyInfoID>(cach.Current));
        }

        protected override void OnFilterSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            base.OnFilterSelected(sender, e);
            CRSiteStatusFilter filter = (CRSiteStatusFilter)e.Row;
            PXCache cache1 = sender;
            int? mode1 = filter.Mode;
            int num1 = 1;
            int num2 = mode1.GetValueOrDefault() == num1 & mode1.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusFilter.historyDate>(cache1, (object)null, num2 != 0);
            PXCache cache2 = sender;
            int? mode2 = filter.Mode;
            int num3 = 1;
            int num4 = mode2.GetValueOrDefault() == num3 & mode2.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusFilter.dropShipSales>(cache2, (object)null, num4 != 0);
            sender.Adjust<PXUIFieldAttribute>().For<CRSiteStatusFilter.customerLocationID>(
                (System.Action<PXUIFieldAttribute>)(a => a.Visible = a.Enabled = filter.Behavior == "BL"));
            PXCache cach = sender.Graph.Caches[typeof(CRSiteStatusSelected)];
            PXCache cache3 = cach;
            int? mode3 = filter.Mode;
            int num5 = 1;
            int num6 = mode3.GetValueOrDefault() == num5 & mode3.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusSelected.qtyLastSale>(cache3, (object)null, num6 != 0);
            PXCache cache4 = cach;
            int? mode4 = filter.Mode;
            int num7 = 1;
            int num8 = mode4.GetValueOrDefault() == num7 & mode4.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusSelected.curyID>(cache4, (object)null, num8 != 0);
            PXCache cache5 = cach;
            int? mode5 = filter.Mode;
            int num9 = 1;
            int num10 = mode5.GetValueOrDefault() == num9 & mode5.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusSelected.curyUnitPrice>(cache5, (object)null, num10 != 0);
            PXCache cache6 = cach;
            int? mode6 = filter.Mode;
            int num11 = 1;
            int num12 = mode6.GetValueOrDefault() == num11 & mode6.HasValue ? 1 : 0;
            PXUIFieldAttribute.SetVisible<CRSiteStatusSelected.lastSalesDate>(cache6, (object)null, num12 != 0);
            PXCacheEx.AttributeAdjuster<PXUIFieldAttribute>.Chained chained = cach.Adjust<PXUIFieldAttribute>()
                .For<CRSiteStatusSelected.dropShipLastBaseQty>((System.Action<PXUIFieldAttribute>)(x =>
                {
                    PXUIFieldAttribute pxuiFieldAttribute = x;
                    bool? dropShipSales = filter.DropShipSales;
                    bool flag = true;
                    int num13 = dropShipSales.GetValueOrDefault() == flag & dropShipSales.HasValue ? 1 : 0;
                    pxuiFieldAttribute.Visible = num13 != 0;
                }));
            chained = chained.SameFor<CRSiteStatusSelected.dropShipLastQty>();
            chained = chained.SameFor<CRSiteStatusSelected.dropShipLastUnitPrice>();
            chained = chained.SameFor<CRSiteStatusSelected.dropShipCuryUnitPrice>();
            chained.SameFor<CRSiteStatusSelected.dropShipLastDate>();
            if (filter.HistoryDate.HasValue)
                return;
            filter.HistoryDate = new DateTime?(sender.Graph.Accessinfo.BusinessDate.GetValueOrDefault().AddMonths(-3));
        }
    }
}
