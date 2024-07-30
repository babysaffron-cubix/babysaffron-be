using System;
using Nop.Services.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;

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
    protected readonly string _sendGridWelcomeEmailTemplateId;
    protected readonly string _sendGridSupportEmailTemplateId;
    protected readonly string _sendGridSupportContactEmailId;
    #endregion

    #region Ctor

    public EmailOtpSenderService(IOtpGeneratorService otpGeneratorService, IConfiguration configuration)
    {
        _otpGeneratorService = otpGeneratorService;
        _configuration = configuration;


        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        _configuration = builder.Build();

        _sendGridAccountSid = _configuration["AppSettings:SendGridAccountSID"];
        _sendGridAuthtoken = _configuration["AppSettings:SendGridAuthToken"];
        _sendGridPathServiceSid = _configuration["AppSettings:SendGridPathServiceSid"];
        _sendGridApiKey = _configuration["AppSettings:SendGridApiKey"];
        _sendGridEmailVerificationTemplateId = _configuration["AppSettings:SendGridEmailVerificationTemplateId"];
        _sendGridSenderEmailId = _configuration["AppSettings:SendGridSenderEmailId"];
        _sendGridSenderName = _configuration["AppSettings:SendGridSenderName"];
        _sendGridWelcomeEmailTemplateId = _configuration["AppSettings:SendGridWelcomeEmailTemplateId"];
        _sendGridSupportEmailTemplateId = _configuration["AppSettings:SendGridSupportEmailTemplateId"];
        _sendGridSupportContactEmailId = _configuration["AppSettings:SendGridSupportContactEmailId"];

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

    private string randomOtp()
    {
        Random r = new Random();
        int randNum = r.Next(1000000);
        string sixDigitNumber = randNum.ToString("D6");
        return sixDigitNumber;
    }

    public async Task SendWelcomeEmail(string email)
    {
        try
        {

            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridSenderEmailId, _sendGridSenderName);
            var to = new EmailAddress(email, email);

            var msg = new SendGridMessage();
            msg.SetFrom(from);
            msg.AddTo(to);
            msg.SetTemplateId(_sendGridWelcomeEmailTemplateId);

            var response = await client.SendEmailAsync(msg);

        }

        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task SendSupportEmail(SupportEmailRequest supportEmailRequest)
    {
        try
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridSenderEmailId, _sendGridSenderName);
            var to = new EmailAddress(supportEmailRequest.Email, supportEmailRequest.Email);

            //var msg = new SendGridMessage();
            //msg.SetFrom(from);
            //msg.AddTo(to);
            //msg.SetTemplateId(_sendGridSupportEmailTemplateId);

            var dynamicTemplateData = new
            {
                Issue = supportEmailRequest.IssueHeading,
                IssueDescription = supportEmailRequest.IssueDescription,
                From = supportEmailRequest.Email
            };

            // Create a message using the template ID and dynamic data
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, _sendGridSupportEmailTemplateId, dynamicTemplateData);


            var response = await client.SendEmailAsync(msg);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}

