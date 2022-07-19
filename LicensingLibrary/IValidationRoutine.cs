namespace LicensingLibrary
{
    public interface IValidationRoutine<TLicense>
    {
        bool Validate(TLicense license);
    }
}