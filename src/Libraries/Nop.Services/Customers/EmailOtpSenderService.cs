using System;
using Nop.Services.Common;

namespace Nop.Services.Customers;

public partial class EmailOtpSenderService : OtpValidationService, IOtpSenderService
{
    #region Fields

    protected readonly IOtpGeneratorService _otpGeneratorService;
    #endregion

    #region Ctor

    public EmailOtpSenderService(IOtpGeneratorService otpGeneratorService)
    {
        _otpGeneratorService = otpGeneratorService;
    }


    #endregion

    public async Task<string> RequestOtp()
    {
        //TODO: Implement logic to request otp on email
        //throw new NotImplementedException();
        var otp = await _otpGeneratorService.GenerateOtp();
        return $"OTP Generated successfully and sent over email - {otp}";
    }
}

