using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace EduTrack.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior for logging requests and responses with correlation tracking
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Activity.Current?.Id ?? Guid.NewGuid().ToString();
        
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["RequestName"] = requestName
        }))
        {
            _logger.LogInformation(
                "Starting request {RequestName} with correlation ID {CorrelationId}",
                requestName, correlationId);

            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var response = await next();
                
                stopwatch.Stop();
                
                _logger.LogInformation(
                    "Completed request {RequestName} with correlation ID {CorrelationId} in {ElapsedMs}ms",
                    requestName, correlationId, stopwatch.ElapsedMilliseconds);
                
                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                
                _logger.LogError(ex,
                    "Request {RequestName} with correlation ID {CorrelationId} failed after {ElapsedMs}ms: {ErrorMessage}",
                    requestName, correlationId, stopwatch.ElapsedMilliseconds, ex.Message);
                
                throw;
            }
        }
    }
}