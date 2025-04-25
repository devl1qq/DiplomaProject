using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.EP;
using PX.Objects.IN;
using PX.Objects.IN.Services;

namespace MileageCustomisation.DAC
{
    public class EPSetupExt : PXCacheExtension<EPSetup>
    {
        #region UsrMileageItem
        [PXDBInt]
        [PXUIField(DisplayName = "Mileage Item")]
        [PXSelector(typeof(Search<InventoryItem.inventoryID, Where<InventoryItem.stkItem.IsEqual<False>>>), 
            SubstituteKey = typeof(InventoryItem.inventoryCD), 
            DescriptionField = typeof(InventoryItem.descr))]
        public virtual int? UsrMileageItem { get; set; }
        public abstract class usrMileageItem : BqlInt.Field<usrMileageItem> { }
        #endregion

        #region UsrGooleMapsAPIKey
        [PXRSACryptString(256, IsUnicode = true)]
        [PXUIField(DisplayName = "Google Maps API Key")]
        public virtual string UsrGooleMapsAPIKey { get; set; }
        public abstract class usrGooleMapsAPIKey : BqlString.Field<usrGooleMapsAPIKey> { }
        #endregion

        #region UsrLocationInputMask 
        [PXDBString(50)]
        [PXUIField(DisplayName = "Location Input Mask")]
        public virtual string UsrLocationInputMask { get; set; }
        public abstract class usrLocationInputMask : BqlString.Field<usrLocationInputMask> { }
        #endregion

        #region UsrLocationValidationRegexp
        [PXDBString(255)]
        [PXUIField(DisplayName = "Location Validation Regexp")]
        public virtual string UsrLocationValidationRegexp { get; set; }
        public abstract class usrLocationValidationRegexp : BqlString.Field<usrLocationValidationRegexp> { }
        #endregion
    }
}
