using System;
using Opayo.DAC;
using Opayo.Tools;
using PX.Data;
using PX.Objects.SO;
using PX.Objects.GL;
using PX.Objects.GL.DAC;
using Messages = Opayo.Tools.Messages;
using Opayo.Webhooks;
using PX.Api.Webhooks.DAC;
using PX.Api.Webhooks.Graph;
using PX.Objects.AR;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using PX.Objects.AR.MigrationMode;
using Microsoft.AspNetCore.Http.Extensions;
using PX.Data.Update;
using RestSharp.Authenticators;
using RestSharp;
using System.Net;
using System.Web.Hosting;

namespace Opayo.Models
{
   
    public interface IAcumaticaEntityToCrypt<in TGraph, Table>
        where TGraph : PXGraph
        where Table : IBqlTable
    {
        CryptModel Parse(TGraph graph, Table table, ArcOpayoAccount account);
    }

    public class OpayoLinkGenerator<Generator, TGraph, Table>
        where TGraph : PXGraph
        where Table : IBqlTable
        where Generator : IAcumaticaEntityToCrypt<TGraph, Table>, new()
    {
        private static ArcOpayoAccount GetOpayoAccount()
        {
            PXGraph graph = PXGraph.CreateInstance<PXGraph>();

            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(graph, PXAccess.GetBranchID());
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(graph, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(graph, branch?.GetExtension<BranchExt>().UsrOpayoAccountID) ?? ArcOpayoAccount.PK.Find(graph, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);
            if (account == null)
                throw new PXException(Messages.OpayoAccountNotSpecified);

            return account;
        }

        private static bool CreateTransaction(Table table, ArcOpayoAccount account, CryptModel cryptModel, string link)
        {
            try
            {
                ArcOpayoTranMaint tranMaint = PXGraph.CreateInstance<ArcOpayoTranMaint>();
                ArcOpayoTran transaction = new ArcOpayoTran()
                {
                    OpayoAccountID = account.AccountID,
                    PaymentLink = link,
                    VendorTxCode = cryptModel.VendorTxCode
                };

                switch (table)
                {
                    case SOOrder order:
                        transaction.Type = TransactionTypesAttribute.Values.SalesOrder;
                        transaction.CustomerID = order.CustomerID;
                        transaction.RelatedEntity = order.NoteID;
                        break;
                    case Customer customer:
                        transaction.Type = TransactionTypesAttribute.Values.Customer;
                        transaction.CustomerID = customer.BAccountID;
                        transaction.RelatedEntity = customer.NoteID;
                        break;
                    case ARInvoice invoice:
                        transaction.Type = TransactionTypesAttribute.Values.Invoice;
                        transaction.CustomerID = invoice.CustomerID;
                        transaction.RelatedEntity = invoice.NoteID;
                        break;
                    case ArcOpayoAccount acc:
                        transaction.Type = TransactionTypesAttribute.Values.Test;
                        transaction.CustomerID = null;
                        transaction.RelatedEntity = null;
                        break;
                    case ARStatement statement:
                        transaction.Type = TransactionTypesAttribute.Values.Statement;
                        transaction.CustomerID = statement.CustomerID;
                        transaction.RelatedEntity = statement.NoteID;
                        break;
                        //default:
                        //    transaction.Type = TransactionTypesAttribute.Values.Test;
                        //    transaction.CustomerID = null;
                        //    transaction.RelatedEntity = null;
                        //    break;
                }

                tranMaint.Transaction.Insert(transaction);
                tranMaint.Save.Press();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Generate(TGraph graph, Table table, ArcOpayoAccount account = null)
        {
            Generator generator = new Generator();

            // Opayo Account
            // to generate link from another account or take from Branches/Company screens
            if (account == null)
                account = GetOpayoAccount();

            if (account.Active != true)
                throw new PXException(Messages.AccountInactive);

            // Crypt
            CryptModel cryptModel = generator.Parse(graph, table, account);
            if (cryptModel == null)
                throw new PXException(Messages.CryptNull);

            // credentials
            string vpsProtocol = account.VPSProtocol ?? throw new PXException(Messages.VPSProtocolNull);
            string txType = account.TxType ?? throw new PXException(Messages.TxTypeNull);

            string url = null;
            string vendor = null;
            string encryptionKey = null;

            switch (account.Environment)
            {
                case EnvironmentAttribute.Values.Test:
                    url = account.TestURL;
                    vendor = account.TestVendor;
                    encryptionKey = account.TestFIEncryptionPassword;
                    break;
                case EnvironmentAttribute.Values.Live:
                    url = account.LiveURL;
                    vendor = account.LiveVendor;
                    encryptionKey = account.LiveFIEncryptionPassword;
                    break;
                default:
                    throw new PXException(Messages.UnknownEnvironment);
            }

            if (url == null)
                throw new PXException(Messages.UrlNull);

            if (vendor == null)
                throw new PXException(Messages.VendorNull);

            if (encryptionKey == null)
                throw new PXException(Messages.EncryptionKeyNull);
            
            if (string.IsNullOrEmpty(account.WebhookURL))
                throw new PXException(Messages.WebhookUrlNull);

            cryptModel.SuccessURL = account.WebhookURL + $"?account={account.AccountID}";
            cryptModel.FailureURL = account.WebhookURL + $"?account={account.AccountID}";

            string crypt = Crypt.Encrypt(CryptModel.Serialize(cryptModel), encryptionKey);
            string link = $"{url}?VPSProtocol={vpsProtocol}&TxType={txType}&Vendor={vendor}&Crypt=@{crypt}";

            //Create Transaction
            if (!CreateTransaction(table, account, cryptModel, link))
                throw new PXException("");

            return link;
        }
    }
}
