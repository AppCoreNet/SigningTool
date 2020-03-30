using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public interface IStrongNameKeyGenerator
    {
        StrongNameKey Generate(int? keySize = null);
    }
}