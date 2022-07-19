using System.Security.Cryptography;

namespace LicensingLibrary.Signature.EcDsa
{
    public class SerializableECParameters
    {
        private ECParameters _parameters;

        public byte[] D
        {
            get => _parameters.D;
            set => _parameters.D = value;
        }

        public SerializableECPoint Q
        {
            get => new SerializableECPoint(_parameters.Q);
            set => _parameters.Q = value.ToECPoint();
        }

        public SerializableECParameters()
        {
            _parameters = new ECParameters
            {
                Curve = ECDsaSignatureProvider.Curve
            };
        }

        public SerializableECParameters(ECParameters parameters)
        {
            _parameters = parameters;
        }

        public ECParameters ToECParameters()
        {
            return _parameters;
        }

        public class SerializableECPoint
        {
            private ECPoint _point;

            public byte[] X
            {
                get => _point.X;
                set => _point.X = value;
            }

            public byte[] Y
            {
                get => _point.Y;
                set => _point.Y = value;
            }

            public SerializableECPoint()
            {
                _point = new ECPoint();
            }

            public SerializableECPoint(ECPoint point)
            {
                _point = point;
            }

            public ECPoint ToECPoint()
            {
                return _point;
            }
        }
    }
}