﻿using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Backend.Dto.Catalog;

/// <summary>
/// Represents a cross-sell product
/// </summary>
public partial class CrossSellProductDto : DtoWithId
{
    /// <summary>
    /// Gets or sets the first product identifier
    /// </summary>
    public int ProductId1 { get; set; }

    /// <summary>
    /// Gets or sets the second product identifier
    /// </summary>
    public int ProductId2 { get; set; }
}