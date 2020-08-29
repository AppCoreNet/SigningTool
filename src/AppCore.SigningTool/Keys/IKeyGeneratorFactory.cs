namespace AppCore.SigningTool.Keys
{
    public interface IKeyGeneratorFactory
    {
        IKeyGenerator Create(string algorithm);
    }
}