using Hubtel.ProgrammableServices.Sdk.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hubtel.ProgrammableServices.Sdk.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Dynamically attaches a middleware to this HTTP pipeline.
        /// When matched with mvcPath during a POST request, it invokes ExecuteInteraction and returns the response
        /// </summary>
        /// <param name="app"></param>
        /// <param name="mvcPath">Must be valid and not null. Format is /{app_name} e.g. /myapp</param>
        /// <param name="controller">The programmable service controller</param>
        /// <param name="action">Optional. Any action in controller with HandleInitiation will be called to handle any initiation request</param>
        public static void UseHubtelProgrammableService(this IApplicationBuilder app,string mvcPath,string controller,string action="")
        {

            app.Map(mvcPath,
                new ProgrammableServicesMvcPipelineHandler(
                        app.ApplicationServices.GetService<IHubtelProgrammableService>(), controller, action)
                    .HandleRequest);
        }
    }
}