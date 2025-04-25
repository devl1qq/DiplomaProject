using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opayo.Tools
{
    public class Messages
    {
        public const string TestEnvironment = "You are using Test system.";
        public const string AutoCreationError = "To enable automatic creation of payments based on processed Opayo transactions, please specify Payment Method and Cash Account.";
        public const string OpayoAccountNotSpecified = "Please specify Opayo Account on the Branches(CS102000) or Companies(CS101500) screen.";
        public const string NoRelatedEntity = "This type has no related entity.";

        // Generating link errors
        public const string WebhookInactive = "Opayo webhook is not active.";
        public const string AccountNull = "Generating Opayo link error: ArcOpayoAccount is null.";
        public const string AccountInactive = "Generating Opayo link error: Account is not active.";
        public const string CryptNull = "Generating Opayo link error: Crypt data is null.";
        public const string LinkNotGeneratedTraceError = "Error: Opayo Link not generated.\n{0}";

        public const string VPSProtocolNull = "Generating Opayo link error: VPSProtocol is null.";
        public const string TxTypeNull = "Generating Opayo link error: TxType is null.";
        public const string UrlNull = "Generating Opayo link error: URL is null.";
        public const string VendorNull = "Generating Opayo link error: Vendor is null.";
        public const string EncryptionKeyNull = "Generating Opayo link error: FI Encryption Password is null.";
        public const string WebhookUrlNull = "Generating Opayo link error: WebhookUrl is null.";
        public const string ModelFieldRequired = "Generating Opayo link error: {0} field is required.";
        public const string ValueMustBeGreater = "Generating Opayo link error: {0} field must be greater than {1}.";
        public const string UnknownEnvironment = "Generating Opayo link error: Unknown ArcOpayoAccount Environment.";

        // Payments
        public const string EncryptionKeyNullPayment = "FI Encryption Password is null.";
        public const string ArcOpayoTranNull = "ArcOpayoTran is null.";
        public const string ArcOpayoAccountNull = "ArcOpayoAccount is null.";
    }
}
