using System;
//using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Nop.Core.Domain.Salesforce;
public partial class SalesforceContacts
{
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    [JsonPropertyName("email_id")]
    public string EmailId { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }
    [JsonPropertyName("state")]
    public string? State { get; set; }
    [JsonPropertyName("city")]
    public string? City { get; set; }
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    [JsonPropertyName("pincode")]
    public string? Pincode { get; set; }
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    [JsonPropertyName("gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("oauth_provider")]
    public string? OAuthProvider { get; set; }

    [JsonPropertyName("oauth_uid")]
    public string? OAuthUId { get; set; }

    [JsonPropertyName("SFDC_Contact_Number")]
    public string? SFDCContactNumber { get; set; }
}

