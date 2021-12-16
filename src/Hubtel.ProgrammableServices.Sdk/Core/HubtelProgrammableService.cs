using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public class HubtelProgrammableService: IHubtelProgrammableService
    {
        private readonly ProgrammableServiceConfiguration _configuration;
        
        private readonly ProgrammableServicesControllerActivator _controllerActivator;
        private readonly IServiceProvider _serviceProvider;


        public HubtelProgrammableService(ProgrammableServiceConfiguration configuration
        
            ,IServiceProvider serviceProvider
            ,ProgrammableServicesControllerActivator controllerActivator
            )
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
            else if (context.Request.GetActionType()== ProgrammableServiceActionType.Query || context.Request.GetActionType()== ProgrammableServiceActionType.Favorite)
            {
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

             
                var sessionResumeKey = $"_sessionResume.{context.Request.Mobile}";

                ProgrammableServiceResponse response;
                if (context.Request.GetActionType()==ProgrammableServiceActionType.Initiation)
                {
                    if (_configuration.EnableResumeSession)
                    {
                        //check if previous session exists
                        var sessionResumeValStr = await context.Store.Get(sessionResumeKey);
                        var sessionResumeList =
                            JsonConvert.DeserializeObject<List<ResumeSessionInfo>>(sessionResumeValStr);

                        if (await context.Store.Exists(sessionResumeKey) && string.Equals(controllerInfo.TheType.Name,sessionResumeList.LastOrDefault()?.ControllerName) )
                        {
                            //if the previous last entry  is an initiation session...then just continue....
                            if (sessionResumeList.LastOrDefault().UserRequest.GetActionType()== ProgrammableServiceActionType.Initiation)
                            {
                                response = await (Task<ProgrammableServiceResponse>)methodInfo.Invoke(controller, args);
                            }
                            else
                            {
                                var previousSessionId = sessionResumeList.FirstOrDefault().UserRequest.SessionId;
                            
                            
                 
                                var keys = await context.Store.GetKeys();


                                var keysWithPreviousSessionId = keys.Where(k => k.StartsWith(previousSessionId)).ToList();
                                //copy everything from the previous session to the new session
                                foreach (var key in keysWithPreviousSessionId)
                                {
                               
                                    var previousValue = await context.Store.Get(key);
                                    await 
                                        context.Store.Set(key.Replace(previousSessionId, context.Request.SessionId),
                                            previousValue);
                              
                                }
                                controller.FormData = await controller.GetFormData(); //important step, if not, FormData is always NULL
                            
                            
                                var sessionResumeMethod = controllerInfo.Methods.Values.FirstOrDefault(m => m.IsSessionResumeMethod);

                                if (sessionResumeMethod==null)
                                {
                                    throw new Exception($"no method found that has HandleResumeSession attribute");
                                }
                            
                                await context.Store.Delete(sessionResumeKey); //so we don't have chaff data for this user
                            
                                response = await (Task<ProgrammableServiceResponse>)sessionResumeMethod.Method.Invoke(controller,new []{sessionResumeList});


                            }
                       

                        }
                        else
                        {
                            //just call the initiation handler...
                            response = await (Task<ProgrammableServiceResponse>)methodInfo.Invoke(controller, args);
                        }
                        
                        
                    }
                    else
                    {
                        response = await (Task<ProgrammableServiceResponse>)methodInfo.Invoke(controller, args);

                    }
                  
                }
                else
                {
                    response = await (Task<ProgrammableServiceResponse>)methodInfo.Invoke(controller, args);

                }
                
               

                if (!response.IsRelease)
                {
                    if (!response.IsRedirect)
                    {
                        if (_configuration.EnableResumeSession)
                        {
                            List<ResumeSessionInfo> sessionResumeList;
                     
                            if (await context.Store.Exists(sessionResumeKey))
                            {
                                var sessionResumeValStr = await context.Store.Get(sessionResumeKey);
                                sessionResumeList =
                                    JsonConvert.DeserializeObject<List<ResumeSessionInfo>>(sessionResumeValStr);
                            
                                sessionResumeList.Add(new ResumeSessionInfo
                                {
                                    UserRequest = context.Request,
                                    ServiceResponse = response,
                                    MethodName = methodInfo.Name,
                                    ControllerName = controllerInfo.TheType.Name
                                });
                            }
                            else
                            {
                                sessionResumeList = new List<ResumeSessionInfo> {new ResumeSessionInfo
                                {
                                    UserRequest = context.Request,
                                    ServiceResponse = response,
                                    MethodName = methodInfo.Name,
                                    ControllerName = controllerInfo.TheType.Name
                                }};
                            }

                            await context.Store.Set(sessionResumeKey, JsonConvert.SerializeObject(sessionResumeList));

                        }
                    
                        
                    }
                    await context.Store.Set(context.NextRouteKey, response.NextRoute);
                }
                else
                {
                    if (_configuration.EnableResumeSession)
                    {
                        await context.Store.Delete(sessionResumeKey);
                    }
                }

                if (response.IsRedirect)
                {
                    continue;
                }

                return response;
            }
        }
    }

    public class ResumeSessionInfo
    {
        public ProgrammableServiceRequest UserRequest { get; set; }
        public ProgrammableServiceResponse ServiceResponse { get; set; }
        public string MethodName { get; set; }
        public string ControllerName { get; set; }
    }
}