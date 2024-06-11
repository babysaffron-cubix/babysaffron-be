using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto;

public partial class BaseModelDtoRequest<TModel> : BaseDto
    where TModel : BaseDto
{
    public TModel Model { get; set; }

    public IDictionary<string, string> Form { get; set; }
}