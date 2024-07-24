using System;
namespace Nop.Services.Customers;

public partial interface IOtpSenderService
{
    Task<OtpGeneratorResult> RequestOtp(string emailOrPhone);
    Task SendWelcomeEmail(string email);
}

