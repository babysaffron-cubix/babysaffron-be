using System;
namespace Nop.Plugin.Misc.WebApi.Framework.Models;

public class AuthenticateResponseCustom :AuthenticateResponse
{
    public AuthenticateResponseCustom(string token) :base(token)
    {
        
        
    }
    public int StatusCode { get; set; }
	
}

