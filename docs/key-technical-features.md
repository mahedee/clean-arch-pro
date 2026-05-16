# ⚙️ Key Technical Features

EduTrack is built on a modern, production-ready technical foundation following Clean Architecture, Domain-Driven Design, and CQRS principles.

---

## 🏗️ Architecture & Design Patterns

| Pattern | Implementation |
|---------|---------------|
| **Clean Architecture** | Strict layer separation: Domain → Application → Infrastructure → API |
| **Domain-Driven Design (DDD)** | Rich domain models, value objects, aggregates, and domain events |
| **CQRS** | Commands and queries separated via MediatR |
| **Repository + Unit of Work** | Data access abstraction over EF Core; no direct DbContext in handlers |
| **Event-Driven** | In-process domain events for decoupled side effects |
| **Specification Pattern** | Encapsulated, reusable query predicates |

---

## ⚙️ Backend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET / ASP.NET Core | 10.0 | Web API framework |
| Entity Framework Core | 10.0 | ORM & data access |
| MediatR | 12.x | CQRS & mediator pattern |
| AutoMapper | 12.x | DTO ↔ Entity mapping |
| FluentValidation | 11.x | Pipeline request validation |
| Serilog | 3.x | Structured logging |
| xUnit + Moq | 2.x | Unit & integration testing |

---

## 🗄️ Database Support

| Database | Status | Notes |
|----------|--------|-------|
| PostgreSQL | ✅ Primary | Recommended; JSONB support |
| SQL Server | ✅ Supported | Enterprise environments |
| Oracle | ✅ Enterprise | Large-scale deployments |
| SQLite | 🔄 Testing Only | In-memory & file-based for tests |

---

## 🔐 Security

- **JWT Authentication** with refresh token rotation
- **Role-Based & Permission-Based Authorization** via ASP.NET Core policies
- **FluentValidation pipeline** — all inputs validated before reaching handlers
- **Structured error responses** — no stack traces or internal details leaked to clients
- **HTTPS enforcement** in production launch profile

---

## 🅰️ Frontend Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 18.x | SPA framework |
| Angular Material | 18.x | UI component library |
| TypeScript | 5.x | Type safety |
| RxJS | 7.x | Reactive state & HTTP |

- Dev server proxies all `/api/*` calls to the backend (`http://localhost:6100`)
- Angular SSR support included for production deployments

---

## 🧪 Testing Strategy

| Layer | Framework | Coverage Target |
|-------|-----------|----------------|
| Domain unit tests | xUnit + Moq | ≥ 95% |
| Application unit tests | xUnit + Moq | ≥ 80% |
| API integration tests | WebApplicationFactory | Key endpoints |
| Frontend unit tests | Karma + Jasmine | ≥ 70% |

---

## 🚀 DevOps & Tooling

- **CI/CD** — GitHub Actions workflows for build, test, and publish
- **Docker** — Multi-stage Dockerfile for optimised production images
- **OpenAPI / Swagger** — Auto-generated API documentation at `/swagger`
- **EF Core Migrations** — Version-controlled schema management
- **Coverlet + ReportGenerator** — HTML code coverage reports

---

For aggregated technical references from other Clean Architecture projects, see [technical-features-and-technologies.md](technical-features-and-technologies.md).
