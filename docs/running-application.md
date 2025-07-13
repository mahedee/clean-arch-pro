# EduTrack - Application Running Guide

This guide provides detailed instructions on how to run the EduTrack Clean Architecture application in different environments and scenarios.

## 🎯 Prerequisites

Before running the application, ensure you have completed the [Environment Setup Guide](./environment-setup.md) and have:

- ✅ Development environment properly configured
- ✅ PostgreSQL database running
- ✅ .NET 8 SDK installed
- ✅ Node.js and Angular CLI installed
- ✅ Project cloned and dependencies restored

---

## 🚀 Quick Start (Development)

### **Option 1: Using Visual Studio 2022**

1. **Open Solution:**
   ```
   File → Open → Project/Solution → Navigate to:
   backend/EduTrack/EduTrack.sln
   ```

2. **Set Startup Project:**
   - Right-click on `EduTrack.Api` project
   - Select "Set as Startup Project"

3. **Configure Database:**
   - Update connection string in `appsettings.Development.json`
   - Open Package Manager Console
   - Run: `Update-Database`

4. **Run Application:**
   - Press `F5` or click "Start Debugging"
   - Application will open at: `https://localhost:7050`

### **Option 2: Using VS Code**

1. **Open Workspace:**
   ```bash
   code clean-arch-pro
   ```

2. **Open Integrated Terminal:**
   - Press `Ctrl + Shift + ` ` (backtick)

3. **Run Backend:**
   ```bash
   cd backend/EduTrack/src/EduTrack.Api
   dotnet run
   ```

4. **Open New Terminal for Frontend (when available):**
   ```bash
   cd frontend
   ng serve
   ```

### **Option 3: Command Line**

**Terminal 1 - Backend:**
```bash
# Navigate to API project
cd backend/EduTrack/src/EduTrack.Api

# Run the application
dotnet run

# Alternative with watch (auto-restart on changes)
dotnet watch run
```

**Terminal 2 - Frontend (Future):**
```bash
# Navigate to frontend project
cd frontend

# Start development server
ng serve

# Alternative with specific port
ng serve --port 4200 --open
```

---

## 🗄️ Database Setup & Management

### **First Time Setup**

1. **Ensure PostgreSQL is Running:**
   ```bash
   # Windows (if installed as service)
   net start postgresql-x64-13
   
   # Check if running
   pg_isready -h localhost -p 5432
   ```

2. **Create Database (if not exists):**
   ```sql
   -- Connect via pgAdmin or psql
   psql -U postgres -h localhost
   
   -- Create database
   CREATE DATABASE "EduTrackDb";
   
   -- Create user (optional)
   CREATE USER edutrack_user WITH PASSWORD 'your_password';
   GRANT ALL PRIVILEGES ON DATABASE "EduTrackDb" TO edutrack_user;
   ```

3. **Apply Database Migrations:**
   ```bash
   cd backend/EduTrack
   
   # Apply migrations
   dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
   ```

### **Database Commands Reference**

```bash
# Navigate to solution directory
cd backend/EduTrack

# Create a new migration
dotnet ef migrations add MigrationName --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Apply pending migrations
dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Drop database (careful!)
dotnet ef database drop --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Script migrations to SQL
dotnet ef migrations script --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
```

---

## 🔧 Configuration Management

### **Backend Configuration**

**Development Settings (appsettings.Development.json):**
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
    "SecretKey": "your-development-secret-key-min-32-characters",
    "Issuer": "EduTrack.Api",
    "Audience": "EduTrack.Client",
    "ExpirationInMinutes": 60
  },
  "EmailSettings": {
    "SmtpHost": "localhost",
    "SmtpPort": 587,
    "EnableSsl": true,
    "Username": "",
    "Password": ""
  }
}
```

**Production Settings (appsettings.Production.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-prod-host;Port=5432;Database=EduTrackDb;Username=your-user;Password=your-password"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "yourdomain.com",
  "Cors": {
    "AllowedOrigins": ["https://yourdomain.com"]
  }
}
```

### **Frontend Configuration (Future)**

**Development (environment.development.ts):**
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7050/api',
  apiVersion: 'v1',
  enableLogging: true,
  logLevel: 'debug'
};
```

**Production (environment.production.ts):**
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.yourdomain.com/api',
  apiVersion: 'v1',
  enableLogging: false,
  logLevel: 'error'
};
```

---

## 🌍 Running in Different Environments

### **Development Environment**

```bash
# Set environment variable
export ASPNETCORE_ENVIRONMENT=Development  # Linux/Mac
$env:ASPNETCORE_ENVIRONMENT = "Development"  # Windows PowerShell

# Run with development settings
cd backend/EduTrack/src/EduTrack.Api
dotnet run --environment Development
```

### **Staging Environment**

```bash
# Set environment variable
export ASPNETCORE_ENVIRONMENT=Staging

# Run with staging settings
dotnet run --environment Staging
```

### **Production Environment**

```bash
# Set environment variable
export ASPNETCORE_ENVIRONMENT=Production

# Build for production
dotnet build --configuration Release

# Run production build
dotnet run --configuration Release --environment Production
```

---

## 🧪 Testing the Application

### **Backend Testing**

**Unit Tests:**
```bash
cd backend/EduTrack

# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/EduTrack.Application.UnitTests/

# Run with detailed output
dotnet test --verbosity normal
```

**Integration Tests:**
```bash
# Run integration tests (when available)
dotnet test tests/EduTrack.Api.IntegrationTests/

# Run with test database
dotnet test --environment Testing
```

### **API Testing with Swagger**

1. **Start the application**
2. **Navigate to Swagger UI:**
   ```
   https://localhost:7050/swagger/index.html
   ```
3. **Test endpoints directly from the browser**

### **API Testing with curl**

```bash
# Test health endpoint
curl -X GET "https://localhost:7050/health" -H "accept: application/json"

# Test get all students
curl -X GET "https://localhost:7050/api/students" -H "accept: application/json"

# Test create student
curl -X POST "https://localhost:7050/api/students" \
  -H "accept: application/json" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "John Doe",
    "dateOfBirth": "2000-01-01T00:00:00.000Z",
    "email": "john.doe@example.com"
  }'
```

### **Frontend Testing (Future)**

```bash
cd frontend

# Run unit tests
ng test

# Run e2e tests
ng e2e

# Run tests with coverage
ng test --code-coverage

# Run tests in headless mode
ng test --watch=false --browsers=ChromeHeadless
```

---

## 🔍 Monitoring & Debugging

### **Application Logs**

**View Logs in Console:**
```bash
# Run with detailed logging
dotnet run --verbosity diagnostic

# Run with specific log level
export Logging__LogLevel__Default=Debug
dotnet run
```

**Log Files Location:**
- Development: Console output
- Production: Configure file logging in appsettings.json

### **Database Monitoring**

**Using pgAdmin:**
1. Connect to your database
2. Navigate to Tools → Server Status
3. Monitor active connections and queries

**Using psql:**
```sql
-- Connect to database
psql -U postgres -d EduTrackDb

-- View active connections
SELECT * FROM pg_stat_activity;

-- View database size
SELECT pg_size_pretty(pg_database_size('EduTrackDb'));

-- View table sizes
SELECT 
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) as size
FROM pg_tables 
WHERE schemaname = 'public';
```

### **Performance Monitoring**

**Application Insights (Future Enhancement):**
```json
// Add to appsettings.json
{
  "ApplicationInsights": {
    "InstrumentationKey": "your-key-here"
  }
}
```

**Health Checks:**
```bash
# Check application health
curl https://localhost:7050/health

# Detailed health check
curl https://localhost:7050/health/detailed
```

---

## 🐳 Docker Support (Future Enhancement)

### **Backend Dockerfile**

```dockerfile
# See https://aka.ms/customizecontainer
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EduTrack.Api/EduTrack.Api.csproj", "src/EduTrack.Api/"]
COPY ["src/EduTrack.Application/EduTrack.Application.csproj", "src/EduTrack.Application/"]
COPY ["src/EduTrack.Domain/EduTrack.Domain.csproj", "src/EduTrack.Domain/"]
COPY ["src/EduTrack.Infrastructure/EduTrack.Infrastructure.csproj", "src/EduTrack.Infrastructure/"]
RUN dotnet restore "./src/EduTrack.Api/EduTrack.Api.csproj"
COPY . .
WORKDIR "/src/src/EduTrack.Api"
RUN dotnet build "./EduTrack.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EduTrack.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EduTrack.Api.dll"]
```

### **Docker Compose**

```yaml
version: '3.8'

services:
  database:
    image: postgres:15
    environment:
      POSTGRES_DB: EduTrackDb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: yourpassword
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  api:
    build:
      context: ./backend/EduTrack
      dockerfile: Dockerfile
    ports:
      - "7050:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Host=database;Port=5432;Database=EduTrackDb;Username=postgres;Password=yourpassword
    depends_on:
      - database

volumes:
  postgres_data:
```

### **Running with Docker**

```bash
# Build and run with docker-compose
docker-compose up --build

# Run in background
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

---

## 🛠️ Troubleshooting Common Issues

### **Issue 1: Database Connection Failed**

**Symptoms:**
- Application fails to start
- Error: "Unable to connect to database"

**Solutions:**
```bash
# Check if PostgreSQL is running
pg_isready -h localhost -p 5432

# Test connection manually
psql -U postgres -h localhost -d EduTrackDb

# Verify connection string in appsettings.json
# Check firewall settings
# Ensure database exists
```

### **Issue 2: Migration Issues**

**Symptoms:**
- Migration commands fail
- Database schema mismatch

**Solutions:**
```bash
# Check migration status
dotnet ef migrations list --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Reset database (development only)
dotnet ef database drop --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api

# Create clean migration
dotnet ef migrations remove --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
dotnet ef migrations add InitialMigration --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
```

### **Issue 3: Port Conflicts**

**Symptoms:**
- "Port already in use" error
- Application won't start

**Solutions:**
```bash
# Check what's using the port
netstat -ano | findstr :7050  # Windows
lsof -i :7050                 # Mac/Linux

# Kill process using port
taskkill /PID <PID> /F        # Windows
kill -9 <PID>                 # Mac/Linux

# Change port in launchSettings.json
```

### **Issue 4: HTTPS Certificate Issues**

**Symptoms:**
- SSL/TLS errors
- Certificate not trusted warnings

**Solutions:**
```bash
# Trust development certificates
dotnet dev-certs https --trust

# Clean and recreate certificates
dotnet dev-certs https --clean
dotnet dev-certs https --trust

# Check certificate
dotnet dev-certs https --check
```

### **Issue 5: Frontend Connection Issues (Future)**

**Symptoms:**
- CORS errors
- API calls failing from frontend

**Solutions:**
```typescript
// Check environment.ts
// Verify API URL
// Check CORS configuration in backend

// Backend: Configure CORS in Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
```

---

## 📊 Development Workflow

### **Daily Development Routine**

1. **Start PostgreSQL** (if not running as service)
2. **Pull latest changes:**
   ```bash
   git pull origin main
   ```

3. **Restore packages:**
   ```bash
   cd backend/EduTrack
   dotnet restore
   ```

4. **Apply migrations:**
   ```bash
   dotnet ef database update --project src/EduTrack.Infrastructure --startup-project src/EduTrack.Api
   ```

5. **Start backend:**
   ```bash
   cd src/EduTrack.Api
   dotnet watch run
   ```

6. **Start frontend (when available):**
   ```bash
   cd frontend
   ng serve
   ```

### **Code Changes Workflow**

1. **Create feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make changes and test locally**

3. **Run tests:**
   ```bash
   dotnet test
   ```

4. **Commit and push:**
   ```bash
   git add .
   git commit -m "Add your feature description"
   git push origin feature/your-feature-name
   ```

5. **Create pull request**

---

## 🔗 Application URLs

### **Development URLs**

- **Backend API**: `https://localhost:7050`
- **Swagger UI**: `https://localhost:7050/swagger`
- **Health Check**: `https://localhost:7050/health`
- **Frontend (Future)**: `http://localhost:4200`

### **API Endpoints**

**Students:**
- `GET /api/students` - Get all students
- `GET /api/students/{id}` - Get student by ID
- `POST /api/students` - Create new student
- `PUT /api/students/{id}` - Update student
- `DELETE /api/students/{id}` - Delete student

**Health:**
- `GET /health` - Basic health check
- `GET /health/ready` - Readiness check
- `GET /health/live` - Liveness check

---

## 📝 Running Checklist

Before starting development, ensure:

- [ ] PostgreSQL is running
- [ ] Database exists and migrations are applied
- [ ] Environment variables are set correctly
- [ ] Latest code is pulled from repository
- [ ] NuGet packages are restored
- [ ] Application builds successfully
- [ ] Tests are passing
- [ ] Swagger UI is accessible
- [ ] API endpoints respond correctly

---

## 🆘 Getting Help

If you encounter issues while running the application:

1. **Check this troubleshooting guide**
2. **Review application logs**
3. **Check GitHub Issues for similar problems**
4. **Create a new issue with:**
   - Error message/stack trace
   - Steps to reproduce
   - Environment details (OS, .NET version, etc.)
   - Configuration files (remove sensitive data)

---

## 📞 Support Contacts

- **Project Maintainer**: [Mahedee Hasan](https://github.com/mahedee)
- **Email**: mahedee.hasan@gmail.com
- **GitHub Issues**: [Create New Issue](https://github.com/mahedee/clean-arch-pro/issues/new)

**Previous Step:** [Environment Setup Guide](./environment-setup.md)  
**Next Step:** Start developing with the [Suggestions Guide](./suggestions.md)
