using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Backend.Dto.Catalog;

/// <summary>
/// Represents a product attribute combination picture
/// </summary>
public partial class ProductAttributeCombinationPictureDto : DtoWithId
{
    /// <summary>
    /// Gets or sets the product attribute combination id
    /// </summary>
    public int ProductAttributeCombinationId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of picture associated with this combination
    /// </summary>
    public int PictureId { get; set; }
}