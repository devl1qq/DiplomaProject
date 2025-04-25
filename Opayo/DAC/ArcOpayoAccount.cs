using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.Tools;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AR;
using PX.Objects.CA;
using PX.Objects.GL;

namespace Opayo.DAC
{
    [PXPrimaryGraph(typeof(ArcOpayoAccountMaint))]
    public class ArcOpayoAccount : PXBqlTable, IBqlTable
    {
        public class PK : PrimaryKeyOf<ArcOpayoAccount>.By<accountID>
        {
            public static ArcOpayoAccount Find(PXGraph graph, int? accountID) => FindBy(graph, accountID);
        }

        #region AccountID
        [PXDBIdentity]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : BqlInt.Field<accountID> { }
        #endregion

        #region AccountCD
        [PXDBString(10, IsKey = true, IsUnicode = true)]
        [PXUIField(DisplayName = "Opayo Account ID")]
        [PXSelector(typeof(ArcOpayoAccount.accountCD))]
        [PXDefault]
        public virtual string AccountCD { get; set; }
        public abstract class accountCD : BqlString.Field<accountCD> { }
        #endregion

        #region Description
        [PXDBString(100, IsUnicode = true)]
        [PXUIField(DisplayName = "Opayo Description")]
        public virtual string Description { get; set; }
        public abstract class description : BqlString.Field<description> { }
        #endregion

        #region WebhookURL
        [PXDBString(255, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Webhook Url")]
        public virtual string WebhookURL { get; set; }
        public abstract class webhookURL : BqlString.Field<webhookURL> { }
        #endregion

        #region Active
        [PXDBBool]
        [PXUIField(DisplayName = "Active")]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? Active { get; set; }
        public abstract class active : BqlBool.Field<active> { }
        #endregion

        #region EnableSalesOrderPaymentLinks
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Sales Order Payment Links")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EnableSalesOrderPaymentLinks { get; set; }
        public abstract class enableSalesOrderPaymentLinks : BqlBool.Field<enableSalesOrderPaymentLinks> { }
        #endregion

        #region EnableInvoicePaymentLinks
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Invoice Payment Links")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EnableInvoicePaymentLinks { get; set; }
        public abstract class enableInvoicePaymentLinks : BqlBool.Field<enableInvoicePaymentLinks> { }
        #endregion

        #region EnableStatementPaymentLinks
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Statement Payment Links")]
        [PXDefault(typeof(enableInvoicePaymentLinks), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EnableStatementPaymentLinks { get; set; }
        public abstract class enableStatementPaymentLinks : BqlBool.Field<enableStatementPaymentLinks> { }
        #endregion


        #region EnableCustomerPaymentLinks
        [PXDBBool]
        [PXUIField(DisplayName = "Enable Customer Payment Links")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? EnableCustomerPaymentLinks { get; set; }
        public abstract class enableCustomerPaymentLinks : BqlBool.Field<enableCustomerPaymentLinks> { }
        #endregion

        #region RegeneratePaymentLinks
        [PXDBBool]
        [PXUIField(DisplayName = "Always Regenerate Payment Links")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? RegeneratePaymentLinks { get; set; }
        public abstract class regeneratePaymentLinks : BqlBool.Field<regeneratePaymentLinks> { }
        #endregion

        #region VPSProtocol
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "VPSProtocol")]
        [PXDefault(typeof(Constants.DefaultVPSProtocol))]
        public virtual string VPSProtocol { get; set; }
        public abstract class vPSProtocol : BqlString.Field<vPSProtocol> { }
        #endregion

        #region TxType
        [PXDBString(20, IsUnicode = true)]
        [PXUIField(DisplayName = "TxType")]
        [PXDefault(typeof(Constants.DefaultTxType))]
        public virtual string TxType { get; set; }
        public abstract class txType : BqlString.Field<txType> { }
        #endregion

        #region VendorEmail
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = "Vendor Email")]
        public virtual string VendorEmail { get; set; }
        public abstract class vendorEmail : BqlString.Field<vendorEmail> { }
        #endregion

        #region PaymentMethod
        [PXDBString(10, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Method")]
        [PXSelector(typeof(Search5<PaymentMethod.paymentMethodID,
            LeftJoin<CustomerPaymentMethod, On<CustomerPaymentMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>,
                LeftJoin<CCProcessingCenterPmntMethod, On<CCProcessingCenterPmntMethod.paymentMethodID, Equal<PaymentMethod.paymentMethodID>>,
                    LeftJoin<CCProcessingCenter, On<CCProcessingCenter.processingCenterID, Equal<CCProcessingCenterPmntMethod.processingCenterID>>>>>,
            Where<PaymentMethod.isActive, Equal<True>, And<PaymentMethod.useForAR, Equal<True>>>,
            Aggregate<GroupBy<PaymentMethod.paymentMethodID, GroupBy<PaymentMethod.useForAR, GroupBy<PaymentMethod.useForAP>>>>>), DescriptionField = typeof(PaymentMethod.descr))]
        public virtual string PaymentMethod { get; set; }
        public abstract class paymentMethod : BqlString.Field<paymentMethod> { }
        #endregion

        #region CashAccount
        [PXUIField(DisplayName = "Cash Account")]
        [CashAccount(null, typeof(Search<CashAccount.cashAccountID,
            Where2<Match<Current<AccessInfo.userName>>,
                And<CashAccount.cashAccountID, In2<Search<PaymentMethodAccount.cashAccountID,
                    Where<PaymentMethodAccount.paymentMethodID, Equal<Current2<ArcOpayoAccount.paymentMethod>>,
                        And<PaymentMethodAccount.useForAR, Equal<True>>>>>>>>), Visibility = PXUIVisibility.Visible)]
        public virtual int? CashAccount { get; set; }
        public abstract class cashAccount : BqlInt.Field<cashAccount> { }
        #endregion

        #region AutoCreatePayments
        [PXDBBool]
        [PXUIField(DisplayName = "Auto Create Payments")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? AutoCreatePayments { get; set; }
        public abstract class autoCreatePayments : BqlBool.Field<autoCreatePayments> { }
        #endregion

        #region AutoReleasePayments
        [PXDBBool]
        [PXUIField(DisplayName = "Auto Release Payments")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? AutoReleasePayments { get; set; }
        public abstract class autoReleasePayments : BqlBool.Field<autoReleasePayments> { }
        #endregion

        #region LiveURL
        [PXDBWeblink]
        [PXUIField(DisplayName = "Live URL")]
        [PXDefault(typeof(Constants.DefaultLiveURL))]
        public virtual string LiveURL { get; set; }
        public abstract class liveURL : BqlString.Field<liveURL> { }
        #endregion

        #region LiveVendor
        //[PXDBString(30, IsUnicode = true)]
        [PXRSACryptString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Live Vendor")]
        public virtual string LiveVendor { get; set; }
        public abstract class liveVendor : BqlString.Field<liveVendor> { }
        #endregion

        #region LiveFIEncryptionPassword
        [PXRSACryptString(16, IsUnicode = true)]
        [PXUIField(DisplayName = "Live FI Encryption Password")]
        public virtual string LiveFIEncryptionPassword { get; set; }
        public abstract class liveFIEncryptionPassword : BqlString.Field<liveFIEncryptionPassword> { }
        #endregion

        #region TestURL
        [PXDBWeblink]
        [PXUIField(DisplayName = "Test URL")]
        [PXDefault(typeof(Constants.DefaultTestURL))]
        public virtual string TestURL { get; set; }
        public abstract class testURL : BqlString.Field<testURL> { }
        #endregion

        #region TestVendor
        //[PXDBString(30, IsUnicode = true)]
        [PXRSACryptString(30, IsUnicode = true)]
        [PXUIField(DisplayName = "Test Vendor")]
        public virtual string TestVendor { get; set; }
        public abstract class testVendor : BqlString.Field<testVendor> { }
        #endregion

        #region TestFIEncryptionPassword
        [PXRSACryptString(16, IsUnicode = true)]
        [PXUIField(DisplayName = "Test FI Encryption Password")]
        public virtual string TestFIEncryptionPassword { get; set; }
        public abstract class testFIEncryptionPassword : BqlString.Field<testFIEncryptionPassword> { }
        #endregion

        #region Environment
        [PXDBString(1)]
        [PXUIField(DisplayName = "Environment")]
        [Environment]
        [PXDefault(EnvironmentAttribute.Values.Live)]
        public virtual string Environment { get; set; }
        public abstract class environment : BqlString.Field<environment> { }
        #endregion

        #region System columns

        #region Tstamp
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        public abstract class Tstamp : BqlByteArray.Field<Tstamp> { }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : BqlGuid.Field<createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : BqlString.Field<createdByScreenID> { }
        #endregion
        #region CreatedDateTime
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : BqlDateTime.Field<createdDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : BqlGuid.Field<lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : BqlString.Field<lastModifiedByScreenID> { }
        #endregion
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #endregion

        public class Constants
        {
            public class DefaultLiveURL : BqlString.Constant<DefaultLiveURL>
            {
                public DefaultLiveURL() : base("https://live.opayo.eu.elavon.com/gateway/service/vspform-register.vsp") { }
            }
            public class DefaultTestURL : BqlString.Constant<DefaultTestURL>
            {
                public DefaultTestURL() : base("https://sandbox.opayo.eu.elavon.com/gateway/service/vspform-register.vsp") { }
            }
            public class DefaultVPSProtocol : BqlString.Constant<DefaultVPSProtocol>
            {
                public DefaultVPSProtocol() : base("4.00") { }
            }
            public class DefaultTxType : BqlString.Constant<DefaultTxType>
            {
                public DefaultTxType() : base("PAYMENT") { }
            }
            public class DefaultEnvironment : BqlString.Constant<DefaultEnvironment>
            {
                public DefaultEnvironment() : base(EnvironmentAttribute.Values.Live) { }
            }
        }
    }
}