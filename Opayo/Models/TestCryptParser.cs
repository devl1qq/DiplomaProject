using Opayo.DAC;
using static Opayo.Tools.OpayoUtils;
using System;

namespace Opayo.Models
{
    public class TestCryptParser : IAcumaticaEntityToCrypt<ArcOpayoAccountMaint, ArcOpayoAccount>
    {
        public CryptModel Parse(ArcOpayoAccountMaint graph, ArcOpayoAccount table, ArcOpayoAccount account)
        {
            var vendorTxCode = SegmentedStringFormatter.Format(new[]
            {
                new Tuple<string, int>("TEST", 4),
                new Tuple<string, int>(GenerateId(), 15)
            });

            CryptModel cryptObject = new CryptModel()
            {
                Amount = "0.01",
                Currency = "GBP",
                VendorTxCode = vendorTxCode,
                Description = "This is test transaction.",
                BillingSurname = "Seniv",
                BillingFirstnames = "Artur",
                BillingAddress1 = "15 Copthorne Way",
                BillingCity = "Cambridge",
                BillingPostCode = "GL2 6AB",
                BillingCountry = "GB",
                DeliverySurname = "Patko",
                DeliveryFirstnames = "Ihor",
                DeliveryAddress1 = "2 Southlands Road",
                DeliveryCity = "Polperro",
                DeliveryPostCode = "PL13 6EU",
                DeliveryCountry = "GB",
            };
            return cryptObject;
        }
    }
}
