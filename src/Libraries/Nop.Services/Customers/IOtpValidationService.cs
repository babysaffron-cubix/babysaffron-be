
namespace Nop.Services.Customers;

public partial interface IOtpValidationService
{
   Task<OtpGeneratorResult> ValidateOtp(string otp);
}

