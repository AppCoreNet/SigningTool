using System;
using AppCore.SigningTool.StrongName;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnSignCommand : ICommand<SnCommand>
    {
        private readonly IStrongNameKeyLoader _keyLoader;
        private readonly IStrongNameSigner _signer;
        private CommandArgument _keyFile;
        private CommandArgument _assemblyFile;
        private CommandOption<bool> _delaySign;

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

            _keyFile = cmd.Argument("KEYFILE", "The path of the strong name key file.")
                .IsRequired(false, "The 'KEYFILE' argument is required.");

            _assemblyFile = cmd.Argument("ASSEMBLY", "The path of the assembly file to sign.")
                .IsRequired(false, "The 'ASSEMBLY' argument is required.");
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool delaySign = _delaySign.HasValue();
            string keyFile = _keyFile.Value;
            string assemblyFile = _assemblyFile.Value;

            try
            {
                if (delaySign)
                {
                    StrongNamePublicKey publicKey = _keyLoader.LoadPublicKey(keyFile);
                    _signer.DelaySignAssembly(assemblyFile, publicKey);
                }
                else
                {
                    StrongNameKey key = _keyLoader.LoadKey(keyFile);
                    _signer.SignAssembly(assemblyFile, key);
                }
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            return ExitCodes.Success;
        }
    }
}