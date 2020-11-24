using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.InteractionElements
{
    /// <summary>
    ///  input.
    /// </summary>
    public class Input
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
      public List<InputOption> Options { get; set; }
        public bool HasOptions { get { return !(Options == null || Options.Count == 0); } }

        public Input()
        {
            Options = new List<InputOption>();
        }

        /// <summary>
        /// New  input.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static Input New(string name, string displayName = null)
        {
            var input = new Input
            {
                Name = name,
                DisplayName = displayName,
            };
            return input;
        }

        /// <summary>
        /// Add possible input option to  input.
        /// Makes input a selection based input.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="displayValue"></param>
        /// <returns></returns>
        public Input Option(string value, string displayValue = null)
        {
            Options.Add(InputOption.New(value, displayValue));
            return this;
        }
    }

    public class InputOption
    {
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public string Index { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public InputOption()
        {
        }

        public static InputOption New(string value, string displayValue = null,
            string index = null, string controller = null, string action = null)
        {
            var option = new InputOption
            {
                Value = value,
                DisplayValue = displayValue,
                Index = index,
                Controller = controller,
                Action = action
            };

            return option;
        }
    }
}