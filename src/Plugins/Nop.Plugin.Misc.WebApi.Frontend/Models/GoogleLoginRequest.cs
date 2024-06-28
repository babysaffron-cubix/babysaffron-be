using System;
namespace Nop.Plugin.Misc.WebApi.Frontend.Models;

public partial class GoogleLoginRequest
{
	public string Token { get; set; }
	public string Name { get; set; }
	public string Email { get; set; }
}

