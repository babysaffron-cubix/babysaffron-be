using Nop.Core;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Models.Common;

namespace Nop.Plugin.Misc.WebApi.Framework.Services;

public partial class EventConsumer : IConsumer<SystemWarningCreatedEvent>
{
    #region Fields

    private readonly ILocalizationService _localizationService;
    private readonly IStoreContext _storeContext;
    private readonly WebApiCommonSettings _settings;

    #endregion

    #region Ctor

    public EventConsumer(ILocalizationService localizationService,
        IStoreContext storeContext,
        WebApiCommonSettings settings)
    {
        _localizationService = localizationService;
        _storeContext = storeContext;
        _settings = settings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(SystemWarningCreatedEvent eventMessage)
    {
        if (!(await _storeContext.GetCurrentStoreAsync()).SslEnabled)
            eventMessage.SystemWarnings.Add(new SystemWarningModel
            {
                Level = SystemWarningLevel.Warning,
                Text = await _localizationService.GetResourceAsync("Plugins.WebApi.SslNotEnabled")
            });

        if (!_settings.DeveloperMode)
            return;

        eventMessage.SystemWarnings.Add(new SystemWarningModel
        {
            Level = SystemWarningLevel.Warning,
            Text = await _localizationService.GetResourceAsync("Plugins.WebApi.DeveloperMode.Warning")
        });
    }

    #endregion
}