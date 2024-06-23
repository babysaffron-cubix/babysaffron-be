using System;
namespace Nop.Services.Customers;

public partial interface IOtpSenderService
{
    Task<string> RequestOtp(string emailOrPhone);
}

