using LicensingLibrary.Signature;
using NSubstitute;
using Shouldly;
using System;
using System.Text;
using System.Text.Json;
using Xunit;

namespace LicensingLibrary.Tests
{
    public class LicenseValidatorTests
    {
        private readonly ISignatureValidator _signatureValidator;

        public LicenseValidatorTests()
        {
            _signatureValidator = Substitute.For<ISignatureValidator>();
            _signatureValidator.ValidateSignature(Arg.Any<byte[]>(), Arg.Any<byte[]>()).Returns(true);
        }

        [Fact]
        public void Constructor_SignatureProviderIsNull_ShouldThrowArgumentNullException()
        {
            var ex = Should.Throw<ArgumentNullException>(() => new LicenseValidator<MockLicense>(null));

            ex.ParamName.ShouldBe("signatureProvider");
        }

        [Fact]
        public void AddValidationRoutine_RoutineIsNull_ShouldThrowArgumentNullException()
        {
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);

            var ex = Should.Throw<ArgumentNullException>(() => sut.AddValidationRoutine(null));

            ex.ParamName.ShouldBe("routine");
        }

        [Fact]
        public void ValidateLicense_TooFewParts_ShouldReturnFalse()
        {
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);
            var key = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}";

            var result = sut.ValidateLicense(key);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateLicense_TooManyParts_ShouldReturnFalse()
        {
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);
            var key = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}." +
                      $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}." +
                      $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}";

            var result = sut.ValidateLicense(key);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateLicense_InvalidSignature_ShouldReturnFalse()
        {
            _signatureValidator.ValidateSignature(Arg.Any<byte[]>(), Arg.Any<byte[]>()).Returns(false);
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);
            var key = $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}." +
                      $"{Convert.ToBase64String(Encoding.UTF8.GetBytes("test"))}";

            var result = sut.ValidateLicense(key);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateLicense_ValidationRoutineFailed_ShouldReturnFalse()
        {
            var license = new MockLicense {MockData = "test"};
            var licenseJson = JsonSerializer.Serialize(license);
            var keyPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(licenseJson));
            var keySignature = Convert.ToBase64String(Encoding.UTF8.GetBytes("signature"));
            var key = $"{keyPayload}.{keySignature}";
            var validationRoutine = Substitute.For<IValidationRoutine<MockLicense>>();
            validationRoutine.Validate(Arg.Any<MockLicense>()).Returns(false);
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);
            sut.AddValidationRoutine(validationRoutine);

            var result = sut.ValidateLicense(key);

            result.ShouldBeFalse();
        }

        [Fact]
        public void ValidateLicense_ValidLicense_ShouldReturnTrue()
        {
            var license = new MockLicense {MockData = "test"};
            var licenseJson = JsonSerializer.Serialize(license);
            var keyPayload = Convert.ToBase64String(Encoding.UTF8.GetBytes(licenseJson));
            var keySignature = Convert.ToBase64String(Encoding.UTF8.GetBytes("signature"));
            var key = $"{keyPayload}.{keySignature}";
            var validationRoutine = Substitute.For<IValidationRoutine<MockLicense>>();
            validationRoutine.Validate(Arg.Any<MockLicense>()).Returns(true);
            var sut = new LicenseValidator<MockLicense>(_signatureValidator);
            sut.AddValidationRoutine(validationRoutine);

            var result = sut.ValidateLicense(key);

            result.ShouldBeTrue();
        }

        public sealed class MockLicense
        {
            public string MockData { get; set; }
        }
    }
}