using System;
using Razorpay.Api;

namespace Nop.Services.Payments;

public partial interface IRazorpayPaymentService
{
    Task<RazorpayOrderCreationResponse> CreateOrder(int orderId);

    Task<string> VerifyPayment(RazorpayPaymentVerificationRequest request);
}

