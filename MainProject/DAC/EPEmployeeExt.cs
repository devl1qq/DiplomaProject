using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.EP;

namespace MainProject.DAC
{
    public class EPEmployeeExt : PXCacheExtension<EPEmployee>
    {
        #region UsrCaseRoleLevel
        [PXDBString(1, IsFixed = true)]
        [PXStringList(new string[] { "L", "M", "H" }, new string[] { "Level 1", "Level 2", "Level 3" })]
        [PXUIField(DisplayName = "Case Role Level", Visibility = PXUIVisibility.SelectorVisible)]
        public virtual string UsrCaseRoleLevel { get; set; }
        public abstract class usrCaseRoleLevel : PX.Data.BQL.BqlString.Field<usrCaseRoleLevel> { }
        #endregion
    }
}
