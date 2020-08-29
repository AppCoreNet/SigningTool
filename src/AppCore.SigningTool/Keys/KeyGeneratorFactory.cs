using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCore.SigningTool.Keys
{
    public class KeyGeneratorFactory : IKeyGeneratorFactory
    {
        private readonly IEnumerable<IKeyGenerator> _generators;

        public KeyGeneratorFactory(IEnumerable<IKeyGenerator> generators)
        {
            _generators = generators;
        }

        public IKeyGenerator Create(string algorithm)
        {
            IKeyGenerator generator = _generators.FirstOrDefault(
                a => string.Equals(a.AlgorithmName, algorithm, StringComparison.InvariantCultureIgnoreCase));

            if (generator == null)
            {
                throw new ArgumentException("Unknown algorithm.");
            }

            return generator;
        }
    }
}