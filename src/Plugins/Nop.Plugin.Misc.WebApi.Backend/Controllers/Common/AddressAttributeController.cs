using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Common;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Common;

public partial class AddressAttributeController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;

    #endregion

    #region Ctor

    public AddressAttributeController(IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService)
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

        var addressAttribute = await _addressAttributeService.GetAttributeByIdAsync(id);

        if (addressAttribute == null)
            return NotFound($"Address attribute Id={id} not found");

        await _addressAttributeService.DeleteAttributeAsync(addressAttribute);

        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AddressAttributeDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
            return BadRequest();

        var addressAttribute = await _addressAttributeService.GetAttributeByIdAsync(id);

        if (addressAttribute == null)
            return NotFound($"Address attribute Id={id} not found");

        return Ok(addressAttribute.ToDto<AddressAttributeDto>());
    }

    /// <summary>
    /// Gets all address attributes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IList<AddressAttributeDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll()
    {
        var addressAttributes = await _addressAttributeService.GetAllAttributesAsync();

        return Ok(addressAttributes.Select(aa => aa.ToDto<AddressAttributeDto>()).ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddressAttributeDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] AddressAttributeDto model)
    {
        var addressAttribute = model.FromDto<AddressAttribute>();

        await _addressAttributeService.InsertAttributeAsync(addressAttribute);

        var addressAttributeDto = addressAttribute.ToDto<AddressAttributeDto>();

        return Ok(addressAttributeDto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] AddressAttributeDto model)
    {
        var addressAttribute = await _addressAttributeService.GetAttributeByIdAsync(model.Id);

        if (addressAttribute == null)
            return NotFound($"Address attribute Id={model.Id} is not found");

        addressAttribute = model.FromDto<AddressAttribute>();

        await _addressAttributeService.UpdateAttributeAsync(addressAttribute);

        return Ok();
    }

    #endregion
}