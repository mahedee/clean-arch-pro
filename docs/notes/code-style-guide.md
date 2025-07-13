# EduTrack Code Style Guide

## 📋 **Overview**
This document defines the coding standards and style guidelines for the EduTrack Clean Architecture project. These rules are automatically enforced by our `.editorconfig` file and should be followed by all team members.

---

## 🎯 **EditorConfig Configuration**

Our project uses EditorConfig to maintain consistent formatting across all IDEs and editors. The `.editorconfig` file is located in the project root and automatically enforced.

### **Key Settings:**
- **Character Encoding**: UTF-8 for all files
- **Line Endings**: CRLF (Windows standard)
- **Final Newline**: All files end with a newline
- **Trailing Whitespace**: Automatically trimmed

---

## 🔧 **Language-Specific Rules**

### **C# Files (.cs)**
```csharp
// ✅ Correct formatting (4 spaces, proper bracing)
namespace EduTrack.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name cannot be empty");
                
            FullName = newName;
        }
    }
}
```

**Rules:**
- **Indentation**: 4 spaces (no tabs)
- **Braces**: New line for classes, methods, properties
- **Naming**: PascalCase for public members, camelCase for private fields
- **Access Modifiers**: Always explicit (public, private, etc.)

### **JSON Files (.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=EduTrackDb"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

**Rules:**
- **Indentation**: 2 spaces
- **Property Names**: Use consistent casing
- **No trailing commas**: Avoid trailing commas for compatibility

### **YAML Files (.yml, .yaml)**
```yaml
version: '3.8'
services:
  edutrack-api:
    image: edutrack/api:latest
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
```

**Rules:**
- **Indentation**: 2 spaces
- **No tabs**: Only spaces allowed
- **Consistent structure**: Align related items

### **XML/Project Files (.csproj, .xml)**
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.4" />
  </ItemGroup>
</Project>
```

**Rules:**
- **Indentation**: 2 spaces
- **Self-closing tags**: Use when appropriate
- **Attribute formatting**: One per line for readability

---

## 🏗️ **Architecture-Specific Guidelines**

### **Clean Architecture Layers**

#### **Domain Layer (EduTrack.Domain)**
```csharp
// ✅ Rich domain model with behavior
public class Student
{
    private readonly List<Course> _enrolledCourses = new();
    
    public Guid Id { get; private set; }
    public FullName FullName { get; private set; }
    public Email Email { get; private set; }
    public IReadOnlyList<Course> EnrolledCourses => _enrolledCourses.AsReadOnly();
    
    public void EnrollInCourse(Course course)
    {
        if (_enrolledCourses.Contains(course))
            throw new DomainException("Student already enrolled in this course");
            
        _enrolledCourses.Add(course);
        // Raise domain event
        AddDomainEvent(new StudentEnrolledEvent(Id, course.Id));
    }
}
```

**Guidelines:**
- Rich domain models with business logic
- Private setters for properties
- Domain events for important business actions
- Value objects for complex types (Email, FullName)

#### **Application Layer (EduTrack.Application)**
```csharp
// ✅ Command/Query pattern with MediatR
public class CreateStudentCommand : IRequest<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
}

public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateStudentCommandHandler(
        IStudentRepository studentRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
    {
        var student = new Student(
            Guid.NewGuid(),
            new FullName(request.FullName),
            new Email(request.Email),
            request.DateOfBirth);
            
        await _studentRepository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return student.Id;
    }
}
```

**Guidelines:**
- CQRS pattern with Commands and Queries
- One handler per command/query
- Async operations with CancellationToken
- No direct database access (use repositories)

#### **Infrastructure Layer (EduTrack.Infrastructure)**
```csharp
// ✅ Repository implementation
public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;
    
    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.EnrolledCourses)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task AddAsync(Student student)
    {
        await _context.Students.AddAsync(student);
    }
}
```

**Guidelines:**
- Implement domain interfaces
- Use Entity Framework features appropriately
- Handle null cases properly
- Include related data when needed

#### **API Layer (EduTrack.Api)**
```csharp
// ✅ Clean controller with proper HTTP verbs
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateStudent(CreateStudentRequest request)
    {
        var command = new CreateStudentCommand
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth
        };
        
        var studentId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetStudent), new { id = studentId }, studentId);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponse>> GetStudent(Guid id)
    {
        var query = new GetStudentByIdQuery(id);
        var student = await _mediator.Send(query);
        
        return student != null ? Ok(student) : NotFound();
    }
}
```

**Guidelines:**
- Use MediatR for command/query dispatch
- Proper HTTP status codes
- RESTful endpoints
- Input validation with data annotations

---

## 🧪 **Testing Guidelines**

### **Unit Tests**
```csharp
// ✅ Well-structured unit test
public class StudentTests
{
    [Fact]
    public void EnrollInCourse_WhenValidCourse_ShouldAddToEnrolledCourses()
    {
        // Arrange
        var student = new Student(Guid.NewGuid(), "John Doe", "john@test.com");
        var course = new Course(Guid.NewGuid(), "Mathematics 101");
        
        // Act
        student.EnrollInCourse(course);
        
        // Assert
        student.EnrolledCourses.Should().Contain(course);
    }
    
    [Fact]
    public void EnrollInCourse_WhenAlreadyEnrolled_ShouldThrowDomainException()
    {
        // Arrange
        var student = new Student(Guid.NewGuid(), "John Doe", "john@test.com");
        var course = new Course(Guid.NewGuid(), "Mathematics 101");
        student.EnrollInCourse(course);
        
        // Act & Assert
        student.Invoking(s => s.EnrollInCourse(course))
            .Should().Throw<DomainException>()
            .WithMessage("Student already enrolled in this course");
    }
}
```

**Guidelines:**
- **Arrange-Act-Assert** pattern
- **Descriptive test names** that explain the scenario
- **FluentAssertions** for readable assertions
- **One assertion per test** when possible

---

## 📝 **Naming Conventions**

### **Files and Folders**
```
✅ Good:
- StudentController.cs
- CreateStudentCommand.cs
- IStudentRepository.cs
- Student.cs

❌ Avoid:
- studentcontroller.cs
- create_student_command.cs
- IStudentRepo.cs
- student.cs
```

### **C# Naming Rules**
```csharp
// ✅ Correct naming conventions
public class StudentController                    // PascalCase for classes
{
    private readonly IStudentRepository _repository;  // camelCase with underscore for private fields
    
    public async Task<Student> GetStudentAsync(Guid id)  // PascalCase for methods
    {
        var foundStudent = await _repository.GetByIdAsync(id);  // camelCase for local variables
        return foundStudent;
    }
}

// ✅ Interface naming
public interface IStudentRepository  // 'I' prefix for interfaces
{
    Task<Student> GetByIdAsync(Guid id);
}

// ✅ Constants
public static class StudentConstants
{
    public const int MAX_NAME_LENGTH = 100;  // UPPER_CASE for constants
    public const string DEFAULT_EMAIL_DOMAIN = "@school.edu";
}
```

---

## 🔍 **Code Quality Rules**

### **General Principles**
1. **Single Responsibility**: Each class/method should have one reason to change
2. **Open/Closed**: Open for extension, closed for modification
3. **Dependency Inversion**: Depend on abstractions, not concretions
4. **DRY**: Don't Repeat Yourself
5. **KISS**: Keep It Simple, Stupid

### **Error Handling**
```csharp
// ✅ Proper exception handling
public async Task<Student> GetStudentAsync(Guid id)
{
    if (id == Guid.Empty)
        throw new ArgumentException("Student ID cannot be empty", nameof(id));
        
    var student = await _repository.GetByIdAsync(id);
    if (student == null)
        throw new NotFoundException($"Student with ID {id} not found");
        
    return student;
}
```

### **Async/Await Best Practices**
```csharp
// ✅ Correct async usage
public async Task<List<Student>> GetActiveStudentsAsync()
{
    return await _context.Students
        .Where(s => s.IsActive)
        .ToListAsync();
}

// ❌ Avoid blocking calls
public List<Student> GetActiveStudents()
{
    return _context.Students
        .Where(s => s.IsActive)
        .ToList();  // Blocking call in async context
}
```

---

## 🛠️ **IDE Configuration**

### **Visual Studio Code**
Add to `.vscode/settings.json`:
```json
{
  "editor.formatOnSave": true,
  "editor.formatOnPaste": true,
  "editor.insertSpaces": true,
  "editor.tabSize": 4,
  "files.trimTrailingWhitespace": true,
  "files.insertFinalNewline": true,
  "editorconfig.generateAutoIndentation": true,
  "omnisharp.enableEditorConfigSupport": true
}
```

### **Visual Studio 2022**
- EditorConfig support is built-in
- Go to Tools → Options → Text Editor → C# → Code Style
- Ensure "Enable EditorConfig support" is checked

---

## ✅ **Pre-commit Checklist**

Before committing code, ensure:

- [ ] Code builds without warnings
- [ ] All unit tests pass
- [ ] EditorConfig rules are applied
- [ ] No trailing whitespace
- [ ] Proper file encoding (UTF-8)
- [ ] Consistent indentation
- [ ] Descriptive commit messages
- [ ] No hardcoded values or secrets

---

## 🔧 **Tools Integration**

### **Recommended Extensions**
- **EditorConfig for VS Code**: Automatic formatting
- **C# Extensions**: IntelliSense and debugging
- **SonarLint**: Code quality analysis
- **GitLens**: Git integration and history

### **Automated Checks**
Our CI/CD pipeline includes:
- Code formatting validation
- Static code analysis
- Test coverage reporting
- Security vulnerability scanning

---

## 📚 **Resources**

- [EditorConfig Official Documentation](https://editorconfig.org/)
- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)

---

*This style guide is enforced by our `.editorconfig` file and should be reviewed regularly to ensure it meets the team's evolving needs.*

**Last Updated**: July 13, 2025  
**Version**: 1.0  
**Project**: EduTrack Clean Architecture
