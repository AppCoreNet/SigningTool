using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public interface IStrongNameSigner
    {
        void DelaySignAssembly(string assemblyPath, StrongNamePublicKey publicKey, string outAssemblyPath = null);

        void SignAssembly(string assemblyPath, StrongNameKey key, string outAssemblyPath = null);
    }
}
