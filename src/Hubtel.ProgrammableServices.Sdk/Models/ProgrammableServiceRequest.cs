using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public class ProgrammableServiceRequest
    {
        public ProgrammableServiceRequest()
        {
            ExtraData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public string SessionId { get; set; }
        public string ServiceCode { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// Android,iOS,mtn-gh,chrome-345t453424
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// Web,Hubtel-App,Hubtel-POS or USSD
        /// </summary>
        public string Platform { get; set; }
        public string Mobile { get; set; }

        public string ClientState { get; set; }
        public int Sequence { get; set; }
        
        public Dictionary<string,object> ExtraData { get; set; }

        [JsonIgnore]
        internal string TrimmedMessage
        {
            get { return Message.Trim(); }
        }
    }
}