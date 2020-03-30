using System;
using System.Security.Cryptography;
using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public static class StrongNamePublicKeyExtensions
    {
        public static byte[] CreatePublicKeyToken(this StrongNamePublicKey key)
        {
            byte[] publicKey = key.CreatePublicKey();

            string hashAlg;
            switch (key.HashAlgorithm)
            {
                case AssemblyHashAlgorithm.SHA1:
                    hashAlg = HashAlgorithmName.SHA1.Name;
                    break;
                case AssemblyHashAlgorithm.SHA_256:
                    hashAlg = HashAlgorithmName.SHA256.Name;
                    break;
                case AssemblyHashAlgorithm.SHA_384:
                    hashAlg = HashAlgorithmName.SHA384.Name;
                    break;
                case AssemblyHashAlgorithm.SHA_512:
                    hashAlg = HashAlgorithmName.SHA512.Name;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var ha = HashAlgorithm.Create("SHA1");
            byte[] hash = ha.ComputeHash(publicKey);
            var keyToken = new byte[8];
            Buffer.BlockCopy(hash, (hash.Length - 8), keyToken, 0, 8);
            Array.Reverse(keyToken, 0, 8);
            return keyToken;
        }
    }
}
