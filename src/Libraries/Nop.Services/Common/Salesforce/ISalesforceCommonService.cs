using System;
namespace Nop.Services.Common.Salesforce;

public interface ISalesforceCommonService
{

    Task<string> SalesforceAPICallHandler(string endpoint, string contactJsonString);

    Task<string> GetSalesforceToken();
}

