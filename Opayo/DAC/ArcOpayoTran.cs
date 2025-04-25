using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opayo.Tools;
using static PX.Objects.IN.INPlanType;
using static PX.SM.AUStepField;
using PX.Objects.AR;
using PX.Objects.CR;
using static PX.Data.PXGenericInqGrph;

namespace Opayo.DAC
{
    [PXPrimaryGraph(typeof(ArcOpayoTranMaint))]
    public class ArcOpayoTran : PXBqlTable, IBqlTable
    {
        #region TranID
        [PXDBIdentity(IsKey = true)]
        [PXUIField(DisplayName = "ID")]
        [PXSelector(typeof(tranID), 
            typeof(tranID),
            typeof(vendorTxCode), 
            typeof(status), 
            typeof(type), 
            typeof(date),
            typeof(customerID),
            typeof(opayoAccountID),
            typeof(paymentType),
            typeof(paymentNbr),
            typeof(paymentDate)
        )]
        public virtual int? TranID { get; set; }
        public abstract class tranID : PX.Data.BQL.BqlInt.Field<tranID> { }
        #endregion

        #region VendorTxCode
        [PXDBString(40)]
        [PXUIField(DisplayName = "VendorTxCode")]
        public virtual string VendorTxCode { get; set; }
        public abstract class vendorTxCode : PX.Data.BQL.BqlString.Field<vendorTxCode> { }
        #endregion

        #region OpayoAccountID
        [PXDBInt]
        [PXUIField(DisplayName = "Opayo Account")]
        [PXSelector(typeof(Search<ArcOpayoAccount.accountID, Where<ArcOpayoAccount.active, Equal<True>>>),
            typeof(ArcOpayoAccount.accountCD),
            typeof(ArcOpayoAccount.enableSalesOrderPaymentLinks),
            typeof(ArcOpayoAccount.enableInvoicePaymentLinks),
            typeof(ArcOpayoAccount.enableCustomerPaymentLinks),
            typeof(ArcOpayoAccount.description),
            typeof(ArcOpayoAccount.vPSProtocol),
            typeof(ArcOpayoAccount.txType),
            typeof(ArcOpayoAccount.vendorEmail),
            typeof(ArcOpayoAccount.paymentMethod),
            typeof(ArcOpayoAccount.cashAccount),
            typeof(ArcOpayoAccount.autoCreatePayments),
            SubstituteKey = typeof(ArcOpayoAccount.accountCD))]
        public virtual int? OpayoAccountID { get; set; }
        public abstract class opayoAccountID : PX.Data.BQL.BqlInt.Field<opayoAccountID> { }
        #endregion

        #region Date
        [PXDBDate]
        [PXUIField(DisplayName = "Date")]
        [PXDefault(typeof(AccessInfo.businessDate), PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual DateTime? Date { get; set; }
        public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
        #endregion

        #region CustomerID
        [PXUIField(DisplayName = "Customer ID")]
        [Customer(
            typeof(Search<BAccountR.bAccountID, Where<True, Equal<True>>>),
            Visibility = PXUIVisibility.SelectorVisible,
            DescriptionField = typeof(Customer.acctName),
            Filterable = true)]
        public virtual int? CustomerID { get; set; }
        public abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion

        #region Type
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Type")]
        [TransactionTypes]
        public virtual string Type { get; set; }
        public abstract class type : PX.Data.BQL.BqlString.Field<type> { }
        #endregion

        #region RelatedEntity
        [PXDBGuid]
        [PXUIField(DisplayName = "Related Entity")]
        public virtual Guid? RelatedEntity { get; set; }
        public abstract class relatedEntity : PX.Data.BQL.BqlGuid.Field<relatedEntity> { }
        #endregion

        #region Status
        [PXDBString(1, IsFixed = true)]
        [PXUIField(DisplayName = "Status")]
        [TransactionStatuses]
        [PXDefault(TransactionStatusesAttribute.Values.Open)]
        public virtual string Status { get; set; }
        public abstract class status : PX.Data.BQL.BqlString.Field<status> { }
        #endregion

        #region PaymentLink
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Link")]
        public virtual string PaymentLink { get; set; }
        public abstract class paymentLink : PX.Data.BQL.BqlString.Field<paymentLink> { }
        #endregion

        #region PaymentDate
        [PXDBDate(PreserveTime = true, InputMask = "g")]
        [PXUIField(DisplayName = "Payment Date")]
        public virtual DateTime? PaymentDate { get; set; }
        public abstract class paymentDate : PX.Data.BQL.BqlDateTime.Field<paymentDate> { }
        #endregion

        #region PaymentType
        [PXDBString(3, IsFixed = true)]
        [PXUIField(DisplayName = "Payment Type")]
        [ARPaymentType.ListEx]
        public virtual string PaymentType { get; set; }
        public abstract class paymentType : PX.Data.BQL.BqlString.Field<paymentType> { }
        #endregion

        #region PaymentNbr
        [PXDBString(15)]
        [PXUIField(DisplayName = "Payment Nbr.")]
        [PXSelector(typeof(Search<ARPayment.refNbr, Where<ARPayment.docType, Equal<Current<paymentType>>>>))]
        public virtual string PaymentNbr { get; set; }
        public abstract class paymentNbr : PX.Data.BQL.BqlString.Field<paymentNbr> { }
        #endregion

        #region System columns

        #region Tstamp
        [PXDBTimestamp]
        public virtual byte[] tstamp { get; set; }
        public abstract class Tstamp : PX.Data.BQL.BqlByteArray.Field<Tstamp> { }
        #endregion
        #region CreatedByID
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion
        #region CreatedByScreenID
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion
        #region CreatedDateTime
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion
        #region LastModifiedByID
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion
        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion
        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion
        #region NoteID
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion

        #endregion
    }
}
