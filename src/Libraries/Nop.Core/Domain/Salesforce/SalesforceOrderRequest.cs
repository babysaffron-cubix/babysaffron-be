using System;
using System.Text.Json.Serialization;

namespace Nop.Core.Domain.Salesforce;

public class SalesforceOrderRequest
{
	public SalesforceOrderComponents OrderWrapper { get; set; }
}


public class SalesforceOrderComponents
{
    [JsonPropertyName("billing_address")]
    public SalesforceAddress BillingAddress { get; set; }

    [JsonPropertyName("shipping_address")]
    public SalesforceAddress ShippingAddress { get; set; }
    public SalesforceOrder Ord { get; set; }

    public List<SalesforceOrderLine> OrdLine { get; set; }
}

public class SalesforceAddress
{
    [JsonPropertyName("street")]
    public string Street { get; set; }
    [JsonPropertyName("street2")]
    public string Street2 { get; set; }
    [JsonPropertyName("street3")]
    public string Street3 { get; set; }
    [JsonPropertyName("state")]
    public string State { get; set; }

	[JsonPropertyName("postal_code")]
	public string PostalCode { get; set; }

	[JsonPropertyName("phone_no")]
	public string PhoneNo { get; set; }

	[JsonPropertyName("phone_code")]
	public string PhoneCode { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("country")]
    public string Country { get; set; }
    [JsonPropertyName("city")]
    public string City { get; set; }

	[JsonPropertyName("address_save_as")]
	public string AddressSaveAs { get; set; }
}


public class SalesforceOrder
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("transactionid")]
    public string Transactionid { get; set; }

    [JsonPropertyName("order_total")]
    public decimal OrderTotal { get; set; }

    [JsonPropertyName("order_status")]
    public string OrderStatus { get; set; }

    [JsonPropertyName("order_number")]
    public string OrderNumber { get; set; }

    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("order_date")]
    public string OrderDate { get; set; }


    [JsonPropertyName("order_currency")]
    public string OrderCurrency { get; set; }


    [JsonPropertyName("is_cancelled")]
    public string? IsCancelled { get; set; }

    [JsonPropertyName("disc_Percent")]
    public decimal? DiscountPercent { get; set; }


    [JsonPropertyName("cancel_reson")]
    public string? CancelReason { get; set; }

}

public class SalesforceOrderLine
{
	[JsonPropertyName("order_id")]
	public string OrderId { get; set; }

	[JsonPropertyName("product_id")]
    public string ProductId { get; set; }

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }

	[JsonPropertyName("weight_in_gram")]
    public decimal? WeightInGram { get; set; }
	[JsonPropertyName("quantity")]
    public int Quantity { get; set; }

	[JsonPropertyName("product_price")]
    public decimal ProductPrice { get; set; }


	[JsonPropertyName("disc_Percent")]
    public decimal DiscountPercentage { get; set; }


	[JsonPropertyName("disc_Amount")]
    public decimal DiscountAmount { get; set; }

	[JsonPropertyName("oli_total")]
    public decimal OliTotal { get; set; }


	[JsonPropertyName("product_currency")]
    public string ProductCurrency { get; set; }

}

