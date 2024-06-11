using Microsoft.AspNetCore.Mvc;
using Nop.Core.Caching;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Orders;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Orders;

public partial class CheckoutAttributeController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;
    private readonly IStaticCacheManager _staticCacheManager;
    private readonly IStoreMappingService _storeMappingService;

    #endregion

    #region Ctor

    public CheckoutAttributeController(IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService,
        IStaticCacheManager staticCacheManager,
        IStoreMappingService storeMappingService)
    {
        _checkoutAttributeService = checkoutAttributeService;
        _staticCacheManager = staticCacheManager;
        _storeMappingService = storeMappingService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes checkout attributes
    /// </summary>
    /// <param name="ids">Array of checkout attributes identifiers (separator - ;)</param>
    [HttpDelete("{ids}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> DeleteByIds(string ids)
    {
        if (string.IsNullOrEmpty(ids))
            return BadRequest();

        var checkoutAttributeIds = ids.Split(";").Where(s => int.TryParse(s, out _)).Select(str => int.Parse(str)).ToArray();
        var checkoutAttributes = await _checkoutAttributeService.GetAttributeByIdsAsync(checkoutAttributeIds);

        await _checkoutAttributeService.DeleteAttributesAsync(checkoutAttributes);

        return Ok();
    }

    /// <summary>
    /// Gets all checkout attributes
    /// </summary>
    /// <param name="storeId">Store identifier</param>
    /// <param name="excludeShippableAttributes">A value indicating whether we should exclude shippable attributes</param>
    [HttpGet]
    [ProducesResponseType(typeof(IList<CheckoutAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll([FromQuery] int storeId = 0, [FromQuery] bool excludeShippableAttributes = false)
    {
        var checkoutAttributes = await _checkoutAttributeService.GetAllAttributesAsync(_staticCacheManager, _storeMappingService, storeId, excludeShippableAttributes);

        var checkoutAttributeDtos = checkoutAttributes.Select(attr => attr.ToDto<CheckoutAttributeDto>()).ToList();

        return Ok(checkoutAttributeDtos);
    }

    /// <summary>
    /// Gets a checkout attribute
    /// </summary>
    /// <param name="id">Checkout attribute identifier</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutAttributeDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest();

        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(id);

        if (checkoutAttribute == null)
        {
            return NotFound($"Checkout attribute Id={id} is not found");
        }

        return Ok(checkoutAttribute.ToDto<CheckoutAttributeDto>());
    }

    /// <summary>
    /// Gets checkout attributes
    /// </summary>
    /// <param name="ids">Array of checkout attribute identifiers (separator - ;)</param>
    [HttpGet("{ids}")]
    [ProducesResponseType(typeof(IList<CheckoutAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetByIds(string ids)
    {
        var checkoutAttributeIds = ids.Split(";").Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
        var checkoutAttributes = await _checkoutAttributeService.GetAttributeByIdsAsync(checkoutAttributeIds);

        var checkoutAttributeDtos = checkoutAttributes.Select(attr => attr.ToDto<CheckoutAttributeDto>()).ToList();

        return Ok(checkoutAttributeDtos);
    }

    /// <summary>
    /// Create a checkout attribute
    /// </summary>
    /// <param name="model">Checkout attribute Dto model</param>
    [HttpPost]
    [ProducesResponseType(typeof(CheckoutAttributeDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] CheckoutAttributeDto model)
    {
        var checkoutAttribute = model.FromDto<CheckoutAttribute>();

        await _checkoutAttributeService.InsertAttributeAsync(checkoutAttribute);

        return Ok(checkoutAttribute.ToDto<CheckoutAttributeDto>());
    }

    /// <summary>
    /// Update a checkout attribute
    /// </summary>
    /// <param name="model">Checkout attribute Dto model</param>
    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] CheckoutAttributeDto model)
    {
        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(model.Id);

        if (checkoutAttribute == null)
            return NotFound("Checkout attribute is not found");

        checkoutAttribute = model.FromDto<CheckoutAttribute>();
        await _checkoutAttributeService.UpdateAttributeAsync(checkoutAttribute);

        return Ok();
    }

    #endregion
}