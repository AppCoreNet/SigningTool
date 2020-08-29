using System;
using System.IO;
using System.Reflection;
using AppCore.SigningTool.Keys;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class SChannelKeyBlobStoreTests
    {
        private static string KeyDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");

        [Fact]
        public void CanLoadKeyGeneratedBySn()
        {
            string keyFile = Path.Combine(KeyDir, "test.snk");

            var loader = new SChannelKeyBlobStore();
            IKeyPair key = loader.Load(keyFile);

            key.PrivateKey.Should()
               .NotBeNull();

            key.PrivateKey.SignatureSize.Should()
               .Be(1024 / 8);

            key.PublicKey.Should()
               .NotBeNull();

            key.PublicKey.HashAlgorithm.Should()
               .Be(AssemblyHashAlgorithm.Sha1);
        }

        [Theory]
        [InlineData(AssemblyHashAlgorithm.Sha1)]
        [InlineData(AssemblyHashAlgorithm.Sha256)]
        public void CanLoadPublicKeyGeneratedBySn(AssemblyHashAlgorithm algorithm)
        {
            string algorithmName = Enum.GetName(typeof(AssemblyHashAlgorithm), algorithm);
            string keyFile = Path.Combine(KeyDir, $"test_public_{algorithmName}.snk");

            var loader = new SChannelKeyBlobStore();
            IKeyPair key = loader.Load(keyFile);

            key.PrivateKey.Should()
               .BeNull();

            key.PublicKey.HashAlgorithm.Should()
               .Be(algorithm);

            /*
            key.SignatureAlgorithm.Should()
               .Be(SignatureAlgorithm.CALG_RSA_SIGN);*/
        }

        [Fact]
        public void LoadKeyThrowsFileNotFoundException()
        {
            string keyFile = Path.Combine(KeyDir, "unknown.snk");

            var loader = new SChannelKeyBlobStore();
            Action method = () => loader.Load(keyFile);

            method.Should()
                  .Throw<FileNotFoundException>();
        }

        [SkippableFact]
        public void SavedKeyPairCanBeReadBySn()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            string keyFile = nameof(SavedKeyPairCanBeReadBySn) + ".snk";
            string publicKeyFile = nameof(SavedKeyPairCanBeReadBySn) + "_public.snk";

            var generator = new RsaKeyGenerator();
            IKeyPair key = generator.Generate();

            new SChannelKeyBlobStore().Save(keyFile, key, true);

            int exitCode = Exec.RunNetFxTool("sn.exe", new[] { "-p", keyFile, publicKeyFile });
            exitCode.Should()
                    .Be(0);
        }

        [SkippableFact]
        public void SavedPublicKeyCanByReadBySn()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

            string keyFile = nameof(SavedKeyPairCanBeReadBySn) + ".snk";

            var generator = new RsaKeyGenerator();
            IKeyPair key = generator.Generate();

            new SChannelKeyBlobStore().Save(keyFile, key, false);

            int exitCode = Exec.RunNetFxTool("sn.exe", new[] { "-t", keyFile });
            exitCode.Should()
                    .Be(0);
        }
    }
}