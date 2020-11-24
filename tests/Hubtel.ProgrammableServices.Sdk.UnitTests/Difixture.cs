using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Extensions;
using Hubtel.ProgrammableServices.Sdk.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Hubtel.ProgrammableServices.Sdk.UnitTests
{
    public class Difixture
    {
        public Difixture()
        {
            var services = new ServiceCollection();



            services.AddHubtelProgrammableServices((p) =>
            {
                return new ProgrammableServiceConfiguration
                {
                    HubtelFulfilmentApiEndpoint = "http://localhost:9000",
                    Storage = new DefaultProgrammableServiceStorage()
                };
            });

            ServiceProvider = services.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
}