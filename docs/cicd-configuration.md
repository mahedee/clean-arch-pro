# EduTrack - CI/CD Configuration Guide

## 🚀 **Complete CI/CD Setup for Clean Architecture Project**

This guide provides production-ready CI/CD configurations for the EduTrack project with multi-database support.

---

## 📋 **GitHub Actions Workflow**

### **Main CI/CD Pipeline** `.github/workflows/ci-cd.yml`

```yaml
name: EduTrack CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  release:
    types: [ published ]

env:
  DOTNET_VERSION: '8.0.x'
  NODE_VERSION: '18.x'
  SONAR_PROJECT_KEY: 'mahedee_clean-arch-pro'
  DOCKER_REGISTRY: 'ghcr.io'

jobs:
  # =====================================
  # BUILD & TEST JOBS
  # =====================================
  
  build-backend:
    name: 🔨 Build Backend
    runs-on: ubuntu-latest
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0  # Shallow clones should be disabled for SonarCloud
        
    - name: 🔧 Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
        
    - name: 📦 Restore dependencies
      run: dotnet restore backend/EduTrack/EduTrack.sln
      
    - name: 🔨 Build solution
      run: dotnet build backend/EduTrack/EduTrack.sln --no-restore --configuration Release
      
    - name: 📋 Upload build artifacts
      uses: actions/upload-artifact@v3
      with:
        name: backend-build
        path: backend/EduTrack/src/*/bin/Release/
        retention-days: 1

  build-frontend:
    name: 🎨 Build Frontend
    runs-on: ubuntu-latest
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: ${{ env.NODE_VERSION }}
        cache: 'npm'
        cache-dependency-path: frontend/package-lock.json
        
    - name: 📦 Install dependencies
      run: |
        cd frontend
        npm ci
        
    - name: 🔨 Build Angular app
      run: |
        cd frontend
        npm run build --prod
        
    - name: 📋 Upload frontend artifacts
      uses: actions/upload-artifact@v3
      with:
        name: frontend-build
        path: frontend/dist/
        retention-days: 1

  # =====================================
  # UNIT TESTING
  # =====================================
  
  unit-tests:
    name: 🧪 Unit Tests
    runs-on: ubuntu-latest
    needs: build-backend
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
        
    - name: 📥 Download build artifacts
      uses: actions/download-artifact@v3
      with:
        name: backend-build
        
    - name: 🧪 Run unit tests
      run: |
        dotnet test backend/EduTrack/tests/**/*.UnitTests.csproj \
          --configuration Release \
          --no-build \
          --verbosity normal \
          --collect:"XPlat Code Coverage" \
          --results-directory ./coverage
          
    - name: 📊 Generate coverage report
      uses: danielpalme/ReportGenerator-GitHub-Action@5.1.24
      with:
        reports: 'coverage/**/coverage.cobertura.xml'
        targetdir: 'coverage-report'
        reporttypes: 'HtmlInline;Cobertura'
        
    - name: 📋 Upload coverage to Codecov
      uses: codecov/codecov-action@v3
      with:
        file: ./coverage-report/Cobertura.xml
        fail_ci_if_error: true

  # =====================================
  # MULTI-DATABASE INTEGRATION TESTS
  # =====================================
  
  integration-tests:
    name: 🗄️ Multi-Database Integration Tests
    runs-on: ubuntu-latest
    needs: build-backend
    
    strategy:
      matrix:
        database: [postgresql, sqlserver]
        
    services:
      postgres:
        image: postgres:15
        env:
          POSTGRES_PASSWORD: postgres
          POSTGRES_DB: edutrack_test
        options: >-
          --health-cmd pg_isready
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 5432:5432
          
      sqlserver:
        image: mcr.microsoft.com/mssql/server:2022-latest
        env:
          SA_PASSWORD: YourStrong@Passw0rd
          ACCEPT_EULA: Y
        options: >-
          --health-cmd "/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q 'SELECT 1'"
          --health-interval 10s
          --health-timeout 5s
          --health-retries 5
        ports:
          - 1433:1433
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
        
    - name: 📥 Download build artifacts
      uses: actions/download-artifact@v3
      with:
        name: backend-build
        
    - name: 🗄️ Run ${{ matrix.database }} integration tests
      env:
        ConnectionStrings__DefaultConnection: ${{ matrix.database == 'postgresql' && 'Host=localhost;Database=edutrack_test;Username=postgres;Password=postgres' || 'Server=localhost;Database=edutrack_test;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true' }}
        DatabaseProvider: ${{ matrix.database == 'postgresql' && 'PostgreSQL' || 'SqlServer' }}
      run: |
        dotnet test backend/EduTrack/tests/**/*.IntegrationTests.csproj \
          --configuration Release \
          --no-build \
          --verbosity normal \
          --logger "trx;LogFileName=integration-tests-${{ matrix.database }}.trx"
          
    - name: 📋 Upload test results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: integration-test-results-${{ matrix.database }}
        path: "**/*.trx"

  # =====================================
  # CODE QUALITY & SECURITY
  # =====================================
  
  sonarcloud:
    name: 📊 SonarCloud Analysis
    runs-on: ubuntu-latest
    needs: [unit-tests]
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      with:
        fetch-depth: 0
        
    - name: 🔧 Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ env.DOTNET_VERSION }}
        
    - name: 🔧 Setup Java (for SonarCloud)
      uses: actions/setup-java@v3
      with:
        distribution: 'zulu'
        java-version: '17'
        
    - name: 📥 Download coverage reports
      uses: actions/download-artifact@v3
      with:
        name: coverage-report
        path: ./coverage-report
        
    - name: 📊 SonarCloud Scan
      uses: SonarSource/sonarcloud-github-action@master
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
        SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      with:
        args: >
          -Dsonar.projectKey=${{ env.SONAR_PROJECT_KEY }}
          -Dsonar.cs.opencover.reportsPaths=./coverage-report/Cobertura.xml
          -Dsonar.coverage.exclusions=**/Migrations/**,**/Program.cs,**/Startup.cs

  security-scan:
    name: 🔒 Security Scanning
    runs-on: ubuntu-latest
    needs: build-backend
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔒 Run Snyk vulnerability scan
      uses: snyk/actions/dotnet@master
      env:
        SNYK_TOKEN: ${{ secrets.SNYK_TOKEN }}
      with:
        args: --severity-threshold=high
        
    - name: 🔒 Run CodeQL analysis
      uses: github/codeql-action/init@v2
      with:
        languages: csharp, javascript
        
    - name: 🔒 Perform CodeQL analysis
      uses: github/codeql-action/analyze@v2

  # =====================================
  # DOCKER BUILD & PUSH
  # =====================================
  
  docker-build:
    name: 🐳 Build Docker Images
    runs-on: ubuntu-latest
    needs: [unit-tests, integration-tests]
    if: github.event_name == 'push' && (github.ref == 'refs/heads/main' || github.ref == 'refs/heads/develop')
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
      
    - name: 🔑 Login to Container Registry
      uses: docker/login-action@v3
      with:
        registry: ${{ env.DOCKER_REGISTRY }}
        username: ${{ github.actor }}
        password: ${{ secrets.GITHUB_TOKEN }}
        
    - name: 📥 Download backend artifacts
      uses: actions/download-artifact@v3
      with:
        name: backend-build
        path: ./backend-build
        
    - name: 📥 Download frontend artifacts
      uses: actions/download-artifact@v3
      with:
        name: frontend-build
        path: ./frontend-build
        
    - name: 🏗️ Extract metadata
      id: meta
      uses: docker/metadata-action@v5
      with:
        images: ${{ env.DOCKER_REGISTRY }}/${{ github.repository }}
        tags: |
          type=ref,event=branch
          type=ref,event=pr
          type=sha,prefix={{branch}}-
          type=raw,value=latest,enable={{is_default_branch}}
          
    - name: 🐳 Build and push API image
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ./backend/EduTrack/Dockerfile
        platforms: linux/amd64,linux/arm64
        push: true
        tags: ${{ steps.meta.outputs.tags }}
        labels: ${{ steps.meta.outputs.labels }}
        cache-from: type=gha
        cache-to: type=gha,mode=max

  # =====================================
  # DEPLOYMENT JOBS
  # =====================================
  
  deploy-staging:
    name: 🚀 Deploy to Staging
    runs-on: ubuntu-latest
    needs: [docker-build, sonarcloud, security-scan]
    if: github.ref == 'refs/heads/develop'
    environment: staging
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup Azure CLI
      uses: azure/setup-kubectl@v3
      
    - name: 🔑 Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
        
    - name: 🚀 Deploy to Azure Container Apps
      run: |
        az containerapp update \
          --name edutrack-api-staging \
          --resource-group edutrack-staging-rg \
          --image ${{ env.DOCKER_REGISTRY }}/${{ github.repository }}:develop-${{ github.sha }}
          
    - name: 🧪 Run smoke tests
      run: |
        sleep 30  # Wait for deployment
        curl -f https://edutrack-api-staging.azurecontainerapps.io/health || exit 1

  deploy-production:
    name: 🚀 Deploy to Production
    runs-on: ubuntu-latest
    needs: [docker-build, sonarcloud, security-scan]
    if: github.ref == 'refs/heads/main'
    environment: production
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup Azure CLI
      uses: azure/setup-kubectl@v3
      
    - name: 🔑 Azure Login
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
        
    - name: 🚀 Deploy to Azure Container Apps
      run: |
        az containerapp update \
          --name edutrack-api-prod \
          --resource-group edutrack-prod-rg \
          --image ${{ env.DOCKER_REGISTRY }}/${{ github.repository }}:latest
          
    - name: 🧪 Run production smoke tests
      run: |
        sleep 30  # Wait for deployment
        curl -f https://edutrack-api.azurecontainerapps.io/health || exit 1
        
    - name: 📧 Notify deployment success
      uses: 8398a7/action-slack@v3
      with:
        status: success
        text: '🚀 EduTrack deployed to production successfully!'
      env:
        SLACK_WEBHOOK_URL: ${{ secrets.SLACK_WEBHOOK_URL }}

  # =====================================
  # E2E TESTING
  # =====================================
  
  e2e-tests:
    name: 🎭 E2E Tests
    runs-on: ubuntu-latest
    needs: deploy-staging
    if: github.ref == 'refs/heads/develop'
    
    steps:
    - name: 📥 Checkout code
      uses: actions/checkout@v4
      
    - name: 🔧 Setup Node.js
      uses: actions/setup-node@v4
      with:
        node-version: ${{ env.NODE_VERSION }}
        
    - name: 📦 Install Cypress
      run: |
        cd frontend
        npm ci
        npx cypress install
        
    - name: 🎭 Run Cypress E2E tests
      run: |
        cd frontend
        npx cypress run --config baseUrl=https://edutrack-staging.azurecontainerapps.io
        
    - name: 📋 Upload E2E test results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: e2e-test-results
        path: frontend/cypress/screenshots/
```

---

## 🐳 **Docker Configuration**

### **Backend Dockerfile** `backend/EduTrack/Dockerfile`

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["src/EduTrack.Api/EduTrack.Api.csproj", "src/EduTrack.Api/"]
COPY ["src/EduTrack.Application/EduTrack.Application.csproj", "src/EduTrack.Application/"]
COPY ["src/EduTrack.Domain/EduTrack.Domain.csproj", "src/EduTrack.Domain/"]
COPY ["src/EduTrack.Infrastructure/EduTrack.Infrastructure.csproj", "src/EduTrack.Infrastructure/"]

RUN dotnet restore "src/EduTrack.Api/EduTrack.Api.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/EduTrack.Api"
RUN dotnet build "EduTrack.Api.csproj" -c Release -o /app/build
RUN dotnet publish "EduTrack.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 edutrack && \
    adduser --system --uid 1001 --group edutrack

# Copy published files
COPY --from=build /app/publish .

# Change ownership to non-root user
RUN chown -R edutrack:edutrack /app
USER edutrack

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "EduTrack.Api.dll"]
```

### **Frontend Dockerfile** `frontend/Dockerfile`

```dockerfile
# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy source code and build
COPY . .
RUN npm run build --prod

# Runtime stage
FROM nginx:alpine AS runtime

# Copy custom nginx config
COPY nginx.conf /etc/nginx/nginx.conf

# Copy built app
COPY --from=build /app/dist/edutrack /usr/share/nginx/html

# Create non-root user
RUN addgroup -g 1001 -S nginx && \
    adduser -S -D -H -u 1001 -h /var/cache/nginx -s /sbin/nologin -G nginx -g nginx nginx

# Change ownership
RUN chown -R nginx:nginx /usr/share/nginx/html && \
    chown -R nginx:nginx /var/cache/nginx && \
    chown -R nginx:nginx /var/log/nginx && \
    chown -R nginx:nginx /etc/nginx/conf.d

USER nginx

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/ || exit 1

CMD ["nginx", "-g", "daemon off;"]
```

---

## ⚙️ **Environment Configuration**

### **Required GitHub Secrets**

```yaml
# Authentication & Authorization
GITHUB_TOKEN: # Auto-provided by GitHub
SONAR_TOKEN: # SonarCloud token
SNYK_TOKEN: # Snyk security scanning token

# Cloud Deployment
AZURE_CREDENTIALS: # Azure service principal JSON
AWS_ACCESS_KEY_ID: # If using AWS
AWS_SECRET_ACCESS_KEY: # If using AWS

# Database Connections
STAGING_CONNECTION_STRING: # Staging database
PRODUCTION_CONNECTION_STRING: # Production database

# Notifications
SLACK_WEBHOOK_URL: # Slack notifications
EMAIL_API_KEY: # Email notifications

# Container Registry
DOCKER_USERNAME: # Docker Hub username (if not using GitHub)
DOCKER_PASSWORD: # Docker Hub password (if not using GitHub)
```

### **Environment Variables**

```yaml
# Application Configuration
ASPNETCORE_ENVIRONMENT: Production
DATABASE_PROVIDER: PostgreSQL  # PostgreSQL | SqlServer | Oracle
CONNECTION_STRING: # Database connection string

# Security
JWT_SECRET: # JWT signing key
ENCRYPTION_KEY: # Data encryption key

# External Services
REDIS_CONNECTION_STRING: # Caching
BLOB_STORAGE_CONNECTION: # File storage
EMAIL_API_KEY: # Email service
```

---

## 🌐 **Deployment Targets**

### **1. Azure Container Apps (Recommended)**
```yaml
Benefits:
  - Serverless scaling
  - Integrated with GitHub Actions
  - Support for multiple database connections
  - Built-in load balancing
  - Easy blue-green deployments
```

### **2. AWS ECS with Fargate**
```yaml
Benefits:
  - AWS ecosystem integration
  - Auto-scaling
  - Service mesh capabilities
  - Cost optimization
```

### **3. Google Cloud Run**
```yaml
Benefits:
  - Pay-per-use pricing
  - Automatic HTTPS
  - Global load balancing
  - Easy rollbacks
```

### **4. Kubernetes (Azure AKS/AWS EKS)**
```yaml
Benefits:
  - Maximum flexibility
  - Advanced orchestration
  - Multi-cloud support
  - Enterprise-grade features
```

---

## 📊 **Monitoring & Observability**

### **Application Performance Monitoring**
```yaml
Tools:
  - Application Insights (Azure)
  - New Relic
  - Datadog
  - Prometheus + Grafana

Metrics to Track:
  - Response times
  - Error rates
  - Database performance
  - Memory/CPU usage
  - User adoption metrics
```

### **Logging Strategy**
```yaml
Structured Logging:
  - Serilog with JSON formatting
  - Centralized log aggregation
  - Log correlation IDs
  - Security audit logs

Log Destinations:
  - Azure Log Analytics
  - ELK Stack (Elasticsearch, Logstash, Kibana)
  - AWS CloudWatch
  - Google Cloud Logging
```

---

## 🔒 **Security Best Practices**

### **Container Security**
```yaml
Security Measures:
  - Use minimal base images (Alpine/Distroless)
  - Regular vulnerability scanning
  - Non-root user execution
  - Secret management with Azure Key Vault
  - Network policies and firewalls
```

### **Database Security**
```yaml
Security Measures:
  - Connection string encryption
  - Database firewall rules
  - SQL injection prevention
  - Data encryption at rest
  - Regular security audits
```

---

## 📋 **Quality Gates**

### **Pre-Production Checklist**
```yaml
Code Quality:
  - [ ] SonarCloud quality gate passed
  - [ ] Unit test coverage > 90%
  - [ ] Integration tests passed
  - [ ] Security scan completed
  - [ ] Performance benchmarks met

Deployment:
  - [ ] Blue-green deployment ready
  - [ ] Database migrations tested
  - [ ] Rollback plan documented
  - [ ] Monitoring alerts configured
  - [ ] Load testing completed
```

---

This comprehensive CI/CD setup ensures your EduTrack project follows industry best practices with robust testing, security, and deployment automation across multiple environments and database providers.
