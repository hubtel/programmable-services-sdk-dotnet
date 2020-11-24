using System.Threading.Tasks;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public interface IProgrammableServicesCallbackApi
    {
        Task<ProgrammableServicesCallbackApiResponse> ReportServiceFulfilmentStatus(string orderId,string sessionId,string transactionId, string description,string serviceStatus, object metaData);
    }
}