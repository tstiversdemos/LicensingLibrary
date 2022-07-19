using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LicensingLibrary.Signature.EcDsa
{
    public static class ECParametersExtensions
    {
        public static string ToBase64EncodedJson(this ECParameters ecParameters)
        {
            var json = JsonSerializer.Serialize(new SerializableECParameters(ecParameters));
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            return Convert.ToBase64String(jsonBytes);
        }

        public static ECParameters? ToECParameters(this string encodedParameters)
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

                return JsonSerializer.Deserialize<SerializableECParameters>(json, jsonOptions).ToECParameters();
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}