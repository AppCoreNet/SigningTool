namespace AppCore.SigningTool.Keys
{
    public interface IPrivateKey
    {
        /// <summary>
        /// Gets the strong name signature size in bytes
        /// </summary>
        int SignatureSize { get; }
    }
}