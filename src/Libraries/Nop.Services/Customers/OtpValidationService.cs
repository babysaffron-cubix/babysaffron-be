using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Nop.Services.Common;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Nop.Services.Customers;

public partial class OtpValidationService : IOtpValidationService
{

    #region Fields

    protected readonly IConfiguration _configuration;
    protected readonly string _sendGridAccountSid;
    protected readonly string _sendGridAuthtoken;
    protected readonly string _sendGridPathServiceSid;

    #endregion


    #region Cotr

    public OtpValidationService(IConfiguration configuration)
    {
        _configuration = configuration;

        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        _sendGridAccountSid = _configuration["AppSettings:SendGridAccountSID"];
        _sendGridAuthtoken = _configuration["AppSettings:SendGridAuthToken"];
        _sendGridPathServiceSid = _configuration["AppSettings:SendGridPathServiceSid"];
    }

    #endregion
    public async Task<OtpValidationResult> ValidateOtp(string email, string otp)
    {
        OtpValidationResult otpValidationResult = new OtpValidationResult();
        try
        {

            string accountSid = _sendGridAccountSid;
            string authToken = _sendGridAuthtoken;

            TwilioClient.Init(accountSid, authToken);

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: email,
                code: otp,
                pathServiceSid: _sendGridPathServiceSid);

            if(verificationCheck.Status == "approved")
            {
                return otpValidationResult;
            }
            otpValidationResult.AddError("Otp is invalid. Please try with correct otp.");
            return otpValidationResult;
        }

        catch (Exception ex)
        {
            // Get the type of the exception object
            Type exceptionType = ex.GetType();

            // Get the PropertyInfo object for the 'Status' property
            PropertyInfo statusProperty = exceptionType.GetProperty("Status");

            if (statusProperty != null)
            {
                // Get the value of the 'Status' property
                var statusValue = statusProperty.GetValue(ex);

                // Output the value
                Console.WriteLine($"Status: {statusValue}");
                if (statusValue == "404")
                {
                    otpValidationResult.AddError("Otp is invalid or expired. Please try with correct otp.");
                }
                else
                {
                    otpValidationResult.AddError("Something went wrong. Please try again later.");
                }
            }
            else
            {
                otpValidationResult.AddError("Something went wrong. Please try again later.");

            }

            return otpValidationResult;

        }
    }

}


