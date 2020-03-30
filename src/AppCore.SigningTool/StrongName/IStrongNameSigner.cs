using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public interface IStrongNameSigner
    {
        void DelaySignAssembly(string assemblyPath, StrongNamePublicKey publicKey);

        void SignAssembly(string assemblyPath, StrongNameKey key);
    }
}
