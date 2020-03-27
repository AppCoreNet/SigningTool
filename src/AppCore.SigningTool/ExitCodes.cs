using System;
using System.IO;
using dnlib.DotNet;

namespace AppCore.SigningTool
{
    internal static class ExitCodes
    {
        public const int Success = 0;

        public const int FileAreadyExists = -1;

        public const int FileNotFound = -2;

        public const int InvalidKey = -3;

        public const int InvalidAssembly = -4;

        public const int Unknown = -1;

        public static int FromException(Exception error)
        {
            switch (error)
            {
                case BadImageFormatException _:
                    return InvalidAssembly;

                case FileNotFoundException _:
                    return FileNotFound;

                case InvalidKeyException _:
                    return InvalidKey;

                default:
                    return Unknown;
            }
        }
    }
}
