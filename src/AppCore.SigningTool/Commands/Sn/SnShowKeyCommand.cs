using System;
using AppCore.SigningTool.Keys;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnShowKeyCommand : ICommand<SnCommand>
    {
        private readonly IKeyStore _keyLoader;
        private CommandOption<bool> _showKey;
        private CommandArgument _keyFile;

        public string Name => "show-key";

        public string Description => "Displays the public key token.";

        public SnShowKeyCommand(IKeyStore keyLoader)
        {
            _keyLoader = keyLoader;
        }

        public void Configure(CommandLineApplication cmd)
        {
            _showKey = cmd.Option<bool>(
                "--with-key",
                "Output the public key in addition to the key token.",
                CommandOptionType.NoValue);

            _keyFile = cmd.Argument("KEYFILE", "The path of the public key file.")
                          .IsRequired(false, "The 'KEYFILE' argument is required.");
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool showKey = _showKey.HasValue();
            string keyFile = _keyFile.Value;

            IKeyPair keyPair;
            try
            {
                keyPair = _keyLoader.Load(keyFile);
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            if (showKey)
            {
                cmd.Out.WriteLine("Public key (hash algorithm {0}) is:");
                //TODO:cmd.Out.WriteLine(keyPair.CreatePublicKey().ToHexString());
                cmd.Out.WriteLine();
            }

            cmd.Out.WriteLine("Public key token is:");
            //TODO:cmd.Out.WriteLine(keyPair.CreatePublicKeyToken().ToHexString());

            return ExitCodes.Success;
        }
    }
}