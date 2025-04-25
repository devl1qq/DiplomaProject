using PX.Objects.AR;
using Opayo.DAC;
using Opayo.Tools;
using PX.Data;
using PX.Objects.CR;
using static Opayo.Tools.OpayoUtils;
using System;

namespace Opayo.Models
{
    public class CustomerCryptParser : IAcumaticaEntityToCrypt<CustomerMaint, Customer>
    {
        public CryptModel Parse(CustomerMaint graph, Customer customer, ArcOpayoAccount account)
        {
            CustomerExt customerExt = customer.GetExtension<CustomerExt>();
            Address defBillAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Customer.defBillAddressID>>>>.Select(graph, customer.DefBillAddressID);
            Address shipAddress = graph.BillAddress.Current;

            bool shippingAddressInvalid = string.IsNullOrEmpty(shipAddress?.AddressLine1) ||
                                          string.IsNullOrEmpty(shipAddress?.City) ||
                                          string.IsNullOrEmpty(shipAddress?.CountryID) ||
                                          string.IsNullOrEmpty(shipAddress?.PostalCode);

            if (shippingAddressInvalid)
                shipAddress = defBillAddress;

            Contact deliveryContact = graph.GetExtension<CustomerMaint.DefLocationExt>().DefLocationContact.Current;
            OpayoUtils.ParsedName billingFullName = OpayoUtils.ParseFullName(new []
            {
                graph.BillContact.Current?.Attention, 
                graph.BillContact.Current?.FullName
            });
            OpayoUtils.ParsedName deliveryFullName = OpayoUtils.ParseFullName(new[] {
                deliveryContact?.Attention ,
                graph.BillContact.Current?.Attention,
                graph.BillContact.Current?.FullName

            });

            var vendorTxCode = SegmentedStringFormatter.Format(new[]
            {
                new Tuple<string, int>("SU", 2),
                new Tuple<string, int>(customer.AcctCD, 21),
                new Tuple<string, int>(GenerateId(), 15), 
            });

            CryptModel cryptObject = new CryptModel()
            {
                Amount = $"{customerExt?.UsrPaymentAmount:0.00}",
                Currency = customerExt?.UsrPaymentCurrency,
                Description = customerExt?.UsrPaymentDescription,
                VendorTxCode = vendorTxCode,

                CustomerEMail = graph.BillContact.Current?.EMail,
                VendorEMail = account.VendorEmail,

                BillingSurname = billingFullName.LastName,
                BillingFirstnames = billingFullName.FirstName,
                BillingAddress1 = defBillAddress?.AddressLine1,
                BillingAddress2 = defBillAddress?.AddressLine2,
                BillingAddress3 = defBillAddress?.AddressLine3,
                BillingCity = defBillAddress?.City,
                BillingPostCode = defBillAddress?.PostalCode,
                BillingCountry = defBillAddress?.CountryID,
                BillingState = defBillAddress?.State,

                DeliverySurname = deliveryFullName.LastName,
                DeliveryFirstnames = deliveryFullName.FirstName,
                DeliveryAddress1 = shipAddress?.AddressLine1,
                DeliveryAddress2 = shipAddress?.AddressLine2,
                DeliveryAddress3 = shipAddress?.AddressLine3,
                DeliveryCity = shipAddress?.City,
                DeliveryPostCode = shipAddress?.PostalCode,
                DeliveryCountry = shipAddress?.CountryID,
                DeliveryState = shipAddress?.State,

                AcctID = customer?.AcctCD
            };

            return cryptObject;
        }
    }
}
