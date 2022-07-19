using System;
using System.Text;
using LicensingLibrary.Signature.EcDsa;
using Shouldly;
using Xunit;

namespace LicensingLibrary.Tests.Signature.EcDsa
{
    public class ECParametersExtensionsTests
    {
        [Fact]
        public void ToECParameters_InvalidBase64String_ShouldReturnNull()
        {
            var input = "invalidbase64string";

            var result = input.ToECParameters();

            result.ShouldBeNull();
        }

        [Fact]
        public void ToECParameters_InvalidJson_ShouldReturnNull()
        {
            var input = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalidjson"));

            var result = input.ToECParameters();

            result.ShouldBeNull();
        }
    }
}