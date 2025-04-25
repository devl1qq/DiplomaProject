using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using Opayo.DAC;
using System.Collections;
using System.Web;
using Opayo.Models;
using Opayo.Tools;
using PX.Api.Webhooks.DAC;
using PX.Data.BQL;
using Opayo.Webhooks;
using PX.Api.Webhooks.Graph;
using Messages = Opayo.Tools.Messages;
using PX.Api;

namespace Opayo
{
    public class ArcOpayoAccountMaint : PXGraph<ArcOpayoAccountMaint, ArcOpayoAccount>
    {
        #region Views

        public PXSelect<ArcOpayoAccount> CurrentAccount;
        public PXSelect<ArcOpayoAccount, Where<ArcOpayoAccount.accountID, Equal<Current<ArcOpayoAccount.accountID>>>> Account;
        public PXFilter<GenerateTestLinkForm> GenerateLinkForm;
        public PXSelect<ArcOpayoAccountLogs, Where<ArcOpayoAccountLogs.accountID, Equal<Current<ArcOpayoAccount.accountID>>>, 
            OrderBy<Desc<ArcOpayoAccountLogs.createdDateTime>>> Logs;

        #endregion

        #region Actions

        public PXAction<ArcOpayoAccount> GenerateTestLink;
        [PXUIField(DisplayName = "Generate Test Link")]
        [PXButton(SpecialType = PXSpecialButtonType.ActionsFolder, IsLockedOnToolbar = true)]
        protected IEnumerable generateTestLink(PXAdapter adapter)
        {
            GenerateLinkForm.Current.Link = OpayoLinkGenerator<TestCryptParser, ArcOpayoAccountMaint, ArcOpayoAccount>.Generate(this, CurrentAccount.Current);
            GenerateLinkForm.UpdateCurrent();
            GenerateLinkForm.AskExt();

            GenerateLinkForm.Cache.IsDirty = false;

            return adapter.Get();
        }

        #endregion

        #region Events
        public void _(Events.FieldDefaulting<ArcOpayoAccount.webhookURL> e)
        {
            var row = (ArcOpayoAccount)e.Row;
            if (row == null) return;

            WebHook webhook = PXSelect<WebHook, Where<WebHook.handler, Equal<Required<WebHook.handler>>, And<WebHook.isActive, Equal<True>>>>
               .Select(PXGraph.CreateInstance<WebhookMaint>(), typeof(OpayoWebhook).FullName);
            
            WebhookMaint webhookMaint = PXGraph.CreateInstance<WebhookMaint>();
            webhookMaint.Webhook.Current = webhook;
            webhook = webhookMaint.Webhook.UpdateCurrent();
            
            e.NewValue = webhook.Url;
        }

        public void _(Events.FieldUpdated<ArcOpayoAccount.autoCreatePayments> e)
        {
            var row = (ArcOpayoAccount)e.Row;
            if (row == null) return;

            if ((bool)e.NewValue != true)
                row.AutoReleasePayments = false;  
        }

        public void _(Events.RowSelected<ArcOpayoAccount> e)
        {
            ArcOpayoAccount row = e.Row;
            if (row == null) return;

            bool testEnv = row.Environment == EnvironmentAttribute.Values.Test;
            bool liveEnv = row.Environment == EnvironmentAttribute.Values.Live;
           
            if (testEnv)
                e.Cache.RaiseExceptionHandling<ArcOpayoAccount.environment>(row, EnvironmentAttribute.Values.Test, 
                    new PXSetPropertyException<ArcOpayoAccount.environment>(Messages.TestEnvironment, PXErrorLevel.Warning));

            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.testURL>(e.Cache, row, testEnv);
            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.testVendor>(e.Cache, row, testEnv);
            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.testFIEncryptionPassword>(e.Cache, row, testEnv);

            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.liveURL>(e.Cache, row, liveEnv);
            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.liveVendor>(e.Cache, row, liveEnv);
            PXUIFieldAttribute.SetVisible<ArcOpayoAccount.liveFIEncryptionPassword>(e.Cache, row, liveEnv);
            
            PXUIFieldAttribute.SetEnabled<ArcOpayoAccount.autoReleasePayments>(e.Cache, row, row.AutoCreatePayments == true);
        }
        public void _(Events.RowPersisting<ArcOpayoAccount> e)
        {
            ArcOpayoAccount row = e.Row;
            if (row == null) return;

            if (row.AutoCreatePayments == true && (row.CashAccount == null || row.PaymentMethod == null))
                e.Cache.RaiseExceptionHandling<ArcOpayoAccount.autoCreatePayments>(row, false, new PXSetPropertyException(Messages.AutoCreationError));
        }

        public void _(Events.RowDeleting<ArcOpayoAccount> e)
        {
            ArcOpayoAccount row = e.Row;
            if (row == null) return;

            bool anyTransactionExist = PXSelectReadonly<ArcOpayoTran, Where<ArcOpayoTran.opayoAccountID, Equal<Required<ArcOpayoAccount.accountID>>>>
                .Select(this, row.AccountID).RowCast<ArcOpayoTran>().Any();

            if (anyTransactionExist)
                throw new PXException("This account has transactions and cannot be deleted.");
        }


        #endregion



        public class GenerateTestLinkForm : PXBqlTable, IBqlTable
        {
            #region Link
            [PXString(255)]
            [PXUIField(DisplayName = "Link", Enabled = false)]
            public virtual string Link { get; set; }
            public abstract class link : BqlString.Field<link> { }
            #endregion
        }
    }
}
