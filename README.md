
# Getting Started with Programmable Services SDK
![.NET Core](https://github.com/hubtel/programmable-services-sdk-dotnet/workflows/.NET%20Core/badge.svg)

This SDK helps you to build apps that are accessible on Hubtel's platform channels (Web, USSD and Hubtel mobile apps). It supercedes the erstwhile USSD Framework (https://github.com/hubtel/hubtel-ussd-csharp), which has been discontinued on March 31st, 2021. Here are a few advantages it offers:
- Improved response times: Reflected instances are cached at startup and served faster
- Dependency Injection: Controllers can now leverage .NET Core's DI pattern. No need to attempt to create custom DI containers
- Unified experience for USSD, Web, POS and mobile apps. JSON response from this SDK contains properties that are actually needed by Hubtel's Programmable Services API. No more "NextRoute", "IsRelease" properties hanging around.
- Simplified programming pattern. A few lines of code can get you up and running.
 
In order to get started with Hubtel.ProgrammableServices.Sdk, create an empty ASP.NET Core Web Application project and follow these steps:

### 1) Install the Nuget Package

    Install-Package Hubtel.ProgrammableServices

                   
### 2) In Startup.cs do the following
``` c#
     // in ConfigureServices method
 services.AddHubtelProgrammableServices((p) =>
           {
                return new ProgrammableServiceConfiguration
                {
                    Storage = new DefaultProgrammableServiceStorage()
                };
            });



//in Configure(...) method 
//we have extension methods to do route-to-code, if you want to ignore controllers explicitly
 //app.UseHubtelProgrammableService("/myapp", nameof(TestController));
```

### 3) Add an empty Web API controller and fill in these lines of code. You might need to resolve some references

``` cs

   [Route("api/requests")]
    
    public class RequestController : Controller
    {
        
        private readonly ILogger<RequestController> _logger;
        private readonly IHubtelProgrammableService _programmableService;

        public RequestController(
            
             ILogger<RequestController> logger           
            , IHubtelProgrammableService programmableService)
        {
            
            _logger = logger;
            _programmableService = programmableService;
        }

        [Route("interactions")]
        [HttpPost]
        public async Task<IActionResult> HandleInteractions([FromBody] ProgrammableServiceRequest request)
        {
            _logger.LogDebug("received interaction request for {ps_interaction_request}",
                JsonConvert.SerializeObject(request));
           
            var response = await _programmableService.ExecuteInteraction(request, nameof(TestController));
            return Ok(response);
        }
    }

```

### 4) Create your ProgrammableService controller by adding a new file

```cs
 
    public class TestController : ProgrammableServiceControllerBase
    {
        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Start()
        {
            var menu = Menu.New("Welcome", Environment.NewLine + "by Hubtel")
                .AddItem("Greet me", nameof(GreetingForm))
                .AddItem("What's the time?", nameof(Time))
                .AddZeroItem("Exit", nameof(Exit));
            return await RenderMenu(menu, new List<ProgrammableServicesResponseData>
            {
                new ProgrammableServicesResponseData("Greet me", "1"),
                new ProgrammableServicesResponseData("What's the time?", "2"),
            }, null, "Welcome", ProgrammableServiceDataTypes.Menu);
        }


        public async Task<ProgrammableServiceResponse> GreetingForm()
        {
            return await EnterNameForm();
        } 
        
        public async Task<ProgrammableServiceResponse> EnterNameForm()
        {
            var form = Form.New("Name:", nameof(SelectGenderForm))
                .AddInput(Input.New("Name"));
               
            return await RenderForm(form, null, null, "Name", ProgrammableServiceDataTypes.Input,
                ProgrammableServiceFieldTypes.Text);
        }
        
        public async Task<ProgrammableServiceResponse> SelectGenderForm()
        {
            var form = Form.New("Gender", nameof(CompleteGreeting))
                
                .AddInput(Input.New("Gender")
                    .Option("M", "Male")
                    .Option("F", "Female"));
            form.Data = FormData; //carry on the previous form's data
            return await RenderForm(form, null, null, "Gender", ProgrammableServiceDataTypes.Select
                );
        }

        public async Task<ProgrammableServiceResponse> CompleteGreeting()
        {
            var hour = DateTime.UtcNow.Hour;
            var greeting = string.Empty;
            if (hour < 12)
            {
                greeting = "Good morning";
            }
            if (hour >= 12)
            {
                greeting = "Good afternoon";
            }
            if (hour >= 18)
            {
                greeting = "Good night";
            }
            var name = FormData["Name"];
            var prefix = FormData["Gender"] == "M" ? "Sir" : "Madam";
            await Task.Delay(0);
            return await RenderResponse($"{greeting}, {prefix} {name}!");
        }

        public async Task<ProgrammableServiceResponse> Time()
        {
            return await Task.FromResult(Render(string.Format("{0:t}", 
                DateTime.UtcNow)));
        }

        public async Task<ProgrammableServiceResponse> Exit()
        {
            return await Task.FromResult(Render("Bye bye!"));
        } 
        
        private async Task<ProgrammableServiceResponse> RenderResponse(string response)
        {
            var resp = Render(response, null, null);
            // setup rich ux for web and mobile
            resp.Label = response;
            resp.DataType = ProgrammableServiceDataTypes.Display;
            return await Task.FromResult(resp);
        }
    }
```

### What does the SDK really do?
A POST request is issued to your application (/api/request/interactions) from Hubtel when a user wants to interact with your service (either by dialling a USSD code or from Hubtel's web or mobile apps). 
Your application is expected to respond promptly (<= 5s for USSD applications)

``` json
//sample request from Hubtel
{
  "Mobile":"233244234546", 
  "SessionId":"234567ityjhgwe654765utrye",  
  "ServiceCode":"7c920dc49b7a40588c657119ce599044", 
  "Type":"initiation", 
  "Message":"*711*23#", 
  "Operator":"mtn",
  "Platform":"USSD"
}

SDK takes this request, parses it, finds method marked as HandleInitiation, and invoke it for a response. The SDK takes care of routing in subsequent requests.
Below is sample response created by the SDK:

{
    "type": "Response",
    "message": "Welcome\n1. Greet me\n2. What's the time?\n0. Exit\n\nby Hubtel",
    "clientState": null,
    "label": "Welcome",
    "dataType": "menu",
    "fieldType": "",
    "fieldName": "",
    "persistAsFavoriteStep": false,
    "item": null,
    "addToCart": null,
    "data": [
        {
            "display": "Greet me",
            "value": "1",
            "amount": 0
        },
        {
            "display": "What's the time?",
            "value": "2",
            "amount": 0
        }
    ]
}
```
Many in-built features including pagination, data store (default is in-memory dictionary), intelligent intra-controller or inter-controller routing, interaction elements (such as Menu, Form and Input) can help you to get up to speed.

Refer to https://developers.hubtel.com/docs/general-services-1 for more information
