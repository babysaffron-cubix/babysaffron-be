using System;
namespace Nop.Services.Customers;

public partial class OtpValidationResult
{

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; }

    public OtpValidationResult()
    {
        Errors = new List<string>();
    }

    /// <summary>
    /// Get a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

    /// <summary>
    /// Add error
    /// </summary>
    /// <param name="error">Error message</param>
    public void AddError(string error)
    {
        Errors.Add(error);
    }
}

