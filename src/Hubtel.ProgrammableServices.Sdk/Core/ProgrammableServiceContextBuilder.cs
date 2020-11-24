using Hubtel.ProgrammableServices.Sdk.Models;
using Hubtel.ProgrammableServices.Sdk.Storage;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    internal class ProgrammableServiceContextBuilder
    {
        private readonly ProgrammableServiceRequest _request;
        private readonly ProgrammableServiceConfiguration _configuration; 
        
        public ProgrammableServiceContextBuilder(ProgrammableServiceRequest request, ProgrammableServiceConfiguration configuration)
        {
            _request = request;
            _configuration = configuration;
        }

        public ProgrammableServiceContext Build()
        {
            var context = new ProgrammableServiceContext();
          
           

            if (_configuration.Storage==null)
            {
                _configuration.Storage = new DefaultProgrammableServiceStorage();
            }
            //NOTE: the ordering of the LOC below is extremely important since context.DataBagKey directly depends on Request.SessionId
            context.Request = _request;
            context.Store = _configuration.Storage;
            var dataBag = new ProgrammableServiceDataBag(_configuration.Storage, context.DataBagKey);

            context.DataBag = dataBag;
          
            return context;
        }
    }
}