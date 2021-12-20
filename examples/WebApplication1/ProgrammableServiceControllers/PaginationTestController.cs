using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.InteractionElements;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace ProgrammableServicesSampleApp.ProgrammableServiceControllers
{
    public class PaginationTestController : ProgrammableServiceControllerBase
    {
        private readonly ILogger<PaginationTestController> _logger;

        public PaginationTestController(ILogger<PaginationTestController> logger)
        {
            _logger = logger;
        }

       // [HandleInitiation]
        public async Task<ProgrammableServiceResponse> TestWebCollection()
        {


            var collection = new Dictionary<string, string>
            {
                {"iPhone 6s", "iphone_6_s"}, //take note: the value could also be a JSON
                {"iPhone 7", "iphone_7"},
                {"iPhone 7+", "iphone_7_p"},
                {"iPhone 8", "iphone_8"},
                {"iPhone X", "iphone_10"},
                {"iPhone Xs", "iphone_10_s"},
                {"iPhone 11", "iphone_11"},
                {"iPhone 11 Pro", "iphone_11_pro"},
                {"Apple Watch Series 3", "iwatch_3"},
                {"Apple Watch Series 4", "iwatch_4"},
                {"Apple Watch Series 5", "iwatch_5"},
                {"Macbook 2019 model", "macbook"},
            };
            FormData = new Dictionary<string, string>();
            var form = new Form
            {
                Title = "Select Bundle",
                Action = nameof(Confirmation),
                Data = FormData,
            };
            form.AddInput(Input.New("bundle-chosen", "Select Bundle"));
            return await RenderForm(form,
                collection.Select(b => new ProgrammableServicesResponseData($"{b.Key}", b.Value, decimal.Zero))
                    .ToList(), null, "Select Bundle", ProgrammableServiceDataTypes.Select);
        }

        public Task<ProgrammableServiceResponse> Confirmation()
        {
            var value = FormData["bundle-chosen"];
            return Task.FromResult(Render($"you selected {value}"));
        }

        
        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Start()
        {
            var collection = new Dictionary<string, string>
            {
                {"iPhone 6s", "iphone_6_s"}, //take note: the value could also be a JSON
                {"iPhone 7", "iphone_7"},
                {"iPhone 7+", "iphone_7_p"},
                {"iPhone 8", "iphone_8"},
                {"iPhone X", "iphone_10"},
                {"iPhone Xs", "iphone_10_s"},
                {"iPhone 11", "iphone_11"},
                {"iPhone 11 Pro", "iphone_11_pro"},
                {"Apple Watch Series 3", "iwatch_3"},
                {"Apple Watch Series 4", "iwatch_4"},
                {"Apple Watch Series 5", "iwatch_5"},
                {"Macbook 2019 model", "macbook"}
            };

            return await PaginateForUssd(collection,
                paginationOptions: new PaginationOptions
                {
                    PageCount = 6,
                    NextPageKey = "99",
                    PreviousPageKey = "00",
                    NextPageDisplayText = "MORE",
                    PreviousPageDisplayText = "BACK",
                    UseDefaultNumberListing = true 
                });
        }

        public override Task<ProgrammableServiceResponse> OnPaginatedItemSelected(string value)
        {
            return Task.FromResult(Render($"you selected {value}"));
        }

        
    }
}