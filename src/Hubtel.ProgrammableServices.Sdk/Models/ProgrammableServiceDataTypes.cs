namespace Hubtel.ProgrammableServices.Sdk.Models
{
    public class ProgrammableServiceDataTypes
    {
        /// <summary>
        /// Signals calling client device to render a menu
        /// </summary>
        public const string Menu = "menu";
        /// <summary>
        /// Signals calling client device to render a drop-down UI
        /// </summary>
        public const string Select = "select";
        /// <summary>
        /// Signals calling client device to render a display
        /// </summary>
        public const string Display = "display";
        /// <summary>
        /// Signals calling client device to render an input
        /// </summary>
        public const string Input = "input";
        /// <summary>
        /// Signals calling client device to display a prompt with 2 options only
        /// </summary>
        public const string Confirm = "confirm";
    }
}