namespace AppCore.SigningTool.StrongName
{
    public interface IKeyPairLoader
    {
        IKeyPair Load(string path);
    }
}