using System;
using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Boards; 

public partial class TestimonialsModelDto : ModelWithIdDto
{
	public string Name { get; set; }
	public string Description { get; set; }
	public string Rating { get; set; }
}

