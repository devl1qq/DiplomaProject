using PX.Data;
using PX.Data.ReferentialIntegrity.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    [Serializable]
    [PXCacheName("ArcVendorScore")]
    public class ArcVendorScore : PXBqlTable, IBqlTable
    {

        #region Keys
        public class PK : PrimaryKeyOf<ArcVendorScore>.By<scoreID>
        {
            public static ArcVendorScore Find(PXGraph graph, int? scoreID, PKFindOptions options = PKFindOptions.None) => FindBy(graph, scoreID, options);
            public static ArcVendorScore FindDirty(PXGraph graph, int? scoreID)
                => (ArcVendorScore)PXSelect<ArcVendorScore, Where<scoreID, Equal<Required<scoreID>>>>.SelectWindowed(graph, 0, 1, scoreID);
        }
        #endregion

        #region ScoreID
        [PXDBIdentity]
        public virtual int? ScoreID { get; set; }
        public abstract class scoreID : PX.Data.BQL.BqlInt.Field<scoreID> { }
        #endregion

        #region Score
        [PXDBString(5, IsUnicode = true, InputMask = "", IsKey = true)]
        [PXDefault]
        [PXUIField(DisplayName = "Score", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Score { get; set; }
        public abstract class score : PX.Data.BQL.BqlString.Field<score> { }
        #endregion

        #region Priority
        [PXDBInt()]
        [PXDefault]
        [PXUIField(DisplayName = "Priority", Required = true, Visibility = PXUIVisibility.SelectorVisible)]
        public virtual int? Priority { get; set; }
        public abstract class priority : PX.Data.BQL.BqlInt.Field<priority> { }
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
