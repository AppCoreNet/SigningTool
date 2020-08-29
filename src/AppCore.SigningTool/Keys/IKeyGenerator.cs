namespace AppCore.SigningTool.Keys
{
    public interface IKeyGenerator
    {
        string AlgorithmName { get; }

        IKeyPair Generate(int? keySize = null);
    }
}