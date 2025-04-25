using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using Opayo.Webhooks;
using PX.Api.Webhooks.DAC;
using PX.Api.Webhooks.Graph;
using PX.Concurrency;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.GL.DAC;

using DetailsResult = PX.Objects.AR.ARStatementPrint.DetailsResult;
using PrintParameters = PX.Objects.AR.ARStatementPrint.PrintParameters;
using Type = System.Type;
using Messages = Opayo.Tools.Messages;
using PX.Objects.AR.CustomerStatements;
using System.Linq;

namespace Opayo
{
    public class ARStatementPrintExt : HttpContextInitializator<ARStatementPrint>
    {

        public void _(Events.RowSelected<PrintParameters> e, PXRowSelected baseHandler)
        {
            try
            {
                baseHandler?.Invoke(e.Cache, e.Args);
            }
            catch { }

            PrintParameters row = e.Row;
            PrintParameters filter = Base.Filter.Cache.CreateCopy(row) as PrintParameters;

            if (row.Action == PrintParameters.Actions.Print)
                Base.Details.SetAsyncProcessDelegate((list, ct) => PrintStatementsExtended(filter, list, ct, Base));
        }

        public static async System.Threading.Tasks.Task PrintStatementsExtended(PrintParameters filter, IEnumerable<DetailsResult> list, CancellationToken cancellationToken, ARStatementPrint Base)
        {
            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                     ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);

            bool enableOpayoLinkGeneration = account != null && account.EnableStatementPaymentLinks == true;
            if (enableOpayoLinkGeneration)
            {
                foreach (DetailsResult detailsResult in list)
                {

                    ARStatementUpdate graph = PXGraph.CreateInstance<ARStatementUpdate>();
                    ARStatementPrint printGraph = PXGraph.CreateInstance<ARStatementPrint>();
                    Customer customer = graph.Customer.Search<Customer.bAccountID>(detailsResult.CustomerID, detailsResult.CustomerID);
                    graph.Customer.Current = customer;
                    ARSetup arSetup = graph.ARSetup.Current;

                    var statements = graph.StatementMC
                        .Select(detailsResult.CustomerID, filter.StatementDate, detailsResult.CuryID, detailsResult.CuryID)
                        .RowCast<ARStatement>()
                        .Where(statement => arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ConsolidatedForAllCompanies ||
                            (arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ForEachBranch && statement.BranchID == filter.BranchID) ||
                            (arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ConsolidatedForCompany && PXAccess.GetBranch(statement.BranchID).Organization.OrganizationID == filter.OrganizationID))
                        .ToList();
                    foreach (ARStatement statement in statements)
                    {

                        if (!statement.IsParentCustomerStatement || statement.CuryEndBalance <= 0 || statement.CuryID != "GBP")
                            continue;

                        statement.GetExtension<ARStatementExt>().UsrPaymentLink = OpayoLinkGenerator<StatementCryptParser, ARStatementUpdate, ARStatement>.Generate(graph, statement);
                        graph.StatementMC.Cache.Update(statement);
                        graph.Save.Press();
                    }
                }
            }
            await ARStatementPrint.PrintStatements(filter, list, cancellationToken);
        }

    }


    public class ARStatementUpdateExt : HttpContextInitializator<ARStatementUpdate>
    {
        public delegate void EMailStatementDelegate(int? branchID,
          string branchCD,
          int? customerID,
          DateTime? statementDate,
          string currency,
          string statementMessage,
          bool markOnly,
          bool showAll,
          int? OrganizationID);

        [PXOverride]
        public virtual void EMailStatement(
            int? branchID,
            string branchCD,
            int? customerID,
            DateTime? statementDate,
            string currency,
            string statementMessage,
            bool markOnly,
            bool showAll,
            int? OrganizationID,
            EMailStatementDelegate baseHandler)
        {
            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, Base.Accessinfo.BranchID);
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                     ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);
            
            bool enableOpayoLinkGeneration = account != null && account.EnableStatementPaymentLinks == true;

            if (enableOpayoLinkGeneration)
            {
                ARStatementUpdate graph = PXGraph.CreateInstance<ARStatementUpdate>();
                ARStatementPrint printGraph = PXGraph.CreateInstance<ARStatementPrint>();
                Customer customer = Base.Customer.Search<Customer.bAccountID>(customerID, customerID);
                Base.Customer.Current = customer;
                ARSetup arSetup = Base.ARSetup.Current;

                var statements = graph.StatementMC
                    .Select(customerID, statementDate, currency, currency)
                    .RowCast<ARStatement>()
                    .Where(statement => arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ConsolidatedForAllCompanies ||
                        (arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ForEachBranch && statement.BranchID == branchID) ||
                        (arSetup?.PrepareStatements == PX.Objects.AR.ARSetup.prepareStatements.ConsolidatedForCompany && PXAccess.GetBranch(statement.BranchID).Organization.OrganizationID == OrganizationID))
                    .ToList();
                foreach (ARStatement statement in statements)
                {
                    if (!statement.IsParentCustomerStatement || statement.CuryEndBalance <= 0 || statement.CuryID != "GBP")
                        continue;

                    statement.GetExtension<ARStatementExt>().UsrPaymentLink = OpayoLinkGenerator<StatementCryptParser, ARStatementUpdate, ARStatement>.Generate(graph, statement);
                    graph.StatementMC.Cache.Update(statement);
                    graph.Save.Press();
                }
            }
            baseHandler?.Invoke(branchID, branchCD, customerID, statementDate, currency, statementMessage, markOnly, showAll, OrganizationID);
        }
    }
}

    
    
