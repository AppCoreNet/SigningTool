namespace AppCore.SigningTool.Keys
{
    public interface IKeyGenerator
    {
        IKeyPair Generate(int? keySize = null);
    }
}