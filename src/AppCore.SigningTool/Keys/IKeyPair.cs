namespace AppCore.SigningTool.Keys
{
    public interface IKeyPair
    {
        IPrivateKey PrivateKey { get; }

        IPublicKey PublicKey { get; }
    }
}