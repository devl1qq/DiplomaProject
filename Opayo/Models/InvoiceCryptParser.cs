using PX.Objects.AR;
using Opayo.DAC;
using Opayo.Tools;
using static Opayo.Tools.OpayoUtils;
using System;
using PX.Api;
using PX.Data;
using PX.Objects.SO;
using System.Linq;

namespace Opayo.Models
{
    public class InvoiceCryptParser : IAcumaticaEntityToCrypt<ARInvoiceEntry, ARInvoice>
    {
        public CryptModel Parse(ARInvoiceEntry graph, ARInvoice invoice, ArcOpayoAccount account)
        {

            ARContact billingContact = graph.Billing_Contact.Select().RowCast<ARContact>().FirstOrDefault();
            ARShippingContact shippingContact = graph.Shipping_Contact.Select().RowCast<ARShippingContact>().FirstOrDefault();

            ARAddress billingAddress = graph.Billing_Address.Select().RowCast<ARAddress>().FirstOrDefault();
            ARAddress shippingAddress = graph.Shipping_Address.Select().RowCast<ARShippingAddress>().FirstOrDefault();

            OpayoUtils.ParsedName billingFullName = OpayoUtils.ParseFullName(new []
            {
                billingContact?.Attention,
                billingContact?.FullName
            });
            OpayoUtils.ParsedName deliveryFullName = OpayoUtils.ParseFullName(new []{
                shippingContact?.Attention,
                billingContact?.Attention,
                billingContact?.FullName

            });

            bool shippingAddressInvalid = string.IsNullOrEmpty(shippingAddress?.AddressLine1) ||
                                          string.IsNullOrEmpty(shippingAddress?.City) ||
                                          string.IsNullOrEmpty(shippingAddress?.CountryID) ||
                                          string.IsNullOrEmpty(shippingAddress?.PostalCode);

            if (shippingAddressInvalid)
                shippingAddress = billingAddress;

            var vendorTxCode = SegmentedStringFormatter.Format(new[]
            {
                new Tuple<string, int>("AR", 2),
                new Tuple<string, int>(invoice.RefNbr, 15),
                new Tuple<string, int>(GenerateId(), 15),
            });

            CryptModel cryptObject = new CryptModel()
            {
                Amount = $"{invoice.CuryDocBal:0.00}",
                Currency = invoice.CuryID,
                Description = invoice.RefNbr,
                VendorTxCode = vendorTxCode,

                CustomerEMail = graph.Billing_Contact.Current?.Email,
                VendorEMail = account.VendorEmail,

                BillingSurname = billingFullName.LastName,
                BillingFirstnames = billingFullName.FirstName,
                BillingAddress1 = billingAddress?.AddressLine1,
                BillingAddress2 = billingAddress?.AddressLine2,
                BillingAddress3 = billingAddress?.AddressLine3,
                BillingCity = billingAddress?.City,
                BillingPostCode = billingAddress?.PostalCode,
                BillingCountry = billingAddress?.CountryID,
                BillingState = billingAddress?.State,

                DeliverySurname = deliveryFullName.LastName,
                DeliveryFirstnames = deliveryFullName.FirstName,
                DeliveryAddress1 = shippingAddress?.AddressLine1,
                DeliveryAddress2 = shippingAddress?.AddressLine2,
                DeliveryAddress3 = shippingAddress?.AddressLine3,
                DeliveryCity = shippingAddress?.City,
                DeliveryPostCode = shippingAddress?.PostalCode,
                DeliveryCountry = shippingAddress?.CountryID,
                DeliveryState = shippingAddress?.State,
                    
                AcctID = graph.customer.Current.AcctCD
            };

            return cryptObject;
        }
    }
}
