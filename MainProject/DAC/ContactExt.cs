using PX.Data;
using PX.Objects.CR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainProject.DAC
{
    public class ContactExt : PXCacheExtension<Contact>
    {
        #region UsrFinanceEmail
        [PXDBString(255, IsUnicode = true, InputMask = "")]
        [PXUIField(DisplayName = "Finance Email")]
        public virtual string UsrFinanceEmail { get; set; }
        public abstract class usrFinanceEmail : PX.Data.BQL.BqlString.Field<usrFinanceEmail> { }
        #endregion
    }
}
