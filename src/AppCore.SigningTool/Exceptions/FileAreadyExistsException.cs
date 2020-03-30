using System.IO;

namespace AppCore.SigningTool.Exceptions
{
    public class FileAreadyExistsException : IOException
    {
        public FileAreadyExistsException(string fileName)
            : base($"File {Path.GetFullPath(fileName)} already exists.")
        {
        }
    }
}
