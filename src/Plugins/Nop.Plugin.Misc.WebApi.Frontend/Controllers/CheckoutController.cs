using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Plugin.Misc.WebApi.Frontend.Dto;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Checkout;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Common;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.ShoppingCart;
using Nop.Plugin.Misc.WebApi.Frontend.Services;
using Nop.Services.Attributes;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Factories;
using Nop.Web.Models.Checkout;
using Nop.Web.Models.Common;
using Nop.Web.Models.ShoppingCart;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

public partial class CheckoutController : BaseNopWebApiFrontendController
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    protected readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    protected readonly IAddressService _addressService;
    protected readonly ICheckoutModelFactory _checkoutModelFactory;
    protected readonly ICheckoutService _checkoutService;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly ILogger _logger;
    protected readonly IOrderProcessingService _orderProcessingService;
    protected readonly IOrderService _orderService;
    protected readonly IPaymentPluginManager _paymentPluginManager;
    protected readonly IPaymentService _paymentService;
    protected readonly IShippingService _shippingService;
    protected readonly IShoppingCartModelFactory _shoppingCartModelFactory;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStoreContext _storeContext;
    protected readonly ITaxService _taxService;
    protected readonly IWorkContext _workContext;
    protected readonly OrderSettings _orderSettings;
    protected readonly PaymentSettings _paymentSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly ShippingSettings _shippingSettings;
    protected readonly TaxSettings _taxSettings;

    private static readonly string[] _separator = ["___"];

    #endregion

    #region Ctor

    public CheckoutController(AddressSettings addressSettings,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService,
        IAddressService addressService,
        ICheckoutModelFactory checkoutModelFactory,
        ICheckoutService checkoutService,
        ICountryService countryService,
        ICustomerService customerService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        ILogger logger,
        IOrderProcessingService orderProcessingService,
        IOrderService orderService,
        IPaymentPluginManager paymentPluginManager,
        IPaymentService paymentService,
        IShippingService shippingService,
        IShoppingCartModelFactory shoppingCartModelFactory,
        IShoppingCartService shoppingCartService,
        IStoreContext storeContext,
        ITaxService taxService,
        IWorkContext workContext,
        OrderSettings orderSettings,
        PaymentSettings paymentSettings,
        RewardPointsSettings rewardPointsSettings,
        ShippingSettings shippingSettings,
        TaxSettings taxSettings
    )
    {
        _addressSettings = addressSettings;
        _addressAttributeParser = addressAttributeParser;
        _addressAttributeService = addressAttributeService;
        _addressService = addressService;
        _checkoutModelFactory = checkoutModelFactory;
        _checkoutService = checkoutService;
        _countryService = countryService;
        _customerService = customerService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _logger = logger;
        _orderProcessingService = orderProcessingService;
        _orderService = orderService;
        _paymentPluginManager = paymentPluginManager;
        _paymentService = paymentService;
        _shippingService = shippingService;
        _shoppingCartModelFactory = shoppingCartModelFactory;
        _shoppingCartService = shoppingCartService;
        _storeContext = storeContext;
        _taxService = taxService;
        _workContext = workContext;
        _orderSettings = orderSettings;
        _paymentSettings = paymentSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _shippingSettings = shippingSettings;
        _taxSettings = taxSettings;
    }

    #endregion

    #region Utilities

    protected virtual async Task<bool> IsMinimumOrderPlacementIntervalValidAsync(Customer customer)
    {
        //prevent 2 orders being placed within an X seconds time frame
        if (_orderSettings.MinimumOrderPlacementInterval == 0)
            return true;

        var store = await _storeContext.GetCurrentStoreAsync();

        var lastOrder = (await _orderService.SearchOrdersAsync(storeId: store.Id,
                customerId: customer.Id, pageSize: 1))
            .FirstOrDefault();

        if (lastOrder == null)
            return true;

        var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;

        return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
    }

    /// <summary>
    /// Parses the value indicating whether the "pickup in store" is allowed
    /// </summary>
    /// <param name="form">The form</param>
    /// <returns>The value indicating whether the "pickup in store" is allowed</returns>
    protected virtual bool ParsePickupInStore(IDictionary<string, string> form)
    {
        var pickupInStore = false;

        var pickupInStoreParameter = form["PickupInStore"];
        if (!string.IsNullOrWhiteSpace(pickupInStoreParameter))
            _ = bool.TryParse(pickupInStoreParameter, out pickupInStore);

        return pickupInStore;
    }

    /// <summary>
    /// Parses the pickup option
    /// </summary>
    /// <param name="cart">Shopping Cart</param>
    /// <param name="form">The form</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the pickup option
    /// </returns>
    protected virtual async Task<PickupPoint> ParsePickupOptionAsync(IList<ShoppingCartItem> cart, IDictionary<string, string> form)
    {
        var pickupPoint = form["pickup-points-id"].Split(_separator, StringSplitOptions.None);

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var address = customer.BillingAddressId.HasValue
            ? await _addressService.GetAddressByIdAsync(customer.BillingAddressId.Value)
            : null;

        var selectedPoint = (await _shippingService.GetPickupPointsAsync(cart, address,
            customer, pickupPoint[1], store.Id)).PickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0])) ?? throw new Exception("Pickup point is not allowed");

        return selectedPoint;
    }

    /// <summary>
    /// Saves the pickup option
    /// </summary>
    /// <param name="pickupPoint">The pickup option</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task SavePickupOptionAsync(PickupPoint pickupPoint)
    {
        var name = !string.IsNullOrEmpty(pickupPoint.Name) ?
            string.Format(await _localizationService.GetResourceAsync("Checkout.PickupPoints.Name"), pickupPoint.Name) :
            await _localizationService.GetResourceAsync("Checkout.PickupPoints.NullName");
        var pickUpInStoreShippingOption = new ShippingOption
        {
            Name = name,
            Rate = pickupPoint.PickupFee,
            Description = pickupPoint.Description,
            ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
            IsPickupInStore = true
        };

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, pickUpInStoreShippingOption, store.Id);
        await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.SelectedPickupPointAttribute, pickupPoint, store.Id);
    }

    /// <summary>
    /// Get custom address attributes from the passed form
    /// </summary>
    /// <param name="form">Form values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the attributes in XML format
    /// </returns>
    protected virtual async Task<string> ParseCustomAddressAttributesAsync(IDictionary<string, string> form)
    {
        ArgumentNullException.ThrowIfNull(form);

        var attributesXml = string.Empty;

        foreach (var attribute in await _addressAttributeService.GetAllAttributesAsync())
        {
            var controlId = string.Format(NopCommonDefaults.AddressAttributeControlName, attribute.Id);
            var attributeValues = form[controlId];
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                    if (!StringValues.IsNullOrEmpty(attributeValues) && int.TryParse(attributeValues, out var value) && value > 0)
                        attributesXml = _addressAttributeParser.AddAttribute(attributesXml, attribute, value.ToString());
                    break;

                case AttributeControlType.Checkboxes:
                    if (!StringValues.IsNullOrEmpty(attributeValues))
                        foreach (var attributeValue in attributeValues.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            if (int.TryParse(attributeValue, out value) && value > 0)
                                attributesXml = _addressAttributeParser.AddAttribute(attributesXml, attribute, value.ToString());

                    break;

                case AttributeControlType.ReadonlyCheckboxes:
                    //load read-only (already server-side selected) values
                    var addressAttributeValues = await _addressAttributeService.GetAttributeValuesAsync(attribute.Id);
                    foreach (var addressAttributeValue in addressAttributeValues)
                        if (addressAttributeValue.IsPreSelected)
                            attributesXml = _addressAttributeParser.AddAttribute(attributesXml, attribute, addressAttributeValue.Id.ToString());

                    break;

                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                    if (!StringValues.IsNullOrEmpty(attributeValues))
                        attributesXml = _addressAttributeParser.AddAttribute(attributesXml, attribute, attributeValues.Trim());
                    break;

                case AttributeControlType.Datepicker:
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.FileUpload:
                default:
                    break;
            }
        }

        return attributesXml;
    }

    /// <summary>
    /// Save customer VAT number
    /// </summary>
    /// <param name="fullVatNumber">The full VAT number</param>
    /// <param name="customer">The customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the Vat number error if exists
    /// </returns>
    protected virtual async Task<string> SaveCustomerVatNumberAsync(string fullVatNumber, Customer customer)
    {
        var (vatNumberStatus, _, _) = await _taxService.GetVatNumberStatusAsync(fullVatNumber);
        customer.VatNumberStatus = vatNumberStatus;
        customer.VatNumber = fullVatNumber;
        await _customerService.UpdateCustomerAsync(customer);

        if (vatNumberStatus != VatNumberStatus.Valid && !string.IsNullOrEmpty(fullVatNumber))
        {
            var warning = await _localizationService.GetResourceAsync("Checkout.VatNumber.Warning");
            return string.Format(warning, await _localizationService.GetLocalizedEnumAsync(vatNumberStatus));
        }

        return string.Empty;
    }

    protected async Task<IActionResult> EditAddressAsync(AddressModelDto addressModel, Func<Customer, IList<ShoppingCartItem>, Address, Task<IActionResult>> getResult)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
            if (!cart.Any())
                return BadRequest("Your cart is empty");

            //find address (ensure that it belongs to the current customer)
            var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressModel.Id);

            if (address == null)
                return NotFound($"Address by id={addressModel.Id} not found.");

            address = addressModel.FromDto<AddressModel>().ToEntity();

            await _addressService.UpdateAddressAsync(address);

            return await getResult(customer, cart, address);
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return BadRequest(exc.Message);
        }
    }

    protected async Task<IActionResult> DeleteAddressAsync(int addressId, Func<IActionResult> getResult)
    {
        try
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var store = await _storeContext.GetCurrentStoreAsync();

            var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

            if (!cart.Any())
                return BadRequest("Your cart is empty");

            var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);
            if (address != null)
            {
                await _customerService.RemoveCustomerAddressAsync(customer, address);
                await _customerService.UpdateCustomerAsync(customer);
                await _addressService.DeleteAddressAsync(address);
            }

            return getResult();
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc, await _workContext.GetCurrentCustomerAsync());
            return BadRequest(exc.Message);
        }
    }

    protected async Task<(Customer customer, Store store, IList<ShoppingCartItem> cart, IActionResult actionResult)> ValidateRequestAsync()
    {
        if (_orderSettings.CheckoutDisabled)
            return (null, null, null, NotFound($"The setting {nameof(_orderSettings.CheckoutDisabled)} is true."));

        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        if (!cart.Any())
            return (null, null, null, BadRequest("Your cart is empty"));

        if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
            return (null, null, null, BadRequest("Anonymous checkout is not allowed"));

        return (customer, store, cart, null);
    }

    #endregion

    #region Methods

    #region Addresses

    /// <summary>
    /// Get specified Address by addressId
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    [HttpGet("{addressId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(AddressModelDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAddressById(int addressId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);

        if (address == null)
            return NotFound($"Address by id={addressId} not found.");

        return Ok(address.ToModel<AddressModel>().ToDto<AddressModelDto>());
    }

    #region Billing address

    /// <summary>
    /// Prepare billing address model
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BillingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> BillingAddress()
    {
        //validation
        var (_, _, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //model
        var model = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true);

        //check whether "billing address" step is enabled
        if (_orderSettings.DisableBillingAddressCheckoutStep && model.ExistingAddresses.Any())
        {
            if (model.ExistingAddresses.Any())
                //choose the first one
                return await SelectBillingAddress(model.ExistingAddresses.First().Id);

            TryValidateModel(model);
            TryValidateModel(model.BillingNewAddress);

            return await NewBillingAddress(new BaseModelDtoRequest<CheckoutBillingAddressModelDto> { Model = model.ToDto<CheckoutBillingAddressModelDto>() });
        }

        return Ok(new BillingAddressResponse { Model = model.ToDto<CheckoutBillingAddressModelDto>() });
    }

    /// <summary>
    /// Select billing address
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    /// <param name="shipToSameAddress">A value indicating "Ship to the same address" option is enabled</param>
    [HttpGet("{addressId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BillingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SelectBillingAddress(int addressId, [FromQuery] bool shipToSameAddress = false)
    {
        //validation
        if (_orderSettings.CheckoutDisabled)
            return NotFound($"The setting {nameof(_orderSettings.CheckoutDisabled)} is true.");

        var customer = await _workContext.GetCurrentCustomerAsync();
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);

        if (address == null)
            return NotFound($"Address by id={addressId} not found.");

        customer.BillingAddressId = address.Id;
        await _customerService.UpdateCustomerAsync(customer);

        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        //ship to the same address?
        //by default Shipping is available if the country is not specified
        var shippingAllowed = !_addressSettings.CountryEnabled || ((await _countryService.GetCountryByAddressAsync(address))?.AllowsShipping ?? false);
        if (_shippingSettings.ShipToSameAddress && shipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart) && shippingAllowed)
        {
            customer.ShippingAddressId = customer.BillingAddressId;
            await _customerService.UpdateCustomerAsync(customer);
            //reset selected shipping method (in case if "pick up in store" was selected)
            await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);

            //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
            return Ok(new BillingAddressResponse { RedirectToMethod = "ShippingMethod" });
        }

        return Ok(new BillingAddressResponse { RedirectToMethod = "ShippingAddress" });
    }

    /// <summary>
    /// New billing address
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BillingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> NewBillingAddress([FromBody] BaseModelDtoRequest<CheckoutBillingAddressModelDto> request)
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        var model = request.Model.FromDto<CheckoutBillingAddressModel>();

        if (await _customerService.IsGuestAsync(customer) && _taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
        {
            var warning = await SaveCustomerVatNumberAsync(model.VatNumber, customer);
            if (!string.IsNullOrEmpty(warning))
                return BadRequest(warning);
        }

        //custom address attributes
        var customAttributes = await ParseCustomAddressAttributesAsync(request.Form);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

        var errors = new List<string>();
        errors.AddRange(customAttributeWarnings);

        var newAddress = model.BillingNewAddress;

        if (!errors.Any())
        {
            //try to find an address with the same values (don't duplicate records)
            var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                newAddress.Address1, newAddress.Address2, newAddress.City,
                newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                newAddress.CountryId, customAttributes);

            if (address == null)
            {
                //address is not found. let's create a new one
                address = newAddress.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;

                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);

                await _customerService.InsertCustomerAddressAsync(customer, address);
            }

            customer.BillingAddressId = address.Id;

            await _customerService.UpdateCustomerAsync(customer);

            //ship to the same address?
            if (_shippingSettings.ShipToSameAddress && model.ShipToSameAddress && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                customer.ShippingAddressId = customer.BillingAddressId;
                await _customerService.UpdateCustomerAsync(customer);

                //reset selected shipping method (in case if "pick up in store" was selected)
                await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
                await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);

                //limitation - "Ship to the same address" doesn't properly work in "pick up in store only" case (when no shipping plugins are available) 
                return Ok(new BillingAddressResponse { RedirectToMethod = "ShippingMethod" });
            }

            return Ok(new BillingAddressResponse { RedirectToMethod = "ShippingAddress" });
        }

        //If we got this far, something failed, redisplay form
        model = await _checkoutModelFactory.PrepareBillingAddressModelAsync(cart,
            selectedCountryId: newAddress.CountryId,
            overrideAttributesXml: customAttributes);

        return Ok(new BillingAddressResponse { Model = model.ToDto<CheckoutBillingAddressModelDto>() });
    }

    /// <summary>
    /// Save edited address
    /// </summary>        
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BillingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SaveEditBillingAddress([FromBody] CheckoutBillingAddressModelDto requestModel)
    {
        try
        {
            return await EditAddressAsync(requestModel.BillingNewAddress, async (customer, _, address) =>
            {
                customer.BillingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(customer);

                return Ok(new BillingAddressResponse
                {
                    RedirectToMethod = "BillingAddress"
                });
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc);
            return BadRequest(exc.Message);
        }
    }

    /// <summary>
    /// Delete edited address
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    [HttpDelete("{addressId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BillingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> DeleteEditBillingAddress(int addressId)
    {
        return await DeleteAddressAsync(addressId, () => Ok(new BillingAddressResponse
        {
            RedirectToMethod = "BillingAddress"
        }));
    }

    #endregion

    #region Shipping address

    /// <summary>
    /// Prepare shipping address model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ShippingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ShippingAddress()
    {
        //validation
        var (_, _, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            return Ok(new ShippingAddressResponse { RedirectToMethod = "ShippingMethod" });

        //model
        var model = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart, prePopulateNewAddressWithCustomerFields: true);

        return Ok(new ShippingAddressResponse { Model = model.ToDto<CheckoutShippingAddressModelDto>() });
    }

    /// <summary>
    /// Select shipping address
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    [HttpGet("{addressId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ShippingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SelectShippingAddress(int addressId)
    {
        //validation
        if (_orderSettings.CheckoutDisabled)
            return NotFound($"The setting {nameof(_orderSettings.CheckoutDisabled)} is true.");

        var customer = await _workContext.GetCurrentCustomerAsync();
        var address = await _customerService.GetCustomerAddressAsync(customer.Id, addressId);

        if (address == null)
            return NotFound($"Address by id={addressId} not found.");

        customer.ShippingAddressId = address.Id;
        await _customerService.UpdateCustomerAsync(customer);

        if (_shippingSettings.AllowPickupInStore)
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            //set value indicating that "pick up in store" option has not been chosen
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
        }

        return Ok(new ShippingAddressResponse { RedirectToMethod = "ShippingMethod" });
    }

    /// <summary>
    /// New shipping address
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ShippingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> NewShippingAddress([FromBody] BaseModelDtoRequest<CheckoutShippingAddressModelDto> request)
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            return Ok(new ShippingAddressResponse { RedirectToMethod = "ShippingMethod" });

        //pickup point
        if (_shippingSettings.AllowPickupInStore && !_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
        {
            var pickupInStore = ParsePickupInStore(request.Form);
            if (pickupInStore)
            {
                var pickupOption = await ParsePickupOptionAsync(cart, request.Form);
                await SavePickupOptionAsync(pickupOption);

                return Ok(new ShippingAddressResponse { RedirectToMethod = "PaymentMethod" });
            }

            //set value indicating that "pick up in store" option has not been chosen
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, null, store.Id);
        }

        //custom address attributes
        var customAttributes = await ParseCustomAddressAttributesAsync(request.Form);
        var customAttributeWarnings = await _addressAttributeParser.GetAttributeWarningsAsync(customAttributes);

        var errors = new List<string>();
        errors.AddRange(customAttributeWarnings);

        //var model = request.Model.FromDto<CheckoutShippingAddressModelDto>();
        var newAddress = request.Model.ShippingNewAddress;

        if (!errors.Any())
        {
            //try to find an address with the same values (don't duplicate records)
            var address = _addressService.FindAddress((await _customerService.GetAddressesByCustomerIdAsync(customer.Id)).ToList(),
                newAddress.FirstName, newAddress.LastName, newAddress.PhoneNumber,
                newAddress.Email, newAddress.FaxNumber, newAddress.Company,
                newAddress.Address1, newAddress.Address2, newAddress.City,
                newAddress.County, newAddress.StateProvinceId, newAddress.ZipPostalCode,
                newAddress.CountryId, customAttributes);

            if (address == null)
            {
                address = newAddress.FromDto<AddressModel>().ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;

                await _addressService.InsertAddressAsync(address);

                await _customerService.InsertCustomerAddressAsync(customer, address);
            }

            customer.ShippingAddressId = address.Id;
            await _customerService.UpdateCustomerAsync(customer);

            return Ok(new ShippingAddressResponse { RedirectToMethod = "ShippingMethod" });
        }

        //if we got this far, something failed, redisplay form
        var model = await _checkoutModelFactory.PrepareShippingAddressModelAsync(cart,
            selectedCountryId: newAddress.CountryId,
            overrideAttributesXml: customAttributes);
        return Ok(new ShippingAddressResponse { Model = model.ToDto<CheckoutShippingAddressModelDto>() });
    }

    /// <summary>
    /// Save edited address
    /// </summary>        
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ShippingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SaveEditShippingAddress([FromBody] CheckoutShippingAddressModelDto requestModel)
    {
        try
        {
            return await EditAddressAsync(requestModel.ShippingNewAddress, async (customer, _, address) =>
            {
                customer.ShippingAddressId = address.Id;
                await _customerService.UpdateCustomerAsync(customer);

                return Ok(new ShippingAddressResponse
                {
                    RedirectToMethod = "ShippingAddress"
                });
            });
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc);
            return BadRequest(exc.Message);
        }
    }

    /// <summary>
    /// Delete edited address
    /// </summary>
    /// <param name="addressId">Address identifier</param>
    [HttpDelete("{addressId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ShippingAddressResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> DeleteEditShippingAddress(int addressId)
    {
        return await DeleteAddressAsync(addressId, () => Ok(new ShippingAddressResponse
        {
            RedirectToMethod = "ShippingAddress"
        }));
    }

    #endregion

    #endregion

    #region Shipping method

    /// <summary>
    /// Prepare shipping method model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ShippingMethodResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ShippingMethod()
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
        {
            await _genericAttributeService.SaveAttributeAsync<ShippingOption>(customer, NopCustomerDefaults.SelectedShippingOptionAttribute, null, store.Id);
            return Ok(new ShippingMethodResponse { RedirectToMethod = "PaymentMethod" });
        }

        //check if pickup point is selected on the shipping address step
        if (!_orderSettings.DisplayPickupInStoreOnShippingMethodPage)
        {
            var selectedPickUpPoint = await _genericAttributeService
                .GetAttributeAsync<PickupPoint>(customer, NopCustomerDefaults.SelectedPickupPointAttribute, store.Id);
            if (selectedPickUpPoint != null)
                return Ok(new ShippingMethodResponse { RedirectToMethod = "PaymentMethod" });
        }

        //model
        var model = await _checkoutModelFactory.PrepareShippingMethodModelAsync(cart, await _customerService.GetCustomerShippingAddressAsync(await _workContext.GetCurrentCustomerAsync()));

        if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
            model.ShippingMethods.Count == 1)
        {
            //if we have only one shipping method, then a customer doesn't have to choose a shipping method
            await _genericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.SelectedShippingOptionAttribute,
                model.ShippingMethods.First().ShippingOption,
                store.Id);

            return Ok(new ShippingMethodResponse { RedirectToMethod = "PaymentMethod" });
        }

        return Ok(new ShippingMethodResponse { Model = model.ToDto<CheckoutShippingMethodModelDto>() });
    }

    /// <summary>
    /// Select shipping method
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutRedirectResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SelectShippingMethod([FromBody] IDictionary<string, string> form,
        [FromQuery, Required] string shippingOption)
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        if (!await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
        {
            await _genericAttributeService.SaveAttributeAsync<ShippingOption>(
                await _workContext.GetCurrentCustomerAsync(),
                NopCustomerDefaults.SelectedShippingOptionAttribute, null,
                store.Id);

            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentMethod" });
        }

        //pickup point
        if (_shippingSettings.AllowPickupInStore)
        {
            var pickupInStore = ParsePickupInStore(form);
            if (pickupInStore)
            {
                var pickupOption = await ParsePickupOptionAsync(cart, form);
                await SavePickupOptionAsync(pickupOption);

                return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentMethod" });
            }

            //set value indicating that "pick up in store" option has not been chosen
            await _genericAttributeService.SaveAttributeAsync<PickupPoint>(
                customer, NopCustomerDefaults.SelectedPickupPointAttribute,
                null, store.Id);
        }

        //parse selected method 
        if (string.IsNullOrEmpty(shippingOption))
            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "ShippingMethod" });

        var splittedOption = shippingOption.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
        if (splittedOption.Length != 2)
            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "ShippingMethod" });

        var selectedName = splittedOption[0];
        var shippingRateComputationMethodSystemName = splittedOption[1];

        //find it
        //performance optimization. try cache first
        var shippingOptions = await _genericAttributeService.GetAttributeAsync<List<ShippingOption>>(
            customer,
            NopCustomerDefaults.OfferedShippingOptionsAttribute, store.Id);
        if (shippingOptions == null || !shippingOptions.Any())
        {
            //not found? let's load them using shipping service
            shippingOptions = (await _shippingService.GetShippingOptionsAsync(cart,
                await _customerService.GetCustomerShippingAddressAsync(customer),
                customer, shippingRateComputationMethodSystemName,
                store.Id)).ShippingOptions.ToList();
        }
        else
        {
            //loaded cached results. let's filter result by a chosen shipping rate computation method
            shippingOptions = shippingOptions.Where(so =>
                    so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName,
                        StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        var shippingOpt = shippingOptions
            .Find(so => !string.IsNullOrEmpty(so.Name) &&
                so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
        if (shippingOpt == null)
            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "ShippingMethod" });

        //save
        await _genericAttributeService.SaveAttributeAsync(customer,
            NopCustomerDefaults.SelectedShippingOptionAttribute, shippingOpt,
            store.Id);

        return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentMethod" });
    }

    #endregion

    #region Payment method

    /// <summary>
    /// Prepare payment method model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(PaymentMethodResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> PaymentMethod()
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //check whether payment work flow is required
        //we ignore reward points during cart total calculation
        var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart, false);
        if (!isPaymentWorkflowRequired)
        {
            await _genericAttributeService.SaveAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, null, store.Id);

            return Ok(new PaymentMethodResponse { RedirectToMethod = "PaymentInfo" });
        }

        //filter by country
        var filterByCountryId = 0;
        if (_addressSettings.CountryEnabled)
            filterByCountryId = (await _customerService.GetCustomerBillingAddressAsync(customer))?.CountryId ?? 0;

        //model
        var paymentMethodModel = await _checkoutModelFactory.PreparePaymentMethodModelAsync(cart, filterByCountryId);

        if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
            paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
        {
            //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
            //so customer doesn't have to choose a payment method
            await _genericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute,
                paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName,
                store.Id);

            return Ok(new PaymentMethodResponse { RedirectToMethod = "PaymentInfo" });
        }

        return Ok(new PaymentMethodResponse { Model = paymentMethodModel.ToDto<CheckoutPaymentMethodModelDto>() });
    }

    /// <summary>
    /// Select payment method
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutRedirectResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SelectPaymentMethod([FromQuery, Required] string paymentMethod, [FromQuery] bool useRewardPoints = false)
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //reward points
        if (_rewardPointsSettings.Enabled)
            await _genericAttributeService.SaveAttributeAsync(customer,
                NopCustomerDefaults.UseRewardPointsDuringCheckoutAttribute, useRewardPoints,
                store.Id);

        //Check whether payment work flow is required
        var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
        if (!isPaymentWorkflowRequired)
        {
            await _genericAttributeService.SaveAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, null,
                store.Id);

            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentInfo" });
        }

        //payment method 
        if (string.IsNullOrEmpty(paymentMethod) ||
            !await _paymentPluginManager.IsPluginActiveAsync(paymentMethod,
                customer, store.Id))
            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentMethod" });

        //save
        await _genericAttributeService.SaveAttributeAsync(customer,
            NopCustomerDefaults.SelectedPaymentMethodAttribute, paymentMethod,
            store.Id);

        return Ok(new CheckoutRedirectResponse { RedirectToMethod = "PaymentInfo" });
    }

    #endregion

    #region Payment info

    /// <summary>
    /// Prepare payment info model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutPaymentInfoModelDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> PaymentInfo()
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //check whether payment work flow is required
        var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);

        if (!isPaymentWorkflowRequired)
            return Ok(new PaymentInfoResponse { RedirectToMethod = "Confirm" });

        //load payment method
        var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
        var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(paymentMethodSystemName,
            customer, store.Id);

        if (paymentMethod == null)
            return NotFound("Payment method is not found.");

        //check whether payment info should be skipped
        if (paymentMethod.SkipPaymentInfo ||
            (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection && _paymentSettings.SkipPaymentInfoStepForRedirectionPaymentMethods))
        {
            //skip payment info page
            var paymentInfo = new ProcessPaymentRequest();

            //paymentInfo save
            await _checkoutService.SavePaymentInfoAsync(paymentInfo);

            return Ok(new PaymentInfoResponse { RedirectToMethod = "Confirm" });
        }

        //model
        var paymentInfoModel = await _checkoutModelFactory.PreparePaymentInfoModelAsync(paymentMethod);

        return Ok(new PaymentInfoResponse { CheckoutPaymentInfoModel = paymentInfoModel.ToDto<CheckoutPaymentInfoModelDto>() });
    }

    /// <summary>
    /// Enter payment Info
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutRedirectResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> EnterPaymentInfo([FromBody] IDictionary<string, string> form)
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //check whether payment workflow is required
        var isPaymentWorkflowRequired = await _orderProcessingService.IsPaymentWorkflowRequiredAsync(cart);
        if (!isPaymentWorkflowRequired)
            return Ok(new CheckoutRedirectResponse { RedirectToMethod = "Confirm" });

        //load payment method
        var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
            NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
        var paymentMethod = await _paymentPluginManager
            .LoadPluginBySystemNameAsync(paymentMethodSystemName, customer, store.Id);

        if (paymentMethod == null)
            return NotFound("Payment method is not found.");

        var warnings = await paymentMethod.ValidatePaymentFormAsync(new FormCollection(form.ToDictionary(i => i.Key, i => new StringValues(i.Value))));

        if (warnings.Any())
            return BadRequest(warnings);

        //get payment info
        var paymentInfo = await paymentMethod.GetPaymentInfoAsync(new FormCollection(form.ToDictionary(i => i.Key, i => new StringValues(i.Value))));
        //set previous order GUID (if exists)
        await _checkoutService.GenerateOrderGuidAsync(paymentInfo);

        //paymentInfo save
        await _checkoutService.SavePaymentInfoAsync(paymentInfo);

        return Ok(new CheckoutRedirectResponse { RedirectToMethod = "Confirm" });
    }

    #endregion

    #region Order

    /// <summary>
    /// Prepare confirm order model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutConfirmModelDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Confirm()
    {
        //validation
        var (_, _, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //model
        var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);

        var dto = model.ToDto<CheckoutConfirmModelDto>();
        var shoppingCartModel = await _shoppingCartModelFactory.PrepareShoppingCartModelAsync(
            new ShoppingCartModel(),
            cart,
            isEditable: false,
            prepareAndDisplayOrderReviewData: true);
        dto.ShoppingCart = shoppingCartModel.ToDto<ShoppingCartModelDto>();
        var orderTotals = await _shoppingCartModelFactory.PrepareOrderTotalsModelAsync(cart, false);
        dto.OrderTotals = orderTotals.ToDto<OrderTotalsModelDto>();

        return Ok(dto);
    }

    /// <summary>
    /// Confirm order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ConfirmOrderResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ConfirmOrder()
    {
        //validation
        var (customer, store, cart, actionResult) = await ValidateRequestAsync();

        if (actionResult != null)
            return actionResult;

        //model
        var model = await _checkoutModelFactory.PrepareConfirmOrderModelAsync(cart);
        try
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (!await IsMinimumOrderPlacementIntervalValidAsync(customer))
                return BadRequest(await _localizationService.GetResourceAsync("Checkout.MinOrderPlacementInterval"));

            //place order
            var processPaymentRequest = await _checkoutService.GetPaymentInfoAsync();

            await _checkoutService.GenerateOrderGuidAsync(processPaymentRequest);
            processPaymentRequest.StoreId = store.Id;
            processPaymentRequest.CustomerId = customer.Id;
            processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);

            await _checkoutService.SavePaymentInfoAsync(processPaymentRequest);

            var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest);
            if (placeOrderResult.Success)
            {
                await _checkoutService.ClearPaymentInfoAsync();

                //var paymentMethod = await _paymentPluginManager
                //    .LoadPluginBySystemNameAsync(placeOrderResult.PlacedOrder.PaymentMethodSystemName, customer, store.Id);

                //if (paymentMethod?.PaymentMethodType == PaymentMethodType.Redirection)
                //    //should be redirected
                //    return Ok(new ConfirmOrderResponse { RedirectToMethod = "Redirect", Id = placeOrderResult.PlacedOrder.Id });

                //var postProcessPaymentRequest = new PostProcessPaymentRequest
                //{
                //    Order = placeOrderResult.PlacedOrder
                //};

                //await _paymentService.PostProcessPaymentAsync(postProcessPaymentRequest);

                return Ok(new ConfirmOrderResponse { RedirectToMethod = "Completed", Id = placeOrderResult.PlacedOrder.Id });
            }

            foreach (var error in placeOrderResult.Errors)
                model.Warnings.Add(error);
        }
        catch (Exception exc)
        {
            await _logger.WarningAsync(exc.Message, exc);
            model.Warnings.Add(exc.Message);
        }

        //if we got this far, something failed
        return Ok(new ConfirmOrderResponse { Model = model.ToDto<CheckoutConfirmModelDto>() });
    }


    [HttpPost]
    [ProducesResponseType(typeof(PlaceOrderResult), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> MarkPaymentStatusAsPaid([FromBody] RazorpayPaymentSaveRequest razorpayPaymentSaveRequest)
    {
        try
        {
            var response = await _orderProcessingService.MarkPaymentStatusAsPaid(razorpayPaymentSaveRequest);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost]
    [ProducesResponseType(typeof(PlaceOrderResult), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> UpdateOrderStatus([FromBody] RazorpayPaymentSaveRequest razorpayPaymentSaveRequest)
    {
        try
        {
            var response = await _orderProcessingService.UpdateOrderStatus(razorpayPaymentSaveRequest);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Prepare checkout completed model
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutCompletedModelDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Completed([FromQuery] int? orderId)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        //validation
        if (await _customerService.IsGuestAsync(customer) && !_orderSettings.AnonymousCheckoutAllowed)
            return BadRequest("Anonymous checkout is not allowed");

        var store = await _storeContext.GetCurrentStoreAsync();

        //load order by identifier (if provided)
        var order = orderId.HasValue
            ? await _orderService.GetOrderByIdAsync(orderId.Value)
            : (await _orderService.SearchOrdersAsync(storeId: store.Id, customerId: customer.Id, pageSize: 1)).FirstOrDefault();

        if (order == null || order.Deleted || customer.Id != order.CustomerId)
            return NotFound("Order not found or does not meet the requirements.");

        //model
        var model = await _checkoutModelFactory.PrepareCheckoutCompletedModelAsync(order);

        return Ok(model.ToDto<CheckoutCompletedModelDto>());
    }

    #endregion

    #endregion
}