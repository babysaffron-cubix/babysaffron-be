﻿using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Common;

public partial class AddressModelDto : ModelWithIdDto
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public bool CompanyEnabled { get; set; }

    public bool CompanyRequired { get; set; }

    public string Company { get; set; }

    public bool CountryEnabled { get; set; }

    public int? CountryId { get; set; }

    public string CountryName { get; set; }

    public bool StateProvinceEnabled { get; set; }

    public int? StateProvinceId { get; set; }

    public string StateProvinceName { get; set; }

    public bool CountyEnabled { get; set; }

    public bool CountyRequired { get; set; }

    public string County { get; set; }

    public bool CityEnabled { get; set; }

    public bool CityRequired { get; set; }

    public string City { get; set; }

    public bool StreetAddressEnabled { get; set; }

    public bool StreetAddressRequired { get; set; }

    public string Address1 { get; set; }

    public bool StreetAddress2Enabled { get; set; }

    public bool StreetAddress2Required { get; set; }

    public string Address2 { get; set; }

    public bool ZipPostalCodeEnabled { get; set; }

    public bool ZipPostalCodeRequired { get; set; }

    public string ZipPostalCode { get; set; }

    public bool PhoneEnabled { get; set; }

    public bool PhoneRequired { get; set; }

    public string PhoneNumber { get; set; }

    public bool FaxEnabled { get; set; }

    public bool FaxRequired { get; set; }

    public string FaxNumber { get; set; }

    public IList<SelectListItemDto> AvailableCountries { get; set; }

    public IList<SelectListItemDto> AvailableStates { get; set; }

    public string FormattedCustomAddressAttributes { get; set; }

    public IList<AddressAttributeModelDto> CustomAddressAttributes { get; set; }
}