using System;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    /// <summary>
    /// This marks a method as the handler for all initiation requests to a controller
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HandleResumeSession : Attribute
    {

    }
}