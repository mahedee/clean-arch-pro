# 🎓 EduTrack - Enterprise Education Management System

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/) [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE) [![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-green)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) [![Multi Database](https://img.shields.io/badge/Database-Multi--Provider-orange)](docs/setup/database-setup.md) [![CQRS](https://img.shields.io/badge/Pattern-CQRS-blueviolet)](https://martinfowler.com/bliki/CQRS.html) [![DDD](https://img.shields.io/badge/Design-Domain%20Driven-red)](https://martinfowler.com/tags/domain%20driven%20design.html) [![GitHub Issues](https://img.shields.io/github/issues/mahedee/clean-arch-pro)](https://github.com/mahedee/clean-arch-pro/issues) [![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com/mahedee/clean-arch-pro/actions)

---

**EduTrack** is a comprehensive **enterprise-grade education management system** built with **Clean Architecture** principles, **Domain-Driven Design (DDD)**, and **modern .NET 10** technologies with an **Angular frontend**. Designed for educational institutions of all sizes, from small schools to large universities.

![](/docs/images/edutrack-dashboard.png)

## 🎯 **Project Vision**

> *Building the next generation of education management software with enterprise-grade architecture, multi-database support, and scalable design patterns.*

### **🎪 Live Demo & Portfolio**
- 🌐 **Live Demo**: *Coming Soon*
- 📊 **Project Metrics**: [View Current Progress](docs/task-list.md)
- 🏗️ **Architecture Documentation**: [Clean Architecture Guide](docs/architecture/clean-architecture-overview.md)

> 🔥 **If this project helps you, please give it a star ⭐ - It means a lot to the team!**

---

## ✨ **Key Features & Capabilities**

### **🏗️ Architecture Excellence**
- ✅ **Clean Architecture** with strict dependency inversion
- ✅ **Domain-Driven Design (DDD)** with rich domain models
- ✅ **CQRS Pattern** with MediatR for scalable request handling
- ✅ **Repository & Unit of Work** patterns for data consistency
- ✅ **Dependency Injection** with built-in .NET DI container
- ✅ **Event-Driven Architecture** with domain events

### **🎓 Education Management Features**
- 📚 **Student Lifecycle Management** (Admission to Graduation)
- 🏫 **Academic Structure** (Departments, Programs, Courses)
- 📅 **Advanced Scheduling System** with conflict resolution
- 📊 **Grade & Assessment Management** with flexible grading schemes
- 👨‍🏫 **Faculty Management** with workload tracking
- 💰 **Financial Management** (Fees, Scholarships, Financial Aid)
- 📈 **Analytics & Reporting** with real-time dashboards
- 🔐 **Dynamic Permission System** with role-based access control

### **🛠️ Technical Features**
- 🗄️ **Multi-Database Support** (PostgreSQL, SQL Server, Oracle)
- 🔒 **JWT Authentication** with refresh token mechanism
- 📝 **Comprehensive Logging** with Serilog
- 🧪 **Extensive Testing** (Unit, Integration, E2E)
- 🐳 **Docker Support** with multi-stage builds
- 🚀 **CI/CD Ready** with GitHub Actions
- 📖 **OpenAPI/Swagger** documentation
- ⚡ **Performance Optimized** with caching and query optimization

### **🎨 Frontend & Integration**
- 🅰️ **Angular Frontend** with Material Design
- 📱 **Responsive Design** for mobile and desktop
- 🔄 **Real-time Updates** with SignalR
- 🌐 **RESTful APIs** with proper HTTP status codes
- 📄 **Export Capabilities** (PDF, Excel, CSV)

---


## 🚀 **Quick Start Guide**

### **Prerequisites**

Before you begin, ensure you have the following installed:

- ✅ **[.NET 10 SDK](https://dotnet.microsoft.com/download)** (10.0 or later)
- ✅ **Database Server** (choose one):
  - [PostgreSQL 14+](https://www.postgresql.org/download/) *(Recommended)*
- ✅ **Development IDE**:
  - [Visual Studio 2026+](https://visualstudio.microsoft.com/) *(Recommended)*
  - [VS Code](https://code.visualstudio.com/) with C# extension
- ✅ **[Git](https://git-scm.com/)** for version control
- ✅ **[Node.js 18+](https://nodejs.org/)** (for frontend development)

### **⚡ 5-Minute Setup**

#### **1. Clone the Repository**
```bash
git clone https://github.com/mahedee/clean-arch-pro.git
cd clean-arch-pro
```

#### **2. Database Setup**
Choose your preferred database provider:

<details>
<summary>🐘 <strong>PostgreSQL Setup (Recommended)</strong></summary>

```bash
# Install PostgreSQL (Windows with Chocolatey)
choco install postgresql

# Or download from: https://www.postgresql.org/download/

# Create database
createdb EduTrackDb

# Update connection string in appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword;"
  }
}
```
</details>

<details>
<summary>🔷 <strong>SQL Server Setup</strong></summary>

```bash
# SQL Server connection string
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EduTrackDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```
</details>

#### **3. Backend Setup**
```bash
# Navigate to backend solution
cd backend/EduTrack

# Restore packages
dotnet restore

# Apply database migrations
cd src/EduTrack.Api
dotnet ef database update

# Run the application
dotnet run

# 🎉 API will be available at: http://localhost:6100
```

#### **4. Verify Backend**
- 🌐 **Swagger UI**: http://localhost:6100/swagger
---

#### **5. Frontend setup**
```bash
# Navigate to frontend directory
cd src/frontend/edutrack-ui

# Install dependencies
npm install

# Run the Angular application
npm start

# 🎉 Frontend will be available at: http://localhost:4200

```

#### **6. Verify Frontend**
- 🌐 **Frontend UI**: http://localhost:4200
---

## 🛠️ **Development Workflow**

### **🔄 Git Workflow**
```bash
# Create feature branch
git checkout -b feature/student-management

# Make your changes and commit
git add .
git commit -m "Add student CRUD operations - Fixes #18"

# Push and create pull request
git push origin feature/student-management
```

### **🧪 Running Tests**
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/EduTrack.Domain.UnitTests/

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### **🗄️ Database Operations**
```bash
# Add new migration
dotnet ef migrations add "AddStudentEntity" --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Update database
dotnet ef database update --project src/EduTrack.Api

# Drop database (development only)
dotnet ef database drop --project src/EduTrack.Api --force
```
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

### **Frontend Technologies (Planned)**
| Technology | Version | Purpose | Status |
|------------|---------|---------|--------|
| **Angular** | 17.x | Frontend framework | 🔄 Planned |
| **Angular Material** | 17.x | UI components | 🔄 Planned |
| **TypeScript** | 5.x | Type safety | 🔄 Planned |
| **RxJS** | 7.x | Reactive programming | 🔄 Planned |

---

## 🤝 **Contributing**

We welcome contributions from developers of all skill levels! Whether you're fixing bugs, adding features, improving documentation, or sharing ideas, your contribution matters.

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

### **🎖️ Recognition**

We believe in recognizing contributors:
- **🥇 Top Contributors**: Featured in README and social media
- **🏆 Monthly Recognition**: Special mentions in project updates
- **📜 Contributor Certificate**: Digital certificates for significant contributions

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

## 👨‍💻 **About the Maintainer**

**[Mahedee Hasan](https://github.com/mahedee)** - *Software Architect*

- 🏢 **Experience**: 17+ years in enterprise software development
- 🌐 **Website**: [mahedee.net](https://mahedee.net)
- 💼 **LinkedIn**: [linkedin.com/in/mahedee](https://linkedin.com/in/mahedee)
- 🐦 **Twitter**: [@mahedee_hasan](https://twitter.com/mahedee_hasan)

---

<div align="center">

### **🎉 Thank you for choosing EduTrack!**

*Building the future of education management, one commit at a time.*

[![Made with ❤️](https://img.shields.io/badge/Made%20with-❤️-red.svg)](https://github.com/mahedee/clean-arch-pro)
[![Contributors Welcome](https://img.shields.io/badge/Contributors-Welcome-blue.svg)](CONTRIBUTING.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-Welcome-brightgreen.svg)](pullrequest-guidelines.md)

---

**⭐ Don't forget to star the repository if you found it helpful! ⭐**

</div>
