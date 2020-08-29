namespace AppCore.SigningTool.Keys
{
    public interface IKeyStore
    {
        string Name { get; }

        IKeyPair Load(string path);

        void Save(string path, IKeyPair keyPair, bool includePrivateKey = false);
    }
}