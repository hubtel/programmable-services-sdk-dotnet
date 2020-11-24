using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hubtel.ProgrammableServices.Sdk.Core;
using Hubtel.ProgrammableServices.Sdk.InteractionElements;
using Hubtel.ProgrammableServices.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace ProgrammableServicesSampleApp.ProgrammableServiceControllers
{

    
    public class EvdController : ProgrammableServiceControllerBase
    {
        private readonly ILogger<EvdController> _logger;

        public EvdController(ILogger<EvdController> logger)
        {
            _logger = logger;
        }

        [HandleInitiation]
        public async Task<ProgrammableServiceResponse> Start()
        {
            _logger.LogDebug("Initial request: {request}", Request);
            try
            {


                if (Request.IsUssd())
                {
                    var header = "Buy my awesome product";

                    var item1 = "Buy for myself";
                    var item2 = "Buy for a different number";

                    var menu = Menu.New(header)
                        .AddItem(item1, $"{nameof(EnterAmountForm)}")
                        .AddItem(item2, $"{nameof(RecipientForm)}");

                    await DataBag.Set("mobileNumber", Request.Mobile);

                    // setup rich ux for web and mobile
                    return await RenderMenu(menu, new List<ProgrammableServicesResponseData>
                    {
                        new ProgrammableServicesResponseData(item1, "1",decimal.Zero),
                        new ProgrammableServicesResponseData(item2, "2",decimal.Zero),
                    }, null, header, ProgrammableServiceDataTypes.Menu);
                }

                return Redirect($"{nameof(RecipientForm)}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        #region Buy flow

        public async Task<ProgrammableServiceResponse> RecipientForm()
        {
            try
            {
                var form = Form.New("", $"{nameof(ProcessRecipientForm)}");
                form.AddInput(Input.New("mobileNumber", "mobile number"));
                return await RenderForm(form, null, null, "Enter mobile number", ProgrammableServiceDataTypes.Input,
                    ProgrammableServiceFieldTypes.Phone,"mobileNumber", true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        public async Task<ProgrammableServiceResponse> ProcessRecipientForm()
        {
            try
            {
                var mobileNumber = FormData["mobileNumber"];

                if (string.IsNullOrEmpty(mobileNumber)) // check if the input phone number is invalid
                {
                    return Redirect($"{nameof(RecipientForm)}");
                }

                await DataBag.Set("mobileNumber", mobileNumber);
                return Redirect($"{nameof(EnterAmountForm)}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        #endregion Buy flow

        #region Enter Amount, Confirm (if USSD) and add to cart for checkout

        public async Task<ProgrammableServiceResponse> EnterAmountForm()
        {
            try
            {
                var form = Form.New("", $"{nameof(Confirmation)}");
                form.AddInput(Input.New("amount", "amount"));
                return await RenderForm(form, null, null, "Enter amount", ProgrammableServiceDataTypes.Input, ProgrammableServiceFieldTypes.Decimal);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        public async Task<ProgrammableServiceResponse> Confirmation()
        {
            try
            {
                var mobileNumber = await DataBag.Get("mobileNumber");
                var amountStr = FormData["amount"]; // you may want to validate amount

                if (!decimal.TryParse(amountStr, out var amount))
                {
                    return await RenderResponse("Amount is not valid");
                }

                if (amount <= decimal.Zero)
                {
                    return await RenderResponse($"Amount is not valid");
                }


                if (amount<1)
                {
                    return await RenderResponse($"Sorry, minimum amount is GHS 1.00");
                }

                if (amount > 100)
                {
                    return await RenderResponse($"Sorry, maximum amount is GHS 100.00");
                }


                await DataBag.Set("amount", amount.ToString());
                
                var header = $"Confirmation\nService: MTN Airtime for {mobileNumber}\nAmount: {amount}";

                var menu = Menu.New(header)
                    .AddItem("Confirm", $"{nameof(ProcessAddToCart)}")
                    .AddZeroItem("Cancel", $"{nameof(Exit)}");
                
                var dataItems = new List<ProgrammableServicesResponseData>
                {
                    new ProgrammableServicesResponseData("Confirm","1")
                    ,
                    new ProgrammableServicesResponseData("Cancel","0")
                };

                if (Request.IsUssd())
                {
                    return await RenderMenu(menu, dataItems, null, header, ProgrammableServiceDataTypes.Confirm);
                }
                
                return await ProcessAddToCart();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        public async Task<ProgrammableServiceResponse> ProcessAddToCart()
        {
            try
            {
                var mobileNumber = await DataBag.Get("mobileNumber");
                var accountId = await DataBag.Get("mobileNumber");
                var rawAmount = await DataBag.Get("amount");
                var rawQuantity = "1";
                var country = "GH";
                decimal amount = decimal.Parse(rawAmount);
                int quantity = int.Parse(rawQuantity);

               //save your data into Storage so that when you get callback from Hubtel, you can 
               //compare and render service
                var cartItem = new ProgrammableServicesResponseCartData($"MTN Airtime Topup for {mobileNumber}", quantity, amount,Guid.NewGuid().ToString("N"))
                {
                    ServiceData = { ["destination"] = mobileNumber, ["amount"] = $"{2m}" }
                };
                cartItem.ServiceData["destination"] = mobileNumber;
                cartItem.ServiceData["amount"] = rawAmount;

                var checkoutMsg =
                    $"Please authorize payment for GHs {amount} as MTN Airtime Topup to {mobileNumber}";


                //any of these methods can work for redirecting a user to another controller

                //return Redirect(nameof(TestController.Index), nameof(TestController));
                // return Redirect("Index", nameof(TestController));
                //return Redirect("Index", "Test");
                // return Redirect("Index", nameof(TestController));


                return AddToCart(checkoutMsg, cartItem);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return await Error();
            }
        }

        #endregion

        #region Exit and Error Responses

        public async Task<ProgrammableServiceResponse> Exit()
        {
            return await RenderResponse("Thank you!");
        }

        private async Task<ProgrammableServiceResponse> RenderResponse(string response)
        {
            var resp = Render(response, null, null);
            // setup rich ux for web and mobile
            resp.Label = response;
            resp.DataType = ProgrammableServiceDataTypes.Display;
            return await Task.FromResult(resp);
        }

        public async Task<ProgrammableServiceResponse> Error()
        {
            return await RenderResponse("An error occured. Please try again!");
        }

        #endregion Exit and Error Responses
    }
}