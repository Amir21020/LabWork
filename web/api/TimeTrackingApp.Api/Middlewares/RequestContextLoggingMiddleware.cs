using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace TimeTrackingApp.Api.Middlewares;

public sealed class RequestContextLoggingMiddleware(RequestDelegate next) 
{
    private const string CorrelationHeaderName = "X-Correlation-Id";

    public async Task Invoke(HttpContext context)
    {
        string correlationId = GetCorrelationId(context);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private string GetCorrelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
