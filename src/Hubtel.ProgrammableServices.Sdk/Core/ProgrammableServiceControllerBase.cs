using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.InteractionElements;
using Hubtel.ProgrammableServices.Sdk.Models;
using Newtonsoft.Json;

namespace Hubtel.ProgrammableServices.Sdk.Core
{
    /// <summary>
    /// The base controller to be inherited by every Programmable Service.
    /// The idea is to mimic the behaviour of MVC Controllers.
    /// The subclasses are transiently instantiated at runtime
    /// </summary>
    public class ProgrammableServiceControllerBase : IDisposable
    {
        private const string MenuProcessorDataKey = "MenuProcessorData";
        private const string FormProcessorDataKey = "FormProcessorData";
        private const string FormDataKey = "FormData";

        public ProgrammableServiceRequest Request { get; set; }
        public ProgrammableServiceDataBag DataBag { get; set; }

        public Dictionary<string, string> Data { get; set; }
        
        public Dictionary<string, string> FormData { get; set; }



        #region helpers for building quick responses
        private string Route(string action, string controller = null)
        {
            if (controller == null)
            {
                controller = GetType().Name;
            }
           
            return new ProgrammableServiceRoute
            {
                ActionName = action,
                ControllerName = controller
            }.ToJson();
            
        }

        /// <summary>
        /// Get Form's response data.
        /// </summary>
        /// <returns></returns>
        internal async Task<Dictionary<string, string>> GetFormData()
        {
            var json = await DataBag.Get(FormDataKey);
            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                return data;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>();
            }
        }


        /// <summary>
        /// Redirect to specified <paramref name="controller"/>'s <paramref name="action"/>.
        /// If <paramref name="controller"/> is not specified this controller is used.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public ProgrammableServiceResponse Redirect(string action, string controller = null)
        {
            return ProgrammableServiceResponse.Redirect(Route(action, controller));
        }

        /// <summary>
        /// Prepares the response for creating an item for cart (i.e. pending payment)
        /// </summary>
        /// <param name="message">For USSD channels, this message is displayed to the user before the payment flow kicks in</param>
        /// <param name="cartData">Must be valid and not null</param>
        /// <returns></returns>
        public ProgrammableServiceResponse AddToCart(string message,ProgrammableServicesResponseCartData cartData)
        {
            if (cartData==null)
            {
                throw new ArgumentNullException(nameof(cartData));
            }
            return ProgrammableServiceResponse.AddToCartForCheckout(message, cartData,Request.IsUssd());
        }

        /// <summary>
        /// Render <paramref name="message"/>
        /// </summary>
        /// <param name="message">Mainly for USSD screens</param>
        /// <param name="action">The next action to be invoked</param>
        /// <param name="controller">The controller which hosts the next action</param>
        /// <param name="label">To be displayed on rich clients</param>
        /// <param name="dataType">Must be valid and not null</param>
        /// <param name="dataItems">List of choices to be presented to the user</param>
        /// <param name="cartData">Must be valid and not null</param>
        /// <param name="fieldType"></param>
        /// <returns></returns>
        public ProgrammableServiceResponse Render(string message, List<ProgrammableServicesResponseData> dataItems = null, ProgrammableServicesResponseCartData cartData = null, string label = null, string dataType = null, string fieldType = "text", string fieldName = "", bool persistAsFavoriteStep = false, string action = null,
            string controller = null)
        {
           
            if (message == null)
            {
                message = string.Empty;
            }

            string route = null;
            if (action != null)
            {
                route = Route(action, controller);
            }

            return ProgrammableServiceResponse.Render(message, dataItems, cartData, label, dataType, fieldType, fieldName, persistAsFavoriteStep, route);
        }


        /// <summary>
        /// Render USSD menu and redirect to appropriate user's choice.
        /// </summary>
        /// <param name="ussdMenu"></param>
        /// <returns></returns>
        public async Task<ProgrammableServiceResponse> RenderMenu(Menu ussdMenu, List<ProgrammableServicesResponseData> dataItems = null, ProgrammableServicesResponseCartData cartItem = null, string label = null, string dataType = null, bool persistAsFavoriteStep = false)
        {
            var json = JsonConvert.SerializeObject(ussdMenu);
            await DataBag.Set(MenuProcessorDataKey, json);
            return Render(ussdMenu.Render(), dataItems, cartItem, label, dataType, "", "", persistAsFavoriteStep, nameof(MenuProcessor));
        }


        public async Task<ProgrammableServiceResponse> MenuProcessor()
        {
            var json = await DataBag.Get(MenuProcessorDataKey);
            var menu = JsonConvert.DeserializeObject<Menu>(json);
            MenuItem item;
            try
            {
                var choice = Convert.ToInt16(Request.TrimmedMessage);
                if (choice == 0 && menu.ZeroItem != null)
                {
                    return Redirect(menu.ZeroItem.Action, menu.ZeroItem.Controller);
                }

                item = menu.Items[choice - 1];
            }
            catch (Exception ex)
            {
                return Render(string.Format("Menu choice {0} does not exist.",
                    Request.TrimmedMessage), null, null);
            }

            await DataBag.Delete(MenuProcessorDataKey);
            return Redirect(item.Action, item.Controller);
        }




        /// <summary>
        /// Render a form (series of inputs).
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public async Task<ProgrammableServiceResponse> RenderForm(Form form, List<ProgrammableServicesResponseData> dataItems = null, ProgrammableServicesResponseCartData cartItem = null, string label = null, string dataType = null, string fieldType = "text", string fieldName = "", bool persistAsFavoriteStep = false)
        {
            var json = JsonConvert.SerializeObject(form);
            await DataBag.Set(FormProcessorDataKey, json);
            //return Redirect("FormInputDisplay");
            return await RichFormInputDisplay(dataItems, cartItem, label, dataType, fieldType, fieldName, persistAsFavoriteStep);
        }


        internal async Task<ProgrammableServiceResponse> RichFormInputDisplay(List<ProgrammableServicesResponseData> dataItems = null, ProgrammableServicesResponseCartData cartItem = null, string label = null, string dataType = null, string fieldType = "text", string fieldName = "", bool persistAsFavoriteStep = false)
        {
            var form = await GetForm();
            var input = form.Inputs[form.ProcessingPosition];
            var displayName = input.DisplayName;
            var message = string.Empty;

            if (!string.IsNullOrWhiteSpace(form.Title))
            {
                message += form.Title + Environment.NewLine;
            }

            if (!input.HasOptions)
            {
                message += string.Format("{0}" + Environment.NewLine, displayName);
            }
            else
            {
                message += string.Format("{0}" + Environment.NewLine, displayName);

                for (int i = 0; i < input.Options.Count; i++)
                {
                    var option = input.Options[i];
                    var value = string.IsNullOrWhiteSpace(option.DisplayValue)
                        ? option.Value
                        : option.DisplayValue;

                    if (!string.IsNullOrWhiteSpace(option.Index))
                    {
                        message += string.Format("{0}. {1}" + Environment.NewLine, option.Index, value);
                    }
                    else
                    {
                        message += string.Format("{0}. {1}" + Environment.NewLine, i + 1, value);
                    }
                }
            }

            return Render(message, dataItems, cartItem, label, dataType, fieldType, fieldName, persistAsFavoriteStep, nameof(FormInputProcessor));
        }

        internal async Task<Form> GetForm()
        {
            var json = await DataBag.Get(FormProcessorDataKey);

            var form = JsonConvert.DeserializeObject<Form>(json);
            return form;
        }


        internal async Task<ProgrammableServiceResponse> FormInputProcessor()
        {
            var form = await GetForm();
            var input = form.Inputs[form.ProcessingPosition];
            var key = input.Name;
            string value = null;
            if (!input.HasOptions)
            {
                value = Request.TrimmedMessage;
            }
            else
            {
                try
                {
                    var requestValue = Request.TrimmedMessage;

                    var inputOption = input.Options.FirstOrDefault(i => i.Index == requestValue);

                    if (inputOption == null)

                    {
                        var choice = Convert.ToInt16(requestValue);
                        inputOption = input.Options[choice - 1];


                        if (inputOption == null || !string.IsNullOrEmpty(inputOption.Index))
                        {
                            return Render($"Option {Request.TrimmedMessage} does not exist.", null, null);
                        }
                    }


                    value = inputOption.Value;
                    if (string.IsNullOrEmpty(value))
                    {
                        return Render($"Option {Request.TrimmedMessage} does not exist.", null,
                            null);
                    }
                }
                catch (Exception ex)
                {
                    return Render($"Option {Request.TrimmedMessage} does not exist.", null, null);
                }
            }

            
            form.Data[key] = value;
            if (form.ProcessingPosition == (form.Inputs.Count - 1))
            {
                await DataBag.Delete(FormProcessorDataKey);
                var jsonData = JsonConvert.SerializeObject(form.Data);
                await DataBag.Set(FormDataKey, jsonData);
                return Redirect(form.Action, form.Controller);
            }

            ++form.ProcessingPosition;
            var json = JsonConvert.SerializeObject(form);
            await DataBag.Set(FormProcessorDataKey, json);
            return Redirect(nameof(FormInputDisplay));
        }

        internal async Task<ProgrammableServiceResponse> FormInputDisplay()
        {
            var form = await GetForm();
            var input = form.Inputs[form.ProcessingPosition];
            var displayName = string.IsNullOrWhiteSpace(input.DisplayName)
                ? input.Name
                : input.DisplayName;
            var message = string.Empty;

            if (!string.IsNullOrWhiteSpace(form.Title))
            {
                message += form.Title + Environment.NewLine;
            }

            if (!input.HasOptions)
            {
                //
                message += string.Format("{0}" + Environment.NewLine, displayName);
            }
            else
            {
                message += string.Format("{0}" + Environment.NewLine, displayName);

                for (int i = 0; i < input.Options.Count; i++)
                {
                    var option = input.Options[i];
                    var value = string.IsNullOrWhiteSpace(option.DisplayValue)
                        ? option.Value
                        : option.DisplayValue;

                    if (!string.IsNullOrWhiteSpace(option.Index))
                    {
                        message += string.Format("{0}. {1}" + Environment.NewLine, option.Index, value);
                    }
                    else
                    {
                        message += string.Format("{0}. {1}" + Environment.NewLine, i + 1, value);
                    }
                }
            }

            return Render(message, null, null, null, null, null, null, false, nameof(FormInputProcessor));
        }


        /// <summary>
        /// Must be used only for USSD.
        /// </summary>
        /// <param name="collection">Must be valid and not null. Dictionary's key should be the display text.</param>
        /// <param name="paginationOptions"></param>
        /// <returns></returns>
        public async Task<ProgrammableServiceResponse> PaginateForUssd(Dictionary<string, string> collection,
            PaginationOptions paginationOptions)
        {

            if (collection==null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (paginationOptions == null)
            {
                throw new ArgumentNullException(nameof(paginationOptions));
            }

            if (string.IsNullOrEmpty(paginationOptions.NextPageKey))
            {
                throw new ArgumentNullException(nameof(paginationOptions.NextPageKey));
            }
            if (string.IsNullOrEmpty(paginationOptions.PreviousPageKey))
            {
                throw new ArgumentNullException(nameof(paginationOptions.PreviousPageKey));
            }

            if (paginationOptions.PreviousPageKey.Equals(paginationOptions.NextPageKey))
            {
                throw new Exception($"{nameof(paginationOptions.NextPageKey)} should not be the same as {nameof(paginationOptions.PreviousPageKey)}");
            }

            if (FormData == null)
            {
                FormData = new Dictionary<string, string>();

            }

            if (!Request.IsUssd())
            {
                throw new InvalidOperationException("this method is meant for USSD context only :|");
            }
            await DataBag.Set(PaginationOptionsKey, JsonConvert.SerializeObject(paginationOptions));
            await DataBag.Set(PaginationCollectionKey, JsonConvert.SerializeObject(collection));
            await DataBag.Set(OnPaginationItemSelectedName, OnPaginationItemSelectedValue);

            var form = (await PaginateCollection(collection, paginationOptions))[0];
            form.Data = FormData;

            return await RenderForm(form);
        }

        private const string PaginationOptionsKey = "__paginationOptions__";
        private const string PaginationCollectionKey = "__paginationCollection__";
        private const string OnPaginationItemSelectedName = "__onPaginationItemSelectedName__";
        private const string OnPaginationItemSelectedValue = "__onPaginationItemSelectedValue__";


        private async Task<Form[]> PaginateCollection(Dictionary<string,string> collection,PaginationOptions paginationOptions)
        {
            var key = await DataBag.Get(OnPaginationItemSelectedName);

            
            var arrayTotal = Math.Floor((decimal)collection.Count / paginationOptions.PageCount);
            var arrayTotalTopup = collection.Count % paginationOptions.PageCount; //just because we want to show 3 items at a time...


            var formarray = new Form[(int)arrayTotal + arrayTotalTopup];

            for (var i = 0; i < formarray.Length; i++)
            {
                formarray[i] = Form.New(paginationOptions.Title,
                    nameof(ProcessNextPreviousPaginationActions));

                formarray[i].AddInput(new Input
                {
                    DisplayName = string.Empty,
                    Name = OnPaginationItemSelectedValue
                });
            }

            for (var i = 0; i < formarray.Length; i++)
            {
                formarray[i].Inputs.Find(x => x.Name == key).Options.AddRange(
                    collection.Skip(((i * paginationOptions.PageCount)))
                        .Take(paginationOptions.PageCount).Select(x => new InputOption
                    {
                        DisplayValue = x.Key,
                        Value = x.Value,
                        Index = paginationOptions.UseDefaultNumberListing? (collection.Keys.ToList().IndexOf(x.Key)+1).ToString() : x.Value
                    }));
            }

            for (var i = 0; i <= paginationOptions.PageCount; i++)
            {
                try
                {
                    if (formarray[i + 1].Inputs.Find(x => x.Name == key).Options.Any())
                    {
                        formarray[i].Inputs.Find(x => x.Name == key).Options.AddRange(
                            new List<InputOption> {
                                new InputOption{ DisplayValue = paginationOptions.NextPageDisplayText, Value = "more" +i,
                                    Index = paginationOptions.NextPageKey
                                }

                            });
                    }
                }
                catch (Exception ex)
                {

                }
            }

            for (var i = 1; i < formarray.Length; i++)
            {
                if (!formarray[i].Inputs.Find(x => x.Name == key).Options.Any())
                    break;

                formarray[i].Inputs.Find(x => x.Name == key).Options.AddRange(
                    new List<InputOption>
                    {
                        new InputOption{
                            DisplayValue = paginationOptions.PreviousPageDisplayText, Value = "back"+i, Index =paginationOptions.PreviousPageKey
                        }
                    });
            }


            return formarray;
        }

        public virtual Task<ProgrammableServiceResponse> OnPaginatedItemSelected(string value)
        {
            return Task.FromResult(Render("pagination works!!! Please override this in your controller :)"));
        }

        public async Task<ProgrammableServiceResponse> ProcessNextPreviousPaginationActions()
        {
            var value = FormData[OnPaginationItemSelectedValue];

            var service = JsonConvert.DeserializeObject<Dictionary<string, string>>(await DataBag.Get(PaginationCollectionKey));
            var paginationOptions = JsonConvert.DeserializeObject<PaginationOptions>(await DataBag.Get(PaginationOptionsKey));

            int page;
            if (value.StartsWith("more"))
            {
                int.TryParse(value.Substring(value.IndexOf("e",
                                                 StringComparison.Ordinal) + 1), out page);


                var form = (await PaginateCollection(service,paginationOptions))[page + 1];
                form.Data = FormData;
                return await RenderForm(form);
            }

            if (!value.StartsWith("back"))
            {
                return await OnPaginatedItemSelected(value);
            }

            int.TryParse(value.Substring(
                value.IndexOf("k", StringComparison.Ordinal) + 1), out page);
            
            var form1 = (await PaginateCollection(service,paginationOptions))[page - 1];
            form1.Data = FormData;
            return await RenderForm(form1);
        }
        #endregion


        public void Dispose()
        {

        }

        
    }
}