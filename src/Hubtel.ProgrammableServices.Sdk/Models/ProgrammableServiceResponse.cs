using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public class ProgrammableServiceResponse
    {
        public ProgrammableServiceResponse()
        {
            Item = new ProgrammableServicesResponseCartData(string.Empty, 0, decimal.Zero);
            Data = new List<ProgrammableServicesResponseData>();

        }
    
        public string Type { get; set; }
        public string Message { get; set; }
        public string ClientState { get; set; }

        public string Label { get; set; }
        public string DataType { get; set; }
        public string FieldType { get; set; }
        public string FieldName { get; set; }
        public bool PersistAsFavoriteStep { get; set; }

        public ProgrammableServicesResponseCartData Item { get; set; }

        public LegacyAddToCart AddToCart => Item==null?null: new LegacyAddToCart
            {Amount = Item.Price, Description = Item.ItemName, ItemId = Item.ItemId, ServiceData = Item.ServiceData};
        public List<ProgrammableServicesResponseData> Data { get; set; }

        [JsonIgnore]
        internal string NextRoute { get; private set; }
        [JsonIgnore]
        internal bool IsRelease => string.IsNullOrWhiteSpace(NextRoute);
        [JsonIgnore]
        internal bool IsRedirect { get; private set; }

        [JsonIgnore]
        internal Exception Exception { get; private set; }


        #region Internal methods => NOT EXPOSED TO USERS

        internal ProgrammableServiceResponse SetException(Exception exception)
        {
            
            return new ProgrammableServiceResponse()
            {
                Exception = exception,
                Message = exception.Message,
                Label = exception.Message
                
            };
        }

        internal static ProgrammableServiceResponse AddToCartForCheckout(string message, ProgrammableServicesResponseCartData data,bool isUssd)
        {
            return new ProgrammableServiceResponse()
            {
                Type =  isUssd? ProgrammableServiceActionType.Release.ToString("G"): ProgrammableServiceActionType.AddToCart.ToString("G"),
                Message = message,
                Item = data,
                DataType = ProgrammableServiceDataTypes.Display
            };
        }


        internal static ProgrammableServiceResponse Redirect(string nextRoute)
        {
            return new ProgrammableServiceResponse()
            {
                NextRoute = nextRoute,
                IsRedirect = true
            };
        }


        internal static ProgrammableServiceResponse Render(string message, List<ProgrammableServicesResponseData> dataItems = null, ProgrammableServicesResponseCartData cartItem = null, string label = null, string dataType = null, string fieldType = "text", string fieldName = "", bool persistAsFavoriteStep = false, string nextRoute = null)
        {
            var type = string.IsNullOrWhiteSpace(nextRoute)
                ? ProgrammableServiceActionType.Release.ToString()
                : ProgrammableServiceActionType.Response.ToString();
            return new ProgrammableServiceResponse()
            {
                Type = type,
                Message = message,
                NextRoute = nextRoute,
                Data = dataItems,
                DataType = dataType,
                Item = cartItem,
                Label = label,
                FieldType = fieldType,
                PersistAsFavoriteStep = persistAsFavoriteStep,
                FieldName = fieldName
            };
        }

        

        #endregion
    }
}