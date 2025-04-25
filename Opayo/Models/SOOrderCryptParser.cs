using Opayo.DAC;
using Opayo.Tools;
using PX.Objects.SO;
using static Opayo.Tools.OpayoUtils;
using System;
using PX.Objects.AR;
using static PX.Objects.SO.SOOrder;
using static PX.Objects.AR.ARInvoice;
using System.Linq;
using PX.Data;

namespace Opayo.Models
{
    public class SOOrderCryptParser : IAcumaticaEntityToCrypt<SOOrderEntry, SOOrder>
    {
        public CryptModel Parse(SOOrderEntry graph, SOOrder table, ArcOpayoAccount account)
        {
            SOBillingContact billingContact = graph.Billing_Contact.Select().RowCast<SOBillingContact>().FirstOrDefault();
            SOShippingContact shippingContact = graph.Shipping_Contact.Select().RowCast<SOShippingContact>().FirstOrDefault();

            SOAddress billingAddress = graph.Billing_Address.Select().RowCast<SOBillingAddress>().FirstOrDefault();
            SOAddress shippingAddress = graph.Shipping_Address.Select().RowCast<SOShippingAddress>().FirstOrDefault();

            OpayoUtils.ParsedName billingFullName = OpayoUtils.ParseFullName(new []
            {
                billingContact?.Attention,
                billingContact?.FullName
            });
            OpayoUtils.ParsedName deliveryFullName = OpayoUtils.ParseFullName(new[]
            {
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
                new Tuple<string, int>("SO", 2),
                new Tuple<string, int>(table.OrderNbr, 15),
                new Tuple<string, int>(GenerateId(), 15),
            });

            decimal amount = table.CuryPrepaymentReqAmt > 0 && table.PrepaymentReqSatisfied != true
                ? table.CuryPrepaymentReqAmt.GetValueOrDefault()
                : table.CuryUnpaidBalance.GetValueOrDefault();

            CryptModel cryptObject = new CryptModel()
            {
                Amount = $"{amount:0.00}",
                Currency = table.CuryID,
                Description = table.OrderNbr,
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
                DeliveryPostCode =  shippingAddress?.PostalCode,
                DeliveryCountry =  shippingAddress?.CountryID,
                DeliveryState = shippingAddress?.State ,

                AcctID = graph.customer.Current.AcctCD
            };

            return cryptObject;
        }
    }
}
