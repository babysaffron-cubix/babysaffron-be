using System;

namespace Nop.Services.Customers;

public partial class OtpValidationService : IOtpValidationService
{
    public async Task<OtpGeneratorResult> ValidateOtp(string otp)
    {
        //TODO: Implement logic to validate the otp
        throw new NotImplementedException();
    }

}


