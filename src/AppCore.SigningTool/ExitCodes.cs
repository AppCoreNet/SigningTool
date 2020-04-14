using System;
using System.IO;
using AppCore.SigningTool.Exceptions;
using dnlib.DotNet;

namespace AppCore.SigningTool
{
    /// <summary>
    /// Exit codes.
    /// </summary>
    internal static class ExitCodes
    {
        public const int Success = 0;

        public const int UnrecognizedCommandOrArgument = 1;

        public const int FileNotFound = 2;

        public const int InvalidKey = 3;

        public const int InvalidAssembly = 4;

        public const int FileAlreadyExists = 5;

        public const int Unknown = -1;

        public static int FromException(Exception error)
        {
            switch (error)
            {
                case FileAlreadyExistsException _:
                    return FileAlreadyExists;

                case FileNotFoundException _:
                    return FileNotFound;

                case InvalidKeyException _:
                    return InvalidKey;

                case BadImageFormatException _:
                    return InvalidAssembly;

                default:
                    return Unknown;
            }
        }
    }
}
