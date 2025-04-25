using PX.Data;
using PX.Objects.AM;
using PX.Objects.AP;
using PX.Objects.CR;
using PX.Objects.PO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.Graph
{
    public class OUSearchMaintExt : PXGraphExtension<OUSearchMaint>
    {
        public void _(Events.RowSelected<OUSearchEntity> e)
        {
            var row = e.Row;
            if (row == null)
                return;
            var vendorExists = PXSelectJoin<Contact, LeftJoin<BAccount, On<BAccount.defContactID, Equal<Contact.contactID>>>, Where<Contact.eMail, Equal<Required<OUSearchEntity.outgoingEmail>>>>.Select(Base, row.OutgoingEmail).RowCast<Contact>().Count() > 0;
            CreatePOOrder.SetVisible(vendorExists);
        }
        public PXAction<OUSearchEntity> CreatePOOrder;
        [PXUIField(DisplayName = "Create Purchase Order", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Visible = true)]
        [PXButton]
        public virtual IEnumerable createPOOrder(PXAdapter adapter)
        {
            var vendor = PXSelectJoin<BAccount, LeftJoin<Contact, On<Contact.contactID, Equal<BAccount.defContactID>>>, Where<Contact.eMail, Equal<Required<OUSearchEntity.outgoingEmail>>, And<BAccount.type, NotEqual<BAccountType.employeeType>>>>.Select(Base, Base.Filter.Current.OutgoingEmail).RowCast<BAccount>().FirstOrDefault();
            if (vendor == null)
                return adapter.Get();

            var graph = PXGraph.CreateInstance<POOrderEntry>();
            var poOrder = graph.Document.Insert();
            graph.Document.Cache.SetValueExt<POOrder.vendorID>(poOrder, vendor.BAccountID);
            graph.Document.Cache.SetValueExt<POOrder.orderDesc>(poOrder, Base.SourceMessage.Current?.Subject);
            graph.Document.Update(poOrder);
            graph.Save.Press();

            return adapter.Get();
        }
    }
}
