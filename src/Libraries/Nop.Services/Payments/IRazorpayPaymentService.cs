using System;
using Razorpay.Api;

namespace Nop.Services.Payments;

public partial interface IRazorpayPaymentService
{
    Task<Order> CreateOrder(decimal amount);

    Task<string> VerifyPayment(RazorpayPaymentVerificationRequest request);
}

