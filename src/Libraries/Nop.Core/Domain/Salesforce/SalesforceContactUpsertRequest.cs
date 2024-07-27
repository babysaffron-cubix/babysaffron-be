using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
namespace Nop.Core.Domain.Salesforce;

public partial class SalesforceContactUpsertRequest
{
    [JsonPropertyName("contacts")]
    public List<SalesforceContacts> Contacts { get; set; }
}



