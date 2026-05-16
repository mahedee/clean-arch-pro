# Setup Instructions for GitHub Issues Creation
# Follow these steps to create GitHub issues using the script

Write-Host "🚀 EduTrack - GitHub Issues Setup" -ForegroundColor Magenta
Write-Host "===================================" -ForegroundColor Magenta
Write-Host ""

Write-Host "📋 Step 1: Get your GitHub Personal Access Token" -ForegroundColor Yellow
Write-Host "1. Go to: https://github.com/settings/tokens" -ForegroundColor Cyan
Write-Host "2. Click 'Generate new token (classic)'" -ForegroundColor Cyan
Write-Host "3. Set expiration (recommended: 30 days)" -ForegroundColor Cyan
Write-Host "4. Select scopes: ✅ repo (Full control of private repositories)" -ForegroundColor Cyan
Write-Host "5. Click 'Generate token'" -ForegroundColor Cyan
Write-Host "6. Copy the token (it will only be shown once!)" -ForegroundColor Red
Write-Host ""

Write-Host "📋 Step 2: Save your token" -ForegroundColor Yellow
Write-Host "1. Open scripts/token.txt file" -ForegroundColor Cyan
Write-Host "2. Replace 'YOUR_GITHUB_TOKEN_HERE' with your actual token" -ForegroundColor Cyan
Write-Host "3. Save the file" -ForegroundColor Cyan
Write-Host ""

Write-Host "📋 Step 3: Run the issue creation script" -ForegroundColor Yellow
Write-Host "Run: .\scripts\create-github-issues.ps1" -ForegroundColor Cyan
Write-Host ""

Write-Host "🔒 Security Note:" -ForegroundColor Red
Write-Host "- The token.txt file is already added to .gitignore" -ForegroundColor White
Write-Host "- Your token will NOT be committed to the repository" -ForegroundColor White
Write-Host "- Keep your token secure and never share it publicly" -ForegroundColor White
Write-Host ""

Write-Host "✅ Ready! Your token is safely stored locally." -ForegroundColor Green
