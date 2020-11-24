
# Getting Started with Programmable Services SDK

This SDK helps you to build apps that are accessible on Hubtel's platform channels (Web, USSD and Hubtel mobile apps). It supercedes the current USSD Framework (https://github.com/hubtel/hubtel-ussd-csharp). Here are a few advantages it offers:
- Improved response times: Reflected instances are cached at startup and served faster
- Dependency Injection: Controllers can now leverage .NET Core's DI pattern. No need to attempt to create custom DI containers
- Unified experience for USSD, Web, POS and mobile apps. JSON response from this SDK contains properties that are actually needed by Hubtel's Programmable Services API. No more "NextRoute", "IsRelease" properties hanging around.
- Simplified programming pattern. A few lines of code can get you up and running.
 
In order to get started with Hubtel.ProgrammableServices.Sdk, do the following activities
### Install-Package Hubtel.ProgrammableServices.Sdk

    Install-Package Hubtel.ProgrammableServices.Sdk

                   
### Add to IServiceCollection in Program.cs or Startup.cs
``` c#
     // in ConfigureServices method
 services.AddHubtelProgrammableServices((p) =>
           {
                return new ProgrammableServiceConfiguration
                {
                    Storage = new DefaultProgrammableServiceStorage()
                };
            });

//in Configure method (if you want to ignore MVC controllers, else ignore this line)
 //app.UseHubtelProgrammableService("/myapp", nameof(TestController));


 //sample controller
  public class TestController : ProgrammableServiceControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        
       
        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Start()
        {
            return await RenderResponse("hello world!");
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
A POST request is issued to your application from Hubtel when a user wants to interact with your service. Your application is expected to respond promptly (<= 7s for USSD applications)

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

//SDK takes this request, parses it, finds method marked as HandleInitiation, and invoke it for a response. Below is sample response:

{
  "type": "Response",
  "message": "Enter mobile number:\r\n",
  "clientState": null,
  "label": "Enter mobile number",
  "dataType": "input",
  "fieldType": "phone",
  "fieldName": "mobileNumber",
  "persistAsFavoriteStep": true,
  "item": null,
  "addToCart": null,
  "data": null
}
```
Many in-built features including pagination, data store (default is in-memory dictionary), interaction elements (such as Menu, Form and Input) can help you to get up to speed.

Refer to https://developers.hubtel.com/docs/general-services-1 for more information
