using Microsoft.IdentityModel.Tokens;

namespace Nop.Plugin.Misc.WebApi.Framework;

/// <summary>
/// Represents plugin constants
/// </summary>
public class WebApiCommonDefaults
{
    /// <summary>
    /// Gets customer key of http context
    /// </summary>
    public static string CustomerKey => "nopApiUser";

    /// <summary>
    /// Gets Claim type
    /// </summary>
    public static string ClaimTypeName => "CustomerId";

    /// <summary>
    /// Gets SystemName of main API plugin
    /// </summary>
    public static string MainSystemName => "Misc.WebApi.Frontend";

    /// <summary>
    /// Gets the RoutePrefix of the Swagger UI
    /// </summary>
    public static string SwaggerUIRoutePrefix => "api";

    /// <summary>
    /// Gets the name of the header to be used for security
    /// </summary>
    public static string SecurityHeaderName => "Authorization";

    /// <summary>
    /// Token lifetime in days
    /// </summary>
    public static int TokenLifeTime => 365;

    /// <summary>
    /// The JWT token signature algorithm
    /// </summary>
    public static string JwtSignatureAlgorithm => SecurityAlgorithms.HmacSha256;

    /// <summary>
    /// The minimal length of secret key applied to signature algorithm
    /// <remarks>
    /// For HmacSha256 it may be at least 32 (256 bites)
    /// </remarks>
    /// </summary>
    public static int MinSecretKeyLength => 32;

    /// <summary>
    /// Gets swagger document version
    /// </summary>
    public const string API_VERSION = "v4.70.1";

    /// <summary>
    /// Gets the locales
    /// </summary>
    public static IDictionary<string, string> Locales =>
        new Dictionary<string, string>
        {
            ["Plugins.WebApi.DeveloperMode"] = "Developer mode",
            ["Plugins.WebApi.DeveloperMode.Hint"] = "Developer mode allows you to make requests without using JWT.",
            ["Plugins.WebApi.DeveloperMode.Warning"] = "Developer mode is enabled for the Web API plugins. Do not use it on the production server, because this mode allows to make requests without using JWT.",
            ["Plugins.WebApi.SecretKey"] = "Secret key",
            ["Plugins.WebApi.SecretKey.Generate"] = "Generate new",
            ["Plugins.WebApi.SecretKey.Hint"] = "The secret key to sign and verify each JWT token",
            ["Plugins.WebApi.SslNotEnabled"] = "The Web API accepts token sent in cleartext in a header over an unencrypted channel. Attackers can easily intercept API calls and retrieve the token. They can then use it to make other API calls. Please, enable the SSL on your store for protect token"
        };
}