using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record TestimonialModel : BaseNopModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Rating { get; set; }

}

