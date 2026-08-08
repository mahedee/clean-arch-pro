using System.Diagnostics;

namespace EduTrack.Api.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Append(CorrelationIdHeader, correlationId);
        }

        context.Response.Headers.Append(CorrelationIdHeader, correlationId);
        Activity.Current?.SetTag("correlation_id", correlationId.ToString());

        await next(context);
    }
}
