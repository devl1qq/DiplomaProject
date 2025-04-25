using PX.Data;
using PX.Data.BQL;
using PX.Objects.CS;
using PX.Objects.IN;
using static PX.Data.BQL.BqlPlaceholder;
using static PX.Data.BQL.Fluent.FbqlJoins;

namespace MainProject.DAC
{
    public class inventorySegmentID : BqlString.Constant<inventorySegmentID>
    {
        public inventorySegmentID() : base("INVENTORY") { }
    }
    public class four : BqlInt.Constant<four>
    {
        public four() : base(4) { }
    }
    public class InventoryItemExt : PXCacheExtension<InventoryItem>
    {
        #region UsrItemURL
        [PXDBWeblink]
        [PXUIField(DisplayName = "Item URL")]
        public virtual string UsrItemURL { get; set; }
        public abstract class usrItemURL : PX.Data.BQL.BqlString.Field<usrItemURL> { }
        #endregion

        #region UsrManufacturer
        [PXDBString(4)]
        [PXUIField(DisplayName = "Manufacturer", IsReadOnly = true)]
        [PXSelector(typeof(Search<SegmentValue.value,
            Where<SegmentValue.dimensionID, Equal<inventorySegmentID>, 
                And<SegmentValue.value, Equal<Left<Current<InventoryItem.inventoryCD>, four>>>>>),
            DescriptionField = typeof(SegmentValue.descr))]
        public virtual string UsrManufacturer { get; set; }
        public abstract class usrManufacturer : PX.Data.BQL.BqlString.Field<usrManufacturer> { }
        #endregion

        #region UsrMSL
        [PXDBString(2)]
        [PXUIField(DisplayName = "MSL")]
        [PXStringList(new[] { "1", "2", "2a", "3", "4", "5", "5a", "6" }, new[] { "1", "2", "2a", "3", "4", "5", "5a", "6" })]
        public virtual string UsrMSL { get; set; }
        public abstract class usrMSL : PX.Data.BQL.BqlString.Field<usrMSL> { }
        #endregion

        #region UsrComponentBodyTypeID
        [PXDBInt]
        [PXUIField(DisplayName = "Component Body Type")]
        [PXSelector(typeof(Search<ArcComponentBodyType.codeID, Where<ArcComponentBodyType.active, Equal<True>>>), 
            DescriptionField = typeof(ArcComponentBodyType.description), 
            SubstituteKey = typeof(ArcComponentBodyType.codeCD))]
        public virtual int? UsrComponentBodyTypeID { get; set; }
        public abstract class usrComponentBodyTypeID : PX.Data.BQL.BqlInt.Field<usrComponentBodyTypeID> { }
        #endregion
    }
}
