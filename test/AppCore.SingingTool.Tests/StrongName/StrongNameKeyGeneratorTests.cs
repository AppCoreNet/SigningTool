using System;
using System.IO;
using dnlib.DotNet;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameKeyGeneratorTests
    {
        [SkippableFact]
        public void GeneratesKeyWhichSnCanExport()
        {
            Skip.IfNot(Environment.OSVersion.Platform == PlatformID.Win32NT);

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
