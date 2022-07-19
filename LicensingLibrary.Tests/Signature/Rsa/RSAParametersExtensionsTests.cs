using System;
using System.Text;
using LicensingLibrary.Signature.Rsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.Rsa
{
    public class RSAParametersExtensionsTests
    {
        [Fact]
        public void ToRSAParameters_InvalidBase64String_ShouldReturnNull()
        {
            var input = "invalidbase64string";

            var result = input.ToRSAParameters();

            result.ShouldBeNull();
        }

        [Fact]
        public void ToRSAParameters_InvalidJson_ShouldReturnNull()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalidjson"));

            var result = input.ToRSAParameters();

            result.ShouldBeNull();
        }
    }
}