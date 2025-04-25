using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using PX.Api;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.AR.Repositories;
using PX.Objects.CR;
using PX.Objects.GL;
using PX.Objects.GL.DAC;
using ARInvoiceExt = Opayo.DAC.ARInvoiceExt;
using Messages = Opayo.Tools.Messages;

namespace Opayo
{
    public class CustomerMaintExt : PXGraphExtension<CustomerMaint>
    {

        #region Actions

        public PXAction<Customer> GeneratePaymentLink;
        [PXUIField(DisplayName = "Generate Payment Link")]
        [PXButton(Category = "Opayo")]
        protected IEnumerable generatePaymentLink(PXAdapter adapter)
        {
            Customer customer = Base.BAccount.Current;
            customer.GetExtension<CustomerExt>().UsrPaymentLink = OpayoLinkGenerator<CustomerCryptParser, CustomerMaint, Customer>.Generate(Base, customer);
            Base.BAccount.Update(customer);
            Base.Save.Press();

            return adapter.Get();
        }

        public delegate IEnumerable CustomerStatementDelegate(PXAdapter adapter);
        [PXOverride]
        public virtual IEnumerable CustomerStatement(PXAdapter adapter, CustomerStatementDelegate baseHandler)
        {
            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                     ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);

            bool enableOpayoLinkGeneration = account != null && account.EnableStatementPaymentLinks == true;
            try
            {
                baseHandler(adapter);
            }
            catch (PXReportRequiredException report)
            {
                if (enableOpayoLinkGeneration)
                {
                    Customer customer = Base.CurrentCustomer.Current;

                    ARStatement lastStatement = new ARStatementRepository(Base).FindLastStatement(
                        customer,
                        priorToDate: null,
                        includeOnDemand: true);
                    GenerateStatementPaymentLink(lastStatement);
                }
                throw report;
            }
            return adapter.Get();
        }

        #endregion

        #region Events

        public void _(Events.RowSelected<Customer> e)
        {
            Customer row = e.Row;
            if (row == null) return;

            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, PXAccess.GetBranchID());
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                      ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);

            bool enableOpayoLinkGeneration = account != null && account.EnableCustomerPaymentLinks == true;

            PXUIFieldAttribute.SetEnabled<CustomerExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetEnabled<CustomerExt.usrPaymentAmount>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetEnabled<CustomerExt.usrPaymentCurrency>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetEnabled<CustomerExt.usrPaymentDescription>(e.Cache, row, enableOpayoLinkGeneration);

            PXUIFieldAttribute.SetVisible<CustomerExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetVisible<CustomerExt.usrPaymentAmount>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetVisible<CustomerExt.usrPaymentCurrency>(e.Cache, row, enableOpayoLinkGeneration);
            PXUIFieldAttribute.SetVisible<CustomerExt.usrPaymentDescription>(e.Cache, row, enableOpayoLinkGeneration);

            GeneratePaymentLink.SetEnabled(enableOpayoLinkGeneration);
            GeneratePaymentLink.SetVisible(enableOpayoLinkGeneration);
        }

        #endregion

        #region Methods
        public void GenerateStatementPaymentLink(ARStatement statement)
        {

            ARStatementUpdate graph = PXGraph.CreateInstance<ARStatementUpdate>();

            if (statement.CuryEndBalance <= 0 || statement.CuryID != "GBP")
                return;

            statement.GetExtension<ARStatementExt>().UsrPaymentLink = OpayoLinkGenerator<StatementCryptParser, ARStatementUpdate, ARStatement>.Generate(graph, statement);
            graph.StatementMC.Cache.Update(statement);
            graph.Save.Press();

        }
        #endregion
    }
}


