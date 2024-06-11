using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Country;
using Nop.Web.Factories;

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;

public partial class CountryController : BaseNopWebApiFrontendController
{
    #region Fields

    private readonly ICountryModelFactory _countryModelFactory;

    #endregion

    #region Ctor

    public CountryController(ICountryModelFactory countryModelFactory)
    {
        _countryModelFactory = countryModelFactory;
    }

    #endregion

    #region States / provinces

    [HttpGet("{countryId}")]
    [ProducesResponseType(typeof(IList<StateProvinceModelDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetStatesByCountryId(int countryId, [FromQuery][Required] bool addSelectStateItem)
    {
        var model = await _countryModelFactory.GetStatesByCountryIdAsync(countryId, addSelectStateItem);
        var modelDto = model.Select(p => p.ToDto<StateProvinceModelDto>()).ToList();

        return Ok(modelDto);
    }

    #endregion
}