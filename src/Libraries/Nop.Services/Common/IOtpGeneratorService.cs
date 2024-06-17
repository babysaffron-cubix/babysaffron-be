using System;
namespace Nop.Services.Common;

public partial interface IOtpGeneratorService
{
    /// <summary>
    /// This function will generate an otp and return the same
    /// </summary>
    /// <returns></returns>
    Task<string> GenerateOtp();
}

