﻿using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Backend.Dto.Directory;

/// <summary>
/// Represents a measure dimension
/// </summary>
public partial class MeasureDimensionDto : DtoWithId
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the system keyword
    /// </summary>
    public string SystemKeyword { get; set; }

    /// <summary>
    /// Gets or sets the ratio
    /// </summary>
    public decimal Ratio { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}