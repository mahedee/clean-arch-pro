# EduTrack - Development Environment Setup Guide

This guide will help you set up the complete development environment for the EduTrack Clean Architecture project.

## 🎯 Prerequisites Overview

Before starting, ensure you have the following tools and technologies:

- **Backend**: ASP.NET Core 8+ with C#
- **Frontend**: Angular 17+
- **Database**: PostgreSQL with pgAdmin
- **IDE/Editors**: Visual Studio 2022 and/or VS Code
- **Version Control**: Git

---

## 🛠️ Step 1: Install Core Development Tools

### **1.1 Git Version Control**
```bash
# Download and install Git from: https://git-scm.com/downloads
# Verify installation
git --version
```

**Configuration:**
```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### **1.2 Visual Studio 2022 (Recommended for Backend)**
- Download from: [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)
- **Required Workloads:**
  - ASP.NET and web development
  - .NET desktop development
  - Data storage and processing

**Essential Extensions:**
- ReSharper (optional, but recommended)
- GitLens
- SonarLint
- NuGet Package Manager

### **1.3 Visual Studio Code (Alternative/Frontend)**
- Download from: [VS Code](https://code.visualstudio.com/)

**Essential Extensions:**
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.vscode-dotnet-runtime",
    "ms-vscode.vscode-json",
    "angular.ng-template",
    "ms-vscode.vscode-typescript-next",
    "esbenp.prettier-vscode",
    "bradlc.vscode-tailwindcss",
    "ms-vscode.powershell",
    "gitpod.gitpod-desktop"
  ]
}
```

---

## 🌐 Step 2: Install .NET Development Environment

### **2.1 .NET 8 SDK**
```bash
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Verify installation
dotnet --version
# Should show 8.0.x or higher
```

### **2.2 .NET CLI Tools**
```bash
# Install Entity Framework Core tools globally
dotnet tool install --global dotnet-ef

# Install development certificates for HTTPS
dotnet dev-certs https --trust

# Verify EF tools installation
dotnet ef --version
```

### **2.3 Additional .NET Tools**
```bash
# Install useful global tools
dotnet tool install --global dotnet-outdated-tool
dotnet tool install --global dotnet-sonarscanner
dotnet tool install --global dotnet-reportgenerator-globaltool

# List installed tools
dotnet tool list --global
```

---

## 🗄️ Step 3: PostgreSQL Database Setup

### **3.1 Install PostgreSQL**

**Windows:**
- Download from: [PostgreSQL Downloads](https://www.postgresql.org/download/windows/)
- Use the installer with these settings:
  - **Port**: 5432 (default)
  - **Superuser**: postgres
  - **Password**: Choose a strong password and remember it

**Alternative - Using Chocolatey:**
```powershell
# Install Chocolatey if not already installed
# Then install PostgreSQL
choco install postgresql
```

**Verify Installation:**
```bash
# Check PostgreSQL version
psql --version

# Connect to default database
psql -U postgres -h localhost
```

### **3.2 Install pgAdmin**
- Download from: [pgAdmin](https://www.pgadmin.org/download/)
- Or install via PostgreSQL installer (usually included)

**Initial Configuration:**
1. Open pgAdmin
2. Add New Server:
   - **Name**: EduTrack Local
   - **Host**: localhost
   - **Port**: 5432
   - **Username**: postgres
   - **Password**: [your password]

### **3.3 Create EduTrack Database**

**Using pgAdmin:**
1. Right-click "Databases" → "Create" → "Database"
2. **Database name**: `EduTrackDb`
3. **Owner**: postgres
4. Click "Save"

**Using Command Line:**
```sql
-- Connect to PostgreSQL
psql -U postgres -h localhost

-- Create database
CREATE DATABASE "EduTrackDb";

-- Create application user (optional but recommended)
CREATE USER edutrack_user WITH PASSWORD 'your_secure_password';
GRANT ALL PRIVILEGES ON DATABASE "EduTrackDb" TO edutrack_user;

-- List databases to verify
\l

-- Exit
\q
```

---

## 🅰️ Step 4: Angular Development Environment

### **4.1 Install Node.js**
- Download LTS version from: [Node.js](https://nodejs.org/)
- **Recommended version**: 18.x or 20.x LTS

**Verify Installation:**
```bash
node --version
npm --version
```

### **4.2 Install Angular CLI**
```bash
# Install Angular CLI globally
npm install -g @angular/cli

# Verify installation
ng version
```

### **4.3 Install Additional Node.js Tools**
```bash
# Package managers
npm install -g yarn pnpm

# Development tools
npm install -g typescript
npm install -g @angular/language-service
npm install -g webpack-bundle-analyzer

# Verify TypeScript
tsc --version
```

---

## 🔧 Step 5: IDE Configuration

### **5.1 Visual Studio 2022 Configuration**

**Project Templates:**
- Ensure ASP.NET Core project templates are available
- Install NuGet Package Manager if not present

**Debugging Configuration:**
```json
// launchSettings.json example
{
  "profiles": {
    "EduTrack.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "launchUrl": "swagger",
      "applicationUrl": "https://localhost:7050;http://localhost:5050",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

### **5.2 VS Code Configuration**

**Workspace Settings (.vscode/settings.json):**
```json
{
  "dotnet.defaultSolution": "backend/EduTrack/EduTrack.sln",
  "omnisharp.enableEditorConfigSupport": true,
  "omnisharp.enableImportCompletion": true,
  "typescript.preferences.importModuleSpecifier": "relative",
  "angular.enable-strict-mode-prompt": false,
  "files.exclude": {
    "**/bin": true,
    "**/obj": true,
    "**/node_modules": true
  },
  "search.exclude": {
    "**/node_modules": true,
    "**/bin": true,
    "**/obj": true,
    "**/.git": true
  }
}
```

**Launch Configuration (.vscode/launch.json):**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/backend/EduTrack/src/EduTrack.Api/bin/Debug/net8.0/EduTrack.Api.dll",
      "args": [],
      "cwd": "${workspaceFolder}/backend/EduTrack/src/EduTrack.Api",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "sourceFileMap": {
        "/Views": "${workspaceFolder}/Views"
      }
    },
    {
      "name": "Angular Development Server",
      "type": "node",
      "request": "launch",
      "program": "${workspaceFolder}/frontend/node_modules/@angular/cli/bin/ng",
      "args": ["serve"],
      "cwd": "${workspaceFolder}/frontend"
    }
  ]
}
```

**Tasks Configuration (.vscode/tasks.json):**
```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/backend/EduTrack/src/EduTrack.Api/EduTrack.Api.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "publish",
      "command": "dotnet",
      "type": "process",
      "args": [
        "publish",
        "${workspaceFolder}/backend/EduTrack/src/EduTrack.Api/EduTrack.Api.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "${workspaceFolder}/backend/EduTrack/src/EduTrack.Api/EduTrack.Api.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    }
  ]
}
```

---

## 🌍 Step 6: Environment Variables & Configuration

### **6.1 Backend Configuration**

**appsettings.Development.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200", "https://localhost:4200"]
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-for-development-only",
    "Issuer": "EduTrack.Api",
    "Audience": "EduTrack.Client",
    "ExpirationInMinutes": 60
  }
}
```

### **6.2 Frontend Configuration**

**environment.development.ts:**
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7050/api',
  enableLogging: true,
  version: '1.0.0-dev'
};
```

### **6.3 System Environment Variables**

**Windows PowerShell:**
```powershell
# Set environment variables for development
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"

# Verify
echo $env:ASPNETCORE_ENVIRONMENT
```

---

## 📦 Step 7: Package Management Setup

### **7.1 Backend NuGet Packages**

**Package Sources:**
```bash
# Add NuGet sources if needed
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# List configured sources
dotnet nuget list source
```

### **7.2 Frontend npm Configuration**

**Package.json scripts section example:**
```json
{
  "scripts": {
    "ng": "ng",
    "start": "ng serve",
    "start:dev": "ng serve --configuration development",
    "build": "ng build",
    "build:prod": "ng build --configuration production",
    "watch": "ng build --watch --configuration development",
    "test": "ng test",
    "test:coverage": "ng test --code-coverage",
    "lint": "ng lint",
    "e2e": "ng e2e"
  }
}
```

---

## 🔍 Step 8: Development Tools & Extensions

### **8.1 Database Tools**

**pgAdmin Extensions:**
- Query Tool with syntax highlighting
- pgAgent for scheduled tasks
- Database monitoring tools

**Alternative Database Tools:**
- **DBeaver**: Free universal database tool
- **DataGrip**: JetBrains database IDE (paid)
- **Azure Data Studio**: Microsoft's cross-platform database tool

### **8.2 API Development Tools**

**Postman:**
- Download from: [Postman](https://www.postman.com/downloads/)
- Import API collections for testing
- Environment configuration for different stages

**Alternative Tools:**
- **Insomnia**: Lightweight REST client
- **Thunder Client**: VS Code extension for API testing
- **REST Client**: VS Code extension for HTTP requests

### **8.3 Code Quality Tools**

```bash
# Install SonarQube scanner
dotnet tool install --global dotnet-sonarscanner

# Install code coverage tools
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild
```

---

## ✅ Step 9: Verification & Testing

### **9.1 Backend Verification**

```bash
# Navigate to backend project
cd backend/EduTrack

# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Check for outdated packages
dotnet outdated
```

### **9.2 Database Connectivity Test**

```bash
# Test database connection
dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Create a test migration (optional)
dotnet ef migrations add TestConnection --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
```

### **9.3 Frontend Verification**

```bash
# Navigate to frontend project
cd frontend

# Install dependencies
npm install

# Check for vulnerabilities
npm audit

# Test build
ng build

# Run linting
ng lint
```

---

## 🚀 Step 10: Final Setup Steps

### **10.1 Clone and Setup Project**

```bash
# Clone the repository
git clone https://github.com/mahedee/clean-arch-pro.git
cd clean-arch-pro

# Setup backend
cd backend/EduTrack
dotnet restore
dotnet build

# Setup frontend (when available)
cd ../../frontend
npm install
```

### **10.2 Database Migration**

```bash
# Apply database migrations
cd backend/EduTrack
dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
```

### **10.3 Start Development Servers**

**Terminal 1 - Backend:**
```bash
cd backend/EduTrack/src/EduTrack.Api
dotnet run
```

**Terminal 2 - Frontend:**
```bash
cd frontend
ng serve
```

---

## 🛠️ Troubleshooting Common Issues

### **Issue 1: PostgreSQL Connection Problems**

**Symptoms:** Cannot connect to PostgreSQL database

**Solutions:**
```bash
# Check if PostgreSQL is running
# Windows
net start postgresql-x64-13

# Check connection
psql -U postgres -h localhost -p 5432

# Verify connection string in appsettings.json
```

### **Issue 2: .NET Core Trust Issues**

**Symptoms:** HTTPS certificate errors

**Solutions:**
```bash
# Trust development certificates
dotnet dev-certs https --trust

# Clear certificates and regenerate
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### **Issue 3: Angular Node Version Conflicts**

**Symptoms:** Angular CLI compatibility issues

**Solutions:**
```bash
# Check Node.js version
node --version

# Use Node Version Manager (if needed)
nvm install 18.17.0
nvm use 18.17.0

# Reinstall Angular CLI
npm uninstall -g @angular/cli
npm install -g @angular/cli@latest
```

### **Issue 4: Package Restoration Issues**

**Symptoms:** NuGet or npm package restoration failures

**Solutions:**
```bash
# .NET packages
dotnet nuget locals all --clear
dotnet restore --force

# Node packages
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

---

## 📝 Environment Checklist

Before starting development, ensure all these items are checked:

- [ ] Git installed and configured
- [ ] Visual Studio 2022 or VS Code installed with required extensions
- [ ] .NET 8 SDK installed and verified
- [ ] PostgreSQL installed and running
- [ ] pgAdmin installed and database created
- [ ] Node.js LTS version installed
- [ ] Angular CLI installed globally
- [ ] Development certificates trusted
- [ ] Environment variables configured
- [ ] Database migrations applied successfully
- [ ] Both backend and frontend projects build successfully

---

## 🔗 Useful Links

- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Angular Documentation](https://angular.io/docs)
- [TypeScript Documentation](https://www.typescriptlang.org/docs/)

---

## 📞 Support

If you encounter any issues during environment setup:

1. Check the troubleshooting section above
2. Review the project's GitHub issues
3. Create a new issue with detailed error information
4. Contact the development team

**Next Step:** Once your environment is set up, proceed to [How to Run the Application](./running-application.md)
