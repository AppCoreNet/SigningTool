using System.Reflection;

namespace AppCore.SigningTool.StrongName
{
    public interface IPublicKey
    {
        AssemblyHashAlgorithm HashAlgorithm { get; }

        IPublicKey WithHashAlgorithm(AssemblyHashAlgorithm hashAlgorithm);
    }
}