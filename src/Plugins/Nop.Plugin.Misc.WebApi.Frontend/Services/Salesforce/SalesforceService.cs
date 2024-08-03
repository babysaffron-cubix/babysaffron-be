using System;
using System.Xml.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Salesforce;
using Nop.Services.Common;
using Nop.Services.Common.Salesforce;
using Nop.Services.Customers;
using Nop.Web.Factories;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Attributes;
using Nop.Services.Orders;
using Nop.Services.Catalog;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Nop.Web.Models.Common;
using Nop.Web.Models.Catalog;


namespace Nop.Plugin.Misc.WebApi.Frontend.Services;

public class SalesforceService : ISalesforceService
{

    #region Fields
    private readonly ICustomerService _customerService;
    private readonly IAddressService _addressService;
    private readonly ICustomerModelFactory _customerModelFactory;
    private readonly ISalesforceCommonService _salesforceCommonService;
    private readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    private readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly IProductModelFactory _productModelFactory;

    private static readonly char[] _separator = [','];
    private static readonly string _weightAttributeName = "Weight";

    #endregion


    #region Ctor

    public SalesforceService(ICustomerService customerService,
        IAddressService addressService,
        ICustomerModelFactory customerModelFactory,
        ISalesforceCommonService salesforceCommonService,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IOrderService orderService,
        IProductService productService,
        IProductModelFactory productModelFactory)
	{
        _customerService = customerService;
        _addressService = addressService;
        _customerModelFactory = customerModelFactory;
        _salesforceCommonService = salesforceCommonService;
        _customerAttributeService = customerAttributeService;
        _customerAttributeParser = customerAttributeParser;
        _orderService = orderService;
        _productService = productService;
        _productModelFactory = productModelFactory;
    }

    #endregion

    #region Methods 

    /// <summary>
    /// for the current order, get all details and send it to salesforce
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public async Task<SalesforceResponse> CreateSalesforceOrder(int orderId)
    {
        try
        {
            return await _customerModelFactory.PrepareSalesforceResponseModelForOrders(orderId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    public async Task<string> GetSalesforceToken()
    {
        var response = await _salesforceCommonService.GetSalesforceToken();
        return response;
    }

    /// <summary>
    /// For the current customer, make api call to send all the details updates to salesforce. If the customer does not exist in salesforce, then it is created
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public async Task<SalesforceContactUpsertResponse> UpsertSalesforceCustomerAsync(int customerId, int? addressId = null)
    {
        try
        {
            return await _customerModelFactory.PrepareSalesforceResponseModelForCustomer(customerId, addressId);
        }
        catch (Exception ex)
        {
            throw;
        }
      
    }


    /// <summary>
    /// Get existing SFDCNumber for the current customer, if any
    /// </summary>
    /// <param name="customCustomerAttributeXML"></param>
    /// <returns></returns>
    private async Task<string> GetSFDCNumber(String customCustomerAttributeXML)
    {
        return await Task.Run(() =>
        {

            XDocument xmlDoc = XDocument.Parse(customCustomerAttributeXML);

            var customerAttributes = xmlDoc
                                    .Element("Attributes")
                                    .Elements("CustomerAttribute");

            foreach (var customerAttribute in customerAttributes)
            {
                XElement valueElement = customerAttribute
                                        .Element("CustomerAttributeValue")
                                        .Element("Value");

                string value = valueElement.Value.Trim();

                if (value.Contains("CON"))
                {
                    return value;
                }

            }
            return null;
        });
    }


    /// <summary>
    /// Get addressType attribute for the current customer address
    /// </summary>
    /// <param name="customAttributesXML"></param>
    /// <returns></returns>
    private async Task<string> GetAddressType(String customAttributesXML)
    {
        return await Task.Run(() =>
        {

            return customAttributesXML.Split(":")[1].Trim();

        });
    }


    /// <summary>
    /// Parse SFDCNumber to xml, to save it for the customer
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    protected virtual async Task<string> ParseCustomCustomerAttributesAsync(IDictionary<string, string> form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;
        var attributes = await _customerAttributeService.GetAllAttributesAsync();
        foreach (var attribute in attributes)
        {
            var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }
                    }

                    break;
                case AttributeControlType.Checkboxes:
                    {
                        var cblAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                            foreach (var item in cblAttributes.Split(_separator, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                    }

                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //load read-only (already server-side selected) values
                        var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
                        foreach (var selectedAttributeId in attributeValues
                                     .Where(v => v.IsPreSelected)
                                     .Select(v => v.Id)
                                     .ToList())
                            attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                attribute, selectedAttributeId.ToString());
                    }

                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    {
                        var ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.Trim();
                            attributesXml = _customerAttributeParser.AddAttribute(attributesXml,
                                attribute, enteredText);
                        }
                    }

                    break;
                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                //not supported customer attributes
                default:
                    break;
            }
        }

        return attributesXml;
    }


    /// <summary>
    /// Get Weight values from the attribute xml
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static decimal ExtractWeightValue(string input)
    {
        // Regular expression to match one or more digits at the beginning of the string
        var match = Regex.Match(input, @"^\d+");

        if (match.Success)
        {
            decimal weightValue = 0;
            Decimal.TryParse(match.Value, out weightValue);
            return weightValue;
        }
        else
        {
            throw new ArgumentException("No numeric value found in the input string.");
        }
    }


    /// <summary>
    /// Customer address mapper 
    /// </summary>
    /// <param name="addressModel"></param>
    /// <returns></returns>
    private async Task<SalesforceAddress> GetSalesforceAddressMapper(AddressModel addressModel)
    {
        try
        {
            return new SalesforceAddress()
            {
                Street = addressModel.Address1,
                Street2 = addressModel.Address2,
                State = addressModel.StateProvinceName,
                PostalCode = addressModel.ZipPostalCode,
                PhoneNo = addressModel.PhoneNumber,
                Name = $"{addressModel.FirstName} {addressModel.LastName}",
                Country = addressModel.CountryName,
                City = addressModel.City,
                AddressSaveAs = !String.IsNullOrEmpty(addressModel.FormattedCustomAddressAttributes) ? await GetAddressType(addressModel.FormattedCustomAddressAttributes) : null
            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Salesforce order Mapper
    /// </summary>
    /// <param name="order"></param>
    /// <param name="customCustomerAttributeXML"></param>
    /// <returns></returns>
    private async Task<SalesforceOrder> GetSalesforceOrderMapper(Order order, string customCustomerAttributeXML)
    {
        try
        {
            return new SalesforceOrder()
            {
                UserId = !String.IsNullOrEmpty(customCustomerAttributeXML) ? await GetSFDCNumber(customCustomerAttributeXML) : null,
                Transactionid = order.CardName, //we are saving razorpay paymentid in this col
                OrderTotal = order.OrderSubtotalExclTax,
                OrderStatus = "Booked",
                OrderNumber = order.Id.ToString(),
                OrderDate = order.CreatedOnUtc.ToString("yyyy-MM-dd"),
                OrderCurrency = order.CustomerCurrencyCode,
                DiscountPercent = 0 //TODO: calculate this

            };
        }
        catch (Exception ex)
        {
            throw;
        }
    }


    /// <summary>
    /// Get weight value for a product
    /// </summary>
    /// <param name="productOverviewModel"></param>
    /// <returns></returns>
    private decimal? GetWeightValue(ProductOverviewModel productOverviewModel)
    {
        try
        {
            var weightString = productOverviewModel.ProductSpecificationModel.Groups.SelectMany(x => x.Attributes)
                               .Where(attr => attr.Name == _weightAttributeName)
                               .Select(attr => attr.Values.FirstOrDefault()?.ValueRaw)
                               .Where(val => val != null).FirstOrDefault();
            var weightValue = ExtractWeightValue(weightString);
            return weightValue;
        }

        catch (Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// Get order details in salesforce format for the current order
    /// </summary>
    /// <param name="orderDetails"></param>
    /// <param name="productOverviewModel"></param>
    /// <param name="salesforceOrderComponents"></param>
    /// <param name="order"></param>
    private void PopulateOrderDetails(IList<OrderItem> orderDetails, List<ProductOverviewModel> productOverviewModel, SalesforceOrderComponents salesforceOrderComponents, Order order)
    {
        if (orderDetails != null)
        {
            salesforceOrderComponents.OrdLine = new List<SalesforceOrderLine>();
            foreach (OrderItem item in orderDetails)
            {
                var currentProduct = productOverviewModel.Where(x => x.Id == item.ProductId).FirstOrDefault();

                var weightValue = GetWeightValue(currentProduct);

                salesforceOrderComponents.OrdLine.Add(new SalesforceOrderLine()
                {
                    OrderId = order.Id.ToString(),
                    ProductId = currentProduct.Sku,
                    ProductName = currentProduct.Name,
                    WeightInGram = weightValue,
                    Quantity = item.Quantity,
                    ProductPrice = item.UnitPriceExclTax,
                    DiscountPercentage = 0,
                    DiscountAmount = 0,
                    OliTotal = item.PriceExclTax,
                    ProductCurrency = order.CustomerCurrencyCode
                });
            }
        }
    }

    #endregion
}

