using System;

namespace AppCore.SigningTool.Keys
{
    public class InvalidKeyException : Exception
    {
        public InvalidKeyException(string message)
            : base(message)
        {
        }

        public InvalidKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
