# EduTrack Testing Quick Reference

## 🚀 Essential Commands

### Run All Tests
```powershell
# Navigate to project root
cd backend/EduTrack

# Run all tests (recommended)
dotnet test

# Run with detailed output
dotnet test --verbosity normal
```

### Unit Tests Only
```powershell
# Exclude integration tests for faster feedback
dotnet test --filter "Category!=Integration"

# Domain tests only
dotnet test tests\EduTrack.Domain.UnitTests\

# Application tests only  
dotnet test tests\EduTrack.Application.UnitTests\
```

### Integration Tests Only
```powershell
# Run API integration tests
dotnet test tests\EduTrack.Api.IntegrationTests\

# Single test method
dotnet test tests\EduTrack.Api.IntegrationTests\ --filter "CreateCourse_WithValidData_ReturnsCreatedCourse"
```

### Test Coverage
```powershell
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
```

### Troubleshooting
```powershell
# Clean and rebuild if tests fail
dotnet clean && dotnet build

# Clear NuGet cache
dotnet nuget locals all --clear
```

## 📊 Current Test Status

- **✅ Total Tests**: 325+ passing
- **✅ Unit Tests**: Domain, Application, Infrastructure layers
- **✅ Integration Tests**: 13 comprehensive API tests
- **✅ Coverage**: >90% across all critical layers

## 🔗 Full Documentation

For comprehensive testing instructions, see [Testing Guide](testing-guide.md).

---

*Quick reference for EduTrack testing - updated January 2025*
