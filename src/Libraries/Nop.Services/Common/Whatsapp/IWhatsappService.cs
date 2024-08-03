using System;
namespace Nop.Services.Common.Whatsapp;

public interface IWhatsappService
{
    Task<string> SendOrderConfirmationOnWhatsapp(string sendToNumber, WhatsappEmailRequest whatsappEmailRequest);
}

