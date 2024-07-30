using System;
namespace Nop.Services.Customers;

public class ContactUsEmailRequest
{
	public string OnMind { get; set; }
    public string GetBackReason { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Country { get; set; }
    public string PhoneNo { get; set; }
    public string Email { get; set; }
    public string Message { get; set; }

}

