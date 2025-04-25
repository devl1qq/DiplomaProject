using System;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.GL;
using MainProject.Attributes;

namespace MainProject.DAC
{
    [Serializable]
    [PXCacheName("ArcSalesTargets")]
    public class ArcSalesTargets : PXBqlTable, IBqlTable
    {
        #region SalesTargetID
        [PXDBIdentity]
        [PXUIField(DisplayName = "Sales target", IsReadOnly = true)]
        public virtual int? SalesTargetID { get; set; }
        public abstract class salesTargetID : PX.Data.BQL.BqlInt.Field<salesTargetID> { }
        #endregion

        #region Date
        [PXDBDate()]
        [PXUIField(DisplayName = "Date", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual DateTime? Date { get; set; }
        public abstract class date : PX.Data.BQL.BqlDateTime.Field<date> { }
        #endregion

        #region SalesPersonID
        [PXDefault]
        [SalesPerson(DisplayName = "Salesperson", IsKey = true, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? SalesPersonID { get; set; }
        public abstract class salesPersonID : PX.Data.BQL.BqlInt.Field<salesPersonID> { }
        #endregion

        #region Subaccount
        [AllSubAccountSelector(Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault]
        public virtual int? Subaccount { get; set; }
        public abstract class subaccount : PX.Data.BQL.BqlInt.Field<subaccount> { }
        #endregion

        #region Target
        [PXDBDecimal()]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXUIField(DisplayName = "Target", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual Decimal? Target { get; set; }
        public abstract class target : PX.Data.BQL.BqlDecimal.Field<target> { }
        #endregion

        #region System columns
        #region Tstamp
        [PXDBTimestamp()]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : PX.Data.BQL.BqlByteArray.Field<tstamp> { }
        #endregion

        #region CreatedByID
        [PXDBCreatedByID()]
        public virtual Guid? CreatedByID { get; set; }
        public abstract class createdByID : PX.Data.BQL.BqlGuid.Field<createdByID> { }
        #endregion

        #region CreatedByScreenID
        [PXDBCreatedByScreenID()]
        public virtual string CreatedByScreenID { get; set; }
        public abstract class createdByScreenID : PX.Data.BQL.BqlString.Field<createdByScreenID> { }
        #endregion

        #region CreatedDateTime
        [PXDBCreatedDateTime()]
        public virtual DateTime? CreatedDateTime { get; set; }
        public abstract class createdDateTime : PX.Data.BQL.BqlDateTime.Field<createdDateTime> { }
        #endregion

        #region LastModifiedByID
        [PXDBLastModifiedByID()]
        public virtual Guid? LastModifiedByID { get; set; }
        public abstract class lastModifiedByID : PX.Data.BQL.BqlGuid.Field<lastModifiedByID> { }
        #endregion

        #region LastModifiedByScreenID
        [PXDBLastModifiedByScreenID()]
        public virtual string LastModifiedByScreenID { get; set; }
        public abstract class lastModifiedByScreenID : PX.Data.BQL.BqlString.Field<lastModifiedByScreenID> { }
        #endregion

        #region LastModifiedDateTime
        [PXDBLastModifiedDateTime()]
        public virtual DateTime? LastModifiedDateTime { get; set; }
        public abstract class lastModifiedDateTime : PX.Data.BQL.BqlDateTime.Field<lastModifiedDateTime> { }
        #endregion

        #region Noteid
        [PXNote()]
        public virtual Guid? Noteid { get; set; }
        public abstract class noteid : PX.Data.BQL.BqlGuid.Field<noteid> { }
        #endregion
        #endregion
    }
}