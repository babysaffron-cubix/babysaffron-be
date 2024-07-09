﻿using System;
using Razorpay.Api;

namespace Nop.Services.Payments;

	public partial class RazorpayOrderCreationResponse
	{
    public RazorpayOrderCreationResponse()
    {
        Errors = new List<string>();
    }

    /// <summary>
    /// Gets a value indicating whether request has been completed successfully
    /// </summary>
    public bool Success => !Errors.Any();

    /// <summary>
    /// Add error
    /// </summary>
    /// <param name="error">Error</param>
    public void AddError(string error)
    {
        Errors.Add(error);
    }

    /// <summary>
    /// Errors
    /// </summary>
    public IList<string> Errors { get; set; }

    public Order Order { get; set; }
}

