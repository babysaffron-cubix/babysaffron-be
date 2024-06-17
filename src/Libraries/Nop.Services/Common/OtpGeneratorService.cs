using System;
namespace Nop.Services.Common;

public partial class OtpGeneratorService : IOtpGeneratorService
{
    public async Task<string> GenerateOtp()
    {
        Random random = new Random();
        int randomCode = random.Next(100000, 1000000);
        return randomCode.ToString();
    }
}

