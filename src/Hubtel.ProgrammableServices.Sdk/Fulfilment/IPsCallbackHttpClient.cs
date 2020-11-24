using System.Net.Http;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public interface IPsCallbackHttpClient
    { HttpClient Client { get; }
    }
}