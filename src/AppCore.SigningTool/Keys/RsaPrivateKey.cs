namespace AppCore.SigningTool.Keys
{
    public class RsaPrivateKey: IPrivateKey
    {
        public byte[] PublicExponent { get; }

        public byte[] Modulus { get; }

        public byte[] Prime1 { get; }

        public byte[] Prime2 { get; }

        public byte[] Exponent1 { get; }

        public byte[] Exponent2 { get; }

        public byte[] Coefficient { get; }

        public byte[] PrivateExponent { get; }

        internal RsaPrivateKey(
            byte[] publicExponent,
            byte[] modulus,
            byte[] prime1,
            byte[] prime2,
            byte[] exponent1,
            byte[] exponent2,
            byte[] coefficient,
            byte[] privateExponent)
        {
            PublicExponent = publicExponent;
            Modulus = modulus;
            Prime1 = prime1;
            Prime2 = prime2;
            Exponent1 = exponent1;
            Exponent2 = exponent2;
            Coefficient = coefficient;
            PrivateExponent = privateExponent;
        }
    }
}