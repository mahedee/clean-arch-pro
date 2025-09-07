# CreateCourseCommandValidator - Complete Validation Flow Explanation

## 🔍 How CreateCourseCommandValidator Works

The `CreateCourseCommandValidator` is integrated into the MediatR pipeline through FluentValidation, providing automatic validation of commands before they reach the handlers.

## 📋 Complete Flow Explanation

### 1. **Registration & Dependency Injection**

When the application starts, the validation system is registered in `ServiceRegistration.cs`:

```csharp
// Register all validators from the assembly
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Register the validation pipeline behavior
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

This automatically discovers and registers `CreateCourseCommandValidator` because it implements `AbstractValidator<CreateCourseCommand>`.

### 2. **MediatR Pipeline Flow**

When a client sends a `CreateCourseCommand`:

```
HTTP Request → Controller → MediatR.Send() → ValidationBehavior → CommandHandler
```

**Step-by-step breakdown:**

1. **Client Request**:
```json
POST /api/courses
{
  "title": "A",  // ❌ Too short (min 3 chars)
  "description": "Short",  // ❌ Too short (min 10 chars)
  "courseCode": "invalid",  // ❌ Wrong format
  "credits": 15,  // ❌ Exceeds max (12)
  "maxCapacity": 0,  // ❌ Must be > 0
  "department": "",  // ❌ Required
  "level": "InvalidLevel"  // ❌ Not in allowed values
}
```

2. **Controller receives request**:
```csharp
[HttpPost]
public async Task<ActionResult<Guid>> CreateCourse([FromBody] CreateCourseCommand command)
{
    // MediatR Send triggers the validation pipeline
    var courseId = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetCourse), new { id = courseId }, courseId);
}
```

3. **ValidationBehavior executes BEFORE handler**:
```csharp
public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, ...)
{
    // Finds CreateCourseCommandValidator for CreateCourseCommand
    var validators = _validators; // Contains CreateCourseCommandValidator
    
    // Runs validation
    var validationResults = await Task.WhenAll(
        validators.Select(v => v.ValidateAsync(context, cancellationToken)));
    
    // Collects failures
    var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null);
    
    if (failures.Any())
    {
        // ❌ STOPS HERE - Throws ValidationException
        throw new ValidationException(failures);
    }
    
    // ✅ If validation passes, continues to CreateCourseCommandHandler
    return await next();
}
```

### 3. **Validation Rules Execution**

The `CreateCourseCommandValidator` runs these rules:

```csharp
public CreateCourseCommandValidator()
{
    RuleFor(x => x.Title)
        .NotEmpty().WithMessage("Course title is required")
        .MaximumLength(200).WithMessage("Course title cannot exceed 200 characters")
        .MinimumLength(3).WithMessage("Course title must be at least 3 characters");

    RuleFor(x => x.Description)
        .NotEmpty().WithMessage("Course description is required")
        .MaximumLength(2000).WithMessage("Course description cannot exceed 2000 characters")
        .MinimumLength(10).WithMessage("Course description must be at least 10 characters");

    RuleFor(x => x.CourseCode)
        .NotEmpty().WithMessage("Course code is required")
        .MaximumLength(20).WithMessage("Course code cannot exceed 20 characters")
        .Matches(@"^[A-Z]{2,4}\d{3,4}$").WithMessage("Course code must be in format like 'CS101' or 'MATH2010'");

    RuleFor(x => x.Credits)
        .GreaterThan(0).WithMessage("Credits must be greater than 0")
        .LessThanOrEqualTo(12).WithMessage("Credits cannot exceed 12");

    RuleFor(x => x.MaxCapacity)
        .GreaterThan(0).WithMessage("Max capacity must be greater than 0")
        .LessThanOrEqualTo(500).WithMessage("Max capacity cannot exceed 500 students");

    RuleFor(x => x.Department)
        .NotEmpty().WithMessage("Department is required")
        .MaximumLength(100).WithMessage("Department name cannot exceed 100 characters");

    RuleFor(x => x.Level)
        .NotEmpty().WithMessage("Course level is required")
        .Must(level => new[] { "Undergraduate", "Graduate", "Doctorate", "Certificate" }.Contains(level))
        .WithMessage("Course level must be one of: Undergraduate, Graduate, Doctorate, Certificate");

    RuleFor(x => x.Prerequisites)
        .Must(prerequisites => prerequisites == null || prerequisites.Count <= 10)
        .WithMessage("A course cannot have more than 10 prerequisites");
}
```

### 4. **Error Response Generation**

When validation fails, the `GlobalExceptionHandlerMiddleware` catches the `ValidationException` and converts it to a structured HTTP response:

```json
{
  "message": "Validation failed",
  "details": "One or more validation errors occurred",
  "errors": [
    {
      "property": "Title",
      "message": "Course title must be at least 3 characters",
      "attemptedValue": "A",
      "code": "MinimumLengthValidator"
    },
    {
      "property": "Description", 
      "message": "Course description must be at least 10 characters",
      "attemptedValue": "Short",
      "code": "MinimumLengthValidator"
    },
    {
      "property": "CourseCode",
      "message": "Course code must be in format like 'CS101' or 'MATH2010'",
      "attemptedValue": "invalid",
      "code": "RegularExpressionValidator"
    },
    {
      "property": "Credits",
      "message": "Credits cannot exceed 12",
      "attemptedValue": "15",
      "code": "LessThanOrEqualValidator"
    },
    {
      "property": "MaxCapacity",
      "message": "Max capacity must be greater than 0",
      "attemptedValue": "0",
      "code": "GreaterThanValidator"
    },
    {
      "property": "Department",
      "message": "Department is required",
      "attemptedValue": "",
      "code": "NotEmptyValidator"
    },
    {
      "property": "Level",
      "message": "Course level must be one of: Undergraduate, Graduate, Doctorate, Certificate",
      "attemptedValue": "InvalidLevel",
      "code": "PredicateValidator"
    }
  ],
  "timestamp": "2025-09-06T10:30:00.000Z"
}
```

**HTTP Status**: `400 Bad Request`

### 5. **Success Flow**

With valid data:

```json
POST /api/courses
{
  "title": "Introduction to Computer Science",
  "description": "A comprehensive introduction to programming and computer science fundamentals",
  "courseCode": "CS101",
  "credits": 3,
  "maxCapacity": 30,
  "department": "Computer Science",
  "level": "Undergraduate",
  "academicPeriod": "Fall 2025"
}
```

**Flow**:
1. ✅ ValidationBehavior runs all rules → All pass
2. ✅ CreateCourseCommandHandler executes
3. ✅ Course entity created and saved
4. ✅ HTTP 201 Created response with course ID

```json
HTTP 201 Created
Location: /api/courses/550e8400-e29b-41d4-a716-446655440000
{
  "id": "550e8400-e29b-41d4-a716-446655440000"
}
```

## 🔧 Key Integration Points

### **Automatic Discovery**
- FluentValidation automatically finds validators by interface: `IValidator<CreateCourseCommand>`
- No manual registration required for each validator

### **Pipeline Integration**
- `ValidationBehavior<TRequest, TResponse>` is a MediatR pipeline behavior
- Executes BEFORE the actual command handler
- Short-circuits the pipeline if validation fails

### **Consistent Error Handling**
- `GlobalExceptionHandlerMiddleware` provides consistent error responses
- Structured validation errors with property names and codes
- Proper HTTP status codes (400 for validation, 500 for server errors)

### **Developer Experience**
- Clear, descriptive error messages
- Property-level error mapping for frontend validation display
- Attempted values included for debugging

This validation system ensures data integrity at the API boundary before any business logic executes, providing robust input validation with detailed error feedback.
