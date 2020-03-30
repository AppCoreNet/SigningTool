using dnlib.DotNet;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameKeyLoader : IStrongNameKeyLoader
    {
        public StrongNamePublicKey LoadPublicKey(string path)
        {
            try
            {
                var privateKey = new StrongNameKey(path);
                return new StrongNamePublicKey(privateKey.PublicKey);
            }
            catch (InvalidKeyException)
            {
                return new StrongNamePublicKey(path);
            }
        }

        public StrongNameKey LoadKey(string path)
        {
            return new StrongNameKey(path);
        }
    }
}