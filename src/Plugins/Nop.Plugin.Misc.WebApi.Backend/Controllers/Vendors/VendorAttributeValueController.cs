using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Vendors;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Vendors;

public partial class VendorAttributeValueController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeService<VendorAttribute, VendorAttributeValue> _vendorAttributeService;

    #endregion

    #region Ctor

    public VendorAttributeValueController(IAttributeService<VendorAttribute, VendorAttributeValue> vendorAttributeService)
    {
        _vendorAttributeService = vendorAttributeService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets a vendor attribute value by identifier
    /// </summary>
    /// <param name="id">Vendor attribute value identifier</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(VendorAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetById(int id)
    {
        var vendorAttributeValue = await _vendorAttributeService.GetAttributeValueByIdAsync(id);

        if (vendorAttributeValue == null)
        {
            return NotFound($"Vendor attribute value Id={id} not found");
        }

        var vendorattributeValueDto = vendorAttributeValue.ToDto<VendorAttributeValueDto>();

        return Ok(vendorattributeValueDto);
    }

    /// <summary>
    /// Gets vendor attribute values by vendor attribute identifier
    /// </summary>
    /// <param name="id">The vendor attribute identifier</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IList<VendorAttributeValueDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetByVendorAttributeId(int id)
    {
        var vendorAttributeValues = await _vendorAttributeService.GetAttributeValuesAsync(id);

        if (vendorAttributeValues == null)
            return NotFound($"Vendor attribute values by vendor attribute Id={id} not found");

        var vendorAttributeValuesDto = vendorAttributeValues.Select(vendorAttributeValue => vendorAttributeValue.ToDto<VendorAttributeValueDto>()).ToList();

        return Ok(vendorAttributeValuesDto);
    }

    /// <summary>
    /// Create a vendor attribute value
    /// </summary>
    /// <param name="model">Vendor attribute value Dto</param>
    [HttpPost]
    [ProducesResponseType(typeof(VendorAttributeValueDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] VendorAttributeValueDto model)
    {
        var vendorAttributeValue = model.FromDto<VendorAttributeValue>();

        await _vendorAttributeService.InsertAttributeValueAsync(vendorAttributeValue);

        var vendorAttributeValueDto = vendorAttributeValue.ToDto<VendorAttributeValueDto>();

        return Ok(vendorAttributeValueDto);
    }

    /// <summary>
    /// Updates the vendor attribute value
    /// </summary>
    /// <param name="model">Vendor attribute value Dto model</param>
    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] VendorAttributeValueDto model)
    {
        var vendorAttributeValue = await _vendorAttributeService.GetAttributeValueByIdAsync(model.Id);

        if (vendorAttributeValue == null)
            return NotFound("Vendor attribute value is not found");

        vendorAttributeValue = model.FromDto<VendorAttributeValue>();

        await _vendorAttributeService.UpdateAttributeValueAsync(vendorAttributeValue);

        return Ok();
    }

    /// <summary>
    /// Delete a vendor attribute value
    /// </summary>
    /// <param name="id">Vendor attribute value identifier</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return BadRequest();

        var vendorAttributeValue = await _vendorAttributeService.GetAttributeValueByIdAsync(id);

        if (vendorAttributeValue == null)
            return NotFound($"Vendor attribute value Id={id} not found");

        await _vendorAttributeService.DeleteAttributeValueAsync(vendorAttributeValue);

        return Ok();
    }

    #endregion
}