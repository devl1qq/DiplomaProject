using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;

namespace MainProject.DAC
{
    public class ARPaymentExt : PXCacheExtension<ARPayment> 
    {
        #region CustomerID
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXRemoveBaseAttribute(typeof(CustomerAttribute))]
        [Customer(typeof(Search<BAccount.bAccountID>), fields: new Type[]
        {
            typeof(Customer.acctCD),
            typeof(Customer.acctName),
            typeof(BAccount.acctReferenceNbr),
            typeof(PX.Objects.CR.Address.addressLine1), 
            typeof(PX.Objects.CR.Address.addressLine2), 
            typeof(PX.Objects.CR.Address.postalCode), 
            typeof(CustomerAttribute.Contact.phone1), 
            typeof(PX.Objects.CR.Address.city), 
            typeof(PX.Objects.CR.Address.countryID), 
            typeof(CustomerAttribute.Location.taxRegistrationID), 
            typeof(Customer.curyID),
            typeof(CustomerAttribute.Contact.attention),
            typeof(Customer.customerClassID), 
            typeof(Customer.status)
        },Visibility = PXUIVisibility.SelectorVisible, Filterable = true, TabOrder = 3)]
        public Int32? CustomerID { get; set; }
        public new abstract class customerID : PX.Data.BQL.BqlInt.Field<customerID> { }
        #endregion
    }
}
