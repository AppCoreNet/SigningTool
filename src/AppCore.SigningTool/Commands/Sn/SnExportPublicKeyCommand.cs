using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using AppCore.SigningTool.Exceptions;
using AppCore.SigningTool.StrongName;
using dnlib.DotNet;
using McMaster.Extensions.CommandLineUtils;

namespace AppCore.SigningTool.Commands.Sn
{
    public class SnExportPublicKeyCommand : ICommand<SnCommand>
    {
        private readonly IStrongNameKeyLoader _keyLoader;
        private CommandOption<bool> _force;
        private CommandOption<string> _hashAlgorithm;
        private CommandArgument _keyFile;
        private CommandArgument _publicKeyFile;

        public string Name => "export-key";

        public string Description => "Exports the public key from a strong name key pair.";

        public SnExportPublicKeyCommand(IStrongNameKeyLoader keyLoader)
        {
            _keyLoader = keyLoader;
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
                            return new ValidationResult($"Unknown hash algorithm '{_hashAlgorithm.ParsedValue}'.");

                        return ValidationResult.Success;
                    });

            _keyFile = cmd.Argument("KEYFILE", "The path of the key file.")
                          .IsRequired(false, "The 'KEYFILE' argument is required.");

            _publicKeyFile = cmd.Argument("PUBLICKEYFILE", "The path of the public key file.")
                                .IsRequired(false, "The 'PUBLICKEYFILE' argument is required.");
        }

        private AssemblyHashAlgorithm ParseAssemblyHashAlgorithm(string value)
        {
            var result = AssemblyHashAlgorithm.None;
            switch (value.ToLowerInvariant())
            {
                case "sha1":
                    result = AssemblyHashAlgorithm.SHA1;
                    break;
                case "sha256":
                    result = AssemblyHashAlgorithm.SHA_256;
                    break;
                case "sha384":
                    result = AssemblyHashAlgorithm.SHA_384;
                    break;
                case "sha512":
                    result = AssemblyHashAlgorithm.SHA_512;
                    break;
            }

            return result;
        }

        public int Execute(CommandLineApplication cmd)
        {
            bool force = _force.HasValue();

            AssemblyHashAlgorithm hashAlgorithm = _hashAlgorithm.HasValue()
                ? ParseAssemblyHashAlgorithm(_hashAlgorithm.ParsedValue)
                : AssemblyHashAlgorithm.SHA1;

            string keyFile = _keyFile.Value;
            string publicKeyFile = _publicKeyFile.Value;

            try
            {
                if (File.Exists(publicKeyFile) && !force)
                    throw new FileAreadyExistsException(publicKeyFile);

                StrongNameKey key = _keyLoader.LoadKey(keyFile);

                byte[] publicKey = key.WithHashAlgorithm(hashAlgorithm)
                                      .PublicKey;

                File.WriteAllBytes(publicKeyFile, publicKey);
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