using System;
using System.IO;
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

        public void DelaySignAssembly(string assemblyPath, StrongNamePublicKey publicKey, string outAssemblyPath = null)
        {
            ModuleDefMD module = LoadAssembly(assemblyPath);
            var options = new ModuleWriterOptions(module)
            {
                DelaySign = true,
                StrongNamePublicKey = publicKey
            };

            module.Write(outAssemblyPath ?? assemblyPath, options);
        }

        public void SignAssembly(string assemblyPath, StrongNameKey key, string outAssemblyPath = null)
        {
            ModuleDefMD module = LoadAssembly(assemblyPath);
            var options = new ModuleWriterOptions(module);
            options.InitializeStrongNameSigning(module, key);
            module.Write(outAssemblyPath ?? assemblyPath, options);
        }
    }
}