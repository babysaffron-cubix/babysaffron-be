using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.WebApi.Backend.Services;

public partial class AuthorizationAdminService : BaseAuthorizationService, IAuthorizationAdminService
{
    #region Ctor

    public AuthorizationAdminService(CustomerSettings customerSettings,
        ICustomerRegistrationService customerRegistrationService,
        ICustomerService customerService,
        IJwtTokenService jwtTokenService) : base(customerSettings, customerRegistrationService, customerService, jwtTokenService)
    {
    }

    #endregion
}