namespace AppCore.SigningTool.StrongName
{
    public interface IKeyGenerator
    {
        IKeyPair Generate(int? keySize = null);
    }
}