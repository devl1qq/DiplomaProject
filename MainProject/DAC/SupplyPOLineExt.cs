using PX.Data;
using PX.Objects.SO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class SupplyPOLineExt : PXCacheExtension<SupplyPOLine>
    {
        #region UsrSONbr
        [PXDBString(15, IsUnicode = true, InputMask = ">CCCCCCCCCCCCCCC", BqlField = typeof(POLineExt.usrSONbr))]
        [PXUIField(DisplayName = "SO Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual string UsrSONbr { get; set; }
        public abstract class usrSONbr : PX.Data.BQL.BqlString.Field<usrSONbr> { }
        #endregion

        #region UsrSOType
        [PXDBString(2, IsFixed = true, InputMask = ">aa", BqlField = typeof(POLineExt.usrSOType))]
        [PXUIField(DisplayName = "SO Type", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual string UsrSOType { get; set; }
        public abstract class usrSOType : PX.Data.BQL.BqlString.Field<usrSOType> { }
        #endregion

        #region UsrSOLineNbr
        [PXDBInt(BqlField = typeof(POLineExt.usrSOLineNbr))]
        [PXUIField(DisplayName = "SO Line Nbr.", Visibility = PXUIVisibility.SelectorVisible, Visible = false, IsReadOnly = true)]
        public virtual int? UsrSOLineNbr { get; set; }
        public abstract class usrSOLineNbr : PX.Data.BQL.BqlInt.Field<usrSOLineNbr> { }
        #endregion
    }
}
