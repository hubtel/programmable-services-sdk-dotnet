using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Fulfilment;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProgrammableServicesSampleApp.ProgrammableServiceControllers;

namespace ProgrammableServicesSampleApp.Controllers
{
    [Route("api/requests")]
    
    public class RequestController : Controller
    {
        
        private readonly ILogger<RequestController> _logger;
        private readonly IHubtelProgrammableService _programmableService;
        private readonly IProgrammableServicesCallbackApi _callbackApi;
        


        public RequestController(
             ILogger<RequestController> logger
            ,IHubtelProgrammableService programmableService
             , IProgrammableServicesCallbackApi callbackApi
             
            )
        {
            _logger = logger;
            _programmableService = programmableService;
            _callbackApi = callbackApi;
            
        }

        [Route("evd")]
        [HttpPost]
        public async Task<IActionResult> DoEvd(
            [FromBody]ProgrammableServiceRequest request
        )
        {
            
            //this action will be called anytime a user wants to interact with your application
            _logger.LogDebug("received request for {msisdn} {session_id} {gs_request}", request.Mobile, request.SessionId,
                JsonConvert.SerializeObject(request));
            
            var response = await _programmableService.ExecuteInteraction(request, nameof(EvdController));
            return Ok(response);
        }

        [Route("evdcallback")]
        [HttpPost]
        public async Task<IActionResult> DoEvdCallback([FromBody]ProgrammableServiceFulfilmentRequest request)
        {
            //Hubtel will call your fulfilment endpoint (this API)
            //after your business logic you are required to send callback to Hubtel
            //below is the code to send callback to Hubtel
            var response = await _callbackApi.ReportServiceFulfilmentStatus(request.OrderId, request.SessionId, "1234",
                "successful", "success", null);

            //we recommend using a messaging queue in order to execute your business logic
            //in the background
            return Ok();
        }
    }
}