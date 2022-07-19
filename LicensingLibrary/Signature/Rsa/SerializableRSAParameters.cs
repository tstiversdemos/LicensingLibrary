using System.Security.Cryptography;

namespace LicensingLibrary.Signature.Rsa
{
    public class SerializableRSAParameters
    {
        private RSAParameters _parameters;

        public byte[] D
        {
            get => _parameters.D;
            set => _parameters.D = value;
        }

        public byte[] DP
        {
            get => _parameters.DP;
            set => _parameters.DP = value;
        }

        public byte[] DQ
        {
            get => _parameters.DQ;
            set => _parameters.DQ = value;
        }

        public byte[] Exponent
        {
            get => _parameters.Exponent;
            set => _parameters.Exponent = value;
        }

        public byte[] InverseQ
        {
            get => _parameters.InverseQ;
            set => _parameters.InverseQ = value;
        }

        public byte[] Modulus
        {
            get => _parameters.Modulus;
            set => _parameters.Modulus = value;
        }

        public byte[] P
        {
            get => _parameters.P;
            set => _parameters.P = value;
        }

        public byte[] Q
        {
            get => _parameters.Q;
            set => _parameters.Q = value;
        }

        public SerializableRSAParameters()
        {
            _parameters = new RSAParameters();
        }

        public SerializableRSAParameters(RSAParameters parameters)
        {
            _parameters = parameters;
        }

        public RSAParameters ToRSAParameters()
        {
            return _parameters;
        }
    }
}