# Validation Pipeline in Clean Architecture

## What is a Validation Pipeline?

A **Validation Pipeline** is a design pattern that automatically validates incoming requests before they reach the business logic handlers. It acts as a middleware layer in the request processing pipeline, ensuring that all data validation rules are applied consistently and automatically across the entire application.

In our Clean Architecture implementation, the Validation Pipeline uses **MediatR's Pipeline Behavior** feature combined with **FluentValidation** to create a centralized, automated validation system.

## Key Benefits of Validation Pipeline

### 1. **Separation of Concerns**
- **Business Logic Isolation**: Handlers focus purely on business logic, not validation
- **Single Responsibility**: Each validator class handles validation for one specific command/query
- **Clean Code**: Removes validation clutter from business logic

### 2. **Automatic Validation**
- **Zero Configuration**: Once set up, validation happens automatically for all requests
- **No Forgotten Validations**: Impossible to bypass validation accidentally
- **Consistent Behavior**: Same validation flow for all commands and queries

### 3. **Centralized Error Handling**
- **Uniform Error Responses**: Consistent error format across the entire API
- **Global Exception Handling**: Single point to handle all validation exceptions
- **Standardized HTTP Status Codes**: Automatic 400 Bad Request for validation failures

### 4. **Maintainability**
- **Easy to Update**: Validation rules are in dedicated classes
- **Testable**: Each validator can be unit tested independently
- **Discoverable**: Easy to find and understand validation rules

### 5. **Performance**
- **Early Validation**: Invalid requests are rejected before expensive operations
- **Async Support**: Non-blocking validation execution
- **Parallel Validation**: Multiple validators can run concurrently

## How It's Implemented

### 1. **ValidationBehavior Implementation**

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // If no validators exist, continue to handler
        if (!_validators.Any())
        {
            return await next();
        }

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Run all validators in parallel
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Throw exception if validation fails
        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        // Continue to next behavior/handler if validation passes
        return await next();
    }
}
```

### 2. **FluentValidation Validators**

```csharp
public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required")
            .MaximumLength(200).WithMessage("Course title cannot exceed 200 characters")
            .MinimumLength(3).WithMessage("Course title must be at least 3 characters");

        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course code is required")
            .Matches(@"^[A-Z]{2,4}\d{3,4}$")
            .WithMessage("Course code must be in format like 'CS101' or 'MATH2010'");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than 0")
            .LessThanOrEqualTo(12).WithMessage("Credits cannot exceed 12");
    }
}
```

### 3. **Dependency Injection Setup**

```csharp
public static IServiceCollection AddApplication(this IServiceCollection services)
{
    // Register MediatR with validation pipeline behavior
    services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        // Add validation behavior to the pipeline
        cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    });

    // Register all FluentValidation validators
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

    return services;
}
```

### 4. **Global Exception Handler**

```csharp
public class GlobalExceptionHandlerMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = 400; // Bad Request
        
        var response = new
        {
            Title = "Validation Failed",
            Status = 400,
            Errors = ex.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Simple Example: Course Creation Flow

### 1. **Request Flow**
```
HTTP POST /api/courses
{
  "title": "",           // Invalid: Empty title
  "courseCode": "cs101", // Invalid: Wrong format
  "credits": 15          // Invalid: Too many credits
}
```

### 2. **Validation Pipeline Execution**

```
Controller → MediatR → ValidationBehavior → CreateCourseCommandValidator
                              ↓
                    ValidationException thrown
                              ↓
            GlobalExceptionHandlerMiddleware
                              ↓
                    HTTP 400 Response
```

### 3. **Automatic Response**
```json
{
  "title": "Validation Failed",
  "status": 400,
  "errors": {
    "Title": ["Course title is required"],
    "CourseCode": ["Course code must be in format like 'CS101' or 'MATH2010'"],
    "Credits": ["Credits cannot exceed 12"]
  }
}
```

### 4. **Success Flow**
```
HTTP POST /api/courses
{
  "title": "Introduction to Computer Science",
  "courseCode": "CS101",
  "credits": 3,
  "description": "Foundational computer science concepts",
  "department": "Computer Science",
  "level": "Undergraduate"
}
```

**Flow**: Controller → MediatR → ValidationBehavior ✅ → CreateCourseCommandHandler → Success Response

## Pipeline Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Controller    │───▶│   MediatR       │───▶│ ValidationBehavior│───▶│ Command Handler │
│                 │    │                 │    │                 │    │                 │
│ Receives HTTP   │    │ Dispatches      │    │ Runs Validators │    │ Executes        │
│ Request         │    │ Commands/       │    │                 │    │ Business Logic  │
│                 │    │ Queries         │    │ Throws Exception│    │                 │
└─────────────────┘    └─────────────────┘    │ if Invalid      │    └─────────────────┘
                                              └─────────────────┘
                                                       │
                                                       ▼
                                              ┌─────────────────┐
                                              │ Global Exception│
                                              │ Handler         │
                                              │                 │
                                              │ Returns HTTP    │
                                              │ 400 with Errors │
                                              └─────────────────┘
```

## Advanced Features

### 1. **Custom Validation Rules**
```csharp
RuleFor(x => x.Email)
    .Must(BeUniqueEmail)
    .WithMessage("Email address is already in use");

private bool BeUniqueEmail(string email)
{
    // Custom validation logic
    return !_userRepository.ExistsByEmail(email);
}
```

### 2. **Conditional Validation**
```csharp
RuleFor(x => x.Prerequisites)
    .NotEmpty()
    .When(x => x.Level == "Graduate")
    .WithMessage("Graduate courses must have prerequisites");
```

### 3. **Complex Object Validation**
```csharp
RuleFor(x => x.Schedule)
    .SetValidator(new CourseScheduleValidator())
    .When(x => x.Schedule != null);
```

### 4. **Async Validation**
```csharp
RuleFor(x => x.TeacherId)
    .MustAsync(async (id, cancellation) => await TeacherExistsAsync(id))
    .WithMessage("Teacher not found");
```

## Testing the Validation Pipeline

### 1. **Unit Testing Validators**
```csharp
[Fact]
public void Should_Have_Error_When_Title_Is_Empty()
{
    // Arrange
    var validator = new CreateCourseCommandValidator();
    var command = new CreateCourseCommand { Title = "" };

    // Act
    var result = validator.TestValidate(command);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title)
          .WithErrorMessage("Course title is required");
}
```

### 2. **Integration Testing**
```csharp
[Fact]
public async Task CreateCourse_WithInvalidData_Returns400()
{
    // Arrange
    var invalidRequest = new CreateCourseDto { Title = "" };

    // Act
    var response = await _client.PostAsJsonAsync("/api/courses", invalidRequest);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

## Best Practices

### 1. **Validator Organization**
- One validator per command/query
- Place validators in the same namespace as commands
- Use descriptive validation error messages

### 2. **Error Message Guidelines**
- Be specific and actionable
- Include field names in messages
- Provide guidance on how to fix the error

### 3. **Performance Considerations**
- Use async validation for database lookups
- Keep validation logic lightweight
- Consider caching for expensive validations

### 4. **Security**
- Validate all user input
- Sanitize input to prevent injection attacks
- Don't rely solely on client-side validation

## Conclusion

The Validation Pipeline provides a robust, maintainable, and consistent approach to request validation in Clean Architecture applications. It ensures that:

- ✅ **All requests are validated automatically**
- ✅ **Business logic remains clean and focused**
- ✅ **Error handling is consistent across the application**
- ✅ **Validation rules are easily testable and maintainable**
- ✅ **Security is enhanced through comprehensive input validation**

This pattern is essential for building reliable, secure, and maintainable enterprise applications.
