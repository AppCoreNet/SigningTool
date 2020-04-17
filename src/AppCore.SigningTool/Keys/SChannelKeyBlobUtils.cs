using System;
using System.IO;
using AppCore.SigningTool.Keys;

namespace AppCore.SigningTool.StrongName
{
    internal static class SChannelKeyBlobUtils
    {
        public static byte[] ReadBytesReverse(this BinaryReader reader, int len)
        {
            byte[] data = reader.ReadBytes(len);
            if (data.Length != len)
                throw new InvalidKeyException("Couldn't read key blob.");

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