using System;
using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    /// <summary>
    /// Presents properties used to hold information about a cart that may be ready for checkout
    /// </summary>
    public class ProgrammableServicesResponseCartData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemName">Friendly label to be displayed to the user</param>
        /// <param name="qty">Must be valid and not null</param>
        /// <param name="price">Must be valid and not null</param>
        /// <param name="itemId">The programmable service Id.
        /// Do note that this is only provided for backward compatibility with older USSD service from Hubtel.
        /// This parameter is fully optional because the new USSD service automatically determines the service ID.
        /// However, to supply this argument correctly, visit https://app-site.hubtel.com/products/services.
        /// Click on your service and copy the service ID here.</param>
        public ProgrammableServicesResponseCartData(string itemName, int qty, decimal price,string itemId="")
        {
            ItemName = itemName;
            Qty = qty;
            Price = price;
            ItemId = itemId;
            ServiceData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        
        /// <summary>
        /// The diplay message that is presented to the user just before checkout is initiated
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// The quantity. Must be non-negative
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// The actual cost of the service
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// The service ID. Fully optional for services obeying the new Programmable Services API.
        /// </summary>
        public string ItemId { get; }
        /// <summary>
        /// May be used to initiate a call to your fulfillment endpoint for Repeat Payments or direct API execution
        /// </summary>
        public Dictionary<string, string> ServiceData { get; set; }
    }
}