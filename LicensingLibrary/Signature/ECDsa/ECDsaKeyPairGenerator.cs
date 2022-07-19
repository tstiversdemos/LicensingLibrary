using System.Security.Cryptography;

namespace LicensingLibrary.Signature.EcDsa
{
    public sealed class ECDsaKeyPairGenerator : IKeyPairGenerator
    {
        public KeyPair GenerateKeyPair()
        {
            using (var ecDsa = ECDsa.Create(ECDsaSignatureProvider.Curve))
            {
                var publicKey = ecDsa.ExportParameters(false).ToBase64EncodedJson();
                var privateKey = ecDsa.ExportParameters(true).ToBase64EncodedJson();

                return new KeyPair
                {
                    PublicKey = publicKey,
                    PrivateKey = privateKey
                };
            }
        }
    }
}