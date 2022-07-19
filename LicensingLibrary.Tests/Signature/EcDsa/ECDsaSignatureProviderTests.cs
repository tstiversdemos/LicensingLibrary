using System;
using System.Security.Cryptography;
using LicensingLibrary.Signature.EcDsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.EcDsa
{
    public class ECDsaSignatureProviderTests
    {
        private readonly string _privateKey;
        private readonly string _publicKey;
        private readonly byte[] _payload;
        private readonly byte[] _signature;

        public ECDsaSignatureProviderTests()
        {
            _payload = new byte[] {1, 2, 3, 4, 5};

            using (var ecDsa = ECDsa.Create(ECDsaSignatureProvider.Curve))
            {
                _publicKey = ecDsa.ExportParameters(false).ToBase64EncodedJson();
                _privateKey = ecDsa.ExportParameters(true).ToBase64EncodedJson();

                _signature = ecDsa.SignData(_payload, ECDsaSignatureProvider.HashAlgorithm);
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
            var ex = Should.Throw<ArgumentException>(() => new ECDsaSignatureProvider(key));

            ex.ParamName.ShouldBe("key");
        }

        [Fact]
        public void GenerateSignature_PayloadIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new ECDsaSignatureProvider(_privateKey);

            var ex = Should.Throw<ArgumentException>(() => sut.GenerateSignature(new byte[0]));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void GenerateSignature_PayloadIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new ECDsaSignatureProvider(_privateKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.GenerateSignature(null));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void GenerateSignature_PrivateKeyNotProvided_ShouldThrowInvalidOperationException()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            Should.Throw<InvalidOperationException>(() => sut.GenerateSignature(_payload));
        }

        [Fact]
        public void GenerateSignature_ShouldReturnCorrectSignature()
        {
            var payload = new byte[] {1, 2, 3, 4, 5};
            var sut = new ECDsaSignatureProvider(_privateKey);

            var signature = sut.GenerateSignature(payload);

            using (var ecDsa = ECDsa.Create(_publicKey.ToECParameters() ?? throw new NullReferenceException()))
            {
                ecDsa.VerifyData(payload, signature, ECDsaSignatureProvider.HashAlgorithm).ShouldBeTrue();
            }
        }

        [Fact]
        public void ValidateSignature_PayloadChanged_ShouldReturnFalse()
        {
            var payload = new byte[] {5, 4, 3, 2, 1};
            var sut = new ECDsaSignatureProvider(_publicKey);

            sut.ValidateSignature(payload, _signature).ShouldBeFalse();
        }

        [Fact]
        public void ValidateSignature_PayloadIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentException>(() => sut.ValidateSignature(new byte[0], _signature));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void ValidateSignature_PayloadIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.ValidateSignature(null, _signature));

            ex.ParamName.ShouldBe("payload");
        }

        [Fact]
        public void ValidateSignature_PublicKeyMismatch_ShouldReturnFalse()
        {
            string publicKey;
            using (var ecDsa = ECDsa.Create(ECDsaSignatureProvider.Curve))
            {
                publicKey = ecDsa.ExportParameters(false).ToBase64EncodedJson();
            }

            var sut = new ECDsaSignatureProvider(publicKey);

            sut.ValidateSignature(_payload, _signature).ShouldBeFalse();
        }

        [Fact]
        public void ValidateSignature_SignatureIsEmpty_ShouldThrowArgumentException()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentException>(() => sut.ValidateSignature(_payload, new byte[0]));

            ex.ParamName.ShouldBe("signature");
        }

        [Fact]
        public void ValidateSignature_SignatureIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            var ex = Should.Throw<ArgumentNullException>(() => sut.ValidateSignature(_signature, null));

            ex.ParamName.ShouldBe("signature");
        }

        [Fact]
        public void ValidateSignature_ValidSignature_ShouldReturnTrue()
        {
            var sut = new ECDsaSignatureProvider(_publicKey);

            sut.ValidateSignature(_payload, _signature).ShouldBeTrue();
        }
    }
}