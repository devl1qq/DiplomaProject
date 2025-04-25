using PX.Data;
using PX.Objects.CR;
using PX.Objects.GDPR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.SM;
using System.Text.RegularExpressions;
using System.Web;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using PX.Common.IO;
using PX.Api;
using static PX.SM.SMPerformanceInfo;
using static PX.Data.BQL.BqlPlaceholder;
using PX.Api.Webhooks;
using System.Net.Http.Headers;
using System.Security.Claims;
using PX.Api.Webhooks.DAC;
using PX.Api.Webhooks.Graph;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth2;

namespace Opayo.Webhooks
{
    public class OpayoWebhook : IWebhookHandler
    {
        public async Task HandleAsync(WebhookContext context, CancellationToken cancellation)
        {
            using (var scope = GetAdminScope())
            {
                context.Request.Query.TryGetValue("account", out var objAccountId);

                if (int.TryParse(objAccountId, out int accountId))
                {
                    ArcOpayoAccount account = PXSelect<ArcOpayoAccount, Where<ArcOpayoAccount.accountID, Equal<Required<ArcOpayoAccount.accountID>>>>.Select(PXGraph.CreateInstance<ArcOpayoAccountMaint>(), accountId);
                    context.Request.Query.TryGetValue("crypt", out var objCrypt);
                    context.Request.Headers.TryGetValue("AbsoluteUri", out var absoluteUri);

                    try
                    {
                        string encryptionKey = null;

                        if (account?.Environment == EnvironmentAttribute.Values.Live)
                            encryptionKey = account.LiveFIEncryptionPassword;

                        if (account?.Environment == EnvironmentAttribute.Values.Test)
                            encryptionKey = account.TestFIEncryptionPassword;

                        if (string.IsNullOrEmpty(encryptionKey))
                            throw new PXException("Encryption key is null.");

                        string decryptedData = Crypt.Decrypt(objCrypt, encryptionKey);
                        CryptModel crypt = CryptModel.Deserialize(decryptedData);

                        ArcOpayoTranMaint tranMaint = PXGraph.CreateInstance<ArcOpayoTranMaint>();
                        ArcOpayoTran transactionToUpdate = PXSelect<ArcOpayoTran,
                                Where<ArcOpayoTran.vendorTxCode, Equal<Required<ArcOpayoTran.vendorTxCode>>,
                                And<ArcOpayoTran.status, NotEqual<TransactionStatusesAttribute.BQL.Completed>>>>
                            .Select(tranMaint, crypt.VendorTxCode);

                        if (transactionToUpdate == null)
                        {
                            CreateHtmlResponse(context.Response, Properties.Resources.success);
                            return;
                        }

                        bool statusOK = crypt.Status == "OK";

                        string responseContent = statusOK
                            ? Properties.Resources.success
                            : Properties.Resources.failure;

                        transactionToUpdate.Status = statusOK
                            ? TransactionStatusesAttribute.Values.Completed
                            : TransactionStatusesAttribute.Values.Failed;

                        transactionToUpdate.PaymentDate = DateTime.Now;

                        tranMaint.Transaction.Update(transactionToUpdate);
                        tranMaint.Save.Press();

                        if (statusOK && account.AutoCreatePayments == true)
                            tranMaint.CreatePayment.Press();

                        CreateLog(account, absoluteUri, "Transaction processed", $"Account: {account.AccountCD}", objCrypt.ToString().ToUpper());
                        CreateHtmlResponse(context.Response, responseContent);
                        return;
                    }
                    catch (Exception e)
                    {
                        CreateLog(account, absoluteUri, e.Message, e.StackTrace, objCrypt.ToString().ToUpper());
                        CreateHtmlResponse(context.Response, Properties.Resources.error);
                        return;
                    }
                }

                CreateHtmlResponse(context.Response, Properties.Resources.error);
            }
        }

        private void CreateHtmlResponse(WebhookResponse response, string content)
        {
            response.StatusCode = 200;
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength = content.Length;

            using (var writer = new StreamWriter(response.Body))
            {
                writer.WriteAsync(content);
            }
        }
        private void CreateLog(ArcOpayoAccount acc,string webhookLink, string desc, string err, string crypt)
        {
            ArcOpayoAccountMaint accountMaint = PXGraph.CreateInstance<ArcOpayoAccountMaint>();
            accountMaint.CurrentAccount.Update(acc);

            accountMaint.Logs.Insert(new ArcOpayoAccountLogs()
            {
                WebhookLink = webhookLink,
                Description = desc,
                Error = err,
                Crypt = crypt,
            });
            accountMaint.Save.Press();
        }
        

        private IDisposable GetAdminScope()
        {
            var userName = "admin";
            if (PXDatabase.Companies.Length > 0)
            {
                var company = PXAccess.GetCompanyName();
                if (string.IsNullOrEmpty(company))
                {
                    company = PXDatabase.Companies[0];
                }
                userName = userName + "@" + company;
            }
            return new PXLoginScope(userName);
        }
    }
}
