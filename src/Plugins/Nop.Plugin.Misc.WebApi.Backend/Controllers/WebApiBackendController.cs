using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.WebApi.Framework.Controllers;
using Nop.Plugin.Misc.WebApi.Framework.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.ADMIN)]
public partial class WebApiBackendController : WebApiConfigController
{
    #region Ctor

    public WebApiBackendController(IJwtTokenService jwtTokenService,
        ILocalizationService localizationService,
        INotificationService notificationService,
        IPermissionService permissionService,
        ISettingService settingService) : base(jwtTokenService, localizationService, notificationService,
        permissionService, settingService)
    {
    }

    #endregion
}