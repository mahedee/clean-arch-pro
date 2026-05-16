# Create Test Coverage Issue Script
# This script creates a GitHub issue for implementing test coverage reports

# GitHub repository details
$owner = "mahedee"
$repo = "clean-arch-pro"
$githubApiUrl = "https://api.github.com/repos/$owner/$repo/issues"

# Read GitHub Personal Access Token from token.txt file
$tokenFile = Join-Path $PSScriptRoot "token.txt"

if (-not (Test-Path $tokenFile)) {
    Write-Host "❌ ERROR: token.txt file not found in scripts folder" -ForegroundColor Red
    exit 1
}

$githubToken = (Get-Content $tokenFile -Raw).Trim()

if ([string]::IsNullOrEmpty($githubToken) -or $githubToken -eq "YOUR_GITHUB_TOKEN_HERE") {
    Write-Host "❌ ERROR: Please set your GitHub Personal Access Token in token.txt file" -ForegroundColor Red
    exit 1
}

# Headers for GitHub API
$headers = @{
    'Authorization' = "token $githubToken"
    'Accept' = 'application/vnd.github.v3+json'
    'Content-Type' = 'application/json'
}

Write-Host "🚀 Creating Test Coverage Implementation Issue..." -ForegroundColor Green
Write-Host "Repository: $owner/$repo" -ForegroundColor Cyan
Write-Host ""

# Issue data for test coverage implementation
$issue = @{
    title = "🧪 Implement Test Coverage Reporting in Application"
    body = @"
## 🎯 Task Overview
**Task ID**: T010B  
**Sprint**: 4  
**Status**: 📋 **NEW TASK**  
**Priority**: 🟡 **HIGH**  
**Duration**: 2-3 days  

## 📋 Description
Implement comprehensive test coverage reporting and quality metrics for the EduTrack application to ensure high code quality and identify areas needing additional testing.

## 🔧 Implementation Requirements

### **Coverage Tools Setup**
- [ ] Install and configure **Coverlet** for .NET code coverage collection
- [ ] Setup **ReportGenerator** for HTML coverage report generation
- [ ] Configure **dotCover** or **OpenCover** as alternative coverage tools
- [ ] Integrate **SonarQube** for code quality and coverage analysis

### **Coverage Integration**
- [ ] Integrate coverage collection in test execution pipeline
- [ ] Setup coverage thresholds and quality gates
- [ ] Create coverage report generation scripts
- [ ] Configure IDE integration for coverage visualization (VS Code/Visual Studio)

### **CI/CD Integration**
- [ ] Setup automated coverage reporting in GitHub Actions
- [ ] Implement branch and line coverage metrics
- [ ] Create coverage badges for repository README
- [ ] Establish minimum coverage requirements per project

### **Reporting Features**
- [ ] Generate HTML coverage reports for all test projects
- [ ] Create coverage trending and history tracking
- [ ] Implement coverage failure notifications
- [ ] Setup coverage report artifacts in CI/CD

## 📊 Coverage Targets

| Project | Minimum Coverage | Target Coverage |
|---------|------------------|-----------------|
| **EduTrack.Domain** | 95% | 98% |
| **EduTrack.Application** | 90% | 95% |
| **EduTrack.Infrastructure** | 85% | 90% |
| **EduTrack.Api** | 80% | 85% |

## 🛠️ Tools and Technologies

### **Primary Tools**
- **Coverlet**: Cross-platform .NET code coverage library
- **ReportGenerator**: Generates readable coverage reports (HTML/XML/JSON)
- **GitHub Actions**: Automated coverage in CI/CD pipeline

### **Alternative/Additional Tools**
- **dotCover**: JetBrains coverage tool
- **SonarQube**: Code quality and coverage analysis
- **Codecov**: Cloud-based coverage reporting

## 📈 Success Criteria

### **Technical Requirements**
- [ ] ✅ HTML coverage reports generated for all test projects
- [ ] ✅ Coverage thresholds enforced in CI/CD pipeline
- [ ] ✅ Real-time coverage feedback in development environment
- [ ] ✅ Automated coverage reporting on every commit
- [ ] ✅ Coverage trending and history tracking implemented

### **Quality Gates**
- [ ] ✅ Minimum coverage requirements met for each project
- [ ] ✅ Coverage reports accessible to all team members
- [ ] ✅ Failed builds when coverage drops below thresholds
- [ ] ✅ Coverage badges visible in README

## 🚀 Implementation Steps

### **Phase 1: Local Setup (Day 1)**
1. Install Coverlet NuGet packages in test projects
2. Configure coverage collection commands
3. Setup ReportGenerator for HTML output
4. Test local coverage generation

### **Phase 2: CI/CD Integration (Day 2)**
1. Add coverage steps to GitHub Actions workflow
2. Configure coverage thresholds and quality gates
3. Setup coverage report artifacts
4. Implement coverage badges

### **Phase 3: Advanced Features (Day 3)**
1. Setup coverage history tracking
2. Implement coverage trend analysis
3. Configure IDE integration
4. Create coverage documentation

## 📝 Current State
**Test Infrastructure**: ✅ **COMPLETE** (162 tests passing)
- EduTrack.Domain.UnitTests: ✅ Comprehensive Value Objects testing
- EduTrack.Application.UnitTests: ✅ Basic structure in place
- EduTrack.Infrastructure.UnitTests: ✅ Repository testing ready
- EduTrack.Api.IntegrationTests: ✅ API endpoint testing ready

## 🔗 Related Tasks
- **T002**: Domain Layer Foundation (80% complete)
- **T010**: Testing Infrastructure (completed)
- **T011**: Student Management CRUD (depends on coverage setup)

## 📚 Documentation References
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator Documentation](https://danielpalme.github.io/ReportGenerator/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/best-practices)

## 🏷️ Labels
- `enhancement`
- `testing`
- `quality`
- `ci-cd`
- `high-priority`
- `sprint-4`

---

**Assignee**: Development Team  
**Milestone**: Phase 1 - Foundation & Core Infrastructure  
**Estimated Effort**: 16-24 hours
"@
    labels = @("enhancement", "testing", "quality", "ci-cd", "high-priority")
    assignees = @()
    milestone = $null
}

try {
    # Convert to JSON
    $jsonBody = $issue | ConvertTo-Json -Depth 10

    # Create the issue
    $response = Invoke-RestMethod -Uri $githubApiUrl -Method Post -Headers $headers -Body $jsonBody
    
    Write-Host "✅ Created issue #$($response.number): $($response.title)" -ForegroundColor Green
    Write-Host "   URL: $($response.html_url)" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "📊 Issue Details:" -ForegroundColor Yellow
    Write-Host "   - Issue Number: #$($response.number)" -ForegroundColor White
    Write-Host "   - State: $($response.state)" -ForegroundColor White
    Write-Host "   - Labels: $($issue.labels -join ', ')" -ForegroundColor White
    Write-Host ""
    Write-Host "🎉 Test Coverage Implementation issue created successfully!" -ForegroundColor Green
    Write-Host "View it at: $($response.html_url)" -ForegroundColor Cyan
    
} catch {
    Write-Host "❌ Failed to create issue: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode
        Write-Host "   Status Code: $statusCode" -ForegroundColor Red
    }
}
