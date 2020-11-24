using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.InteractionElements;
using Hubtel.ProgrammableServices.Sdk.Models;

namespace Hubtel.ProgrammableServices.Sdk.UnitTests
{
    public class Example2Controller : ProgrammableServiceControllerBase
    {
        public Example2Controller()
        {

        }

        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Index()
        {
            var form = Form.New("", $"{nameof(GreetMe)}");
            form.AddInput(Input.New("name", "Tell us your name :)"));
            return await RenderForm(form, null, null, "Tell us your name :)", ProgrammableServiceDataTypes.Input);
        }


        public async Task<ProgrammableServiceResponse> GreetMe()
        {
            var name = FormData["name"];

            await Task.Delay(0);
            return Render($"your name is {name}");
        }


    }
}