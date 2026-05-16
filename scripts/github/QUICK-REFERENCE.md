# 🚀 Quick Reference - GitHub Issue Creation

## 📋 Essential Commands

### List Available Issues
```powershell
.\create-github-issue.ps1 -ListIssues
```

### Test Issue Creation (Dry Run)
```powershell
.\create-github-issue.ps1 -IssueFile "your-issue.json" -DryRun
```

### Create Real Issue
```powershell
.\create-github-issue.ps1 -IssueFile "your-issue.json"
```

### Batch Create All Issues
```powershell
.\create-all-issues.ps1
```

## 🏷️ Issue Types & Files

| Type | Emoji | Example File | Usage |
|------|-------|--------------|--------|
| **Bug Report** | 🐛 | `bug-student-validation.json` | Software defects, errors |
| **Feature Request** | ✨ | `feature-attendance-analytics.json` | New functionality |
| **Task** | 📋 | `task-course-crud.json` | Development work |
| **Documentation** | 📚 | `doc-student-api.json` | Docs, guides, specs |
| **Enhancement** | 🔧 | `enhancement-error-handling.json` | Improvements |

## 📝 JSON Template

```json
{
  "title": "🐛 Fix: Brief description of the issue",
  "body": "## 🐛 Bug Report\n\n### Description\nDetailed description...",
  "labels": ["bug", "high-priority", "module-name"],
  "assignees": ["username"]
}
```

## 🎯 Common Labels

### Priority
- `high-priority` - Urgent
- `medium-priority` - Important  
- `low-priority` - Nice to have

### Type
- `bug` `feature` `enhancement` `documentation` `task`

### Domain
- `student-management` `course-management` `attendance` `analytics` `api`

## ⚡ Quick Workflow

1. **Navigate**: `cd scripts\github`
2. **List**: `.\create-github-issue.ps1 -ListIssues`
3. **Test**: `.\create-github-issue.ps1 -IssueFile "file.json" -DryRun`
4. **Create**: `.\create-github-issue.ps1 -IssueFile "file.json"`

## 🔧 Troubleshooting

| Problem | Solution |
|---------|----------|
| Token not found | Check `config\github-token.json` exists |
| Invalid JSON | Validate JSON syntax |
| Rate limit | Wait 1 hour or check usage |
| Permission denied | Verify GitHub token scopes |

---
**💡 Pro Tip**: Always use `-DryRun` first to preview your issue!
