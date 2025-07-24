# 🏗️ Clean Architecture Overview

## Introduction

This document provides a comprehensive overview of how Clean Architecture is implemented in the EduTrack project, following Uncle Bob's Clean Architecture principles.

## Architecture Layers

### 1. Domain Layer (Core)
**Location**: `src/EduTrack.Domain/`
**Purpose**: Contains the business logic and rules that are independent of any external concerns.

#### Components:
- **Entities**: Core business objects with identity
- **Value Objects**: Objects without identity that represent descriptive aspects
- **Domain Services**: Domain logic that doesn't naturally fit into entities
- **Repository Interfaces**: Contracts for data access
- **Domain Events**: Events that represent something important that happened in the domain

#### Dependencies:
- **None** - This layer has no dependencies on other layers

### 2. Application Layer
**Location**: `src/EduTrack.Application/`
**Purpose**: Orchestrates the flow of data to and from the entities, and directs those entities to use their business rules.

#### Components:
- **Use Cases**: Application-specific business rules
- **Commands & Queries**: CQRS implementation using MediatR
- **Command/Query Handlers**: Process commands and queries
- **DTOs**: Data transfer objects for external communication
- **Mapping Profiles**: AutoMapper configurations
- **Validators**: Input validation using FluentValidation

#### Dependencies:
- **Domain Layer** ✅

### 3. Infrastructure Layer
**Location**: `src/EduTrack.Infrastructure/`
**Purpose**: Provides implementations for external concerns like databases, file systems, web services, etc.

#### Components:
- **Repository Implementations**: Concrete implementations of domain repository interfaces
- **Database Context**: Entity Framework DbContext
- **External Services**: Email, SMS, file storage implementations
- **Configurations**: Entity type configurations
- **Migrations**: Database schema migrations

#### Dependencies:
- **Domain Layer** ✅
- **Application Layer** ❌ (Violates Clean Architecture)

### 4. Presentation Layer (API)
**Location**: `src/EduTrack.Api/`
**Purpose**: Handles HTTP requests and responses, user input validation, and presentation logic.

#### Components:
- **Controllers**: HTTP request handlers
- **Middleware**: Custom middleware components
- **Filters**: Action filters for cross-cutting concerns
- **DTOs**: Request/Response models
- **Configuration**: Application startup and dependency injection

#### Dependencies:
- **Application Layer** ✅
- **Infrastructure Layer** ✅ (For dependency injection only)

## Dependency Flow

```
┌─────────────────┐
│   Presentation  │
│     (API)       │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│   Application   │
│     Layer       │
└─────────┬───────┘
          │
          ▼
┌─────────────────┐
│     Domain      │
│      Layer      │
└─────────────────┘
          ▲
          │
┌─────────┴───────┐
│ Infrastructure  │
│     Layer       │
└─────────────────┘
```

## Benefits of This Architecture

### 1. **Independence of Frameworks**
The architecture doesn't depend on the existence of some library of feature laden software.

### 2. **Testability**
The business rules can be tested without the UI, Database, Web Server, or any other external element.

### 3. **Independence of UI**
The UI can change easily, without changing the rest of the system.

### 4. **Independence of Database**
You can swap out Oracle or SQL Server, for Mongo, BigTable, CouchDB, or something else.

### 5. **Independence of External Agency**
Your business rules simply don't know anything at all about the outside world.

## CQRS Implementation

The project implements Command Query Responsibility Segregation (CQRS) using MediatR:

### Commands (Write Operations)
```csharp
public class CreateStudentCommand : IRequest<StudentDto>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, StudentDto>
{
    // Implementation
}
```

### Queries (Read Operations)
```csharp
public class GetStudentByIdQuery : IRequest<StudentDto>
{
    public Guid StudentId { get; set; }
}

public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, StudentDto>
{
    // Implementation
}
```

## Domain-Driven Design (DDD) Patterns

### Aggregate Roots
```csharp
public class Student : AggregateRoot<Guid>
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    
    // Domain methods
    public void UpdateContactInformation(Email newEmail)
    {
        if (newEmail == null)
            throw new ArgumentNullException(nameof(newEmail));
            
        Email = newEmail;
        AddDomainEvent(new StudentContactUpdatedEvent(Id, newEmail));
    }
}
```

### Value Objects
```csharp
public class Email : ValueObject
{
    public string Value { get; }
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty");
            
        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format");
            
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

### Repository Pattern
```csharp
// Domain Layer - Interface
public interface IStudentRepository : IRepository<Student>
{
    Task<Student> GetByEmailAsync(Email email);
    Task<IEnumerable<Student>> GetByProgramAsync(Guid programId);
}

// Infrastructure Layer - Implementation
public class StudentRepository : Repository<Student>, IStudentRepository
{
    public async Task<Student> GetByEmailAsync(Email email)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Email.Value == email.Value);
    }
}
```

## Testing Strategy

### Unit Testing
- **Domain Layer**: Test business logic in isolation
- **Application Layer**: Test use cases with mocked dependencies
- **Infrastructure Layer**: Test data access with in-memory database

### Integration Testing
- **API Layer**: Test HTTP endpoints end-to-end
- **Database Integration**: Test with real database providers

### Example Unit Test
```csharp
[Fact]
public void UpdateContactInformation_ValidEmail_ShouldUpdateAndRaiseDomainEvent()
{
    // Arrange
    var student = Student.Create("John", "Doe", new Email("john@example.com"));
    var newEmail = new Email("john.doe@example.com");
    
    // Act
    student.UpdateContactInformation(newEmail);
    
    // Assert
    Assert.Equal(newEmail, student.Email);
    Assert.Single(student.DomainEvents);
    Assert.IsType<StudentContactUpdatedEvent>(student.DomainEvents.First());
}
```

## Best Practices

### 1. **Keep Domain Pure**
- No dependencies on external frameworks
- Business logic stays in the domain
- Use domain events for side effects

### 2. **Use Dependency Injection**
- Register services at the composition root
- Follow SOLID principles
- Use interfaces for abstraction

### 3. **Validate at Boundaries**
- Validate input at the API layer
- Use FluentValidation for complex rules
- Domain objects should always be in valid state

### 4. **Handle Errors Properly**
- Use domain exceptions for business rule violations
- Application exceptions for application concerns
- Global exception handling at API layer

### 5. **Maintain Consistency**
- Use Unit of Work pattern for transactions
- Domain events for eventual consistency
- Repository pattern for data access

## Next Steps

1. **Complete Domain Layer**: Implement remaining entities and value objects
2. **Add Authentication**: JWT-based authentication system
3. **Implement CQRS**: Complete command and query handlers
4. **Add Validation**: FluentValidation rules
5. **Testing**: Comprehensive test coverage

For more detailed information about specific patterns, see:
- [Domain Design Patterns](domain-patterns.md)
- [CQRS Implementation](cqrs-patterns.md)
- [Testing Strategy](../testing/testing-strategy.md)
