using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.WebApi.Framework.Middleware;

/// <summary>
/// The custom error handler middleware
/// </summary>
public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly ISettingService _settingService;

    public ErrorHandlerMiddleware(ILogger logger, ISettingService settingService, RequestDelegate next)
    {
        _next = next;
        _logger = logger;
        _settingService = settingService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            //rethrow exception if it's happened not in the API call
            if (!(context.Request.Path.Value?.StartsWith("/api-") ?? false))
                throw;

            var response = context.Response;
            response.ContentType = "application/json";

            response.StatusCode = error switch
            {
                // custom application error
                NopException => (int)HttpStatusCode.BadRequest,
                // not found error
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                // unhandled error
                _ => (int)HttpStatusCode.InternalServerError
            };

            var apiSettings = await _settingService.LoadSettingAsync<WebApiCommonSettings>();

            var result = JsonSerializer.Serialize(new { message = error.Message, inner_exception_message = error.InnerException?.Message, stack_trace = apiSettings.DeveloperMode ? error.ToString() : string.Empty });

            await _logger.ErrorAsync(error.Message, error);

            await response.WriteAsync(result);
        }
    }
}