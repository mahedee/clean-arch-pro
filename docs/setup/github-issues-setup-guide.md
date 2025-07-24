# Complete Guide: Creating GitHub Issues from VS Code

## 🎯 Overview
This guide will walk you through installing and configuring everything needed to create GitHub issues directly from VS Code using multiple methods. This guide is specifically designed for the EduTrack project and includes automated scripts for bulk issue creation from your task list.

## 📋 Quick Start (Recommended for EduTrack)
If you want to quickly create GitHub issues for your completed tasks, use our automated scripts:

### **Method A: Automated Script (Easiest)**
```powershell
# 1. Get GitHub token from: https://github.com/settings/tokens
# 2. Run the automated setup script
.\scripts\setup-and-run.ps1 -Token "your_github_token_here"
```

### **Method B: Manual Script Execution**
```powershell
# 1. Edit scripts\create-github-issues.ps1
# 2. Replace "YOUR_GITHUB_TOKEN_HERE" with your token
# 3. Run the script
.\scripts\create-github-issues.ps1
```

Both methods will create **7 professional GitHub issues** for all your completed EduTrack tasks automatically.

---

## 🚀 EduTrack Task List Integration

### **Creating Issues from Completed Tasks**

Our project includes automated scripts to convert your completed tasks from `docs/task-list.md` into professional GitHub issues.

#### **Script Features:**
- ✅ **Automated Issue Creation**: Creates 7 issues for all completed T001 tasks
- ✅ **Professional Formatting**: Each issue includes metrics, technical details, and proper labels
- ✅ **Security First**: Automatically removes your token after use
- ✅ **Error Handling**: Comprehensive error reporting and troubleshooting

#### **What Issues Get Created:**
1. **Clean Architecture Solution Structure Setup** (T001-1)
2. **Architecture Dependency Violations Fixed** (T001-2)  
3. **Repository Interfaces Moved to Domain Layer** (T001-3)
4. **Infrastructure Reference Added to API Layer** (T001-4)
5. **Missing Test Projects Created** (T001-5)
6. **EditorConfig and Code Style Rules** (T001-6)
7. **Git Repository with Enhanced .gitignore** (T001-7)

#### **Each Issue Includes:**
- 📋 **Task Overview**: ID, Sprint, Status, Duration
- 📊 **Impact Metrics**: Quantified achievements and improvements
- 🔧 **Technical Implementation**: Framework, patterns, and architecture details
- 🔗 **Task Dependencies**: Relationships to other tasks
- 📝 **Detailed Notes**: Context and reasoning behind changes

### **Available Scripts:**

#### **1. Interactive Setup Script** (`scripts/setup-and-run.ps1`)
```powershell
# Run with your GitHub token
.\scripts\setup-and-run.ps1 -Token "your_github_token_here"

# Or run without token for instructions
.\scripts\setup-and-run.ps1
```

**Features:**
- Interactive token setup
- Automatic security cleanup
- Confirmation prompts
- Comprehensive error handling

#### **2. Direct Issue Creator** (`scripts/create-github-issues.ps1`)
```powershell
# Edit the script first to add your token, then run
.\scripts\create-github-issues.ps1
```

**Features:**
- Direct execution
- Detailed progress reporting
- Rate limiting for API calls
- Professional issue formatting

#### **3. Manual Issue Templates** (`docs/github-issues/completed-tasks-issues.md`)
Contains ready-to-copy issue content if you prefer manual creation.

---

## 📋 Method 1: GitHub CLI (gh) - Recommended

### Step 1: Install GitHub CLI

#### Option A: Using Chocolatey (Recommended for Windows)
```powershell
# Install Chocolatey first (if not installed)
Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install GitHub CLI
choco install gh

# Verify installation
gh --version
```

#### Option B: Using winget (Windows 10+)
```powershell
# Install GitHub CLI
winget install GitHub.cli

# Verify installation
gh --version
```

#### Option C: Manual Download
1. Go to: https://github.com/cli/cli/releases/latest
2. Download `gh_X.X.X_windows_amd64.msi`
3. Run the installer
4. Restart your terminal/VS Code

### Step 2: Authenticate GitHub CLI
```powershell
# Login to GitHub
gh auth login

# Follow the prompts:
# 1. Choose "GitHub.com"
# 2. Choose "HTTPS"
# 3. Choose "Login with a web browser"
# 4. Copy the one-time code
# 5. Open browser and paste the code
# 6. Authorize GitHub CLI

# Verify authentication
gh auth status
```

### Step 3: Test GitHub CLI
```powershell
# Navigate to your repository
cd "d:\Projects\Github\clean-arch-pro"

# Test creating an issue
gh issue create --title "Test Issue" --body "This is a test issue created from CLI"

# List issues
gh issue list

# View issue in browser
gh issue view 1 --web
```

---

## 📋 Method 2: VS Code GitHub Extension

### Step 1: Install GitHub Pull Requests and Issues Extension
1. Open VS Code
2. Go to Extensions (Ctrl+Shift+X)
3. Search for "GitHub Pull Requests and Issues"
4. Install the extension by GitHub
5. Reload VS Code

### Step 2: Sign in to GitHub
1. Press `Ctrl+Shift+P` to open Command Palette
2. Type "GitHub: Sign In"
3. Select the command
4. Follow the authentication flow in browser

### Step 3: Configure the Extension
1. Press `Ctrl+Shift+P`
2. Type "GitHub Issues: Configure"
3. Select your repository
4. Set up issue queries and labels

### Step 4: Create Issues from VS Code
1. Press `Ctrl+Shift+P`
2. Type "GitHub Issues: Create Issue"
3. Fill in the issue template
4. Submit

---

## 📋 Method 3: VS Code Terminal with PowerShell Scripts

### Step 1: Setup PowerShell Execution Policy
```powershell
# Allow script execution (run as Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Verify
Get-ExecutionPolicy
```

### Step 2: Get GitHub Personal Access Token
1. Go to: https://github.com/settings/tokens
2. Click "Generate new token (classic)"
3. Set expiration (30 days recommended for testing)
4. Select scopes:
   - ✅ `repo` (Full control of private repositories)
   - ✅ `write:org` (if working with organization repos)
5. Click "Generate token"
6. **IMPORTANT**: Copy the token immediately (you won't see it again)

### Step 3: Configure Your PowerShell Script
```powershell
# Navigate to your project
cd "d:\Projects\Github\clean-arch-pro"

# Edit the script to add your token
code scripts\create-github-issues.ps1

# Replace "YOUR_GITHUB_TOKEN_HERE" with your actual token
```

### Step 4: Run the Script
```powershell
# Execute the script
.\scripts\create-github-issues.ps1

# Or use the quick version for testing
.\scripts\quick-create-issues.ps1
```

---

## 📋 Method 4: VS Code REST Client Extension

### Step 1: Install REST Client Extension
1. Open VS Code Extensions (Ctrl+Shift+X)
2. Search for "REST Client"
3. Install the extension by Huachao Mao
4. Reload VS Code

### Step 2: Create REST API File
```http
### Create GitHub Issue
POST https://api.github.com/repos/mahedee/clean-arch-pro/issues
Authorization: token YOUR_GITHUB_TOKEN_HERE
Content-Type: application/json

{
  "title": "✅ COMPLETED - Clean Architecture Solution Structure Setup",
  "body": "## 🎯 Task Overview\n**Task ID**: T001-1  \n**Sprint**: 1  \n**Status**: ✅ COMPLETED  \n\nSuccessfully created the foundational solution structure following Clean Architecture principles.",
  "labels": ["completed", "architecture", "task", "foundation"]
}

### List Issues
GET https://api.github.com/repos/mahedee/clean-arch-pro/issues
Authorization: token YOUR_GITHUB_TOKEN_HERE
```

### Step 3: Execute REST Requests
1. Save the file as `github-api.http`
2. Replace `YOUR_GITHUB_TOKEN_HERE` with your token
3. Click "Send Request" above each HTTP request

---

## 📋 Method 5: Command Line with curl

### Step 1: Verify curl Installation
```powershell
# Check if curl is available
curl --version

# If not available, curl comes with Windows 10+ by default
# For older versions, download from: https://curl.se/windows/
```

### Step 2: Create Issues with curl
```powershell
# Set your GitHub token as environment variable
$env:GITHUB_TOKEN = "your_github_token_here"

# Create an issue
curl -X POST `
  -H "Authorization: token $env:GITHUB_TOKEN" `
  -H "Accept: application/vnd.github.v3+json" `
  -H "Content-Type: application/json" `
  https://api.github.com/repos/mahedee/clean-arch-pro/issues `
  -d '{
    "title": "✅ COMPLETED - Test Issue from curl",
    "body": "This issue was created using curl from command line",
    "labels": ["test", "completed"]
  }'
```

---

## 🔧 Troubleshooting

### **EduTrack-Specific Issues**

#### **1. "YOUR_GITHUB_TOKEN_HERE" Error**
```powershell
# Error: "❌ ERROR: Please set your GitHub Personal Access Token in the script"
# Solution: Use the automated setup script
.\scripts\setup-and-run.ps1 -Token "your_actual_token_here"

# Or manually edit scripts\create-github-issues.ps1
# Replace "YOUR_GITHUB_TOKEN_HERE" with your actual token
```

#### **2. Script Execution Blocked**
```powershell
# Error: "execution of scripts is disabled on this system"
# Solution: Set execution policy (run as Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

#### **3. Token Permission Issues**
```
# Error: "HTTP 403: Forbidden" or "Bad credentials"
# Solution: Check your GitHub token settings
```
**Required Token Settings:**
- ✅ **Type**: Classic Personal Access Token
- ✅ **Scope**: `repo` (Full control of private repositories)
- ✅ **Not Expired**: Check expiration date
- ✅ **Correct Repository**: Ensure token has access to `mahedee/clean-arch-pro`

#### **4. Issues Already Exist**
```
# Error: "HTTP 422: Validation Failed" - might indicate duplicate issues
# Solution: Check existing issues first
```
- Go to: https://github.com/mahedee/clean-arch-pro/issues
- Look for existing issues with same titles
- Delete duplicates if needed before re-running script

#### **5. Network/API Rate Limiting**
```
# Error: "HTTP 403: API rate limit exceeded"
# Solution: Wait 1 hour or use authenticated requests
```
The script includes rate limiting (1-second delays), but if you hit limits:
- Wait 1 hour for rate limit reset
- Ensure you're using an authenticated token
- Run script during off-peak hours

### **General Issues and Solutions**

#### **6. GitHub CLI Not Found**
```powershell
# Error: "gh: The term 'gh' is not recognized"
# Solution: Install GitHub CLI
choco install gh
# OR
winget install GitHub.cli
# Then restart terminal/VS Code
```

#### **7. PowerShell Script Won't Run**
```powershell
# Error: Various PowerShell execution errors
# Solutions:
# 1. Run PowerShell as Administrator
# 2. Set execution policy
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
# 3. Unblock downloaded scripts
Unblock-File -Path "scripts\*.ps1"
```

#### **8. VS Code Extension Authentication**
```
# Error: VS Code GitHub extension not working
# Solutions:
```
1. **Sign out and sign in again**:
   - `Ctrl+Shift+P` → "GitHub: Sign Out"
   - `Ctrl+Shift+P` → "GitHub: Sign In"
2. **Check repository configuration**:
   - Ensure you're in the correct repository folder
   - Verify remote origin is set correctly
3. **Restart VS Code** after authentication

#### **9. curl SSL Certificate Errors**
```powershell
# Error: SSL certificate verification errors
# Solution: Use -k flag (not recommended for production)
curl -k -X POST ...
# Better solution: Update certificates or use proper SSL
```

#### **10. Token Security Concerns**
```
# Issue: Token accidentally committed to repository
# Solution: Immediately revoke and create new token
```
1. **Revoke compromised token**: Go to GitHub settings → Tokens → Delete
2. **Remove from git history**: 
   ```powershell
   git filter-branch --force --index-filter 'git rm --cached --ignore-unmatch scripts/create-github-issues.ps1' HEAD
   ```
3. **Create new token** and update scripts
4. **Force push** (if repository is private): `git push --force`

### **🔍 Debugging Steps**

#### **Step 1: Verify Prerequisites**
```powershell
# Check PowerShell version (should be 5.1+ or 7+)
$PSVersionTable.PSVersion

# Check execution policy
Get-ExecutionPolicy

# Test internet connectivity
Test-NetConnection github.com -Port 443
```

#### **Step 2: Test GitHub API Access**
```powershell
# Test basic API access
curl -H "Authorization: token your_token_here" https://api.github.com/user

# Test repository access
curl -H "Authorization: token your_token_here" https://api.github.com/repos/mahedee/clean-arch-pro
```

#### **Step 3: Validate Script Files**
```powershell
# Check if all required scripts exist
ls scripts\*.ps1

# Expected files:
# - create-github-issues.ps1
# - setup-and-run.ps1
# - quick-create-issues.ps1 (optional)
```

#### **Step 4: Run with Verbose Output**
```powershell
# Enable verbose output for debugging
$VerbosePreference = "Continue"
.\scripts\create-github-issues.ps1
```

### **🆘 Emergency Procedures**

#### **If Scripts Fail Completely:**
1. **Use Manual Issue Creation**:
   - Open: `docs/github-issues/completed-tasks-issues.md`
   - Copy and paste each issue manually into GitHub
   - Go to: https://github.com/mahedee/clean-arch-pro/issues/new

2. **Use REST Client in VS Code**:
   - Install "REST Client" extension
   - Create a `.http` file with GitHub API calls
   - Execute requests manually

3. **Use GitHub CLI (if available)**:
   ```powershell
   gh issue create --title "Issue Title" --body "Issue body content"
   ```

#### **If Token is Compromised:**
1. **Immediate Actions**:
   - Go to: https://github.com/settings/tokens
   - Delete the compromised token immediately
   - Create a new token with same permissions
   - Update all scripts with new token

2. **Repository Cleanup**:
   - Check git history for token exposure
   - Use git filter-branch if needed
   - Force push to remove sensitive data (private repos only)

### **📞 Getting Help**

#### **Log Collection for Support:**
```powershell
# Collect system information
Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion, PowerShellVersion
$PSVersionTable
Get-ExecutionPolicy -List

# Test GitHub connectivity
Test-NetConnection github.com -Port 443
Test-NetConnection api.github.com -Port 443

# Save to file for support
$info = @{
    System = Get-ComputerInfo | Select-Object WindowsProductName, WindowsVersion
    PowerShell = $PSVersionTable
    ExecutionPolicy = Get-ExecutionPolicy -List
    GitHubConnectivity = Test-NetConnection github.com -Port 443
}
$info | ConvertTo-Json | Out-File "debug-info.json"
```

#### **Common Support Resources:**
- **GitHub API Documentation**: https://docs.github.com/en/rest/issues
- **PowerShell Documentation**: https://docs.microsoft.com/en-us/powershell/
- **VS Code GitHub Extension**: https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github

---

## 🎯 Recommended Workflow

### For Daily Use:
1. **GitHub CLI** - Most powerful and flexible
2. **VS Code Extension** - Integrated experience
3. **PowerShell Scripts** - Bulk operations

### For One-time Setup:
1. **REST Client** - Good for testing API calls
2. **curl** - Universal, works everywhere

---

## ⚡ Quick Start Commands

### GitHub CLI Quick Commands
```powershell
# Create issue with title and body
gh issue create --title "Issue Title" --body "Issue description"

# Create issue with template
gh issue create --template bug_report

# Create issue and assign to yourself
gh issue create --title "Title" --body "Body" --assignee @me

# Create issue with labels
gh issue create --title "Title" --body "Body" --label "bug,urgent"

# List all issues
gh issue list

# View issue details
gh issue view 1

# Open issue in browser
gh issue view 1 --web

# Close an issue
gh issue close 1
```

### VS Code Command Palette
- `Ctrl+Shift+P` → "GitHub Issues: Create Issue"
- `Ctrl+Shift+P` → "GitHub Issues: Create Issue from Selection"
- `Ctrl+Shift+P` → "GitHub Issues: Create Issue from Clipboard"

---

## 🏁 Verification Steps

### Test Your Setup:
1. **Create a test issue**:
   ```powershell
   gh issue create --title "✅ Setup Test" --body "Testing GitHub CLI setup"
   ```

2. **Verify in browser**:
   - Go to: https://github.com/mahedee/clean-arch-pro/issues
   - Check if your test issue appears

3. **Clean up**:
   ```powershell
   gh issue close 1  # Replace 1 with your test issue number
   ```

---

## 🎉 Success!

---

## ✅ Setup Verification & Testing

### **Complete Setup Verification Checklist**

Before creating your GitHub issues, verify your setup with this comprehensive checklist:

#### **1. Environment Verification**
```powershell
# ✅ Check PowerShell version (should be 5.1+ or 7+)
$PSVersionTable.PSVersion

# ✅ Check execution policy (should allow RemoteSigned)
Get-ExecutionPolicy

# ✅ Test GitHub connectivity
Test-NetConnection github.com -Port 443
Test-NetConnection api.github.com -Port 443

# ✅ Verify project structure
Get-ChildItem -Path "scripts" -Filter "*.ps1" | Select-Object Name
Get-ChildItem -Path "docs" -Recurse -Filter "*task*" | Select-Object Name
```

#### **2. GitHub Token Verification**
```powershell
# ✅ Test token authentication (replace with your token)
$token = "your_github_token_here"
$headers = @{ 'Authorization' = "token $token" }
Invoke-RestMethod -Uri "https://api.github.com/user" -Headers $headers

# ✅ Test repository access
Invoke-RestMethod -Uri "https://api.github.com/repos/mahedee/clean-arch-pro" -Headers $headers

# ✅ Check token scopes (should include 'repo')
$response = Invoke-WebRequest -Uri "https://api.github.com/user" -Headers $headers
$response.Headers['X-OAuth-Scopes']
```

#### **3. Script Files Verification**
```powershell
# ✅ Verify all required scripts exist
$requiredScripts = @(
    "scripts\create-github-issues.ps1",
    "scripts\setup-and-run.ps1"
)

foreach ($script in $requiredScripts) {
    if (Test-Path $script) {
        Write-Host "✅ Found: $script" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $script" -ForegroundColor Red
    }
}

# ✅ Verify documentation files
$requiredDocs = @(
    "docs\task-list.md",
    "docs\github-issues\completed-tasks-issues.md",
    "docs\setup\github-issues-setup-guide.md"
)

foreach ($doc in $requiredDocs) {
    if (Test-Path $doc) {
        Write-Host "✅ Found: $doc" -ForegroundColor Green
    } else {
        Write-Host "❌ Missing: $doc" -ForegroundColor Red
    }
}
```

#### **4. Test Issue Creation (Dry Run)**
```powershell
# ✅ Test with a single dummy issue first
$testIssue = @{
    title = "🧪 TEST - Setup Verification"
    body = "This is a test issue to verify the GitHub Issues setup is working correctly. **DELETE THIS ISSUE** after verification."
    labels = @("test", "setup-verification")
} | ConvertTo-Json

$headers = @{
    'Authorization' = "token your_github_token_here"
    'Accept' = 'application/vnd.github.v3+json'
    'Content-Type' = 'application/json'
}

try {
    $response = Invoke-RestMethod -Uri "https://api.github.com/repos/mahedee/clean-arch-pro/issues" -Method Post -Body $testIssue -Headers $headers
    Write-Host "✅ Test issue created successfully: $($response.html_url)" -ForegroundColor Green
    Write-Host "🗑️  Remember to delete this test issue: Issue #$($response.number)" -ForegroundColor Yellow
} catch {
    Write-Host "❌ Test issue creation failed: $($_.Exception.Message)" -ForegroundColor Red
}
```

### **🎯 Pre-Flight Checklist for EduTrack Issues**

Before running the main issue creation script, ensure:

- [ ] ✅ **GitHub Token**: Personal Access Token with `repo` scope created and tested
- [ ] ✅ **PowerShell**: Version 5.1+ or 7+, with RemoteSigned execution policy
- [ ] ✅ **Internet**: Stable connection to github.com and api.github.com
- [ ] ✅ **Scripts**: All PowerShell scripts present and accessible
- [ ] ✅ **Repository**: You have write access to `mahedee/clean-arch-pro`
- [ ] ✅ **Task List**: `docs/task-list.md` contains completed tasks to convert
- [ ] ✅ **Test**: Dry run test issue creation successful
- [ ] ✅ **Backup**: Current issue list backed up (if any existing issues)

### **🚀 Ready to Create Issues**

Once all verifications pass, choose your preferred method:

#### **Method 1: Automated Setup (Recommended)**
```powershell
.\scripts\setup-and-run.ps1 -Token "your_github_token_here"
```

#### **Method 2: Direct Script Execution**
```powershell
# Edit scripts\create-github-issues.ps1 first, then:
.\scripts\create-github-issues.ps1
```

#### **Method 3: Manual Creation**
Use the templates in `docs\github-issues\completed-tasks-issues.md`

---

## 🎉 Success Indicators

### **What Success Looks Like:**
1. **7 Issues Created**: All T001 subtasks documented as GitHub issues
2. **Professional Formatting**: Each issue includes metrics, technical details, and proper labels
3. **Proper Organization**: Issues are labeled and categorized correctly
4. **Security Maintained**: GitHub token removed from scripts after use
5. **Documentation Updated**: Task list reflects completed work

### **Expected Output:**
```
🚀 Creating GitHub Issues for EduTrack Completed Tasks
Repository: mahedee/clean-arch-pro

Creating issue: ✅ COMPLETED - Clean Architecture Solution Structure Setup
✅ Created issue #X: ✅ COMPLETED - Clean Architecture Solution Structure Setup
   URL: https://github.com/mahedee/clean-arch-pro/issues/X

[... 6 more issues ...]

📊 Results:
   Total Issues: 7
   Created Successfully: 7
   Failed: 0

🎉 All issues created successfully!
View them at: https://github.com/mahedee/clean-arch-pro/issues
```

### **Post-Creation Verification:**
1. **Visit Repository**: https://github.com/mahedee/clean-arch-pro/issues
2. **Check Issue Count**: Should see 7 new issues (or total expected)
3. **Verify Content**: Each issue should have detailed content, not just title
4. **Confirm Labels**: Issues should have appropriate labels (completed, architecture, etc.)
5. **Test Links**: All URLs in issues should work correctly

---

## 🎯 Integration with Development Workflow

### **Using GitHub Issues in VS Code**

#### **1. Install GitHub Extension**
```
Extension: GitHub Pull Requests and Issues
Publisher: GitHub
```

#### **2. View Issues in VS Code**
- Press `Ctrl+Shift+P`
- Type "GitHub Issues: Focus on Issues View"
- See all issues in the sidebar

#### **3. Reference Issues in Commits**
```bash
git commit -m "Complete Task T002-1 - Fixes #18"
# This automatically links the commit to issue #18
```

#### **4. Create New Issues from VS Code**
- Press `Ctrl+Shift+P`
- Type "GitHub Issues: Create Issue"
- Fill in the template

### **Task List Integration**

#### **Updating Task Status**
When you complete new tasks, update `docs/task-list.md`:
```markdown
- [x] ✅ **COMPLETE**: Your new completed task
```

#### **Creating Issues for New Tasks**
1. Update the task list with completed items
2. Modify `scripts/create-github-issues.ps1` to include new tasks
3. Run the script to create new issues

#### **Linking Issues to Pull Requests**
```markdown
## Related Issues
- Closes #18 (Architecture Setup)
- References #19 (Dependency Fixes)
```

---

## 📚 Additional Resources

### **EduTrack-Specific Documentation**
- **Task List**: `docs/task-list.md` - Master task tracking
- **Issue Templates**: `docs/github-issues/` - Ready-to-use issue content
- **Setup Guide**: `docs/setup/github-issues-setup-guide.md` - This document

### **GitHub Resources**
- **GitHub CLI Documentation**: https://cli.github.com/manual/
- **GitHub API Documentation**: https://docs.github.com/en/rest/issues
- **Personal Access Tokens**: https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token

### **VS Code Resources**
- **GitHub Extension**: https://marketplace.visualstudio.com/items?itemName=GitHub.vscode-pull-request-github
- **REST Client Extension**: https://marketplace.visualstudio.com/items?itemName=humao.rest-client
- **PowerShell Extension**: https://marketplace.visualstudio.com/items?itemName=ms-vscode.PowerShell

### **PowerShell Resources**
- **PowerShell Documentation**: https://docs.microsoft.com/en-us/powershell/
- **Execution Policies**: https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies

---

## 🎉 Conclusion

This guide provides everything you need to:
- ✅ **Setup** GitHub issue creation from VS Code
- ✅ **Automate** bulk issue creation from your task list
- ✅ **Troubleshoot** common problems
- ✅ **Integrate** with your development workflow
- ✅ **Maintain** security best practices

The EduTrack project now has professional issue tracking capabilities that scale with your development process. Each completed task can be automatically documented as a professional GitHub issue, creating an excellent project portfolio and development history.

**Ready to start?** Follow the Quick Start section at the top of this guide!
