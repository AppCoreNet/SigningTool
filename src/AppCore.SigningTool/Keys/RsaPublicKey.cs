using System.Reflection;

namespace AppCore.SigningTool.StrongName
{
    public class RsaPublicKey : IPublicKey
    {
        public byte[] Modulus { get; }

        public byte[] PublicExponent { get; }

        public AssemblyHashAlgorithm HashAlgorithm { get; }

        internal RsaPublicKey(byte[] modulus, byte[] publicExponent, AssemblyHashAlgorithm hashAlgorithm)
        {
            Modulus = modulus;
            PublicExponent = publicExponent;
            HashAlgorithm = hashAlgorithm;
        }

        public IPublicKey WithHashAlgorithm(AssemblyHashAlgorithm hashAlgorithm)
        {
            return new RsaPublicKey(Modulus, PublicExponent, hashAlgorithm);
        }
    }
}