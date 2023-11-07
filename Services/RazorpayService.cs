// RazorpayService.cs
using School_Login_SignUp.Models;
using Razorpay.Api;
using System.Security.Cryptography;
using System.Text;

namespace School_Login_SignUp.Services
{

    public class RazorpayService
    {
        private const string _apiKey = "rzp_test_8Hp4RmqbRbDRFm";
        private const string _apiSecret = "2hoVwSgAFChLrL3ghwCdAfsb";

        public string CreateOrder(decimal amount, string currency, Dictionary<string, string> notes)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_apiKey, _apiSecret);
                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    { "amount", (int)(amount * 100) }, // Amount in paise (INR)
                    { "currency", currency },
                    { "notes", notes }
                };

                Order order = client.Order.Create(options);
                return order.Attributes["id"].ToString();
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw;
            }
        }

        public RegistrationModel Registration()
        {
            var model = new RegistrationModel()
            {
                Amount = 10
            };
            return model;
        }

        public RazorPayOptionsModel Payment(RegistrationModel registration)
        {
            OrderModel order = new OrderModel()
            {
                OrderAmount = registration.Amount,
                Currency = "INR",
                Payment_Capture = 1,    // 0 - Manual capture, 1 - Auto capture
                Notes = new Dictionary<string, string>()
                {
                    {
                        "note 1", "first note while creating order"
                    },
                }
            };
            var orderId = CreateOrder(order);
            RazorPayOptionsModel razorPayOptions = new RazorPayOptionsModel()
            {
                Key = _apiKey,
                AmountInSubUnits = order.OrderAmountInSubUnits,
                Currency = order.Currency,
                Name = "RazorPay",
                Description = "For Dotnet Payment Gateway",
                ImageLogUrl = "",
                OrderId = orderId,
                ProfileName = registration.Name,
                ProfileContact = registration.Mobile,
                ProfileEmail = registration.Email,
                Notes = new Dictionary<string, string>()
                {
                    {
                        "note 1", "this is a payment note"
                    },
                }
            };
            return razorPayOptions;
        }


        private string CreateOrder(OrderModel order)
        {
            try
            {
                RazorpayClient client = new RazorpayClient(_apiKey, _apiSecret);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", order.OrderAmountInSubUnits);
                options.Add("currency", order.Currency);
                options.Add("payment_capture", order.Payment_Capture);
                options.Add("notes", order.Notes);

                Order orderResponse = client.Order.Create(options);
                var orderId = orderResponse.Attributes["id"].ToString();
                return orderId;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }

}

