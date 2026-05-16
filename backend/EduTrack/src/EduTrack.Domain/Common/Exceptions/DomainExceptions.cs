using System.Net;

namespace EduTrack.Domain.Common.Exceptions;

/// <summary>
/// Base domain exception class
/// </summary>
public abstract class DomainException : Exception
{
    public string ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }

    protected DomainException(string message, string errorCode, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected DomainException(string message, string errorCode, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.BadRequest) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message, string errorCode = "BUSINESS_RULE_VIOLATION")
        : base(message, errorCode, HttpStatusCode.BadRequest)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found", "ENTITY_NOT_FOUND", HttpStatusCode.NotFound)
    {
    }
}

/// <summary>
/// Exception thrown when attempting to create a duplicate entity
/// </summary>
public class DuplicateEntityException : DomainException
{
    public DuplicateEntityException(string entityName, string property, object value)
        : base($"{entityName} with {property} '{value}' already exists", "DUPLICATE_ENTITY", HttpStatusCode.Conflict)
    {
    }
}

/// <summary>
/// Exception thrown when domain invariants are violated
/// </summary>
public class DomainInvariantException : DomainException
{
    public DomainInvariantException(string message, string errorCode = "DOMAIN_INVARIANT_VIOLATION")
        : base(message, errorCode, HttpStatusCode.BadRequest)
    {
    }
}