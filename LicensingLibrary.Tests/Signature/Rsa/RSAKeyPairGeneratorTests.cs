using LicensingLibrary.Signature.Rsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.Rsa
{
    public class RSAKeyPairGeneratorTests
    {
        [Fact]
        public void GenerateKeyPair_ShouldGenerateNewKeyPairEachTime()
        {
            var sut = new RSAKeyPairGenerator();

            var keyPair1 = sut.GenerateKeyPair();
            var keyPair2 = sut.GenerateKeyPair();

            keyPair1.PublicKey.ShouldNotBe(keyPair2.PublicKey);
            keyPair1.PrivateKey.ShouldNotBe(keyPair2.PrivateKey);
        }
    }
}