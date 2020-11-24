using System;
using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class ProgrammableServiceControllerInfo
    {
        public ProgrammableServiceControllerInfo()
        {
            Methods = new Dictionary<string, ProgammableServicesMethodInfo>(StringComparer.OrdinalIgnoreCase);
        }
        public Dictionary<string, ProgammableServicesMethodInfo> Methods { get; set; }
        public Type TheType { get; set; }
    }
}