# Rich vs Anemic Domain Model

A practical guide using EduTrack's [Attendance.cs](../../src/backend/EduTrack/src/EduTrack.Domain/Entities/Attendance.cs) entity as the reference example.

---

## 1. Anemic Domain Model

### Definition
An **anemic domain model** is an object model where classes contain **only data (properties/fields)** and **little or no behavior**. All business logic lives outside the entity — typically in "service" or "manager" classes.

Coined as an **anti-pattern** by Martin Fowler ([article](https://martinfowler.com/bliki/AnemicDomainModel.html)), because it looks object-oriented but is procedural in disguise.

### Example (Anemic)

```csharp
// Entity — just a data bag
public class Attendance
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public bool IsVerified { get; set; }
}

// All logic lives here
public class AttendanceService
{
    public Attendance RecordAttendance(Guid studentId, Guid courseId,
        DateTime date, bool isPresent, string? notes)
    {
        if (studentId == Guid.Empty) throw new ArgumentException(...);
        if (courseId == Guid.Empty) throw new ArgumentException(...);
        if (date > DateTime.Now)    throw new ArgumentException(...);
        if (notes?.Length > 500)    throw new ArgumentException(...);

        return new Attendance {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            CourseId  = courseId,
            AttendanceDate = date,
            IsPresent = isPresent,
            Notes = notes,
            IsVerified = false
        };
    }

    public void MarkPresent(Attendance a, string? notes)
    {
        a.IsPresent = true;
        a.Notes = notes;
    }

    public void Verify(Attendance a, string verifiedBy)
    {
        if (string.IsNullOrWhiteSpace(verifiedBy)) throw new ArgumentException(...);
        a.IsVerified = true;
    }
}
```

### Pros
| # | Pro |
|---|-----|
| 1 | **Simple & familiar** — looks like a DB row; easy for CRUD apps. |
| 2 | **Low learning curve** — no DDD knowledge required. |
| 3 | **Easy serialization/mapping** — works seamlessly with ORMs, JSON, DTOs. |
| 4 | **Fits transaction-script style** for very small / short-lived apps. |

### Cons
| # | Con |
|---|-----|
| 1 | **Business rules scattered** across many services → duplication. |
| 2 | **Invariants not enforced** — any caller can put the entity in an invalid state. |
| 3 | **Procedural, not OO** — violates encapsulation; entities are just structs. |
| 4 | **Hard to test business logic** in isolation — coupled to services. |
| 5 | **Doesn't scale** with domain complexity — services become "god classes". |
| 6 | **No expressive ubiquitous language** — code reads like SQL, not domain. |

---

## 2. Rich Domain Model

### Definition
A **rich domain model** places **both data and behavior** inside the entity/aggregate. The model enforces its own **invariants**, exposes **intent-revealing methods**, and is the centerpiece of **Domain-Driven Design (DDD)**.

### Example (Rich) — EduTrack's actual `Attendance`

See [Attendance.cs](../../src/backend/EduTrack/src/EduTrack.Domain/Entities/Attendance.cs):

```csharp
public class Attendance : AggregateRoot<Guid>
{
    public Guid StudentId { get; private set; }
    public bool IsPresent { get; private set; }
    public bool IsVerified { get; private set; }
    // ...all setters private

    private Attendance() { }   // EF Core only

    public static Attendance RecordAttendance(
        Guid studentId, Guid courseId, DateTime date,
        bool isPresent, string? recordedBy = null, string? notes = null)
    {
        if (studentId == Guid.Empty) throw new ArgumentException(...);
        // ...invariants enforced here
        var a = new Attendance { /* ... */ };
        a.AddDomainEvent(new AttendanceRecordedEvent(...));
        return a;
    }

    public void MarkPresent(string? notes = null) { /* + domain event */ }
    public void MarkAbsent(string? notes = null)  { /* + domain event */ }
    public void Verify(string verifiedBy)         { /* invariant + state */ }
    public bool CanBeModified(int maxDays = 7)    { /* domain query */ }
}
```

### Pros
| # | Pro |
|---|-----|
| 1 | **Encapsulation** — invalid states are unreachable. |
| 2 | **Single source of truth** for business rules. |
| 3 | **Expressive API** — `attendance.MarkPresent()` reads like the domain. |
| 4 | **Easy unit testing** — pure objects, no infrastructure. |
| 5 | **Supports DDD tactical patterns** — Aggregates, Value Objects, Domain Events. |
| 6 | **Scales with complexity** — logic stays cohesive as domain grows. |

### Cons
| # | Con |
|---|-----|
| 1 | **Steeper learning curve** — DDD, aggregates, value objects. |
| 2 | **More boilerplate** for trivial CRUD scenarios. |
| 3 | **ORM friction** — needs private constructors, backing fields, navigation rules. |
| 4 | **Mapping to DTOs** requires extra layer (AutoMapper, etc.). |
| 5 | **Overkill for simple apps** — adds ceremony without payoff. |

---

## 3. Side-by-Side Comparison

| Aspect | Anemic | Rich |
|---|---|---|
| Where is logic? | Services | Entity / Aggregate |
| Setters | Public | Private |
| Invariants | Enforced externally (or not) | Enforced internally |
| Object construction | `new` + property init | Factory methods |
| State changes | Direct property assignment | Intent-revealing methods |
| Domain events | Rarely used | First-class |
| Testability of rules | Through services | Through entity directly |
| Best fit | Simple CRUD apps | Complex business domains |

---

## 4. When to Use Which

**Use Anemic when:**
- Pure CRUD app, minimal business rules.
- Short-lived prototypes / admin dashboards.
- Reporting / read-only models (CQRS read side).

**Use Rich when:**
- Non-trivial business rules and invariants.
- Multiple workflows operate on the same entity.
- You're applying **DDD / Clean Architecture** (which is exactly what `EduTrack` does — see [.github/copilot-instructions.md](../../.github/copilot-instructions.md)).
- You expect the domain to evolve over time.

---

## 5. Verdict for EduTrack

The project follows **Clean Architecture + DDD**, and entities like [Attendance.cs](../../src/backend/EduTrack/src/EduTrack.Domain/Entities/Attendance.cs) are correctly implemented as **rich domain models** — with private setters, factory methods, invariants, and domain events. Continue this pattern for new entities (`Student`, `Course`, etc.).

---

## References
- Martin Fowler — [AnemicDomainModel](https://martinfowler.com/bliki/AnemicDomainModel.html)
- Eric Evans — *Domain-Driven Design: Tackling Complexity in the Heart of Software*
- Vaughn Vernon — *Implementing Domain-Driven Design*
