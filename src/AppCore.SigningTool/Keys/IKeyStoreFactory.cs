namespace AppCore.SigningTool.Keys
{
    public interface IKeyStoreFactory
    {
        IKeyStore Create(string name);
    }
}