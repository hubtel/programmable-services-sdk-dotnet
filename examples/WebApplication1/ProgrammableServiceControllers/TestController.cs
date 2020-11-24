using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace ProgrammableServicesSampleApp.ProgrammableServiceControllers
{
    public class TestController : ProgrammableServiceControllerBase
    {
        private readonly ILogger<EvdController> _logger;

        public TestController(ILogger<EvdController> logger)
        {
            _logger = logger;
        }

        
       

        private async Task<ProgrammableServiceResponse> Index()
        {
            return await RenderResponse("Called from another controller's method!");
        }

        private async Task<ProgrammableServiceResponse> RenderResponse(string response)
        {
            var resp = Render(response, null, null);
            // setup rich ux for web and mobile
            resp.Label = response;
            resp.DataType = ProgrammableServiceDataTypes.Display;
            return await Task.FromResult(resp);
        }

       

        
    }
}