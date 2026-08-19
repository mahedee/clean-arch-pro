# Project Description

**EduTrack** is a comprehensive educational tracking system designed to manage students, courses, enrollments, grades, and related academic data. The project follows **Clean Architecture** principles with clear separation of concerns across multiple layers.

### Key Features
- Student and course management
- Enrollment tracking and grade recording
- CQRS pattern implementation for command/query separation
- RESTful API with OpenAPI/Swagger documentation
- Angular-based frontend for user interaction
- Comprehensive unit and integration testing

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core Web API, Entity Framework Core
- **Database**: PostgreSQL
- **Frontend**: Angular, TypeScript
- **Testing**: xUnit, Moq
- **Architecture**: Clean Architecture, CQRS with MediatR
- **Validation**: FluentValidation
- **Mapping**: AutoMapper

## Project Structure

```
edu-track/
├── backend/EduTrack/
│   ├── src/
│   │   ├── EduTrack.Api/              # ASP.NET Core Web API layer
│   │   │   ├── Controllers/           # API endpoint controllers
│   │   │   ├── Middleware/            # Custom middleware (error handling, etc.)
│   │   │   └── Program.cs             # Application entry point and DI configuration
│   │   ├── EduTrack.Application/      # Business logic and use cases
│   │   │   ├── Commands/              # CQRS command handlers
│   │   │   ├── Queries/               # CQRS query handlers
│   │   │   ├── DTOs/                  # Data Transfer Objects
│   │   │   ├── Validators/            # FluentValidation validators
│   │   │   └── Mappings/              # AutoMapper profiles
│   │   ├── EduTrack.Domain/           # Core domain entities and interfaces
│   │   │   ├── Entities/              # Domain entities (Student, Course, etc.)
│   │   │   ├── Interfaces/            # Repository and service interfaces
│   │   │   └── Common/                # Base entity classes and domain logic
│   │   └── EduTrack.Infrastructure/   # External concerns and implementations
│   │       ├── Data/                  # EF Core DbContext and configurations
│   │       ├── Repositories/          # Repository implementations
│   │       └── Services/              # External service implementations
│   └── tests/
│       ├── EduTrack.Api.IntegrationTests/    # API integration tests
│       ├── EduTrack.Application.UnitTests/   # Application layer unit tests
│       ├── EduTrack.Domain.UnitTests/        # Domain layer unit tests
│       └── EduTrack.Infrastructure.UnitTests/# Infrastructure unit tests
├── frontend/edutrack-ui/              # Angular frontend application
│   ├── src/app/
│   │   ├── components/                # Angular components
│   │   ├── services/                  # HTTP services
│   │   └── models/                    # TypeScript models/interfaces
├── docs/                              # Project documentation
├── scripts/                           # PowerShell and shell scripts for automation
└── .github/                           # GitHub workflows and instructions
```

### Layer Dependencies (Inward Flow)
- **Api** → **Application** → **Domain** ← **Infrastructure**
- Changes flow inward; outer layers depend on inner layers
- Domain has no external dependencies (pure business logic)
- Infrastructure implements interfaces defined in Domain

## Project Guidelines

### Architecture Principles

- Follow **Clean Architecture** with clear layer separation: `Domain`, `Application`, `Infrastructure`, `Api`
- **Domain layer** contains entities, value objects, domain events, and interfaces (no dependencies)
- **Application layer** contains business logic, CQRS handlers, DTOs, validators, and mappings
- **Infrastructure layer** implements persistence, external services, and data access
- **Api layer** handles HTTP concerns, routing, middleware, and dependency injection setup

### CQRS Pattern with MediatR

- **Always** implement CQRS using **MediatR** for all business operations
- Create separate **Commands** for write operations (Create, Update, Delete)
  - Example: `CreateStudentCommand`, `UpdateStudentCommand`
- Create separate **Queries** for read operations (Get, List, Search)
  - Example: `GetStudentByIdQuery`, `GetAllStudentsQuery`
- Each command/query must have a dedicated **Handler**
  - Example: `CreateStudentCommandHandler`, `GetStudentByIdQueryHandler`
- Commands return `Result<T>` or `Result` for operation status
- Queries return `Result<TDto>` or `Result<List<TDto>>`

### Data Access Pattern

- **Never** use `DbContext` directly in Application layer handlers
- Always use **Repository pattern** via interfaces defined in Domain layer
- Implement **Unit of Work** pattern for transaction management
- Repositories should be accessed through `IUnitOfWork`
- Example pattern in handler:
  ```csharp
  var student = await _unitOfWork.Students.GetByIdAsync(id);
  _unitOfWork.Students.Add(newStudent);
  await _unitOfWork.SaveChangesAsync();
  ```

### Validation and Mapping

- Use **FluentValidation** for all input validation
- Every Command and Query must have a corresponding `Validator` class
  - Example: `CreateStudentCommandValidator` for `CreateStudentCommand`
- Place validators in `Application/Validators/` directory
- Use **AutoMapper** for all Entity ↔ DTO conversions
- Define mapping profiles in `Application/Mappings/` directory
- Never manually map properties in handlers; inject and use `IMapper`

### Naming Conventions

| Artifact | Convention | Example |
|----------|-----------|---------|
| **Entity** | Singular PascalCase | `Student`, `Course`, `Enrollment` |
| **DTO** | Suffix with `Dto` | `StudentDto`, `CourseDto` |
| **Interface** | Prefix with `I` | `IStudentRepository`, `IUnitOfWork` |
| **Command** | Verb + Entity + `Command` | `CreateStudentCommand`, `UpdateCourseCommand` |
| **Query** | Get + Entity + `Query` | `GetStudentByIdQuery`, `GetAllCoursesQuery` |
| **Handler** | Command/Query name + `Handler` | `CreateStudentCommandHandler` |
| **Validator** | Command/Query name + `Validator` | `CreateStudentCommandValidator` |
| **Repository** | Entity + `Repository` | `StudentRepository`, `CourseRepository` |
| **Controller** | Plural Entity + `Controller` | `StudentsController`, `CoursesController` |

### Error Handling and Logging

- Use **structured logging** via `ILogger<T>` throughout the application
- Never use `Console.WriteLine()` or string interpolation in log messages
- Use log templates with parameters: `_logger.LogInformation("Creating student with Id: {StudentId}", studentId);`
- Implement global exception handling middleware in Api layer
- Return appropriate HTTP status codes from API endpoints
- Use `Result<T>` pattern for operation outcomes in Application layer

### Testing Standards

- Write **unit tests** for all layers using **xUnit** and **Moq**
- Test projects should mirror the source structure
- Minimum test coverage: 80% for Application and Domain layers
- Integration tests should use in-memory database or test containers
- Test naming convention: `MethodName_Scenario_ExpectedBehavior`
  - Example: `CreateStudent_ValidInput_ReturnsSuccess`
- Mock all external dependencies in unit tests
- Use **FluentAssertions** for readable test assertions

### API Development

- All controllers should inherit from `ControllerBase` (not `Controller`)
- Use `[ApiController]` attribute on all controllers
- Define proper HTTP verbs: `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]`
- Use route patterns: `[Route("api/[controller]")]`
- Document endpoints with XML comments for Swagger generation
- Always validate model state; use `[FromBody]`, `[FromRoute]`, `[FromQuery]` attributes
- Return `IActionResult` with proper status codes:
  - `Ok(200)` for successful reads
  - `Created(201)` for successful creates
  - `NoContent(204)` for successful updates/deletes
  - `BadRequest(400)` for validation failures
  - `NotFound(404)` for missing resources

### Code Organization Best Practices

- One class per file; file name matches class name
- Keep methods focused and under 50 lines when possible
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Group related functionality into feature folders when appropriate
- Follow SOLID principles
- Avoid circular dependencies between layers
- Keep constructors simple; use dependency injection

### Version Control and Documentation

- Update requirements and documentation as features are added
- Keep CHANGELOG.md updated with notable changes
- Document breaking changes clearly
- Create descriptive commit messages following conventional commits
- Update API documentation when endpoints change
