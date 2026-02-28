# 🏗️ Clean Architecture Overview

## Introduction

This document describes how Clean Architecture is implemented in the EduTrack project, based on the actual application structure. EduTrack follows Uncle Bob's Clean Architecture principles combined with Domain-Driven Design (DDD) and CQRS, built on **.NET 10** (backend) and **Angular 18 + Angular Material 18** (frontend).

---

## Technology Stack

| Layer | Technology | Version |
|---|---|---|
| Backend Framework | .NET / ASP.NET Core | 10.0 |
| ORM | Entity Framework Core | 9.x |
| Mediator | MediatR | 12.5 |
| API Documentation | Swashbuckle (Swagger) | 8.x |
| Frontend Framework | Angular | 18.x |
| UI Components | Angular Material | 18.x |
| State Management | NgRx (Store, Effects) | 18.x |
| Frontend Rendering | Angular SSR | 18.x |

---

## Architecture Layers

```
┌──────────────────────────────────────────────────────────────┐
│               🎨 Frontend (Angular 18 + SSR)                 │
│         Features: Dashboard · Students · Courses             │
│         State: NgRx Store + Effects                          │
│         Core: Services · Auth Interceptor                    │
└──────────────────────────────┬───────────────────────────────┘
                               │ HTTP / REST
┌──────────────────────────────▼───────────────────────────────┐
│              🎯 API Layer  (EduTrack.Api)                     │
│         Controllers · Middleware · Models                    │
└──────────────────────────────┬───────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────┐
│           💼 Application Layer  (EduTrack.Application)       │
│    Features (CQRS) · Pipeline Behaviors · Event Handlers     │
└──────────────────────────────┬───────────────────────────────┘
                               │
┌──────────────────────────────▼───────────────────────────────┐
│              🏢 Domain Layer  (EduTrack.Domain)              │
│   Entities · Value Objects · Domain Events · Contracts       │
└──────────────────────────────┬───────────────────────────────┘
                               │ implements
┌──────────────────────────────▼───────────────────────────────┐
│         ⚙️ Infrastructure Layer  (EduTrack.Infrastructure)   │
│    EF Core DbContext · Repositories · Migrations · Identity  │
└──────────────────────────────────────────────────────────────┘
```

---

## Layer Details

### 1. Domain Layer — `EduTrack.Domain`

The innermost layer. No dependencies on any other project layer or external framework.

#### Entities
| Entity | Description |
|---|---|
| `Student` | Aggregate root — student lifecycle, GPA, enrollment |
| `Course` | Aggregate root — course status, scheduling, teacher assignment |
| `Teacher` | Faculty member with academic title and employment status |
| `Department` | Organizational unit; head assignment and status |
| `Attendance` | Attendance records linked to students and courses |

#### Value Objects
`Address` · `Email` · `FullName` · `GPA` · `PhoneNumber` · `StudentId`

#### Enums
`StudentStatus` · `CourseStatus` · `CourseLevel` · `DepartmentStatus` · `AcademicTitle` · `EmploymentStatus`

#### Domain Events (34 events)
Every meaningful state change raises a domain event, grouped by aggregate:

- **Student**: `StudentCreatedEvent`, `StudentStatusChangedEvent`, `StudentGPAUpdatedEvent`, `StudentEnrolledInCourseEvent`, `StudentWithdrewFromCourseEvent`, `StudentGraduatedEvent`, `StudentDeactivatedEvent`, `StudentReactivatedEvent`, `StudentContactUpdatedEvent`, `StudentPersonalInfoUpdatedEvent`, `StudentAddressUpdatedEvent`, `StudentPhoneNumberUpdatedEvent`, `StudentAcademicStandingChangedEvent`
- **Course**: `CourseCreatedEvent`, `CourseUpdatedEvent`, `CourseActivatedEvent`, `CourseScheduledEvent`, `CourseCompletedEvent`, `CourseCancelledEvent`
- **Teacher**: `TeacherCreatedEvent`, `TeacherContactUpdatedEvent`, `TeacherTitleUpdatedEvent`, `TeacherAssignedToCourseEvent`, `TeacherRemovedFromCourseEvent`, `TeacherDeactivatedEvent`, `TeacherReactivatedEvent`
- **Department**: `DepartmentCreatedEvent`, `DepartmentNameUpdatedEvent`, `DepartmentContactUpdatedEvent`, `DepartmentHeadAssignedEvent`, `DepartmentHeadRemovedEvent`, `DepartmentStatusChangedEvent`
- **Attendance**: `AttendanceRecordedEvent`, `AttendanceUpdatedEvent`

#### Common Primitives
| Type | Purpose |
|---|---|
| `BaseEntity` | Base class with `Id`, `CreatedAt`, `UpdatedAt` |
| `AggregateRoot` | Extends `BaseEntity`; owns the domain event collection |
| `DomainEvent` | Base record for all domain events |
| `IHasDomainEvents` | Interface enforced on aggregate roots |
| `Result<T>` | Functional result type for explicit error handling |

#### Contracts
- `IStudentRepository` — student-specific data access contract
- `ICourseRepository` — course-specific data access contract
- `IUnitOfWork` — transaction boundary contract
- `IDomainService` — marker interface for domain services

#### Dependencies
> **None** — zero references to external projects or frameworks.

---

### 2. Application Layer — `EduTrack.Application`

Orchestrates use cases using CQRS via MediatR. Depends only on the Domain layer.

#### Features — Students

| Type | Operations |
|---|---|
| Commands | `CreateStudent`, `UpdateStudent`, `DeleteStudent`, `ChangeStudentStatus`, `UpdateStudentContact`, `UpdateStudentGPA` |
| Queries | `GetStudent`, `GetStudentList`, `GetStudentsByStatus`, `GetStudentsOnProbation` |

Each feature folder contains: `Command/Query` · `Handler` · `Validator` (FluentValidation) · `Dto` · `MappingProfile`

#### Features — Courses

| Type | Operations |
|---|---|
| Commands | `CreateCourse`, `UpdateCourse`, `DeleteCourse`, `ActivateCourse`, `ScheduleCourse`, `CompleteCourse` |
| Queries | `GetCourse`, `GetCourseList` (and variants) |

#### Pipeline Behaviors (MediatR)
| Behavior | Responsibility |
|---|---|
| `ValidationBehavior` | Runs FluentValidation before handler; throws on failure |
| `LoggingBehavior` | Structured request/response logging via `ILogger<T>` |
| `PerformanceBehavior` | Logs a warning when a request exceeds the performance threshold |

#### Event Handlers
- `EventHandlers/Students/` — handles domain events raised by the `Student` aggregate

#### Common
- `MappingProfiles.cs` — central AutoMapper profile registration
- `Common/Exceptions/` — application-level exceptions

#### Dependencies
> Domain Layer ✅

---

### 3. Infrastructure Layer — `EduTrack.Infrastructure`

Implements contracts defined in the Domain layer. Depends on Domain only.

#### Data
| Component | Description |
|---|---|
| `ApplicationDbContext` | EF Core DbContext; registers all entity sets |
| `ApplicationDbContextFactory` | Design-time factory for EF migrations |
| `Configurations/` | Fluent entity type configurations (IEntityTypeConfiguration) |
| `Migrations/` | EF Core migration history |
| `SeedData/` | Initial/reference data seeding |

#### Repositories
| Class | Contract |
|---|---|
| `StudentRepository` | `IStudentRepository` |
| `CourseRepository` | `ICourseRepository` |
| `UnitOfWork` | `IUnitOfWork` |

#### Identity
Placeholder for JWT-based authentication and role management (planned).

#### Services
Placeholder for external service implementations — email, notifications (planned).

#### Dependencies
> Domain Layer ✅

---

### 4. API Layer — `EduTrack.Api`

Presentation layer. Handles HTTP concerns only; all business logic lives in lower layers.

#### Controllers
| Controller | Responsibilities |
|---|---|
| `StudentsController` | REST endpoints: create, read, update, delete, status change, GPA update |
| `CoursesController` | REST endpoints: create, read, update, activate, schedule, complete, delete |

Controllers dispatch requests directly to MediatR — no business logic in controllers.

#### Middleware
| Middleware | Responsibility |
|---|---|
| `GlobalExceptionHandlerMiddleware` | Catches all unhandled exceptions; returns structured ProblemDetails responses |

#### Configuration
- `Program.cs` — service registration (DI composition root), middleware pipeline
- `appsettings.json` / `appsettings.Development.json` — environment-specific configuration

#### Dependencies
> Application Layer ✅ · Infrastructure Layer ✅ (DI registration only)

---

### 5. Frontend — `edutrack-ui` (Angular 18)

A standalone Angular 18 application with Server-Side Rendering (SSR).

#### Directory Structure
```
src/app/
├── core/
│   ├── services/          # auth.service, student.service, course.service
│   ├── interceptors/      # auth.interceptor (JWT token attachment)
│   └── core.module.ts
├── features/
│   ├── students/          # Student list, detail, create/edit pages
│   ├── courses/           # Course list, detail, create/edit pages
│   └── dashboard/         # Overview and metrics dashboard
├── layout/                # Shell layout (nav, sidebar, header)
├── models/                # TypeScript interfaces matching backend DTOs
├── shared/                # Reusable components, pipes, directives
├── app.routes.ts          # Lazy-loaded route definitions
├── app.config.ts          # Standalone app configuration (provideRouter, etc.)
└── app.config.server.ts   # SSR-specific providers
```

#### State Management (NgRx)
- `@ngrx/store` — centralized state
- `@ngrx/effects` — side effects (API calls)
- `@ngrx/store-devtools` — Redux DevTools integration

---

## Dependency Rule (summary)

```
Frontend  →  Api  →  Application  →  Domain  ←  Infrastructure
```

- Dependencies point **inward only**
- Domain has **no outward dependencies**
- Infrastructure and API reference Domain (or Application), never the reverse

---

## CQRS Flow (end-to-end example)

```
POST /api/students
      │
      ▼
StudentsController
  sends CreateStudentCommand via MediatR
      │
      ▼  Pipeline behaviors run in order:
      │    1. ValidationBehavior  (FluentValidation)
      │    2. LoggingBehavior     (structured log)
      │    3. PerformanceBehavior (timing)
      ▼
CreateStudentCommandHandler
  → calls IStudentRepository.AddAsync()
  → calls IUnitOfWork.CommitAsync()
  → Student aggregate raises StudentCreatedEvent
      │
      ▼
Domain event dispatched → StudentCreatedEventHandler
  → performs side effects (e.g., notifications)
      │
      ▼
Returns StudentDto  →  201 Created response
```

---

## Testing Structure

| Project | Scope |
|---|---|
| `EduTrack.Domain.UnitTests` | Entity behavior, value object validation, domain event raising |
| `EduTrack.Application.UnitTests` | Command/query handlers with mocked repositories (Moq) |
| `EduTrack.Infrastructure.UnitTests` | Repository implementations with in-memory EF Core |
| `EduTrack.Api.UnitTests` | Controller logic, middleware behavior |
| `EduTrack.Api.IntegrationTests` | Full HTTP pipeline with real database |

All tests use **xUnit** and **Moq**.

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| MediatR pipeline behaviors | Cross-cutting concerns (logging, validation, performance) stay out of handlers |
| Domain events (not application events) | Side effects are triggered by the domain, not by application orchestration |
| `Result<T>` return type | Explicit, exception-free error handling in domain operations |
| Contracts in Domain (not Application) | Repository interfaces belong to the domain — the domain defines what it needs |
| SSR on Angular frontend | Better SEO and initial page load performance |
| NgRx for state management | Predictable state in a complex, data-driven UI |
