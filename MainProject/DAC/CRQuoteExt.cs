using System;
using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AM;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.SO;
using static PX.Objects.AR.CustomerMaint;
using static MainProject.DAC.CRQuoteExt.usrAvailableCredit;
using static MainProject.DAC.CRQuoteExt.usrCreditLimit;
using static MainProject.DAC.SOOrderExt.usrCustomerDescription;

namespace MainProject.DAC
{
    public class CRQuoteExt : PXCacheExtension<CRQuote>
    {
        #region Keys
        public class PK : PrimaryKeyOf<CRQuote>.By<CRQuote.noteID>
        {
            public static CRQuote Find(PXGraph graph, Guid? noteID)
                => FindBy(graph, noteID);
        }
        #endregion
        #region BAccountID
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(CRMBAccountAttribute))]
        [CRMBAccount(bAccountTypes: new Type[]
            {
                typeof(BAccountType.prospectType),
                typeof(BAccountType.customerType),
                typeof(BAccountType.combinedType),
            },
            fieldList: new Type[]
            {
                typeof (BAccount.acctCD),
                typeof (BAccount.acctName),
                typeof (BAccount.acctReferenceNbr),
                typeof (BAccount.type),
                typeof (BAccount.classID),
                typeof (BAccount.status),
                typeof (Contact.phone1),
                typeof (Address.city),
                typeof (Address.state),
                typeof (Address.countryID),
                typeof (Contact.eMail)
            },
            BqlField = typeof(PX.Objects.CR.Standalone.CROpportunityRevision.bAccountID),
            Enabled = false)]
        [PXDefault(typeof(Search<CROpportunity.bAccountID, Where<CROpportunity.opportunityID, Equal<Current<CRQuote.opportunityID>>>>), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual Int32? BAccountID { get; set; }
        public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
        #endregion
        #region UsrFlowDownNotes
        [PXDBInt(BqlField = typeof(CRQuoteStandaloneExt.usrFlowDownNotes))]
        [PXUIField(DisplayName = "Flow Down Notes")]
        [PXSelector(typeof(Search<ArcFlowDownNotes.downNoteID, Where<ArcFlowDownNotes.active, Equal<True>>>),
            SubstituteKey = typeof(ArcFlowDownNotes.downNoteCD))]
        public virtual int? UsrFlowDownNotes { get; set; }
        public abstract class usrFlowDownNotes : PX.Data.BQL.BqlInt.Field<usrFlowDownNotes> { }
        #endregion
        #region UsrCreditLimit
        [PXDecimal()]
        [PXUIField(DisplayName = "Credit Limit", IsReadOnly = true)]
        [CustomerCreditLimitSelector]
        public virtual decimal? UsrCreditLimit { get; set; }
        public abstract class usrCreditLimit : PX.Data.BQL.BqlDecimal.Field<usrCreditLimit>
        {
            public class CustomerCreditLimitSelectorAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
            {
                public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
                {
                    var row = (CRQuote)e.Row;
                    if (row == null) return;

                    var customer = Customer.PK.Find(sender.Graph, row.BAccountID);
                    e.ReturnValue = customer?.CreditLimit;
                }
            }
        }
        #endregion
        #region UsrAvailableCredit
        [PXDecimal()]
        [PXUIField(DisplayName = "Available Credit", IsReadOnly = true)]
        [CustomerRemainingCreditLimitSelector]
        public virtual decimal? UsrAvailableCredit { get; set; }
        public abstract class usrAvailableCredit : PX.Data.BQL.BqlDecimal.Field<usrAvailableCredit>
        {
            public class CustomerRemainingCreditLimitSelectorAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
            {
                public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
                {
                    var row = (CRQuote)e.Row;
                    if (row == null) return;

                    var customer = Customer.PK.Find(sender.Graph, row.BAccountID);
                    if ((customer?.CreditRule == "C" || customer?.CreditRule == "B") && customer != null)
                    {
                        ARBalances aRBalances = GetCustomerBalances<PX.Objects.AR.Override.Customer.sharedCreditCustomerID>(sender.Graph, customer?.BAccountID);
                        if (customer?.SharedCreditChild == true)
                        {
                            aRBalances = GetCustomerBalances<PX.Objects.AR.Override.Customer.sharedCreditCustomerID>(sender.Graph, customer?.SharedCreditCustomerID);
                        }

                        e.ReturnValue = customer?.CreditLimit -
                            (decimal?)(aRBalances?.CurrentBal.GetValueOrDefault() +
                            aRBalances?.UnreleasedBal.GetValueOrDefault() +
                            aRBalances?.TotalOpenOrders.GetValueOrDefault() +
                            aRBalances?.TotalShipped.GetValueOrDefault() -
                            aRBalances?.TotalPrepayments.GetValueOrDefault());
                    }
                    else
                    {
                        e.ReturnValue = 0m;
                    }
                }
            }
        }
        #endregion
        #region UsrSalesPerson
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = "Sales Person", Visibility = PXUIVisibility.Visible)]
        [PXSelector(typeof(Search<SalesPerson.salesPersonCD>), DescriptionField = typeof(SalesPerson.descr))]
        public virtual string UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlString.Field<usrSalesPerson> { }
        #endregion
        #region UsrBDM
        [PXDBInt]
        [PXUIField(DisplayName = "BDM")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<bdmTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrBDM { get; set; }
        public abstract class usrBDM : PX.Data.BQL.BqlInt.Field<usrBDM> { }
        #endregion
        #region UsrKAM
        [PXDBInt]
        [PXUIField(DisplayName = "KAM")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<kamTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrKAM { get; set; }
        public abstract class usrKAM : PX.Data.BQL.BqlInt.Field<usrKAM> { }
        #endregion
        #region UsrCSR
        [PXDBInt]
        [PXUIField(DisplayName = "CSR")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonID, Where<SalesPerson.isActive, Equal<True>, And<SalesPersonExt.usrRole, Equal<csrTypeConstant>>>>),
            SubstituteKey = typeof(SalesPerson.salesPersonCD), DescriptionField = typeof(SalesPerson.descr))]
        public virtual int? UsrCSR { get; set; }
        public abstract class usrCSR : PX.Data.BQL.BqlInt.Field<usrCSR> { }
        #endregion
    }
}
