# GitHub Issue Templates and Samples - Quick Reference

This document provides a quick reference for all available issue templates and sample issues in the EduTrack project.

## 📁 Template Structure

### Templates Location: `scripts/github/templates/`
- `bug-report.json` - Bug reporting with detailed reproduction steps
- `feature-request.json` - Feature requests with technical requirements
- `task-completion.json` - Task completion documentation
- `default-issue.json` - General purpose issue template
- `documentation-improvement.json` - Documentation enhancement requests
- `general-issue.json` - Discussion and general concerns

### Sample Issues Location: `scripts/github/issues/`

## 🐛 Bug Report Samples

### 1. `sample-bug-report.json`
**Title**: Student Registration Form Validation Not Working  
**Severity**: High  
**Focus**: Form validation, data integrity  
**Labels**: validation, frontend, student-management, data-integrity

### 2. `sample-memory-leak-bug.json`
**Title**: Memory Leak in Student Performance Analytics Dashboard  
**Severity**: High  
**Focus**: Performance, memory management  
**Labels**: memory-leak, performance, analytics, frontend, charts

## ✨ Feature Request Samples

### 1. `sample-analytics-feature.json`
**Title**: Advanced Student Performance Analytics Dashboard  
**Priority**: High  
**Focus**: Analytics, reporting, data visualization  
**Labels**: analytics, dashboard, reporting, performance, frontend, backend

### 2. `sample-mobile-dashboard-feature.json`
**Title**: Mobile-First Responsive Design for Teacher Dashboard  
**Priority**: High  
**Focus**: Mobile UX, responsive design  
**Labels**: mobile, responsive-design, teacher-dashboard, ux-improvement, accessibility

## ✅ Task Completion Samples

### 1. `task-t001a-completion.json`
**Task**: GitHub Workflow & Copilot Setup (T001A)  
**Status**: Completed  
**Focus**: GitHub automation, CI/CD  
**Labels**: task-completion, github-workflow, copilot, foundation

### 2. `sample-task-completion.json`
**Task**: Domain Layer Foundation Implementation (T002)  
**Status**: Completed  
**Focus**: Clean Architecture, domain modeling  
**Labels**: domain-layer, clean-architecture, entities, value-objects, foundation

## 📚 Documentation Samples

### 1. `sample-documentation-improvement.json`
**Title**: API Documentation Enhancement for EduTrack REST Endpoints  
**Priority**: Medium  
**Focus**: API documentation, developer experience  
**Labels**: api, swagger, developer-experience, integration

## 🛠️ General Issue Samples

### 1. `sample-general-discussion.json`
**Title**: Database Performance Optimization Strategy Discussion  
**Impact**: High  
**Focus**: Performance planning, architecture  
**Labels**: database, performance, scalability, architecture, planning

### 2. `sample-notification-system.json`
**Title**: Implement Real-time Notification System for Student Activities  
**Priority**: Medium  
**Focus**: Real-time features, communication  
**Labels**: notifications, real-time, signalr, communication, scalability

## 🚀 Usage Examples

### Create Issue from Sample
```powershell
# Bug report
.\create-github-issue.ps1 -IssueFile "sample-bug-report.json"

# Feature request
.\create-github-issue.ps1 -IssueFile "sample-analytics-feature.json"

# Task completion
.\create-github-issue.ps1 -IssueFile "sample-task-completion.json"

# Documentation improvement
.\create-github-issue.ps1 -IssueFile "sample-documentation-improvement.json"

# General discussion
.\create-github-issue.ps1 -IssueFile "sample-general-discussion.json"
```

### Dry Run Testing
```powershell
# Test any sample without creating actual issue
.\create-github-issue.ps1 -IssueFile "sample-mobile-dashboard-feature.json" -DryRun
```

### Validation Only
```powershell
# Validate issue data without creation
.\create-github-issue.ps1 -IssueFile "sample-memory-leak-bug.json" -ValidateOnly
```

## 📋 Template Variables Reference

### Common Variables (All Templates)
- `timestamp` - Automatically generated
- `priority` - low, medium, high, critical
- `issue_type` - bug, enhancement, task, documentation

### Bug Report Variables
- `bug_title`, `bug_description`, `severity`
- `step_1`, `step_2`, `step_3` - Reproduction steps
- `expected_behavior`, `actual_behavior`
- `os`, `browser`, `version` - Environment details

### Feature Request Variables
- `feature_title`, `feature_description`
- `problem_statement`, `proposed_solution`
- `technical_requirements`, `affected_components`
- `criteria_1`, `criteria_2`, `criteria_3` - Acceptance criteria

### Task Completion Variables
- `task_name`, `task_id`, `phase`, `completed_by`
- `work_performed`, `deliverables`, `files_changed`
- `verification_steps`, `next_steps`, `quality_level`

## 🎯 Template Selection Guide

| Issue Type | Template | Best For |
|------------|----------|----------|
| 🐛 Bug | `bug-report.json` | Defects, errors, unexpected behavior |
| ✨ Feature | `feature-request.json` | New functionality, enhancements |
| ✅ Task | `task-completion.json` | Project task documentation |
| 📚 Docs | `documentation-improvement.json` | Documentation updates |
| 🛠️ General | `general-issue.json` | Discussions, questions, planning |
| 🎯 Custom | `default-issue.json` | Flexible general-purpose issues |

## 🔧 Customization Tips

1. **Copy and Modify**: Start with existing samples and modify variables
2. **Add Labels**: Use `additionalLabels` for project-specific tags
3. **Template Variables**: All `{{variable}}` placeholders must be defined
4. **Validation**: Use `-ValidateOnly` to check data before creation
5. **Testing**: Always use `-DryRun` for testing new issue definitions

---

**Quick Start**: Copy any sample file, modify the variables, and run with the PowerShell script!

**Need Help?** Check the main README.md in the `scripts/github/` directory for detailed documentation.
