using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Orders;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Orders;

public partial class CheckoutAttributeParserController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeParser;
    private readonly IAttributeService<CheckoutAttribute, CheckoutAttributeValue> _checkoutAttributeService;

    #endregion

    #region Ctor

    public CheckoutAttributeParserController(IAttributeParser<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeParser,
        IAttributeService<CheckoutAttribute, CheckoutAttributeValue> checkoutAttributeService)
    {
        _checkoutAttributeParser = checkoutAttributeParser;
        _checkoutAttributeService = checkoutAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets selected checkout attributes
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IList<CheckoutAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseCheckoutAttributes([FromBody] string attributesXml)
    {
        var attributes = await _checkoutAttributeParser.ParseAttributesAsync(attributesXml);
        var attributesDto = attributes.Select(a => a.ToDto<CheckoutAttributeDto>());

        return Ok(attributesDto);
    }

    /// <summary>
    /// Get checkout attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<ParseCheckoutAttributeValuesResponse>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseCheckoutAttributeValues([FromBody] string attributesXml)
    {
        var values = await _checkoutAttributeParser.ParseAttributeValues(attributesXml).ToListAsync();

        var valuesDto = await values.SelectAwait(async value => new ParseCheckoutAttributeValuesResponse
        {
            Attribute = value.attribute.ToDto<CheckoutAttributeDto>(),
            Values = (await value.values.ToListAsync()).Select(p => p.ToDto<CheckoutAttributeValueDto>()).ToList()
        }).ToListAsync();

        return Ok(valuesDto);
    }

    /// <summary>
    /// Gets selected checkout attribute value
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Checkout attribute identifier</param>
    [HttpPost("{attributeId}")]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    public IActionResult ParseValues([FromBody] string attributesXml, int attributeId)
    {
        var values = _checkoutAttributeParser.ParseValues(attributesXml, attributeId);

        return Ok(values);
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="checkoutAttributeId">Checkout attribute</param>
    /// <param name="value">Value</param>
    [HttpPost("{checkoutAttributeId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> AddCheckoutAttribute([FromBody] string attributesXml,
        int checkoutAttributeId,
        [FromQuery, Required] string value)
    {
        if (checkoutAttributeId <= 0)
            return BadRequest();

        var customerAttribute = await _checkoutAttributeService.GetAttributeByIdAsync(checkoutAttributeId);

        if (customerAttribute == null)
            return NotFound($"Checkout attribute Id={checkoutAttributeId} not found");

        var rezAttributesXml =
            _checkoutAttributeParser.AddAttribute(attributesXml, customerAttribute, value);

        return Ok(rezAttributesXml);
    }

    /// <summary>
    /// Check whether condition of some attribute is met (if specified). Return "null" if not condition is specified
    /// </summary>
    /// <param name="attributeId">Checkout attribute Id</param>
    /// <param name="attributesXml">Selected attributes (XML format)</param>
    [HttpPost("{attributeId}")]
    [ProducesResponseType(typeof(bool?), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> IsConditionMet(int attributeId, [FromBody] string attributesXml)
    {
        if (attributeId <= 0)
            return BadRequest();

        var attribute = await _checkoutAttributeService.GetAttributeByIdAsync(attributeId);

        if (attribute == null)
            return NotFound($"Checkout attribute Id={attributeId} not found");

        var flag = await _checkoutAttributeParser.IsConditionMetAsync(attribute.ConditionAttributeXml, attributesXml);

        return Ok(flag);
    }

    /// <summary>
    /// Remove an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Checkout attribute Id</param>
    [HttpPost("{attributeId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> RemoveCheckoutAttribute([FromBody] string attributesXml, int attributeId)
    {
        if (attributeId <= 0)
            return BadRequest();

        var attribute = await _checkoutAttributeService.GetAttributeByIdAsync(attributeId);

        if (attribute == null)
            return NotFound($"Checkout attribute Id={attributeId} not found");

        var xml = _checkoutAttributeParser.RemoveAttribute(attributesXml, attribute.Id);

        return Ok(xml);
    }

    #endregion
}