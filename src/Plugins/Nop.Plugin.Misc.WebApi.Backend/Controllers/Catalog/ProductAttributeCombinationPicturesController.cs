using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.WebApi.Backend.Dto.Catalog;
using Nop.Plugin.Misc.WebApi.Framework.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Catalog;

public partial class ProductAttributeCombinationPicturesController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IProductAttributeService _productAttributeService;
    private readonly IRepository<ProductAttributeCombinationPicture> _productAttributeCombinationPictureRepository;

    #endregion

    #region Ctor

    public ProductAttributeCombinationPicturesController(IProductAttributeService productAttributeService,
        IRepository<ProductAttributeCombinationPicture> productAttributeCombinationPictureRepository)
    {
        _productAttributeService = productAttributeService;
        _productAttributeCombinationPictureRepository = productAttributeCombinationPictureRepository;
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

        var combinationPicture = await _productAttributeCombinationPictureRepository.GetByIdAsync(id);

        if (combinationPicture == null)
            return NotFound($"Product attribute combination picture Id={id} not found");

        await _productAttributeService.DeleteProductAttributeCombinationPictureAsync(combinationPicture);

        return Ok();
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductAttributeCombinationPictureDto), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Create([FromBody] ProductAttributeCombinationPictureDto model)
    {
        var productAttributeCombinationPicture = model.FromDto<ProductAttributeCombinationPicture>();

        await _productAttributeService.InsertProductAttributeCombinationPictureAsync(productAttributeCombinationPicture);

        var productAttributeCombinationPictureDto = productAttributeCombinationPicture.ToDto<ProductAttributeCombinationPictureDto>();

        return Ok(productAttributeCombinationPictureDto);
    }

    [HttpPut]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> Update([FromBody] ProductAttributeDto model)
    {
        var combinationPicture = await _productAttributeCombinationPictureRepository.GetByIdAsync(model.Id);

        if (combinationPicture == null)
            return NotFound($"Product attribute combination picture Id={model.Id} not found");

        combinationPicture = model.FromDto<ProductAttributeCombinationPicture>();

        await _productAttributeService.UpdateProductAttributeCombinationPictureAsync(combinationPicture);

        return Ok();
    }

    /// <summary>
    /// Get product attribute combination pictures
    /// </summary>
    /// <param name="combinationId">Combination id</param>
    [HttpGet("combinationId")]
    [ProducesResponseType(typeof(IList<ProductAttributeCombinationPictureDto>), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> GetAll(int combinationId)
    {
        var combinationPictures = await _productAttributeService.GetProductAttributeCombinationPicturesAsync(combinationId);

        return Ok(combinationPictures);
    }

    #endregion
}