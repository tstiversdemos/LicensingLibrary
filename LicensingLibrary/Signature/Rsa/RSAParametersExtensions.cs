using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LicensingLibrary.Signature.Rsa
{
    public static class RSAParametersExtensions
    {
        public static string ToBase64EncodedJson(this RSAParameters parameters)
        {
            var json = JsonSerializer.Serialize(new SerializableRSAParameters(parameters));
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            return Convert.ToBase64String(jsonBytes);
        }

        public static RSAParameters? ToRSAParameters(this string encodedParameters)
        {
            byte[] jsonBytes;

            try
            {
                jsonBytes = Convert.FromBase64String(encodedParameters);
            }
            catch (FormatException)
            {
                return null;
            }

            var json = Encoding.UTF8.GetString(jsonBytes);

            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    IgnoreNullValues = true
                };

                return JsonSerializer.Deserialize<SerializableRSAParameters>(json, jsonOptions).ToRSAParameters();
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}