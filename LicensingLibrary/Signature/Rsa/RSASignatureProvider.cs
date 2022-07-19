using System;
using System.Security.Cryptography;

namespace LicensingLibrary.Signature.Rsa
{
    public sealed class RSASignatureProvider : ISignatureGenerator, ISignatureValidator
    {
        public static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
        public static readonly RSASignaturePadding SignaturePadding = RSASignaturePadding.Pkcs1;
        public const int KeySize = 3072;

        private readonly RSAParameters _parameters;
        private readonly bool _canSign;

        public RSASignatureProvider(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be null or whitespace.", nameof(key));

            _parameters = key.ToRSAParameters() ?? throw new ArgumentException("Invalid key.", nameof(key));
            _canSign = HasPrivateKey();

            // Throw exception immediately if the key is invalid.
            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(_parameters);
            }
        }

        public byte[] GenerateSignature(byte[] payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (payload.Length == 0) throw new ArgumentException("Payload must not be empty.", nameof(payload));
            if (!_canSign) throw new InvalidOperationException("Private key not provided.");

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(_parameters);

                return rsa.SignData(payload, HashAlgorithm, SignaturePadding);
            }
        }

        public bool ValidateSignature(byte[] payload, byte[] signature)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (signature == null) throw new ArgumentNullException(nameof(signature));
            if (payload.Length == 0) throw new ArgumentException("Payload must not be empty.", nameof(payload));
            if (signature.Length == 0) throw new ArgumentException("Signature must not be empty.", nameof(signature));

            using (var rsa = RSA.Create())
            {
                rsa.ImportParameters(_parameters);

                return rsa.VerifyData(payload, signature, HashAlgorithm, SignaturePadding);
            }
        }

        private bool HasPrivateKey()
        {
            if (_parameters.D == null) return false;
            if (_parameters.DP == null) return false;
            if (_parameters.DQ == null) return false;
            if (_parameters.InverseQ == null) return false;
            if (_parameters.P == null) return false;
            if (_parameters.Q == null) return false;

            return true;
        }
    }
}