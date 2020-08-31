using System.IO;

namespace AppCore.SigningTool.Exceptions
{
    public class FileAlreadyExistsException : IOException
    {
        public FileAlreadyExistsException(string fileName)
            : base($"File {Path.GetFullPath(fileName)} already exists.")
        {
        }
    }
}
