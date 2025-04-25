using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CM;
using static Opayo.DAC.CustomerExt;

namespace Opayo.DAC
{
    public class CustomerExt : PXCacheExtension<Customer>
    {
        #region UsrPaymentLink
        [PXDBString(2500, IsUnicode = true)]
        [PXUIField(DisplayName = "Payment Link")]
        public virtual string UsrPaymentLink { get; set; }
        public abstract class usrPaymentLink : PX.Data.BQL.BqlString.Field<usrPaymentLink> { }
        #endregion

        #region UsrPaymentAmount
        [PXDBDecimal(2)]
        [PXUIField(DisplayName = "Payment Amount")]
        [PXDefault(TypeCode.Decimal, "0.00", PersistingCheck = PXPersistingCheck.Nothing)]
        public virtual decimal? UsrPaymentAmount { get; set; }
        public abstract class usrPaymentAmount : PX.Data.BQL.BqlDecimal.Field<usrPaymentAmount> { }
        #endregion

        #region UsrPaymentCurrency
        [PXDBString(5)]
        [PXUIField(DisplayName = "Payment Currency")]
        [PXSelector(typeof(Currency.curyID), CacheGlobal = true)]
        public virtual string UsrPaymentCurrency { get; set; }
        public abstract class usrPaymentCurrency : PX.Data.BQL.BqlString.Field<usrPaymentCurrency> { }
        #endregion

        #region UsrPaymentDescription
        [PXDBString(100)]
        [PXUIField(DisplayName = "Payment Description")]
        public virtual string UsrPaymentDescription { get; set; }
        public abstract class usrPaymentDescription : PX.Data.BQL.BqlString.Field<usrPaymentDescription> { }
        #endregion
    }
}
