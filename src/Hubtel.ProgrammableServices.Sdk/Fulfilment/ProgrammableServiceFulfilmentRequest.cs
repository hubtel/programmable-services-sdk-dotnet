using System;
using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.Fulfilment
{
    public class ProgrammableServiceFulfilmentRequest
    {
        public ProgrammableServiceFulfilmentRequest()
        {
            ExtraData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            OrderInfo = new PaidOrderInfo();
        }

        public string OrderId { get; set; }
        public string SessionId { get; set; }
        public PaidOrderInfo OrderInfo { get; set; }
        public Dictionary<string, string> ExtraData { get; set; }

    }
}