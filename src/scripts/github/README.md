# GitHub Issue Creator - Simplified System

A streamlined PowerShell-based system for creating GitHub issues with direct JSON definitions, designed for the EduTrack Clean Architecture project.

## 🚀 Features

- **Simplified Design**: Direct JSON issue definitions - no complex templates
- **Fully Functional**: Successfully creates professional GitHub issues
- **Security-First**: Secure token handling with validation
- **Comprehensive Logging**: Detailed logging with color output
- **Dry Run Mode**: Test issue creation without actually creating issues
- **Multiple Issue Types**: Pre-built issues for bugs, features, tasks, docs, enhancements
- **Batch Creation**: Create multiple issues at once

## 📚 Documentation & Tutorials

- **[📖 TUTORIAL.md](TUTORIAL.md)** - Complete tutorial for creating different issue types
- **[⚡ QUICK-REFERENCE.md](QUICK-REFERENCE.md)** - Quick commands and reference
- **[👨‍💻 EXAMPLES.md](EXAMPLES.md)** - Step-by-step examples and walkthroughs
- **[✅ AUTOMATION-SUCCESS.md](AUTOMATION-SUCCESS.md)** - System overview and achievements
- **[🔧 Configuration Guide](config/config.json)** - JSON configuration options

## 📁 Directory Structure

```
scripts/github/
├── create-github-issue.ps1     # Main script (simplified)
├── create-all-issues.ps1       # Batch creation script
├── config/
│   ├── config.json             # Configuration settings
│   └── github-token.json       # Secure token storage
├── simple-issues/              # Direct issue definitions
│   ├── bug-*.json             # Bug reports
│   ├── feature-*.json         # Feature requests
│   ├── task-*.json            # Development tasks
│   ├── doc-*.json             # Documentation tasks
│   └── enhancement-*.json     # System enhancements
└── archive/                    # Old complex system (archived)
│   └── task-completion.json     # Task completion template
├── issues/
│   ├── task-t001a-completion.json       # T001A completion issue
│   └── database-seeding-feature.json    # Database seeding feature
├── create-github-issue.ps1      # Main script
├── .gitignore                   # Security exclusions
└── README.md                    # This file
```

## 🎓 Getting Started

### 📖 New to the System? Start Here!

1. **[Read the Tutorial](TUTORIAL.md)** - Complete guide for beginners
2. **[Quick Reference](QUICK-REFERENCE.md)** - Essential commands
3. **[See Examples](EXAMPLES.md)** - Step-by-step walkthroughs

### ⚡ Quick Start

```powershell
# Navigate to scripts directory
cd scripts\github

# List available issues
.\create-github-issue.ps1 -ListIssues

# Test creating an issue (dry run)
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json" -DryRun

# Create the actual issue
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json"
```

## 🛠️ Setup

### 1. GitHub Token Setup

Create a GitHub Personal Access Token with `repo` scope:

1. Go to GitHub → Settings → Developer settings → Personal access tokens
2. Generate new token with `repo` scope
3. Create `config/github-token.json`:

```json
{
  "github_token": "your_github_token_here"
}
```

### 2. Configuration

The system uses `config/config.json` for settings (already configured for this project).

## 📋 Usage

### Basic Commands

```powershell
# List available issue definitions
.\create-github-issue.ps1 -ListIssues

# Create single issue with dry run test
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json" -DryRun
.\create-github-issue.ps1 -IssueFile "bug-student-validation.json"

# Create multiple issues at once
.\create-all-issues.ps1
```

### Information Commands

```powershell
# List available templates
.\create-github-issue.ps1 -ListTemplates

# List available issue definitions
.\create-github-issue.ps1 -ListIssues

# Verbose output
.\create-github-issue.ps1 -IssueFile "example.json" -Verbose
```

## 📝 Creating Issue Definitions

Issue definitions combine templates with specific data:

```json
{
  "template": "task-completion.json",
  "variables": {
    "task_name": "Your Task Name",
    "task_id": "T001A",
    "phase": "Phase 1: Foundation",
    "completed_by": "Developer Name",
    "work_performed": "Detailed description of work..."
  },
  "additionalLabels": [
    "custom-label",
    "project-specific"
  ]
}
```

## 🎨 Creating Templates

Templates use `{{variable}}` syntax for substitution:

```json
{
  "title": "{{issue_title}}",
  "body": "## Description\n\n{{description}}\n\n## Additional Info\n\n{{additional_info}}",
  "labels": ["{{priority}}", "{{type}}"],
  "variables": [
    "issue_title",
    "description", 
    "additional_info",
    "priority",
    "type"
  ]
}
```

## 🔧 Configuration Options

### GitHub Settings
- `owner`: Repository owner
- `repository`: Repository name
- `apiUrl`: GitHub API endpoint
- `tokenFile`: Token file location

### Issue Settings
- `defaultPriority`: Default issue priority
- `templateDirectory`: Template storage location
- `issueDirectory`: Issue definition storage location

### Validation Rules
- `requireTitle`: Title is mandatory
- `requireBody`: Body is mandatory
- `minTitleLength`: Minimum title length
- `maxTitleLength`: Maximum title length
- `minBodyLength`: Minimum body length

### Security Settings
- `validateToken`: Enable token validation
- `tokenMinLength`: Minimum token length
- `allowedScopes`: Required token scopes

## 📊 Logging and Output

The system provides comprehensive logging:

- **Console Output**: Color-coded messages
- **Log File**: Detailed execution logs
- **Results Export**: JSON output of created issues

### Log Levels
- `INFO`: General information
- `WARN`: Warnings
- `ERROR`: Errors
- `DEBUG`: Detailed debugging (with -Verbose)

## 🛡️ Security Features

- **Token Protection**: Tokens excluded from git via `.gitignore`
- **Validation**: Token and permission validation
- **Secure Storage**: Separate token files
- **Environment Variables**: Support for `GITHUB_TOKEN` env var

## 📋 Available Templates

### 1. Default Issue (`default-issue.json`)
General-purpose issue template with basic fields.

### 2. Bug Report (`bug-report.json`)
Structured bug reporting with reproduction steps.

### 3. Feature Request (`feature-request.json`)
Comprehensive feature request with technical details.

### 4. Task Completion (`task-completion.json`)
Task completion documentation with deliverables.

## 🚨 Error Handling

The system includes comprehensive error handling:

- Configuration validation
- Token validation
- Template parsing
- API communication
- Data validation

## 🎯 Examples

### Example 1: Task Completion
```powershell
.\create-github-issue.ps1 -IssueFile "task-t001a-completion.json"
```

### Example 2: Feature Request with Dry Run
```powershell
.\create-github-issue.ps1 -IssueFile "database-seeding-feature.json" -DryRun -Verbose
```

### Example 3: Validation Only
```powershell
.\create-github-issue.ps1 -IssueFile "my-issue.json" -ValidateOnly
```

## 🔄 Integration with Existing Scripts

This enhanced system complements the existing `create-github-issues.ps1` script by providing:

- More structured approach
- Better configuration management
- Enhanced security
- Template reusability
- Comprehensive validation

## 🚧 Troubleshooting

### Common Issues

1. **Token Not Found**
   - Ensure `github-token.txt` exists
   - Or set `GITHUB_TOKEN` environment variable

2. **Permission Denied**
   - Verify token has `repo` scope
   - Check repository access permissions

3. **Template Not Found**
   - Verify template file exists in `templates/` directory
   - Check file name in issue definition

4. **Validation Errors**
   - Review validation settings in config
   - Check issue data against validation rules

## 📈 Future Enhancements

- Interactive mode for template variable input
- Bulk issue creation
- Issue template inheritance
- Custom validation rules
- Integration with project management tools

## 🤝 Contributing

When adding new templates or features:

1. Follow existing naming conventions
2. Update configuration if needed
3. Add validation rules as appropriate
4. Include comprehensive error handling
5. Update this README

## 📄 License

This system is part of the EduTrack Clean Architecture project and follows the same licensing terms.

---

**Created**: 2024-01-15  
**Version**: 1.0  
**Author**: GitHub Copilot Assistant
