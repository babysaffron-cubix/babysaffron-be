using System;
using Nop.Core.Domain.Salesforce;

namespace Nop.Plugin.Misc.WebApi.Frontend.Services;

public interface ISalesforceService
{ 
    Task<SalesforceContactUpsertResponse> UpsertSalesforceCustomerAsync(int customerId);

    Task<SalesforceOrderResponse> CreateSalesforceOrder(int orderId);

    Task<string> GetSalesforceToken();
}




