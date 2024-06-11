﻿namespace Nop.Plugin.Misc.WebApi.Backend.Dto.Customers;

public partial class ChangePasswordResult
{
    public ChangePasswordResult(Nop.Services.Customers.ChangePasswordResult result)
    {
        Success = result.Success;
        Errors = new List<string>(result.Errors);
    }

    /// <summary>
    /// Gets a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; }
}