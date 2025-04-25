using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using PX.Objects.AM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class ArcAMWCLocations : PXBqlTable, IBqlTable
    {
        #region Keys
        public class PK : PrimaryKeyOf<ArcAMWCLocations>.By<locationID>
        {
            public static ArcAMWCLocations Find(PXGraph graph, int? locationID) => FindBy(graph, locationID);
        }
        public static class FK
        {
            public class WorkCenter : AMWC.PK.ForeignKeyOf<ArcAMWCLocations>.By<wCID> { }
        }
        #endregion
        #region WcID
        [PXDBString(20)]
        [PXDBDefault(typeof(AMWC.wcID))]
        [PXParent(typeof(FK.WorkCenter))]
        public virtual string WCID { get; set; }
        public abstract class wCID : PX.Data.BQL.BqlString.Field<wCID> { }
        #endregion

        #region LocationID
        [PXDBIdentity]
        public virtual int? LocationID { get; set; }
        public abstract class locationID : PX.Data.BQL.BqlInt.Field<locationID> { }
        #endregion

        #region LocationCD
        [PXDBString(30, IsKey = true, InputMask = "")]
        [PXDefault]
        [PXUIField(DisplayName = "Location", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string LocationCD { get; set; }
        public abstract class locationCD : PX.Data.BQL.BqlString.Field<locationCD> { }
        #endregion

        #region Description
        [PXDBString(60)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }
        public abstract class description : PX.Data.BQL.BqlString.Field<description> { }
        #endregion

        #region Active
        [PXDBBool()]
        [PXDefault(false)]
        [PXUIField(DisplayName = "Active")]
        public virtual bool? Active { get; set; }
        public abstract class active : PX.Data.BQL.BqlBool.Field<active> { }
        #endregion

        #region System Columns
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

        #region NoteID
        [PXNote()]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : PX.Data.BQL.BqlGuid.Field<noteID> { }
        #endregion
        #endregion
    }
}
