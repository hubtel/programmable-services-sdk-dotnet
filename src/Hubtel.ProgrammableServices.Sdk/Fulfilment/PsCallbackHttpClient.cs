using System.Net.Http;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public class PsCallbackHttpClient:IPsCallbackHttpClient
    {
        public HttpClient Client { get; }
        public PsCallbackHttpClient(HttpClient client)
        {
            Client = client;
            
        }
    }
}