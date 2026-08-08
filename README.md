# EduTrack — Education Management System


[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) [![Multi Database](https://img.shields.io/badge/Database-Multi--Provider-orange)](docs/setup/database-setup.md) [![CQRS](https://img.shields.io/badge/Pattern-CQRS-blueviolet)](https://martinfowler.com/bliki/CQRS.html) [![DDD](https://img.shields.io/badge/Design-Domain%20Driven-red)](https://martinfowler.com/tags/domain%20driven%20design.html) [![GitHub Issues](https://img.shields.io/github/issues/mahedee/clean-arch-pro)](https://github.com/mahedee/clean-arch-pro/issues) [![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com/mahedee/clean-arch-pro/actions)

---

**EduTrack** is a comprehensive **enterprise-grade education management system** built with **Clean Architecture** principles, **Domain-Driven Design (DDD)**, and **modern .NET 10** technologies with an **Angular frontend**. Designed for educational institutions of all sizes, from small schools to large universities.

![EduTrack Dashboard](docs/images/edutrack-dashboard_v02.png)

---

## 📋 Table of Contents

- [🎯 Project Vision](#-project-vision)
- [🎓 Key Business Features](#-key-business-features)
- [⚙️ Key Technical Features](#️-key-technical-features)
- [🚀 Quick Start Guide](#-quick-start-guide)
- [⚙️ Prerequisites](#prerequisites)
- [🗄️ Database Setup](#️-database-setup)
- [🚀 Running the Backend](#-running-the-backend)
- [🅰️ Running the Frontend](#running-the-frontend)
- [🧪 Running Tests](#running-tests)
- [🛠️ Developer Guide](#️-developer-guide)
- [🧰 Technology Stack](#-technology-stack)
- [🤝 Contributing](#-contributing)
- [📜 License](#-license)
- [📞 Support & Community](#-support--community)
- [👨‍💻 About the Maintainer](#-about-the-maintainer)

---

## 🎯 **Project Vision**

> *Building the next generation of education management software with enterprise-grade architecture, multi-database support, and scalable design patterns.*

### **🎪 Live Demo & Portfolio**
- 🌐 **Live Demo**: *Coming Soon*
- 🏗️ **Architecture Documentation**: Full guide coming soon. For an immediate overview, please refer to the [Developer's Guide](DEVELOPER-GUIDE.md) and the [Clean Architecture Guide](docs/architecture/clean-architecture-overview.md)

> 🔥 **If this project helps you, please give it a star ⭐ - It means a lot to the team!**

---

## 🎓 Key Business Features

- 📚 **Student Lifecycle Management** — Admission, enrollment, progression, and graduation
- 🏫 **Academic Structure** — Departments, programs, courses, and scheduling with conflict detection
- 👨‍🏫 **Faculty Management** — Profiles, academic titles, workload tracking, and employment workflows
- 📊 **Grades & Assessments** — Flexible grading schemes, assessment weightings, and transcript generation
- 🔐 **Access Control** — Role-based and permission-based authorization per user or role
- 📈 **Reporting & Analytics** — Dashboards, custom reports, and PDF/Excel/CSV exports

📄 [See full business features →](docs/key-business-features.md)

---

## ⚙️ Key Technical Features

- 🏗️ **Clean Architecture** with Domain-Driven Design and strict layer separation
- ⚡ **CQRS** via MediatR — all commands and queries handled through a pipeline
- 🗄️ **Multi-Database Support** — PostgreSQL (primary), SQL Server, Oracle, SQLite
- 🔐 **JWT Authentication** with role-based and permission-based authorization
- 🧪 **Comprehensive Testing** — Domain (≥95%), Application (≥80%), and integration tests
- 🚀 **CI/CD Ready** — GitHub Actions, Docker multi-stage builds, OpenAPI/Swagger

📄 [See full technical features →](docs/key-technical-features.md)

---

##  Quick Start Guide

## Prerequisites

Before you begin, ensure you have the following installed:

- ✅ **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (10.0 or later)
- ✅ **Database Server** (choose one):
  - [PostgreSQL 14+](https://www.postgresql.org/download/) *(Recommended)*
- ✅ **Development IDE**:
  - [Visual Studio 2026+](https://visualstudio.microsoft.com/) *(Recommended)*
  - [VS Code](https://code.visualstudio.com/) with C# extension
- ✅ **[Git](https://git-scm.com/)** for version control
- ✅ **[Node.js 18+](https://nodejs.org/)** (for frontend development)

---

## 🗄️ Database Setup

### PostgreSQL (Recommended)

**1. Install PostgreSQL**

- **Windows:** Download the installer from [postgresql.org/download](https://www.postgresql.org/download/) or use Chocolatey:
  ```bash
  choco install postgresql
  ```
- **macOS:**
  ```bash
  brew install postgresql
  ```
- **Linux (Ubuntu/Debian):**
  ```bash
  sudo apt install postgresql postgresql-contrib
  ```

**2. Create the database**

```bash
creatdb EduTrackDb
```

Or using `psql`:

```sql
CREATE DATABASE "EduTrackDb";
```

**3. Configure the connection string**

Create `appsettings.Development.json` inside `backend/EduTrack/src/EduTrack.Api/` with your credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword;"
  }
}
```

> 📖 [Configure PostgreSQL with pgAdmin 4](docs/how-to-configure-postgressql-in-pgadmin.md) — step-by-step guide for setting up the database using the pgAdmin UI.

### Other Supported Databases

<details>
<summary><strong>SQL Server</strong></summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EduTrackDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

</details>

<details>
<summary><strong>Oracle</strong></summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "User Id=youruser;Password=yourpassword;Data Source=localhost:1521/XEPDB1"
  }
}
```

</details>

---

## 🚀 Running the Backend

### 1. Clone the repository

```bash
git clone https://github.com/mahedee/clean-arch-pro.git
cd clean-arch-pro
```

### 2. Configure the database connection

Create `appsettings.Development.json` inside `backend/EduTrack/src/EduTrack.Api/` (this file is git-ignored). See the [Database Setup](#database-setup) section above for connection string examples.

> 💡 Use `appsettings.Production.json` as a template.

### 3. Restore packages

```bash
cd backend/EduTrack
dotnet restore
```

### 4. Apply database migrations

```bash
cd backend/EduTrack/src/EduTrack.Api
dotnet ef database update
```

> If `dotnet ef` is not found, install it: `dotnet tool install --global dotnet-ef`

### 5. Run the API

```bash
cd backend/EduTrack/src/EduTrack.Api
dotnet run
```

| URL | Description |
|-----|-------------|
| `http://localhost:6100` | API base URL |
| `http://localhost:6100/swagger` | Swagger / OpenAPI UI |

**Run with a specific launch profile:**

```bash
dotnet run --launch-profile http        # HTTP only
dotnet run --launch-profile https       # HTTPS + HTTP
dotnet run --launch-profile Staging     # Staging environment
dotnet run --launch-profile Production  # Production environment
```

**Using Visual Studio or VS Code:**

- **Visual Studio:** Open `backend/EduTrack/EduTrack.sln`, select `EduTrack.Api`, and press **F5**.
- **VS Code:** Open the `backend/EduTrack` folder and use the **Run and Debug** panel (a launch configuration is included in `.vscode/`).

### Troubleshooting

| Problem | Fix |
|---------|-----|
| `connection refused` on DB | Ensure PostgreSQL is running and credentials are correct in `appsettings.Development.json` |
| Port 6100 already in use | Change `applicationUrl` in `Properties/launchSettings.json` or stop the conflicting process |
| Pending migrations error | Run `dotnet ef database update` from the `EduTrack.Api` folder |
| `dotnet ef` not found | Run `dotnet tool install --global dotnet-ef` |

---

## Running the Frontend

### 1. Install dependencies

```bash
cd frontend/edutrack-ui
npm install
```

### 2. Start the development server

```bash
cd frontend/edutrack-ui
npm start
```

The app is available at **`http://localhost:4200`**. The dev server proxies API calls to `http://localhost:6100` (backend must be running). Changes to source files are reflected automatically via hot reload.

**Run on a custom port:**

```bash
ng serve --port 4201
```

### 3. Build for production

```bash
npm run build
# or:
ng build --configuration production
```

Output is generated in `dist/edutrack-ui/`.

### 4. Run with Server-Side Rendering (SSR)

```bash
npm run build
node dist/edutrack-ui/server/server.mjs
```

---

### 5. Run Everything with a Single Script

The easiest way to start both the backend and frontend together is to use the provided PowerShell scripts from the repository root.

**Start both applications at once:**

```powershell
.\scripts\run-edutrack-all.ps1
```

This opens each application in its own terminal window:

| Script | What it does |
|--------|-------------|
| `scripts\run-edutrack-all.ps1` | Launches backend + frontend in separate windows |
| `scripts\run-edutrack-backend.ps1` | Builds and starts the backend API only |
| `scripts\run-edutrack-frontend.ps1` | Installs dependencies and starts the Angular dev server only |

Once running, the following URLs are available:

| URL | Description |
|-----|-------------|
| `http://localhost:6100` | Backend API |
| `http://localhost:6100/swagger` | Swagger / OpenAPI UI |
| `http://localhost:4200` | Angular frontend |

> **Prerequisites:** PowerShell 7+ must be installed. .NET 10 SDK and Node.js must be on the system PATH.

Press **Ctrl+C** in each window to stop the respective application.

### Troubleshooting

| Problem | Fix |
|---------|-----|
| `ng: command not found` | Run `npm install -g @angular/cli` |
| `npm install` fails | Delete `node_modules/` and `package-lock.json`, then re-run `npm install` |
| API calls return 404 or CORS errors | Ensure the backend is running on `http://localhost:6100` |
| Port 4200 already in use | Use `ng serve --port 4201` or stop the conflicting process |

---

## 🧪 Running Tests

### Backend tests

```bash
# Run all tests
cd backend/EduTrack
dotnet test

# Run a specific test project
dotnet test tests/EduTrack.Domain.UnitTests/

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

See [backend unit tests](docs/backend-unit-tests.md) and [backend test coverage](docs/backend-test-coverage.md) for details.

### Frontend tests

```bash
cd frontend/edutrack-ui

# Run tests in watch mode
npm test

# Run tests headless (CI)
npm run test:ci
```

See [frontend unit tests](docs/frontend-unit-tests.md) and [frontend test coverage](docs/frontend-test-coverage.md) for details.

---

## 🛠️ Developer Guide

For a full developer reference — including project structure, architecture deep-dive, coding conventions, logging, and configuration — see the **[Developer's Guide](DEVELOPER-GUIDE.md)**.

### 📁 Project Structure

The solution follows a four-layer Clean Architecture layout. See **[Developer's Guide → Project Structure](DEVELOPER-GUIDE.md)** for the full breakdown.

### 🏗️ Architecture

EduTrack is built on Clean Architecture, DDD, and CQRS. See the **[Clean Architecture Overview](docs/architecture/clean-architecture-overview.md)** for layer dependencies, design decisions, and key patterns.

### ✨ Adding a New Feature

Adding a new feature follows a consistent CQRS workflow across Domain → Application → Infrastructure → API → Tests. See the **[API Implementation Guide](docs/api-implementation-guide.md)** for a worked example.

### 🗄️ Database migrations

```bash
# Add a new migration
dotnet ef migrations add "MigrationName" \
  --project src/EduTrack.Infrastructure \
  --startup-project src/EduTrack.Api

# Apply migrations
dotnet ef database update --project src/EduTrack.Api

# Drop database (development only)
dotnet ef database drop --project src/EduTrack.Api --force
```

### 🔄 Git Workflow

```bash
# Create a feature branch
git checkout -b feature/your-feature-name

# Commit with a descriptive message referencing the issue
git commit -m "Add student CRUD operations - Fixes #18"

# Push and open a pull request
git push origin feature/your-feature-name
```

See [CONTRIBUTING.md](CONTRIBUTING.md) and [pull request guidelines](pullrequest-guidelines.md) for the full process.

### 📝 Coding Conventions

- **Entities:** singular noun — `Student`, `Course`
- **DTOs:** suffix with `Dto` — `StudentDto`
- **Interfaces:** prefix with `I` — `IStudentRepository`
- **Logging:** structured logging via `ILogger<T>`

---

## 🧰 **Technology Stack**

### **Backend Technologies**
| Technology | Version | Purpose | Documentation |
|------------|---------|---------|---------------|
| **.NET** | 10.0 | Core framework | [📖 .NET Docs](https://docs.microsoft.com/en-us/dotnet/) |
| **ASP.NET Core** | 10.0 | Web API framework | [📖 ASP.NET Docs](https://docs.microsoft.com/en-us/aspnet/core/) |
| **Entity Framework Core** | 10.0 | ORM & Data Access | [📖 EF Core Docs](https://docs.microsoft.com/en-us/ef/core/) |
| **MediatR** | 12.x | CQRS & Mediator pattern | [📖 MediatR](https://github.com/jbogard/MediatR) |
| **AutoMapper** | 12.x | Object mapping | [📖 AutoMapper](https://automapper.org/) |
| **FluentValidation** | 11.x | Input validation | [📖 FluentValidation](https://fluentvalidation.net/) |
| **Serilog** | 3.x | Structured logging | [📖 Serilog](https://serilog.net/) |
| **xUnit** | 2.x | Unit testing framework | [📖 xUnit](https://xunit.net/) |

### **Database Support**
| Database | Status | Performance | Use Case |
|----------|--------|-------------|----------|
| **PostgreSQL** | ✅ Primary | Excellent | General purpose, JSONB support |
| **SQL Server** | ✅ Supported | Excellent | Enterprise environments |
| **Oracle** | ✅ Enterprise | Good | Large enterprise systems |
| **SQLite** | 🔄 Testing Only | Good | Development & testing |

### **Frontend Technologies**

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 18.x | Frontend framework |
| Angular Material | 18.x | UI components |
| TypeScript | 5.x | Type safety |
| RxJS | 7.x | Reactive programming |

---

## 🤝 **Contributing**

We welcome contributions from developers of all skill levels! Whether you're fixing bugs, adding features, improving documentation, or sharing ideas, your contribution matters. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before submitting a pull request.

### **🎯 How to Contribute**

#### **🐛 Report Issues**
Found a bug or have a suggestion? Please check existing issues first, then:
- **[🐞 Report a Bug](https://github.com/mahedee/clean-arch-pro/issues/new?template=bug_report.md)**
- **[💡 Request a Feature](https://github.com/mahedee/clean-arch-pro/issues/new?template=feature_request.md)**
- **[📝 Improve Documentation](https://github.com/mahedee/clean-arch-pro/issues/new?template=documentation_improvement.md)**
- **[❓ Ask a Question](https://github.com/mahedee/clean-arch-pro/issues/new?template=general_issue.md)**


#### **🔧 Code Contributions**
1. **Fork the repository** and create your feature branch
2. **Follow our coding standards** (see [CONTRIBUTING.md](CONTRIBUTING.md))
3. **Write tests** for your changes
4. **Submit a pull request** following our [PR Guidelines](pullrequest-guidelines.md)

#### **📖 Documentation Contributions**
- Improve existing documentation
- Add code examples and tutorials
- Translate documentation to other languages
- Create video tutorials or blog posts

### **🏆 Contributors**

Thanks to all the amazing people who have contributed to this project!

<!-- Contributors will be added here automatically -->
<a href="https://github.com/mahedee/clean-arch-pro/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=mahedee/clean-arch-pro" />
</a>

---


## 📜 **License**

This project is licensed under the **[MIT License](LICENSE)** - see the LICENSE file for details.

### **What this means:**
- ✅ **Commercial use** - Use in commercial projects
- ✅ **Modification** - Modify the code as needed
- ✅ **Distribution** - Distribute your modifications
- ✅ **Private use** - Use privately without restrictions
- ⚠️ **Attribution** - Include original license and copyright notice


---

## 📞 **Support & Community**

### **🆘 Getting Help**
- **📖 Documentation**: Check our comprehensive docs first
- **🐛 Issues**: [GitHub Issues](https://github.com/mahedee/clean-arch-pro/issues)
- **💬 Discussions**: [GitHub Discussions](https://github.com/mahedee/clean-arch-pro/discussions)
- **📧 Email**: [mahedee.hasan@gmail.com](mailto:mahedee.hasan@gmail.com)

### **🌟 Show Your Support**
If this project helps you build better applications:
- ⭐ **Star the repository** on GitHub
- 🐦 **Share on social media** (Twitter, LinkedIn)
- 📝 **Write a blog post** about your experience
- 🗣️ **Tell your colleagues** about EduTrack

### **📊 Project Stats**
- 📈 **GitHub Stars**: ![GitHub Repo stars](https://img.shields.io/github/stars/mahedee/clean-arch-pro?style=social)
- 🍴 **Forks**: ![GitHub forks](https://img.shields.io/github/forks/mahedee/clean-arch-pro?style=social)
- 👀 **Watchers**: ![GitHub watchers](https://img.shields.io/github/watchers/mahedee/clean-arch-pro?style=social)
- 📝 **Issues**: ![GitHub issues](https://img.shields.io/github/issues/mahedee/clean-arch-pro)
- 🔄 **Pull Requests**: ![GitHub pull requests](https://img.shields.io/github/issues-pr/mahedee/clean-arch-pro)

---

## 👨‍💻 About the Maintainer

**[Mahedee Hasan](https://github.com/mahedee)** — *Software Architect*

- 🏢 **Experience:** 17+ years in enterprise software development
- 🌐 **Website:** [mahedee.net](https://mahedee.net)
- 💼 **LinkedIn:** [linkedin.com/in/mahedee](https://linkedin.com/in/mahedee)
- 🐦 **Twitter:** [@mahedee_hasan](https://twitter.com/mahedee_hasan)

---

<div align="center">

### **🎉 Thank you for choosing EduTrack!**

*Building the future of education management, one commit at a time.*

[![Made with ❤️](https://img.shields.io/badge/Made%20with-❤️-red.svg)](https://github.com/mahedee/clean-arch-pro) [![Contributors Welcome](https://img.shields.io/badge/Contributors-Welcome-blue.svg)](CONTRIBUTING.md) [![PRs Welcome](https://img.shields.io/badge/PRs-Welcome-brightgreen.svg)](pullrequest-guidelines.md)

---

**⭐ Don't forget to star the repository if you found it helpful! ⭐**

</div>
