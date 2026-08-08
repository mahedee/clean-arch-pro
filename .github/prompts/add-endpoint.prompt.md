---
mode: agent
description: Add a new controller endpoint to an existing API controller following EduTrack conventions
---

Add a new endpoint to **${input:controller}Controller** (e.g., `Students`, `Courses`, `Teachers`) for the operation **${input:operation}** (e.g., `GetByStatus`, `Enroll`, `Deactivate`).

Follow the existing pattern in `EduTrack.Api/Controllers/StudentsController.cs`.

## Rules

- The controller already extends `ControllerBase` with `[ApiController]` and `[Route("api/[controller]")]` — do not change those.
- Inject only `IMediator` — no repositories or services in controllers.
- Map incoming DTOs to a Command or Query, then call `await _mediator.Send(...)`.
- Never construct entities or call business logic in the controller.

## HTTP method and return conventions

| Operation type | HTTP method | Success return |
|---|---|---|
| Create | `[HttpPost]` | `CreatedAtAction(nameof(Get{Entity}), new { id }, id)` |
| Full update | `[HttpPut("{id:guid}")]` | `NoContent()` |
| Partial update / status change | `[HttpPatch("{id:guid}/action")` | `NoContent()` |
| Delete | `[HttpDelete("{id:guid}")]` | `NoContent()` |
| Get single | `[HttpGet("{id:guid}")]` | `Ok(result)` |
| Get list / search | `[HttpGet]` | `Ok(result)` |

## What to generate

1. **Request DTO** (if needed) in `EduTrack.Api/Models/` — simple class with `[FromBody]` properties.
2. **Action method** in the correct controller file — thin, no business logic.
3. **Corresponding Command or Query** if it does not already exist — place in `EduTrack.Application/Features/${controller}/Commands/` or `Queries/`.
4. **Handler** if it does not already exist — follows `IRequestHandler<TRequest, TResponse>` pattern, uses `IUnitOfWork`.
5. **Validator** for the command/query — extends `AbstractValidator<T>`.

## Example pattern to follow

```csharp
[HttpPut("{id:guid}/status")]
public async Task<ActionResult> ChangeStatus(Guid id, [FromBody] ChangeStatusDto dto)
{
    var command = new ChangeStudentStatusCommand(id, dto.Status);
    await _mediator.Send(command);
    return NoContent();
}
```
