using AppCore.SigningTool.Keys;
using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public interface IStrongNameSigner
    {
        void DelaySignAssembly(string assemblyPath, IPublicKey publicKey, string outAssemblyPath = null);

        void SignAssembly(string assemblyPath, IPrivateKey key, string outAssemblyPath = null);
    }
}
