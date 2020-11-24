using System.Collections.Concurrent;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class ProgrammableServicesControllerActivator
    {
        public ConcurrentDictionary<string, ProgrammableServiceControllerInfo> ControllerCollection { get; }

        public ProgrammableServicesControllerActivator(ConcurrentDictionary<string, ProgrammableServiceControllerInfo> col)
        {
            ControllerCollection = col;
        }
    }
}