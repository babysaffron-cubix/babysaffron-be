using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Catalog;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Catalog;

public partial class ProductAttributeValuePictureController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IProductAttributeService _productAttributeService;
    private readonly IRepository<ProductAttributeValuePicture> _productAttributeValuePictureRepository;

    #endregion

    #region Ctor

    public ProductAttributeValuePictureController(IProductAttributeService productAttributeService,
        IRepository<ProductAttributeValuePicture> productAttributeCombinationPictureRepository)
    {
        _productAttributeService = productAttributeService;
        _productAttributeValuePictureRepository = productAttributeCombinationPictureRepository;
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

        var valuePicture = await _productAttributeValuePictureRepository.GetByIdAsync(id);

        if (valuePicture == null)
            return NotFound($"Product attribute value picture Id={id} not found");

        await _productAttributeService.DeleteProductAttributeValuePictureAsync(valuePicture);

        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductAttributeValuePictureDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] ProductAttributeValuePictureDto model)
    {
        var productAttributeCombinationPicture = model.FromDto<ProductAttributeValuePicture>();

        await _productAttributeService.InsertProductAttributeValuePictureAsync(productAttributeCombinationPicture);

        var productAttributeCombinationPictureDto = productAttributeCombinationPicture.ToDto<ProductAttributeValuePictureDto>();

        return Ok(productAttributeCombinationPictureDto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] ProductAttributeDto model)
    {
        var valuePicture = await _productAttributeValuePictureRepository.GetByIdAsync(model.Id);

        if (valuePicture == null)
            return NotFound($"Product attribute value picture Id={model.Id} not found");

        valuePicture = model.FromDto<ProductAttributeValuePicture>();

        await _productAttributeService.UpdateProductAttributeValuePictureAsync(valuePicture);

        return Ok();
    }

    /// <summary>
    /// Get product attribute value pictures
    /// </summary>
    /// <param name="valueId">Value id</param>
    [HttpGet("valueId")]
    [ProducesResponseType(typeof(IList<ProductAttributeValuePictureDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll(int valueId)
    {
        var valuePictures = await _productAttributeService.GetProductAttributeValuePicturesAsync(valueId);

        return Ok(valuePictures);
    }

    #endregion
}