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
    protected readonly string _sendGridApiKey;
    protected readonly string _sendGridEmailVerificationTemplateId;
    protected readonly string _sendGridSenderEmailId;
    protected readonly string _sendGridSenderName;
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
        _sendGridApiKey = _configuration["AppSettings:SendGridApiKey"];
        _sendGridEmailVerificationTemplateId = _configuration["AppSettings:SendGridEmailVerificationTemplateId"];
        _sendGridSenderEmailId = _configuration["AppSettings:SendGridSenderEmailId"];
        _sendGridSenderName = _configuration["AppSettings:SendGridSenderName"];
    }


    #endregion

    public async Task<OtpGeneratorResult> RequestOtp(string email)
    {

        var response = await SendEmailUsingTemplate(email);
        return response;
    }



    private  async Task<OtpGeneratorResult> SendEmailUsingTemplate(string email)
    {
        OtpGeneratorResult otpGeneratorResult = new OtpGeneratorResult();

        try
        {
        string accountSid = _sendGridAccountSid;
        string authToken = _sendGridAuthtoken;
            string apiKey = _sendGridApiKey;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_sendGridSenderEmailId, _sendGridSenderName);
            var to = new EmailAddress(email, email);

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetTemplateId(_sendGridEmailVerificationTemplateId);

            msg.SetTemplateData(new
            {
                twilio_code = randomOtp()
            });

            //TwilioClient.Init(accountSid, authToken);

            //var verification = await VerificationResource.CreateAsync(
            //    channel: "email",
            //    to: email,
            //    pathServiceSid: _sendGridPathServiceSid);

            //    otpGeneratorResult.Message = "OTP generated successfully and sent over email.";
            //    return otpGeneratorResult;

            var response = await client.SendEmailAsync(msg);

            return otpGeneratorResult;
        }
       
        catch (Exception ex)
        {
            otpGeneratorResult.AddError(ex.Message);
            return otpGeneratorResult;
        }
    }

    private string randomOtp()
    {
        Random r = new Random();
        int randNum = r.Next(1000000);
        string sixDigitNumber = randNum.ToString("D6");
        return sixDigitNumber;
    }
}

