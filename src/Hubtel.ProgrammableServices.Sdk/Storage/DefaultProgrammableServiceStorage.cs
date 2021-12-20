using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.ProgrammableServices.Sdk.Storage
{
    /// <summary>
    /// De-facto storage space that uses ConcurrentDictionary as an in-memory store to persist data
    /// </summary>
    public class DefaultProgrammableServiceStorage : IProgrammableServiceStorage
    {
        private ConcurrentDictionary<string, string> _backingStore;
        public DefaultProgrammableServiceStorage()
        {
            _backingStore = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        public Task Set(string key, string value)
        {
            _backingStore[key] = value;
            return Task.CompletedTask;
        }

        public Task<string> Get(string key)
        {
            if (_backingStore.ContainsKey(key))
            {
                return Task.FromResult(_backingStore[key]);
            }

            return Task.FromResult(string.Empty);
        }

        public Task<List<string>> GetKeys() => Task.FromResult(_backingStore.Keys.ToList());

        public Task<bool> Exists(string key)
        {
            return Task.FromResult(_backingStore.ContainsKey(key));
        }

        public Task Delete(string key)
        {
            if (_backingStore.ContainsKey(key))
            {
                return Task.FromResult(_backingStore.TryRemove(key, out var val));
            }
            return Task.CompletedTask;
        }
    }
}