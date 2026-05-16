# Quick Setup Script for GitHub Issues
# This helps you add your GitHub token and run the issues creator

param(
    [string]$Token
)

# Display instructions if no token provided
if ([string]::IsNullOrEmpty($Token)) {
    Write-Host ""
    Write-Host "🔑 GitHub Personal Access Token Required" -ForegroundColor Yellow
    Write-Host "=======================================" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "To create GitHub issues, you need a Personal Access Token." -ForegroundColor White
    Write-Host ""
    Write-Host "📋 Steps to get your token:" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Open: https://github.com/settings/tokens" -ForegroundColor White
    Write-Host "2. Click: 'Generate new token (classic)'" -ForegroundColor White
    Write-Host "3. Description: 'EduTrack Issues Creator'" -ForegroundColor White
    Write-Host "4. Expiration: 30 days" -ForegroundColor White
    Write-Host "5. Scope: ✅ repo (Full control of private repositories)" -ForegroundColor White
    Write-Host "6. Click: 'Generate token'" -ForegroundColor White
    Write-Host "7. Copy the token immediately!" -ForegroundColor Red
    Write-Host ""
    Write-Host "📝 Once you have the token, run this command:" -ForegroundColor Green
    Write-Host ""
    Write-Host '   .\scripts\setup-and-run.ps1 -Token "your_token_here"' -ForegroundColor Cyan
    Write-Host ""
    Write-Host "   OR edit the file manually:" -ForegroundColor Green
    Write-Host "   1. Open: scripts\create-github-issues.ps1" -ForegroundColor White
    Write-Host '   2. Replace: "YOUR_GITHUB_TOKEN_HERE" with your actual token' -ForegroundColor White
    Write-Host "   3. Run: .\scripts\create-github-issues.ps1" -ForegroundColor White
    Write-Host ""
    exit 0
}

# If token is provided, update the script and run it
Write-Host "🚀 Setting up GitHub Issues Creator..." -ForegroundColor Green
Write-Host ""

# Read the current script
$scriptPath = "scripts\create-github-issues.ps1"
$scriptContent = Get-Content $scriptPath -Raw

# Replace the placeholder with the actual token
$updatedContent = $scriptContent -replace 'YOUR_GITHUB_TOKEN_HERE', $Token

# Write the updated content back
Set-Content -Path $scriptPath -Value $updatedContent -Encoding UTF8

Write-Host "✅ Token added to script successfully!" -ForegroundColor Green
Write-Host ""

# Ask for confirmation before running
$confirm = Read-Host "Ready to create 7 GitHub issues for your completed tasks? (y/n)"

if ($confirm -eq 'y' -or $confirm -eq 'Y' -or $confirm -eq 'yes' -or $confirm -eq 'YES') {
    Write-Host ""
    Write-Host "🚀 Creating GitHub Issues..." -ForegroundColor Magenta
    Write-Host ""
    
    # Run the main script
    & $scriptPath
    
    Write-Host ""
    Write-Host "🔒 Cleaning up token for security..." -ForegroundColor Yellow
    
    # Remove the token from the script for security
    $cleanContent = $updatedContent -replace $Token, 'YOUR_GITHUB_TOKEN_HERE'
    Set-Content -Path $scriptPath -Value $cleanContent -Encoding UTF8
    
    Write-Host "✅ Token removed from script!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🎉 All done! Check your issues at:" -ForegroundColor Green
    Write-Host "🔗 https://github.com/mahedee/clean-arch-pro/issues" -ForegroundColor Cyan
    
} else {
    Write-Host ""
    Write-Host "❌ Cancelled. Your token is saved in the script." -ForegroundColor Yellow
    Write-Host "Run the script manually when ready: .\scripts\create-github-issues.ps1" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "🔒 Don't forget to remove your token from the script after use!" -ForegroundColor Red
}
