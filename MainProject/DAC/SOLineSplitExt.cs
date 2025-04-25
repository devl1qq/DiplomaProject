using PX.Data;
using PX.Objects.CR;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class SOLineSplitExt : PXCacheExtension<SOLineSplit>
    {
        #region UsrNCQty
        [PXInt]
        [PXUIField(DisplayName = "NC Qty")]
        [PXDefault(0, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual int? UsrNCQty { get; set; }
        public abstract class usrNCQty : PX.Data.BQL.BqlInt.Field<usrNCQty> { }
        #endregion
        #region UsrSelected
        [PXBool]
        [PXUIField(DisplayName = "Selected")]
        [PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual bool? UsrSelected { get; set; }
        public abstract class usrSelected : PX.Data.BQL.BqlBool.Field<usrSelected> { }
        #endregion
        #region UsrCaseClassID
        [PXString(10, IsUnicode = true, InputMask = ">aaaaaaaaaa")]
        [PXUIField(DisplayName = "Case Class")]
        [PXSelector(typeof(CRCaseClass.caseClassID),
            DescriptionField = typeof(CRCaseClass.description),
            CacheGlobal = true)]
        public virtual string UsrCaseClassID { get; set; }
        public abstract class usrCaseClassID : PX.Data.BQL.BqlString.Field<usrCaseClassID> { }
        #endregion
    }
}
