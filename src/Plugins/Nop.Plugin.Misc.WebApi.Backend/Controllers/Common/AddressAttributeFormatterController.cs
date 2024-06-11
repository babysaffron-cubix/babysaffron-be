using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Common;
using Nop.Services.Attributes;

namespace Nop.Plugin.Misc.WebApi.Backend.Controllers.Common;

public partial class AddressAttributeFormatterController : BaseNopWebApiBackendController
{
    #region Fields

    private readonly IAttributeFormatter<AddressAttribute, AddressAttributeValue> _addressAttributeFormatter;

    #endregion

    #region Ctor

    public AddressAttributeFormatterController(IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter)
    {
        _addressAttributeFormatter = addressAttributeFormatter;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Formats attributes
    /// </summary>
    /// <param name="attributesXml">Attributes in XML format</param>
    /// <param name="separator">Separator</param>
    /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> FormatAttributes([FromBody] string attributesXml,
        [FromQuery] string separator = "<br />",
        [FromQuery] bool htmlEncode = true)
    {
        var formattedAttribute = await
            _addressAttributeFormatter.FormatAttributesAsync(attributesXml, separator, htmlEncode);

        return Ok(formattedAttribute);
    }

    #endregion
}