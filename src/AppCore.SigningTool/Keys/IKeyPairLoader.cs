namespace AppCore.SigningTool.Keys
{
    public interface IKeyPairLoader
    {
        IKeyPair Load(string path);
    }
}