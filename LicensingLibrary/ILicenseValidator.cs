namespace LicensingLibrary
{
    public interface ILicenseValidator<TLicense>
    {
        void AddValidationRoutine(IValidationRoutine<TLicense> routine);
        bool ValidateLicense(string licenseKey);
    }
}