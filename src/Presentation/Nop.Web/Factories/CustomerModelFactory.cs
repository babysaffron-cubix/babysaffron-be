using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Salesforce;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Common.Salesforce;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories;

/// <summary>
/// Represents the customer model factory
/// </summary>
public partial class CustomerModelFactory : ICustomerModelFactory
{
    #region Fields

    protected readonly AddressSettings _addressSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DateTimeSettings _dateTimeSettings;
    protected readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly GdprSettings _gdprSettings;
    protected readonly IAddressModelFactory _addressModelFactory;
    protected readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    protected readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;
    protected readonly IAuthenticationPluginManager _authenticationPluginManager;
    protected readonly ICountryService _countryService;
    protected readonly ICustomerService _customerService;
    protected readonly IDateTimeHelper _dateTimeHelper;
    protected readonly IExternalAuthenticationService _externalAuthenticationService;
    protected readonly IGdprService _gdprService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
    protected readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
    protected readonly IOrderService _orderService;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IProductService _productService;
    protected readonly IReturnRequestService _returnRequestService;
    protected readonly IStateProvinceService _stateProvinceService;
    protected readonly IStoreContext _storeContext;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWorkContext _workContext;
    protected readonly MediaSettings _mediaSettings;
    protected readonly OrderSettings _orderSettings;
    protected readonly RewardPointsSettings _rewardPointsSettings;
    protected readonly SecuritySettings _securitySettings;
    protected readonly TaxSettings _taxSettings;
    protected readonly IAddressService _addressService;
    protected readonly ISalesforceCommonService _salesforceCommonService;
    protected readonly VendorSettings _vendorSettings;
    private readonly IProductModelFactory _productModelFactory;


    private static readonly char[] _separator = [','];
    private static readonly string _weightAttributeName = "Weight";


    #endregion

    #region Ctor

    public CustomerModelFactory(AddressSettings addressSettings,
        CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        DateTimeSettings dateTimeSettings,
        ExternalAuthenticationSettings externalAuthenticationSettings,
        ForumSettings forumSettings,
        GdprSettings gdprSettings,
        IAddressModelFactory addressModelFactory,
        IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService,
        IAuthenticationPluginManager authenticationPluginManager,
        ICountryService countryService,
        ICustomerService customerService,
        IDateTimeHelper dateTimeHelper,
        IExternalAuthenticationService externalAuthenticationService,
        IGdprService gdprService,
        IGenericAttributeService genericAttributeService,
        ILocalizationService localizationService,
        IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
        INewsLetterSubscriptionService newsLetterSubscriptionService,
        IOrderService orderService,
        IPermissionService permissionService,
        IPictureService pictureService,
        IProductService productService,
        IReturnRequestService returnRequestService,
        IStateProvinceService stateProvinceService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        IWorkContext workContext,
        MediaSettings mediaSettings,
        OrderSettings orderSettings,
        RewardPointsSettings rewardPointsSettings,
        SecuritySettings securitySettings,
        TaxSettings taxSettings,
        VendorSettings vendorSettings,
        IAddressService addressService,
        ISalesforceCommonService salesforceCommonService,
        IProductModelFactory productModelFactory)
    {
        _addressSettings = addressSettings;
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _dateTimeSettings = dateTimeSettings;
        _externalAuthenticationService = externalAuthenticationService;
        _externalAuthenticationSettings = externalAuthenticationSettings;
        _forumSettings = forumSettings;
        _gdprSettings = gdprSettings;
        _addressModelFactory = addressModelFactory;
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
        _authenticationPluginManager = authenticationPluginManager;
        _countryService = countryService;
        _customerService = customerService;
        _dateTimeHelper = dateTimeHelper;
        _gdprService = gdprService;
        _genericAttributeService = genericAttributeService;
        _localizationService = localizationService;
        _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
        _newsLetterSubscriptionService = newsLetterSubscriptionService;
        _orderService = orderService;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _productService = productService;
        _returnRequestService = returnRequestService;
        _stateProvinceService = stateProvinceService;
        _storeContext = storeContext;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _orderSettings = orderSettings;
        _rewardPointsSettings = rewardPointsSettings;
        _securitySettings = securitySettings;
        _taxSettings = taxSettings;
        _vendorSettings = vendorSettings;
        _addressService = addressService;
        _salesforceCommonService = salesforceCommonService;
        _productModelFactory = productModelFactory;
    }

    #endregion

    #region Utilities

    /// <returns>A task that represents the asynchronous operation</returns>
    protected virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsent consent, bool accepted)
    {
        ArgumentNullException.ThrowIfNull(consent);

        var requiredMessage = await _localizationService.GetLocalizedAsync(consent, x => x.RequiredMessage);
        return new GdprConsentModel
        {
            Id = consent.Id,
            Message = await _localizationService.GetLocalizedAsync(consent, x => x.Message),
            IsRequired = consent.IsRequired,
            RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
            Accepted = accepted
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the customer info model
    /// </summary>
    /// <param name="model">Customer info model</param>
    /// <param name="customer">Customer</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer info model
    /// </returns>
    public virtual async Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
        bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(customer);

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        var store = await _storeContext.GetCurrentStoreAsync();
        if (!excludeProperties)
        {
            model.VatNumber = customer.VatNumber;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Gender = customer.Gender;
            var dateOfBirth = customer.DateOfBirth;
            if (dateOfBirth.HasValue)
            {
                var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                model.DateOfBirthDay = currentCalendar.GetDayOfMonth(dateOfBirth.Value);
                model.DateOfBirthMonth = currentCalendar.GetMonth(dateOfBirth.Value);
                model.DateOfBirthYear = currentCalendar.GetYear(dateOfBirth.Value);
            }
            model.Company = customer.Company;
            model.StreetAddress = customer.StreetAddress;
            model.StreetAddress2 = customer.StreetAddress2;
            model.ZipPostalCode = customer.ZipPostalCode;
            model.City = customer.City;
            model.County = customer.County;
            model.CountryId = customer.CountryId;
            model.StateProvinceId = customer.StateProvinceId;
            model.Phone = customer.Phone;
            model.Fax = customer.Fax;

            //newsletter
            var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
            model.Newsletter = newsletter != null && newsletter.Active;

            model.Signature = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute);

            model.Email = customer.Email;
            model.Username = customer.Username;
        }
        else
        {
            if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                model.Username = customer.Username;
        }

        if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
            model.EmailToRevalidate = customer.EmailToRevalidate;

        var currentLanguage = await _workContext.GetWorkingLanguageAsync();
        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            if (model.CountryId == 0)
                model.CountryId = _customerSettings.DefaultCountryId ?? 0;

            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }

            }
        }

        model.DisplayVatNumber = _taxSettings.EuVatEnabled;
        model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

        //external authentication
        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
        model.AllowCustomersToRemoveAssociations = _externalAuthenticationSettings.AllowCustomersToRemoveAssociations;
        model.NumberOfExternalAuthenticationProviders = (await _authenticationPluginManager
                .LoadActivePluginsAsync(currentCustomer, store.Id))
            .Count;
        foreach (var record in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
        {
            var authMethod = await _authenticationPluginManager
                .LoadPluginBySystemNameAsync(record.ProviderSystemName, currentCustomer, store.Id);
            if (!_authenticationPluginManager.IsPluginActive(authMethod))
                continue;

            model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
            {
                Id = record.Id,
                Email = record.Email,
                ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                    ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                AuthMethodName = await _localizationService.GetLocalizedFriendlyNameAsync(authMethod, currentLanguage.Id)
            });
        }

        //custom customer attributes
        var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
        foreach (var attribute in customAttributes)
            model.CustomerAttributes.Add(attribute);

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
            foreach (var consent in consents)
            {
                var accepted = await _gdprService.IsConsentAcceptedAsync(consent.Id, currentCustomer.Id);
                model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, accepted.HasValue && accepted.Value));
            }
        }

        return model;
    }

    /// <summary>
    /// Prepare the customer register model
    /// </summary>
    /// <param name="model">Customer register model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer register model
    /// </returns>
    public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
        string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)
    {
        ArgumentNullException.ThrowIfNull(model);

        var customer = await _workContext.GetCurrentCustomerAsync();

        model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
        foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
            model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

        //VAT
        model.DisplayVatNumber = _taxSettings.EuVatEnabled;
        if (_taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
            model.VatNumber = customer.VatNumber;

        //form fields
        model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
        model.LastNameEnabled = _customerSettings.LastNameEnabled;
        model.FirstNameRequired = _customerSettings.FirstNameRequired;
        model.LastNameRequired = _customerSettings.LastNameRequired;
        model.GenderEnabled = _customerSettings.GenderEnabled;
        model.NeutralGenderEnabled = _customerSettings.NeutralGenderEnabled;
        model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
        model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        model.CompanyEnabled = _customerSettings.CompanyEnabled;
        model.CompanyRequired = _customerSettings.CompanyRequired;
        model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
        model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
        model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
        model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
        model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
        model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
        model.CityEnabled = _customerSettings.CityEnabled;
        model.CityRequired = _customerSettings.CityRequired;
        model.CountyEnabled = _customerSettings.CountyEnabled;
        model.CountyRequired = _customerSettings.CountyRequired;
        model.CountryEnabled = _customerSettings.CountryEnabled;
        model.CountryRequired = _customerSettings.CountryRequired;
        model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
        model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
        model.PhoneEnabled = _customerSettings.PhoneEnabled;
        model.PhoneRequired = _customerSettings.PhoneRequired;
        model.FaxEnabled = _customerSettings.FaxEnabled;
        model.FaxRequired = _customerSettings.FaxRequired;
        model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
        model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
        model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
        model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
        model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
        model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
        model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
        if (setDefaultValues)
        {
            //enable newsletter by default
            model.Newsletter = _customerSettings.NewsletterTickedByDefault;
        }

        //countries and states
        if (_customerSettings.CountryEnabled)
        {
            model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
            model.CountryId = _customerSettings.DefaultCountryId ?? 0;
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
            {
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                if (states.Any())
                {
                    model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }

            }
        }

        //custom customer attributes
        var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
        foreach (var attribute in customAttributes)
            model.CustomerAttributes.Add(attribute);

        //GDPR
        if (_gdprSettings.GdprEnabled)
        {
            var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
            foreach (var consent in consents)
            {
                model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, false));
            }
        }

        return model;
    }

    /// <summary>
    /// Prepare the login model
    /// </summary>
    /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the login model
    /// </returns>
    public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
    {
        var model = new LoginModel
        {
            UsernamesEnabled = _customerSettings.UsernamesEnabled,
            RegistrationType = _customerSettings.UserRegistrationType,
            CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
            DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the password recovery model
    /// </summary>
    /// <param name="model">Password recovery model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the password recovery model
    /// </returns>
    public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the register result model
    /// </summary>
    /// <param name="resultId">Value of UserRegistrationType enum</param>
    /// <param name="returnUrl">URL to redirect</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the register result model
    /// </returns>
    public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
    {
        var resultText = (UserRegistrationType)resultId switch
        {
            UserRegistrationType.Disabled => await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"),
            UserRegistrationType.Standard => await _localizationService.GetResourceAsync("Account.Register.Result.Standard"),
            UserRegistrationType.AdminApproval => await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
            UserRegistrationType.EmailValidation => await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
            _ => null
        };

        var model = new RegisterResultModel
        {
            Result = resultText,
            ReturnUrl = returnUrl
        };

        return model;
    }

    /// <summary>
    /// Prepare the customer navigation model
    /// </summary>
    /// <param name="selectedTabId">Identifier of the selected tab</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer navigation model
    /// </returns>
    public virtual async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
    {
        var model = new CustomerNavigationModel();

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = "CustomerInfo",
            Title = await _localizationService.GetResourceAsync("Account.CustomerInfo"),
            Tab = (int)CustomerNavigationEnum.Info,
            ItemClass = "customer-info"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = "CustomerAddresses",
            Title = await _localizationService.GetResourceAsync("Account.CustomerAddresses"),
            Tab = (int)CustomerNavigationEnum.Addresses,
            ItemClass = "customer-addresses"
        });

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = "CustomerOrders",
            Title = await _localizationService.GetResourceAsync("Account.CustomerOrders"),
            Tab = (int)CustomerNavigationEnum.Orders,
            ItemClass = "customer-orders"
        });

        var store = await _storeContext.GetCurrentStoreAsync();
        var customer = await _workContext.GetCurrentCustomerAsync();

        if (_orderSettings.ReturnRequestsEnabled &&
            (await _returnRequestService.SearchReturnRequestsAsync(store.Id,
                customer.Id, pageIndex: 0, pageSize: 1)).Any())
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerReturnRequests",
                Title = await _localizationService.GetResourceAsync("Account.CustomerReturnRequests"),
                Tab = (int)CustomerNavigationEnum.ReturnRequests,
                ItemClass = "return-requests"
            });
        }

        if (!_customerSettings.HideDownloadableProductsTab)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerDownloadableProducts",
                Title = await _localizationService.GetResourceAsync("Account.DownloadableProducts"),
                Tab = (int)CustomerNavigationEnum.DownloadableProducts,
                ItemClass = "downloadable-products"
            });
        }

        if (!_customerSettings.HideBackInStockSubscriptionsTab)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerBackInStockSubscriptions",
                Title = await _localizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                Tab = (int)CustomerNavigationEnum.BackInStockSubscriptions,
                ItemClass = "back-in-stock-subscriptions"
            });
        }

        if (_rewardPointsSettings.Enabled)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerRewardPoints",
                Title = await _localizationService.GetResourceAsync("Account.RewardPoints"),
                Tab = (int)CustomerNavigationEnum.RewardPoints,
                ItemClass = "reward-points"
            });
        }

        model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
        {
            RouteName = "CustomerChangePassword",
            Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
            Tab = (int)CustomerNavigationEnum.ChangePassword,
            ItemClass = "change-password"
        });

        if (_customerSettings.AllowCustomersToUploadAvatars)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAvatar",
                Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                Tab = (int)CustomerNavigationEnum.Avatar,
                ItemClass = "customer-avatar"
            });
        }

        if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerForumSubscriptions",
                Title = await _localizationService.GetResourceAsync("Account.ForumSubscriptions"),
                Tab = (int)CustomerNavigationEnum.ForumSubscriptions,
                ItemClass = "forum-subscriptions"
            });
        }
        if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerProductReviews",
                Title = await _localizationService.GetResourceAsync("Account.CustomerProductReviews"),
                Tab = (int)CustomerNavigationEnum.ProductReviews,
                ItemClass = "customer-reviews"
            });
        }
        if (_vendorSettings.AllowVendorsToEditInfo && await _workContext.GetCurrentVendorAsync() != null)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerVendorInfo",
                Title = await _localizationService.GetResourceAsync("Account.VendorInfo"),
                Tab = (int)CustomerNavigationEnum.VendorInfo,
                ItemClass = "customer-vendor-info"
            });
        }
        if (_gdprSettings.GdprEnabled)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "GdprTools",
                Title = await _localizationService.GetResourceAsync("Account.Gdpr"),
                Tab = (int)CustomerNavigationEnum.GdprTools,
                ItemClass = "customer-gdpr"
            });
        }

        if (_captchaSettings.Enabled && _customerSettings.AllowCustomersToCheckGiftCardBalance)
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CheckGiftCardBalance",
                Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                Tab = (int)CustomerNavigationEnum.CheckGiftCardBalance,
                ItemClass = "customer-check-gift-card-balance"
            });
        }

        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication) &&
            await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
        {
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "MultiFactorAuthenticationSettings",
                Title = await _localizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                Tab = (int)CustomerNavigationEnum.MultiFactorAuthentication,
                ItemClass = "customer-multiFactor-authentication"
            });
        }

        model.SelectedTab = selectedTabId;

        return model;
    }

    /// <summary>
    /// Prepare the customer address list model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer address list model
    /// </returns>
    public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            //enabled for the current store
            .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
            .ToListAsync();

        var model = new CustomerAddressListModel();
        foreach (var address in addresses)
        {
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);
            model.Addresses.Add(addressModel);
        }
        return model;
    }

    /// <summary>
    /// Prepare the customer downloadable products model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer downloadable products model
    /// </returns>
    public virtual async Task<CustomerDownloadableProductsModel> PrepareCustomerDownloadableProductsModelAsync()
    {
        var model = new CustomerDownloadableProductsModel();
        var customer = await _workContext.GetCurrentCustomerAsync();
        var items = await _orderService.GetDownloadableOrderItemsAsync(customer.Id);
        foreach (var item in items)
        {
            var order = await _orderService.GetOrderByIdAsync(item.OrderId);
            var product = await _productService.GetProductByIdAsync(item.ProductId);

            var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
            {
                OrderItemGuid = item.OrderItemGuid,
                OrderId = order.Id,
                CustomOrderNumber = order.CustomOrderNumber,
                CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                ProductAttributes = item.AttributeDescription,
                ProductId = item.ProductId
            };
            model.Items.Add(itemModel);

            if (await _orderService.IsDownloadAllowedAsync(item))
                itemModel.DownloadId = product.DownloadId;

            if (await _orderService.IsLicenseDownloadAllowedAsync(item))
                itemModel.LicenseId = item.LicenseDownloadId ?? 0;
        }

        return model;
    }

    /// <summary>
    /// Prepare the user agreement model
    /// </summary>
    /// <param name="orderItem">Order item</param>
    /// <param name="product">Product</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the user agreement model
    /// </returns>
    public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product)
    {
        ArgumentNullException.ThrowIfNull(orderItem);

        ArgumentNullException.ThrowIfNull(product);

        var model = new UserAgreementModel
        {
            UserAgreementText = product.UserAgreementText,
            OrderItemGuid = orderItem.OrderItemGuid
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the change password model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the change password model
    /// </returns>
    public virtual Task<ChangePasswordModel> PrepareChangePasswordModelAsync()
    {
        var model = new ChangePasswordModel();

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the customer avatar model
    /// </summary>
    /// <param name="model">Customer avatar model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the customer avatar model
    /// </returns>
    public virtual async Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
            await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AvatarPictureIdAttribute),
            _mediaSettings.AvatarPictureSize,
            false);

        return model;
    }

    /// <summary>
    /// Prepare the GDPR tools model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the gDPR tools model
    /// </returns>
    public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
    {
        var model = new GdprToolsModel();

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the check gift card balance madel
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the check gift card balance madel
    /// </returns>
    public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
    {
        var model = new CheckGiftCardBalanceModel();

        return Task.FromResult(model);
    }

    /// <summary>
    /// Prepare the multi-factor authentication model
    /// </summary>
    /// <param name="model">Multi-factor authentication model</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        model.IsEnabled = !string.IsNullOrEmpty(
            await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

        var store = await _storeContext.GetCurrentStoreAsync();
        var multiFactorAuthenticationProviders = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id)).ToList();
        foreach (var multiFactorAuthenticationProvider in multiFactorAuthenticationProviders)
        {
            var providerModel = new MultiFactorAuthenticationProviderModel();
            var sysName = multiFactorAuthenticationProvider.PluginDescriptor.SystemName;
            providerModel = await PrepareMultiFactorAuthenticationProviderModelAsync(providerModel, sysName);
            model.Providers.Add(providerModel);
        }

        return model;
    }

    /// <summary>
    /// Prepare the multi-factor authentication provider model
    /// </summary>
    /// <param name="providerModel">Multi-factor authentication provider model</param>
    /// <param name="sysName">Multi-factor authentication provider system name</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the multi-factor authentication model
    /// </returns>
    public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
        var store = await _storeContext.GetCurrentStoreAsync();

        var multiFactorAuthenticationProvider = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id))
            .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

        if (multiFactorAuthenticationProvider != null)
        {
            providerModel.Name = await _localizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await _workContext.GetWorkingLanguageAsync()).Id);
            providerModel.SystemName = sysName;
            providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
            providerModel.LogoUrl = await _multiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
            providerModel.ViewComponent = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponent() : multiFactorAuthenticationProvider.GetPublicViewComponent();
            providerModel.Selected = sysName == selectedProvider;
        }

        return providerModel;
    }

    /// <summary>
    /// Prepare the custom customer attribute models
    /// </summary>
    /// <param name="customer">Customer</param>
    /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the list of the customer attribute model
    /// </returns>
    public virtual async Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "")
    {
        ArgumentNullException.ThrowIfNull(customer);

        var result = new List<CustomerAttributeModel>();

        var customerAttributes = await _customerAttributeService.GetAllAttributesAsync();
        foreach (var attribute in customerAttributes)
        {
            var attributeModel = new CustomerAttributeModel
            {
                Id = attribute.Id,
                Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                IsRequired = attribute.IsRequired,
                AttributeControlType = attribute.AttributeControlType,
            };

            if (attribute.ShouldHaveValues)
            {
                //values
                var attributeValues = await _customerAttributeService.GetAttributeValuesAsync(attribute.Id);
                foreach (var attributeValue in attributeValues)
                {
                    var valueModel = new CustomerAttributeValueModel
                    {
                        Id = attributeValue.Id,
                        Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                        IsPreSelected = attributeValue.IsPreSelected
                    };
                    attributeModel.Values.Add(valueModel);
                }
            }

            //set already selected attributes
            var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                overrideAttributesXml :
                customer.CustomCustomerAttributesXML;
            switch (attribute.AttributeControlType)
            {
                case AttributeControlType.DropdownList:
                case AttributeControlType.RadioList:
                case AttributeControlType.Checkboxes:
                {
                    if (!string.IsNullOrEmpty(selectedAttributesXml))
                    {
                        if (!_customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id).Any())
                            break;

                        //clear default selection                                
                        foreach (var item in attributeModel.Values)
                            item.IsPreSelected = false;

                        //select new values
                        var selectedValues = await _customerAttributeParser.ParseAttributeValuesAsync(selectedAttributesXml);
                        foreach (var attributeValue in selectedValues)
                        foreach (var item in attributeModel.Values)
                            if (attributeValue.Id == item.Id)
                                item.IsPreSelected = true;
                    }
                }
                    break;
                case AttributeControlType.ReadonlyCheckboxes:
                {
                    //do nothing
                    //values are already pre-set
                }
                    break;
                case AttributeControlType.TextBox:
                case AttributeControlType.MultilineTextbox:
                {
                    if (!string.IsNullOrEmpty(selectedAttributesXml))
                    {
                        var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                        if (enteredText.Any())
                            attributeModel.DefaultValue = enteredText[0];
                    }
                }
                    break;
                case AttributeControlType.ColorSquares:
                case AttributeControlType.ImageSquares:
                case AttributeControlType.Datepicker:
                case AttributeControlType.FileUpload:
                default:
                    //not supported attribute control types
                    break;
            }

            result.Add(attributeModel);
        }

        return result;
    }


    /// <summary>
    /// For the current customerid, get available address
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public async Task<CustomerAddressListModel> PrepareCustomerAddressModelByCustomerIdAsync(int customerId)
    {
        var customer = await _customerService.GetCustomerByIdAsync(customerId);
        int addressId = customer.BillingAddressId != null ? Convert.ToInt32(customer.BillingAddressId) : (customer.ShippingAddressId != null ? Convert.ToInt32(customer.ShippingAddressId) : 0);
        var model = new CustomerAddressListModel();

        if (addressId != 0)
        {
            var address = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
            //enabled for the current store
            .WhereAwait(async a => (a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a))) && a.Id == addressId)
            .FirstOrDefaultAsync();

            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);
            model.Addresses.Add(addressModel);
        
        }

        return model;
    }


    /// <summary>
    /// for the input addressIds, get address information
    /// </summary>
    /// <param name="addressIds"></param>
    /// <returns></returns>
    public async Task<CustomerAddressListModel> PrepareAddressModelByAddressIdsAsync(List<int> addressIds)
    {
        var model = new CustomerAddressListModel();
        foreach (int addressId in addressIds)
        {
            var address = await _addressService.GetAddressByIdAsync(addressId);
            var addressModel = new AddressModel();
            await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings);
            model.Addresses.Add(addressModel);
        }

        return model;
    }


    /// <summary>
    /// for preparing a model of salesforce request for creating customer at salesforce
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="addressId"></param>
    /// <returns></returns>
    public async Task<SalesforceContactUpsertResponse> PrepareSalesforceResponseModelForCustomer(int customerId, int? addressId = null)
    {
        try
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            SalesforceOrderResponse salesforceOrderResponse = new SalesforceOrderResponse();
            if (customer != null)
            {

                int addressIdForSalesforce = addressId != null ? Convert.ToInt32(addressId) : (customer.BillingAddressId != null ? Convert.ToInt32(customer.BillingAddressId) : (customer.ShippingAddressId != null ? Convert.ToInt32(customer.ShippingAddressId) : 0));
                var addresses = addressIdForSalesforce == 0 ? await PrepareCustomerAddressModelByCustomerIdAsync(customerId) : await PrepareAddressModelByAddressIdsAsync(new List<int>() { addressIdForSalesforce });

                var address = (addresses != null && addresses.Addresses != null && addresses.Addresses.Count() > 0) ? addresses.Addresses[0] : null;

                // there is a possibility that the customer has just registered with email and does not have a first, last name.
                // in this case, fetch the first, last name from the address and use it to updated the customer. Later this updated detail will be send to salesforce
                if (customer.FirstName == null || customer.LastName == null)
                {
                    customer.FirstName = address.FirstName;
                    customer.LastName = address.LastName;

                    await _customerService.UpdateCustomerAsync(customer);
                }

                //get sfdc contact number, which might have been saved the last time in the db
                string sfdcContactNumber = customer.CustomCustomerAttributesXML != null ? await GetSFDCNumber(customer.CustomCustomerAttributesXML) : null;

                SalesforceContactUpsertRequest salesforceContactUpsertRequest = new SalesforceContactUpsertRequest() { Contacts = new List<SalesforceContacts>() };

                SalesforceContacts salesforceContacts = new SalesforceContacts()
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    EmailId = customer.Email,
                    Mobile = customer.Phone,
                    State = address != null ? address.StateProvinceName : null,
                    City = address != null ? address.City : null,
                    Address = address != null ? address.Address1 : null,
                    Pincode = address != null ? address.ZipPostalCode : null,
                    Country = address != null ? address.CountryName : null,
                    Gender = customer.Gender,
                    OAuthProvider = "Google",
                    SFDCContactNumber = sfdcContactNumber
                };
                salesforceContactUpsertRequest.Contacts.Add(salesforceContacts);


                #region MakeAPICall


                var contactJsonString = JsonSerializer.Serialize(salesforceContactUpsertRequest);
                string endpoint = "apexrest/Web2SFDCcontactUpsert";
                var responseContent = await _salesforceCommonService.SalesforceAPICallHandler(endpoint, contactJsonString);

                if (responseContent != null)
                {
                    var jsonToken = JToken.Parse(responseContent);
                    JArray jsonArray = (JArray)jsonToken;
                    foreach (JObject json in jsonArray)
                    {
                        string sfdcNumber = json["SFDCNumber"]?.ToString();
                        salesforceOrderResponse.SFDCNumber = sfdcNumber;
                        salesforceOrderResponse.SFDCRecordId = json["SFDCRecordId"]?.ToString();
                        salesforceOrderResponse.ResultMsg = json["ResultMsg"]?.ToString();
                        salesforceOrderResponse.CalloutErrorResult = Convert.ToBoolean(json["CalloutErrorResult"]);

                        if (sfdcNumber != null)
                        {
                            IDictionary<string, string> form = new Dictionary<string, string>();
                            form.Add("customer_attribute_1", sfdcNumber);
                            var updatedCustomCustomerAttributeXML = await ParseCustomCustomerAttributesAsync(form);
                            customer.CustomCustomerAttributesXML = updatedCustomCustomerAttributeXML;
                            await _customerService.UpdateCustomerAsync(customer);
                        }
                    }
                }
                #endregion



            }


            SalesforceContactUpsertResponse salesforceContactUpsertResponse = new SalesforceContactUpsertResponse() { SalesforceResponse = new List<SalesforceOrderResponse>() };
            salesforceContactUpsertResponse.SalesforceResponse.Add(salesforceOrderResponse);
            return salesforceContactUpsertResponse;

        }
        catch (Exception ex)
        {
            throw ex;
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



    public async Task<SalesforceOrderResponse> PrepareSalesforceResponseModelForOrders(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        SalesforceOrderResponse salesforceOrderResponse = new SalesforceOrderResponse();
        SalesforceOrderRequest salesforceOrderRequest = new SalesforceOrderRequest();
        if (order != null)
        {
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            if (!String.IsNullOrEmpty(customer.CustomCustomerAttributesXML))
            {
                int[] productIds;
                var orderDetails = await _orderService.GetOrderItemsAsync(order.Id);

                productIds = orderDetails.Select(x => x.ProductId).ToArray();

                var products = await _productService.GetProductsByIdsAsync(productIds);
                // get model contains all details related to each product in the current order
                var productOverviewModels = (await _productModelFactory.PrepareProductOverviewModelsAsync(products, true, true, null, true, false)).ToList();


                int billingAddressId = order.BillingAddressId;
                int shippingAddressId = (int)(order.ShippingAddressId != null ? order.ShippingAddressId : billingAddressId);

                // get address mode information for billing and shipping address
                var addresses = await PrepareAddressModelByAddressIdsAsync(new List<int> { billingAddressId, shippingAddressId });

                SalesforceOrderComponents salesforceOrderComponents = new SalesforceOrderComponents();

                if (addresses.Addresses.Count() == 2)
                {
                    var billingAddress = addresses.Addresses[0];
                    salesforceOrderComponents.BillingAddress = await GetSalesforceAddressMapper(billingAddress);

                    var shippingAddress = addresses.Addresses[1];
                    salesforceOrderComponents.ShippingAddress = await GetSalesforceAddressMapper(shippingAddress);
                }

                salesforceOrderComponents.Ord = await GetSalesforceOrderMapper(order, customer.CustomCustomerAttributesXML);

                PopulateOrderDetails(orderDetails, productOverviewModels, salesforceOrderComponents, order);
                salesforceOrderRequest.OrderWrapper = salesforceOrderComponents;

                #region MakeApiCall

                var contactJsonString = JsonSerializer.Serialize(salesforceOrderRequest);
                string endpoint = "apexrest/Web2SFDCorder";
                var responseContent = await _salesforceCommonService.SalesforceAPICallHandler(endpoint, contactJsonString);

                if (responseContent != null)
                {

                    JObject json = JObject.Parse(responseContent);
                    string SFDCNumber = json["SFDCNumber"]?.ToString();
                    salesforceOrderResponse.SFDCNumber = SFDCNumber;
                    salesforceOrderResponse.SFDCRecordId = json["SFDCRecordId"]?.ToString();
                    salesforceOrderResponse.ResultMsg = json["ResultMsg"]?.ToString();
                    salesforceOrderResponse.CalloutErrorResult = Convert.ToBoolean(json["CalloutErrorResult"]);

                    if (!String.IsNullOrEmpty(SFDCNumber))
                    {
                        order.AuthorizationTransactionCode = SFDCNumber;
                        await _orderService.UpdateOrderAsync(order);
                    }

                }

                #endregion
            }

        }
        return salesforceOrderResponse;
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

    #endregion
}