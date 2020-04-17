using System;
using System.IO;

namespace AppCore.SigningTool.StrongName
{
    public class SChannelKeyBlobLoader : IKeyPairLoader
    {
        public IKeyPair Load(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return SChannelKeyBlobFormatter.Deserialize(fileStream);
        }
    }
}