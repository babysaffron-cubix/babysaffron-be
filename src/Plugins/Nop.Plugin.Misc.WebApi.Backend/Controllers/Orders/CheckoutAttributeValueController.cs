using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Orders;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Orders;

public partial class CheckoutAttributeValueController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;

    #endregion

    #region Ctor

    public CheckoutAttributeValueController(IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService)
    {
        _checkoutAttributeService = checkoutAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Deletes a checkout attribute value
    /// </summary>
    /// <param name="id">Checkout attribute value identifier</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest();

        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(id);

        if (checkoutAttributeValue == null)
            return NotFound($"Checkout attribute value Id={id} not found");

        await _checkoutAttributeService.DeleteAttributeValueAsync(checkoutAttributeValue);

        return Ok();
    }

    /// <summary>
    /// Gets checkout attribute values by checkout attribute identifier
    /// </summary>
    /// <param name="id">Checkout attribute identifier</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(IList<CheckoutAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetByCheckoutAttributeId(int id)
    {
        if (id <= 0)
            return BadRequest();

        var checkoutAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(id);

        if (checkoutAttribute == null)
        {
            return NotFound($"Checkout attribute Id={id} is not found");
        }

        var checkoutAttributeValues = await _checkoutAttributeService.GetAttributeValuesAsync(id);
        var checkoutAttributeValueDtos = checkoutAttributeValues.Select(attr => attr.ToDto<CheckoutAttributeValueDto>()).ToList();

        return Ok(checkoutAttributeValueDtos);
    }

    /// <summary>
    /// Gets a checkout attribute value
    /// </summary>
    /// <param name="id">Checkout attribute value identifier</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CheckoutAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest();

        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(id);

        if (checkoutAttributeValue == null)
        {
            return NotFound($"Checkout attribute value Id={id} is not found");
        }

        return Ok(checkoutAttributeValue.ToDto<CheckoutAttributeValueDto>());
    }

    /// <summary>
    /// Create a checkout attribute value
    /// </summary>
    /// <param name="model">Checkout attribute value Dto model</param>
    [HttpPost]
    [ProducesResponseType(typeof(CheckoutAttributeDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] CheckoutAttributeValueDto model)
    {
        var checkoutAttributeValue = model.FromDto<CheckoutAttributeValue>();

        await _checkoutAttributeService.InsertAttributeValueAsync(checkoutAttributeValue);

        return Ok(checkoutAttributeValue.ToDto<CheckoutAttributeValueDto>());
    }

    /// <summary>
    /// Update a checkout attribute value
    /// </summary>
    /// <param name="model">Checkout attribute value Dto model</param>
    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] CheckoutAttributeValueDto model)
    {
        var checkoutAttributeValue = await _checkoutAttributeService.GetAttributeValueByIdAsync(model.Id);

        if (checkoutAttributeValue == null)
            return NotFound("Checkout attribute value is not found");

        checkoutAttributeValue = model.FromDto<CheckoutAttributeValue>();
        await _checkoutAttributeService.UpdateAttributeValueAsync(checkoutAttributeValue);

        return Ok();
    }

    #endregion
}