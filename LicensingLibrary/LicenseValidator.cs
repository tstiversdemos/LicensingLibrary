using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using LicensingLibrary.Signature;

namespace LicensingLibrary
{
    public sealed class LicenseValidator<TLicense> : ILicenseValidator<TLicense>
    {
        private readonly ISignatureValidator _signatureProvider;
        private readonly List<IValidationRoutine<TLicense>> _validationRoutines;

        public LicenseValidator(ISignatureValidator signatureProvider)
        {
            _signatureProvider = signatureProvider ?? throw new ArgumentNullException(nameof(signatureProvider));
            _validationRoutines = new List<IValidationRoutine<TLicense>>();
        }

        public void AddValidationRoutine(IValidationRoutine<TLicense> routine)
        {
            if (routine == null) throw new ArgumentNullException(nameof(routine));

            _validationRoutines.Add(routine);
        }

        public bool ValidateLicense(string licenseKey)
        {
            var parts = licenseKey.Split('.');
            if (parts.Length != 2) return false;

            var encodedJson = parts[0];
            var encodedSignature = parts[1];

            var jsonBytes = Convert.FromBase64String(encodedJson);
            var signatureBytes = Convert.FromBase64String(encodedSignature);

            if (!_signatureProvider.ValidateSignature(jsonBytes, signatureBytes)) return false;

            var json = Encoding.UTF8.GetString(jsonBytes);
            var license = JsonSerializer.Deserialize<TLicense>(json);

            return PerformValidationRoutines(license);
        }

        private bool PerformValidationRoutines(TLicense license)
        {
            foreach (var routine in _validationRoutines)
            {
                if (!routine.Validate(license)) return false;
            }

            return true;
        }
    }
}