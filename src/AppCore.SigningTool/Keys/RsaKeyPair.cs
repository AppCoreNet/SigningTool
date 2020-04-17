using System.Reflection;

namespace AppCore.SigningTool.StrongName
{
    public class RsaKeyPair : IKeyPair
    {
        public RsaPrivateKey PrivateKey { get; }

        IPrivateKey IKeyPair.PrivateKey => PrivateKey;

        public RsaPublicKey PublicKey { get; }

        IPublicKey IKeyPair.PublicKey => PublicKey;

        internal RsaKeyPair(
            byte[] publicExponent,
            byte[] modulus,
            byte[] prime1,
            byte[] prime2,
            byte[] exponent1,
            byte[] exponent2,
            byte[] coefficient,
            byte[] privateExponent)
        {
            PrivateKey = new RsaPrivateKey(
                publicExponent,
                modulus,
                prime1,
                prime2,
                exponent1,
                exponent2,
                coefficient,
                privateExponent);

            PublicKey = new RsaPublicKey(modulus, publicExponent, AssemblyHashAlgorithm.Sha1);
        }

        internal RsaKeyPair(RsaPrivateKey privateKey, RsaPublicKey publicKey)
        {
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }
    }
}