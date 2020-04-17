using System.Security.Cryptography;

namespace AppCore.SigningTool.StrongName
{
    public class RsaKeyGenerator : IKeyGenerator
    {
        public IKeyPair Generate(int? keySize = null)
        {
            using RSA rsa = keySize.HasValue
                ? RSA.Create(keySize.Value)
                : RSA.Create(1024);

            RSAParameters parameters = rsa.ExportParameters(true);

            return new RsaKeyPair(
                parameters.Exponent,
                parameters.Modulus,
                parameters.P,
                parameters.Q,
                parameters.DP,
                parameters.DQ,
                parameters.InverseQ,
                parameters.D);
        }
    }
}