using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Plugin.Misc.WebApi.Frontend.Models;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.WebApi.Frontend.Services;

public partial class AuthorizationUserService : BaseAuthorizationService, IAuthorizationUserService
{
    #region Fields

    private readonly IWorkContext _workContext;

    #endregion

    #region Ctor

    public AuthorizationUserService(
        CustomerSettings customerSettings,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IJwtTokenService jwtTokenService,
        IWorkContext workContext
    ) : base(customerSettings, customerRegistrationService, customerService, jwtTokenService)
    {
        _workContext = workContext;
    }

    #endregion

    #region Methods

    public virtual async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateCustomerRequest request)
    {
        if (!request.IsGuest)
            return await base.AuthenticateAsync(request);

        var customer = await _customerService.InsertGuestCustomerAsync();
        await _workContext.SetCurrentCustomerAsync(customer);

        return GetAuthenticateResponse(customer);
    }

    #endregion
}