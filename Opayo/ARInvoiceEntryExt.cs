using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL.DAC;
using PX.Objects.GL;
using Messages = Opayo.Tools.Messages;
using ARInvoiceExt = Opayo.DAC.ARInvoiceExt;
using PX.Common;
using PX.Objects.SO;

namespace Opayo
{
    public class ARInvoiceEntryExt : PXGraphExtension<ARInvoiceEntry>
    {
        #region Actions

        public delegate IEnumerable PrintInvoiceDelegate(PXAdapter adapter, string reportID = null);
        public delegate IEnumerable EmailInvoiceDelegate(PXAdapter adapter, [PXString] string notificationCD = null);

        [PXOverride]
        public virtual IEnumerable PrintInvoice(PXAdapter adapter, string reportID, PrintInvoiceDelegate baseHandler)
        {
            try
            {
                var res = baseHandler(adapter, reportID);
                return res;
            }
            catch (PXReportRequiredException ex)
            {

                foreach (ARInvoice invoice in adapter.Get<ARInvoice>())
                {
                    try
                    {
                        Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, invoice?.BranchID);
                        Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch.OrganizationID);
                        ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch.GetExtension<BranchExt>().UsrOpayoAccountID) ?? ArcOpayoAccount.PK.Find(Base, organization.GetExtension<OrganizationExt>().UsrOpayoAccountID);
                        bool IsCreditMemo = invoice.DocType == "CRM";
                        if (account != null && (account.RegeneratePaymentLinks == true || string.IsNullOrEmpty(invoice.GetExtension<ARInvoiceExt>().UsrPaymentLink)) && !IsCreditMemo)
                        {
                            ARInvoiceEntry graph = PXGraph.CreateInstance<ARInvoiceEntry>();
                            graph.Document.Update(invoice);
                            graph.GetExtension<ARInvoiceEntryExt>().GeneratePaymentLink.Press();
                        }


                    }
                    catch (Exception e)
                    {
                        PXTrace.WriteError(Messages.LinkNotGeneratedTraceError, e.Message);
                    }
                }
                throw ex;
            }
        }

        [PXOverride]
        public virtual IEnumerable EmailInvoice(PXAdapter adapter, [PXString] string notificationCD, EmailInvoiceDelegate baseHandler)
        {
            foreach (ARInvoice invoice in adapter.Get<ARInvoice>())
            {
                try
                {
                    Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, invoice?.BranchID);
                    Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch.OrganizationID);
                    ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch.GetExtension<BranchExt>().UsrOpayoAccountID) ?? ArcOpayoAccount.PK.Find(Base, organization.GetExtension<OrganizationExt>().UsrOpayoAccountID);

                    bool IsCreditMemo = invoice.DocType == "CRM";
                    if (account != null && (account.RegeneratePaymentLinks == true || string.IsNullOrEmpty(invoice.GetExtension<ARInvoiceExt>().UsrPaymentLink)) && !IsCreditMemo)
                    {
                        ARInvoiceEntry graph = PXGraph.CreateInstance<ARInvoiceEntry>();
                        graph.Document.Update(invoice);
                        graph.GetExtension<ARInvoiceEntryExt>().GeneratePaymentLink.Press();
                        graph.Save.Press();
                    }
                }
                catch (Exception e)
                {
                    PXTrace.WriteError(Messages.LinkNotGeneratedTraceError, e.Message);
                }
            }
            Base.Cancel.Press();
            return baseHandler(adapter, notificationCD);
        }

        public PXAction<ARInvoice> GeneratePaymentLink;
        [PXUIField(DisplayName = "Generate Payment Link")]
        [PXButton(Category = "Opayo")]
        protected IEnumerable generatePaymentLink(PXAdapter adapter)
        {
            ARInvoice invoice = Base.Document.Current;            
            invoice.GetExtension<ARInvoiceExt>().UsrPaymentLink = OpayoLinkGenerator<InvoiceCryptParser, ARInvoiceEntry, ARInvoice>.Generate(Base, invoice);
            Base.Document.Update(invoice);
            Base.Save.Press();

            return adapter.Get();           
        }

        #endregion

        #region Events

        public void _(Events.RowSelected<ARInvoice> e)
        {
            ARInvoice row = e.Row;
            if (row == null) return;

            Branch branch = PXSelect<Branch, Where<Branch.branchID, Equal<Required<Branch.branchID>>>>.Select(Base, row.BranchID);
            Organization organization = PXSelect<Organization, Where<Organization.organizationID, Equal<Required<Branch.organizationID>>>>.Select(Base, branch?.OrganizationID);

            ArcOpayoAccount account = ArcOpayoAccount.PK.Find(Base, branch?.GetExtension<BranchExt>().UsrOpayoAccountID)
                                      ?? ArcOpayoAccount.PK.Find(Base, organization?.GetExtension<OrganizationExt>().UsrOpayoAccountID);

            bool enableOpayoLinkGeneration = account != null && account.EnableInvoicePaymentLinks == true;
            bool isRightInvoiceType = Base.Document.Current.DocType == ARInvoiceType.Invoice ||
                                      Base.Document.Current.DocType == ARInvoiceType.DebitMemo;

            PXUIFieldAttribute.SetEnabled<ARInvoiceExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration && isRightInvoiceType);
            PXUIFieldAttribute.SetVisible<ARInvoiceExt.usrPaymentLink>(e.Cache, row, enableOpayoLinkGeneration && isRightInvoiceType);
            GeneratePaymentLink.SetEnabled(enableOpayoLinkGeneration && isRightInvoiceType);
            GeneratePaymentLink.SetVisible(enableOpayoLinkGeneration && isRightInvoiceType);
        }

        #endregion
    }
}
