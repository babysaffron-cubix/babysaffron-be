using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Framework.Models;

public abstract class AuthenticateRequest : BaseDto
{
    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; }
}