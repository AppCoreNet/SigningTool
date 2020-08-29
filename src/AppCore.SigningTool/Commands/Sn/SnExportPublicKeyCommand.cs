using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using AppCore.SigningTool.Exceptions;
using AppCore.SigningTool.Keys;
using AppCore.SigningTool.StrongName;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnExportPublicKeyCommand : ICommand<SnCommand>
    {
        private readonly IKeyStore _keyStore;
        private CommandOption<bool> _force;
        private CommandOption<string> _hashAlgorithm;
        private CommandArgument _keyFile;
        private CommandArgument _publicKeyFile;

        public string Name => "export-key";

        public string Description => "Exports the public key from a strong name key pair.";

        public SnExportPublicKeyCommand(IKeyStore keyStore)
        {
            _keyStore = keyStore;
        }

        public void Configure(CommandLineApplication cmd)
        {
            _force = cmd.Option<bool>("-f|--force", "Forces overwrite of an existing key file.",
                                      CommandOptionType.NoValue);

            _hashAlgorithm = cmd.Option<string>(
                "--hash <HASHALG>",
                "Specifies the embedding hash algorithm. Possible values: SHA1|SHA256|SHA384|SHA512. If not specified it defaults to SHA1.",
                CommandOptionType.SingleValue);

            _hashAlgorithm
                .OnValidate(
                    vc =>
                    {
                        if (ParseAssemblyHashAlgorithm(_hashAlgorithm.ParsedValue) == AssemblyHashAlgorithm.None)
                            return new ValidationResult($"Unknown or unsupported hash algorithm '{_hashAlgorithm.ParsedValue}'.");

                        return ValidationResult.Success;
                    });

            _keyFile = cmd.Argument("KEYFILE", "The path of the key file.")
                          .IsRequired(false, "The 'KEYFILE' argument is required.");

            _publicKeyFile = cmd.Argument("PUBLICKEYFILE", "The path of the public key file.")
                                .IsRequired(false, "The 'PUBLICKEYFILE' argument is required.");
        }

        private AssemblyHashAlgorithm ParseAssemblyHashAlgorithm(string value)
        {
            Enum.TryParse(value, out AssemblyHashAlgorithm result);
            switch (result)
            {
                case AssemblyHashAlgorithm.MD5:
                    result = AssemblyHashAlgorithm.None;
                    break;
            }

            return result;
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool force = _force.HasValue();

            AssemblyHashAlgorithm hashAlgorithm = _hashAlgorithm.HasValue()
                ? ParseAssemblyHashAlgorithm(_hashAlgorithm.ParsedValue)
                : AssemblyHashAlgorithm.Sha1;

            string keyFile = _keyFile.Value;
            string publicKeyFile = _publicKeyFile.Value;

            try
            {
                if (File.Exists(publicKeyFile) && !force)
                    throw new FileAreadyExistsException(publicKeyFile);

                IKeyPair keyPair = _keyStore.Load(keyFile);
                keyPair = keyPair.WithHashAlgorithm(hashAlgorithm);

                new SChannelKeyBlobStore().Save(publicKeyFile, keyPair);
            }
            catch (Exception error)
            {
                cmd.Error.WriteLine("ERROR: {0}", error.Message);
                return ExitCodes.FromException(error);
            }

            cmd.Out.WriteLine("Public key written to '{0}'.", publicKeyFile);
            return ExitCodes.Success;
        }
    }
}