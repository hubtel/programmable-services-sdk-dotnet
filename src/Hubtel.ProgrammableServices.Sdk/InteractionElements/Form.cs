using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.InteractionElements
{
    /// <summary>
    /// Setup a new form to collect a series of inputs.
    /// </summary>
    public class Form
    {
        public string Title { get; set; }
        public List<Input> Inputs { get; set; }
        public int ProcessingPosition { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public Form() : this(null, null, null)
        {
        }

        private Form(string title, string action, string controller)
        {
            Title = title;
            Inputs = new List<Input>();
            Action = action;
            Controller = controller;
            ProcessingPosition = 0;
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// New creates an instance of Form
        /// </summary>
        /// <param name="title"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static Form New(string title, string action, string controller = null)
        {
            return new Form(title, action, controller);
        }

        /// <summary>
        /// Add input field to form.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Form AddInput(Input input)
        {
            Inputs.Add(input);
            return this;
        }
    }
}