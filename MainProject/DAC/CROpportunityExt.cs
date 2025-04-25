using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using System;
using System.Linq;
using static PX.Objects.AR.ARDocumentEnq.ARDocumentFilter;
using static PX.Objects.AR.CustomerMaint;
using static MainProject.DAC.CROpportunityExt.usrAvailableCredit;
using static MainProject.DAC.CROpportunityExt.usrCreditLimit;

namespace MainProject.DAC
{
    public class CROpportunityExt : PXCacheExtension<CROpportunity>
    {
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
            BqlField = typeof(PX.Objects.CR.Standalone.CROpportunityRevision.bAccountID)
            )]
        public virtual Int32? BAccountID { get; set; }
        public abstract class bAccountID : PX.Data.BQL.BqlInt.Field<bAccountID> { }
        #endregion
        #region UsrCreditLimit
        [PXDecimal()]
        [PXUIField(DisplayName = "Credit Limit", IsReadOnly = true)]
        [CustomerCreditLimitSelector]
        public virtual decimal? UsrCreditLimit { get; set; }
        public abstract class usrCreditLimit : PX.Data.BQL.BqlDecimal.Field<usrCreditLimit> {
            public class CustomerCreditLimitSelectorAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
            {
                public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
                {
                    var row = (CROpportunity)e.Row;
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
                    var row = (CROpportunity)e.Row;
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
        [PXUIField(DisplayName = "Sales Person")]
        [PXSelector(typeof(Search<SalesPerson.salesPersonCD>), DescriptionField = typeof(SalesPerson.descr))]
        public virtual string UsrSalesPerson { get; set; }
        public abstract class usrSalesPerson : PX.Data.BQL.BqlString.Field<usrSalesPerson> { }
        #endregion
    }
}
