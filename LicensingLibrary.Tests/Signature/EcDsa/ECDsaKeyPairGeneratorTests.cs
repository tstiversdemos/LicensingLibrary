using LicensingLibrary.Signature.EcDsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.EcDsa
{
    public class ECDsaKeyPairGeneratorTests
    {
        [Fact]
        public void GenerateKeyPair_ShouldGenerateNewKeyPairEachTime()
        {
            var sut = new ECDsaKeyPairGenerator();

            var keyPair1 = sut.GenerateKeyPair();
            var keyPair2 = sut.GenerateKeyPair();

            keyPair1.PublicKey.ShouldNotBe(keyPair2.PublicKey);
            keyPair1.PrivateKey.ShouldNotBe(keyPair2.PrivateKey);
        }
    }
}