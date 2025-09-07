# EduTrack Testing Guide

## 📋 Overview
This guide provides comprehensive instructions for running unit tests and integration tests in the EduTrack project. The project follows Test-Driven Development (TDD) principles with excellent test coverage across all layers.

## 🏗️ Project Test Structure

```
backend/EduTrack/
├── tests/
│   ├── EduTrack.Domain.UnitTests/           # Domain entity unit tests
│   ├── EduTrack.Application.UnitTests/      # Application layer unit tests  
│   ├── EduTrack.Infrastructure.UnitTests/   # Infrastructure unit tests
│   └── EduTrack.Api.IntegrationTests/       # API integration tests
└── src/
    ├── EduTrack.Domain/                      # Domain entities & logic
    ├── EduTrack.Application/                 # CQRS handlers & DTOs
    ├── EduTrack.Infrastructure/              # Data access & services
    └── EduTrack.Api/                         # Web API controllers
```

## 🧪 Test Technologies Stack

- **Unit Testing Framework**: xUnit.net
- **Mocking Framework**: Moq
- **Assertion Library**: FluentAssertions
- **Integration Testing**: ASP.NET Core TestHost
- **Test Database**: Entity Framework InMemory Provider
- **Coverage Analysis**: Built-in .NET coverage tools

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code with C# extension
- PowerShell (for batch scripts)

### Navigate to Project Directory
```powershell
cd backend/EduTrack
```

## 📝 Unit Testing

### Running All Unit Tests
```powershell
# Run all unit tests across all projects
dotnet test --filter "Category!=Integration"

# Alternative: Run unit tests with detailed output
dotnet test --filter "Category!=Integration" --verbosity normal
```

### Running Specific Test Projects

#### 1. Domain Unit Tests (Entities & Value Objects)
```powershell
# Run all domain tests
dotnet test tests\EduTrack.Domain.UnitTests\

# Run specific test class
dotnet test tests\EduTrack.Domain.UnitTests\ --filter "CourseTests"

# Run specific test method
dotnet test tests\EduTrack.Domain.UnitTests\ --filter "CreateCourse_WithValidData_ShouldCreateCourse"
```

#### 2. Application Unit Tests (CQRS Handlers)
```powershell
# Run all application layer tests
dotnet test tests\EduTrack.Application.UnitTests\

# Run specific feature tests
dotnet test tests\EduTrack.Application.UnitTests\ --filter "CreateCourseCommandHandlerTests"

# Run with coverage analysis
dotnet test tests\EduTrack.Application.UnitTests\ --collect:"XPlat Code Coverage"
```

#### 3. Infrastructure Unit Tests
```powershell
# Run infrastructure tests
dotnet test tests\EduTrack.Infrastructure.UnitTests\

# Run repository tests specifically
dotnet test tests\EduTrack.Infrastructure.UnitTests\ --filter "RepositoryTests"
```

### Test Output Examples

#### Successful Test Run
```
✅ Test Run Successful.
Total tests: 325
     Passed: 325
     Failed: 0
     Skipped: 0
 Total time: 2.4567 Seconds
```

#### Failed Test Example
```
❌ Test Run Failed.
Total tests: 325
     Passed: 324
     Failed: 1
     Skipped: 0

Failed   CreateCourse_WithInvalidData_ShouldThrowException [2ms]
  Error Message:
   Expected exception of type ArgumentException but got InvalidOperationException
```

## 🔧 Integration Testing

### Running All Integration Tests
```powershell
# Run all integration tests
dotnet test tests\EduTrack.Api.IntegrationTests\

# Run with detailed logging
dotnet test tests\EduTrack.Api.IntegrationTests\ --verbosity normal

# Run specific test class
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "CoursesControllerIntegrationTests"
```

### Running Specific Integration Test Scenarios

#### 1. Course API Integration Tests
```powershell
# Run all course API tests
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "CoursesController"

# Run specific workflow test
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "CourseWorkflow_CreateScheduleActivateComplete_WorksCorrectly"

# Run CRUD operation tests
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "CreateCourse_WithValidData_ReturnsCreatedCourse"
```

#### 2. End-to-End Workflow Tests
```powershell
# Run complete business workflow tests
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "Category=Workflow"

# Run API endpoint tests
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "Category=API"
```

### Integration Test Features

#### Test Database Setup
- **In-Memory Database**: Each test gets a fresh database instance
- **Isolated Tests**: No test interference or shared state
- **Real HTTP Requests**: Tests actual API endpoints

#### Test Data Management
```csharp
// Example: Test automatically creates test data
[Fact]
public async Task CreateCourse_WithValidData_ReturnsCreatedCourse()
{
    // Arrange - Test data is automatically set up
    var createCourseDto = new CreateCourseDto
    {
        Title = "Introduction to Computer Science",
        CourseCode = "CS101",
        Description = "Fundamentals of programming and computer science concepts",
        CreditHours = 3,
        Department = "Computer Science",
        Level = CourseLevel.Undergraduate
    };

    // Act - Real HTTP request to API
    var response = await _client.PostAsJsonAsync("/api/courses", createCourseDto);

    // Assert - Verify response and data
    response.StatusCode.Should().Be(HttpStatusCode.Created);
}
```

## 📊 Advanced Testing Scenarios

### Running Tests with Code Coverage
```powershell
# Generate code coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

# View coverage in browser (requires reportgenerator tool)
reportgenerator -reports:"./TestResults/*/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html
```

### Running Tests in Parallel
```powershell
# Run tests in parallel for faster execution
dotnet test --parallel

# Control parallelization
dotnet test --parallel --max-degree-of-parallelism:4
```

### Running Tests with Filters

#### Category-Based Filtering
```powershell
# Run only fast tests
dotnet test --filter "Category=Fast"

# Run only domain tests
dotnet test --filter "Category=Domain"

# Exclude slow tests
dotnet test --filter "Category!=Slow"
```

#### Test Name Filtering
```powershell
# Run tests containing "Course" in name
dotnet test --filter "Course"

# Run tests with multiple criteria
dotnet test --filter "Course&Create"

# Use regex patterns
dotnet test --filter "Name~Course.*Create"
```

## 🛠️ Troubleshooting Common Issues

### 1. Integration Tests Failing Due to Database Issues
```powershell
# Clean and rebuild solution
dotnet clean
dotnet build

# Clear NuGet cache if package conflicts
dotnet nuget locals all --clear
```

### 2. Entity Framework Configuration Issues
```powershell
# Check if migrations are up to date
dotnet ef database update

# Reset test database
dotnet ef database drop --force
dotnet ef database update
```

### 3. Port Conflicts in Integration Tests
```
Error: Unable to bind to https://localhost:5001
```
**Solution**: Tests use TestServer, no actual ports needed. Check for running API instances.

### 4. Value Object Serialization Issues
```
Error: No suitable constructor found for entity type 'Address'
```
**Solution**: Ensure value objects have parameterless constructors or proper EF configuration.

## 📈 Test Coverage Analysis

### Current Test Statistics
- **Total Tests**: 325+ passing tests
- **Domain Coverage**: >95%
- **Application Coverage**: >90%
- **Integration Coverage**: 13 comprehensive API tests

### Coverage Goals
- **Domain Layer**: 95%+ coverage (business logic critical)
- **Application Layer**: 90%+ coverage (CQRS handlers)
- **API Controllers**: 85%+ coverage (integration tests)
- **Infrastructure**: 80%+ coverage (data access)

## 🔄 Continuous Integration

### GitHub Actions Test Workflow
```yaml
# Example CI configuration for automated testing
name: Run Tests
on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Restore dependencies
      run: dotnet restore backend/EduTrack/
    
    - name: Run unit tests
      run: dotnet test backend/EduTrack/ --filter "Category!=Integration" --logger trx --collect:"XPlat Code Coverage"
    
    - name: Run integration tests
      run: dotnet test backend/EduTrack/tests/EduTrack.Api.IntegrationTests/
```

## 📚 Best Practices

### 1. Test Organization
- **Arrange-Act-Assert**: Clear test structure
- **One Assertion Per Test**: Focus on single behavior
- **Descriptive Names**: `CreateCourse_WithValidData_ShouldReturnCourseId`

### 2. Test Data Management
- **Builder Pattern**: Use test data builders for complex objects
- **Factory Methods**: Centralized test data creation
- **Isolation**: Each test should be independent

### 3. Mocking Guidelines
- **Mock External Dependencies**: Database, HTTP clients, file system
- **Don't Mock Value Objects**: Test them directly
- **Verify Interactions**: Ensure mocks are called correctly

### 4. Integration Test Strategy
- **Test Happy Paths**: Normal workflow scenarios
- **Test Error Cases**: Invalid data, not found scenarios
- **Test Business Rules**: Domain-specific validation

## 🎯 Quick Commands Reference

```powershell
# Essential test commands
dotnet test                                    # Run all tests
dotnet test --filter "Category!=Integration"  # Unit tests only
dotnet test tests\EduTrack.Api.IntegrationTests\ # Integration tests only
dotnet test --verbosity normal               # Detailed output
dotnet test --collect:"XPlat Code Coverage"  # With coverage
dotnet test --filter "Course"                # Tests containing "Course"
dotnet test --logger trx                     # Generate test report
```

## 📞 Support

For testing issues or questions:
1. Check this guide first
2. Review test output and error messages
3. Verify project builds successfully: `dotnet build`
4. Check for recent changes in test configuration
5. Create issue with test output and steps to reproduce

---

*This testing guide covers the EduTrack project's comprehensive test suite. Keep this document updated as new test projects and scenarios are added.*
