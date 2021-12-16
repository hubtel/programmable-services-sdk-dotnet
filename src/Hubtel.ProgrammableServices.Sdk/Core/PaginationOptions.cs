namespace Hubtel.ProgrammableServices.Sdk.Core
{
    public sealed class PaginationOptions
    {
        public string Title { get; set; } = "Select";
        public int PageCount { get; set; } = 3;
        public string NextPageKey { get; set; } = "9";
        public string NextPageDisplayText { get; set; } = "More";
        public string PreviousPageKey { get; set; } = "8";
        public string PreviousPageDisplayText { get; set; } = "Back";

        public bool UseDefaultNumberListing { get; set; } = true;

    }

    public class PaginationElement
    {
        public string Key { get; set; }
        public string Display { get; set; }
        public string NumberingIndex { get; set; }
    }
}