using Hubtel.ProgrammableServices.Sdk.Storage;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class ProgrammableServiceConfiguration
    {
        public IProgrammableServiceStorage Storage { get; set; }
        public string HubtelFulfilmentApiEndpoint { get; set; }
    }
}