using System;
using System.IO;

namespace AppCore.SigningTool.Keys
{
    public class SChannelKeyBlobStore : IKeyStore
    {
        public string Name => "snk";

        public IKeyPair Load(string path)
        {
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            return SChannelKeyBlobFormatter.Deserialize(fileStream);
        }

        public void Save(string path, IKeyPair keyPair, bool includePrivateKey = false)
        {
            var rsaKeyPair = keyPair as RsaKeyPair;
            if (rsaKeyPair == null)
                throw new NotSupportedException("Only RSA keys can be serialized as SChannel key blobs.");

            using var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            fileStream.SetLength(0);
            SChannelKeyBlobFormatter.Serialize(fileStream, rsaKeyPair, includePrivateKey);
        }
    }
}