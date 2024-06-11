﻿using Nop.Plugin.Misc.WebApi.Framework.Dto;

namespace Nop.Plugin.Misc.WebApi.Backend.Dto.Media;

/// <summary>
/// Represents a picture
/// </summary>
public partial class PictureDto : DtoWithId
{
    /// <summary>
    /// Gets or sets the picture mime type
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// Gets or sets the SEO friendly filename of the picture
    /// </summary>
    public string SeoFilename { get; set; }

    /// <summary>
    /// Gets or sets the "alt" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
    /// </summary>
    public string AltAttribute { get; set; }

    /// <summary>
    /// Gets or sets the "title" attribute for "img" HTML element. If empty, then a default rule will be used (e.g. product name)
    /// </summary>
    public string TitleAttribute { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the picture is new
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    /// Gets or sets the picture virtual path
    /// </summary>
    public string VirtualPath { get; set; }
}