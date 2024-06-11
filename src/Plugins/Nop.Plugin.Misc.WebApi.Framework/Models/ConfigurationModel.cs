using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.WebApi.Framework.Models;

/// <summary>
/// Represents plugin configuration model
/// </summary>
public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.WebApi.DeveloperMode")]
    public bool DeveloperMode { get; set; }

    [NopResourceDisplayName("Plugins.WebApi.SecretKey")]
    public string SecretKey { get; set; }

    public string ControllerName { get; set; }
}