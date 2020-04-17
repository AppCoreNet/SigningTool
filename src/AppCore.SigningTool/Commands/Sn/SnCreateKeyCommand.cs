using System;
using System.IO;
using AppCore.SigningTool.Exceptions;
using AppCore.SigningTool.StrongName;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnCreateKeyCommand : ICommand<SnCommand>
    {
        private readonly IStrongNameKeyGenerator _keyGenerator;
        private CommandOption<bool> _force;
        private CommandOption<int> _keySize;
        private CommandArgument _keyFile;

        public string Name => "create-key";

        public string Description => "Creates a new strong name key pair.";

        public SnCreateKeyCommand(IStrongNameKeyGenerator keyGenerator)
        {
            _keyGenerator = keyGenerator;
        }

        public void Configure(CommandLineApplication cmd)
        {
            _force = cmd.Option<bool>(
                "-f|--force",
                "Forces overwrite of an existing key file.",
                CommandOptionType.NoValue);

            _keySize = cmd.Option<int>(
                "-s|--size <KEYSIZE>",
                "Specifies the size of the key in bits. If not specified it defaults to 1024 bits.",
                CommandOptionType.SingleValue);

            _keyFile = cmd.Argument("KEYFILE", "The path of the key file.")
                .IsRequired(false, "The 'KEYFILE' argument is required.");
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool force = _force.HasValue();
            int? keySize = _keySize.HasValue() ? _keySize.ParsedValue : (int?) null;
            string keyFile = _keyFile.Value;
            
            try
            {
                if (File.Exists(keyFile) && !force)
                    throw new FileAlreadyExistsException(keyFile);

                StrongNameKey key = _keyGenerator.Generate(keySize);
                File.WriteAllBytes(keyFile, key.CreateStrongName());
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            cmd.Out.WriteLine("Key pair written to '{0}'.", keyFile);
            return ExitCodes.Success;
        }
    }
}