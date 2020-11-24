using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Storage;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public sealed class ProgrammableServiceDataBag
    {
        private readonly IProgrammableServiceStorage _storage;
        private readonly string _dataBagKey;

        public ProgrammableServiceDataBag(IProgrammableServiceStorage storage, string dataBagKey)
        {
            _storage = storage;
            _dataBagKey = dataBagKey;
        }

        public async Task Set(string key, string value)
        {
            await _storage.Set($"{_dataBagKey}_{key}", value);
        }

        public async Task<string> Get(string key)
        {
            return await _storage.Get($"{_dataBagKey}_{key}");
        }

        public async Task<bool> Exists(string key)
        {
            return await _storage.Exists($"{_dataBagKey}_{key}");
        }

        public async Task Delete(string key)
        {
            await _storage.Delete($"{_dataBagKey}_{key}");
        }
    }
}