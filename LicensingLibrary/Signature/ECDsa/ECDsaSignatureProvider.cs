using System;
using System.Security.Cryptography;

namespace LicensingLibrary.Signature.EcDsa
{
    public sealed class ECDsaSignatureProvider : ISignatureGenerator, ISignatureValidator
    {
        public static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
        public static readonly ECCurve Curve = ECCurve.NamedCurves.brainpoolP512t1;

        private readonly ECParameters _parameters;
        private readonly bool _canSign;


        public ECDsaSignatureProvider(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key must not be null or whitespace.", nameof(key));

            _parameters = key.ToECParameters() ?? throw new ArgumentException("Invalid key.", nameof(key));
            _canSign = _parameters.D != null;

            // Throw exception immediately if the key is invalid.
            // Use ImportParameters instead of passing parameters to Create to
            // prevent this block from being optimized out in release mode.
            using (var ecDsa = ECDsa.Create(Curve))
            {
                ecDsa.ImportParameters(_parameters);
            }
        }

        public byte[] GenerateSignature(byte[] payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (payload.Length == 0) throw new ArgumentException("Payload must not be empty.", nameof(payload));
            if (!_canSign) throw new InvalidOperationException("Signing is not a valid operation for provided key.");

            using (var ecDsa = ECDsa.Create(_parameters))
            {
                return ecDsa.SignData(payload, HashAlgorithm);
            }
        }

        public bool ValidateSignature(byte[] payload, byte[] signature)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            if (signature == null) throw new ArgumentNullException(nameof(signature));
            if (payload.Length == 0) throw new ArgumentException("Payload must not be empty.", nameof(payload));
            if (signature.Length == 0) throw new ArgumentException("Signature must not be empty.", nameof(signature));

            using (var ecDsa = ECDsa.Create(_parameters))
            {
                return ecDsa.VerifyData(payload, signature, HashAlgorithm);
            }
        }
    }
}