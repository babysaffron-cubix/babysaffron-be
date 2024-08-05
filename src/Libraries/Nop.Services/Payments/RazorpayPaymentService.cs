using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Nop.Services.Orders;
using Org.BouncyCastle.Asn1.Pkcs;
using Razorpay.Api;
namespace Nop.Services.Payments;

public partial class RazorpayPaymentService : IRazorpayPaymentService
{
    public string _key;
    public string _secret;
    protected readonly IOrderService _orderService;
    private readonly IConfiguration _configuration;


    public RazorpayPaymentService(IOrderService orderService, IConfiguration configuration)
    {
        _configuration = configuration;
        _orderService = orderService;

        var builder = new ConfigurationBuilder()
         .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
         .AddEnvironmentVariables();

        _configuration = builder.Build();

        _key = _configuration["AppSettings:RazorpayKey"]; ;
        _secret = _configuration["AppSettings:RazorpaySecret"]; ;
    }
    public async Task<RazorpayOrderCreationResponse> CreateOrder(int orderId)
    {
        RazorpayOrderCreationResponse razorpayOrderCreationResponse = new RazorpayOrderCreationResponse();
        try
        {
            var nopcommerceOrder = await _orderService.GetOrderByIdAsync(orderId);
            if (nopcommerceOrder != null)
            {
                // this field contains the total post discounts
                //multiplying it with CurrencyRate to handle scenarions where currency is USD and what is saved in db is always INR(as it is marked as primary currency in admin)
                var amount = (nopcommerceOrder.OrderTotal * nopcommerceOrder.CurrencyRate) + nopcommerceOrder.OrderShippingInclTax;

                if (amount <=0)
                {
                    razorpayOrderCreationResponse.AddError("Amount is 0, please re-validate your request.");
                }
                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", (int)(amount * 100)); //amount in paisa
                options.Add("currency", nopcommerceOrder.CustomerCurrencyCode);
                options.Add("receipt", Guid.NewGuid());

                Order order = await Task.Run(() => client.Order.Create(options));
                razorpayOrderCreationResponse.Order = order;

            }

            return razorpayOrderCreationResponse;
        }
        catch (Exception ex)
        {
            razorpayOrderCreationResponse.AddError(ex.Message);
            return razorpayOrderCreationResponse;
        }
    }

    public async Task<string> VerifyPayment(RazorpayPaymentVerificationRequest request)
    {
        try
        {

            bool isSignatureValid = await VerifyRazorpaySignature(request.RazorpayPaymentId, request.RazorpayOrderId, _secret, request.RazorpaySignature);

            if (isSignatureValid)
            {
                return "Payment Verified";
            }

            return "Payment verification failed.";


        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public async Task<bool> VerifyRazorpaySignature(string paymentId, string orderId, string secret, string signature)
    {
        string payload = $"{orderId}|{paymentId}";

        // Create HMAC-SHA256 signature using secret
        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
        {
            byte[] hashMessage = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(payload));
            string expectedSignature = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();

            return expectedSignature.Equals(signature);
        }
    }
}

