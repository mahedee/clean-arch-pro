# EduTrack - Clean Architecture Template Using ASP.NET Core

[![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-blue)](https://dotnet.microsoft.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL-informational)](https://www.postgresql.org/) [![Entity Framework Core](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-green)](https://learn.microsoft.com/en-us/ef/core/) [![Fluent API](https://img.shields.io/badge/Configuration-Fluent%20API-orange)](https://learn.microsoft.com/en-us/ef/core/modeling/) [![MediatR](https://img.shields.io/badge/Library-MediatR-red)](https://github.com/jbogard/MediatR) [![CQRS](https://img.shields.io/badge/Pattern-CQRS-blueviolet)](https://martinfowler.com/bliki/CQRS.html) [![Docker](https://img.shields.io/badge/Container-Docker-blue)](https://www.docker.com/) [![Serilog](https://img.shields.io/badge/Logging-Serilog-lightgrey)](https://serilog.net/)

---

EduTrack is a clean architecture template for building scalable, maintainable, and testable ASP.NET Core applications. It is designed for **education tracking or school management systems**, but can be extended to any domain. This project demonstrates the best practices using modern .NET tooling, patterns, and layered architecture.

> 🔥 If you find this project useful, please **give it a star ⭐** Thanks!

---

## 🚀 Features

- ✅ Clean Architecture with Domain-Driven Design (DDD)
- ✅ ASP.NET Core 8
- ✅ PostgreSQL (with option to switch to SQL Server easily)
- ✅ Entity Framework Core with Fluent API
- ✅ MediatR and CQRS
- ✅ AutoMapper
- ✅ Unit of Work and Repository Pattern
- ✅ Structured Logging
- ✅ Unit and Integration Tests
- ✅ Docker-friendly structure
- ✅ Swagger UI with minimal setup
- ✅ Open for community contributions

---

## 🧰 Tech Stack

- ASP.NET Core
- PostgreSQL
- Entity Framework Core
- MediatR
- AutoMapper
- xUnit / NUnit / FluentAssertions
- Serilog
- Docker (optional)
- Swagger / Swashbuckle

---

## 📂 Project Structure

```bash
EduTrack/
├── EduTrack.Api/             # API project (Presentation layer)
├── EduTrack.Application/     # Application layer (CQRS, DTOs, interfaces)
├── EduTrack.Domain/          # Domain layer (entities, enums, interfaces)
├── EduTrack.Infrastructure/  # Infrastructure layer (EF Core, DB access)
├── EduTrack.Tests/           # Unit and integration tests
├── .editorconfig
├── .gitignore
├── README.md
└── LICENSE

```

## ⚙️ Getting Started

Follow these steps to run EduTrack locally:

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [EF Core Tools](https://learn.microsoft.com/ef/core/cli/dotnet) (if not installed: `dotnet tool install --global dotnet-ef`)

### 🔧 Setup Instructions

__1. Clone the repository__

```bash
git clone https://github.com/mahedee/clean-arch-pro.git
cd EduTrack
```

__2. Setup PostgreSQL__

Install PostgreSQL (if not already):

- [Download for Windows](https://www.postgresql.org/download/windows/)

Update `appsettings.Development.json` in `EduTrack.Api` with your PostgreSQL credentials:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword"
}
```

__3. Apply Migrations__

```bash
cd src/EduTrack.Api
dotnet ef database update
```

__4. Run the Application__

```bash
dotnet run --project EduTrack.Api
```

- Or, you can run the project using Visual Studio or your preferred IDE.
- Make sure to set `EduTrack.Api` as the startup project.
- Go to clean-arch-pro/EduTrack and click on EduTrack.sln to open the solution in Visual Studio.
- Build and run the solution.

__5. Browse Swagger__
👉 [https://localhost:7050/index.html](https://localhost:7050/index.html)

---

Here's the rewritten and improved version of your contribution section with relevant issue templates and pull request guideline links included:

---

## ✅ Contributing

We welcome contributions from the community! Whether you're fixing a bug, adding a feature, or improving documentation, your help is appreciated.

🔧 To get started, please read our [Contribution Guidelines](CONTRIBUTING.md).

### 🙌 Ways to Contribute

* 🐞 [Report a bug](https://github.com/mahedee/clean-arch-pro/issues/new?template=bug_report.md)
* 💡 [Suggest a new feature](https://github.com/mahedee/clean-arch-pro/issues/new?template=feature_request.md)
* 📝 [Improve the documentation](https://github.com/mahedee/clean-arch-pro/issues/new?template=documentation_improvement.md)
* 🔁 [Submit a pull request](pullrequest-guidelines.md)

---

Here's the rewritten section for general issues, excluding the 4 specific categories you've already mentioned:

---

### 🛠️ General Issues

If you have any concerns, suggestions, or questions that don't fall into the categories of bugs, features, or documentation improvements, please [open a general issue](https://github.com/mahedee/clean-arch-pro/issues/new?template=general_issue.md).

Feel free to provide as much detail as possible to help us understand your request or discussion point. Whether it’s feedback, a discussion about potential improvements, or something else entirely, we welcome your input!


## 📜 License

This project is licensed under the [MIT License](LICENSE).

---

## ⭐ Found it useful?

If you find this project helpful or inspiring, don’t forget to **star ⭐** the repository!

---

## 📬 Contact

Maintainer: [Mahedee Hasan](https://github.com/mahedee)  
Email: `mahedee.hasan@gmail.com`

---