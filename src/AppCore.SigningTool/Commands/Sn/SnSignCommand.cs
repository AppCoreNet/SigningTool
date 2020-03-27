using System;
using System.IO;
using dnlib.DotNet;
using dnlib.DotNet.Writer;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnSignCommand : ICommand
    {
        private CommandArgument _keyFile;
        private CommandArgument _assemblyFile;
        private CommandOption<bool> _delaySign;

        public string Name => "sign";

        public string Description => "Signs or re-signs an assembly with a strong name.";

        public void Configure(CommandLineApplication cmd)
        {
            _delaySign = cmd.Option<bool>("-d|--delay-sign", "Delay signs the assembly.",
                CommandOptionType.NoValue);

            _keyFile = cmd.Argument("KEYFILE", "The path of the strong name key file.")
                .IsRequired(false);

            _assemblyFile = cmd.Argument("ASSEMBLY", "The path of the assembly file to sign.")
                .IsRequired(false, "The 'assembly' argument is required.");
        }

        public int OnExecute(CommandLineApplication cmd)
        {
            bool delaySign = _delaySign.HasValue();
            string keyFile = _keyFile.Value;
            string assemblyFile = _assemblyFile.Value;

            try
            {
                var module = ModuleDefMD.Load(File.ReadAllBytes(assemblyFile));
                if (module.Assembly == null)
                {
                    throw new BadImageFormatException($"The file '{Path.GetFullPath(assemblyFile)}' is not a .NET assembly.");
                }

                var options = new ModuleWriterOptions(module);
                if (delaySign)
                {
                    options.DelaySign = true;
                    options.StrongNamePublicKey = LoadPublicKey(keyFile);
                }
                else
                {
                    var strongNameKey = new StrongNameKey(keyFile);
                    options.InitializeStrongNameSigning(module, strongNameKey);
                }

                module.Write("test.dll", options);
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            return ExitCodes.Success;
        }

        private StrongNamePublicKey LoadPublicKey(string keyFile)
        {
            try
            {
                var privateKey = new StrongNameKey(keyFile);
                return new StrongNamePublicKey(privateKey.PublicKey);
            }
            catch (InvalidKeyException)
            {
                return new StrongNamePublicKey(keyFile);
            }

        }
    }
}