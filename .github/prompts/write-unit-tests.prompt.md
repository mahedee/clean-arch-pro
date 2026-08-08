---
mode: agent
description: Write xUnit + Moq unit tests for a command handler, query handler, or domain entity in EduTrack
---

Write unit tests for **${input:target}** (e.g., `CreateStudentCommandHandler`, `GetStudentQueryHandler`, `Student` entity).

Place tests in the correct project under `backend/EduTrack/tests/`:
- Handler tests → `EduTrack.Application.UnitTests/Features/`
- Domain entity tests → `EduTrack.Domain.UnitTests/Entities/`
- Repository tests → `EduTrack.Infrastructure.UnitTests/Repositories/`

Follow the xUnit + Moq conventions used in this project.

## Rules

- Use **xUnit** (`[Fact]`, `[Theory]`, `[InlineData]`)
- Use **Moq** (`Mock<T>`, `.Setup()`, `.Returns()`, `.ReturnsAsync()`, `.Verify()`)
- **Never** use real `DbContext` or real repositories — mock `IUnitOfWork` and the specific repository interface
- Name tests using: `MethodName_StateUnderTest_ExpectedBehavior` (e.g., `Handle_ValidCommand_ReturnsNewStudentId`)
- Use `// Arrange / // Act / // Assert` comments
- One assertion concept per test (multiple `.Assert` calls are fine if testing the same concern)
- Prefer `Assert.Equal`, `Assert.NotNull`, `Assert.True` over `Assert.That`

## Handler test structure

```csharp
public class ${Handler}Tests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<I${Entity}Repository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ${Handler} _handler;

    public ${Handler}Tests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<I${Entity}Repository>();
        _mapperMock = new Mock<IMapper>();

        _unitOfWorkMock.Setup(u => u.${Entity}s).Returns(_repositoryMock.Object);

        _handler = new ${Handler}(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_Returns${ExpectedResult}()
    {
        // Arrange
        // Act
        // Assert
    }
}
```

## Required test cases for each handler type

### Command handlers (Create)
- Happy path: valid input → returns new `Guid`, `AddAsync` called once, `SaveChangesAsync` called once
- Duplicate detected: returns existing entity → throws `InvalidOperationException`
- Invalid input (if no validator): missing required fields → throws `ArgumentException` or `ValidationException`

### Command handlers (Update / Delete)
- Happy path: entity found → updated/deleted, `SaveChangesAsync` called once
- Entity not found: `GetByIdAsync` returns `null` → throws `NotFoundException` or equivalent

### Query handlers
- Entity found: returns mapped DTO
- Entity not found: returns `null` or throws `NotFoundException`
- List query: returns paged result with correct count

### Domain entity tests
- Factory method: valid args → entity created with correct state
- Factory method: invalid args → throws `ArgumentException` or `DomainException`
- State mutation methods: state changes correctly, domain events raised
