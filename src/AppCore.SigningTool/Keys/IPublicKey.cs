using System.Reflection;

namespace AppCore.SigningTool.Keys
{
    public interface IPublicKey
    {
        AssemblyHashAlgorithm HashAlgorithm { get; }

        IPublicKey WithHashAlgorithm(AssemblyHashAlgorithm hashAlgorithm);
    }
}