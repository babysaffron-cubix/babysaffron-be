using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Plugin.Misc.WebApi.Frontend.Models;

namespace Nop.Plugin.Misc.WebApi.Frontend.Services;

public interface IAuthorizationUserService
{
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateCustomerRequest request);

    Task<AuthenticateResponse> AuthenticateAsyncByEmail(AuthenticateRequestEmail request);

}