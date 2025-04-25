//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using PX.Data;
//using PX.Objects.AR;
//using static PX.Objects.AR.ARStatementPrint;

//namespace Opayo.DAC
//{
//    public class DetailsResultExt : PXCacheExtension<DetailsResult>
//    {
//        #region UsrPaymentLink
//        [PXDBString(2500, IsUnicode = true)]
//        [PXUIField(DisplayName = "Payment Link")]
//        public virtual string UsrPaymentLink { get; set; }
//        public abstract class usrPaymentLink : PX.Data.BQL.BqlString.Field<usrPaymentLink> { }
//        #endregion

//        #region UsrPaymentDescription
//        [PXDBString(100)]
//        [PXUIField(DisplayName = "Payment Description")]
//        public virtual string UsrPaymentDescription { get; set; }
//        public abstract class usrPaymentDescription : PX.Data.BQL.BqlString.Field<usrPaymentDescription> { }
//        #endregion
//    }
//}
