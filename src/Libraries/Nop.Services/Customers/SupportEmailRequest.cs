using System;
namespace Nop.Services.Customers;

public class SupportEmailRequest
{
	public string IssueHeading { get; set; }
    public string IssueDescription { get; set; }
    public string Email { get; set; }

}

