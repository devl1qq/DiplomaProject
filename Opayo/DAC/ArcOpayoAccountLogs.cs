using PX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PX.SM.AUStepField;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using static PX.SM.TableReference;

namespace Opayo.DAC
{
    public class ArcOpayoAccountLogs : PXBqlTable, IBqlTable
    {
        #region LogID
        [PXDBIdentity(IsKey = true)]
        public virtual int? LogID { get; set; }
        public abstract class logID : BqlInt.Field<logID> { }
        #endregion

        #region AccountID
        [PXDBInt]
        [PXDBDefault(typeof(ArcOpayoAccount.accountID))]
        [PXParent(typeof(Select<ArcOpayoAccount, Where<ArcOpayoAccount.accountID, Equal<Current<accountID>>>>))]
        public virtual int? AccountID { get; set; }
        public abstract class accountID : BqlInt.Field<accountID> { }
        #endregion

        #region Description
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Description")]
        public virtual string Description { get; set; }
        public abstract class description : BqlString.Field<description> { }
        #endregion

        #region WebhookLink
        [PXDBString(250, IsUnicode = true)]
        [PXUIField(DisplayName = "Webhook Link")]
        public virtual string WebhookLink { get; set; }
        public abstract class webhookLink : BqlString.Field<webhookLink> { }
        #endregion

        #region Error
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Error")]
        public virtual string Error { get; set; }
        public abstract class error : BqlString.Field<error> { }
        #endregion

        #region Crypt
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Crypt")]
        public virtual string Crypt { get; set; }
        public abstract class crypt : BqlString.Field<crypt> { }
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
        [PXDBCreatedDateTime(InputMask = "g")]
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
        #region Noteid
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #endregion

    }
}
