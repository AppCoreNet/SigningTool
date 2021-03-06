using System;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using dnlib.DotNet;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameSignerTests
    {
        private static string KeyDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");

        private PEReader GetPEReader(string outputAssembly)
        {
            return new PEReader(new MemoryStream(File.ReadAllBytes(outputAssembly)));
        }

        private static AssemblyName GetAssemblyName(MetadataReader reader)
        {
            return reader.GetAssemblyDefinition()
                         .GetAssemblyName();
        }

        [Theory]
        [InlineData("DelaySignedAssembly.dll", "DelaySignedAssembly_out.dll")]
        [InlineData("UnsignedAssembly.dll", "UnsignedAssembly_out.dll")]
        [InlineData("SignedAssembly.dll", "SignedAssembly_out.dll")]
        public void CanSignAssembly(string inputAssembly, string outputAssembly)
        {
            string keyFile = Path.Combine(KeyDir, "test.snk");
            var key = new StrongNameKey(keyFile);

            var signer = new StrongNameSigner();
            signer.SignAssembly(inputAssembly, key, outputAssembly);

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                int exitCode = Exec.RunNetFxTool("sn.exe", new[] { "-v", outputAssembly });
                exitCode.Should()
                        .Be(0);
            }

            PEReader assemblyReader = GetPEReader(outputAssembly);
            assemblyReader.PEHeaders.CorHeader.Flags.Should()
                          .HaveFlag(CorFlags.StrongNameSigned);

            AssemblyName assemblyName = GetAssemblyName(assemblyReader.GetMetadataReader());
            StrongNamePublicKey publicKey = key.GetPublicKey();

            assemblyName.GetPublicKey()
                        .Should()
                        .BeEquivalentTo(publicKey.CreatePublicKey());

            assemblyName.GetPublicKeyToken()
                        .Should()
                        .BeEquivalentTo(publicKey.CreatePublicKeyToken());
        }

        [Theory]
        [InlineData("DelaySignedAssembly.dll", "DelaySignedAssembly_out.dll")]
        [InlineData("UnsignedAssembly.dll", "UnsignedAssembly_out.dll")]
        [InlineData("SignedAssembly.dll", "SignedAssembly_out.dll")]
        public void CanDelaySignAssembly(string inputAssembly, string outputAssembly)
        {
            string keyFile = Path.Combine(KeyDir, "test_public_sha1.snk");
            var publicKey = new StrongNamePublicKey(keyFile);

            var signer = new StrongNameSigner();
            signer.DelaySignAssembly(inputAssembly, publicKey, outputAssembly);

            PEReader assemblyReader = GetPEReader(outputAssembly);
            assemblyReader.PEHeaders.CorHeader.Flags.Should()
                          .NotHaveFlag(CorFlags.StrongNameSigned);

            AssemblyName assemblyName = GetAssemblyName(assemblyReader.GetMetadataReader());

            assemblyName.GetPublicKey()
                        .Should()
                        .BeEquivalentTo(publicKey.CreatePublicKey());

            assemblyName.GetPublicKeyToken()
                        .Should()
                        .BeEquivalentTo(publicKey.CreatePublicKeyToken());
        }
    }
}