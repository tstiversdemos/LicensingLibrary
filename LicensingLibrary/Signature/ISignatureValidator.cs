namespace LicensingLibrary.Signature
{
    public interface ISignatureValidator
    {
        bool ValidateSignature(byte[] payload, byte[] signature);
    }
}