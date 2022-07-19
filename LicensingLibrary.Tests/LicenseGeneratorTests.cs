using LicensingLibrary.Signature;
using NSubstitute;
using Shouldly;
using System;
using System.Text;
using System.Text.Json;
using Xunit;

namespace LicensingLibrary.Tests
{
    public class LicenseGeneratorTests
    {
        private readonly ISignatureGenerator _signatureGenerator;

        public LicenseGeneratorTests()
        {
            _signatureGenerator = Substitute.For<ISignatureGenerator>();
        }

        [Fact]
        public void Constructor_SignatureProviderIsNull_ShouldThrowArgumentNullException()
        {
            var ex = Should.Throw<ArgumentNullException>(() => new LicenseGenerator<MockLicense>(null));

            ex.ParamName.ShouldBe("signatureProvider");
        }

        [Fact]
        public void GenerateKey_LicenseIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new LicenseGenerator<MockLicense>(_signatureGenerator);

            var ex = Should.Throw<ArgumentNullException>(() => sut.GenerateKey(null));

            ex.ParamName.ShouldBe("license");
        }

        [Fact]
        public void GenerateKey_ShouldReturnCorrectlyEncodedLicense()
        {
            var signature = new byte[] {1, 2, 3, 4, 5};
            var license = new MockLicense {MockData = "test"};
            _signatureGenerator.GenerateSignature(Arg.Any<byte[]>()).Returns(signature);
            var sut = new LicenseGenerator<MockLicense>(_signatureGenerator);
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(license));
            var expectedPayload = Convert.ToBase64String(jsonBytes);
            var expectedSignature = Convert.ToBase64String(signature);

            var licenseKey = sut.GenerateKey(license);

            licenseKey.ShouldBe($"{expectedPayload}.{expectedSignature}");
        }

        public sealed class MockLicense
        {
            public string MockData { get; set; }
        }
    }
}