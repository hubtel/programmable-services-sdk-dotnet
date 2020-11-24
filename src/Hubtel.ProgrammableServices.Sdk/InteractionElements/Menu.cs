using System;
using System.Collections.Generic;

namespace Hubtel.ProgrammableServices.Sdk.InteractionElements
{
    /// <summary>
    ///  menu to redirect to appropriate choice.
    /// </summary>
    public class Menu
    {
        public string Header { get; set; }
        public string Footer { get; set; }
        public List<MenuItem> Items { get; set; }
        public MenuItem ZeroItem { get; set; }

        public Menu() : this(null, null)
        {
        }

        private Menu(string header, string footer)
        {
            Header = header;
            Footer = footer;
            Items = new List<MenuItem>();
            ZeroItem = null;
        }

        /// <summary>
        /// Create a new  menu.
        /// </summary>
        /// <param name="header">Displayed before menu items.</param>
        /// <param name="footer">Displayed after menu items.</param>
        /// <returns></returns>
        public static Menu New(string header = null, string footer = null)
        {
            return new Menu(header, footer);
        }

        /// <summary>
        /// Add item to menu items.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Menu AddItem(string display, string action, string controller = null)
        {
            Items.Add(new MenuItem()
            {
                Display = display,
                Action = action,
                Controller = controller,
            });
            return this;
        }

        /// <summary>
        /// Add an item to be rendered as last 0 option on menu.
        /// </summary>
        /// <param name="display"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public Menu AddZeroItem(string display, string action, string controller = null)
        {
            ZeroItem = new MenuItem()
            {
                Display = display,
                Action = action,
                Controller = controller,
            };
            return this;
        }

        /// <summary>
        /// Render the menu to be displayed.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            var display = string.Empty;
            if (!string.IsNullOrWhiteSpace(Header))
            {
                display += Header + Environment.NewLine;
            }
            for (int i = 0; i < Items.Count; i++)
            {
                display += string.Format("{0}. {1}" + Environment.NewLine,
                    i + 1, Items[i].Display);
            }
            if (ZeroItem != null)
            {
                display += string.Format("0. {0}" + Environment.NewLine,
                    ZeroItem.Display);
            }
            if (!string.IsNullOrWhiteSpace(Footer))
            {
                display += Footer;
            }
            return display;
        }
    }

    public class MenuItem
    {
        public string Display { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
    }
}