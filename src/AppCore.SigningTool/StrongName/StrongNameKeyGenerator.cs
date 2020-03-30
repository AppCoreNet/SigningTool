using System.IO;
using System.Security.Cryptography;
using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameKeyGenerator : IStrongNameKeyGenerator
    {
        public StrongNameKey Generate(int? keySize = null)
        {
            RSA rsa = keySize.HasValue
                ? RSA.Create(keySize.Value)
                : RSA.Create(1024);

            byte[] strongName = GetStrongNameBlob(rsa);
            return new StrongNameKey(strongName);
        }

        private static byte[] GetStrongNameBlob(RSA rsa)
        {
            const uint RSA2_SIG = 0x32415352;
            RSAParameters parameters = rsa.ExportParameters(true);
            var outStream = new MemoryStream();
            var writer = new BinaryWriter(outStream);
            writer.Write((byte)7);          // bType (public/private key)
            writer.Write((byte)2);          // bVersion
            writer.Write((ushort)0);        // reserved
            writer.Write((uint)SignatureAlgorithm.CALG_RSA_SIGN);   // aiKeyAlg
            writer.Write(RSA2_SIG);         // magic (RSA2)
            writer.Write(parameters.Modulus.Length * 8);   // bitlen
            writer.WriteReverse(parameters.Exponent);
            writer.Write(new byte[4 - parameters.Exponent.Length]); // pad to DWORD
            writer.WriteReverse(parameters.Modulus);
            writer.WriteReverse(parameters.P);
            writer.WriteReverse(parameters.Q);
            writer.WriteReverse(parameters.DP);
            writer.WriteReverse(parameters.DQ);
            writer.WriteReverse(parameters.InverseQ);
            writer.WriteReverse(parameters.D);
            return outStream.ToArray();
        }
    }
}