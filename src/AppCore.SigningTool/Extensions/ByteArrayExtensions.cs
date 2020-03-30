using System;

namespace AppCore.SigningTool.Extensions
{
    internal static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] array)
        {
            return BitConverter.ToString(array)
                               .Replace("-", "")
                               .ToLowerInvariant();
        }
    }
}