using System.Linq;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public static class ProgrammableServiceFulfilmentRequestExtensions
    {
        public static bool IsDirectApiCall(this ProgrammableServiceFulfilmentRequest message)
        {
            return message.ExtraData.Any() && string.IsNullOrEmpty(message.SessionId);
        }


        public static bool IsRecurringServiceCall(this ProgrammableServiceFulfilmentRequest message)
        {
            return message.OrderInfo?.IsRecurring ?? false;
        }
    }
}