using System;
using System.Security.Cryptography;
using LicensingLibrary.Signature.Rsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.Rsa
{
    public class RSASignatureProviderTests
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly byte[] _payload;
        private readonly byte[] _signature;

        public RSASignatureProviderTests()
        {
            _payload = new byte[] {1, 2, 3, 4, 5};

            using (var rsa = RSA.Create(RSASignatureProvider.KeySize))
            {
                _publicKey = rsa.ExportParameters(false).ToBase64EncodedJson();
                _privateKey = rsa.ExportParameters(true).ToBase64EncodedJson();

                _signature = rsa.SignData(
                    _payload,
                    RSASignatureProvider.HashAlgorithm,
                    RSASignatureProvider.SignaturePadding);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r\n\r\n")]
        public void Constructor_KeyIsNullOrWhiteSpace_ShouldThrowArgumentException(string key)
        {
            var ex = Should.Throw<ArgumentException>(() => new RSASignatureProvider(key));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void GenerateSignature_PayloadIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new RSASignatureProvider(_privateKey);

            var ex = Should.Throw<ArgumentException>(() => sut.GenerateSignature(new byte[0]));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void GenerateSignature_PayloadIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new RSASignatureProvider(_privateKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.GenerateSignature(null));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void GenerateSignature_PrivateKeyNotProvided_ShouldThrowInvalidOperationException()
        {
            var sut = new RSASignatureProvider(_publicKey);

            Should.Throw<InvalidOperationException>(() => sut.GenerateSignature(_payload));
        }

        [Fact]
        public void GenerateSignature_ShouldReturnCorrectSignature()
        {
            var payload = new byte[] {1, 2, 3, 4, 5};
            var sut = new RSASignatureProvider(_privateKey);

            var signature = sut.GenerateSignature(payload);

            using (var ecDsa = RSA.Create(_publicKey.ToRSAParameters() ?? throw new NullReferenceException()))
            {
                ecDsa.VerifyData(
                    payload,
                    signature,
                    RSASignatureProvider.HashAlgorithm,
                    RSASignatureProvider.SignaturePadding)
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void ValidateSignature_PayloadChanged_ShouldReturnFalse()
        {
            var payload = new byte[] {5, 4, 3, 2, 1};
            var sut = new RSASignatureProvider(_publicKey);

            sut.ValidateSignature(payload, _signature).ShouldBeFalse();
        }

        [Fact]
        public void ValidateSignature_PayloadIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new RSASignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentException>(() => sut.ValidateSignature(new byte[0], _signature));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void ValidateSignature_PayloadIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new RSASignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.ValidateSignature(null, _signature));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void ValidateSignature_PublicKeyMismatch_ShouldReturnFalse()
        {
            string publicKey;
            using (var ecDsa = RSA.Create(RSASignatureProvider.KeySize))
            {
                publicKey = ecDsa.ExportParameters(false).ToBase64EncodedJson();
            }

            var sut = new RSASignatureProvider(publicKey);

            sut.ValidateSignature(_payload, _signature).ShouldBeFalse();
        }

        [Fact]
        public void ValidateSignature_SignatureIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new RSASignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentException>(() => sut.ValidateSignature(_payload, new byte[0]));

            ex.ParamName.ShouldBe("signature");
        }

        [Fact]
        public void ValidateSignature_SignatureIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new RSASignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.ValidateSignature(_signature, null));

            ex.ParamName.ShouldBe("signature");
        }

        [Fact]
        public void ValidateSignature_ValidSignature_ShouldReturnTrue()
        {
            var sut = new RSASignatureProvider(_publicKey);

            sut.ValidateSignature(_payload, _signature).ShouldBeTrue();
        }
    }
}