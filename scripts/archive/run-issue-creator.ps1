# Interactive GitHub Issues Creator
# This script will help you create all 7 GitHub issues for your completed tasks

Write-Host "🚀 EduTrack - GitHub Issues Creator" -ForegroundColor Magenta
Write-Host "====================================" -ForegroundColor Magenta
Write-Host ""

# Step 1: Check if user has GitHub token
Write-Host "📋 Step 1: GitHub Personal Access Token Setup" -ForegroundColor Yellow
Write-Host ""
Write-Host "To create GitHub issues, you need a Personal Access Token." -ForegroundColor White
Write-Host ""
Write-Host "🔗 Get your token here: https://github.com/settings/tokens" -ForegroundColor Cyan
Write-Host ""
Write-Host "Required settings:" -ForegroundColor Green
Write-Host "  ✅ Token type: Classic" -ForegroundColor White
Write-Host "  ✅ Expiration: 30 days" -ForegroundColor White
Write-Host "  ✅ Scope: repo (Full control of private repositories)" -ForegroundColor White
Write-Host ""

# Prompt for token
$githubToken = Read-Host "Enter your GitHub Personal Access Token (paste it here)"

if ([string]::IsNullOrEmpty($githubToken)) {
    Write-Host "❌ No token provided. Please get a token first." -ForegroundColor Red
    Write-Host "📖 See the setup guide: docs/setup/github-issues-setup-guide.md" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ Token received!" -ForegroundColor Green
Write-Host ""

# Step 2: Update the main script with the token
Write-Host "📋 Step 2: Updating the script with your token..." -ForegroundColor Yellow

$scriptPath = "scripts\create-github-issues.ps1"
$scriptContent = Get-Content $scriptPath -Raw

# Replace the placeholder token
$updatedContent = $scriptContent -replace 'YOUR_GITHUB_TOKEN_HERE', $githubToken

# Write back to script
Set-Content -Path $scriptPath -Value $updatedContent

Write-Host "✅ Script updated with your token!" -ForegroundColor Green
Write-Host ""

# Step 3: Run the script
Write-Host "📋 Step 3: Creating GitHub Issues..." -ForegroundColor Yellow
Write-Host "This will create 7 issues for all your completed tasks" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Ready to create the issues? (y/n)"

if ($confirm -eq 'y' -or $confirm -eq 'Y' -or $confirm -eq 'yes') {
    Write-Host ""
    Write-Host "🚀 Creating GitHub Issues..." -ForegroundColor Green
    Write-Host ""
    
    # Execute the main script
    & ".\scripts\create-github-issues.ps1"
    
    Write-Host ""
    Write-Host "🎉 Done! Check your GitHub repository:" -ForegroundColor Green
    Write-Host "🔗 https://github.com/mahedee/clean-arch-pro/issues" -ForegroundColor Cyan
    
} else {
    Write-Host "❌ Cancelled. Run this script again when ready." -ForegroundColor Yellow
}

# Step 4: Clean up (remove token from script for security)
Write-Host ""
Write-Host "🔒 Security: Removing token from script..." -ForegroundColor Yellow

$cleanContent = $scriptContent -replace $githubToken, 'YOUR_GITHUB_TOKEN_HERE'
Set-Content -Path $scriptPath -Value $cleanContent

Write-Host "✅ Token removed from script for security!" -ForegroundColor Green
Write-Host ""
Write-Host "📝 Note: Your token is now safely removed from the script file." -ForegroundColor Cyan
