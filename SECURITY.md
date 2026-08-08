# Security Policy

## Supported Versions

| Version | Supported |
|---|---|
| latest (`main`) | ✅ |
| older branches | ❌ |

## Reporting a Vulnerability

**Please do not open a public GitHub issue for security vulnerabilities.**

Report vulnerabilities privately via [GitHub Security Advisories](../../security/advisories/new).

Include:
- Description of the vulnerability
- Steps to reproduce
- Affected component (backend API, frontend, infrastructure)
- Potential impact

You will receive a response within **5 business days**. If the report is confirmed, a fix will be prioritised and a CVE advisory will be published after the patch is released.

## Security Measures in Place

- **Static analysis**: CodeQL scans all C# and TypeScript on every push and PR
- **Dependency scanning**: Dependabot monitors NuGet and npm packages weekly
- **Secret scanning**: GitHub secret scanning is enabled on this repository
- **Input validation**: All API inputs validated via FluentValidation before reaching handlers
- **Parameterized queries**: EF Core is used exclusively — no raw SQL string concatenation
- **Structured logging**: PII and sensitive fields are excluded from all log output
