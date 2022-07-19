namespace LicensingLibrary.Signature
{
    public interface ISignatureGenerator
    {
        byte[] GenerateSignature(byte[] payload);
    }
}