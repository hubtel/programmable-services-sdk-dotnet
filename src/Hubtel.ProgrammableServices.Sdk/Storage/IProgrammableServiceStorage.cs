using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hubtel.ProgrammableServices.Sdk.Storage
{
    /// <summary>
    /// Exposes helpful methods that are called by the framework to manage a storage space.
    /// Storage space could be in-memory, file, relational or any custom implementation.
    /// Care must be taken to ensure prompt response for better user experience
    /// </summary>
    public interface IProgrammableServiceStorage
    {
        Task Set(string key, string value);
        Task<string> Get(string key);
        Task<List<string>> GetKeys();
        Task<bool> Exists(string key);
        Task Delete(string key);
    }
}