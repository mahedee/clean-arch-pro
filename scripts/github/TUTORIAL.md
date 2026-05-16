# 📚 GitHub Issue Creation Tutorial

## 🎯 Overview

This tutorial will guide you through creating GitHub issues using our automated PowerShell script system. Learn how to create different types of issues efficiently and professionally.

## 🛠️ Prerequisites

Before starting, ensure you have:
- ✅ PowerShell 5.1 or later
- ✅ GitHub Personal Access Token configured
- ✅ Script files in place

## 📁 Script Structure

```
scripts/github/
├── create-github-issue.ps1      # Main script
├── create-all-issues.ps1        # Batch creation
├── config/
│   ├── config.json              # Settings
│   └── github-token.json        # Your GitHub token
└── simple-issues/               # Issue definitions
    ├── bug-*.json               # Bug reports
    ├── feature-*.json           # Feature requests
    ├── task-*.json              # Task definitions
    ├── doc-*.json               # Documentation tasks
    └── enhancement-*.json       # Enhancements
```

## 🚀 Quick Start Guide

### Step 1: Navigate to Script Directory
```powershell
cd d:\Projects\Github\clean-arch-pro\scripts\github
```

### Step 2: List Available Issues
```powershell
.\create-github-issue.ps1 -ListIssues
```

### Step 3: Test with Dry Run
```powershell
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json" -DryRun
```

### Step 4: Create Real Issue
```powershell
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json"
```

## 📋 Issue Types & Examples

### 🐛 Bug Reports

**Purpose**: Report software defects, errors, or unexpected behavior

**Example File**: `bug-student-validation.json`
```json
{
  "title": "🐛 Fix: Student registration validation error",
  "body": "## 🐛 Bug Report\n\n### Description\nStudent registration form is not properly validating email addresses...",
  "labels": ["bug", "high-priority", "student-management", "validation"],
  "assignees": ["mahedee"]
}
```

**How to Create**:
```powershell
# Test first
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json" -DryRun

# Create the bug report
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json"
```

**When to Use**:
- Application crashes or errors
- Incorrect functionality
- Data validation issues
- UI/UX problems

---

### ✨ Feature Requests

**Purpose**: Request new functionality or enhancements

**Example File**: `feature-attendance-analytics.json`
```json
{
  "title": "✨ Feature: Add attendance analytics dashboard",
  "body": "## ✨ Feature Request\n\n### Summary\nAdd a comprehensive analytics dashboard...",
  "labels": ["feature", "analytics", "attendance", "dashboard", "medium-priority"],
  "assignees": ["mahedee"]
}
```

**How to Create**:
```powershell
# Preview the feature request
.\create-github-issue.ps1 -IssueFile "feature-attendance-analytics.json" -DryRun

# Create the feature request
.\create-github-issue.ps1 -IssueFile "feature-attendance-analytics.json"
```

**When to Use**:
- New feature ideas
- User interface improvements
- Integration requests
- Performance enhancements

---

### 📋 Task Definitions

**Purpose**: Define development tasks and implementation work

**Example File**: `task-course-crud.json`
```json
{
  "title": "📋 Task: Implement course management CRUD operations",
  "body": "## 📋 Task\n\n### Description\nImplement complete CRUD operations for course management...",
  "labels": ["task", "course-management", "crud", "clean-architecture", "medium-priority"],
  "assignees": ["mahedee"]
}
```

**How to Create**:
```powershell
# Review the task details
.\create-github-issue.ps1 -IssueFile "task-course-crud.json" -DryRun

# Create the task
.\create-github-issue.ps1 -IssueFile "task-course-crud.json"
```

**When to Use**:
- Implementation tasks
- Code refactoring
- Architecture improvements
- Technical debt resolution

---

### 📚 Documentation Tasks

**Purpose**: Document features, APIs, and processes

**Example File**: `doc-student-api.json`
```json
{
  "title": "📚 Documentation: Update API documentation for student endpoints",
  "body": "## 📚 Documentation Task\n\n### Overview\nUpdate and improve API documentation...",
  "labels": ["documentation", "api", "student-management", "swagger", "medium-priority"],
  "assignees": ["mahedee"]
}
```

**How to Create**:
```powershell
# Check documentation requirements
.\create-github-issue.ps1 -IssueFile "doc-student-api.json" -DryRun

# Create documentation task
.\create-github-issue.ps1 -IssueFile "doc-student-api.json"
```

**When to Use**:
- API documentation updates
- User guides
- Technical specifications
- Code comments and README updates

---

### 🔧 Enhancement Issues

**Purpose**: Improve existing functionality or system architecture

**Example File**: `enhancement-error-handling.json`
```json
{
  "title": "🔧 Enhancement: Improve error handling across application layers",
  "body": "## 🔧 Enhancement\n\n### Summary\nImprove error handling consistency...",
  "labels": ["enhancement", "error-handling", "logging", "high-priority", "production-ready"],
  "assignees": ["mahedee"]
}
```

**How to Create**:
```powershell
# Review enhancement details
.\create-github-issue.ps1 -IssueFile "enhancement-error-handling.json" -DryRun

# Create enhancement issue
.\create-github-issue.ps1 -IssueFile "enhancement-error-handling.json"
```

**When to Use**:
- Code quality improvements
- Performance optimizations
- Security enhancements
- Architecture refinements

## 🎛️ Advanced Usage

### Batch Creation

Create multiple issues at once:
```powershell
# Test batch creation
.\create-all-issues.ps1 -DryRun

# Create all defined issues
.\create-all-issues.ps1
```

### Custom Configuration

Use different config files:
```powershell
.\create-github-issue.ps1 -ConfigFile ".\config\custom-config.json" -IssueFile "my-issue.json"
```

## 📝 Creating Your Own Issue Definitions

### Step 1: Create JSON File
Create a new file in `simple-issues/` folder:

```json
{
  "title": "🔒 Security: Implement JWT token refresh mechanism",
  "body": "## 🔒 Security Enhancement\n\n### Problem\nCurrent JWT tokens don't have refresh capability...\n\n### Solution\nImplement secure token refresh mechanism...",
  "labels": ["security", "authentication", "jwt", "high-priority"],
  "assignees": ["your-username"]
}
```

### Step 2: Test Your Issue
```powershell
.\create-github-issue.ps1 -IssueFile "security-jwt-refresh.json" -DryRun
```

### Step 3: Create the Issue
```powershell
.\create-github-issue.ps1 -IssueFile "security-jwt-refresh.json"
```

## 🏷️ Label Guidelines

### Priority Labels
- `high-priority` - Critical issues requiring immediate attention
- `medium-priority` - Important but not urgent
- `low-priority` - Nice to have, can be scheduled later

### Type Labels
- `bug` - Software defects
- `feature` - New functionality
- `enhancement` - Improvements to existing features
- `documentation` - Documentation related
- `task` - Development tasks

### Domain Labels
- `student-management` - Student-related features
- `course-management` - Course-related features
- `attendance` - Attendance tracking
- `analytics` - Data analysis and reporting
- `authentication` - Login and security
- `api` - API-related issues

## 🔧 Troubleshooting

### Common Issues

**Issue**: "GitHub token not found"
```powershell
# Solution: Check token file exists
Test-Path ".\config\github-token.json"
```

**Issue**: "Invalid JSON format"
```powershell
# Solution: Validate JSON syntax
Get-Content ".\simple-issues\your-issue.json" | ConvertFrom-Json
```

**Issue**: "API rate limit exceeded"
```powershell
# Solution: Wait or check your rate limit
# GitHub allows 5000 requests per hour for authenticated users
```

### Debug Mode

Run with verbose output:
```powershell
.\create-github-issue.ps1 -IssueFile "your-issue.json" -Verbose
```

## 📊 Best Practices

### 1. Always Use Dry Run First
```powershell
# ALWAYS test first
.\create-github-issue.ps1 -IssueFile "new-issue.json" -DryRun

# Then create for real
.\create-github-issue.ps1 -IssueFile "new-issue.json"
```

### 2. Use Descriptive Titles
- ✅ Good: "🐛 Fix: Student email validation accepts invalid formats"
- ❌ Bad: "Fix bug"

### 3. Include Emojis for Visual Recognition
- 🐛 for bugs
- ✨ for features
- 📋 for tasks
- 📚 for documentation
- 🔧 for enhancements
- 🔒 for security

### 4. Assign Appropriate Labels
Use multiple relevant labels for better categorization:
```json
"labels": ["bug", "high-priority", "student-management", "validation"]
```

### 5. Assign to Team Members
```json
"assignees": ["developer1", "developer2"]
```

## 🎯 Summary

This script system allows you to:
- ✅ Create professional GitHub issues quickly
- ✅ Maintain consistency across issue types
- ✅ Test before creating (dry run mode)
- ✅ Batch create multiple issues
- ✅ Customize for your specific needs

## 📞 Need Help?

- Check the logs: `github-issues.log`
- Review configuration: `config/config.json`
- Validate JSON: Use online JSON validators
- Test with dry run: Always use `-DryRun` first

---

**Happy Issue Creating!** 🚀

*Last Updated: September 6, 2025*
