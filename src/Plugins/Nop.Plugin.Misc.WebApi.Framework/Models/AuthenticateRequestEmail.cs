using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Framework.Models;

public abstract class AuthenticateRequestEmail : BaseDto
{
    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; }

}