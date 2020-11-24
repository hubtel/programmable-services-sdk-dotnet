using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class HubtelProgrammableService: IHubtelProgrammableService
    {
        private readonly ProgrammableServiceConfiguration _configuration;
        
        private readonly ProgrammableServicesControllerActivator _controllerActivator;
        private readonly IServiceProvider _serviceProvider;


        public HubtelProgrammableService(ProgrammableServiceConfiguration configuration
        
            ,IServiceProvider serviceProvider
            ,ProgrammableServicesControllerActivator controllerActivator)
        {
            _configuration = configuration;
            
            _controllerActivator = controllerActivator;
            _serviceProvider = serviceProvider;
        }
        public async Task<ProgrammableServiceResponse> ExecuteInteraction(ProgrammableServiceRequest request, string controllerName, string actionName = "")
        {
            return await ExecuteInteractionCore(request, controllerName, actionName);
        }

        private async Task<ProgrammableServiceResponse> ExecuteInteractionCore(ProgrammableServiceRequest request, string controllerName
            , string actionName="")
        {

            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentNullException(nameof(controllerName));
            }
            controllerName = controllerName.EndsWith("controller", StringComparison.OrdinalIgnoreCase)
                ? controllerName
                : $"{controllerName}Controller";

            if (!_controllerActivator.ControllerCollection.ContainsKey(controllerName))
            {
                return new ProgrammableServiceResponse
                {
                    Message = $"Could not find {controllerName}",
                    Type = ProgrammableServiceActionType.Release.ToString("G"),
                    Label = $"Could not find {controllerName}",
                    DataType = ProgrammableServiceDataTypes.Display,
                    FieldType = ProgrammableServiceFieldTypes.Text
                };
            }


            if (request==null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var context = new ProgrammableServiceContextBuilder(request, _configuration).Build();

           
            if (context.Request.GetActionType()==ProgrammableServiceActionType.Unknown)
            {
                return new ProgrammableServiceResponse
                {
                    Message = "Request type could not be parsed",
                    Type = ProgrammableServiceActionType.Release.ToString("G"),
                    Label = "Request type could not be parsed",
                    DataType = ProgrammableServiceDataTypes.Display,
                    FieldType = ProgrammableServiceFieldTypes.Text
                };
            }

            if (context.Request.GetActionType() == ProgrammableServiceActionType.Timeout)
            {
                //we have to check if the user wants to explicitly ignore timeouts
                return new ProgrammableServiceResponse
                {
                    Message = "Timeout from gateway. Request has been ignored",
                    Type = ProgrammableServiceActionType.Release.ToString("G"),
                    Label = "Timeout from gateway. Request has been ignored",
                    DataType = ProgrammableServiceDataTypes.Display,
                    FieldType = ProgrammableServiceFieldTypes.Text
                };
            }

           var controllerInfo = _controllerActivator.ControllerCollection[controllerName];

            if (context.Request.GetActionType() == ProgrammableServiceActionType.Initiation)
            {
                //we ought to check if the action name is in the controller
                //if actionName is null, we find the method that has [HandleInitiation] attribute
                if (string.IsNullOrEmpty(actionName))
                {
                    
                    var intendedAction = controllerInfo.Methods.Values.FirstOrDefault(m => m.IsInitiationMethod);
                    if (intendedAction==null)
                    {
                        return new ProgrammableServiceResponse
                        {
                            Message = "no initiation handler",
                            Type = ProgrammableServiceActionType.Release.ToString("G"),
                            Label = "no initiation handler",
                            DataType = ProgrammableServiceDataTypes.Display,
                            FieldType = ProgrammableServiceFieldTypes.Text
                        };
                    }

                    actionName = intendedAction.Method.Name;
                    if (string.IsNullOrEmpty(actionName)) //perhaps the initiation handler attribute is not anywhere 
                    {
                        return new ProgrammableServiceResponse
                        {
                            Message = "no initiation handler",
                            Type = ProgrammableServiceActionType.Release.ToString("G"),
                            Label = "no initiation handler",
                            DataType = ProgrammableServiceDataTypes.Display,
                            FieldType = ProgrammableServiceFieldTypes.Text
                        };
                    }
                }
                else
                {
                    if (!controllerInfo.Methods.ContainsKey(actionName))
                    {
                        return new ProgrammableServiceResponse
                        {
                            Message = $"Initiation handler {actionName} does not exists",
                            Type = ProgrammableServiceActionType.Release.ToString("G"),
                            Label = $"Initiation handler {actionName} does not exists",
                            DataType = ProgrammableServiceDataTypes.Display,
                            FieldType = ProgrammableServiceFieldTypes.Text
                        };
                    }
                      
                }

                await context.Store.Delete(context.NextRouteKey);
                await context.Store.Delete(context.DataBagKey);

                await context.Store.Set(context.NextRouteKey, new ProgrammableServiceRoute
                {
                    ActionName = actionName,
                    ControllerName = controllerName
                }.ToJson());


            }

            try
            {
                return await OnResponse(context, controllerInfo);
            }
            catch (Exception e)
            {
                return new ProgrammableServiceResponse
                {
                    Message =e.Message,
                    Type = ProgrammableServiceActionType.Release.ToString("G"),
                    Label = e.Message,
                    DataType = ProgrammableServiceDataTypes.Display,
                    FieldType = ProgrammableServiceFieldTypes.Text
                };
            }
          
        }

       private async Task<ProgrammableServiceResponse> OnResponse(ProgrammableServiceContext context, ProgrammableServiceControllerInfo controllerInfo)
        {
            while (true)
            {
                var exists = await context.Store.Exists(context.NextRouteKey);
                if (!exists)
                {
                    throw new Exception("Session does not exist.");
                }

                var routeInfoJson = await context.Store.Get(context.NextRouteKey);
                var routeInfo = ProgrammableServiceRoute.FromJson(routeInfoJson);


                if (controllerInfo.TheType.Name.Equals(routeInfo.FullControllerName))
                {
                    if (!controllerInfo.Methods.ContainsKey(routeInfo.ActionName))
                    {
                        return new ProgrammableServiceResponse
                        {
                            Message = $"Action handler \"{routeInfo.ActionName}\" on controller \"{controllerInfo.TheType.Name}\" does not exist",
                            Type = ProgrammableServiceActionType.Release.ToString("G"),
                            Label = $"Action handler \"{routeInfo.ActionName}\" on controller \"{controllerInfo.TheType.Name}\" does not exist",
                            DataType = ProgrammableServiceDataTypes.Display,
                            FieldType = ProgrammableServiceFieldTypes.Text
                        };
                    }
                }

                else if (!_controllerActivator.ControllerCollection.ContainsKey(routeInfo.FullControllerName))
                {
                    return new ProgrammableServiceResponse
                    {
                        Message = $"Could not find controller \"{routeInfo.FullControllerName}\"",
                        Type = ProgrammableServiceActionType.Release.ToString("G"),
                        Label = $"Could not find controller \"{routeInfo.FullControllerName}\"",
                        DataType = ProgrammableServiceDataTypes.Display,
                        FieldType = ProgrammableServiceFieldTypes.Text
                    };
                }
                else
                {
                    controllerInfo = _controllerActivator.ControllerCollection[routeInfo.FullControllerName];

                    if (!controllerInfo.Methods.ContainsKey(routeInfo.ActionName))
                    {
                        return new ProgrammableServiceResponse
                        {
                            Message = $"Action handler \"{routeInfo.ActionName}\" on controller \"{controllerInfo.TheType.Name}\" does not exist",
                            Type = ProgrammableServiceActionType.Release.ToString("G"),
                            Label = $"Action handler \"{routeInfo.ActionName}\" on controller \"{controllerInfo.TheType.Name}\" does not exist",
                            DataType = ProgrammableServiceDataTypes.Display,
                            FieldType = ProgrammableServiceFieldTypes.Text
                        };
                    }
                }

              

                var controller =
                    (ProgrammableServiceControllerBase) _serviceProvider.GetRequiredService(controllerInfo.TheType);

                controller.Request = context.Request;
                controller.DataBag = context.DataBag;
               
                controller.Data = new Dictionary<string, string>();
                controller.FormData = await controller.GetFormData();
                var methodInfo = controllerInfo.Methods[routeInfo.ActionName].Method;
                object[] args = { };
                var response = await (Task<ProgrammableServiceResponse>)methodInfo.Invoke(controller, args);
               

                if (!response.IsRelease)
                {
                    await context.Store.Set(context.NextRouteKey, response.NextRoute);
                }

                if (response.IsRedirect)
                {
                    continue;
                }

                return response;
            }
        }
    }
}