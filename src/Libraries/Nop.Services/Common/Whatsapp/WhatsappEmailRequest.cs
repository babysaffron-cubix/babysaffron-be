using System;
namespace Nop.Services.Common.Whatsapp;

public class WhatsappEmailRequest
{
	public string Name { get; set; }
	public int OrderId { get; set; }
	public string Weight { get; set; }
	public string OrderAmount { get; set; }
}

