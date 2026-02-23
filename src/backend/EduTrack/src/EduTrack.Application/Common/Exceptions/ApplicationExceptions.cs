using EduTrack.Domain.Common.Exceptions;

namespace EduTrack.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when application business logic fails
/// </summary>
public class ApplicationException : DomainException
{
    public ApplicationException(string message, string errorCode = "APPLICATION_ERROR")
        : base(message, errorCode)
    {
    }

    public ApplicationException(string message, Exception innerException, string errorCode = "APPLICATION_ERROR")
        : base(message, errorCode, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when a requested resource cannot be found
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string message)
        : base(message, "NOT_FOUND")
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.", "NOT_FOUND")
    {
    }
}

/// <summary>
/// Exception thrown when validation fails in application layer
/// </summary>
public class ValidationException : ApplicationException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.", "VALIDATION_ERROR")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}

/// <summary>
/// Exception thrown when forbidden operations are attempted
/// </summary>
public class ForbiddenAccessException : ApplicationException
{
    public ForbiddenAccessException()
        : base("Access to this resource is forbidden.", "FORBIDDEN_ACCESS")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message, "FORBIDDEN_ACCESS")
    {
    }
}

/// <summary>
/// Exception thrown when external service operations fail
/// </summary>
public class ExternalServiceException : ApplicationException
{
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName, string message)
        : base($"External service '{serviceName}' failed: {message}", "EXTERNAL_SERVICE_ERROR")
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException)
        : base($"External service '{serviceName}' failed: {message}", innerException, "EXTERNAL_SERVICE_ERROR")
    {
        ServiceName = serviceName;
    }
}