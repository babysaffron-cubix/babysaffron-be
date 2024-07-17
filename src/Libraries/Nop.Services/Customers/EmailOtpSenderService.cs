using System;
using Nop.Services.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Nop.Services.Customers;

public partial class EmailOtpSenderService : OtpGeneratorService, IOtpSenderService
{
    #region Fields

    protected readonly IOtpGeneratorService _otpGeneratorService;
    protected readonly IConfiguration _configuration;
    protected readonly string _sendGridAccountSid;
    protected readonly string _sendGridAuthtoken;
    protected readonly string _sendGridPathServiceSid;
    #endregion

    #region Ctor

    public EmailOtpSenderService(IOtpGeneratorService otpGeneratorService, IConfiguration configuration)
    {
        _otpGeneratorService = otpGeneratorService;
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

    public async Task<OtpGeneratorResult> RequestOtp(string email)
    {

        var response = await SendEmailUsingTemplate(email);
        return response;
    }

    private async Task SendOtpToEmail(string email, string otp)
    {
        var apiKey = "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";
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("vipulm124@gmail.com", "Example User");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress(email, email);
        var plainTextContent = $"The otp for BabySaffron is {otp}";
        var htmlContent = $"The otp for BabySaffron is <strong>{otp}</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
    }

    private  async Task<OtpGeneratorResult> SendEmailUsingTemplate(string email)
    {
        OtpGeneratorResult otpGeneratorResult = new OtpGeneratorResult();

        try
        {
        string accountSid = _sendGridAccountSid;
        string authToken = _sendGridAuthtoken;

        TwilioClient.Init(accountSid, authToken);

        var verification = await VerificationResource.CreateAsync(
            channel: "email",
            to: email,
            pathServiceSid: _sendGridPathServiceSid);
            otpGeneratorResult.Message = "OTP generated successfully and sent over email.";
            return otpGeneratorResult;
        }
       
        catch (Exception ex)
        {
            otpGeneratorResult.AddError(ex.Message);
            return otpGeneratorResult;
        }
    }
}

