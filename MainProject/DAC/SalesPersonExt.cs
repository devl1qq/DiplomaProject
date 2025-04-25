using PX.Data;
using PX.Data.BQL;
using PX.Objects.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainProject.DAC.SalesPersonExt;

namespace MainProject.DAC
{
    public class bdmTypeConstant : BqlString.Constant<bdmTypeConstant>
    {
        public bdmTypeConstant() : base("BDM") { }
    }
    public class kamTypeConstant : BqlString.Constant<kamTypeConstant>
    {
        public kamTypeConstant() : base("KAM") { }
    }
    public class csrTypeConstant : BqlString.Constant<csrTypeConstant>
    {
        public csrTypeConstant() : base("CSR") { }
    }
    public class SalesPersonExt : PXCacheExtension<SalesPerson>
    {
        #region UsrRole
        [PXDBString(3, IsFixed = true)]
        [PXUIField(DisplayName = "Role")]
        [PXStringList(new[] { "BDM", "KAM", "CSR" }, new[] { "BDM", "KAM", "CSR" })]
        public virtual string UsrRole { get; set; }
        public abstract class usrRole : BqlString.Field<usrRole> { }
        #endregion
    }
}
