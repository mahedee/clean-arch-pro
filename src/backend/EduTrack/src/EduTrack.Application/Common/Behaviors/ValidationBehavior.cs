using EduTrack.Application.Common.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EduTrack.Application.Common.Behaviors;

/// <summary>
/// Enhanced MediatR pipeline behavior that runs FluentValidation validators before command handlers
/// with improved error logging and correlation tracking
/// </summary>
/// <typeparam name="TRequest">The request type (command/query)</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, 
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // If no validators are registered for this request type, continue to next behavior/handler
        if (!_validators.Any())
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        
        _logger.LogDebug("Running validation for {RequestName} with {ValidatorCount} validators", 
            requestName, _validators.Count());

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Run all validators for this request type
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // If there are validation failures, log and throw enhanced ValidationException
        if (failures.Any())
        {
            _logger.LogWarning("Validation failed for {RequestName} with {FailureCount} errors: {Failures}",
                requestName, failures.Count, 
                string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

            throw new FluentValidation.ValidationException(failures);
        }

        _logger.LogDebug("Validation passed for {RequestName}", requestName);

        // If validation passes, continue to the next behavior/handler
        return await next();
    }
}
