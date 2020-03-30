using System;
using System.IO;
using AppCore.SigningTool.Exceptions;
using dnlib.DotNet;

namespace AppCore.SigningTool
{
    /// <summary>
    /// Exit codes.
    /// https://www.febooti.com/products/automation-workshop/online-help/actions/run-dos-cmd-command/exit-codes/
    /// </summary>
    internal static class ExitCodes
    {
        public const int Success = 0;

        public const int FileAreadyExists = -1;

        public const int FileNotFound = 2;

        public const int InvalidKey = -3;

        public const int InvalidAssembly = -4;

        public const int Unknown = -1;

        public static int FromException(Exception error)
        {
            switch (error)
            {
                case BadImageFormatException _:
                    return InvalidAssembly;

                case FileAreadyExistsException _:
                    return FileAreadyExists;

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
