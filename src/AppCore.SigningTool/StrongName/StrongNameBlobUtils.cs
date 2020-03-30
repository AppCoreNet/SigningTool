using System;
using System.IO;
using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    internal static class StrongNameBlobUtils
    {
        public static byte[] ReadBytesReverse(this BinaryReader reader, int len)
        {
            byte[] data = reader.ReadBytes(len);
            if (data.Length != len)
                throw new InvalidKeyException("Can't read more bytes");

            Array.Reverse(data);
            return data;
        }

        public static void WriteReverse(this BinaryWriter writer, byte[] data)
        {
            byte[] d = (byte[])data.Clone();
            Array.Reverse(d);
            writer.Write(d);
        }
    }
}