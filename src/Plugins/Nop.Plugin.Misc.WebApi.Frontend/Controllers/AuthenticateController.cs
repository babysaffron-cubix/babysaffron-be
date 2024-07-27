using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.WebApi.Framework;
using Nop.Plugin.Misc.WebApi.Framework.Controllers;
using Nop.Plugin.Misc.WebApi.Framework.Helpers;
using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Plugin.Misc.WebApi.Frontend.Models;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Plugin.Misc.WebApi.Frontend.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Nop.Services.Customers;
using Nop.Services;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

[Route("api-frontend/[controller]/[action]")]
[ApiExplorerSettings(GroupName = "frontend_" + WebApiCommonDefaults.API_VERSION)]
public partial class AuthenticateController : BaseNopWebApiController
{
    #region Fields

    private readonly IAuthorizationUserService _authorizationUserService;
    private readonly WebApiCommonSettings _webApiCommonSettings;
    private readonly ICustomerService _customerService;
    private readonly ISalesforceService _salesforceService;

    #endregion

    #region Ctor

    public AuthenticateController(
        IAuthorizationUserService authorizationUserService,
        WebApiCommonSettings webApiCommonSettings,
        ICustomerService customerService,
        ISalesforceService salesforceService)
    {
        _authorizationUserService = authorizationUserService;
        _webApiCommonSettings = webApiCommonSettings;
        _customerService = customerService;
        _salesforceService = salesforceService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Authenticate user
    /// </summary>
    /// <param name="request"></param>
    [Authorize(true)]
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> GetToken([FromBody] AuthenticateCustomerRequest request)
    {
        var response = await _authorizationUserService.AuthenticateAsync(request);

        if (response == null)
            return Unauthorized("Username or password is incorrect");

        return Ok(response);
    }


    /// <summary>
    /// Authenticate user by Email
    /// </summary>
    /// <param name="request"></param>
    [Authorize(true)]
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> GetTokenByEmail([FromBody] AuthenticateCustomerRequestEmail request)
    {
        var response = await _authorizationUserService.AuthenticateAsyncByEmail(request);

        if (response == null)
            return Unauthorized("Email is incorrect");

        return Ok(response);
    }



    /// <summary>
    /// Gets API version
    /// </summary>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult GetApiVersion()
    {
        return Ok(WebApiCommonDefaults.API_VERSION);
    }


    [Authorize(true)]
    [HttpPost]
    [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {

            var principal = GetPrincipalFromExpiredToken(request.Token);
            if (principal.Identity is ClaimsIdentity identity && identity.Claims.Count() >= 5)
            {
                var emailId = identity.Claims.ElementAt(4).Value;
                if (emailId != null)
                {

                    var user = await _customerService.GetCustomerByEmailAsync(emailId);
                    if (user != null)
                    {
                        AuthenticateCustomerRequestEmail authenticateCustomerRequestEmail = new AuthenticateCustomerRequestEmail() { Email = emailId };
                        var response = await _authorizationUserService.AuthenticateAsyncByEmail(authenticateCustomerRequestEmail);
                        return Ok(response.Token);
                    }
                    else
                    {
                        return NotFound("User not found.");
                    }
                }
                else
                {
                    return NotFound(emailId);
                }

            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest("Invalid token.");
        }



    }





    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
{
    var key = Encoding.UTF8.GetBytes(_webApiCommonSettings.SecretKey);
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = false // Here we are saying that we don't care about the token's expiration date
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    SecurityToken securityToken;
    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
    var jwtSecurityToken = securityToken as JwtSecurityToken;

    if (jwtSecurityToken == null ||
        !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
    {
        throw new SecurityTokenException("Invalid token");
    }

    return principal;
}




    #endregion
}