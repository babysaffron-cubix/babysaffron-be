﻿using System;
namespace Nop.Core.Domain.Salesforce;

public partial class SalesforceOrderResponse
{
	public string SFDCRecordId { get; set; }
	public string SFDCNumber { get; set; }
	public string ResultMsg { get; set; }
	public bool CalloutErrorResult { get; set; }
}
