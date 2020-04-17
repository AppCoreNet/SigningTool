using System;
using System.Reflection;

namespace AppCore.SigningTool.StrongName
{
    public interface IKeyPair
    {
        IPrivateKey PrivateKey { get; }

        IPublicKey PublicKey { get; }
    }
}