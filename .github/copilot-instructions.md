# GitHub Copilot Instructions for EduTrack

- Use **Clean Architecture** principles: `Domain`, `Application`, `Infrastructure`, `Api` projects.  
- Always implement CQRS with **MediatR** for queries and commands.  
- Use **PostgreSQL with EF Core** for persistence.  
- Apply **Repository + Unit of Work** pattern instead of directly using DbContext in handlers.  
- Use **AutoMapper** for DTO ↔ Entity mapping.  
- Use **FluentValidation** for request validation.  
- Follow naming conventions:
  - Entities: singular (e.g., `Student`, not `Students`)
  - DTOs: suffix with `Dto` (e.g., `StudentDto`)
  - Interfaces: prefix with `I` (e.g., `IStudentRepository`)
- Logging should use **structured logging** via `ILogger<T>`.
- Write unit tests with **xUnit** and **Moq**.
