using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.WebApi.Framework.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public partial class WebApiConfigController : BasePluginController
{
    #region Fields

    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILocalizationService _localizationService;
    private readonly INotificationService _notificationService;
    private readonly IPermissionService _permissionService;
    private readonly ISettingService _settingService;

    #endregion

    #region Ctor 

    public WebApiConfigController(IJwtTokenService jwtTokenService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService)
    {
        _jwtTokenService = jwtTokenService;
        _localizationService = localizationService;
        _notificationService = notificationService;
        _permissionService = permissionService;
        _settingService = settingService;
    }

    #endregion

    #region Methods

    public virtual async Task<IActionResult> Configure()
    {
        if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePlugins))
            return AccessDeniedView();

        //load settings for active store scope
        var webApiSettings = await _settingService.LoadSettingAsync<WebApiCommonSettings>();

        var pluginFolder = "";
        var controllerNme = GetType().Name.Replace("Controller", string.Empty);

        switch (controllerNme)
        {
            case "WebApiFrontend":
                pluginFolder = "Misc.WebApi.Frontend";
                break;
            case "WebApiBackend":
                pluginFolder = "Misc.WebApi.Backend";
                break;
        }

        //prepare model
        var model = new ConfigurationModel
        {
            DeveloperMode = webApiSettings.DeveloperMode,
            SecretKey = webApiSettings.SecretKey,
            ControllerName = controllerNme
        };

        return View($"~/Plugins/{pluginFolder}/Views/Configure.cshtml", model);
    }

    [HttpPost, ActionName("Configure")]
    [FormValueRequired("save")]
    public virtual async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        //load settings for active store scope
        var webApiSettings = await _settingService.LoadSettingAsync<WebApiCommonSettings>();

        //set settings
        webApiSettings.DeveloperMode = model.DeveloperMode;
        webApiSettings.SecretKey = model.SecretKey;

        await _settingService.SaveSettingAsync(webApiSettings, settings => settings.DeveloperMode, clearCache: false);
        await _settingService.SaveSettingAsync(webApiSettings, settings => settings.SecretKey, clearCache: false);
        await _settingService.ClearCacheAsync();

        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

        return await Configure();
    }

    [HttpPost]
    public virtual IActionResult Generate()
    {
        return Ok(_jwtTokenService.NewSecretKey);
    }

    #endregion
}