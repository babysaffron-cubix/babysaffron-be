using System;
using Nop.Core.Domain.Salesforce;

namespace Nop.Plugin.Misc.WebApi.Frontend.Services;

public interface ISalesforceService
{ 
    Task<SalesforceContactUpsertResponse> UpsertSalesforceCustomerAsync(int customerId, int? addressId = null);

    Task<SalesforceResponse> CreateSalesforceOrder(int orderId);

    Task<string> GetSalesforceToken();
}




