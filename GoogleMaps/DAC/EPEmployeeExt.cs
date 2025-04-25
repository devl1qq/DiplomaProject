using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.EP;

namespace MileageCustomisation.DAC
{
    public class EPEmployeeExt : PXCacheExtension<EPEmployee>
    {
        #region UsrMileageType
        [PXDBInt]
        [PXUIField(DisplayName = "Mileage Type")]
        [PXSelector(typeof(ArcMileageType.mileageID), 
            SubstituteKey = typeof(ArcMileageType.mileageCD), 
            DescriptionField = typeof(ArcMileageType.description))]
        public virtual int? UsrMileageType { get; set; }
        public abstract class usrMileageType : BqlInt.Field<usrMileageType> { }
        #endregion
    }
}
