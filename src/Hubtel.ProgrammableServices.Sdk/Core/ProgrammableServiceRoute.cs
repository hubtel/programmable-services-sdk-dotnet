using System;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    internal class ProgrammableServiceRoute
    {
        private const string ControllerSuffix = "Controller";
        public string ControllerName { get; set; }

        public string FullControllerName =>
            ControllerName.EndsWith(ControllerSuffix, StringComparison.OrdinalIgnoreCase)
                ? ControllerName
                : $"{ControllerName}{ControllerSuffix}";
        public string ActionName { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
        public static ProgrammableServiceRoute FromJson(string json)
        {
            return JsonConvert.DeserializeObject<ProgrammableServiceRoute>(json);
        }
    }
}