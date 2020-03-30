using System.IO;
using dnlib.DotNet;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameKeyGeneratorTests
    {
        [Fact]
        public void GeneratesKeyWhichSnCanExport()
        {
            string keyFile = nameof(GeneratesKeyWhichSnCanExport) + ".snk";
            string publicKeyFile = nameof(GeneratesKeyWhichSnCanExport) + "_public.snk";

            var generator = new StrongNameKeyGenerator();
            StrongNameKey key = generator.Generate();
            File.WriteAllBytes(keyFile, key.CreateStrongName());

            int exitCode = Exec.RunNetFxTool("sn.exe", new[] {"-p", keyFile, publicKeyFile});
            exitCode.Should()
                    .Be(0);
        }
    }
}
