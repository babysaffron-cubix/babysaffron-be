using System;
using Nop.Plugin.Misc.WebApi.Framework.Dto;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Product;
using Nop.Web.Models.Media;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Catalog;

public partial class ActiveProductsModelDto : ModelWithIdDto
{
    public ActiveProductsModelDto()
    {
        PictureModels = new List<PictureModelDto>();
    }

    public string Name { get; set; }

    public string ShortDescription { get; set; }

    public string FullDescription { get; set; }

    public string Sku { get; set; }

    public string Price { get; set; }

    public IList<PictureModelDto> PictureModels { get; set; }
}

