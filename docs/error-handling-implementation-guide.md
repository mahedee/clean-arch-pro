# Error Handling Implementation Guide

## Overview

This document describes the comprehensive error handling improvements implemented across all layers of the EduTrack application, following RFC 7807 Problem Details standard and modern error handling best practices.

## 🎯 Key Improvements Implemented

### 1. **API Layer Enhancements**

#### RFC 7807 Problem Details Support
- ✅ Standardized error responses using `ProblemDetailsResponse` class
- ✅ Proper HTTP status code mapping for different exception types
- ✅ Correlation ID support for request tracking across services
- ✅ Environment-specific error details (more verbose in development)

#### Enhanced Global Exception Middleware
- ✅ Structured logging with correlation IDs and request context
- ✅ Comprehensive exception type mapping
- ✅ Security-conscious error responses (no sensitive data in production)

**Example Error Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "One or more validation errors occurred",
  "instance": "/api/students",
  "traceId": "00-1234567890abcdef-fedcba0987654321-01",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000",
  "timestamp": "2026-02-23T10:30:00.123Z",
  "errorCode": "VALIDATION_ERROR",
  "errors": {
    "Email": ["Email format is invalid"],
    "Name": ["Name is required"]
  }
}
```

### 2. **Domain Layer Enhancements**

#### Custom Domain Exceptions
- ✅ `DomainException` - Base class with error codes and HTTP status mapping
- ✅ `BusinessRuleViolationException` - For business logic violations
- ✅ `EntityNotFoundException` - For entity lookup failures
- ✅ `DuplicateEntityException` - For uniqueness constraint violations
- ✅ `DomainInvariantException` - For invariant violations

#### Result Pattern Implementation
- ✅ `Result` and `Result<T>` classes for better error handling
- ✅ `ValidationResult` with multiple validation errors
- ✅ Implicit conversions for easier usage

**Usage Example:**
```csharp
public Result<Student> CreateStudent(string name, string email)
{
    if (string.IsNullOrEmpty(name))
        return Result.Failure<Student>("Name is required", "INVALID_NAME");
    
    var student = new Student(name, email);
    return Result.Success(student);
}
```

### 3. **Application Layer Enhancements**

#### Enhanced MediatR Pipeline Behaviors
- ✅ `LoggingBehavior` - Request/response logging with correlation tracking
- ✅ `PerformanceBehavior` - Performance monitoring and slow operation alerts
- ✅ Enhanced `ValidationBehavior` - Better error context and logging

#### Application-Specific Exceptions
- ✅ `ApplicationException` - Base application exception
- ✅ `NotFoundException` - Application-level not found errors
- ✅ `ValidationException` - Enhanced validation error handling
- ✅ `ForbiddenAccessException` - Authorization failures
- ✅ `ExternalServiceException` - External service failures

### 4. **Infrastructure Layer Enhancements**

#### Database Resilience
- ✅ Connection retry policies with exponential backoff
- ✅ PostgreSQL-specific error code mapping
- ✅ Database timeout handling
- ✅ Connection pool optimization

#### Retry Policies (Using Polly)
- ✅ Database retry policy with exponential backoff
- ✅ HTTP retry policy with jittered backoff
- ✅ Circuit breaker pattern for external services
- ✅ Bulkhead isolation for resource protection
- ✅ Timeout policies for operations

#### Infrastructure Exceptions
- ✅ `DatabaseConnectionException` - Database connectivity issues
- ✅ `DatabaseTimeoutException` - Database operation timeouts
- ✅ PostgreSQL error code mapping with proper categorization

### 5. **Structured Logging with Serilog**

#### Logging Configuration
- ✅ Structured JSON logging for production
- ✅ Console logging with colors for development
- ✅ File-based logging with rotation
- ✅ Seq integration support
- ✅ Correlation ID enrichment

#### Log Levels by Environment
- **Development**: Debug level with detailed EF Core logging
- **Production**: Information level with security-conscious filtering

### 6. **Health Checks**

- ✅ `/health` - Basic health endpoint
- ✅ `/health/ready` - Readiness check with database connectivity
- ✅ Entity Framework health checks

## 🚀 Configuration Examples

### appsettings.json
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    }
  },
  "Resilience": {
    "Database": {
      "MaxRetryCount": 3,
      "MaxRetryDelay": "00:00:30"
    },
    "Http": {
      "MaxRetryCount": 3,
      "CircuitBreakerThreshold": 5
    }
  }
}
```

## 🧪 Testing

### Unit Tests
- ✅ `GlobalExceptionHandlerMiddlewareTests` - Exception handling scenarios
- ✅ Validation behavior tests
- ✅ Custom exception tests

### Integration Tests
- ✅ End-to-end error handling flow
- ✅ Correlation ID propagation
- ✅ Health check validation
- ✅ Database error scenarios

## 📊 Monitoring & Observability

### Key Metrics Tracked
- Request correlation IDs for distributed tracing
- Performance metrics for slow operations (>500ms warning)
- Exception rates and types
- Database connection health

### Structured Logging Fields
```json
{
  "Timestamp": "2026-02-23T10:30:00.123Z",
  "Level": "Error",
  "MessageTemplate": "Request {RequestName} failed",
  "Properties": {
    "RequestName": "CreateStudentCommand",
    "CorrelationId": "550e8400...",
    "UserId": "user123",
    "ElapsedMs": 1234
  }
}
```

## 🔧 NuGet Packages Added

### Infrastructure Layer
- `Polly` (8.4.2) - Resilience patterns
- `Polly.Extensions.Http` (3.0.0) - HTTP resilience
- `Serilog.AspNetCore` (8.0.4) - Structured logging
- `Serilog.Exceptions` (8.4.0) - Exception enrichment
- Health check packages

### API Layer
- `Microsoft.AspNetCore.Diagnostics.HealthChecks` (2.2.0)
- Problem Details support (built-in .NET 9)

## 📈 Benefits Achieved

1. **Improved User Experience**
   - Consistent, informative error messages
   - Proper HTTP status codes
   - No sensitive information leakage

2. **Enhanced Debugging**
   - Correlation IDs for request tracking
   - Structured logging with context
   - Performance monitoring

3. **Better System Reliability**
   - Automatic retry policies
   - Circuit breaker protection
   - Database connection resilience

4. **Monitoring & Alerting**
   - Health check endpoints
   - Performance metrics
   - Error rate tracking

## 🛠️ Usage Guidelines

### Adding New Exception Types
1. Inherit from appropriate base class (`DomainException`, `ApplicationException`)
2. Define specific error codes
3. Map in `GlobalExceptionHandlerMiddleware`
4. Add unit tests

### Correlation ID Usage
- Automatically generated for each request
- Included in all log entries
- Returned in error responses
- Used for distributed tracing

### Performance Monitoring
- Operations >500ms generate warnings
- Database queries logged in development
- Slow operation alerts configured

## 🔍 Troubleshooting Common Issues

### Database Connection Failures
- Check PostgreSQL service status
- Verify connection string
- Review retry policy configuration
- Check network connectivity

### Validation Errors
- Review FluentValidation rules
- Check request payload format
- Verify model binding

### Performance Issues
- Review performance logs for slow operations
- Check database query efficiency
- Monitor resource usage

## 🎯 Next Steps & Recommendations

1. **Advanced Monitoring**
   - Integrate with APM tools (Application Performance Monitoring)
   - Set up alerting for error rates
   - Configure dashboards for key metrics

2. **Security Enhancements**
   - Implement rate limiting
   - Add request validation
   - Enhance audit logging

3. **Performance Optimization**
   - Database query optimization
   - Caching strategies
   - Response compression

4. **Documentation**
   - API error response documentation
   - Troubleshooting guides
   - Operational runbooks

This implementation provides a robust, scalable error handling system that improves both developer experience and application reliability.