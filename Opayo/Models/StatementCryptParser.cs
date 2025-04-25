using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Opayo.DAC;
using Opayo.Models;
using Opayo.Tools;
using Opayo.Webhooks;
using PX.Api.Webhooks.DAC;
using PX.Api.Webhooks.Graph;
using PX.Concurrency;
using PX.Data;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.Objects.CS;
using PX.Objects.EP;
using PX.Objects.GL;
using PX.Objects.GL.DAC;

using DetailsResult = PX.Objects.AR.ARStatementPrint.DetailsResult;
using PrintParameters = PX.Objects.AR.ARStatementPrint.PrintParameters;
using Type = System.Type;
using Messages = Opayo.Tools.Messages;
using System.Linq;
using static Opayo.Tools.OpayoUtils;

namespace Opayo.Models
{
    public class StatementCryptParser : IAcumaticaEntityToCrypt<ARStatementUpdate, ARStatement>
    {
        public CryptModel Parse(ARStatementUpdate graph, ARStatement statement, ArcOpayoAccount account)
        {
            Customer customer = Customer.PK.Find(PXGraph.CreateInstance<PXGraph>(), statement.CustomerID);
            CustomerMaint customerGraph = PXGraph.CreateInstance<CustomerMaint>();
            customerGraph.BAccount.Update(customer);

            Contact deliveryContact = customerGraph.GetExtension<CustomerMaint.DefLocationExt>().DefLocationContact.Select();
            OpayoUtils.ParsedName billingFullName = OpayoUtils.ParseFullName(new[]
            {
                customerGraph.BillContact.Select().RowCast<Contact>().FirstOrDefault().Attention,
                customerGraph.BillContact.Select().RowCast<Contact>().FirstOrDefault().FullName
            });

            OpayoUtils.ParsedName deliveryFullName = OpayoUtils.ParseFullName(new[] {
                deliveryContact?.Attention,
                customerGraph.BillContact.Select().RowCast<Contact>().FirstOrDefault().Attention,
                customerGraph.BillContact.Select().RowCast<Contact>().FirstOrDefault().FullName
            });

            Address defBillAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Customer.defBillAddressID>>>>.Select(PXGraph.CreateInstance<PXGraph>(), customer.DefBillAddressID);
            Address shipAddress = PXSelect<Address, Where<Address.addressID, Equal<Required<Customer.defAddressID>>>>.Select(PXGraph.CreateInstance<PXGraph>(), customer.DefAddressID);

            var vendorTxCode = SegmentedStringFormatter.Format(new[]
            {
                new Tuple<string, int>(customer.AcctCD, 10),
                new Tuple<string, int>(statement.StatementDate?.ToString("dd-MM-yy"), 8),
                new Tuple<string, int>(GenerateId(), 15),
            });

            CryptModel cryptObject = new CryptModel()
            {
                Amount = $"{statement?.CuryEndBalance:0.00}",
                Currency = statement?.CuryID,
                Description = $"Statement as of {statement?.StatementDate}",

                CustomerEMail = customerGraph.BillContact.Current?.EMail,
                VendorEMail = account?.VendorEmail,
                VendorTxCode = vendorTxCode,

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