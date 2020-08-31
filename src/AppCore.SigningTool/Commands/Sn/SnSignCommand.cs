using System;
using System.IO;
using AppCore.SigningTool.Exceptions;
using AppCore.SigningTool.StrongName;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnSignCommand : ICommand<SnCommand>
    {
        private readonly IStrongNameKeyLoader _keyLoader;
        private readonly IStrongNameSigner _signer;
        private CommandOption<bool> _delaySign;
        private CommandOption<bool> _force;
        private CommandArgument _keyFile;
        private CommandArgument _assemblyFile;
        private CommandArgument _outAssemblyFile;

        public string Name => "sign";

        public string Description => "Signs or re-signs an assembly with a strong name.";

        public SnSignCommand(IStrongNameKeyLoader keyLoader, IStrongNameSigner signer)
        {
            _keyLoader = keyLoader;
            _signer = signer;
        }

        public void Configure(CommandLineApplication cmd)
        {
            _delaySign = cmd.Option<bool>("-d|--delay-sign", "Delay signs the assembly.",
                CommandOptionType.NoValue);

            _force = cmd.Option<bool>("-f|--force", "Forces overwrite of an existing assembly.",
                                      CommandOptionType.NoValue);

            _keyFile = cmd.Argument("KEYFILE", "The path of the key file. If the assembly is delay-signed this can be the public key.")
                .IsRequired(false, "The 'KEYFILE' argument is required.");

            _assemblyFile = cmd.Argument("ASSEMBLY", "The path of the assembly file to sign.")
                .IsRequired(false, "The 'ASSEMBLY' argument is required.");

            _outAssemblyFile = cmd.Argument(
                "OUTASSEMBLY",
                "The path of the signed assembly. If not specified the input assembly is overwritten.");
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool delaySign = _delaySign.HasValue();
            bool force = _force.HasValue();
            string keyFile = _keyFile.Value;
            string assemblyFile = _assemblyFile.Value;
            string outAssemblyFile = !string.IsNullOrEmpty(_outAssemblyFile.Value)
                ? _outAssemblyFile.Value
                : null;

            try
            {
                if (File.Exists(outAssemblyFile) && !force)
                    throw new FileAlreadyExistsException(outAssemblyFile);

                if (delaySign)
                {
                    StrongNamePublicKey publicKey = _keyLoader.LoadPublicKey(keyFile);
                    _signer.DelaySignAssembly(assemblyFile, publicKey, outAssemblyFile);
                }
                else
                {
                    StrongNameKey key = _keyLoader.LoadKey(keyFile);
                    _signer.SignAssembly(assemblyFile, key, outAssemblyFile);
                }
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            cmd.Out.WriteLine("Assembly '{0}' successfully signed.", outAssemblyFile);
            return ExitCodes.Success;
        }
    }
}