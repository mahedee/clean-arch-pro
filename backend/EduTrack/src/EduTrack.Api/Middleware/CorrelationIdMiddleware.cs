using System.Diagnostics;

namespace EduTrack.Api.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationIdValues)
            ? correlationIdValues.ToString()
            : Guid.NewGuid().ToString();

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items[CorrelationIdHeader] = correlationId;
        Activity.Current?.SetTag("correlation_id", correlationId);

        await next(context);
    }
}
