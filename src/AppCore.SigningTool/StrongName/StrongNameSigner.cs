using System;
using System.IO;
using AppCore.SigningTool.Keys;
using dnlib.DotNet;
using dnlib.DotNet.Writer;

namespace AppCore.SigningTool.StrongName
{
    public class StrongNameSigner : IStrongNameSigner
    {
        private static ModuleDefMD LoadAssembly(string assemblyPath)
        {
            ModuleDefMD module = ModuleDefMD.Load(File.ReadAllBytes(assemblyPath));
            if (module.Assembly == null)
                throw new BadImageFormatException($"The file '{Path.GetFullPath(assemblyPath)}' is not a .NET assembly.");

            return module;
        }

        public void DelaySignAssembly(string assemblyPath, IPublicKey publicKey, string outAssemblyPath = null)
        {
            var rsaPublicKey = publicKey as RsaPublicKey;

            var strongNamePublicKey = new StrongNamePublicKey(
                rsaPublicKey.Modulus,
                rsaPublicKey.PublicExponent,
                (AssemblyHashAlgorithm)rsaPublicKey.HashAlgorithm);

            ModuleDefMD module = LoadAssembly(assemblyPath);

            var options = new ModuleWriterOptions(module)
            {
                DelaySign = true,
                StrongNamePublicKey = strongNamePublicKey
            };

            module.Write(outAssemblyPath ?? assemblyPath, options);
        }

        public void SignAssembly(string assemblyPath, IPrivateKey key, string outAssemblyPath = null)
        {
            var rsaPrivateKey = key as RsaPrivateKey;

            var keyBlobStream = new MemoryStream();
            SChannelKeyBlobFormatter.Serialize(keyBlobStream, rsaPrivateKey);
            var strongNameKey = new StrongNameKey(keyBlobStream.ToArray());

            ModuleDefMD module = LoadAssembly(assemblyPath);
            var options = new ModuleWriterOptions(module);
            options.InitializeStrongNameSigning(module, strongNameKey);
            module.Write(outAssemblyPath ?? assemblyPath, options);
        }
    }
}