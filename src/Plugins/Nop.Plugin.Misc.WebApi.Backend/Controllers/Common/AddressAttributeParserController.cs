using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Common;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Common;

public partial class AddressAttributeParserController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    private readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;

    #endregion

    #region Ctor

    public AddressAttributeParserController(IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService)
    {
        _addressAttributeParser = addressAttributeParser;
        _addressAttributeService = addressAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets selected address attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<AddressAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseAddressAttributes([FromBody] string attributesXml)
    {
        var attributes = await _addressAttributeParser.ParseAttributesAsync(attributesXml);

        return Ok(attributes.ToDto<AddressAttributeDto>());
    }

    /// <summary>
    /// Get address attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<AddressAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseAddressAttributeValues([FromBody] string attributesXml)
    {
        var values = await _addressAttributeParser.ParseAttributeValuesAsync(attributesXml);

        return Ok(values.ToDto<AddressAttributeDto>());
    }

    /// <summary>
    /// Gets selected address attribute value
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="addressAttributeId">Address attribute identifier</param>
    [HttpPost("{addressAttributeId}")]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    public IActionResult ParseValues([FromBody] string attributesXml, int addressAttributeId)
    {
        var values = _addressAttributeParser.ParseValues(attributesXml, addressAttributeId);

        return Ok(values);
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="addressAttributeId">Address attribute</param>
    /// <param name="value">Value</param>
    [HttpPost("{addressAttributeId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> AddAddressAttribute([FromBody] string attributesXml,
        int addressAttributeId,
        [FromQuery, Required] string value)
    {
        if (addressAttributeId <= 0)
            return BadRequest();

        var attribute =
            await _addressAttributeService.GetAttributeByIdAsync(addressAttributeId);

        if (attribute == null)
            return NotFound($"Address attribute Id={addressAttributeId} not found");

        var attributes = _addressAttributeParser.AddAttribute(attributesXml, attribute, value);

        return Ok(attributes);
    }

    /// <summary>
    /// Validates address attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAttributeWarnings([FromBody] string attributesXml)
    {
        var warnings = await _addressAttributeParser.GetAttributeWarningsAsync(attributesXml);

        return Ok(warnings);
    }

    #endregion
}