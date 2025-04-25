using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.SO;
using static MainProject.DAC.SOOrderExt.usrCustomerDescription;

namespace MainProject.DAC
{
    public class SOOrderExt : PXCacheExtension<SOOrder>
    {
        #region UsrCustomerDescription
        [PXString(60)]
        [PXUIField(DisplayName = "Customer Class Description", Enabled = false)]
        [CustomerClassDescrSelecting]

        public virtual string UsrCustomerDescription { get; set; }
        public abstract class usrCustomerDescription : PX.Data.BQL.BqlString.Field<usrCustomerDescription> 
        {
            public class CustomerClassDescrSelectingAttribute : PXEventSubscriberAttribute, IPXFieldSelectingSubscriber
            {
                public void FieldSelecting(PXCache sender, PXFieldSelectingEventArgs e)
                {
                    var row = (SOOrder)e.Row;
                    if (row == null) return;

                    var customer = Customer.PK.Find(sender.Graph, row.CustomerID);
                    var customerClass = CustomerClass.PK.Find(sender.Graph, customer?.CustomerClassID);
                    e.ReturnValue = customerClass?.Descr;
                }
            }
        }
        #endregion
        #region UsrHold
        [PXBool()]
        [PXUIField(DisplayName = "Customer On Hold")]
        [PXFormula(typeof(Selector<SOOrder.customerID, CustomerExt.usrHold>))]
        public virtual bool? UsrHold { get; set; }
        public abstract class usrHold : PX.Data.BQL.BqlBool.Field<usrHold> { }
        #endregion
        #region UsrCustomerApproveSalesOrder
        [PXBool()]
        [PXFormula(typeof(Selector<SOOrder.customerID, CustomerExt.usrApproveSalesOrder>))]
        public virtual bool? UsrCustomerApproveSalesOrder { get; set; }
        public abstract class usrCustomerApproveSalesOrder : PX.Data.BQL.BqlBool.Field<usrCustomerApproveSalesOrder> { }
        #endregion
        #region UsrCustomerApproved
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Customer Approved")]
        public virtual bool? UsrCustomerApproved { get; set; }
        public abstract class usrCustomerApproved : PX.Data.BQL.BqlBool.Field<usrCustomerApproved> { }
        #endregion
        #region UsrMainProjectHoldReason
        [PXDBString(256)]
        [PXUIField(DisplayName = "Hold Reason")]
        public virtual string UsrMainProjectHoldReason { get; set; }
        public abstract class usrMainProjectHoldReason : PX.Data.BQL.BqlString.Field<usrMainProjectHoldReason> { }
        #endregion
        #region UsrFlowDownNotes
        [PXDBInt]
        [PXUIField(DisplayName = "Flow Down Notes")]
        [PXSelector(typeof(Search<ArcFlowDownNotes.downNoteID, Where<ArcFlowDownNotes.active, Equal<True>>>),
            SubstituteKey = typeof(ArcFlowDownNotes.downNoteCD))]
        [PXDefault]
        public virtual int? UsrFlowDownNotes { get; set; }
        public abstract class usrFlowDownNotes : PX.Data.BQL.BqlInt.Field<usrFlowDownNotes> { }
        #endregion
        #region UsrOrderRating
        [PXDBInt]
        [PXUIField(DisplayName = "Order Rating")]
        [PXSelector(typeof(Search<ArcOrderRating.ratingID, Where<ArcOrderRating.active, Equal<True>>>),
            SubstituteKey = typeof(ArcOrderRating.ratingCD))]
        [PXDefault]
        public virtual int? UsrOrderRating { get; set; }
        public abstract class usrOrderRating : PX.Data.BQL.BqlInt.Field<usrOrderRating> { }
        #endregion
        #region UsrConfirmed
        [PXDBBool()]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = "Confirmed")]
        public virtual bool? UsrConfirmed { get; set; }
        public abstract class usrConfirmed : PX.Data.BQL.BqlBool.Field<usrConfirmed> { }
        #endregion
        #region UsrDraft
        [PXDBString(1, IsUnicode = true, InputMask = "")]
        [PXStringList(new string[] { "D", "C" }, new string[] { "Draft", "Confirmed" })]
        [PXUIField(DisplayName = "Draft", IsReadOnly = true)]
        public virtual string UsrDraft { get; set; }
        public abstract class usrDraft : PX.Data.BQL.BqlString.Field<usrDraft> { }
        #endregion
        #region UsrBannedCountry
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Banned Country", IsReadOnly = true)]
        public virtual bool? UsrBannedCountry { get; set; }
        public abstract class usrBannedCountry : PX.Data.BQL.BqlBool.Field<usrBannedCountry> { }
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
