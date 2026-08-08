# EduTrack — Claude Code Instructions

## Project Overview

EduTrack is an educational tracking system built with **Clean Architecture** in .NET (backend) and Angular (frontend).

## Repository Structure

```
backend/EduTrack/src/
  EduTrack.Api/            # ASP.NET Core Web API
  EduTrack.Application/    # CQRS handlers, DTOs, validators
  EduTrack.Domain/         # Entities, interfaces, domain logic
  EduTrack.Infrastructure/ # EF Core, repositories, external services
frontend/edutrack-ui/      # Angular application
```

## Architecture & Patterns

- **Clean Architecture**: changes flow inward — Api → Application → Domain; Infrastructure implements Domain interfaces.
- **CQRS with MediatR**: all business operations go through commands (`CreateXCommand`) and queries (`GetXQuery`).
- **Repository + Unit of Work**: never use `DbContext` directly in handlers; go through `IUnitOfWork`.
- **AutoMapper**: map between entities and DTOs in Application layer profiles.
- **FluentValidation**: add a `XValidator` class alongside every command/query.

## Naming Conventions

| Artifact | Convention | Example |
|---|---|---|
| Entity | Singular PascalCase | `Student` |
| DTO | Suffix `Dto` | `StudentDto` |
| Interface | Prefix `I` | `IStudentRepository` |
| Command | Suffix `Command` | `CreateStudentCommand` |
| Query | Suffix `Query` | `GetStudentQuery` |
| Handler | Suffix `Handler` | `CreateStudentCommandHandler` |
| Validator | Suffix `Validator` | `CreateStudentCommandValidator` |

## Technology Stack

- **Backend**: .NET 8, ASP.NET Core, EF Core, PostgreSQL
- **Frontend**: Angular, TypeScript
- **Testing**: xUnit, Moq
- **Logging**: structured logging via `ILogger<T>`

## Key Rules

1. Do not use `DbContext` directly in Application layer handlers.
2. Always add a FluentValidation validator for new commands/queries.
3. Use structured logging — no string interpolation in log calls.
4. Unit tests go in `backend/EduTrack/tests/`; use xUnit + Moq.
5. Keep domain entities free of infrastructure concerns.
