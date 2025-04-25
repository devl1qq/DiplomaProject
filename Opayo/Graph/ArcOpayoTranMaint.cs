using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using PX.Api;
using PX.Data;
using PX.Data.WorkflowAPI;
using PX.Objects.AR;
using PX.Objects.SO;
using PX.Objects.SO.GraphExtensions.ARPaymentEntryExt;
using Messages = Opayo.Tools.Messages;

namespace Opayo
{
    public class ArcOpayoTranMaint : PXGraph<ArcOpayoTranMaint>
    {

        public PXSelect<ArcOpayoTran> Transaction;

        #region Actions and Methods

        public PXSave<ArcOpayoTran> Save;
        public PXCancel<ArcOpayoTran> Cancel;
        public PXFirst<ArcOpayoTran> First;
        public PXPrevious<ArcOpayoTran> Prev;
        public PXNext<ArcOpayoTran> Next;
        public PXLast<ArcOpayoTran> Last;

        #region CreatePayment
        public PXAction<ArcOpayoTran> CreatePayment;
        [PXUIField(DisplayName = "Create Payment")]
        [PXButton(Connotation = ActionConnotation.Success, IsLockedOnToolbar = true)]
        protected IEnumerable createPayment(PXAdapter adapter)
        {
            PXLongOperation.StartOperation(this, () =>
            {
                using (PXTransactionScope scope = new PXTransactionScope())
                {
                    ArcOpayoTran tran = Transaction.Current;
                    if (tran == null)
                        throw new PXException(Messages.ArcOpayoTranNull);

                    ArcOpayoAccount account = ArcOpayoAccount.PK.Find(this, tran.OpayoAccountID);
                    if (account == null)
                        throw new PXException(Messages.ArcOpayoAccountNull);

                    NameValueCollection queryString = HttpUtility.ParseQueryString(new Uri(tran.PaymentLink).Query);

                    // get encryption key
                    string host = new Uri(tran.PaymentLink).Host;
                    string encryptionKey = null;

                    if (account.LiveURL?.Contains(host) ?? false)
                        encryptionKey = account.LiveFIEncryptionPassword;

                    if (account.TestURL?.Contains(host) ?? false)
                        encryptionKey = account.TestFIEncryptionPassword;

                    if (string.IsNullOrEmpty(encryptionKey))
                        throw new PXException(Messages.EncryptionKeyNullPayment);

                    CryptModel model = CryptModel.Deserialize(Crypt.Decrypt(queryString["crypt"], encryptionKey));

                    ARPaymentEntry paymentEntry = PXGraph.CreateInstance<ARPaymentEntry>();
                    ARPayment payment = paymentEntry.Document.Insert();
                    paymentEntry.Document.Cache.SetValueExt<ARPayment.customerID>(payment, tran.CustomerID);  

                    payment.DocType = ARDocType.Payment;
                    payment.DocDate = tran.PaymentDate;
                    payment.PaymentMethodID = account.PaymentMethod;
                    payment.CashAccountID = account.CashAccount;
                    payment.ExtRefNbr = tran.VendorTxCode;
                    payment.CuryOrigDocAmt = Decimal.Parse(model.Amount);
                    payment = paymentEntry.Document.Update(payment);


                    if (tran.Type == TransactionTypesAttribute.Values.SalesOrder)
                    {
                        paymentEntry.Save.Press();

                        // insert sales order
                        SOOrder soorder = PXSelect<SOOrder, Where<SOOrder.noteID, Equal<Required<ArcOpayoTran.relatedEntity>>>>.Select(this, tran.RelatedEntity);
                        paymentEntry.GetExtension<OrdersToApplyTab>().SOAdjustments.Insert(new SOAdjust()
                        {
                            AdjdOrderType = soorder.OrderType,
                            AdjdOrderNbr = soorder.OrderNbr,
                            CuryAdjdAmt = Decimal.Parse(model.Amount)
                        });
                    }

                    if (tran.Type == TransactionTypesAttribute.Values.Invoice)
                    {
                        paymentEntry.Save.Press();

                        // insert documents to apply
                        ARInvoice invoice = PXSelect<ARInvoice, Where<ARInvoice.noteID, Equal<Required<ArcOpayoTran.relatedEntity>>>>.Select(this, tran.RelatedEntity);
                        if (invoice.Status == ARDocStatus.Open && (invoice.DocType == ARDocType.Invoice || invoice.DocType == ARDocType.DebitMemo))
                        {
                            paymentEntry.Adjustments.Insert(new ARAdjust()
                            {
                                AdjdDocType = invoice.DocType,
                                AdjdRefNbr = invoice.RefNbr,
                                CuryAdjdAmt = Decimal.Parse(model.Amount)
                            });
                        }
                    }

                    
                    paymentEntry.Save.Press();

                    tran.PaymentType = payment.DocType;
                    tran.PaymentNbr = payment.RefNbr;
                    Transaction.Update(tran);
                    Save.Press();

                    scope.Complete();

                    if (account.AutoReleasePayments == true && payment.Status == ARDocStatus.Hold)
                        paymentEntry.releaseFromHold.Press();
                    if (account.AutoReleasePayments == true && payment.Status == ARDocStatus.Balanced)
                        paymentEntry.release.Press();
                }
            });
            
            return adapter.Get();
        }
        #endregion

        #region ViewRelatedEntity
        public PXAction<ArcOpayoTran> ViewRelatedEntity;
        [PXUIField(DisplayName = "View Related Entity")]
        [PXButton(IsLockedOnToolbar = true)]
        protected IEnumerable viewRelatedEntity(PXAdapter adapter)
        {
            switch (Transaction.Current.Type)
            {
                case TransactionTypesAttribute.Values.SalesOrder:
                    var orderEntry = CreateInstance<SOOrderEntry>();
                    orderEntry.Document.Current = PXSelect<SOOrder, Where<SOOrder.noteID, Equal<Required<ArcOpayoTran.relatedEntity>>>>.Select(this, Transaction.Current.RelatedEntity);
                    throw new PXRedirectRequiredException(orderEntry, true, "");

                case TransactionTypesAttribute.Values.Invoice:
                    var invoiceEntry = CreateInstance<ARInvoiceEntry>();
                    invoiceEntry.Document.Current = PXSelect<ARInvoice, Where<ARInvoice.noteID, Equal<Required<ArcOpayoTran.relatedEntity>>>>.Select(this, Transaction.Current.RelatedEntity);
                    throw new PXRedirectRequiredException(invoiceEntry, true, "");

                case TransactionTypesAttribute.Values.Customer:
                    var customerMaint = CreateInstance<CustomerMaint>();
                    customerMaint.BAccount.Current = PXSelect<Customer, Where<Customer.noteID, Equal<Required<ArcOpayoTran.relatedEntity>>>>.Select(this, Transaction.Current.RelatedEntity);
                    throw new PXRedirectRequiredException(customerMaint, true, "");
                default:
                    throw new PXException(Messages.NoRelatedEntity);
            }
            
            return adapter.Get();
        }
        #endregion

        #region ReOpenTransaction
        public PXAction<ArcOpayoTran> ReOpenTransaction;
        [PXUIField(DisplayName = "Re-Open")]
        [PXButton(IsLockedOnToolbar = true)]
        protected IEnumerable reOpenTransaction(PXAdapter adapter)
        {
            foreach (var tran in adapter.Get<ArcOpayoTran>())
                UpdateTranStatus(tran, TransactionStatusesAttribute.Values.Open);

            return adapter.Get();
        }
        #endregion

        #region CancelTransaction
        public PXAction<ArcOpayoTran> CancelTransaction;
        [PXUIField(DisplayName = "Cancel")]
        [PXButton(IsLockedOnToolbar = true)]
        protected IEnumerable cancelTransaction(PXAdapter adapter)
        {
            foreach (var tran in adapter.Get<ArcOpayoTran>())
                UpdateTranStatus(tran, TransactionStatusesAttribute.Values.Canceled);

            return adapter.Get();
        }
        #endregion

        private void UpdateTranStatus(ArcOpayoTran tran, string status)
        {
            var graph = PXGraph.CreateInstance<ArcOpayoTranMaint>();
            tran.Status = status;
            graph.Transaction.Update(tran);
            graph.Save.Press();
        }

        #endregion

        #region Events

        public void _(Events.RowSelected<ArcOpayoTran> e)
        {
            ArcOpayoTran row = e.Row;
            if (row == null) return;

            bool isCompleted = row.Status == TransactionStatusesAttribute.Values.Completed;
            bool isCanceled = row.Status == TransactionStatusesAttribute.Values.Canceled;
            bool isOpened = row.Status == TransactionStatusesAttribute.Values.Open;
            bool isStatementType = (Transaction.Current.Type == TransactionTypesAttribute.Values.Statement);

            ViewRelatedEntity.SetVisible(!isStatementType);
            ViewRelatedEntity.SetEnabled(!isStatementType);
            
            CreatePayment.SetEnabled(isCompleted && row.PaymentNbr == null);
            CreatePayment.SetVisible(isCompleted && row.PaymentNbr == null);

            ReOpenTransaction.SetEnabled(isCanceled);
            ReOpenTransaction.SetVisible(isCanceled);

            CancelTransaction.SetEnabled(isOpened);
            CancelTransaction.SetVisible(isOpened);
        }

        #endregion

    }
}
