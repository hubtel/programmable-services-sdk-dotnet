using System;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Models;

namespace Hubtel.ProgrammableServices.Sdk.UnitTests
{
    public class Example1Controller : ProgrammableServiceControllerBase
    {
        public Example1Controller()
        {
            
        }

        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Index()
        {
            await Task.Delay(0);
            return Render($"Hello world. The time is {DateTime.UtcNow}");
        }


    }
}