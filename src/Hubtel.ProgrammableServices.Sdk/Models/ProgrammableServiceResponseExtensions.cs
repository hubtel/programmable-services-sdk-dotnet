using Hubtel.ProgrammableServices.Sdk.Core;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public static class ProgrammableServiceResponseExtensions
    {
        /// <summary>
        /// Attempts to determine the action type from Hubtel
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ProgrammableServiceActionType GetActionType(this ProgrammableServiceResponse request)
        {
            if (string.IsNullOrEmpty(request.Type))
            {
                return ProgrammableServiceActionType.Unknown;
            }
            switch (request.Type.ToLower())
            {
                case "initiation":
                    return ProgrammableServiceActionType.Initiation;
                case "response":
                    return ProgrammableServiceActionType.Response;
                case "release":
                    return ProgrammableServiceActionType.Release;
                case "timeout":
                    return ProgrammableServiceActionType.Timeout;

                default:
                    return ProgrammableServiceActionType.Unknown;
            }

        }
    }
}