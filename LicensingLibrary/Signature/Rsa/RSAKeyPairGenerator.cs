using System.Security.Cryptography;

namespace LicensingLibrary.Signature.Rsa
{
    public sealed class RSAKeyPairGenerator : IKeyPairGenerator
    {
        public KeyPair GenerateKeyPair()
        {
            using (var rsa = RSA.Create())
            {
                rsa.KeySize = RSASignatureProvider.KeySize;

                var publicKey = rsa.ExportParameters(false).ToBase64EncodedJson();
                var privateKey = rsa.ExportParameters(true).ToBase64EncodedJson();

                return new KeyPair
                {
                    PublicKey = publicKey,
                    PrivateKey = privateKey
                };
            }
        }
    }
}