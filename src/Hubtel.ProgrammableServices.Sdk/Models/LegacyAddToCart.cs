using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.Models
{
    /// <summary>
    /// Backward-compatible with old USSD Service from Hubtel
    /// </summary>
    public class LegacyAddToCart
    {
        public LegacyAddToCart()
        {
            ServiceData = new Dictionary<string, string>();
        }

        public LegacyAddToCart(decimal amount, string itemId, string description, Dictionary<string, string> serviceData)
        {
            Amount = amount;
            ItemId = itemId;
            Description = description;
            ServiceData = serviceData;
        }
        public decimal Amount { get; set; }
        public string ItemId { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> ServiceData { get; set; }
    }
}