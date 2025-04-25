using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MileageCustomisation.Graph;
using PX.Data;
using PX.Data.BQL;
using PX.Data.ReferentialIntegrity.Attributes;

namespace MileageCustomisation.DAC
{
    [PXPrimaryGraph(typeof(ArcMileageTypeMaint))]
    public class ArcMileageType : PXBqlTable, IBqlTable
    {
        #region Keys

        public class PK : PrimaryKeyOf<ArcMileageType>.By<mileageID>
        {
            public static ArcMileageType Find(PXGraph graph, int? mileageId) => FindBy(graph, mileageId);
        }

        #endregion

        #region MileageID
        [PXDBIdentity]
        public virtual int? MileageID { get; set; }
        public abstract class mileageID : BqlInt.Field<mileageID> { }
        #endregion

        #region MileageCD
        [PXDBString(30, IsKey = true)]
        [PXUIField(DisplayName = "Mileage CD", Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(mileageCD))]
        [PXDefault]
        public virtual string MileageCD { get; set; }
        public abstract class mileageCD : BqlString.Field<mileageCD> { }
        #endregion

        #region MileageType
        [PXDBString(60)]
        [PXUIField(DisplayName = "Mileage Type", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string MileageType { get; set; }
        public abstract class mileageType : BqlString.Field<mileageType> { }
        #endregion

        #region Description
        [PXDBString(100)]
        [PXUIField(DisplayName = "Description", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string Description { get; set; }
        public abstract class description : BqlString.Field<description> { }
        #endregion

        #region FuelPart
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Fuel Part", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? FuelPart { get; set; }
        public abstract class fuelPart : BqlDecimal.Field<fuelPart> { }
        #endregion

        #region WearAndTearPart
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Wear and Tear Part", Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        public virtual decimal? WearAndTearPart { get; set; }
        public abstract class wearAndTearPart : BqlDecimal.Field<wearAndTearPart> { }
        #endregion

        #region Rate
        [PXDBDecimal(4)]
        [PXUIField(DisplayName = "Rate", Visibility = PXUIVisibility.SelectorVisible, Enabled = false)]
        [PXDefault(TypeCode.Decimal, "0.0")]
        [PXFormula(typeof(Add<fuelPart, wearAndTearPart>))]
        public virtual decimal? Rate { get; set; }
        public abstract class rate : BqlDecimal.Field<rate> { }
        #endregion

        #region System fields

        #region Tstamp
        [PXDBTimestamp]
        [PXUIField(DisplayName = "Tstamp")]
        public virtual byte[] Tstamp { get; set; }
        public abstract class tstamp : BqlByteArray.Field<tstamp> { }
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
        #region Noteid
        [PXNote]
        public virtual Guid? NoteID { get; set; }
        public abstract class noteID : BqlGuid.Field<noteID> { }
        #endregion

        #endregion
    }
}
