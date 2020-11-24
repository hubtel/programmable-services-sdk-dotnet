using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Models;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public interface IHubtelProgrammableService
    {
        /// <summary>
        /// Executes an interaction flow against an input
        /// </summary>
        /// <param name="request">Must be valid and not null</param>
        /// <param name="controllerName">Must be valid and not null</param>
        /// <param name="actionName">Optional. If empty, any method that has [HandleInitiation] attribute will be called to execute any initiation request.</param>
        /// <returns></returns>
        Task<ProgrammableServiceResponse> ExecuteInteraction(ProgrammableServiceRequest request, string controllerName,
            string actionName="");
    }


}
