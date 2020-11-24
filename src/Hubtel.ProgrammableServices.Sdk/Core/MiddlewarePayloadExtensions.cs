using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public static class MiddlewarePayloadExtensions
    {
        public static async Task<string> ReadRequestStreamAsStringAsync(this HttpRequest request)
        {
            var requestPayload = string.Empty;
            try
            {
              
                using (var reader = new StreamReader(request.Body))
                {
                    requestPayload = await reader.ReadToEndAsync();

                    // Do something
                    
                }

                // More code
                request.Body.Seek(0, SeekOrigin.Begin);

            }
            catch (Exception e)
            {

            }

            return requestPayload;
        }



        public static async Task<string> ReadResponseStreamAsStringAsync(this HttpResponse response)
        {
           
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return text;
        }

    }
}