---
mode: agent
description: Create a new domain entity with EF Core config, repository interface, repository implementation, DTO, and AutoMapper profile
---

Create a new domain entity named **${input:entity}** with the following properties: **${input:properties}** (e.g., `Name: string, Code: string, Credits: int`).

Follow the existing `Student` entity pattern in `EduTrack.Domain/Entities/Student.cs`.

## Files to create

### 1. Domain Entity — `EduTrack.Domain/Entities/${entity}.cs`

- Extend `AggregateRoot<Guid>` from `EduTrack.Domain.Common`
- Use **private backing fields** for all properties; expose via public read-only properties
- Add a **static factory method** `${entity}.Create(...)` — no public constructor
- Use Value Objects where appropriate (e.g., `Email`, `FullName`, `Address`, `PhoneNumber` from `EduTrack.Domain.ValueObjects`)
- Raise domain events in the factory method and state-change methods
- Keep all mutation methods on the entity (e.g., `Update${entity}(...)`, `Activate()`, `Deactivate()`)

### 2. Domain Repository Interface — `EduTrack.Domain/Contracts/Repositories/I${entity}Repository.cs`

```csharp
public interface I${entity}Repository
{
    Task<List<${entity}>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<${entity}?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(${entity} entity, CancellationToken cancellationToken = default);
    void Update(${entity} entity);
    void Delete(${entity} entity);
}
```

Add entity-specific queries only if requested.

### 3. Register in IUnitOfWork — `EduTrack.Domain/Contracts/Repositories/IUnitOfWork.cs`

Add: `I${entity}Repository ${entity}s { get; }`

### 4. EF Core Configuration — `EduTrack.Infrastructure/Data/Configurations/`

- Create `${entity}Configuration.cs` implementing `IEntityTypeConfiguration<${entity}>`
- Map value object properties using `.OwnsOne()` or `.Property().HasConversion()` as needed
- Apply in `ApplicationDbContext.OnModelCreating()` via `modelBuilder.ApplyConfiguration(new ${entity}Configuration())`

### 5. Repository Implementation — `EduTrack.Infrastructure/Repositories/${entity}Repository.cs`

- Inject `ApplicationDbContext` and `ILogger<${entity}Repository>`
- Implement all interface methods using EF Core
- Use structured logging: `_logger.LogDebug("Fetching {Entity} by ID {Id}", nameof(${entity}), id)`

### 6. Register in DI — `EduTrack.Infrastructure/DependencyInjection/`

Add: `services.AddScoped<I${entity}Repository, ${entity}Repository>()`
Update `UnitOfWork.cs` to include the new repository.

### 7. DTO — `EduTrack.Application/Features/${entity}s/DTOs/${entity}Dto.cs`

Simple record with public get-only properties matching the entity's public surface.

### 8. AutoMapper Profile — `EduTrack.Application/Features/${entity}s/Mappings/${entity}Profile.cs`

```csharp
public class ${entity}Profile : Profile
{
    public ${entity}Profile()
    {
        CreateMap<${entity}, ${entity}Dto>();
    }
}
```

Add `.ForMember()` overrides only where property names or types differ.

### 9. EF Migration reminder

After all files are created, remind the user to run:
```
dotnet ef migrations add Add${entity}Entity --project EduTrack.Infrastructure --startup-project EduTrack.Api
dotnet ef database update --project EduTrack.Infrastructure --startup-project EduTrack.Api
```
