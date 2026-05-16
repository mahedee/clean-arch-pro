# Scripts Directory

This directory contains automation scripts for the EduTrack project.

## GitHub Issues Creation

### 📋 Quick Start

1. **Get GitHub Token**: Run `.\setup-token.ps1` for detailed instructions
2. **Save Token**: Put your GitHub Personal Access Token in `token.txt`
3. **Create Issues**: Run `.\create-github-issues.ps1`

### 🔐 Security

- `token.txt` is in `.gitignore` - your token won't be committed
- Never share your personal access token publicly
- Tokens should have minimal required permissions (repo scope only)

### 📂 Available Scripts

| Script | Purpose |
|--------|---------|
| `setup-token.ps1` | Shows setup instructions for GitHub token |
| `create-github-issues.ps1` | Creates GitHub issues using token from token.txt |
| `create-test-coverage-issue.ps1` | Creates specific test coverage implementation issue |
| `quick-create-issues.ps1` | Interactive version (prompts for token) |
| `run-issue-creator.ps1` | Interactive wrapper script |

### 🚀 Usage Examples

```powershell
# Show setup instructions
.\scripts\setup-token.ps1

# Create all GitHub issues (after setting up token.txt)
.\scripts\create-github-issues.ps1

# Create specific test coverage issue
.\scripts\create-test-coverage-issue.ps1
```

### 📝 Token.txt Format

The `token.txt` file should contain only your GitHub Personal Access Token:

```
ghp_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

### ✅ Troubleshooting

- **"token.txt not found"**: Create the file in the scripts directory
- **"Invalid token"**: Check your token has 'repo' scope permissions
- **"Rate limited"**: Wait a few minutes and try again
- **"Repository not found"**: Verify the owner/repo settings in the script
