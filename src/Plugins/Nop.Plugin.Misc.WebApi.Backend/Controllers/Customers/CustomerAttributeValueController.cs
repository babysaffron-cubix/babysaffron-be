﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Customers;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Customers;

public partial class CustomerAttributeValueController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<CustomerAttribute, CustomerAttributeValue> _customerAttributeService;

    #endregion

    #region Ctor

    public CustomerAttributeValueController(IAttributeService<CustomerAttribute, CustomerAttributeValue> customerAttributeService)
    {
        _customerAttributeService = customerAttributeService;
    }

    #endregion

    #region Methods

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest();

        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id);

        if (customerAttributeValue == null)
            return NotFound($"Customer attribute value Id={id} not found");

        await _customerAttributeService.DeleteAttributeValueAsync(customerAttributeValue);

        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CustomerAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest();

        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(id);

        if (customerAttributeValue == null)
            return NotFound($"Customer attribute value Id={id} not found");

        return Ok(customerAttributeValue.ToDto<CustomerAttributeValueDto>());
    }

    /// <summary>
    /// Gets customer attribute values by customer attribute identifier
    /// </summary>
    /// <param name="attributeId">The customer attribute identifier</param>
    [HttpGet("{attributeId}")]
    [ProducesResponseType(typeof(IList<CustomerAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll(int attributeId)
    {
        var customerAttributeValues = await _customerAttributeService.GetAttributeValuesAsync(attributeId);
        var customerAttributeValuesDto =
            customerAttributeValues.Select(av => av.ToDto<CustomerAttributeValueDto>());

        return Ok(customerAttributeValuesDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CustomerAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] CustomerAttributeValueDto model)
    {
        var customerAttributeValue = model.FromDto<CustomerAttributeValue>();

        await _customerAttributeService.InsertAttributeValueAsync(customerAttributeValue);

        var customerAttributeValueDto = customerAttributeValue.ToDto<CustomerAttributeValueDto>();

        return Ok(customerAttributeValueDto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] CustomerAttributeValueDto model)
    {
        var customerAttributeValue = await _customerAttributeService.GetAttributeValueByIdAsync(model.Id);

        if (customerAttributeValue == null)
            return NotFound($"Customer attribute value Id={model.Id} is not found");

        customerAttributeValue = model.FromDto<CustomerAttributeValue>();

        await _customerAttributeService.UpdateAttributeValueAsync(customerAttributeValue);

        return Ok();
    }

    #endregion
}