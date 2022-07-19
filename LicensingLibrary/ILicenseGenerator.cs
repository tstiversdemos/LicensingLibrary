namespace LicensingLibrary
{
    public interface ILicenseGenerator<TLicense>
    {
        string GenerateKey(TLicense license);
    }
}