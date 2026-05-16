# 👨‍💻 Step-by-Step Examples - GitHub Issue Creation

## 🎯 Scenario-Based Walkthroughs

### Example 1: Creating a Bug Report 🐛

**Scenario**: You found a bug where student email validation isn't working properly.

#### Step 1: Navigate to Scripts Directory
```powershell
cd d:\Projects\Github\clean-arch-pro\scripts\github
```

#### Step 2: Check What Issues Are Available
```powershell
PS > .\create-github-issue.ps1 -ListIssues

🚀 GitHub Issue Creator - Simplified System
===========================================
Available Issue Definitions:
=============================
  - bug-student-validation.json    ← Perfect for our bug!
  - feature-attendance-analytics.json
  - task-course-crud.json
```

#### Step 3: Preview the Bug Report (Dry Run)
```powershell
PS > .\create-github-issue.ps1 -IssueFile "bug-student-validation.json" -DryRun

🚀 GitHub Issue Creator - Simplified System
===========================================
[INFO] Token validation successful. Authenticated as: mahedee
[INFO] DRY RUN - Would create issue with data:
{
  "title": "🐛 Fix: Student registration validation error",
  "body": "## 🐛 Bug Report\n\n### Description\nStudent registration form is not properly validating email addresses...",
  "labels": ["bug", "high-priority", "student-management", "validation"],
  "assignees": ["mahedee"]
}
✅ Issue creation completed successfully!
Issue #DRY-RUN: https://github.com/dry-run
```

#### Step 4: Create the Actual Bug Report
```powershell
PS > .\create-github-issue.ps1 -IssueFile "bug-student-validation.json"

🚀 GitHub Issue Creator - Simplified System
===========================================
[INFO] Token validation successful. Authenticated as: mahedee
[INFO] Issue created successfully: #45
✅ Issue creation completed successfully!
Issue #45: https://github.com/mahedee/clean-arch-pro/issues/45
```

#### Step 5: Verify on GitHub
Open the URL to see your professionally formatted bug report!

---

### Example 2: Requesting a New Feature ✨

**Scenario**: You want to add a dashboard for viewing attendance analytics.

#### Step 1: Preview the Feature Request
```powershell
PS > .\create-github-issue.ps1 -IssueFile "feature-attendance-analytics.json" -DryRun

[INFO] DRY RUN - Would create issue with data:
{
  "title": "✨ Feature: Add attendance analytics dashboard",
  "body": "## ✨ Feature Request\n\n### Summary\nAdd a comprehensive analytics dashboard for tracking student attendance patterns...",
  "labels": ["feature", "analytics", "attendance", "dashboard", "medium-priority"],
  "assignees": ["mahedee"]
}
```

#### Step 2: Create the Feature Request
```powershell
PS > .\create-github-issue.ps1 -IssueFile "feature-attendance-analytics.json"

✅ Issue creation completed successfully!
Issue #46: https://github.com/mahedee/clean-arch-pro/issues/46
```

---

### Example 3: Creating a Custom Issue 📝

**Scenario**: You need to create a security enhancement issue that's not in the predefined list.

#### Step 1: Create Your Own Issue Definition
Create `simple-issues/security-jwt-refresh.json`:
```json
{
  "title": "🔒 Security: Implement JWT token refresh mechanism",
  "body": "## 🔒 Security Enhancement\n\n### Problem\nCurrent JWT tokens don't have refresh capability, forcing users to re-login frequently.\n\n### Solution\nImplement secure token refresh mechanism:\n- [ ] Add refresh token endpoint\n- [ ] Implement token rotation\n- [ ] Add security validation\n- [ ] Update client-side handling\n\n### Security Considerations\n- Use secure HttpOnly cookies for refresh tokens\n- Implement proper token rotation\n- Add rate limiting to prevent abuse\n\n### Acceptance Criteria\n- [ ] Users can refresh expired tokens without re-login\n- [ ] Refresh tokens expire after 7 days of inactivity\n- [ ] Security audit passes\n- [ ] Unit tests cover all scenarios",
  "labels": ["security", "authentication", "jwt", "high-priority", "enhancement"],
  "assignees": ["mahedee"]
}
```

#### Step 2: Test Your Custom Issue
```powershell
PS > .\create-github-issue.ps1 -IssueFile "security-jwt-refresh.json" -DryRun

[INFO] DRY RUN - Would create issue with data:
{
  "title": "🔒 Security: Implement JWT token refresh mechanism",
  "body": "## 🔒 Security Enhancement\n\n### Problem\nCurrent JWT tokens don't have refresh capability...",
  "labels": ["security", "authentication", "jwt", "high-priority", "enhancement"],
  "assignees": ["mahedee"]
}
```

#### Step 3: Create Your Custom Security Issue
```powershell
PS > .\create-github-issue.ps1 -IssueFile "security-jwt-refresh.json"

✅ Issue creation completed successfully!
Issue #47: https://github.com/mahedee/clean-arch-pro/issues/47
```

---

### Example 4: Batch Creating Multiple Issues 🚀

**Scenario**: You want to create several issues at once for a sprint planning session.

#### Step 1: Test Batch Creation
```powershell
PS > .\create-all-issues.ps1 -DryRun

🚀 Batch Creating GitHub Issues
=================================

📝 Creating issue: doc-student-api.json
[INFO] DRY RUN - Would create issue...
✅ Success: doc-student-api.json

📝 Creating issue: enhancement-error-handling.json
[INFO] DRY RUN - Would create issue...
✅ Success: enhancement-error-handling.json

📊 Batch Creation Summary
=========================
Total Issues: 2
Created Successfully: 2
Failed: 0
```

#### Step 2: Create All Issues
```powershell
PS > .\create-all-issues.ps1

🚀 Batch Creating GitHub Issues
=================================

📝 Creating issue: doc-student-api.json
✅ Issue creation completed successfully!
Issue #48: https://github.com/mahedee/clean-arch-pro/issues/48

📝 Creating issue: enhancement-error-handling.json
✅ Issue creation completed successfully!
Issue #49: https://github.com/mahedee/clean-arch-pro/issues/49

📊 Batch Creation Summary
=========================
Total Issues: 2
Created Successfully: 2
Failed: 0

🎉 Batch creation completed!
```

---

### Example 5: Handling Errors 🔧

**Scenario**: Something goes wrong and you need to debug.

#### Common Error: Invalid JSON
```powershell
PS > .\create-github-issue.ps1 -IssueFile "broken-issue.json"

❌ Error: Failed to parse issue definition: Invalid JSON syntax at line 5
```

**Solution**: Validate your JSON
```powershell
# Test JSON syntax
PS > Get-Content ".\simple-issues\broken-issue.json" | ConvertFrom-Json
```

#### Common Error: Token Issues
```powershell
❌ Error: GitHub token not found. Please create 'config\github-token.json'
```

**Solution**: Check token configuration
```powershell
# Verify token file exists
PS > Test-Path ".\config\github-token.json"

# Check token content
PS > Get-Content ".\config\github-token.json"
```

#### Common Error: Rate Limiting
```powershell
❌ Error: API rate limit exceeded. Please try again later.
```

**Solution**: Wait or check your rate limit status
```powershell
# Check current rate limit (requires curl or Invoke-RestMethod)
# GitHub allows 5000 requests per hour for authenticated users
```

---

## 🎯 Pro Tips for Success

### 1. Always Dry Run First
```powershell
# This workflow prevents mistakes:
.\create-github-issue.ps1 -IssueFile "my-issue.json" -DryRun  # Preview
.\create-github-issue.ps1 -IssueFile "my-issue.json"         # Create
```

### 2. Use Descriptive File Names
```
✅ Good: bug-student-email-validation.json
✅ Good: feature-attendance-dashboard.json
✅ Good: task-implement-course-crud.json

❌ Bad: issue1.json
❌ Bad: bug.json
❌ Bad: new-feature.json
```

### 3. Include Rich Details in Issue Body
```json
{
  "body": "## 🐛 Bug Report\n\n### Description\nDetailed problem description\n\n### Steps to Reproduce\n1. Step one\n2. Step two\n\n### Expected Behavior\nWhat should happen\n\n### Actual Behavior\nWhat actually happens\n\n### Environment\n- OS: Windows 11\n- Browser: Chrome 115+\n\n### Priority\nHigh - Affects user registration"
}
```

### 4. Use Consistent Labels
```json
{
  "labels": [
    "bug",                    // Type
    "high-priority",          // Priority
    "student-management",     // Domain
    "validation"              // Technical area
  ]
}
```

---

## 🎉 Summary

You now know how to:
- ✅ Create different types of issues (bugs, features, tasks, docs, enhancements)
- ✅ Use dry run mode to preview issues
- ✅ Create custom issue definitions
- ✅ Batch create multiple issues
- ✅ Handle common errors and troubleshoot problems

**Next Steps**: Start creating issues for your project using these examples as templates!

---

*Last Updated: September 6, 2025*
