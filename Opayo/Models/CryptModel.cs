using System;
using System.Linq;
using System.Text.RegularExpressions;
using PX.Data;
using Messages = Opayo.Tools.Messages;

namespace Opayo.Models
{
    class MaxLengthAttribute : Attribute
    {
        public int MaxLength { get; set; }
        public MaxLengthAttribute() : this(0) { }
        public MaxLengthAttribute(int len) => MaxLength = len;
    }

    class RequiredAttribute : Attribute {}

    class GreaterThanAttribute : Attribute
    {
        public int Value { get; set; }
        public GreaterThanAttribute() : this(0) { }
        public GreaterThanAttribute(int value) => Value = value;
    }

    public class CryptModel
    {
        const char separator = '&';

        #region Fields
        [MaxLength(40)][Required] public string VendorTxCode { get; set; }
        [Required][GreaterThan(0)] public string Amount { get; set; }
        [MaxLength(3)][Required] public string Currency { get; set; }
        [MaxLength(100)][Required] public string Description { get; set; }
        [MaxLength(2000)][Required] public string SuccessURL { get; set; }
        [MaxLength(2000)][Required] public string FailureURL { get; set; }
        [MaxLength(80)] public string CustomerEMail { get; set; }
        [MaxLength(255)] public string VendorEMail { get; set; }
        public string SendEMail { get; set; }

        // BILLING
        [MaxLength(20)][Required] public string BillingSurname { get; set; }
        [MaxLength(20)][Required] public string BillingFirstnames { get; set; }
        [MaxLength(50)][Required] public string BillingAddress1 { get; set; }
        [MaxLength(50)] public string BillingAddress2 { get; set; }
        [MaxLength(50)] public string BillingAddress3 { get; set; }
        [MaxLength(40)][Required] public string BillingCity { get; set; }
        [MaxLength(10)][Required] public string BillingPostCode { get; set; }
        [MaxLength(2)][Required] public string BillingCountry { get; set; }
        [MaxLength(2)] public string BillingState { get; set; }

        // DELIVERY
        [MaxLength(20)][Required] public string DeliverySurname { get; set; }
        [MaxLength(20)][Required] public string DeliveryFirstnames { get; set; }
        [MaxLength(50)][Required] public string DeliveryAddress1 { get; set; }
        [MaxLength(50)] public string DeliveryAddress2 { get; set; }
        [MaxLength(50)] public string DeliveryAddress3 { get; set; }
        [MaxLength(40)][Required] public string DeliveryCity { get; set; }
        [MaxLength(10)][Required] public string DeliveryPostCode { get; set; }
        [MaxLength(2)][Required] public string DeliveryCountry { get; set; }
        [MaxLength(2)] public string DeliveryState { get; set; }

        // OTHER
        [MaxLength(64)] public string AcctID { get; set; }
        [MaxLength(20)] public string COFUsage { get; set; }
        [MaxLength(20)] public string InitiatedType { get; set; }

        // OPAYO RESPONSE
        // Amount, VendorTxCode are returned as well
        public string VPSTxId { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public string TxAuthNo { get; set; }
        public string AVSCV2 { get; set; }
        public string AddressResult { get; set; }
        public string PostCodeResult { get; set; }
        public string CV2Result { get; set; }
        public string _3DSecureStatus { get; set; }
        public string Last4Digits { get; set; }
        public string DeclineCode { get; set; }
        public string ExpiryDate { get; set; }
        public string SchemeTraceID { get; set; }
        public string BankAuthCode { get; set; }
        #endregion

        public CryptModel()
        {
            SendEMail = "1";
            COFUsage = "FIRST";
            InitiatedType = "CIT";
        }
        public static string Serialize(CryptModel obj)
        {
            string res = "";

            foreach (var propInfo in obj.GetType().GetProperties())
            {
                string value = ((string)propInfo.GetValue(obj, null))?.Trim();
                string propName = propInfo.Name;

                // Required attr
                RequiredAttribute required = (RequiredAttribute)propInfo
                    .GetCustomAttributes(typeof(RequiredAttribute), false)
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(value))
                {
                    if (required != null)
                        throw new PXException(Messages.ModelFieldRequired, propName);
                    continue;
                }
                // Max len attr 
                MaxLengthAttribute maxLenAttribute = (MaxLengthAttribute)propInfo
                    .GetCustomAttributes(typeof(MaxLengthAttribute), false)
                    .FirstOrDefault();

                if (maxLenAttribute != null && value.Length > maxLenAttribute.MaxLength)
                    value = value.Remove(maxLenAttribute.MaxLength);

                // Not equals attr
                GreaterThanAttribute greaterThanAttribute = (GreaterThanAttribute)propInfo
                    .GetCustomAttributes(typeof(GreaterThanAttribute), false)
                    .FirstOrDefault();
                if (greaterThanAttribute != null && decimal.TryParse(value, out decimal parsed))
                {
                    if (parsed <= greaterThanAttribute.Value)
                        throw new PXException(Messages.ValueMustBeGreater, propName, greaterThanAttribute.Value);
                }

                res += $"{propName}={value}{separator}";
            }

            if (obj.BillingCountry == "US" && string.IsNullOrEmpty(obj.BillingState))
                throw new PXException(Messages.ModelFieldRequired, "BillingState");

            if (obj.DeliveryCountry == "US" && string.IsNullOrEmpty(obj.DeliveryState))
                throw new PXException(Messages.ModelFieldRequired, "DeliveryState");

            return res[res.Length - 1] == separator ? res.Remove(res.Length - 1) : res;
        }
        public static CryptModel Deserialize(string data)
        {
            CryptModel result = new CryptModel();

            var lines = data.Split(separator);
            foreach (var line in lines)
            {
                try
                {
                    var pair = line.Split('=');
                    string property = Regex.IsMatch(pair[0], @"^\d")
                        ? pair[0].Insert(0, "_")
                        : pair[0];
                    string value = pair[1];
                    typeof(CryptModel).GetProperty(property)?.SetValue(result, value);
                } 
                catch { }
            }

            return result;
        }
    }
}
