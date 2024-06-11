﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Customers;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Customers;

public partial class CustomerAttributeParserController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeParser<CustomerAttribute, CustomerAttributeValue> _customerAttributeParser;
    private readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;

    #endregion

    #region Ctor

    public CustomerAttributeParserController(IAttributeParser<CustomerAttribute, CustomerAttributeValue> customerAttributeParser,
        IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService)
    {
        _customerAttributeParser = customerAttributeParser;
        _customerAttributeService = customerAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets selected customer attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<CustomerAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseCustomerAttributes([FromBody] string attributesXml)
    {
        var attributes = await _customerAttributeParser.ParseAttributesAsync(attributesXml);
        var attributesDto = attributes.Select(a => a.ToDto<CustomerAttributeDto>());

        return Ok(attributesDto);
    }

    /// <summary>
    /// Get customer attribute values
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<CustomerAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> ParseCustomerAttributeValues([FromBody] string attributesXml)
    {
        var attributes = await _customerAttributeParser.ParseAttributeValuesAsync(attributesXml);
        var attributesDto = attributes.Select(a => a.ToDto<CustomerAttributeDto>());

        return Ok(attributesDto);
    }

    /// <summary>
    /// Gets selected customer attribute value
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="attributeId">Customer attribute identifier</param>
    [HttpPost("{attributeId}")]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    public IActionResult ParseValues([FromBody] string attributesXml, int attributeId)
    {
        var values = _customerAttributeParser.ParseValues(attributesXml, attributeId);

        return Ok(values);
    }

    /// <summary>
    /// Adds an attribute
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="customerAttributeId">Customer attribute</param>
    /// <param name="value">Value</param>
    [HttpPost("{customerAttributeId}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public virtual async Task<IActionResult> AddCustomerAttribute([FromBody] string attributesXml, int customerAttributeId, [FromQuery][Required] string value)
    {
        if (customerAttributeId <= 0)
            return BadRequest();

        var customerAttribute = await _customerAttributeService.GetAttributeByIdAsync(customerAttributeId);

        if (customerAttribute == null)
            return NotFound($"Customer attribute Id={customerAttributeId} not found");

        var response = _customerAttributeParser.AddAttribute(attributesXml, customerAttribute, value);

        return Ok(response);
    }

    /// <summary>
    /// Validates customer attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    [HttpPost]
    [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAttributeWarnings([FromBody] string attributesXml)
    {
        var warnings = await _customerAttributeParser.GetAttributeWarningsAsync(attributesXml);

        return Ok(warnings);
    }

    #endregion
}