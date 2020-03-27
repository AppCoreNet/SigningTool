using System;
using System.IO;
using System.Security.Cryptography;
using dnlib.DotNet;

namespace AppCore.SigningTool.StrongNaming
{
    public class KeyGenerator
    {
        public StrongNameKey Generate(int? keySize)
        {
            RSA rsa = keySize.HasValue
                ? RSA.Create(keySize.Value)
                : RSA.Create(1024);

            var strongName = CreateStrongName(rsa.ExportParameters(true));
            return new StrongNameKey(strongName);
        }

        public byte[] CreateStrongName(RSAParameters parameters)
        {
            const uint RSA2_SIG = 0x32415352;

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

    static class StrongNameUtils
    {
        public static byte[] ReadBytesReverse(this BinaryReader reader, int len)
        {
            var data = reader.ReadBytes(len);
            if (data.Length != len)
                throw new InvalidKeyException("Can't read more bytes");
            Array.Reverse(data);
            return data;
        }

        public static void WriteReverse(this BinaryWriter writer, byte[] data)
        {
            var d = (byte[])data.Clone();
            Array.Reverse(d);
            writer.Write(d);
        }
    }
}