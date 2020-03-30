using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public static class StrongNameKeyExtensions
    {
        public static StrongNamePublicKey GetPublicKey(this StrongNameKey key)
        {
            return new StrongNamePublicKey(key.PublicKey);
        }
    }
}