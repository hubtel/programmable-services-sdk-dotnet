using System.Reflection;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
   
    public class ProgammableServicesMethodInfo
    {
        public bool IsInitiationMethod { get; set; }
        public MethodInfo Method { get; set; }
    }
}