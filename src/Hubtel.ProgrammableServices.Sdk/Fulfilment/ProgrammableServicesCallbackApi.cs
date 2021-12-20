using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
 public class ProgrammableServicesCallbackApi: IProgrammableServicesCallbackApi
    {
        private readonly IPsCallbackHttpClient _client;
        private readonly ProgrammableServiceConfiguration _config;

        public ProgrammableServicesCallbackApi(IPsCallbackHttpClient client, ProgrammableServiceConfiguration config)
        {
            _client = client;
            _config = config;
        }

        public async Task<ProgrammableServicesCallbackApiResponse> ReportServiceFulfilmentStatus(string orderId, string sessionId, string transactionId,
            string description, string serviceStatus, object metaData)
        {
            var requestPayload = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(_config.HubtelFulfilmentApiEndpoint))
                {
                    throw new ArgumentNullException(nameof(_config.HubtelFulfilmentApiEndpoint));
                }
                requestPayload = JsonConvert.SerializeObject(new
                {
                    sessionId,
                    orderId,
                    transactionId,
                    description,
                    serviceStatus,
                    metaData
                });
                var content = new StringContent(requestPayload, Encoding.UTF8, "application/json");


                var httpResp = await _client.Client.PostAsync(new Uri(_config.HubtelFulfilmentApiEndpoint), content);


                var resp = await httpResp.Content.ReadAsStringAsync();

                return new ProgrammableServicesCallbackApiResponse
                {
                    Successful = httpResp.IsSuccessStatusCode,
                    RequestPayload = requestPayload,
                    ResponsePayload = resp
                };

            }
            catch (Exception e)
            {
                return new ProgrammableServicesCallbackApiResponse
                {
                    RequestPayload = requestPayload,
                    ResponsePayload = e.Message
                };
            }
        }
    }
}
