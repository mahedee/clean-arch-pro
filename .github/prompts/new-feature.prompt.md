---
mode: agent
description: Scaffold a complete CQRS feature (command + query + handler + validator + DTO + mapping + tests)
---

Scaffold a complete CQRS feature for the entity **${input:entity}** with the operation **${input:operation}** (e.g., Create, Update, Delete, Get, GetList).

Follow these rules exactly — mirror the existing `Students` feature in `EduTrack.Application/Features/Students/`.

## Files to create

### Application layer — `EduTrack.Application/Features/${entity}s/${operation}${entity}/`

1. **Command or Query**
   - If mutating: `${operation}${entity}Command.cs` — use `record` implementing `IRequest<T>` (return `Guid` for creates, nothing for updates/deletes using `Unit`)
   - If reading: `Get${entity}Query.cs` — use `class` implementing `IRequest<${entity}Dto?>`

2. **Handler** — `${operation}${entity}CommandHandler.cs` or `Get${entity}QueryHandler.cs`
   - Inject `IUnitOfWork` and `IMapper` (for commands); inject `IUnitOfWork` and `IMapper` for queries
   - Never inject `DbContext` directly
   - Use `_unitOfWork.${entity}s` to access the repository
   - Call `_unitOfWork.SaveChangesAsync(cancellationToken)` after mutations
   - Use structured logging: `_logger.LogInformation("Creating {Entity} with email {Email}", ...)` — no string interpolation

3. **Validator** — `${operation}${entity}CommandValidator.cs`
   - Extend `AbstractValidator<${operation}${entity}Command>`
   - Validate all required fields with `.NotEmpty()`, length rules, and format rules
   - Use `.MustAsync()` for async uniqueness checks via the repository interface

### Application layer — DTOs and Mappings

4. **DTO** — `EduTrack.Application/Features/${entity}s/DTOs/${entity}Dto.cs`
   - Simple record or class with public get-only properties

5. **AutoMapper Profile** — `EduTrack.Application/Features/${entity}s/Mappings/${entity}Profile.cs`
   - Extend `Profile`, call `CreateMap<${entity}, ${entity}Dto>()`

### Domain layer — `EduTrack.Domain/`

6. **Repository interface** — `EduTrack.Domain/Contracts/Repositories/I${entity}Repository.cs`
   - Methods: `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `Update`, `Delete`
   - Add entity-specific query methods as needed (e.g., `GetByEmailAsync`)

7. **Register** the new repository in `IUnitOfWork` interface

### Infrastructure layer

8. **Repository implementation** — `EduTrack.Infrastructure/Repositories/${entity}Repository.cs`
   - Inject `ApplicationDbContext` and `ILogger<${entity}Repository>`
   - Implement all interface methods using EF Core

9. **Register** in `EduTrack.Infrastructure/DependencyInjection/` alongside existing registrations

### API layer

10. **Controller action** — add to `EduTrack.Api/Controllers/${entity}sController.cs`
    - Use `[HttpPost]`, `[HttpGet("{id:guid}")]`, etc. as appropriate
    - Accept `[FromBody]` DTO, construct command, send via `_mediator.Send()`
    - Return `CreatedAtAction` for POST, `NoContent()` for PUT/DELETE, `Ok()` for GET

## Tests to create

11. **Handler unit test** — `EduTrack.Application.UnitTests/Features/${entity}s/${operation}${entity}CommandHandlerTests.cs`
    - Use xUnit + Moq
    - Mock `IUnitOfWork`, `IMapper`, repository
    - Test: happy path, entity-not-found (throws), duplicate (throws)
