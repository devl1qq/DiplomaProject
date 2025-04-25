using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;

namespace MainProject.DAC
{
    public class CustomerExt : PXCacheExtension<Customer>
    {
        #region UsrShippingNotes
        [PXDBString(1024)]
        [PXUIField(DisplayName = "Shipping Notes")]
        public virtual string UsrShippingNotes { get; set; }
        public abstract class usrShippingNotes : PX.Data.BQL.BqlString.Field<usrShippingNotes> { }
        #endregion
        #region UsrHold
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Hold")]
        public virtual bool? UsrHold { get; set; }
        public abstract class usrHold : PX.Data.BQL.BqlBool.Field<usrHold> { }
        #endregion
        #region UsrApproveSalesOrder
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Approve Sales Order")]
        public virtual bool? UsrApproveSalesOrder { get; set; }
        public abstract class usrApproveSalesOrder : PX.Data.BQL.BqlBool.Field<usrApproveSalesOrder> { }
        #endregion
        #region UsrMainProjectHoldReason
        [PXDBString(256)]
        [PXUIField(DisplayName = "Hold Reason")]
        public virtual string UsrMainProjectHoldReason { get; set; }
        public abstract class usrMainProjectHoldReason : PX.Data.BQL.BqlString.Field<usrMainProjectHoldReason> { }
        #endregion
        #region UsrSendInvoiceReminders
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Send Invoice Reminders")]
        public virtual bool? UsrSendInvoiceReminders { get; set; }
        public abstract class usrSendInvoiceReminders : PX.Data.BQL.BqlBool.Field<usrSendInvoiceReminders> { }
        #endregion
        #region UsrAllowBusinessFromBannedCountry
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Allow Business From Banned Country")]
        public virtual bool? UsrAllowBusinessFromBannedCountry { get; set; }
        public abstract class usrAllowBusinessFromBannedCountry : PX.Data.BQL.BqlBool.Field<usrAllowBusinessFromBannedCountry> { }
        #endregion
        #region UsrBAccountExtRefNbr
        public abstract class acctReferenceNbr : PX.Data.BQL.BqlString.Field<acctReferenceNbr> { }
        #endregion
    }
}
