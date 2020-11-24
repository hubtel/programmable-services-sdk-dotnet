using Hubtel.ProgrammableServices.Sdk.Models;
using Hubtel.ProgrammableServices.Sdk.Storage;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class ProgrammableServiceContext 
    {
        public string NextRouteKey => Request.SessionId + "NextRoute";
        public string DataBagKey => Request.SessionId + "DataBag";

        public ProgrammableServiceRequest Request { get; set; }
        public IProgrammableServiceStorage Store { get; set; }
        public ProgrammableServiceDataBag DataBag { get; set; }

      
    }
}