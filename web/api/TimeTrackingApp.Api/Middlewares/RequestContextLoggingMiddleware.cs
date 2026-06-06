using Microsoft.Extensions.Primitives;
using Serilog.Context;

namespace TimeTrackingApp.Api.Middlewares;

public sealed class RequestContextLoggingMiddleware(RequestDelegate next) 
{
    private const string CorrelationHeaderName = "X-Corelation-Id";

    public async Task Invoke(HttpContext context)
    {
        string correlationId = GetCorelationId(context);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(context);
        }
    }

    private string GetCorelationId(HttpContext context)
    {
        context.Request.Headers.TryGetValue(
            CorrelationHeaderName, out StringValues correlationId);

        return correlationId.FirstOrDefault() ?? context.TraceIdentifier;
    }
}
