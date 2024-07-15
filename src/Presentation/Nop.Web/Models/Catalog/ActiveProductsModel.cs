using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Media;

namespace Nop.Web.Models.Catalog;

public partial record ActiveProductsModel : BaseNopEntityModel
{
	public ActiveProductsModel()
	{
		PictureModels = new List<PictureModel>();
    }

    //public int Id { get; set; }
    public string Name { get; set; }
    public string ShortDescription { get; set; }
    public string FullDescription { get; set; }
    public string Price { get; set; }

    public string Sku { get; set; }
    //pictures
    public IList<PictureModel> PictureModels { get; set; }
}

