using System;
using Hubtel.ProgrammableServices.Sdk.Core;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public static class ProgrammableServiceRequestExtensions
    {
        public static ProgrammableServiceActionType GetActionType(this ProgrammableServiceRequest request)
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
                case "query":
                    return ProgrammableServiceActionType.Query;
                case "favorite":
                    return ProgrammableServiceActionType.Favorite;
                default:
                    return ProgrammableServiceActionType.Unknown;
            }
            
        }

        /// <summary>
        /// Returns true or otherwise if request is detected to be from USSD source
        /// </summary>
        /// <param name="request"></param>
        /// <returns>True if request is detected to be from USSD source</returns>
        public static bool IsUssd(this ProgrammableServiceRequest request)
        {
            if ("ussd".Equals(request.Platform,StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return
                "glo".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "glo-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "mtn".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "mtn-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "tigo".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "tigo-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "airtel".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "airtel-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "vodafone".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "vodafone-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "airteltigo".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "airteltigo-gh".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "safaricom".Equals(request.Operator,StringComparison.OrdinalIgnoreCase)
                || "safaricom-ke".Equals(request.Operator,StringComparison.OrdinalIgnoreCase);
        }


        
    }
}