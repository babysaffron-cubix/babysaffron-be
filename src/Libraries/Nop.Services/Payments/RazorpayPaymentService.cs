using System;
using Org.BouncyCastle.Asn1.Pkcs;
using Razorpay.Api;
namespace Nop.Services.Payments;

public partial class RazorpayPaymentService : IRazorpayPaymentService
{
    public string _key;
    public string _secret;
    public RazorpayPaymentService()
    {
        _key = "rzp_test_jESpwwUNibFtvH";
        _secret = "0tNTap3BA5OIQ2MBdAHR7ZmW";
    }
    public async Task<Order> CreateOrder(decimal amount)
    {   
        try
        {
            RazorpayClient client = new RazorpayClient(_key, _secret);
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", amount * 100); //amount in paisa
            options.Add("currency", "INR");
            options.Add("receipt", "create_own_receipt");

            Order order = await Task.Run(() => client.Order.Create(options));
            return order;

        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<string> VerifyPayment(RazorpayPaymentVerificationRequest request)
    {
        try
        {
            var attributes = new Dictionary<string, string>
            {
                    { "razorpay_order_id", request.RazorpayOrderId },
                    { "razorpay_payment_id", request.RazorpayPaymentId },
                    { "razorpay_signature", request.RazorpaySignature }
                };

            Utils.verifyPaymentSignature(attributes);

            await Task.Run(() => Utils.verifyPaymentSignature(attributes));

            return "Payment Verified";

        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

