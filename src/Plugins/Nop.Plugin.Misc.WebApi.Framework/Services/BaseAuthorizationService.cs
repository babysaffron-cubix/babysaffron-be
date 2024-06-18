using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.WebApi.Framework.Services;

public abstract partial class BaseAuthorizationService
{
    #region Fields

    protected readonly CustomerSettings _customerSettings;
    protected readonly ICustomerRegistrationService _customerRegistrationService;
    protected readonly ICustomerService _customerService;
    protected readonly IJwtTokenService _jwtTokenService;

    #endregion

    #region Ctor

    protected BaseAuthorizationService(CustomerSettings customerSettings,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IJwtTokenService jwtTokenService)
    {
        _customerSettings = customerSettings;
        _customerRegistrationService = customerRegistrationService;
        _customerService = customerService;
        _jwtTokenService = jwtTokenService;
    }

    #endregion

    #region Utilities

    protected virtual AuthenticateResponse GetAuthenticateResponse(Customer customer)
    {
        return new AuthenticateResponse(_jwtTokenService.GetNewJwtToken(customer))
        {
            CustomerId = customer.Id,
            CustomerGuid = customer.CustomerGuid,
            Username = _customerSettings.UsernamesEnabled ? customer.Username : customer.Email
        };
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate JWT token for customer
    /// </summary>
    /// <param name="request">Authenticate request</param>
    /// <returns>JWT token as authenticate response</returns>
    public virtual async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest request)
    {
        var username = _customerSettings.UsernamesEnabled ? request.Username : request.Email;

        var loginResult = await _customerRegistrationService.ValidateCustomerAsync(username, request.Password);

        if (loginResult != CustomerLoginResults.Successful)
            return null;

        var customer = await (_customerSettings.UsernamesEnabled
            ? _customerService.GetCustomerByUsernameAsync(username)
            : _customerService.GetCustomerByEmailAsync(username));

        _ = await _customerRegistrationService.SignInCustomerAsync(customer, null);

        return GetAuthenticateResponse(customer);
    }


    /// <summary>
    /// Get JWT token for customer
    /// </summary>
    /// <param name="request">Authentication request using phone number</param>
    /// <returns>JWT token as authenticate response</returns>
    public virtual async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequestEmail request)
    {
        var loginResult = await _customerRegistrationService.ValidateCustomerAsync(request.Email);

        if (loginResult != CustomerLoginResults.Successful)
            return null;

        var customer = await (_customerService.GetCustomerByEmail(request.Email));

        _ = await _customerRegistrationService.SignInCustomerAsync(customer, null);

        return GetAuthenticateResponse(customer);
    }


    

    #endregion
}