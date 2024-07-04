using System;
namespace Nop.Services.Payments;

public partial class RazorpayPaymentVerificationRequest
{
	public string RazorpayOrderId { get; set; }
    public string RazorpayPaymentId { get; set; }
    public string RazorpaySignature { get; set; }

}

