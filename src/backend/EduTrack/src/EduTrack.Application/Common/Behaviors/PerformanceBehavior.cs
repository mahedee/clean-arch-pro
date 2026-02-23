using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EduTrack.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for performance monitoring and alerting on slow operations
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _timer;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;
        var requestName = typeof(TRequest).Name;

        // Log warning for operations taking longer than 500ms
        if (elapsedMilliseconds > 500)
        {
            _logger.LogWarning("Long running request detected: {RequestName} took {ElapsedMs}ms to complete",
                requestName, elapsedMilliseconds);
        }
        else
        {
            _logger.LogInformation("Request {RequestName} completed in {ElapsedMs}ms",
                requestName, elapsedMilliseconds);
        }

        return response;
    }
}