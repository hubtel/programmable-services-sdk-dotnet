using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hubtel.ProgrammableServices.Sdk.UnitTests
{
    public class HubtelProgrammableServicesShould:IClassFixture<Difixture>
    {

        private readonly IServiceProvider serviceProvider;
        public HubtelProgrammableServicesShould(Difixture fixture)
        {
            serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public void Throw_Exception_When_No_ControllerName_Is_Specified()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();

            //act and assert

            Assert.ThrowsAsync<ArgumentException>(async () => await service.ExecuteInteraction(new ProgrammableServiceRequest(), ""));
        }


        [Fact]
        public async Task Provide_Response_With_Release_Type_When_Controller_Is_Not_Found()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();

            //act
            var resp = await service.ExecuteInteraction(new ProgrammableServiceRequest(),"Unknown");
            
            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }

        [Fact]
        public void Throw_Exception_When_No_Request_Is_Null()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
          
            //act and assert
            
            Assert.ThrowsAsync<ArgumentException>(async () => await service.ExecuteInteraction(null, nameof(Example1Controller)));
        }

        [Fact]
        public async Task Provide_Response_With_Release_Type_When_Request_Type_Is_Unknown()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var request = new ProgrammableServiceRequest
            {
                Type = "blah blah"
            };
            //act
            var resp = await service.ExecuteInteraction(request, nameof(Example1Controller));

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }


        [Fact]
        public async Task Provide_Response_With_Release_Type_When_Request_Type_Is_Timeout_From_Hubtel()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var request = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Timeout.ToString("G")
            };
            //act
            var resp = await service.ExecuteInteraction(request, nameof(Example1Controller));

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }

        [Fact]
        public async Task Provide_Response_With_Release_Type_When_ActionMethod_Is_Unknown()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var request = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Initiation.ToString("G"),
                Message = "",
                Operator = "Hubtel-Android",
                Mobile = "233249441409",
                SessionId = Guid.NewGuid().ToString("N")
            };
            //act
            var resp = await service.ExecuteInteraction(request, nameof(Example1Controller), "Boom");

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }

        [Fact]
        public async Task Provide_Response_With_Release_Type_From_Index_Method_On_SampleController()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var request = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Initiation.ToString("G"),
                Message = "",
                Operator = "Hubtel-Android",
                Mobile = "233249441409",
                SessionId = Guid.NewGuid().ToString("N")
            };
            //act
            var resp = await service.ExecuteInteraction(request, nameof(Example1Controller),
                nameof(Example1Controller.Index));

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }

        [Fact]
        public async Task Provide_Response_With_Release_Type_When_SessionId_Is_Not_Found_For_Initial_Response_Request()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var request = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Response.ToString("G"), //the test case
                Message = "",
                Operator = "Hubtel-Android",
                Mobile = "233249441409",
                SessionId = Guid.NewGuid().ToString("N")
            };
            //act
            var resp = await service.ExecuteInteraction(request, nameof(Example1Controller),
                nameof(Example1Controller.Index));

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }


        [Fact]
        public async Task Provide_Response_With_Release_Type_When_SessionId_Is_Not_Found_For_Any_Response_Request()
        {
            //arrange
            var service = serviceProvider.GetService<IHubtelProgrammableService>();
            var sessionIdFromHubtel = Guid.NewGuid().ToString("N");
            var request = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Initiation.ToString("G"),
                Message = "",
                Operator = "Hubtel-Android",
                Mobile = "233249441409",
                SessionId = sessionIdFromHubtel
            };
           


            //arrange again
            var request2 = new ProgrammableServiceRequest
            {
                Type = ProgrammableServiceActionType.Response.ToString("G"),
                Message = "Augustine", //the expected response
                Operator = "Hubtel-Android",
                Mobile = "233249441409",
                SessionId = Guid.NewGuid().ToString("") //perhaps a wrong call from Hubtel???
            };

            //act
            
            var resp = await service.ExecuteInteraction(request, nameof(Example2Controller),
                nameof(Example2Controller.Index));

            resp = await service.ExecuteInteraction(request2, nameof(Example2Controller));

            resp.Type.Should().Be(ProgrammableServiceActionType.Release.ToString("G"));
        }



    }
}
