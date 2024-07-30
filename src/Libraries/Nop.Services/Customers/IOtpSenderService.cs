using System;
namespace Nop.Services.Customers;

public partial interface IOtpSenderService
{
    Task<EmailSendResult> RequestOtp(string emailOrPhone);
    Task SendWelcomeEmail(string email);

    Task<EmailSendResult> SendSupportEmail(SupportEmailRequest supportEmailRequest);
    Task<EmailSendResult> SendContactUsEmail(ContactUsEmailRequest contactUsEmailRequest);

}

