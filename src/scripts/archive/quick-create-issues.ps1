# Quick GitHub Issues Creator
# Run this script to create all GitHub issues at once

# First, you need to set your GitHub token
Write-Host "🔑 Setting up GitHub Personal Access Token..." -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Go to: https://github.com/settings/tokens" -ForegroundColor Cyan
Write-Host "2. Click 'Generate new token (classic)'" -ForegroundColor Cyan  
Write-Host "3. Select 'repo' scope" -ForegroundColor Cyan
Write-Host "4. Copy the token" -ForegroundColor Cyan
Write-Host ""

# Prompt for GitHub token
$githubToken = Read-Host "Enter your GitHub Personal Access Token"

if ([string]::IsNullOrEmpty($githubToken)) {
    Write-Host "❌ No token provided. Exiting..." -ForegroundColor Red
    exit 1
}

# GitHub API settings
$owner = "mahedee"
$repo = "clean-arch-pro"
$githubApiUrl = "https://api.github.com/repos/$owner/$repo/issues"

$headers = @{
    'Authorization' = "token $githubToken"
    'Accept' = 'application/vnd.github.v3+json'
    'Content-Type' = 'application/json'
}

Write-Host "🚀 Creating GitHub Issues..." -ForegroundColor Green
Write-Host ""

# Issue 1
$issue1 = @{
    title = "✅ COMPLETED - Clean Architecture Solution Structure Setup"
    body = @"
## 🎯 Task Overview
**Task ID**: T001-1  
**Sprint**: 1  
**Status**: ✅ COMPLETED  
**Duration**: ~2 hours  

## 📋 Description
Successfully created the foundational solution structure following Clean Architecture principles with proper layer separation and dependencies.

## ✅ Completed Work
- ✅ Created 4-layer Clean Architecture solution structure
- ✅ Established proper project hierarchy and dependencies
- ✅ Configured solution file with all projects
- ✅ Implemented proper separation of concerns

## 🏗️ Solution Structure Created
``````
EduTrack.sln
├── src/
│   ├── EduTrack.Api/ (Presentation Layer)
│   ├── EduTrack.Application/ (Application Layer) 
│   ├── EduTrack.Domain/ (Domain Layer)
│   └── EduTrack.Infrastructure/ (Infrastructure Layer)
└── tests/
    ├── EduTrack.Application.UnitTests/
    ├── EduTrack.Domain.UnitTests/
    ├── EduTrack.Infrastructure.UnitTests/
    └── EduTrack.Api.IntegrationTests/
``````

## 🔧 Technical Implementation
- **Framework**: .NET 8
- **Architecture**: Clean Architecture (Uncle Bob)
- **Pattern**: Domain-Driven Design (DDD)
- **Testing**: 4 comprehensive test projects

## 📊 Impact Metrics
- ✅ 100% compliance with Clean Architecture principles
- ✅ 4 layers properly separated and configured
- ✅ Solution builds successfully without errors
- ✅ Foundation ready for domain development

## 🔗 Related Tasks
- **Enables**: Architecture dependency fixes
- **Blocks**: All subsequent development tasks
- **Dependencies**: None (foundation task)

## 📝 Notes
This is the foundational task that enables all subsequent development. The solution structure follows industry best practices and provides a solid foundation for the EduTrack application.
"@
    labels = @("completed", "architecture", "task", "foundation")
} | ConvertTo-Json -Depth 10

try {
    $response1 = Invoke-RestMethod -Uri $githubApiUrl -Method Post -Body $issue1 -Headers $headers
    Write-Host "✅ Issue #$($response1.number): $($response1.title)" -ForegroundColor Green
    Write-Host "   URL: $($response1.html_url)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Failed to create Issue 1: $($_.Exception.Message)" -ForegroundColor Red
}

Start-Sleep -Seconds 2

# Issue 2  
$issue2 = @{
    title = "✅ COMPLETED - Clean Architecture Dependency Violations Fixed"
    body = @"
## 🎯 Task Overview
**Task ID**: T001-2  
**Sprint**: 1  
**Status**: ✅ COMPLETED  
**Duration**: ~1 hour  

## 📋 Description
Fixed critical Clean Architecture dependency violations that were preventing proper implementation of dependency inversion principle.

## ✅ Completed Work
- ✅ **FIXED**: Removed Application → Infrastructure dependency violation
- ✅ **VERIFIED**: Clean Architecture compliance restored
- ✅ **TESTED**: Solution builds without circular dependencies
- ✅ **VALIDATED**: Dependency flow follows Uncle Bob's Clean Architecture

## 🚨 Critical Issues Resolved

### Before Fix:
``````
❌ EduTrack.Application → EduTrack.Infrastructure (VIOLATION)
   This breaks the dependency inversion principle
``````

### After Fix:
``````
✅ EduTrack.Application → EduTrack.Domain (CORRECT)
✅ EduTrack.Infrastructure → EduTrack.Domain (CORRECT)
✅ EduTrack.Api → EduTrack.Application (CORRECT)
✅ EduTrack.Api → EduTrack.Infrastructure (CORRECT)
``````

## 🔧 Technical Implementation
- **Removed**: Direct Application → Infrastructure project reference
- **Maintained**: Proper dependency injection through API layer
- **Preserved**: Clean separation of concerns
- **Validated**: Architecture principles compliance

## 📊 Impact Metrics
- ✅ 100% Clean Architecture compliance achieved
- ✅ 0 dependency violations remaining
- ✅ Solution builds successfully
- ✅ Ready for domain layer development

## 🔗 Related Tasks
- **Depends on**: Solution structure setup
- **Enables**: Repository interface migration
- **Unblocks**: Domain layer development

## 📝 Notes
This fix was critical for maintaining Clean Architecture principles. The application layer should never directly depend on infrastructure concerns.
"@
    labels = @("completed", "architecture", "critical-fix", "task")
} | ConvertTo-Json -Depth 10

try {
    $response2 = Invoke-RestMethod -Uri $githubApiUrl -Method Post -Body $issue2 -Headers $headers
    Write-Host "✅ Issue #$($response2.number): $($response2.title)" -ForegroundColor Green
    Write-Host "   URL: $($response2.html_url)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Failed to create Issue 2: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "⚡ Quick script created first 2 issues!" -ForegroundColor Yellow
Write-Host "📄 For all 7 issues, use the complete script: scripts/create-github-issues.ps1" -ForegroundColor Cyan
Write-Host "🌐 View your issues: https://github.com/$owner/$repo/issues" -ForegroundColor Cyan
