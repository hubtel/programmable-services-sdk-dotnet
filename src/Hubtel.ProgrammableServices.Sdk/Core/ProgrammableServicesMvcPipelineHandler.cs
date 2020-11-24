using System;
using System.Net;
using System.Text;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class ProgrammableServicesMvcPipelineHandler
    {
        private readonly IHubtelProgrammableService _programmableService;
        private readonly string _controller;
        private readonly string _action;

        public ProgrammableServicesMvcPipelineHandler(IHubtelProgrammableService programmableService, string controller, string action="")
        {
            _programmableService = programmableService;
            _controller = controller;
            _action = action;
        }

      
        public void HandleRequest(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                //read payload, convert to request object
                //grab response from _programmableService instance, convert to JSON and send back
                var requestString = await context.Request.ReadRequestStreamAsStringAsync();

                ProgrammableServiceRequest request = null;
                ProgrammableServiceResponse response;
                try
                {
                    request = JsonConvert.DeserializeObject<ProgrammableServiceRequest>(requestString);
                }
                catch (Exception e)
                {
                    // we don't know what happened oooo   
                }

                if (request==null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = new ProgrammableServiceResponse
                    {
                        Message = "Something bad happened",
                        Type = ProgrammableServiceActionType.Release.ToString("G"),
                        Label = "Something bad happened"

                    };
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                
                    response = await _programmableService.ExecuteInteraction(request, _controller, _action);
                }

                
                var responseJson = JsonConvert.SerializeObject(response,new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    Formatting = Formatting.Indented
                });


                context.Response.ContentLength = responseJson.Length;
                context.Response.ContentType = "application/json";


                await context.Response.WriteAsync(responseJson, Encoding.UTF8);
            });
        }
    }
}