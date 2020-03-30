using System;
using System.IO;
using dnlib.DotNet;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameKeyLoaderTests
    {
        private static string KeyDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");

        [Fact]
        public void CanLoadKeyGeneratedBySn()
        {
            string keyFile = Path.Combine(KeyDir, "test.snk");

            var loader = new StrongNameKeyLoader();
            StrongNameKey key = loader.LoadKey(keyFile);

            key.SignatureSize.Should()
               .Be(1024 / 8);
        }

        [Fact]
        public void CanLoadPublicKeyGeneratedBySn()
        {
            string keyFile = Path.Combine(KeyDir, "test_public_sha1.snk");

            var loader = new StrongNameKeyLoader();
            StrongNamePublicKey key = loader.LoadPublicKey(keyFile);

            key.HashAlgorithm.Should()
               .Be(AssemblyHashAlgorithm.SHA1);

            key.SignatureAlgorithm.Should()
               .Be(SignatureAlgorithm.CALG_RSA_SIGN);
        }

        [Fact]
        public void CanLoadPublicKeyFromKey()
        {
            string keyFile = Path.Combine(KeyDir, "test.snk");

            var loader = new StrongNameKeyLoader();
            loader.LoadPublicKey(keyFile);
        }

        [Fact]
        public void LoadKeyThrowsFileNotFoundException()
        {
            string keyFile = Path.Combine(KeyDir, "unknown.snk");

            var loader = new StrongNameKeyLoader();
            Action method = () => loader.LoadKey(keyFile);

            method.Should()
                  .Throw<FileNotFoundException>();
        }

        [Fact]
        public void LoadKeyThrowsInvalidKeyExceptionForPublicKey()
        {
            string keyFile = Path.Combine(KeyDir, "test_public_sha1.snk");

            var loader = new StrongNameKeyLoader();
            Action method = () => loader.LoadKey(keyFile);

            method.Should()
                  .Throw<InvalidKeyException>();
        }

        [Fact]
        public void LoadPublicKeyThrowsFileNotFoundException()
        {
            string keyFile = Path.Combine(KeyDir, "unknown.snk");

            var loader = new StrongNameKeyLoader();
            Action method = () => loader.LoadPublicKey(keyFile);

            method.Should()
                  .Throw<FileNotFoundException>();
        }
    }
}