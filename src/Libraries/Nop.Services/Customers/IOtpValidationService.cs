
namespace Nop.Services.Customers;

public partial interface IOtpValidationService
{
   Task<OtpValidationResult> ValidateOtp(string email, string otp);
}

