using AppCore.SigningTool.Keys;
using FluentAssertions;
using Xunit;

namespace AppCore.SigningTool.StrongName
{
    public class RsaKeyGeneratorTests
    {
        [Theory]
        [InlineData(1024)]
        [InlineData(2048)]
        public void GeneratesKeyWithSpecifiedSize(int keySize)
        {
            var generator = new RsaKeyGenerator();
            IKeyPair key = generator.Generate(keySize);

            key.PrivateKey.SignatureSize.Should()
               .Be(keySize / 8);
        }
    }
}
