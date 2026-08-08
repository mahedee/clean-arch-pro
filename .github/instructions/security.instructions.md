---
applyTo: "**/*.cs,**/*.ts,**/*.html"
description: Secure coding rules for EduTrack — OWASP Top 10 guidance for Copilot-generated code
---

# Secure Coding Rules

Apply these rules to all generated code. Flag any violation as a comment before generating.

## Input Validation & Injection Prevention (OWASP A03)

- **Never** build SQL strings manually. Always use EF Core LINQ queries or parameterized raw SQL (`FromSqlRaw` with parameters only — never string interpolation).
- **Never** interpolate user input into log messages, file paths, URLs, or shell commands.
- Validate all external input at the boundary (FluentValidation validators) before it reaches the Application layer.
- In Angular: use Angular's built-in sanitization (`DomSanitizer`) — never use `innerHTML` with untrusted data; never bypass `bypassSecurityTrustHtml` without explicit justification.

## Authentication & Authorization (OWASP A01, A07)

- Every controller action must be decorated with `[Authorize]` unless it is explicitly a public endpoint.
- Never expose internal IDs or implementation details in error messages returned to clients.
- Passwords must never be logged, returned in DTOs, or stored in plain text.
- JWT tokens must be validated on every request — never trust claims from an unverified token.

## Sensitive Data Exposure (OWASP A02)

- Connection strings, API keys, and secrets must only come from environment variables or `IConfiguration` — never hardcoded.
- Do not include sensitive fields (passwords, tokens, SSNs) in DTOs sent to the client.
- Ensure PII (email, date of birth, phone) is excluded from log output.

## Security Misconfiguration (OWASP A05)

- CORS must use explicit allowed origins from configuration — never `AllowAnyOrigin()` in production.
- Swagger/OpenAPI must only be enabled in Development environments.
- `HttpsRedirection` must be called before any auth or data middleware.
- Never disable SSL certificate validation (`ServerCertificateCustomValidationCallback` returning `true`).

## Vulnerable & Outdated Components (OWASP A06)

- Do not add NuGet or npm packages without checking their CVE status.
- Prefer packages with active maintenance and recent releases.

## Error Handling & Logging (OWASP A09)

- Use the global `GlobalExceptionHandlerMiddleware` — do not catch and swallow exceptions silently.
- Return RFC 7807 `ProblemDetails` responses, not raw exception messages or stack traces.
- Use structured logging: `_logger.LogWarning("User {UserId} failed login", userId)` — no string interpolation.

## C# / .NET Specific

- Use `CancellationToken` in all async repository and handler methods.
- Prefer `Guid` over sequential `int` IDs for public-facing resource identifiers to prevent enumeration attacks.
- Validate `Guid` route parameters with `{id:guid}` route constraint.

## Angular / TypeScript Specific

- Use Angular `HttpClient` interceptors for attaching auth headers — never attach tokens manually per request.
- Avoid storing tokens in `localStorage`; prefer `sessionStorage` or in-memory with silent refresh.
- Always handle HTTP errors in services, not in components.
