# Running Backend Unit Tests

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) installed
- Solution dependencies restored (`dotnet restore`)

---

## Test Projects

| Project | Scope |
|---------|-------|
| `EduTrack.Domain.UnitTests` | Domain entities, value objects, domain events |
| `EduTrack.Application.UnitTests` | Command/query handlers, validators, pipeline behaviors |
| `EduTrack.Infrastructure.UnitTests` | Repository implementations, EF Core interactions |
| `EduTrack.Api.IntegrationTests` | Full HTTP request/response cycle via WebApplicationFactory |

---

## Run All Tests

From the solution root:

```bash
cd src/backend/EduTrack
dotnet test
```

---

## Run a Specific Test Project

```bash
cd src/backend/EduTrack
dotnet test tests/EduTrack.Domain.UnitTests
dotnet test tests/EduTrack.Application.UnitTests
dotnet test tests/EduTrack.Infrastructure.UnitTests
dotnet test tests/EduTrack.Api.IntegrationTests
```

---

## Run with Verbose Output

```bash
dotnet test --verbosity normal
```

---

## Filter Tests by Name

```bash
# Run tests whose name contains a keyword
dotnet test --filter "FullyQualifiedName~Student"

# Run a single test class
dotnet test --filter "ClassName=CreateStudentCommandHandlerTests"

# Run tests by category trait
dotnet test --filter "Category=Unit"
```

---

## Run in Release Configuration

```bash
dotnet test --configuration Release
```

---

## Test Output

Results are printed to the console. By default, a summary shows passed, failed, and skipped counts. Detailed output for failed tests is shown automatically.

To export results in TRX format:

```bash
dotnet test --logger "trx;LogFileName=test-results.trx" --results-directory ./TestResults
```

---

## Tech Stack

- **Test framework:** xUnit
- **Mocking:** Moq
- **Coverage collector:** Coverlet (configured via `coverlet.runsettings`)
