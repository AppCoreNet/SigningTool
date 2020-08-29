using System;
using System.Collections.Generic;
using System.Linq;

namespace AppCore.SigningTool.Keys
{
    public class KeyStoreFactory : IKeyStoreFactory
    {
        private readonly IEnumerable<IKeyStore> _stores;

        public KeyStoreFactory(IEnumerable<IKeyStore> stores)
        {
            _stores = stores;
        }

        public IKeyStore Create(string name)
        {
            IKeyStore store = _stores.FirstOrDefault(
                a => string.Equals(a.Name, name, StringComparison.InvariantCultureIgnoreCase));

            if (store == null)
            {
                throw new ArgumentException("Unknown store.");
            }

            return store;
        }
    }
}