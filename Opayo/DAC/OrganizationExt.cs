using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.BQL;
using PX.Objects.GL.DAC;

namespace Opayo.DAC
{
    public class OrganizationExt : PXCacheExtension<Organization>
    {
        #region UsrOpayoAccountID
        [PXDBInt]
        [PXUIField(DisplayName = "Opayo Account")]
        [PXSelector(typeof(Search<ArcOpayoAccount.accountID, Where<ArcOpayoAccount.active, Equal<True>>>), 
            typeof(ArcOpayoAccount.accountCD),
            typeof(ArcOpayoAccount.enableSalesOrderPaymentLinks),
            typeof(ArcOpayoAccount.enableInvoicePaymentLinks),
            typeof(ArcOpayoAccount.enableCustomerPaymentLinks),
            typeof(ArcOpayoAccount.description),
            typeof(ArcOpayoAccount.vPSProtocol),
            typeof(ArcOpayoAccount.txType),
            typeof(ArcOpayoAccount.vendorEmail),
            typeof(ArcOpayoAccount.paymentMethod),
            typeof(ArcOpayoAccount.cashAccount),
            typeof(ArcOpayoAccount.autoCreatePayments),
            SubstituteKey = typeof(ArcOpayoAccount.accountCD))]
        public virtual int? UsrOpayoAccountID { get; set; }
        public abstract class usrOpayoAccountID : BqlInt.Field<usrOpayoAccountID> { }
        #endregion
    }
}
