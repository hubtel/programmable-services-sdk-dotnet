namespace Hubtel.ProgrammableServices.Sdk.Models
{
    /// <summary>
    /// Presents properties that are used when displaying a collection to a user
    /// </summary>
    public class ProgrammableServicesResponseData
    {

        public ProgrammableServicesResponseData(string display, string value, decimal amount = decimal.Zero)
        {
            Display = display;
            Value = value;
            Amount = amount;
        }

        /// <summary>
        /// The friendly name of an item in the collection
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// The underlying value
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// The price tag. Usually, a user is presented with items that include a certain price tag.
        /// </summary>
        public decimal Amount { get; }
    }
}