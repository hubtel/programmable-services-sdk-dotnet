using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.Fulfilment;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Hubtel.ProgrammableServices.Sdk.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers Hubtel's Programmable Services SDK core into this DI container
        /// </summary>
        /// <param name="services">Must be valid and not null</param>
        /// <param name="configurationFactory">A function that returns ProgrammableServiceConfiguration, given an instance of IServiceProvider as input</param>
        public static void AddHubtelProgrammableServices(this IServiceCollection services, Func<IServiceProvider, ProgrammableServiceConfiguration> configurationFactory)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            services.AddSingleton(configurationFactory);
            services.RegisterProgrammableServicesControllers();
            services.AddHttpClient<IPsCallbackHttpClient, PsCallbackHttpClient>()
                .ConfigurePrimaryHttpMessageHandler(handler =>

                new HttpClientHandler
                {

                    CheckCertificateRevocationList = false,

                    ServerCertificateCustomValidationCallback = delegate { return true; },
                });
            services.AddSingleton<IProgrammableServicesCallbackApi, ProgrammableServicesCallbackApi>();
            services.AddSingleton<IHubtelProgrammableService, HubtelProgrammableService>();
     
        }

        
        private static void RegisterProgrammableServicesControllers(this IServiceCollection services)
        {
           var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var controllers = new List<Type>();
            foreach (var assembly in assemblies)
            {
                Type[] allTypes = null;

                try
                {
                    allTypes = assembly.GetTypes();
                }
                catch (Exception exception)
                {
                    
                    allTypes = null;
                }

                if (allTypes != null)
                {
                    foreach (var type in allTypes)
                    {
                        if (IsController(type.GetTypeInfo())
                        )
                        {
                            controllers.Add(type);
                        }
                    }

                }
            }

            if (!controllers.Any())
            {
                throw new Exception("At least one subclass of ProgrammableServiceControllerBase should be in your project or the class must have \"Controller\" suffix :|");
            }
            var col = new ConcurrentDictionary<string, ProgrammableServiceControllerInfo>();
            foreach (var controller in controllers)
            {
                //find all methods and include information as to whether it has HandleInitiation attribute

                var methods = controller.GetMethods( BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                var methodInfos = new Dictionary<string,ProgammableServicesMethodInfo>();
                foreach (var methodInfo in methods)
                {
                    if (methodInfo.ReturnType == typeof(Task<ProgrammableServiceResponse>))
                    {
                        if (methodInfo.GetCustomAttributes(typeof(HandleInitiation), true).FirstOrDefault() is HandleInitiation handleInitiationAttr)
                        {
                            methodInfos[methodInfo.Name] = new ProgammableServicesMethodInfo
                            {
                                IsInitiationMethod = true,
                                Method = methodInfo
                            };
                            continue;
                        }
                        methodInfos[methodInfo.Name] = new ProgammableServicesMethodInfo
                        {
                            
                            Method = methodInfo
                        };
                    }

                }

                col[controller.Name] = new ProgrammableServiceControllerInfo
                {
                    Methods  = methodInfos,
                    TheType = controller
                };

                services.TryAddTransient(controller, controller);
                services.TryAddSingleton(new ProgrammableServicesControllerActivator(col));
            }


          
        }

        private const string ControllerTypeNameSuffix = "Controller";

        private static bool IsController(TypeInfo typeInfo)
        {
            if (!typeInfo.IsClass)
            {
                return false;
            }

            if (typeInfo.IsAbstract)
            {
                return false;
            }

            // We only consider public top-level classes as controllers. IsPublic returns false for nested
            // classes, regardless of visibility modifiers
            if (!typeInfo.IsPublic)
            {
                return false;
            }

            if (typeInfo.ContainsGenericParameters)
            {
                return false;
            }

           
            if (!typeInfo.IsSubclassOf(typeof(ProgrammableServiceControllerBase)))
            {
                return false;
            }

            if (!typeInfo.Name.EndsWith(ControllerTypeNameSuffix, StringComparison.OrdinalIgnoreCase)
                )
            {
                return false;
            }

            return true;
        }
    }
}