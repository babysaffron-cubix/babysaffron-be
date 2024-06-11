﻿using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Common;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Common;

public partial class AddressAttributeValueController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;

    #endregion

    #region Ctor

    public AddressAttributeValueController(IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService)
    {
        _addressAttributeService = addressAttributeService;
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

        var addressAttributeValue = await _addressAttributeService.GetAttributeValueByIdAsync(id);

        if (addressAttributeValue == null)
            return NotFound($"Address attribute value Id={id} not found");

        await _addressAttributeService.DeleteAttributeValueAsync(addressAttributeValue);

        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AddressAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest();

        var addressAttributeValue = await _addressAttributeService.GetAttributeValueByIdAsync(id);

        if (addressAttributeValue == null)
            return NotFound($"Address attribute value Id={id} not found");

        return Ok(addressAttributeValue.ToDto<AddressAttributeValueDto>());
    }

    /// <summary>
    /// Gets address attribute values by address attribute identifier
    /// </summary>
    /// <param name="addressAttributeId">The address attribute identifier</param>
    [HttpGet("{addressAttributeId}")]
    [ProducesResponseType(typeof(IList<AddressAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll(int addressAttributeId)
    {
        var addressAttributeValues = await _addressAttributeService.GetAttributeValuesAsync(addressAttributeId);

        return Ok(addressAttributeValues.Select(v => v.ToDto<AddressAttributeValueDto>()).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddressAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] AddressAttributeValueDto model)
    {
        var addressAttributeValue = model.FromDto<AddressAttributeValue>();

        await _addressAttributeService.InsertAttributeValueAsync(addressAttributeValue);

        var addressAttributeValueDto = addressAttributeValue.ToDto<AddressAttributeValueDto>();

        return Ok(addressAttributeValueDto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] AddressAttributeValueDto model)
    {
        var addressAttributeValue = await _addressAttributeService.GetAttributeValueByIdAsync(model.Id);

        if (addressAttributeValue == null)
            return NotFound($"Address attribute value Id={model.Id} is not found");

        addressAttributeValue = model.FromDto<AddressAttributeValue>();

        await _addressAttributeService.UpdateAttributeValueAsync(addressAttributeValue);

        return Ok();
    }

    #endregion
}