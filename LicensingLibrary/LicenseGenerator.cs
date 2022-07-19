using LicensingLibrary.Signature;
using System;
using System.Text;
using System.Text.Json;

namespace LicensingLibrary
{
    public sealed class LicenseGenerator<TLicense> : ILicenseGenerator<TLicense>
    {
        private readonly ISignatureGenerator _signatureProvider;

        public LicenseGenerator(ISignatureGenerator signatureProvider)
        {
            _signatureProvider = signatureProvider ?? throw new ArgumentNullException(nameof(signatureProvider));
        }

        public string GenerateKey(TLicense license)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));

            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(license));
            var signatureBytes = _signatureProvider.GenerateSignature(jsonBytes);

            var encodedJson = Convert.ToBase64String(jsonBytes);
            var encodedSignature = Convert.ToBase64String(signatureBytes);

            return $"{encodedJson}.{encodedSignature}";
        }
    }
}