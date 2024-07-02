using System;
using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Customer;

public partial class CustomerUpdateRequestModelDto : ModelDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int CountryId { get; set; }

    public string Phone { get; set; }
}

