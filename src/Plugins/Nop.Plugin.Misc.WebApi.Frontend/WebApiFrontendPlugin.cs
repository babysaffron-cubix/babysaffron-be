using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Plugin.Misc.WebApi.Framework;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.WebApi.Frontend;

/// <summary>
/// Represents the Web API frontend plugin
/// </summary>
public partial class WebApiFrontendPlugin : BasePlugin, IMiscPlugin
{
    #region Fields

    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILocalizationService _localizationService;
    private readonly ISettingService _settingService;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly WebApiHttpClient _webApiHttpClient;

    #endregion

    #region Ctor

    public WebApiFrontendPlugin(IActionContextAccessor actionContextAccessor,
        IJwtTokenService jwtTokenService,
        ILocalizationService localizationService,
        ISettingService settingService,
        IUrlHelperFactory urlHelperFactory,
        WebApiHttpClient webApiHttpClient)
    {
        _actionContextAccessor = actionContextAccessor;
        _jwtTokenService = jwtTokenService;
        _localizationService = localizationService;
        _settingService = settingService;
        _urlHelperFactory = urlHelperFactory;
        _webApiHttpClient = webApiHttpClient;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a configuration page URL
    /// </summary>
    public override string GetConfigurationPageUrl()
    {
        ArgumentNullException.ThrowIfNull(_actionContextAccessor.ActionContext);

        return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(WebApiFrontendDefaults.ConfigurationRouteName);
    }

    /// <summary>
    /// Install the plugin
    /// </summary>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task InstallAsync()
    {
        //locales
        await _localizationService.AddOrUpdateLocaleResourceAsync(WebApiCommonDefaults.Locales);

        //settings
        await _settingService.SaveSettingAsync(new WebApiCommonSettings
        {
            TokenLifetimeDays = WebApiCommonDefaults.TokenLifeTime,
            SecretKey = _jwtTokenService.NewSecretKey
        });

        //plugin installation confirmation
        try
        {
            var _ = await _webApiHttpClient.InstallationCompletedAsync(PluginDescriptor);
        }
        catch
        {
            // ignored
        }

        await base.InstallAsync();
    }

    /// <summary>
    /// Update plugin
    /// </summary>
    /// <param name="currentVersion">Current version of plugin</param>
    /// <param name="targetVersion">New version of plugin</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public override async Task UpdateAsync(string currentVersion, string targetVersion)
    {
        decimal version;
        try
        {
            version = Convert.ToDecimal(currentVersion.Replace(".", ""), CultureInfo.InvariantCulture);
        }
        catch
        {
            return;
        }

        if (version < 46005M)
        {
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.WebApi.Frontend");
            await _localizationService.AddOrUpdateLocaleResourceAsync(WebApiCommonDefaults.Locales);
        }
    }

    #endregion
}