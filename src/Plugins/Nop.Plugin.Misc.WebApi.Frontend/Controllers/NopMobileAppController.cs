using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Plugin.Misc.WebApi.Framework;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Media;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Product;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

public class NopMobileAppController : BaseNopWebApiFrontendController
{
    #region Fields

    private readonly IPermissionService _permissionService;
    private readonly IPictureService _pictureService;
    private readonly ISettingService _settingService;
    private readonly WebApiMobileSettings _mobileSettings;

    #endregion

    #region Ctor

    public NopMobileAppController(IPermissionService permissionService,
        IPictureService pictureService,
        ISettingService settingService,
        WebApiMobileSettings mobileSettings)
    {
        _permissionService = permissionService;
        _pictureService = pictureService;
        _settingService = settingService;
        _mobileSettings = mobileSettings;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get all settings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IDictionary<string, string>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Settings()
    {
        var settings = await _settingService.GetAllSettingsAsync();

        //Settings (required)
        _mobileSettings.AllowedSettings.Add($"{nameof(RewardPointsSettings)}.{nameof(RewardPointsSettings.Enabled)}");
        _mobileSettings.AllowedSettings.Add($"{nameof(CustomerSettings)}.{nameof(CustomerSettings.HideBackInStockSubscriptionsTab)}");
        _mobileSettings.AllowedSettings.Add($"{nameof(CustomerSettings)}.{nameof(CustomerSettings.HideDownloadableProductsTab)}");

        var dictionary = settings
            .Where(setting =>
                _mobileSettings.AllowedSettings.Contains(setting.Name, StringComparer.InvariantCultureIgnoreCase))
            .ToDictionary(setting => setting.Name, setting => setting.Value);

        //Permissions
        var hasWishlistAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist);
        var hasShoppingCartAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);

        dictionary.Add(StandardPermissionProvider.EnableWishlist.SystemName.ToLowerInvariant(), hasWishlistAccess.ToString());
        dictionary.Add(StandardPermissionProvider.EnableShoppingCart.SystemName.ToLowerInvariant(), hasShoppingCartAccess.ToString());

        return Ok(dictionary);
    }

    [HttpGet]
    [ProducesResponseType(typeof(SliderDataDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> SliderData()
    {
        var rez = new SliderDataDto();

        var productImages = _mobileSettings.ProductsSliderImages.SelectAwait(async pi =>
        {
            var picture = await _pictureService.GetPictureByIdAsync(pi.Key);

            if (picture == null)
                return null;

            var (fullSizeImageUrl, _) = await _pictureService.GetPictureUrlAsync(picture);
            var (imageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, _mobileSettings.SliderPictureSize);
            var (thumbImageUrl, _) = await _pictureService.GetPictureUrlAsync(picture, _mobileSettings.SliderPictureThumbSize);

            return new SliderDataDto.SliderProductDto
            {
                ProductId = pi.Value,
                Picture = new PictureModelDto
                {
                    FullSizeImageUrl = fullSizeImageUrl,
                    ImageUrl = imageUrl,
                    ThumbImageUrl = thumbImageUrl,
                }
            };
        });

        rez.Products.AddRange(await productImages.Where(p => p != null).ToArrayAsync());

        return Ok(rez);
    }

    #endregion
}