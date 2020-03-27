using System.IO;
using AppCore.SigningTool.StrongNaming;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnCreateKeyCommand : ICommand
    {
        private CommandOption<bool> _force;
        private CommandOption<int> _keySize;
        private CommandArgument _keyFile;

        public string Name => "create-key";

        public string Description => "Creates a new strong name key pair.";

        public void Configure(CommandLineApplication cmd)
        {
            _force = cmd.Option<bool>("-f|--force", "Forces overwrite of an existing key file.",
                CommandOptionType.NoValue);

            _keySize = cmd.Option<int>("-s|--size <KEYSIZE>", "Specifies the size of the key in bits. If not specified it defaults to 1024 bits.",
                CommandOptionType.SingleValue);

            _keyFile = cmd.Argument("KEYFILE", "The name of the key file.")
                .IsRequired();
        }

        public int OnExecute(CommandLineApplication cmd)
        {
            var force = _force.HasValue();
            var keySize = _keySize.HasValue() ? _keySize.ParsedValue : (int?) null;
            var keyFile = _keyFile.Value;
            
            if (File.Exists(keyFile) && !force)
            {
                cmd.Error.WriteLine("Key file '{0}' already exists.", keyFile);
                return ExitCodes.FileAreadyExists;
            }

            var key = new KeyGenerator().Generate(keySize);
            File.WriteAllBytes(keyFile, key.CreateStrongName());
            cmd.Out.WriteLine("Key pair written to '{0}'.", keyFile);

            return ExitCodes.Success;
        }
    }
}