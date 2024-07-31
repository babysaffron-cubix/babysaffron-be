using System;
using Nop.Services.Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using Nop.Services.Messages;

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
    protected readonly string _sendGridContactUsEmailTemplateId;
    protected readonly IEmailAccountService _emailAccountService;
    protected readonly string _sendGridSupportContactEmailDisplayName;


    #endregion

    #region Ctor

    public EmailOtpSenderService(IOtpGeneratorService otpGeneratorService, IConfiguration configuration, IEmailAccountService emailAccountService)
    {
        _otpGeneratorService = otpGeneratorService;
        _configuration = configuration;
        _emailAccountService = emailAccountService;


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
        _sendGridContactUsEmailTemplateId = _configuration["AppSettings:SendGridContactUsEmailTemplateId"];
        _sendGridSupportContactEmailDisplayName = _configuration["AppSettings:SendGridSupportContactEmailDisplayName"];

    }

    private async Task<string> GetSupportContactReceiverEmailId()
    {
        var allConfigEmails = await _emailAccountService.GetAllEmailAccountsAsync();
        var supportContactEmailId = allConfigEmails.Where(x => x.DisplayName == _sendGridSupportContactEmailDisplayName).Select(x=>x.Email).FirstOrDefault();
        return supportContactEmailId;
    }


    #endregion

    public async Task<EmailSendResult> RequestOtp(string email)
    {

        var response = await SendEmailUsingTemplate(email);
        return response;
    }



    private  async Task<EmailSendResult> SendEmailUsingTemplate(string email)
    {
        EmailSendResult emailSendResult  = new EmailSendResult();

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

            emailSendResult.Message = "OTP generated successfully and sent over email.";
            return emailSendResult;

        }
       
        catch (Exception ex)
        {
            emailSendResult.AddError(ex.Message);
            return emailSendResult;
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

    public async Task<EmailSendResult> SendSupportEmail(SupportEmailRequest supportEmailRequest)
    {   EmailSendResult emailSendResult = new EmailSendResult();
        try
        {
            if (String.IsNullOrEmpty(supportEmailRequest.Email) || String.IsNullOrEmpty(supportEmailRequest.IssueHeading))
            {
                emailSendResult.AddError("Missing details");
                return emailSendResult;

            }
            var supportContactEmail = await GetSupportContactReceiverEmailId();
            if (String.IsNullOrEmpty(supportContactEmail))
            {
                emailSendResult.AddError("There is no configuration for the support email in admin. Cannot send email");
                return emailSendResult;
            }
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridSenderEmailId, _sendGridSenderName);
            var to = new EmailAddress(supportContactEmail, supportContactEmail);

            var dynamicTemplateData = new
            {
                Issue = supportEmailRequest.IssueHeading,
                IssueDescription = supportEmailRequest.IssueDescription,
                From = supportEmailRequest.Email
            };

            // Create a message using the template ID and dynamic data
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, _sendGridSupportEmailTemplateId, dynamicTemplateData);


            var response = await client.SendEmailAsync(msg);

            emailSendResult.Message = "Suport email sent.";
            return emailSendResult;

        }
        catch (Exception ex)
        {
            emailSendResult.AddError(ex.Message);
            return emailSendResult;
        }
    }


    public async Task<EmailSendResult> SendContactUsEmail(ContactUsEmailRequest contactUsEmailRequest)
    {
        EmailSendResult emailSendResult = new EmailSendResult();
        try
        {
            if (String.IsNullOrEmpty(contactUsEmailRequest.Email) || string.IsNullOrEmpty(contactUsEmailRequest.FirstName) || string.IsNullOrEmpty(contactUsEmailRequest.Message))
            {
                emailSendResult.AddError("Missing details.");
                return emailSendResult;
            }
            var supportContactEmail = await GetSupportContactReceiverEmailId();
            if (String.IsNullOrEmpty(supportContactEmail))
            {
                emailSendResult.AddError("There is no configuration for the support email in admin. Cannot send email");
                return emailSendResult;
            }

            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress(_sendGridSenderEmailId, _sendGridSenderName);
            var to = new EmailAddress(supportContactEmail, supportContactEmail);

            var dynamicTemplateData = new
            {
                OnMind = contactUsEmailRequest.OnMind,
                GetBackReason = contactUsEmailRequest.GetBackReason,
                Message = contactUsEmailRequest.Message,
                FirstName = contactUsEmailRequest.FirstName,
                LastName = contactUsEmailRequest.LastName,
                Country = contactUsEmailRequest.Country,
                PhoneNo = contactUsEmailRequest.PhoneNo,
                Email = contactUsEmailRequest.Email
            };

            // Create a message using the template ID and dynamic data
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, _sendGridContactUsEmailTemplateId, dynamicTemplateData);


            var response = await client.SendEmailAsync(msg);

            emailSendResult.Message = "ContactUs email sent.";
            return emailSendResult;

        }
        catch (Exception ex)
        {
            emailSendResult.AddError(ex.Message);
            return emailSendResult;
        }
    }
}

