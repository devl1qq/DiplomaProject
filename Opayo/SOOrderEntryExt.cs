using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using PX.Data;
using PX.Objects.SO;
using PX.Objects.GL.DAC;
using PX.Objects.GL;
using Messages = Opayo.Tools.Messages;
using System.Web;

namespace Opayo
{
    public class SOOrderEntryExt : PXGraphExtension<SOOrderEntry>
    {
        #region Actions

        public delegate IEnumerable PrintSalesOrderDelegate(PXAdapter adapter, string reportID = null);
        public delegate IEnumerable EmailSalesOrderDelegate(PXAdapter adapter, [PXString] string notificationCD = null);

        [PXOverride]
        public virtual IEnumerable PrintSalesOrder(PXAdapter adapter, string reportID, PrintSalesOrderDelegate baseHandler)
        {
            foreach (SOOrder order in adapter.Get<SOOrder>())
            {
                try
                {

                    Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, order?.BranchID);
                    Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch.OrganizationID);
                    ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch.GetExtension<BranchExt>().UsrOpayoAccountID) ?? ArcOpayoAccount.PK.Find(Base, organization.GetExtension<OrganizationExt>().UsrOpayoAccountID);

                    if (account != null && (account.RegeneratePaymentLinks == true || string.IsNullOrEmpty(order.GetExtension<SOOrderExt>().UsrPaymentLink)))
                    {
                        SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
                        graph.Document.Update(order);
                        graph.GetExtension<SOOrderEntryExt>().GeneratePaymentLink.Press();
                    }

                }
                catch (Exception e)
                {
                    PXTrace.WriteError(Messages.LinkNotGeneratedTraceError, e.Message);
                }
            }


            return baseHandler(adapter, reportID);
        }

        [PXOverride]
        public virtual IEnumerable EmailSalesOrder(PXAdapter adapter, [PXString] string notificationCD, EmailSalesOrderDelegate baseHandler)
        {

            foreach (SOOrder order in adapter.Get<SOOrder>())
            {
                try
                {

                    Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, order?.BranchID);
                    Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch.OrganizationID);
                    ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch.GetExtension<BranchExt>().UsrOpayoAccountID) ?? ArcOpayoAccount.PK.Find(Base, organization.GetExtension<OrganizationExt>().UsrOpayoAccountID);

                    if (account != null && (account.RegeneratePaymentLinks == true || string.IsNullOrEmpty(order.GetExtension<SOOrderExt>().UsrPaymentLink)))
                    {
                        SOOrderEntry graph = PXGraph.CreateInstance<SOOrderEntry>();
                        graph.Document.Update(order);
                        graph.GetExtension<SOOrderEntryExt>().GeneratePaymentLink.Press();
                    }

                }
                catch (Exception e)
                {
                    PXTrace.WriteError(Messages.LinkNotGeneratedTraceError, e.Message);
                }
            }

            return baseHandler(adapter, notificationCD);
        }

        public PXAction<SOOrder> GeneratePaymentLink;
        [PXUIField(DisplayName = "Generate Payment Link")]
        [PXButton(Category = "Opayo")]
        protected IEnumerable generatePaymentLink(PXAdapter adapter)
        {
            SOOrder order = Base.Document.Current;
            order.GetExtension<SOOrderExt>().UsrPaymentLink = OpayoLinkGenerator<SOOrderCryptParser, SOOrderEntry, SOOrder>.Generate(Base, order);
            Base.Document.Update(order);
            Base.Save.Press();

            return adapter.Get();
        }

        #endregion

        #region Events

        public void _(Events.RowSelected<SOOrder> e)
        {
            SOOrder row = e.Row;
            if (row == null) return;

            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, row.BranchID);
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                      ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);

            bool enableOpayoLinkGeneration = account != null && account.EnableSalesOrderPaymentLinks == true;

            PXUIFieldAttribute.SetEnabled<SOOrderExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetVisible<SOOrderExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration);
            GeneratePaymentLink.SetEnabled(enableOpayoLinkGeneration);
            GeneratePaymentLink.SetVisible(enableOpayoLinkGeneration);
        }

        #endregion
    }
}
