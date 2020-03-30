using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public interface IStrongNameKeyLoader
    {
        StrongNamePublicKey LoadPublicKey(string path);

        StrongNameKey LoadKey(string path);
    }
}